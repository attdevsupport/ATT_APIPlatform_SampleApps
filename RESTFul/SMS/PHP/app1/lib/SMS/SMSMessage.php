<?php
namespace Att\Api\SMS;

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
 * Immutable class that holds an SMS message.
 *
 * @category API
 * @package  SMS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/SMS
 */
final class SMSMessage
{
    private $_msgId;

    private $_msg;

    private $_senderAddr;


    public function __construct($msgId, $msg, $senderAddr)
    {
        $this->_msgId = $msgId;
        $this->_msg = $msg;
        $this->_senderAddr = $senderAddr;
    }

    public function getMessageId()
    {
        return $this->_msgId;
    }
    public function getMessage()
    {
        return $this->_msg;
    }
    public function getSenderAddress()
    {
        return $this->_senderAddr;
    }

    public static function fromArray($arr)
    {
        $msgId = $arr['MessageId'];
        $msg = $arr['Message'];
        $senderAddr = $arr['SenderAddress'];

        return new SMSMessage($msgId, $msg, $senderAddr);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
