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

final class IMMNMessageContent
{
    private $_contentType;
    private $_contentLength;
    private $_content;

    private function __construct()
    {
    }

    public function getContentType()
    {
        return $this->_contentType;
    }

    public function getContentLength()
    {
        return $this->_contentLength;
    }

    public function getContent()
    {
        return $this->_content;
    }

    public static function fromArray($arr)
    {
        $immnMsgContent = new IMMNMessageContent();

        $immnMsgContent->_contentType = $arr['contentType'];
        $immnMsgContent->_contentLength = $arr['contentLength'];
        $immnMsgContent->_content = $arr['content'];

        return $immnMsgContent;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
