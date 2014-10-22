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

require_once __DIR__ . '/SMSMessage.php';

/**
 * Immutable class that holds a Get SMS response.
 *
 * @category API
 * @package  SMS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 * @link     https://developer.att.com/docs/apis/rest/3/SMS
 */
final class GetSMSResponse
{
    private $_numberOfMsgs;

    private $_resourceUrl;

    private $_pendingMsgs;

    private $_smsMessageList;

    public function __construct(
        $numberOfMsgs, $resourceUrl, $pendingMsgs, $smsMessageList
    ) {
        $this->_numberOfMsgs = $numberOfMsgs;

        $this->_resourceUrl = $resourceUrl;

        $this->_pendingMsgs = $pendingMsgs;

        $this->_smsMessageList = $smsMessageList;

    }

    public function getNumberOfMessages()
    {
        return $this->_numberOfMsgs;
    }

    public function getResourceUrl()
    {
        return $this->_resourceUrl;
    }

    public function getNumberOfPendingMessages()
    {
        return $this->_pendingMsgs;
    }

    public function getMessages()
    {
        return $this->_smsMessageList;
    }

    public static function fromArray($arr)
    {
        $inboundList = $arr['InboundSmsMessageList'];

        $numberOfMsgs = $inboundList['NumberOfMessagesInThisBatch'];
        $resourceUrl = $inboundList['ResourceUrl'];
        $pendingMsgs = $inboundList['TotalNumberOfPendingMessages'];

        $smsMsgs = array();
        $inboundSmsMessage = $inboundList['InboundSmsMessage'];
        foreach ($inboundSmsMessage as $inboundMsg) {
            $smsMsgs[] = SMSMessage::fromArray($inboundMsg);
        }

        return new GetSMSResponse(
            $numberOfMsgs, $resourceUrl, $pendingMsgs, $smsMsgs
        );
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
