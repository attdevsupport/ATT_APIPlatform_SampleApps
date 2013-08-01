<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/MMS/MMSService.php';
require_once __DIR__ . '/../../lib/Util/FileUtil.php';

class MMSController extends APIController {
    private $_attachmentsFolder;

    private function handleSendMMS() {
        if (!isset($_REQUEST['address'])) {
            return;
        }

        try {
            $srvc = new MMSService($this->apiFQDN, $this->getFileToken());
            $rawAddr = $_REQUEST['address']; 
            $addr = Util::convertAddresses($rawAddr);
            $addr = count($addr) == 1 ? $addr[0] : $addr;
            $subject = $_REQUEST['subject'];
            $attachment = $_REQUEST['attachment'];
            $attachArr = array();
            if (strcmp($attachment, '') != 0) {
                $attachDir = 'attachment/' . $attachment;
                $attachArr = array($attachDir);
            }
            $notifyDeliveryStatus = isset($_REQUEST['chkGetOnlineStatus']);

            /* save input to session */
            $_SESSION['addr'] = $rawAddr;
            $_SESSION['subject'] = $subject;
            $_SESSION['attachment'] = $attachment;
            $_SESSION['notifyDeliveryStatus'] = $notifyDeliveryStatus;

            $response = $srvc->sendMMS(
                $addr, $attachArr, $subject, null, $notifyDeliveryStatus
            );
            $outboundResponse = $response['outboundMessageResponse'];
            $this->results['messageId'] = $outboundResponse['messageId'];

            if (!$notifyDeliveryStatus) { 
                $_SESSION['id'] = $outboundResponse['messageId'];
            }
            if (isset($outboundResponse['resourceReference'])) {
                $rRef = $outboundResponse['resourceReference'];
                $this->results['resourceURL'] = $rRef['resourceURL'];
            }

            $this->results['sendMMS'] = true;
        } catch (Exception $e) {
            $this->errors['sendMMS'] = $e->getMessage();
        }
    }

    private function handleGetStatus() {
        if (!isset($_REQUEST['mmsId'])) {
            return;
        }

        try {
            $mmsId = $_REQUEST['mmsId'];
            $srvc = new MMSService($this->apiFQDN, $this->getFileToken());
            $response = $srvc->getMMSStatus($mmsId);
            $this->results['getStatus'] = $response;
        } catch (Exception $e) {
            $this->errors['getStatus'] = $e->getMessage();
        }
    }

    public function __construct() {
        parent::__construct();

        // Copy config values to member variables
        require __DIR__ . '/../../config.php';

        $this->_attachmentsFolder = $attachments_dir;
    }

    public function handleRequest() {
        $this->handleSendMMS();
        $this->handleGetStatus();

        // notifications
        $pathS = __DIR__ . '/../../listener/status.db';
        $this->results['resultStatusN'] = FileUtil::loadArray($pathS);

        // attachments
        $fnames = FileUtil::getFiles($this->_attachmentsFolder);
        array_unshift($fnames, ""); // no attachment
        $this->results['fnames'] = $fnames;

        // images
        $path = __DIR__ . '/../../MMSImages/mmslistener.db';
        $this->results['messages'] = FileUtil::loadArray($path);

        if (isset($_SESSION['id'])) {
            $this->results['id'] = $_SESSION['id'];
        }
    }
}
?>
