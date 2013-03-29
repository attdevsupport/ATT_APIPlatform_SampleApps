<?php

require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/Notary/NotaryRequest.php';
require_once __DIR__ . '/../../lib/Payment/PaymentRequest.php';
require_once __DIR__ . '/../../lib/Util/Util.php';
require_once __DIR__ . '/../../lib/Payment/PaymentFileHandler.php';

class PaymentService {
    // TODO: Move string indices to constants

    private $_FQDN;
    private $_clientId;
    private $_clientSecret;
    private $_oauthFile;
    private $_scope;
    private $_minTransValue;
    private $_maxTransValue;
    private $_minSubValue;
    private $_maxSubValue;
    private $_redirectURL;

    private $_arrIds;
    private $_errors;
    private $_results;

    private $_fileHandler;

    /**
     * Gets an access token. First attempts to load access token from file, and
     * if not successful, will send API request for access token.
     *
     * @return OAuthToken access token if successful.
     * @throws Exception if any underlying code throws exception
     */
    private function getToken() {
        // Try loading token from file
        $token = OAuthToken::loadToken($this->_oauthFile);

        // No token saved or token is expired... send token request
        if (!$token || $token->isAccessTokenExpired()) {
            $URL = $this->_FQDN . '/oauth/token';
            $id = $this->_clientId;
            $secret = $this->_clientSecret;
            $tokenRequest = new OAuthTokenRequest($URL, $id, $secret);
            $token = $tokenRequest->getTokenUsingScope($this->_scope);

            // Save token for future use 
            $token->saveToken($this->_oauthFile);
        }

        return $token;
    }

    private function getSubscriptionDetails($merchantSID, $consumerId) {
        $token = $this->getToken();

        $endpoint = $this->_FQDN . '/rest/3/Commerce/Payment/Subscriptions/'
            . $merchantSID . '/Detail/' . $consumerId;

        $paymentReq = new PaymentRequest($endpoint, $token);

        return $paymentReq->getSubscriptionDetails();
    }

    private function getSubscriptionInfo($type, $value) {
        $token = $this->getToken();
        $subURL = 'SubscriptionAuthCode';
        if (strcmp($type, '2') == 0) {
            $subURL = 'MerchantTransactionId';
        } else if (strcmp($type, '3') == 0) {
            $subURL = 'SubscriptionId';
        }

        $endpoint = $this->_FQDN . '/rest/3/Commerce/Payment/Subscriptions/' 
            . $subURL . '/' . $value;

        $paymentReq = new PaymentRequest($endpoint, $token);
        $info = $paymentReq->getSubscriptionStatus();
        return $info;
    }

    private function getTransactionInfo($type, $value) {
        $token = $this->getToken();
        $subURL = 'TransactionAuthCode';
        if (strcmp($type, '2') == 0) {
            $subURL = 'TransactionId';
        } else if (strcmp($type, '3') == 0) {
            $subURL = 'MerchantTransactionId';
        }

        $endpoint = $this->_FQDN . '/rest/3/Commerce/Payment/Transactions/' 
            . $subURL . '/' . $value;

        $paymentReq = new PaymentRequest($endpoint, $token);
        $info = $paymentReq->getTransactionStatus();
        return $info;
    }

    private function getSubscriptionNotary($price) {
        $subArgs = new SubscriptionNotaryArguments();
        $subArgs
            ->setAmount($price)
            ->setCategory(NotaryArguments::CATEGORY_APPLICATION_OTHER)
            ->setDescription('SAMPLE APP')
            ->setMerchantTransactionId('mtid' . time())
            ->setMerchantProductId('mpid' . time())
            ->setMerchantRedirectUrl($this->_redirectURL);

        $endpoint = $this->_FQDN . '/Security/Notary/Rest/1/SignedPayload';
        $request = new NotaryRequest($endpoint, $this->_clientId, 
                $this->_clientSecret);
        return $request->getSubscriptionNotary($subArgs);
    }

