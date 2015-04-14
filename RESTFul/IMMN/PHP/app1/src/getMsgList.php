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

    /* TODO: move to config */
    $limit = 5;
    $offset = 0;

    $msgIds = null;
    $type = null;


    $fvt = isset($_POST['favorite']) ? $_POST['favorite'] : null;
    $unread = isset($_POST['unread']) ? true : false;
    $incoming = isset($_POST['incoming']) ? true : false;
    $keyword = $_POST['keyword'];

    $msgList = $immnSrvc->getMessageList(
        $limit, $offset, $msgIds, $unread, $type, $keyword, $incoming, $fvt
    );

    $msgValues = array();
    $msgs = $msgList->getMessages();
    foreach ($msgs as $msg) {
        $msgValues[] = Util::convertNulls(array(
            $msg->getMessageId(), $msg->getFrom(), $msg->getRecipients(),
            $msg->getText(), $msg->getTimeStamp(), $msg->isFavorite(),
            $msg->isUnread(), $msg->isIncoming(), $msg->getMessageType()
        ));
    }

    $arr = array(
        'success' => true,
        'tables' => array(
            array(
                'caption' => 'Details:',
                'headers' => array(
                    'Limit', 'Offset', 'Total', 'Cache Status',
                    'Failed Messages', 'State',
                ),
                'values' => array(
                    array(
                        $msgList->getLimit(), $msgList->getOffset(),
                        $msgList->getTotal(), $msgList->getCacheStatus(),
                        $msgList->getFailedMessages(), $msgList->getState(),
                    )
                )
            ),
            array(
                'caption' => 'Messages:',
                'headers' => array(
                    'Message ID', 'From', 'Recipients', 'Text', 'Timestamp',
                    'Favorite', 'Unread', 'Incoming', 'Type',
                ),
                'values' => $msgValues,
            )
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
