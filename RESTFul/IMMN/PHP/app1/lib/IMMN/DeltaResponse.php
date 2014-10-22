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

require_once __DIR__ . '/Delta.php';

use Att\Api\IMMN\Delta;

/**
 * Immutable class used to hold a DeltaResponse object.
 *
 * @category API
 * @package  IMMN
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 */
final class DeltaResponse
{
    private $_state;
    private $_deltas;

    private function __construct($state, $deltas)
    {
        $this->_state = $state;
        $this->_deltas = $deltas;
    }

    public function getState()
    {
        return $this->_state;
    }

    public function getDeltas()
    {
        return $this->_deltas;
    }

    public static function fromArray($arr)
    {
        $deltasResponseArr = $arr['deltaResponse'];
        $state = $deltasResponseArr['state'];

        $deltas = array();
        $deltasArr = $deltasResponseArr['delta'];
        foreach($deltasArr as $deltaArr) {
            $delta = Delta::fromArray($deltaArr);
            $deltas[] = $delta;
        }

        return new DeltaResponse($state, $deltas);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