    private function getTransactionNotary($price) {
        $transArgs = new TransactionNotaryArguments();
        $transArgs
            ->setAmount($price)
            ->setCategory(NotaryArguments::CATEGORY_APPLICATION_OTHER)
            ->setDescription('SAMPLE APP')
            ->setMerchantTransactionId('mtid' . time())
            ->setMerchantProductId('mpid' . time())
            ->setMerchantRedirectUrl($this->_redirectURL);

        $endpoint = $this->_FQDN . '/Security/Notary/Rest/1/SignedPayload';
        $request = new NotaryRequest($endpoint, $this->_clientId, 
                $this->_clientSecret);
        return $request->getTransactionNotary($transArgs);
    }

    private function handleNotary() {
        try {
            if (!isset($_REQUEST['signPayload'])) {
                return;
            }

            $payload = $_REQUEST['payload'];
            $endpoint = $this->_FQDN . '/Security/Notary/Rest/1/SignedPayload';
            $req = new NotaryRequest($endpoint, $this->_clientId, 
                    $this->_clientSecret);
            $this->_results['notary'] = $req->getNotary($payload);
        } catch (Exception $e) {
            $this->_errors['notary'] = $e->getMessage();
        }
    }

    private function handleNewSubscription() {
        try {
            if (!isset($_REQUEST['newSubscription'])) {
                return;
            }

            $cost = $this->_minSubValue;
            if(strcmp($_REQUEST['product'], '2') == 0) {
                $cost = $this->_maxSubValue;
            }
            $notary = $this->getSubscriptionNotary($cost);
            $_SESSION['notary'] = serialize($notary);

            // redirect
            PaymentRequest::newSubscription($this->_FQDN, $this->_clientId, 
                    $notary); 
        } catch (Exception $e) {
            $this->_errors['newSub'] = $e->getMessage();
        }
    }
    private function handleNewTransaction() {
        try {
            if (!isset($_REQUEST['newTransaction'])) {
                return;
            }

            $cost = $this->_minTransValue;
            if(strcmp($_REQUEST['product'], '2') == 0) {
                $cost = $this->_maxTransValue;
            }
            $notary = $this->getTransactionNotary($cost);
            $_SESSION['notary'] = serialize($notary);

            // redirect
            PaymentRequest::newTransaction($this->_FQDN, $this->_clientId, 
                    $notary); 
        } catch (Exception $e) {
            $this->_errors['newTrans'] = $e->getMessage();
        }
    }

    private function handleSubscriptionAuthCode() {
        try {
            if (!isset($_REQUEST['SubscriptionAuthCode'])) {
                return;
            }

            $authCode = $_REQUEST['SubscriptionAuthCode'];
            $info = $this->getSubscriptionInfo('1', $authCode);
            $merchantTransId = $info['MerchantTransactionId'];
            $subId = $info['SubscriptionId'];
            $consumerId = $info['ConsumerId'];
            $merchantSubId = $info['MerchantSubscriptionId'];
            $this->_results['subInfo'] = $info;
            $this->_fileHandler->addSubscriptionInfo($subId, $merchantTransId, 
                    $authCode, $consumerId, $merchantSubId);
        } catch (Exception $e) {
            $this->_errors['subInfo'] = $e->getMessage();
        }
    }

    private function handleTransactionAuthCode() {
        try {
            if (!isset($_REQUEST['TransactionAuthCode'])) {
                return;
            }
            
            $authCode = $_REQUEST['TransactionAuthCode'];
            $info = $this->getTransactionInfo('1', $authCode);

            $merchantTransId = $info['MerchantTransactionId'];
            $transId = $info['TransactionId'];
            $this->_results['transInfo'] = $info;
            $this->_fileHandler->addTransactionInfo(
                    $transId, $merchantTransId, $authCode);
        } catch (Exception $e) {
            $this->_errors['transAuthCode'] = $e->getMessage();
        }
    }

