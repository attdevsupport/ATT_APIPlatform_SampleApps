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
 * Contains response values for a Get Subscription call.
 *
 * @category API
 * @package  Webhooks
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 */
final class GetSubscriptionResponse
{
    private $_contentType;
    private $_subscriptionId;
    private $_expiresIn;
    private $_events;
    private $_callbackData;
    private $_systemTransId;

    public function __construct(
        $contentType, $subscriptionId, $expiresIn, $events, $callbackData,
        $systemTransId=null
    ) {
        $this->_contentType = $contentType;
        $this->_subscriptionId = $subscriptionId;
        $this->_expiresIn = $expiresIn;
        $this->_events = $events;
        $this->_callbackData = $callbackData;
        $this->_systemTransId = $systemTransId;
    }

    public function getContentType()
    {
        return $this->_contentType;
    }

    public function getSubscriptionId()
    {
        return $this->_subscriptionId;
    }

    public function getExpiresIn()
    {
        return $this->_expiresIn;
    }

    public function getEvents()
    {
        return $this->_events;
    }

    public function getCallbackData()
    {
        return $this->_callbackData;
    }

    public function getSystemTransactionId()
    {
        return $this->_systemTransId;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
