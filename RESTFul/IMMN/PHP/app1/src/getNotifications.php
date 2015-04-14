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

require_once __DIR__ . '/../config.php';
require_once __DIR__ . '/../lib/Util/Util.php';
require_once __DIR__ . '/../lib/Util/FileUtil.php';

use Att\Api\Util\FileUtil;
use Att\Api\Util\Util;

if (isset($_SESSION['subscriptionExpiry'])) {
    $tnow = time();
    $expiry = $_SESSION['subscriptionExpiry'];
    if ($tnow >= $expiry) {
        unset($_SESSION['subscriptionId']);
    }
}
if (!isset($_SESSION['subscriptionId'])) {
    $arr = array('stopPolling' => true);
    echo json_encode($arr);
    return;
}
$subscriptionId = $_SESSION['subscriptionId'];
$arr = FileUtil::loadArray(NOTIFICATION_FILE);

$vals = array();
foreach ($arr as $msgNotifications) {
    foreach ($msgNotifications as $subscriptionNotifications) {
        $subId = $subscriptionNotifications['subscriptionId'];
        if ($subId != $subscriptionId) {
            continue;
        }
        $callbackData = $subscriptionNotifications['callbackData'];
        $notificationEvents = $subscriptionNotifications['notificationEvents'];
        foreach ($notificationEvents as $evt) {
            $vals[] = Util::convertNulls(array(
                $subId,
                $callbackData,
                $evt['messageId'],
                $evt['conversationThreadId'],
                $evt['eventType'],
                $evt['event'],
                $evt['text'],
                $evt['isTextTruncated'],
                $evt['isFavorite'],
                $evt['isUnread'],
            ));
        }
    }
}

echo json_encode($vals);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
