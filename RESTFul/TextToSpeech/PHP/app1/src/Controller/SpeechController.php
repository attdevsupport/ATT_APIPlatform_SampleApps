<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/Speech/SpeechService.php';

class SpeechController extends APIController
{
    const RESULT_TTS = 0;

    const ERROR_TTS = 1;

    // values to load from config
    private $_xArgs;

    private function _handleTextToSpeech()
    {
        if (!isset($_REQUEST['TextToSpeechButton'])) {
            return;
        }

        try {
            $ctype = $_REQUEST['ContentType'];

            $txt = null;
            if (strcmp($ctype, 'text/plain') == 0) {
                $txt = file_get_contents(__DIR__ . '/../../text/PlainText.txt'); 
            } else {
                $txt = file_get_contents(__DIR__ . '/../../text/SSMLWithPhoneme.txt');
            }

            $srvc = new SpeechService($this->apiFQDN, $this->getFileToken());
            $response = $srvc->textToSpeech($ctype, $txt, $this->_xArgs);
            $this->results[SpeechController::RESULT_TTS] = $response;

        } catch (Exception $e) {
            $this->errors[SpeechController::ERROR_TTS] = $e->getMessage();
        }
    }

    /**
     * Creates a SpeechService object.
     */
    public function __construct() {
        parent::__construct();
        // Copy config values to member variables
        require __DIR__ . '/../../config.php';

        $this->_xArgs = $x_arg;
    }

    public function handleRequest()
    {
        $this->_handleTextToSpeech();
    }
}
?>
