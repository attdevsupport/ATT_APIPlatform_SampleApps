<?php

require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/SMS/SMSRequest.php';
require_once __DIR__ . '/../../lib/Util/Util.php';
require_once __DIR__ . '/../../lib/Util/FileUtil.php';

class SMSService {
    private $_FQDN;
    private $_clientId;
    private $_clientSecret;
    private $_getMsgsShortCode;
    private $_receiveMsgsShortCode;
    private $_oauthFile;
    private $_scope;

    private $_sendError;
    private $_deliveryStatusError;
    private $_getMessagesError;
    private $_results;

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
            $tokenRequest->setAcceptAllCerts(true);
            $token = $tokenRequest->getTokenUsingScope($this->_scope);

            // Save token for future use 
            $token->saveToken($this->_oauthFile);
        }

        return $token;
    }

    public function __construct() {
        require __DIR__ . '/../../config.php';

        $this->_FQDN = $FQDN;
        $this->_clientId = $api_key;
        $this->_clientSecret = $secret_key;
        $this->_receiveMsgsShortCode = $receiveMsgsShortCode;
        $this->_getMsgsShortCode = $getMsgsShortCode;
        $this->_oauthFile = $oauth_file;
        $this->_scope = $scope;

        $this->_sendError = NULL;
        $this->_deliveryStatusError = NULL;
        $this->_getMessagesError = NULL;
        $this->_results = array();
    }

    public function sendSMS() {
        if (!isset($_REQUEST['sendSMS'])) {
            return NULL;
        }

        try {
            $token = $this->getToken();
            $rawaddrs = $_REQUEST['address'];
            $_SESSION['rawaddrs'] = $rawaddrs;
            $addrArray = Util::convertAddresses($rawaddrs);
            $addr = count($addrArray) == 1 ? $addrArray[0] : $addrArray;
            $msg = $_REQUEST['message'];
            $url = $this->_FQDN . '/sms/v3/messaging/outbox';

            $request = new SMSRequest($url, $token);
            $request->setAcceptAllCerts(true);
            $getNotification = isset($_REQUEST['chkGetOnlineStatus']);
            $result = $request->sendSMS($addr, $msg, $getNotification);
            if (!$getNotification) {
                $_SESSION['SmsId'] = $result->outboundSMSResponse->messageId;
            }

            return $result;
        } catch (Exception $e) {
            $this->_sendError = $e->getMessage();
            return NULL;
        }
    }

    public function getSMSDeliveryStatus() {
        if (!isset($_REQUEST['getStatus'])) {
            return NULL;
        }

        try {
            $token = $this->getToken();
            $id = $_REQUEST['messageId'];
            $_SESSION['SmsId'] = $id;
            $url = $this->_FQDN . '/sms/v3/messaging/outbox/'
                . urlencode($id);  

            $request = new SMSRequest($url, $token);
            $request->setAcceptAllCerts(true);
            $result = $request->getSMSDeliveryStatus();

            return $result;
        } catch (Exception $e) {
            $this->_deliveryStatusError = $e->getMessage();
            return NULL;
        }
    }

    public function getMessages() {
        if (!isset($_REQUEST['getMessages'])) {
            return NULL;
        } 

        $shortCode = $this->_getMsgsShortCode;

        try {
            $token = $this->getToken();
            $url = $this->_FQDN . '/rest/sms/2/messaging/inbox?' 
                . '&RegistrationID=' . urlencode($shortCode);  

            $request = new SMSRequest($url, $token);
            $request->setAcceptAllCerts(true);
            $result = $request->getMessages();

            return $result;
        } catch (Exception $e) {
            $this->_getMessagesError = $e->getMessage();
            return NULL;
        }
    }

    public function getResults() {
        $pathS = __DIR__ . '/../../listener/status.db';
        $_results['resultStatusN'] = FileUtil::loadArray($pathS);

        $pathM = __DIR__ . '/../../listener/msgs.db';
        $_results['resultMsgs'] = FileUtil::loadArray($pathM);

        return $_results;
    }

    public function getSendError() {
        return $this->_sendError;
    }

    public function getDeliveryStatusError() {
        return $this->_deliveryStatusError;
    }

    public function getMessagesError() {
        return $this->_getMessagesError;
    }
}
?>
