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

final class ChannelResponse
{
    private $_channelId;
    private $_maxEventsPerNotification;

    public function __construct($channelId, $maxEventsPerNotification)
    {
        $this->_channelId = $channelId;
        $this->_maxEventsPerNotification = $maxEventsPerNotification;
    }
}

/**
 * Contains response for creating notification.
 *
 * @category API
 * @package  Webhooks
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 */
final class CreateNotificationResponse
{
    private $_location;
    private $_systemTransId;
    private $_channel;

    public function __construct(
        $location, $systemTransId, $channel
    ) {
        $this->_location = $location;
        $this->_systemTransId = $systemTransId;
        $this->_channel = $channel;
    }

    public function getLocation()
    {
        return $this->_location;
    }

    public function getSystemTransactionId()
    {
        return $this->_systemTransId;
    }

    public function getChannel()
    {
        return $this->_channel;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
