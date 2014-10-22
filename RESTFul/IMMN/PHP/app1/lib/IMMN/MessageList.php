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

require_once __DIR__ . '/Message.php';

use Att\Api\IMMN\IMMNMessage;

final class IMMNMessageList
{
    private $_msgs;
    private $_limit;
    private $_offset;
    private $_total;
    private $_state;
    private $_cacheStatus;
    private $_failedMessages;

    // disallow default contructor
    private function __construct()
    {
        $_msgs = null;
        $_limit = null;
        $_offset = null;
        $_total = null;
        $_state = null;
        $_cacheStatus = null;
        $_failedMessages = null;
    }

    public function getMessages()
    {
        return $this->_msgs;
    }

    public function getLimit()
    {
        return $this->_limit;
    }

    public function getOffset()
    {
        return $this->_offset;
    }

    public function getTotal()
    {
        return $this->_total;
    }

    public function getState()
    {
        return $this->_state;
    }

    public function getCacheStatus()
    {
        return $this->_cacheStatus;
    }

    public function getFailedMessages()
    {
        return $this->_failedMessages;
    }

    public static function fromArray($arr)
    {
        $immnMsgList = new IMMNMessageList();

        // TODO: finish
        $msgList = $arr['messageList'];

        $immnMsgList->_offset = $msgList['offset'];
        $immnMsgList->_limit = $msgList['limit'];
        $immnMsgList->_total = $msgList['total'];
        $immnMsgList->_state = $msgList['state'];
        $immnMsgList->_cacheStatus = $msgList['cacheStatus'];
        
        if (isset($msgList['failedMessages'])) {
            $immnMsgList->_failedMessages = $msgList['failedMessages'];
        }

        $msgs = $msgList['messages'];
        $immnMsgs = array();

        foreach ($msgs as $msg) {
            $immnMsgs[] = IMMNMessage::fromArray($msg);
        }

        $immnMsgList->_msgs = $immnMsgs;

        return $immnMsgList;
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
