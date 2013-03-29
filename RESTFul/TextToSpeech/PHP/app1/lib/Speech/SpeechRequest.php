<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../Common/RESTFulRequest.php';
require_once __DIR__ . '/../OAuth/OAuthTokenRequest.php';

class SpeechRequest extends RESTFulRequest {
    private $_token;

    public function __construct($URL, OAuthToken $token) {
        parent::__construct($URL);
        $this->_token = $token;
    }

    public function speechToText($ctype, $txt, $xArg = NULL) {
    	$this->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $this->setHeader('Accept', 'audio/x-wav');
        $this->addAuthorizationHeader($this->_token);
        $this->setHeader('Content-Type', $ctype);
        $this->setBody($txt);

        if ($xArg != NULL) {
            $this->setHeader('X-Arg', $xArg);
        }

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($response);
        }
        return $response;
    }
}
?>
