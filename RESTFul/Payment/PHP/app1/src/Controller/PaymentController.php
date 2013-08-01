<?php

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/Payment/PaymentFileHandler.php';
require_once __DIR__ . '/../../lib/Payment/PaymentService.php';
require_once __DIR__ . '/../../lib/Util/Util.php';

class PaymentController extends APIController {
    // TODO: Move string indices to constants

    private $_minTransValue;
    private $_maxTransValue;
    private $_minSubValue;
    private $_maxSubValue;
    private $_redirectURL;

    private $_arrIds;

    private $_fileHandler;

    private function getSubscriptionDetails($merchantSID, $consumerId) {
        $paymentReq = $this->createPaymentService();
        return $paymentReq->getSubscriptionDetails($merchantSID, $consumerId);
    }

    private function createPaymentService() {
        $srvc = new PaymentService($this->apiFQDN, $this->getFileToken());
        return $srvc;
    }

    private function getSubscriptionInfo($type, $value) {
        // TODO: Change subURL varname to a better name 
        $subURL = 'SubscriptionAuthCode';
        if (strcmp($type, '2') == 0) {
            $subURL = 'MerchantTransactionId';
        } else if (strcmp($type, '3') == 0) {
            $subURL = 'SubscriptionId';
        }

        $paymentReq = $this->createPaymentService();
        $info = $paymentReq->getSubscriptionStatus($subURL, $value);
        return $info;
    }

    private function getTransactionInfo($type, $value) {
        // TODO: Change subURL varname to a better name 
        $subURL = 'TransactionAuthCode';
        if (strcmp($type, '2') == 0) {
            $subURL = 'TransactionId';
        } else if (strcmp($type, '3') == 0) {
            $subURL = 'MerchantTransactionId';
        }

        $paymentReq = $this->createPaymentService(); 
        $info = $paymentReq->getTransactionStatus($subURL, $value);
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

        $request = new NotaryService($this->apiFQDN, $this->clientId, 
                $this->clientSecret);
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

        $request = new NotaryService($this->apiFQDN, $this->clientId, 
                $this->clientSecret);
        return $request->getTransactionNotary($transArgs);
    }

    private function handleNotary() {
        try {
            if (!isset($_REQUEST['signPayload'])) {
                return;
            }

            $payload = $_REQUEST['payload'];
            $req = new NotaryService($this->apiFQDN, $this->clientId, 
                    $this->clientSecret);
            $this->results['notary'] = $req->getNotary($payload);
        } catch (Exception $e) {
            $this->errors['notary'] = $e->getMessage();
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
            PaymentService::newSubscription($this->apiFQDN, $this->clientId, 
                    $notary); 
        } catch (Exception $e) {
            $this->errors['newSub'] = $e->getMessage();
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
            PaymentService::newTransaction($this->apiFQDN, $this->clientId, 
                    $notary); 
        } catch (Exception $e) {
            $this->errors['newTrans'] = $e->getMessage();
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
            $this->results['subInfo'] = $info;
            $this->_fileHandler->addSubscriptionInfo($subId, $merchantTransId, 
                    $authCode, $consumerId, $merchantSubId);
        } catch (Exception $e) {
            $this->errors['subInfo'] = $e->getMessage();
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
            $this->results['transInfo'] = $info;
            $this->_fileHandler->addTransactionInfo(
                    $transId, $merchantTransId, $authCode);
        } catch (Exception $e) {
            $this->errors['transAuthCode'] = $e->getMessage();
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
                $this->results['subDetails'] = $dinfo;
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
                $this->results['subDetails'] = $dinfo;
            }
        } catch (Exception $e) {
            $this->errors['subDetails'] = $e->getMessage();
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
                    $this->results['subInfo'] = $info;
                    return;
                }
            }
        } catch (Exception $e) {
            $this->errors['subInfo'] = $e->getMessage();
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
                    $this->results['transInfo'] = $info;
                    return;
                }
            }
        } catch (Exception $e) {
            $this->errors['transInfo'] = $e->getMessage();
        }
    }

    private function handleSubRefund() {
        try {
            if (!isset($_REQUEST['refundSubscriptionId'])) {
                return;
            }
            $sid = $_REQUEST['refundSubscriptionId'];

            $request = $this->createPaymentService(); 
            $rinfo = $request->refundSubscription($sid, 'Sample App Test');
            $this->results['s_refund'] = $rinfo;
        } catch (Exception $e) {
            $this->errors['s_refund'] = $e->getMessage();
        }
    }

    private function handleSubCancel() {
        try {
            if (!isset($_REQUEST['cancelSubscriptionId'])) {
                return;
            }
            $sid = $_REQUEST['cancelSubscriptionId'];

            $request = $this->createPaymentService(); 
            $rinfo = $request->cancelSubscription($sid, 'Sample App Test');

            $this->results['s_cancel'] = $rinfo;
        } catch (Exception $e) {
            $this->errors['s_cancel'] = $e->getMessage();
        }
    }

    private function handleTransRefund() {
        try {
            if (!isset($_REQUEST['refundTransactionId'])) {
                return;
            }
            $tid = $_REQUEST['refundTransactionId'];

            $request = $this->createPaymentService(); 
            $rinfo = $request->refundTransaction($tid, 'Sample App Test');
            $this->results['t_refund'] = $rinfo;
        } catch (Exception $e) {
            $this->errors['t_refund'] = $e->getMessage();
        }
    }

    public function __construct() {
        parent::__construct();

        require __DIR__ . '/../../config.php';
        $this->_minTransValue = $minTransactionValue;
        $this->_maxTransValue = $maxTransactionValue;
        $this->_minSubValue = $minSubscriptionValue;
        $this->_maxSubValue = $maxSubscriptionValue;
        $this->_redirectURL = $redirect_url;

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
            $this->results[$iName] = $fValues[$pVal];
        }
    }
                
    public function handleRequest() {
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
    
        // notification information
        $this->results['notifications'] 
            = FileUtil::loadArray('notifications.db');

        // notary
        if (isset($_SESSION['notary'])) {
            $this->results['notary'] = unserialize($_SESSION['notary']);
        }
    }

    public function showTransaction() {
        return isset($this->results['transInfo']) 
            || isset($this->errors['transInfo'])
            || isset($this->results['t_refund'])
            || isset($this->errors['t_refund']);
    }

    public function showSubscription() {
        return isset($this->results['subInfo']) 
            || isset($this->errors['subInfo'])
            || isset($this->results['subDetails'])
            || isset($this->errors['subDetails'])
            || isset($this->results['s_refund'])
            || isset($this->errors['s_refund'])
            || isset($this->results['s_cancel'])
            || isset($this->errors['s_cancel']);
    }

    public function showNotary() {
        return isset($this->results['notary']) 
            || isset($this->errors['notary']);
    }

    public function showNotifications() {
        return isset($_REQUEST['refreshNotifications']);
    }
      
}
?>
