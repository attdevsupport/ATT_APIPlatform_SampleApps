<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '../../OAuth/OAuthToken.php';
require_once __DIR__ . '../../Common/RESTFulRequest.php';

class ADSRequest extends RESTFulRequest {
    private $_token;

    public function __construct($URL, OAuthToken $token) {
        parent::__construct($URL);
        $this->_token = $token; 
    }

    public function getAdvertisement($category) {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $this->addParam('Category', $category);

        // TODO: properly handle
        $this->setHeader('User-Agent', $_SERVER['HTTP_USER_AGENT']);
        $this->setHeader('UDID', md5("RANDOM TRUST ME"));
        $this->addAuthorizationHeader($this->_token);

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        $responseArr = json_decode($response, true);
        return $responseArr;
    }
}
?>
