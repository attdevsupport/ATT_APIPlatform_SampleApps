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

require_once __DIR__ . '/DeltaChange.php';

use Att\Api\IMMN\IMMNDeltaChange;

/**
 * Immutable class used to hold a Delta object.
 *
 * @category API
 * @package  IMMN
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 */
final class Delta
{
    private $_type;
    private $_adds;
    private $_deletes;
    private $_updates;

    public function __construct($type, $adds, $deletes, $updates)
    {
        $this->_type = $type;
        $this->_adds = $adds;
        $this->_deletes = $deletes;
        $this->_updates = $updates;
    }

    public function getDeltaType()
    {
        return $this->_type;
    }

    public function getAdds()
    {
        return $this->_adds;
    }

    public function getDeletes()
    {
        return $this->_deletes;
    }

    public function getUpdates()
    {
        return $this->_updates;
    }

    private static function getDeltaChanges($arr)
    {
        $deltaChanges = array();
        foreach($arr as $deltaChangeArr) {
            $deltaChange = IMMNDeltaChange::fromArray($deltaChangeArr);
            $deltaChanges[] = $deltaChange;
        }

        return $deltaChanges;
    }

    public static function fromArray($arr)
    {
        $type = $arr['type'];
        $adds = Delta::getDeltaChanges($arr['adds']); 
        $deletes = Delta::getDeltaChanges($arr['deletes']); 
        $updates = Delta::getDeltaChanges($arr['updates']); 

        return new Delta($type, $adds, $deletes, $updates);
    }
    
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
