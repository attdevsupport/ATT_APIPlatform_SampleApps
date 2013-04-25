<?php

require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/Util/Util.php';
require_once __DIR__ . '/../../lib/Util/FileUtil.php';
require_once __DIR__ . '/../../lib/MMS/MMSRequest.php';

class MMSService {
    private $_FQDN;
    private $_clientId;
    private $_clientSecret;
    private $_oauthFile;
    private $_scope;
    private $_attachmentsFolder;

    private $_errors;
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
            $token = $tokenRequest->getTokenUsingScope($this->_scope);

            // Save token for future use 
            $token->saveToken($this->_oauthFile);
        }

        return $token;
    }

    private function handleSendMMS() {
        if (!isset($_REQUEST['address'])) {
            return;
        }

        try {
            $token = $this->getToken();
            $endpoint = $this->_FQDN . '/mms/v3/messaging/outbox';
            $request = new MMSRequest($endpoint, $token); 
            $rawAddr = $_REQUEST['address']; 
            $addr = Util::convertAddresses($rawAddr);
            $addr = count($addr) == 1 ? $addr[0] : $addr;
            $subject = $_REQUEST['subject'];
            $attachment = $_REQUEST['attachment'];
            $attachArr = array();
            if (strcmp($attachment, '') != 0) {
                $attachDir = 'attachment/' . $attachment;
                $attachArr = array($attachDir);
            }
            $notifyDeliveryStatus = isset($_REQUEST['chkGetOnlineStatus']);

            /* save input to session */
            $_SESSION['addr'] = $rawAddr;
            $_SESSION['subject'] = $subject;
            $_SESSION['attachment'] = $attachment;
            $_SESSION['notifyDeliveryStatus'] = $notifyDeliveryStatus;

            $response = $request->sendMMS($addr, 
                    $attachArr, $subject, NULL, $notifyDeliveryStatus);
            $outboundResponse = $response['outboundMessageResponse'];
            $this->_results['messageId'] = $outboundResponse['messageId'];

            if (!$notifyDeliveryStatus) { 
                $_SESSION['id'] = $outboundResponse['messageId'];
            }
            if (isset($outboundResponse['resourceReference'])) {
                $rRef = $outboundResponse['resourceReference'];
                $this->_results['resourceURL'] = $rRef['resourceURL'];
            }

            $this->_results['sendMMS'] = true;
        } catch (Exception $e) {
            $this->_errors['sendMMS'] = $e->getMessage();
        }
    }

    private function handleGetStatus() {
        if (!isset($_REQUEST['mmsId'])) {
            return;
        }

        try {
            $token = $this->getToken();
            $endpoint = $this->_FQDN . '/mms/v3/messaging/outbox/'
                . $_REQUEST['mmsId'];
            $request = new MMSRequest($endpoint, $token); 

            $response = $request->getMMSStatus();
            $this->_results['getStatus'] = $response;
        } catch (Exception $e) {
            $this->_errors['getStatus'] = $e->getMessage();
        }
    }

    public function __construct() {
        // Copy config values to member variables
        require __DIR__ . '/../../config.php';

        $this->_FQDN = $FQDN;
        $this->_scope = $scope;
        $this->_clientId = $api_key;
        $this->_clientSecret = $secret_key;
        $this->_oauthFile = $oauth_file;
        $this->_attachmentsFolder = $attachments_dir;

        $this->_errors = array();
        $this->_results = array();
    }

    public function getResults() {
        $this->handleSendMMS();
        $this->handleGetStatus();

        // notifications
        $pathS = __DIR__ . '/../../listener/status.db';
        $this->_results['resultStatusN'] = FileUtil::loadArray($pathS);

        // attachments
        $fnames = Util::getFiles($this->_attachmentsFolder);
        array_unshift($fnames, ""); // no attachment
        $this->_results['fnames'] = $fnames;

        // images
        $path = __DIR__ . '/../../MMSImages/mmslistener.db';
        $this->_results['messages'] = FileUtil::loadArray($path);

        if (isset($_SESSION['id'])) {
            $this->_results['id'] = $_SESSION['id'];
        }
        return $this->_results;
    }

    public function getErrors() {
        return $this->_errors;
    }
}
?>
