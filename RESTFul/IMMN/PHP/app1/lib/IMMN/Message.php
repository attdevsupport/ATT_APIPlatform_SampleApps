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

require_once __DIR__ . '/TypeMetaData.php';
require_once __DIR__ . '/MmsContent.php';

use Att\Api\IMMN\IMMNMmsContent;
use Att\Api\IMMN\IMMNTypeMetaData;

final class IMMNMessage
{

    private $_msgId;
    private $_from;
    private $_recipients;
    private $_text;
    private $_mmsContent;
    private $_timeStamp;
    private $_isFavorite;
    private $_isUnread;
    private $_type;
    private $_typeMetaData;
    private $_isIncoming;

    // disallow default contructor
    private function __construct()
    {
        $this->_msgId = null;
        $this->_from = null;
        $this->_recipients = null;
        $this->_text = null;
        $this->_mmsContent = null;
        $this->_timeStamp = null;
        $this->_isFavorite = null;
        $this->_isUnread = null;
        $this->_type = null;
        $this->_typeMetaData = null;
        $this->_isIncoming = null;
    }

    public function getMessageId()
    {
        return $this->_msgId;
    }

    public function getFrom()
    {
        return $this->_from;
    }
    public function getRecipients()
    {
        return $this->_recipients;
    }

    public function getText()
    {
        return $this->_text;
    }

    public function getMmsContent()
    {
        return $this->_mmsContent;
    }

    public function getTimeStamp()
    {
        return $this->_timeStamp;
    }

    public function isFavorite()
    {
        return $this->_isFavorite;
    }

    public function isUnread()
    {
        return $this->_isUnread;
    }

    public function getMessageType()
    {
        return $this->_type;
    }

    public function getTypeMetaData()
    {
        return $this->_typeMetaData;
    }

    public function isIncoming()
    {
        return $this->_isIncoming;
    }

    public static function fromArray($arr)
    {
        $immnMsg = new IMMNMessage();

        // required
        $immnMsg->_msgId = $arr['messageId'];
        $immnMsg->_isFavorite = $arr['isFavorite'];
        $immnMsg->_isUnread = $arr['isUnread'];
        $immnMsg->_type = $arr['type'];

        $typeMetaDataArr = $arr['typeMetaData'];
        $immnMsg->_typeMetaData = IMMNTypeMetaData::fromArray($typeMetaDataArr);

        // optional

        if (isset($arr['from'])) {
            $fromObj = $arr['from'];
            $immnMsg->_from = $fromObj['value'];
        }

        if (isset($arr['recipients'])) {
            $recipients = array();

            $recipientsArr = $arr['recipients'];
            foreach ($recipientsArr as $recipientObj) {
                $recipients[] = $recipientObj['value'];
            }
                
            $immnMsg->_recipients = $recipients;
        }

        if (isset($arr['text']))
            $immnMsg->_text = $arr['text'];

        if (isset($arr['mmsContent'])) {
            $mmsContents = array();

            $mmsContentArr = $arr['mmsContent'];
            foreach ($mmsContentArr as $mmsContentObj) {
                $mmsContents[] = IMMNMmsContent::fromArray($mmsContentObj);
            }

            $immnMsg->_mmsContent = $arr['mmsContent'];
        }

        if (isset($arr['timeStamp']))
            $immnMsg->_timeStamp = $arr['timeStamp'];

        if (isset($arr['isIncoming']))
            $immnMsg->_isIncoming = $arr['isIncoming'] == 'true' ? true : false;

        return $immnMsg;
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
