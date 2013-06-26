<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */


require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/IMMN/IMMNService.php';

class IMMNController extends APIController {

    const RESULT_SEND_MSG = 0;
    const RESULT_GET_HEADERS = 1;
    const RESULT_GET_BODY = 2;

    const ERROR_SEND_MSG = 0;
    const ERROR_GET_HEADERS = 1;
    const ERROR_GET_BODY = 2;

    // values to load from config
    private $_attachmentsFolder;

    private function convertAddresses($addrStr) {
        // TODO: Move to common
        $addresses = explode(',', $addrStr);
        $encodedAddr = array(); 
        foreach ($addresses as $addr) {
            $cleanAddr = str_replace('-', '', $addr);
            $cleanAddr = str_replace('tel:', '', $cleanAddr);
            $cleanAddr = str_replace('+1', '', $cleanAddr);
            if (preg_match("/\d{10}/",$cleanAddr)) {
                $encodedAddr[] = 'tel:' . $cleanAddr;
            } else if (preg_match("/^[^@]*@[^@]*\.[^@]*$/", $cleanAddr)) {
                $encodedAddr[] = $cleanAddr;
            } else if (preg_match("/\d[3-8]/", $cleanAddr)){
                $encodedAddr[] = 'short:' . $cleanAddr;
            } else {
                throw new InvalidArgumentException('Invalid address: ' . $addr);
            }
        }

        return $encodedAddr;
    }

    public function __construct() {
        parent::__construct();
        // Copy config values to member variables
        require __DIR__ . '/../../config.php';
        $this->_attachmentsFolder = $attachments_folder;
    }

    public function handleRequest() {

        $this->handleSendMessage();
        $this->handleGetMessageBody();
        $this->handleGetMessageHeaders();

    }

    public function handleSendMessage() {
        $vnames = array('sendMessage', 'Address', 'message', 'subject', 
                'attachment', 'groupCheckBox');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['sendMessage'])) {
            return;
        }

        try {
            $addr = $this->convertAddresses($_SESSION['Address']);
            $msg = $_SESSION['message'];
            $subject = $_SESSION['subject'];
            $attachment = $_SESSION['attachment'];
            if (strcmp($attachment, 'None') == 0) {
                $attachment = null;
            } else {
                $attachment = 
                    array($this->_attachmentsFolder . '/' .  $attachment);
            }

            $checkbox = isset($_REQUEST['groupCheckBox']);
            $_SESSION['checkbox'] = $checkbox;

            $immnSrvc = new IMMNService($this->FQDN, $this->getSessionToken());
            $this->clearSession(array('sendMessage'));
            $id = $immnSrvc->sendMessage($addr, $msg, $subject, $attachment);
            $this->results[IMMNController::RESULT_SEND_MSG] = $id;
        } catch (Exception $e) {
            $this->errors[IMMNController::ERROR_SEND_MSG] = $e->getMessage();
            $this->clearSession(array('sendMessage'));
        }
    }

    public function handleGetMessageBody() {
        $vnames = array ('getMessageContent', 'MessageId', 'PartNumber');
        $this->copyToSession($vnames);

        if (!isset($_SESSION['getMessageContent'])) {
            return;
        }

        $msgId = $_SESSION['MessageId'];
        $partNum = $_SESSION['PartNumber'];

        try {
            $immnSrvc = new IMMNService($this->FQDN, $this->getSessionToken());
            $this->clearSession(array('getMessageContent'));
            $result = $immnSrvc->getMessageBody($msgId, $partNum); 
            $this->results[IMMNController::RESULT_GET_BODY] = $result;
        } catch (Exception $e) {
            $this->clearSession(array('getMessageContent'));
            $this->errors[IMMNController::ERROR_GET_BODY] = $e->getMessage();
        }
    }

    public function handleGetMessageHeaders() 
    {
        $vnames = array('getMessageHeaders', 'headerCountTextBox',
                'indexCursorTextBox');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['getMessageHeaders'])) {
            return;
        }

        $headerCount = intval($_SESSION['headerCountTextBox']);
        $indexCursor = $_SESSION['indexCursorTextBox'];

        try {
            $immnSrvc = new IMMNService($this->FQDN, $this->getSessionToken());
            $this->clearSession(array('getMessageHeaders'));
            $result = $immnSrvc->getMessageHeaders($headerCount, $indexCursor);
            $this->results[IMMNController::RESULT_GET_HEADERS] = $result;
        } catch (Exception $e) {
            $this->clearSession(array('getMessageHeaders'));
            $this->errors[IMMNController::ERROR_GET_HEADERS] = $e->getMessage();
        }
    }

    /**
     * Returns a list of attachments.
     */
    public function getAttachments() {
        $allFiles = scandir($this->_attachmentsFolder);
        $attachFiles = array();

        // copy all files except directories
        foreach ($allFiles as $fname) {
            if (!is_dir($fname)) {
                $attachFiles[] = $fname;
            }
        }

        return $attachFiles;
    }
}
?>
