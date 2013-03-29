<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '../../Common/RESTFulRequest.php';


class MMSRequest extends RESTFulRequest {
    private $_token;

    public function __construct($url, OAuthToken $token) {
        parent::__construct($url);
        $this->_token = $token;
    }

    public function sendMMS($addr, $fnames, $subject = NULL, 
            $priority = NULL, $notifyDeliveryStatus = false) {

        $this->setHttpMethod(RESTFULRequest::HTTP_METHOD_POST);
        $this->setHeader('Content-Type', 'application/json');
        $this->setHeader('Accept', 'application/json');
        $this->addAuthorizationHeader($this->_token);

        $outboundRequest = array(
                'address' => $addr,
                'notifyDeliveryStatus' => $notifyDeliveryStatus
                );

        if ($subject != NULL) {
            $outboundRequest['subject'] = $subject;
        }
        if ($priority != NULL) {
            $outboundRequest['priority'] = $priority;
        }

        $vals = array('outboundMessageRequest' => $outboundRequest);
        $jvals = json_encode($vals);

        $mpart = new Multipart();
        $mpart->addJSONPart($jvals);
        foreach ($fnames as $fname) {
            $mpart->addFilePart($fname);
        }
        $this->setMultipart($mpart);

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($response);
        }
        return json_decode($response, true);
    }

    public function getMMSStatus() {
        $this->setHttpMethod(RESTFULRequest::HTTP_METHOD_GET);
        $this->setHeader('Accept', 'application/json');
        $this->addAuthorizationHeader($this->_token);

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        return json_decode($response, true);
    }
}

?>