    private function handleGetSubscriptionDetails() {
        try {
            if (isset($_REQUEST['getSDetailsMSID'])) {
                $fValues = $this->_fileHandler->getFileValues();
                $merchantSubIds 
                    = $fValues[PaymentFileHandler::INDEX_S_MERCHANT_SUB_ID];

                $merchantSubId = $_REQUEST['getSDetailsMSID'];
                $index = array_search($merchantSubId, $merchantSubIds);

                $consumerIds 
                    = $fValues[PaymentFileHandler::INDEX_S_CONSUMER_ID];
                $consumerId = $consumerIds[$index];

                $dinfo = $this->getSubscriptionDetails($merchantSubId, 
                        $consumerId);
                $this->_results['subDetails'] = $dinfo;
            } else if (isset($_REQUEST['getSDetailsConsumerId'])) {
                $fValues = $this->_fileHandler->getFileValues();
                $merchantSubIds 
                    = $fValues[PaymentFileHandler::INDEX_S_MERCHANT_SUB_ID];
                $consumerIds 
                    = $fValues[PaymentFileHandler::INDEX_S_CONSUMER_ID];

                $consumerId = $_REQUEST['getSDetailsConsumerId'];
                $index = array_search($consumerId, $consumerIds);

                $merchantSubId = $merchantSubIds[$index];

                $dinfo = $this->getSubscriptionDetails($merchantSubId,
                        $consumerId);
                $this->_results['subDetails'] = $dinfo;
            }
        } catch (Exception $e) {
            $this->_errors['subDetails'] = $e->getMessage();
        }
    }

    private function handleGetSubscriptionStatus() {
        try {
            $names = array('getSubscriptionMTID', 'getSubscriptionAuthCode', 
                    'getSubscriptionTID');
            $types = array('2', '1', '3');
            for ($i = 0; $i < count($names); ++$i) {
                $name = $names[$i];
                if (isset($_REQUEST[$name])) {
                    $value = $_REQUEST[$name];
                    $info = $this->getSubscriptionInfo($types[$i], $value);
                    $this->_results['subInfo'] = $info;
                    return;
                }
            }
        } catch (Exception $e) {
            $this->_errors['subInfo'] = $e->getMessage();
        }
    }

    private function handleGetTransactionStatus() {
        try {
            $names = array('getTransactionMTID', 'getTransactionAuthCode', 
                    'getTransactionTID');
            $types = array('3', '1', '2');
            for ($i = 0; $i < count($names); ++$i) {
                $name = $names[$i];
                if (isset($_REQUEST[$name])) {
                    $value = $_REQUEST[$name];
                    $info = $this->getTransactionInfo($types[$i], $value);
                    $this->_results['transInfo'] = $info;
                    return;
                }
            }
        } catch (Exception $e) {
            $this->_errors['transInfo'] = $e->getMessage();
        }
    }

    private function handleSubRefund() {
        try {
            if (!isset($_REQUEST['refundSubscriptionId'])) {
                return;
            }
            $sid = $_REQUEST['refundSubscriptionId'];
            $token = $this->getToken();
            $endpoint = $this->_FQDN . '/rest/3/Commerce/Payment/Transactions/'
                . $sid; 
            $request = new PaymentRequest($endpoint, $token);
            $rinfo = $request->refundSubscription('Sample App Test');
            $this->_results['s_refund'] = $rinfo;
        } catch (Exception $e) {
            $this->_errors['s_refund'] = $e->getMessage();
        }
    }

    private function handleSubCancel() {
        try {
            if (!isset($_REQUEST['cancelSubscriptionId'])) {
                return;
            }
            $sid = $_REQUEST['cancelSubscriptionId'];
            $token = $this->getToken();
            $endpoint = $this->_FQDN . '/rest/3/Commerce/Payment/Transactions/'
                . $sid; 
            $request = new PaymentRequest($endpoint, $token);
            $rinfo = $request->cancelSubscription('Sample App Test');
            $this->_results['s_cancel'] = $rinfo;
        } catch (Exception $e) {
            $this->_errors['s_cancel'] = $e->getMessage();
        }
    }

