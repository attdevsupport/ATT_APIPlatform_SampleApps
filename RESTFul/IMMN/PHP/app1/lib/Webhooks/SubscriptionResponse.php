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
 * Contains response values for the subscription object.
 *
 * @category API
 * @package  Webhooks
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 */
final class SubscriptionResponse
{
    private $_subscriptionId;
    private $_expiresIn;

    public function __construct($subscriptionId, $expiresIn) {
        $this->_subscriptionId = $subscriptionId;
        $this->_expiresIn = $expiresIn;
    }

    public function getSubscriptionId()
    {
        return $this->_subscriptionId;
    }

    public function getExpiresIn()
    {
        return $this->_expiresIn;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
