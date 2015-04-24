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
    $msgId = $_POST['msgId'];

    $response = $mmsService->getMMSStatus($msgId);
    $deliveryInfoList = $response->getDeliveryInfoList();
    $values = array();
    foreach ($deliveryInfoList as $deliveryInfo) {
        $values[] = array(
            $deliveryInfo->getId(), $deliveryInfo->getAddress(), 
            $deliveryInfo->getDeliveryStatus()
        );
    }

    $arr = array(
        'success' => true,
        'text' => ('ResourceURL: ' . $response->getResourceUrl()),
        'tables' => array(array(
            'caption' => 'Status:',
            'headers' => array('MessageId', 'Address', 'DeliveryStatus'),
            'values' => $values,
        ))
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
