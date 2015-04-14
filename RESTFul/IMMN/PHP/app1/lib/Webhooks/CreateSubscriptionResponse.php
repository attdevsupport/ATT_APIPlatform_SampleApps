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

require_once __DIR__ . '/SubscriptionResponse.php';

use Att\Api\Webhooks\SubscriptionResponse;

/**
 * Contains response values for a Create Subscription call.
 *
 * @category API
 * @package  Webhooks
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 */
final class CreateSubscriptionResponse
{
    private $_contentType;
    private $_location;
    private $_systemTransId;
    private $_subscription;

    public function __construct(
        $contentType, $location, $systemTransId,
        SubscriptionResponse $subscription
    ) {
        $this->_contentType = $contentType;
        $this->_location = $location;
        $this->_systemTransId = $systemTransId;
        $this->_subscription = $subscription;
    }

    public function getContentType()
    {
        return $this->_contentType;
    }

    public function getLocation()
    {
        return $this->_location;
    }

    public function getSystemTransactionId()
    {
        return $this->_systemTransId;
    }

    public function getSubscription()
    {
        return $this->_subscription;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
