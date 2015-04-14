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
require_once __DIR__ . '/../lib/Webhooks/UpdateSubscriptionArgs.php';
require_once __DIR__ . '/../lib/Webhooks/WebhooksService.php';

use Att\Api\Webhooks\UpdateSubscriptionArgs;
use Att\Api\Webhooks\WebhooksService;

$arr = null;
try {
    if (!isset($_SESSION['subscriptionId'])) {
        throw new Exception('You must first create a subscription.');
    }
    $events = array();
    if (isset($_REQUEST['updateSubscriptionText'])) {
        $events[] = 'TEXT';
    }
    if (isset($_REQUEST['updateSubscriptionMms'])) {
        $events[] = 'MMS';
    }
    if (count($events) == 0) {
        throw new Exception("You must select at least one of Text or MMS");
    }
    $subscriptionId = $_SESSION['subscriptionId'];

    envinit();
    $webhooksSrvc = new WebhooksService(getFqdn(), getSessionToken());

    $callbackData = $_REQUEST['updateCallbackData'];
    if ($callbackData == '') {
        $callbackData = null;
    }
    $args = new UpdateSubscriptionArgs(
        CHANNEL_ID, $subscriptionId, $events, $callbackData, EXPIRES_IN
    );
    $webhooksSrvc->UpdateNotificationSubscription($args);
    $_SESSION['subscriptionExpiry'] = EXPIRES_IN + time();

    $arr = array(
        'success' => true,
        'text' => 'Subscription updated.'
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
