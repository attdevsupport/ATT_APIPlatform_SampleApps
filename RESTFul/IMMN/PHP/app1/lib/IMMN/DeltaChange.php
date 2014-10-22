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

/**
 * Immutable class used to hold a Delta object.
 *
 * @category API
 * @package  IMMN
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 */
final class IMMNDeltaChange
{
    private $_msgId;
    private $_favorite;
    private $_unread;

    public function __construct($msgId, $favorite, $unread)
    {
        $this->_msgId = $msgId;
        $this->_favorite = $favorite;
        $this->_unread = $unread;
    }

    public function getMessageId()
    {
        return $this->_msgId;
    }

    public function isFavorite()
    {
        return $this->_favorite;
    }

    public function isUnread()
    {
        return $this->_unread;
    }

    public function toArray()
    {
        $arr = array();
        $arr['messageId'] = $this->getMessageId();

        if (isset($arr['isFavorite'])) {
            $arr['isFavorite'] = $this->isFavorite();
        }
        if (isset($arr['isUnread'])) {
            $arr['isUnread'] = $this->isUnread();
        }

        return $arr;
    }

    public static function fromArray($arr)
    {
        $msgId = $arr['messageId'];
        $favorite = $arr['isFavorite'];
        $unread = $arr['isUnread'];

        return new IMMNDeltaChange($msgId, $favorite, $unread);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
