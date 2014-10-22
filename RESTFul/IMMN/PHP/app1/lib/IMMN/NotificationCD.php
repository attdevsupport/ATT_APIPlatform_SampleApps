<?php
namespace Att\Api\IMMN;

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

// immn notification connection details
final class IMMNNotificactionCD
{
    private $_uname;
    private $_pw;
    private $_httpsUrl;
    private $_wssUrl;
    private $_queues;

    public function __construct($uname, $pw, $httpsUrl, $wssUrl, $queues)
    {
        $this->_uname = $uname;
        $this->_pw = $pw;
        $this->_httpsUrl = $httpsUrl;
        $this->_wssUrl = $wssUrl;
        $this->_queues = $queues;
    }

    public function getUsername()
    {
        return $this->_uname;
    }

    public function getPassword()
    {
        return $this->_pw;
    }

    public function getHttpsUrl()
    {
        return $this->_httpsUrl;
    }

    public function getWssUrl()
    {
        return $this->_wssUrl;
    }

    public function getQueues()
    {
        return $this->_queues;
    }

    public static function fromArray($arr)
    {
        $notificationCDArr = $arr['notificationConnectionDetails'];

        $uname = $notificationCDArr['username'];
        $pw = $notificationCDArr['password'];
        $httpsUrl = $notificationCDArr['httpsUrl'];
        $wssUrl = $notificationCDArr['wssUrl'];
        $queues = $notificationCDArr['queues'];

        return new IMMNNotificactionCD(
            $uname, $pw, $httpsUrl, $wssUrl, $queues
        );
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
