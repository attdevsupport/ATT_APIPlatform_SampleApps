<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/NBest.php';
require_once __DIR__ . '/SpeechMultipart.php';
require_once __DIR__ . '/SpeechResponse.php';
require_once __DIR__ . '/../Common/RESTFulRequest.php';
require_once __DIR__ . '/../OAuth/OAuthTokenRequest.php';

class SpeechRequest extends RESTFulRequest {
    private $_token;

    /** 
     * Builds a speech response using the specified JSON object and returns
     * the result as a SpeechResponse object.
     *
     * @param JSONObject json object
     * @return SpeechResponse JSON Object converted to a SpeechResponse Object
     */
    private function buildSpeechResponse($jsonObj) {
        $responseId = $jsonObj->ResponseId;
        $status = $jsonObj->Status;
        $jsonNBest = $jsonObj->NBest[0];

        $nBest = NULL;
        if (strcmp($status, "OK") == 0) {
            $nBest = new NBest(
                    $jsonNBest->Hypothesis,
                    $jsonNBest->LanguageId,
                    $jsonNBest->Confidence,
                    $jsonNBest->Grade,
                    $jsonNBest->ResultText,
                    $jsonNBest->Words,
                    $jsonNBest->WordScores);
        }
    
        $sResponse = new SpeechResponse($responseId, $status, $nBest);
        return $sResponse;
    }

    public function __construct($URL, OAuthToken $token) {
        parent::__construct($URL);
        $this->_token = $token;
    }

    public function sendSpeechRequest($cntxt, $fname, $gfname = NULL, 
            $dfname = NULL, $xArg = NULL) {

        $this->setHeader('Accept', 'application/json');
        $this->addAuthorizationHeader($this->_token);
        $mpart = new SpeechMultipart();
        $this->setHeader('Content-Type', $mpart->getContentType());
        if ($dfname != NULL) {
            $mpart->addXDictionaryPart($dfname);
        }
        if ($gfname != NULL) {
            $mpart->addXGrammarPart($gfname);
        }
        if ($xArg != NULL) {
            $this->setHeader('X-Arg', $xArg);
        }
        $mpart->addFilePart($fname);
        $this->setMultipart($mpart);

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($response);
        }
        $responseArr = json_decode($response, true);
        return $responseArr;
    }
}
?>
