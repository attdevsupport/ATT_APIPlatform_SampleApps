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
require_once __DIR__ . '/../lib/Webhooks/WebhooksService.php';

use Att\Api\Webhooks\WebhooksService;

$arr = null;
try {
    if (!isset($_SESSION['subscriptionId'])) {
        throw new Exception('You must first create a subscription.');
    }
    $subscriptionId = $_SESSION['subscriptionId'];
    envinit();
    $webhooksSrvc = new WebhooksService(getFqdn(), getSessionToken());

    $response = $webhooksSrvc->getNotificationSubscription(CHANNEL_ID, $subscriptionId);
    $arr = array(
        'success' => true,
        'tables' => array(
            array(
                'caption' => 'Subscription Details',
                'headers' => array(
                    'Subscription Id', 'Expires In', 'Queues', 'Callback Data'
                ),
                'values' => array(
                    array(
                        $response->getSubscriptionId(),
                        $response->getExpiresIn(),
                        $response->getEvents(),
                        $response->getCallbackData(),
                    ),
                )
            )
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
