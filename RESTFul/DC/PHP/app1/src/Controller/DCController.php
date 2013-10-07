<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/DC/DCService.php';

use Att\Api\Controller\APIController;
use Att\Api\DC\DCService;

class DCController extends APIController 
{
    const RESULT_DEVICE_INFO = 0;

    const ERROR_DEVICE_INFO = 0;

    public function __construct() 
    {
        parent::__construct(); 
    }

    public function handleGetDeviceInfo()
    {
        try {
            if (isset($_REQUEST['error'])) {
                throw new Exception('error=' . $_REQUEST['error'] . '&' 
                        . 'error_description=' . $_REQUEST['error_description']);
            }

            $srvc = new DCService($this->apiFQDN, $this->getSessionToken());
            $deviceInfo = $srvc->getDeviceInformation();
            $this->results[DCController::RESULT_DEVICE_INFO] = $deviceInfo;
        } catch (Exception $e) {
            $this->errors[DCController::ERROR_DEVICE_INFO] = $e->getMessage();
        }
    }

    public function handleRequest() 
    {
        $this->handleGetDeviceInfo();
    }
}
?>
