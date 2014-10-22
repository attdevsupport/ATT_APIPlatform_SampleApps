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

final class IMMNSegmentationDetails
{
    private $_segmentationMsgRefNumber;
    private $_totalNumberOfParts;
    private $_thisPartNumber;

    public function __construct($refNumber, $numParts, $partNumb)
    {
        $this->_segmentationMsgRefNumber = $refNumber;
        $this->_totalNumberOfParts = $numParts;
        $this->_thisPartNumber = $partNumb;
    }

    public function getSegmentationMsgRefNumber()
    {
        return $this->_segmentationMsgRefNumber;
    }

    public function getTotalNumberOfParts()
    {
        return $this->_totalNumberOfParts;
    }

    public function getThisPartNumber()
    {
        return $this->_thisPartNumber;
    }

    public static function fromArray($arr)
    {
        $refNumber = $arr['segmentationMsgRefNumber'];
        $numParts = $arr['totalNumberOfParts'];
        $partNumb = $arr['thisPartNumber'];

        return new IMMNSegmentationDetails($refNumber, $numParts, $partNumb);
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
