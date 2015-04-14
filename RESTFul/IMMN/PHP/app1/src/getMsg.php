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
require_once __DIR__ . '/../lib/IMMN/IMMNService.php';
require_once __DIR__ . '/../lib/Util/Util.php';

use Att\Api\IMMN\IMMNService;
use Att\Api\Util\Util;

$arr = null;
try {
    envinit();
    $immnSrvc = new IMMNService(getFqdn(), getSessionToken());

    $msgId = $_POST['getMsgId'];

    $msg = $immnSrvc->getMessage($msgId);
    $msgValues = array(
        $msg->getMessageId(), $msg->getFrom(), $msg->getRecipients(),
        $msg->getText(), $msg->getTimeStamp(), $msg->isFavorite(),
        $msg->isUnread(), $msg->isIncoming(), $msg->getMessageType()
    );
    $msgValues = Util::convertNulls($msgValues);
    $arr = array(
        'success' => true,
        'tables' => array(
            array(
                'caption' => 'Message:',
                'headers' => array(
                    'message id', 'from', 'recipients', 'text', 'timestamp',
                    'isFavorite', 'isUnread', 'isIncoming', 'type',
                ),
                'values' => array($msgValues),
            ),
        ),
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
