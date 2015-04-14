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

final class IMMNMessageIndexInfo
{
    private $_status;
    private $_state;
    private $_msgCount;

    public function __construct($status, $state, $msgCount)
    {
        $this->_status = $status;
        $this->_state = $state;
        $this->_msgCount = $msgCount;
    }

    public function getStatus()
    {
        return $this->_status;
    }

    public function getState()
    {
        return $this->_state;
    }

    public function getMessageCount()
    {
        return $this->_msgCount;
    }

    public static function fromArray($arr)
    {
        $msgIndexInfoArr = $arr['messageIndexInfo'];

        $status = $msgIndexInfoArr['status'];
        $state = $msgIndexInfoArr['state'];
        $msgCount = $msgIndexInfoArr['messageCount'];

        return new IMMNMessageIndexInfo($status, $state, $msgCount);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
