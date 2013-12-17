<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/IMMN/IMMNService.php';

use Att\Api\Controller\APIController;
use Att\Api\IMMN\IMMNService;
use Att\Api\IMMN\IMMNDeltaChange;


/* Result and error constants */
define('C_ATTACHMENTS', 0);
define('C_SEND_MSG', 1);
define('C_CREATE_MSG_INDEX', 2);
define('C_GET_MSG_LIST', 3);
define('C_GET_MSG', 4);
define('C_GET_MSG_CONTENT', 5);
define('C_GET_DELTA', 6);
define('C_GET_MSG_INDEX_INFO', 7);
define('C_UPDATE_MSGS', 8);
define('C_DELETE_MSGS', 9);
define('C_NOTIFICATION_DETAILS', 10);

class IMMNController extends APIController
{

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

    public function __construct()
    {
        parent::__construct();
        // Copy config values to member variables
        require __DIR__ . '/../../config.php';
        $this->_attachmentsFolder = $attachments_folder;
    }

    public function handleRequest()
    {
        $attachments = $this->getAttachments();
        $this->results[C_ATTACHMENTS] = $attachments;

        $this->handleSendMessage();
        $this->handleCreateMsgIndex();
        $this->handleGetMsgLIst();
        $this->handleGetMsg();
        $this->handleGetMsgContent();
        $this->handleGetDelta();
        $this->handleGetMsgIndexInfo();
        $this->handleUpdateMsgs();
        $this->handleDeleteMsgs();
        $this->handleGetNotifDetails();
    }

