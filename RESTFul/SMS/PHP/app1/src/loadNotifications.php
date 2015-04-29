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
require __DIR__ . '/../config.php';
require_once __DIR__ . '/../lib/Util/FileUtil.php';

use Att\Api\Util\FileUtil;

$msgs = array();
$msgsArr = FileUtil::loadArray(MSGS_FILE);
foreach ($msgsArr as $msgsArrEntry) {
    $msgs[] = array(
        $msgsArrEntry['MessageId'],
        $msgsArrEntry['DateTime'],
        $msgsArrEntry['SenderAddress'],
        $msgsArrEntry['DestinationAddress'],
        $msgsArrEntry['DestinationAddress'],
    );
}

$statuses = array();
$statusesArr = FileUtil::loadArray(STATUS_FILE);
foreach ($statusesArr as $statusesArrEntry) {
    $info = $statusesArrEntry['deliveryInfoNotification'];
    $dinfo = $info['deliveryInfo'];
    $statuses[] = array(
        $info['messageId'],
        $dinfo['address'],
        $dinfo['deliveryStatus'],
    );
}

echo json_encode(array(
    'messages' => $msgs,
    'deliveryStatus' => $statuses,
));

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