    private function handleTransRefund() {
        try {
            if (!isset($_REQUEST['refundTransactionId'])) {
                return;
            }
            $tid = $_REQUEST['refundTransactionId'];
            $token = $this->getToken();
            $endpoint = $this->_FQDN . '/rest/3/Commerce/Payment/Transactions/'
                . $tid; 
            $request = new PaymentRequest($endpoint, $token);
            $rinfo = $request->refundTransaction('Sample App Test');
            $this->_results['t_refund'] = $rinfo;
        } catch (Exception $e) {
            $this->_errors['t_refund'] = $e->getMessage();
        }
    }

    public function __construct() {
        require __DIR__ . '/../../config.php';
        $this->_FQDN = $FQDN;
        $this->_clientId = $api_key;
        $this->_clientSecret = $secret_key;
        $this->_oauthFile = $oauth_file;
        $this->_scope = $scope;
        $this->_minTransValue = $minTransactionValue;
        $this->_maxTransValue = $maxTransactionValue;
        $this->_minSubValue = $minSubscriptionValue;
        $this->_maxSubValue = $maxSubscriptionValue;
        $this->_redirectURL = $redirect_url;

        $this->_errors = array();
        $this->_results = array();

        $this->_fileHandler = new PaymentFileHandler('saves.db');
    }


    private function setFileResults() {
        $iNames = array('t_merchantTransIds', 't_authCodes', 't_transIds',
                's_merchantTransIds', 's_authCodes', 's_subIds', 
                's_consumerIds', 's_merchantSubIds');
        $pVals = array(
                PaymentFileHandler::INDEX_MERCHANT_TRANS_ID, 
                PaymentFileHandler::INDEX_AUTH_CODE, 
                PaymentFileHandler::INDEX_TRANS_ID, 
                PaymentFileHandler::INDEX_S_MERCHANT_TRANS_ID, 
                PaymentFileHandler::INDEX_S_AUTH_CODE, 
                PaymentFileHandler::INDEX_SUB_ID, 
                PaymentFileHandler::INDEX_S_CONSUMER_ID,
                PaymentFileHandler::INDEX_S_MERCHANT_SUB_ID
                );
        $fValues = $this->_fileHandler->getFileValues();
        for ($i = 0; $i < count($iNames); ++$i) {
            $iName = $iNames[$i];
            $pVal = $pVals[$i];
            $this->_results[$iName] = $fValues[$pVal];
        }
    }
                
    public function getResults() {
        // handle transactions
        $this->handleNewTransaction();
        $this->handleTransactionAuthCode();
        $this->handleGetTransactionStatus();
        $this->handleTransRefund();

        // handle subscriptions
        $this->handleNewSubscription();
        $this->handleSubscriptionAuthCode();
        $this->handleGetSubscriptionStatus();
        $this->handleGetSubscriptionDetails();
        $this->handleSubCancel();
        $this->handleSubRefund();

        // handle part notary
        $this->handleNotary();

        // load file contents
        $this->setFileResults();
    
        // refund information
        $this->_results['refundTrans'] = FileUtil::loadArray('refundtrans.db');
        $this->_results['refundSubs'] = FileUtil::loadArray('refundsubs.db');

        // notary
        if (isset($_SESSION['notary'])) {
            $this->_results['notary'] = unserialize($_SESSION['notary']);
        }

        return $this->_results;
    }

    public function getErrors() {
        return $this->_errors;
    }

    public function showTransaction() {
        return isset($this->_results['transInfo']) 
            || isset($this->_errors['transInfo'])
            || isset($this->_results['t_refund'])
            || isset($this->_errors['t_refund'])
            || isset($_REQUEST['refreshTransactionNotifications']);
    }

    public function showSubscription() {
        return isset($this->_results['subInfo']) 
            || isset($this->_errors['subInfo'])
            || isset($this->_results['subDetails'])
            || isset($this->_errors['subDetails'])
            || isset($this->_results['s_refund'])
            || isset($this->_errors['s_refund'])
            || isset($this->_results['s_cancel'])
            || isset($this->_errors['s_cancel'])
            || isset($_REQUEST['refreshSubscriptionNotifications']);
    }

    public function showNotary() {
        return isset($this->_results['notary']) 
            || isset($this->_errors['notary']);
    }
}
?>
