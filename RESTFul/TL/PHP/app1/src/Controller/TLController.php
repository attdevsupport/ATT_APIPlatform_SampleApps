<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * TL Sample Application 
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2013. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category TL Sample Application 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../../lib/TL/TLService.php';
require_once __DIR__ . '/../../lib/Controller/APIController.php';

class TLController extends APIController {
    const RESULT_LOCATION = 0;

    const ERROR_LOCATION = 0;

    public function __construct() 
    {
    	parent::__construct();
    }

    public function handleRequest() 
    {
        $vnames = array('getLocation', 'acceptableAccuracy',
                'requestedAccuracy', 'tolerance');
        $this->copyToSession($vnames);

        /* default values */
        if(!isset($_SESSION['requestedAccuracy'])) {
            $_SESSION['requestedAccuracy'] = 1000;
        }
        if (!isset($_SESSION['acceptableAccuracy'])) {
            $_SESSION['acceptableAccuracy'] = 10000;
        }
        if (!isset($_SESSION['tolerance'])) {
            $_SESSION['tolerance'] = 'LowDelay';
        }

        if (!isset($_SESSION['getLocation'])) {
            return;
        }

        try {
            $acceptableAccuracy = $_SESSION['acceptableAccuracy'];
            $requestedAccuracy = $_SESSION['requestedAccuracy'];
            $tolerance = $_SESSION['tolerance'];

            $tlSrvc = new TLService($this->apiFQDN, $this->getSessionToken());
            $this->clearSession(Array('getLocation'));
            $location = $tlSrvc->getLocation(
                $requestedAccuracy, $acceptableAccuracy, $tolerance
            );
            $this->results[TLController::RESULT_LOCATION] = $location;
        } catch (Exception $e) {
            $this->clearSession(Array('getLocation'));
            $this->errors[TLController::ERROR_LOCATION] = $e->getMessage();
        }
    }
}
?>
