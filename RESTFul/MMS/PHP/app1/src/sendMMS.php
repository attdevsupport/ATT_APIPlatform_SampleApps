<?php
/*
 * Copyright 2015 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
session_start();

require_once __DIR__ . '/common.php';
require_once __DIR__ . '/../lib/Util/Util.php';
require_once __DIR__ . '/../lib/MMS/MMSService.php';

use Att\Api\Util\Util;
use Att\Api\MMS\MMSService;

$arr = null;
try {
    envinit();
    $mmsService = new MMSService(getFqdn(), getFileToken());

    $addr = Util::convertAddresses($_POST['address']);
    $addr = count($addr) == 1 ? $addr[0] : $addr;
    $subject = $_POST['sendMsgInput'];
    $attachment = $_POST['attachmentInput'];
    $notifyDeliveryStatus = isset($_POST['receiveStatus']);
    $allowedFiles = array('None', 'att.gif', 'coupon.jpg');
    if (!in_array($attachment, $allowedFiles, true)) {
        throw new Exception('Invalid attachment file specified');
    } 

    if ($attachment === 'None') {
        $attachment = array();
    } else {
        $attachment = array(__DIR__ . '/../attachment/' . $attachment);
    }

    $response = $mmsService->sendMMS(
        $addr, $attachment, $subject, null, $notifyDeliveryStatus
    );

    $resourceUrl = $response->getResourceUrl();
    $resourceUrl = $resourceUrl == null ? '-' : $resourceUrl;
    $arr = array(
        'success' => true,
        'tables' => array(array(
            'headers' => array('MessageId', 'ResourceURL'),
            'values' => array(
                array($response->getMessageId(), $resourceUrl)
            )),
        )
    );
} catch (Exception $e) {
    $arr = array(
        'success' => false,
        'text' => $e->getMessage()
    );
}

echo json_encode($arr);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
