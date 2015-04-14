<?php
namespace Att\Api\Webhooks;

/*
 * Copyright 2014 AT&T
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

require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '../../Srvc/ServiceException.php';
require_once __DIR__ . '/Channel.php';
require_once __DIR__ . '/CreateNotificationResponse.php';
require_once __DIR__ . '/CreateSubscriptionArgs.php';
require_once __DIR__ . '/CreateSubscriptionResponse.php';
require_once __DIR__ . '/GetNotificationResponse.php';
require_once __DIR__ . '/GetSubscriptionResponse.php';
require_once __DIR__ . '/UpdateSubscriptionArgs.php';
require_once __DIR__ . '/UpdateSubscriptionResponse.php';

use Att\Api\OAuth\OAuthToken;
use Att\Api\Restful\HttpGet;
use Att\Api\Restful\HttpPost;
use Att\Api\Restful\HttpPut;
use Att\Api\Restful\RestfulRequest;
use Att\Api\Srvc\APIService;
use Att\Api\Srvc\Service;
use Att\Api\Srvc\ServiceException;
use Att\Api\Webhooks\CreateSubscriptionArgs;
use Att\Api\Webhooks\CreateSubscriptionResponse;
use Att\Api\Webhooks\GetChannelResponse;
use Att\Api\Webhooks\GetNotificationResponse;
use Att\Api\Webhooks\GetSubscriptionResponse;
use Att\Api\Webhooks\SubscriptionResponse;
use Att\Api\Webhooks\UpdateSubscriptionArgs;
use Att\Api\Webhooks\UpdateSubscriptionResponse;

/**
 * Used to interact with version 1 of the Webhooks API.
 *
 * @category API
 * @package  Webhooks
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 */
class WebhooksService extends APIService
{

    /**
     * Creates a WebhooksService object that can be used to interact with
     * the Webhooks API.
     *
     * @param string     $FQDN  fully qualified domain name to which requests
     *                          will be sent
     * @param OAuthToken $token OAuth token used for authorization
     */
    public function __construct($FQDN, OAuthToken $token)
    {
        parent::__construct($FQDN, $token);
    }

    public function createNotificationChannel(Channel $channel)
    {
        $endpoint = $this->getFqdn() . '/notification/v1/channels';

        $req = new RestfulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');
        // PHP strips out .0 from 1.0 during json_encode; therefore, the string
        // has to be manually constructed.
        // Issue has been fixed in future PHP versions by specifying the
        // JSON_PRESERVE_ZERO_FRACTION flag.
        // See: https://bugs.php.net/bug.php?id=50224
        $bodyString = '{"channel":{"serviceName":';
        $bodyString .= ('"'. $channel->getServiceName() . '"');
        $bodyString .= ',"notificationContentType":';
        $bodyString .= json_encode($channel->getNotificationContentType());
        $bodyString .= ',"notificationVersion":1.0}}';
        $httpPost = new HttpPost();
        $httpPost->setBody($bodyString);
        $result = $req->sendHttpPost($httpPost);
        $location = $result->getHeader('location');
        $systemTransId = $result->getHeader('x-systemTransactionId');

        $successCodes = array(201);
        $arr = Service::parseJson($result, $successCodes);
        $arrChannel = $arr['channel'];
        $channelResponseId = $arrChannel['channelId'];
        $channelResponseMaxEvts = null;
        if (isset($arrChannel['maxEventsPerNotification'])) {
            $channelResponseMaxEvts = $arrChannel['maxEventsPerNotification'];
        }

        $channelResponse = new ChannelResponse(
            $channelResponseId, $channelResponseMaxEvts
        );

        return new CreateNotificationResponse(
            $location, $systemTransId, $channelResponse
        );
    }

    public function getNotificationChannel($channelId)
    {
        $channelId = urlencode($channelId);
        $suburl = '/notification/v1/channels/' . $channelId;
        $endpoint = $this->getFqdn() . $suburl;

        $req = new RestfulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json');
        $result = $req->sendHttpGet();
        $systemTransId = $result->getHeader('x-systemTransactionId');

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);
        $arrChannel = $arr['channel'];
        $arrChannelId = $arrChannel['channelId'];
        $arrMaxEvts = null;
        if (isset($arrChannel['maxEventsPerNotification'])) {
            $arrMaxEvts = $arrChannel['maxEventsPerNotification'];
        }
        $arrServiceName = $arrChannel['serviceName'];
        $arrChannelType = null;
        if (isset($arrChannel['channelType'])) {
            $arrChannelType = $arrChannel['channelType'];
        }
        $arrNotificationContenType = $arrChannel['notificationContentType'];
        $arrNotificationVersion = $arrChannel['notificationVersion'];

