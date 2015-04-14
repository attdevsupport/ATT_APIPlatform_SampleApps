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


/**
 * Contains arguments for updating a subscription.
 *
 * @category API
 * @package  Webhooks
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 */
final class UpdateSubscriptionArgs
{
    private $_channelId;
    private $_subscriptionId;
    private $_events;
    private $_callbackData;
    private $_expiresIn;

    public function __construct(
        $channelId, $subscriptionId, $events, $callbackData=null,
        $expiresIn=null
    ) {
        $this->_channelId = $channelId;
        $this->_subscriptionId = $subscriptionId;
        $this->_events = $events;
        $this->_callbackData = $callbackData;
        $this->_expiresIn = $expiresIn;
    }

    public function getChannelId()
    {
        return $this->_channelId;
    }

    public function getSubscriptionId()
    {
        return $this->_subscriptionId;
    }

    public function getEvents()
    {
        return $this->_events;
    }

    public function getCallbackData()
    {
        return $this->_callbackData;
    }

    public function getExpiresIn()
    {
        return $this->_expiresIn;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
