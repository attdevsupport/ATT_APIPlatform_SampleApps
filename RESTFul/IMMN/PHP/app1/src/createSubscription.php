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
require_once __DIR__ . '/../lib/Webhooks/CreateSubscriptionArgs.php';
require_once __DIR__ . '/../lib/Webhooks/WebhooksService.php';

use Att\Api\Webhooks\CreateSubscriptionArgs;
use Att\Api\Webhooks\WebhooksService;

$arr = null;
try {
    if (isset($_SESSION['subscriptionId'])) {
        throw new Exception('You must first delete your existing subscription.');
    }
    $events = array();
    if (isset($_REQUEST['subscriptionText'])) {
        $events[] = 'TEXT';
    }
    if (isset($_REQUEST['subscriptionMms'])) {
        $events[] = 'MMS';
    }
    if (count($events) == 0) {
        throw new Exception("You must select at least one of Text or MMS");
    }

    envinit();
    $webhooksSrvc = new WebhooksService(getFqdn(), getSessionToken());

    $callbackData = $_REQUEST['callbackData'];
    if ($callbackData == '') {
        $callbackData = null;
    }
    $args = new CreateSubscriptionArgs(
        CHANNEL_ID, $events, $callbackData, EXPIRES_IN
    );
    $response = $webhooksSrvc->createNotificationSubscription($args);
    $subscription = $response->getSubscription();
    $subscriptionId = $subscription->getSubscriptionId();
    $_SESSION['subscriptionId'] = $subscriptionId;
    $_SESSION['subscriptionExpiry'] = EXPIRES_IN + time();

    $arr = array(
        'success' => true,
        'text' => 'Subscription created.'
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
