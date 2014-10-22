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

final class IMMNMmsContent
{
    private $_contentName;
    private $_contentUrl;
    private $_contentType;
    private $_type;

    public function __construct($contentName, $contentType, $contentUrl, $type)
    {
        $this->_contentName = $contentName;
        $this->_contentType = $contentType;
        $this->_contentUrl = $contentUrl;
        $this->_type = $type;
    }

    public function getContentName()
    {
        return $this->_contentName;
    }

    public function getContentUrl()
    {
        return $this->_contentUrl;
    }

    public function getContentType()
    {
        return $this->_contentType;
    }

    public function getMessageType()
    {
        return $this->_type;
    }

    public static function fromArray($arr)
    {
        $cname = $arr['contentName'];
        $ctype = $arr['contentType'];
        $curl = $arr['contentUrl'];
        $type = $arr['type'];

        $content = new IMMNMmsContent($cname, $ctype, $curl, $type);
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
