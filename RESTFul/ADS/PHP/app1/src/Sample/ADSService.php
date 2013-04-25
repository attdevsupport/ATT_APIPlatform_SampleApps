<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */


require_once __DIR__ . '/../../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthTokenRequest.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthCodeRequest.php';
require_once __DIR__ . '/../../lib/OAuth/OAuthCode.php';
require_once __DIR__ . '/../../lib/ADS/ADSRequest.php';

/**
 * Service class for handling the sample application's logic.
 *
 */
class ADSService {

    // values to load from config
    private $_FQDN;
    private $_clientId;
    private $_clientSecret;
    private $_scope;
    private $_oauthFile;

    private $_error;
    private $_result;

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

    private function saveInputsToSession() {
        $inputs = array('category', 'MMA', 'ageGroup', 'Premium', 'gender',
                'over18', 'zipCode', 'city', 'areaCode', 'country', 'latitude',
                'longitude', 'keywords');

        foreach ($inputs as $input) {
            if (isset($_REQUEST[$input])) {
                $_SESSION[$input] = $_REQUEST[$input];
            }
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

        $this->_error = NULL;
        $this->_result = NULL;
    }

    public function getAdvertisement() {
        if (!isset($_REQUEST['btnGetAds'])) {
            return;
        }

        try {
            // save user input to session 
            $this->saveInputsToSession();

            $category = $_REQUEST['category'];

            $token = $this->getToken();
            $endpoint = $this->_FQDN . '/rest/1/ads';
            $adsRequest = new ADSRequest($endpoint, $token);
            $adsRequest->setAcceptAllCerts(true);

            $inputs = array('MMA', 'ageGroup', 'Premium', 'gender',
                    'over18', 'zipCode', 'city', 'areaCode', 'country', 
                    'latitude', 'longitude', 'keywords');

            $keys = array('MMASize', 'AgeGroup', 'Premium', 'Gender', 'Over18',
                    'ZipCode', 'City', 'AreaCode', 'Country', 'Latitude', 
                    'Longitude', 'Keywords');

            $optVals = array();
            for ($i = 0; $i < count($inputs); ++$i) {
                $key = $keys[$i];
                $input = $inputs[$i];

                if (isset($_REQUEST[$input]) && $_REQUEST[$input] != "") {
                    $optVals[$key] = $_REQUEST[$input];
                }
            }

            $this->_result = $adsRequest->getAdvertisement($category, $optVals);
            if ($this->_result == NULL) {
                $this->_result = 'No Ads were returned';
            }
        } catch (Exception $e) {
            $this->_error = $e->getMessage();
            return NULL;
        }
    }

    public function getError() {
        return $this->_error;
    }

    public function getResult() {
        return $this->_result;
    }
}
?>
