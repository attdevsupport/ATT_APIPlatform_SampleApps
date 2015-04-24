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
require_once __DIR__ . '/../lib/DC/DCService.php';

use Att\Api\Util\Util;
use Att\Api\DC\DCService;

$arr = null;
try {
    envinit();
    $dcService = new DCService(getFqdn(), getFileToken());

    $response = $dcService->getDeviceInformation();
    $dcaps = $response->getCapabilities();
    $headers = array(
        'TypeAllocationCode',
        'Name',
        'Vendor',
        'Model',
        'FirmwareVersion',
        'UaProf',
        'MmsCapable',
        'AssistedGps',
        'LocationTechnology',
        'DeviceBrowser',
        'WapPushCapable'
    );
    $valuesEntry = array(
        $response->getTypeAllocationCode(),
        $dcaps->getName(),
        $dcaps->getVendor(),
        $dcaps->getModel(),
        $dcaps->getFirmwareVersion(),
        $dcaps->getUaProf(),
        $dcaps->isMmsCapable() ? 'Y' : 'N',
        $dcaps->isAssistedGps() ? 'Y' : 'N',
        $dcaps->getLocationTechnology(),
        $dcaps->getDeviceBrowser(),
        $dcaps->isWapPushCapable() ? 'Y' : 'N'
    );

    $arr = array(
        'success' => true,
        'tables' => array(
            array(
                'caption' => 'Device Capabilities',
                'headers' => $headers,
                'values' => array($valuesEntry)
            ),
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
