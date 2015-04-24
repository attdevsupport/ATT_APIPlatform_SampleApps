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
$statusNotifications = array();

$fileArr = FileUtil::loadArray($statusFile);
foreach ($fileArr as $statusNotification) {
    $dInfoNotification = $statusNotification['deliveryInfoNotification'];
    $dInfo = $dInfoNotification['deliveryInfo'];
    $msgId = $dInfoNotification['messageId'];
    $addr = preg_replace('/\d\d\d($)/', '***${1}', $dInfo['address']);
    $deliveryStatus = $dInfo['deliveryStatus'];
    $statusNotifications[] = array($msgId, $addr, $deliveryStatus);
}

$mmsNotificationsArr = FileUtil::loadArray($imagesDbPath);
$mmsNotifications = array();
$nsize = count($mmsNotificationsArr);
for ($i = $nsize - $mmsLimit; $i < $nsize; ++$i) {
    if ($i < 0) {
        continue;
    }
    $mmsNotification = $mmsNotificationsArr[$i];
    if (isset($mmsNotification['text'])) {
        $fpath = __DIR__ . '/../' . $mmsNotification['text'];
        $mmsNotification['text'] = file_get_contents($fpath);
    } else {
        $mmsNotification['text'] = '-';
    }
    $mmsNotifications[] = $mmsNotification;
}

$arr = array(
    'statusNotifications' => $statusNotifications,
    'mmsNotifications' => $mmsNotifications,
);

echo json_encode($arr);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
