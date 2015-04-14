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

final class GetChannelResponse
{
    private $_channelId;
    private $_maxEventsPerNotification;
    private $_serviceName;
    private $_channelType;
    private $_notificationContentType;
    private $_notificationVersion;

    public function __construct(
        $channelId, $maxEventsPerNotification,
        $serviceName, $channelType, $notificationContentType,
        $notificationVersion
    ) {
        $this->_channelId = $channelId;
        $this->_maxEventsPerNotification = $maxEventsPerNotification;
        $this->_serviceName = $serviceName;
        $this->_channelType = $channelType;
        $this->_notificationContentType = $notificationContentType;
        $this->_notificationVersion = $notificationVersion;
    }

    public function getChannelId()
    {
       return $this->_channelId; 
    }

    public function getMaxEventsPerNotification()
    {
        return $this->_maxEventsPerNotification;
    }

    public function getServiceName()
    {
        return $this->_serviceName;
    }

    public function getChannelType()
    {
        return $this->_channelType;
    }

    public function getNotificationContentType()
    {
        return $this->_notificationContentType;
    }

    public function getNotificationVersion()
    {
        return $this->_notificationVersion;
    }
}

final class GetNotificationResponse
{
    private $_systemTransId;
    private $_channel;

    public function __construct($systemTransId, $channel) {
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
