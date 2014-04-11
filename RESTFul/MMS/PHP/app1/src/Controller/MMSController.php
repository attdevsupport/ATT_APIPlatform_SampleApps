<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/MMS/MMSService.php';
require_once __DIR__ . '/../../lib/Util/FileUtil.php';

use Att\Api\Controller\APIController;
use Att\Api\MMS\MMSService;
use Att\Api\Util\FileUtil;
use Att\Api\Util\Util;

class MMSController extends APIController
{
    const RESULT_SEND_MMS = 0;
    const RESULT_FNAMES = 1;
    const RESULT_STATUS_DB = 2;
    const RESULT_MSGS_DB = 3;
    const RESULT_MSG_ID = 4;
    const RESULT_GET_STATUS = 5;

    const ERROR_SEND_MMS = 0;
    const ERROR_GET_STATUS = 1;

    private $_attachmentsFolder;

    private function handleSendMMS()
    {
        if (!isset($_REQUEST['address'])) {
            return;
        }

        try {
            $this->copyToSession(array(
                'address', 'subject', 'attachment', 'chkGetOnlineStatus'
            ));

            $addr = Util::convertAddresses($_REQUEST['address']);
            $addr = count($addr) == 1 ? $addr[0] : $addr;
            $subject = $_REQUEST['subject'];
            $attachment = $_REQUEST['attachment'];

            $attachArr = array();
            if (strcmp($attachment, '') != 0)
                $attachArr = array('attachment/' . $attachment);

            $notifyDeliveryStatus = isset($_REQUEST['chkGetOnlineStatus']);

            $srvc = new MMSService($this->apiFQDN, $this->getFileToken());
            $response = $srvc->sendMMS(
                $addr, $attachArr, $subject, null, $notifyDeliveryStatus
            );

            if (!$notifyDeliveryStatus)
                $_SESSION['id'] = $response->getMessageId();

            $this->results[MMSController::RESULT_SEND_MMS] = $response;
        } catch (Exception $e) {
            $this->errors[MMSController::ERROR_SEND_MMS] = $e->getMessage();
        }
    }

    private function handleGetStatus()
    {
        if (!isset($_REQUEST['mmsId'])) {
            return;
        }

        try {
            $mmsId = $_REQUEST['mmsId'];
            $srvc = new MMSService($this->apiFQDN, $this->getFileToken());
            $response = $srvc->getMMSStatus($mmsId);
            $this->results[MMSController::RESULT_GET_STATUS] = $response;
        } catch (Exception $e) {
            $this->errors[MMSController::ERROR_GET_STATUS] = $e->getMessage();
        }
    }

    public function __construct()
    {
        parent::__construct();

        // Copy config values to member variables
        require __DIR__ . '/../../config.php';

        $this->_attachmentsFolder = $attachments_dir;
    }

    public function handleRequest()
    {
        $this->handleSendMMS();
        $this->handleGetStatus();

        // notifications
        $pathS = __DIR__ . '/../../listener/status.db';
        $this->results[MMSController::RESULT_STATUS_DB] 
            = FileUtil::loadArray($pathS);

        // attachments
        $fnames = FileUtil::getFiles($this->_attachmentsFolder);
        array_unshift($fnames, ""); // no attachment
        $this->results[MMSController::RESULT_FNAMES] = $fnames;

        // images
        $path = __DIR__ . '/../../MMSImages/mmslistener.db';
        $this->results[MMSController::RESULT_MSGS_DB]
            = FileUtil::loadArray($path);

        if (isset($_SESSION['id'])) {
            $this->results[MMSController::RESULT_MSG_ID] = $_SESSION['id'];
        }
    }
}
?>