    public function handleSendMessage()
    {
        $vnames = array('sendMessage', 'address', 'message', 'subject', 
                'attachment', 'groupCheckBox');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['sendMessage'])) {
            return;
        }

        try {
            if (!isset($_REQUEST['groupCheckBox'])) {
                unset($_SESSION['groupCheckBox']);
            }

            $addr = $this->convertAddresses($_SESSION['address']);
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

            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('sendMessage'));
            $id = $immnSrvc->sendMessage($addr, $msg, $subject, $attachment);
            $this->results[C_SEND_MSG] = $id;
        } catch (Exception $e) {
            $this->errors[C_SEND_MSG] = $e->getMessage();
            $this->clearSession(array('sendMessage'));
        }
    }

    public function handleCreateMsgIndex()
    {
        $vnames = array('createMessageIndex');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['createMessageIndex'])) {
            return;
        }


        try {
            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('createMessageIndex'));
            $immnSrvc->createMessageIndex();
            $this->results[C_CREATE_MSG_INDEX] = true;
        } catch (Exception $e) {
            $this->errors[C_CREATE_MSG_INDEX] = $e->getMessage();
            $this->clearSession(array('createMessageIndex'));
        }
        
    }

    public function handleGetMsgList()
    {
        $vnames = array('getMessageList', 'favorite', 'unread', 'incoming', 'keyword');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['getMessageList'])) {
            return;
        }

        try {
            /* unset checkboxes */
            if (!isset($_REQUEST['favorite'])) { 
                unset($_SESSION['favorite']);
            }
            if (!isset($_REQUEST['unread'])) { 
                unset($_SESSION['unread']);
            }
            if (!isset($_REQUEST['incoming'])) { 
                unset($_SESSION['incoming']);
            }

            $limit = 5; /* TODO: move to config */
            $offset = 0; /* TODO: move to config */
            $msgIds = null;
            $type = null;

            $fvt = isset($_SESSION['favorite']) ? $_SESSION['favorite'] : null;
            $unread = isset($_SESSION['unread']) ? $_SESSION['unread'] : null;
            $incoming = isset($_SESSION['incoming']) ? $_SESSION['incoming'] : null;
            $keyword = isset($_SESSION['keyword']) ? $_SESSION['keyword'] : null;

            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('getMessageList'));
            $msgList = $immnSrvc->getMessageList($limit, $offset, $msgIds,
                $unread, $type, $keyword, $incoming, $fvt);

            $this->results[C_GET_MSG_LIST] = $msgList;
        } catch (Exception $e) {
            $this->errors[C_GET_MSG_LIST] = $e->getMessage();
            $this->clearSession(array('getMessageList'));
        }
    }

    public function handleGetMsg()
    {
        $vnames = array('getMessage', 'messageId');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['getMessage'])) {
            return;
        }

        try {
            $msgId = $_SESSION['messageId'];

            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('getMessage'));
            $msg = $immnSrvc->getMessage($msgId);

            $this->results[C_GET_MSG] = $msg;
        } catch (Exception $e) {
            $this->errors[C_GET_MSG] = $e->getMessage();
            $this->clearSession(array('getMessage'));
        }
    }
    
    public function handleGetMsgContent()
    {
        $vnames = array('getMessageContent', 'messageId', 'partNumber');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['getMessageContent'])) {
            return;
        }

        try {
            $msgId = $_SESSION['messageId'];
            $pNumber = $_SESSION['partNumber'];

            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('getMessageContent'));
            $content = $immnSrvc->getMessageContent($msgId, $pNumber);

            $this->results[C_GET_MSG_CONTENT] = $content;
        } catch (Exception $e) {
            $this->errors[C_GET_MSG_CONTENT] = $e->getMessage();
            $this->clearSession(array('getMessageContent'));
        }
    }

    public function handleGetDelta()
    {
        $vnames = array('getDelta', 'state');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['getDelta'])) {
            return;
        }

        try {
            $state = $_SESSION['state'];

            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('getDelta'));
            $delta = $immnSrvc->getMessagesDelta($state);

            $this->results[C_GET_DELTA] = $delta;
        } catch (Exception $e) {
            $this->errors[C_GET_DELTA] = $e->getMessage();
            $this->clearSession(array('getDelta'));
        }
    }

    public function handleGetMsgIndexInfo()
    {
        $vnames = array('getMessageIndexInfo');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['getMessageIndexInfo'])) {
            return;
        }

        try {
            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('getMessageIndexInfo'));
            $indexInfo = $immnSrvc->getMessageIndexInfo();

            $this->results[C_GET_MSG_INDEX_INFO] = $indexInfo;
        } catch (Exception $e) {
            $this->errors[C_GET_MSG_INDEX_INFO] = $e->getMessage();
            $this->clearSession(array('getMessageIndexInfo'));
        }
    }

    public function handleUpdateMsgs()
    {
        $vnames = array('updateMessage', 'readflag', 'messageId');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['updateMessage'])) {
            return;
        }

        try {
            $isUnread = $_SESSION['readflag'] == 'unread' ? true : false;
            $msgIds = $_SESSION['messageId'];
            $msgIds = explode(',', $msgIds);

            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('updateMessage'));

            if (count($msgIds) > 1) { 
                $deltaChanges = array();
                foreach ($msgIds as $msgId) {
                    $deltaChanges[] = new IMMNDeltaChange($msgId, null, $isUnread);
                }
                $immnSrvc->updateMessages($deltaChanges);
            } else { /* one message id */
                $msgId = $msgIds[0];
                $immnSrvc->updateMessage($msgId, $isUnread);
            }

            $this->results[C_UPDATE_MSGS] = true;
        } catch (Exception $e) {
            $this->errors[C_UPDATE_MSGS] = $e->getMessage();
            $this->clearSession(array('updateMessage'));
        }
    }

    public function handleDeleteMsgs()
    {
        $vnames = array('deleteMessage', 'messageId');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['deleteMessage'])) {
            return;
        }

        try {
            $msgIds = $_SESSION['messageId'];
            $msgIds = explode(',', $msgIds);

            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('deleteMessage'));

            if (count($msgIds) > 1) { 
                $immnSrvc->deleteMessages($msgIds);
            } else { /* one message id */
                $msgId = $msgIds[0];
                $immnSrvc->deleteMessage($msgId);;
            }

            $this->results[C_DELETE_MSGS] = true;
        } catch (Exception $e) {
            $this->errors[C_DELETE_MSGS] = $e->getMessage();
            $this->clearSession(array('deleteMessage'));
        }
    }

    public function handleGetNotifDetails()
    {
        $vnames = array('getNotifyDetails', 'queues');
        $this->copyToSession($vnames);
        if (!isset($_SESSION['getNotifyDetails'])) {
            return;
        }

        try {
            if (!isset($_SESSION['queues'])) {
                throw new Exception('Invalid value for queues');
            }

            $queues = strtoupper($_SESSION['queues']);
            if ($queues != 'TEXT' && $queues != 'MMS') {
                throw new Exception('Invalid value for queues');
            }

            $immnSrvc = new IMMNService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(array('getNotifyDetails'));

            $cd = $immnSrvc->getNotificationConnectionDetails($queues);

            $this->results[C_NOTIFICATION_DETAILS] = $cd;
        } catch (Exception $e) {
            $this->errors[C_NOTIFICATION_DETAILS] = $e->getMessage();
            $this->clearSession(array('getNotifyDetails'));
        }
    }

    /**
     * Returns a list of attachments.
     */
    public function getAttachments() {
        $allFiles = scandir($this->_attachmentsFolder);
        $attachFiles = array();
        $attachFiles[] = 'None'; // first entry is no attachment

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
