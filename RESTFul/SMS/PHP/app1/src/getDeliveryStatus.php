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
require_once __DIR__ . '/../lib/SMS/SMSService.php';

use Att\Api\Util\Util;
use Att\Api\SMS\SMSService;

$arr = null;
try {
    envinit();
    $smsService = new SMSService(getFqdn(), getFileToken());

    $msgId = $_POST['messageId'];
    $response = $smsService->getSMSDeliveryStatus($msgId);
    $resourceUrl = $response->getResourceUrl();
    $values = array();
    $dinfoList = $response->getDeliveryInfoList();
    foreach ($dinfoList as $dinfo) {
        $val = array(
            $dinfo->getId(), $dinfo->getAddress(), $dinfo->getDeliveryStatus()
        );
        $values[] = $val;
    }

    $arr = array(
        'success' => true,
        'tables' => array(array(
            'caption' => "Resource URL: $resourceUrl",
            'headers' => array('Message ID', 'Address', 'Delivery Status'),
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