        return new GetChannelResponse(
            $arrChannel, $arrMaxEvts, $arrServiceName, $arrChannelType,
            $arrNotificationContenType, $arrNotificationVersion
        );
    }

    public function deleteNotificationChannel($channelId)
    {
        $channelId = urlencode($channelId);
        $suburl = '/notification/v1/channels' . $channelId;
        $endpoint = $this->getFqdn() . $suburl;

        $req = new RestfulRequest($endpoint);
        $req->setAuthorizationHeader($this->getToken());
        $result = $req->sendHttpDelete();
        $code = $result->getResponseCode();
        $body = $result->getResponseBody();
        if ($code != 204) {
            throw new ServiceException($body, $code);
        }

        return $result->getHeader('x-systemTransactionId');
    }

    public function createNotificationSubscription(
        CreateSubscriptionArgs $args
    ) {
        $channelId = urlencode($args->getChannelId());
        $suburl = '/notification/v1/channels/' . $channelId . '/subscriptions';
        $endpoint = $this->getFqdn() . $suburl;

        $subscription = array("events" => $args->getEvents());
        if ($args->getCallbackData() != null) {
            $subscription['callbackData'] = $args->getCallbackData();
        }
        if ($args->getExpiresIn() != null) {
            $subscription['expiresIn'] = $args->getExpiresIn();
        }
        $jvals = json_encode(array("subscription" => $subscription));
        $httpPost = new HttpPost();
        $httpPost->setBody($jvals);

        $req = new RestfulRequest($endpoint);
        $result = $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Content-Type', 'application/json')
            ->setHeader('Accept', 'application/json')
            ->sendHttpPost($httpPost);

        $successCodes = array(201);
        $arr = Service::parseJson($result, $successCodes);
        $arrSubscription = $arr['subscription'];
        $arrSubscriptionId = $arrSubscription['subscriptionId'];
        $arrExpiresIn = null;
        if (isset($arrSubscription['expiresIn'])) {
            $arrExpiresIn = $arrSubscription['expiresIn'];
        }

        $subscriptionResponse = new SubscriptionResponse(
            $arrSubscriptionId, $arrExpiresIn
        );
        $contentType = $result->getHeader('content-type');
        $location = $result->getHeader('location');
        $systemTransId = $result->getHeader('x-systemTransactionId');
        $createSubscriptionResponse = new CreateSubscriptionResponse(
            $contentType, $location, $systemTransId, $subscriptionResponse
        );

        return $createSubscriptionResponse;
    }

    public function updateNotificationSubscription(
        UpdateSubscriptionArgs $args
    ) {
        $channelId = urlencode($args->getChannelId());
        $suburl = '/notification/v1/channels/' . $channelId . '/subscriptions/'
            . urlencode($args->getSubscriptionId());
        $endpoint = $this->getFqdn() . $suburl;

        $subscription = array("events" => $args->getEvents());
        if ($args->getCallbackData() != null) {
            $subscription['callbackData'] = $args->getCallbackData();
        }
        if ($args->getExpiresIn() != null) {
            $subscription['expiresIn'] = $args->getExpiresIn();
        }
        $jvals = json_encode(array("subscription" => $subscription));
        $httpPut = new HttpPut($jvals);

        $req = new RestfulRequest($endpoint);
        $result = $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Content-Type', 'application/json')
            ->setHeader('Accept', 'application/json')
            ->sendHttpPut($httpPut);

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);
        $arrSubscription = $arr['subscription'];
        $arrSubscriptionId = $arrSubscription['subscriptionId'];
        $arrExpiresIn = null;
        if (isset($arrSubscription['expiresIn'])) {
            $arrExpiresIn = $arrSubscription['expiresIn'];
        }

        $subscriptionResponse = new SubscriptionResponse(
            $arrSubscriptionId, $arrExpiresIn
        );
        $contentType = $result->getHeader('content-type');
        $systemTransId = $result->getHeader('x-systemTransactionId');
        $updateSubscriptionResponse = new UpdateSubscriptionResponse(
            $contentType, $systemTransId, $subscriptionResponse
        );

        return $updateSubscriptionResponse;
    }

    public function getNotificationSubscription($channelId, $subscriptionId)
    {
        $channelId = urlencode($channelId);
        $subscriptionId = urlencode($subscriptionId);
        $suburl = '/notification/v1/channels/' . $channelId . '/subscriptions/'
            . $subscriptionId;
        $endpoint = $this->getFqdn() . $suburl;

        $req = new RestfulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json');

        $result = $req->sendHttpGet();
        $contentType = $result->getHeader('content-type');
        $systemTransId = $result->getHeader('x-systemTransactionId');

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);
        $arrSubscription = $arr['subscription'];
        $arrSubscriptionId = $arrSubscription['subscriptionId'];
        $arrExpiresIn = $arrSubscription['expiresIn'];
        /* TODO: remove work-around for events/eventFilters check */
        $arrEvents = null;
        if (isset($arrSubscription['events'])) {
            $arrEvents = $arrSubscription['events'];
        } else {
            $arrEvents = $arrSubscription['eventFilters'];
        }
        $arrCallbackData = $arrSubscription['callbackData'];

        return new GetSubscriptionResponse(
            $contentType, $arrSubscriptionId, $arrExpiresIn, $arrEvents,
            $arrCallbackData, $systemTransId
        );
    }

    public function deleteNotificationSubscription($channelId, $subscriptionId)
    {
        $channelId = urlencode($channelId);
        $subscriptionId = urlencode($subscriptionId);
        $suburl = '/notification/v1/channels/' . $channelId . '/subscriptions/';
        $suburl = $suburl . $subscriptionId;
        $endpoint = $this->getFqdn() . $suburl;

        $req = new RestfulRequest($endpoint);
        $req->setAuthorizationHeader($this->getToken());
        $result = $req->sendHttpDelete();
        $code = $result->getResponseCode();
        $body = $result->getResponseBody();
        if ($code != 204) {
            throw new ServiceException($body, $code);
        }

        return $result->getHeader('x-systemTransactionId');
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
