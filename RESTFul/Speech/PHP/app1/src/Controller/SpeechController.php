<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/Speech/SpeechService.php';

class SpeechController extends APIController
{

    const RESULT_SPEECH_TO_TEXT = 0;
    
    const ERROR_SPEECH_TO_TEXT = 0;

    // values to load from config
    private $_audioFolder;
    private $_speechContexts;
    private $_xSpeechSubContext;

    public function handleSpeechToText()
    {
        if (!isset($_REQUEST['SpeechToText'])) {
            return;
        }

        try {
            $context = $_REQUEST['SpeechContext'];
            $_SESSION['SpeechContext'] = $context;

            $filename = $_REQUEST['audio_file'];
            $_SESSION['audio_file'] = $filename;

            $chunked = isset($_REQUEST['chkChunked']) ? 
                $_REQUEST['chkChunked'] : false;
            $_SESSION['chunked'] = $chunked;

            $flocation = $this->_audioFolder . '/' . $filename; 

            $subContext = null;
            if ($context == 'Gaming') {
                $subContext = $this->_xSpeechSubContext;
            }

            $srvc = new SpeechService($this->apiFQDN, $this->getFileToken());
            $response = $srvc->speechToText(
                $flocation, $context, $subContext, $this->_xArgs, $chunked
            );
            $this->results[SpeechController::RESULT_SPEECH_TO_TEXT] = $response;

        } catch (Exception $e) {
            $this->errors[SpeechController::ERROR_SPEECH_TO_TEXT] = $e->getMessage();
        }
    }
    /**
     * Creates a SpeechService object.
     */
    public function __construct() {
        parent::__construct();
        // Copy config values to member variables
        require __DIR__ . '/../../config.php';

        $this->_audioFolder = $audioFolder;
        $this->_speechContexts = $speech_context_config;
        $this->_xSpeechSubContext = $xSpeechSubContext;
        $this->_xArgs = $x_arg;
    }

    /**
    * Gets the list of audio files that are specified in config value 
    * 'audioFolder'.
    *
    * @return array list of files
    */
    public function getAudioFiles() {
        $allFiles = scandir($this->_audioFolder);
        $audioFiles = array();

        // copy all files except directories
        foreach ($allFiles as $fname) {
            if (!is_dir($fname)) {
                array_push($audioFiles, $fname);
            }
        }

        return $audioFiles;
    }

    /**
    * Gets whether the specified audio file was selected in a previous session.
    *
    * @return boolean true if previously selected, false otherwise
    */
    public function isAudioFileSelected($fname) {
    	return isset($_SESSION['audio_file']) 
                && strcmp($_SESSION['audio_file'], $fname) == 0;
    } 

    /**
    * Returns an array of speech contexts.
    *
    * @return array array of speech contexts
    */
    public function getSpeechContexts() {
        return $this->_speechContexts;
    }

    /**
    * Gets whether the specified context was selected in a previous session.
    *
    * @return boolean true if previously selected, false otherwise
    */
    public function isSpeechContextSelected($cname) {
        return isset($_SESSION['SpeechContext']) 
            && strcmp($_SESSION['SpeechContext'], $cname) == 0; 
    }

    /**
    * Gets whether chunked was selected in a previous session.
    */
    public function isChunkedSelected() {
        return isset($_SESSION['chunked']) && $_SESSION['chunked'];
    } 


    public function handleRequest() 
    {
        $this->handleSpeechToText();
    }
}
?>
