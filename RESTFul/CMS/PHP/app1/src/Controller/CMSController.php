<?php /* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/CMS/CMSService.php';

use Att\Api\Controller\APIController;
use Att\Api\CMS\CMSService;

class CMSController extends APIController {
    const RESULT_CREATE_SESSION = 0;
    const RESULT_SEND_SIGNAL = 1;
    const RESULT_SCRIPT_CONTENTS = 2;
    const RESULT_SESSION_ID = 3;

    const ERROR_CREATE_SESSION = 0;
    const ERROR_SEND_SIGNAL = 1;

    private $_number;

    private function _getSessionId() 
    {
        return isset($_SESSION['sessionId']) ? $_SESSION['sessionId'] : '';
    }

    public function __construct() {
        parent::__construct();
        require __DIR__ . '/../../config.php';

        $this->_number = $number;
    }

    public function handleRequest() 
    {
        // set file contents
        $fname = __DIR__ . '/../../First.tphp';
        $contents = file_get_contents($fname);
        $this->results[CMSController::RESULT_SCRIPT_CONTENTS] = $contents;

        // set session id, if any
        $sid = $this->_getSessionId();
        $this->results[CMSController::RESULT_SESSION_ID] = $sid;

        $this->handleCreateSession();
        $this->handleSendSignal();

    } 

    public function handleCreateSession() 
    {
        if (!isset($_REQUEST['btnCreateSession'])) {
            return NULL;
        }

        try {
            /* save to session */
            $this->copyToSession(array(
                'txtNumberToDial', 'scriptType', 'txtNumber', 'txtMessageToPlay'
                ));

            $vals = array(
                'smsCallerId' => $this->_number,
                'feature' => $_REQUEST['scriptType'],
                'numberToDial' => $_REQUEST['txtNumberToDial'],
                'featurenumber' => $_REQUEST['txtNumber'],
                'messageToPlay' => $_REQUEST['txtMessageToPlay']
            );
                            
            $cmsSrvc = new CMSService($this->apiFQDN, $this->getFileToken());
            $result = $cmsSrvc->createSession($vals);

            // Save session id
            $sessionId = $result->getId();
            $this->results[CMSController::RESULT_SESSION_ID] = $sessionId;
            $_SESSION['sessionId'] = $sessionId;

            $this->results[CMSController::RESULT_CREATE_SESSION] = $result;
        } catch (Exception $e) {
            $msg = $e->getMessage();
            $this->errors[CMSController::ERROR_CREATE_SESSION] = $msg;
        }
    }

    public function handleSendSignal() 
    {
        if (!isset($_REQUEST['btnSendSignal'])) {
            return NULL;
        }

        try {
            $sessionId = $this->_getSessionId();
            if ($sessionId == NULL || $sessionId == '') {
                $error = "Session ID must not be empty.";
                throw new InvalidArgumentException($error);
            }
            $signal = $_REQUEST['signal'];
            
            /* save signal to session */
            $_SESSION['signal'] = $signal;

            $srvc = new CMSService($this->apiFQDN, $this->getFileToken());
            $result = $srvc->sendSignal($signal, $sessionId);
            $this->results[CMSController::RESULT_SEND_SIGNAL] = $result;
        } catch (Exception $e) {
            $this->errors[CMSController::ERROR_SEND_SIGNAL] = $e->getMessage();
        }
    }
}
?>
