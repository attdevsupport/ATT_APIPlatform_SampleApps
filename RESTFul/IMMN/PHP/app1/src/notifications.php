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

require_once __DIR__ . '/../config.php';
require_once __DIR__ . '/../lib/Util/FileUtil.php';

use Att\Api\Util\FileUtil;

// TODO: set a limit on the number of notifications saved
// currently the number will keep growing without bound
$rawNotification = file_get_contents('php://input');
$notifications = json_decode($rawNotification, true);
if ($notifications == null) {
    return;
}
$arr = FileUtil::loadArray(NOTIFICATION_FILE);
$msgNotifications = null;
if (isset($notifications['notifications'])) {
    $msgNotifications = $notifications['notification'];
} else {
    $msgNotifications = $notifications['messageNotifications'];
}
if (isset($msgNotifications['subscriptions'])) {
    $arr[] = $msgNotifications['subscriptions'];
} else {
    $arr[] = $msgNotifications['subscriptionNotifications'];
}
FileUtil::saveArray($arr, NOTIFICATION_FILE);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
