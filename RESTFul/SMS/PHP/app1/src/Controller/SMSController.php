<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/SMS/SMSService.php';
require_once __DIR__ . '/../../lib/Util/Util.php';
require_once __DIR__ . '/../../lib/Util/FileUtil.php';

use Att\Api\Controller\APIController;
use Att\Api\SMS\SMSService;
use Att\Api\Util\FileUtil;
use Att\Api\Util\Util;

class SMSController extends APIController
{
    const RESULT_SEND_SMS = 0;
    const RESULT_SMS_DELIVERY = 1;
    const RESULT_GET_MSGS = 2;
    const RESULT_STATUS_ARR = 3;
    const RESULT_MSGS_ARR = 4;

    const ERROR_SEND_SMS = 0;
    const ERROR_SMS_DELIVERY = 1;
    const ERROR_GET_MSGS = 2;

    private $_getMsgsShortCode;
    private $_receiveMsgsShortCode;

    private function _handleGetResults() 
    {
        $pathS = __DIR__ . '/../../listener/status.db';
        $statusArr = FileUtil::loadArray($pathS);
        $this->results[SMSController::RESULT_STATUS_ARR] = $statusArr;

        $pathM = __DIR__ . '/../../listener/msgs.db';
        $msgsArr = FileUtil::loadArray($pathM);
        $this->results[SMSController::RESULT_MSGS_ARR] = $msgsArr;
    }

    private function _handleSendSMS() 
    {
        if (!isset($_REQUEST['sendSMS'])) {
            return;
        }

        try {
            $rawaddrs = $_REQUEST['address'];
            $_SESSION['rawaddrs'] = $rawaddrs;

            $addrArray = Util::convertAddresses($rawaddrs);
            $addr = count($addrArray) == 1 ? $addrArray[0] : $addrArray;
            $msg = $_REQUEST['message'];
            $getNotification = isset($_REQUEST['chkGetOnlineStatus']);
            $srvc = new SMSService($this->apiFQDN, $this->getFileToken());
            $result = $srvc->sendSMS($addr, $msg, $getNotification);

            if (!$getNotification) {
                $_SESSION['SmsId'] = $result->getMessageId();
            }

            $this->results[SMSController::RESULT_SEND_SMS] = $result;
        } catch (Exception $e) {
            $this->errors[SMSController::ERROR_SEND_SMS] = $e->getMessage();
        }
    }

    private function _handleGetSMSDeliveryStatus() 
    {
        if (!isset($_REQUEST['getStatus'])) {
            return;
        }

        try {
            $id = $_REQUEST['messageId'];
            $_SESSION['SmsId'] = $id;

            $srvc = new SMSService($this->apiFQDN, $this->getFileToken());
            $result = $srvc->getSMSDeliveryStatus($id);
            $this->results[SMSController::RESULT_SMS_DELIVERY] = $result;
        } catch (Exception $e) {
            $this->errors[SMSController::ERROR_SMS_DELIVERY] = $e->getMessage();
        }
    }

    private function _handleGetMessages()
    {
        if (!isset($_REQUEST['getMessages'])) {
            return;
        } 

        $shortCode = $this->_getMsgsShortCode;

        try {
            $srvc = new SMSService($this->apiFQDN, $this->getFileToken());
            $result = $srvc->getMessages($shortCode);
            $this->results[SMSController::RESULT_GET_MSGS] = $result;
        } catch (Exception $e) {
            $this->errors[SMSController::ERROR_GET_MSGS] = $e->getMessage();
        }
    }

    public function __construct() 
    {
        parent::__construct();

        require __DIR__ . '/../../config.php';

        $this->_receiveMsgsShortCode = $receiveMsgsShortCode;
        $this->_getMsgsShortCode = $getMsgsShortCode;
    }

    public function handleRequest() 
    {
        $this->_handleSendSMS();
        $this->_handleGetSMSDeliveryStatus();
        $this->_handleGetMessages();
        $this->_handleGetResults();
    }
}
?>
