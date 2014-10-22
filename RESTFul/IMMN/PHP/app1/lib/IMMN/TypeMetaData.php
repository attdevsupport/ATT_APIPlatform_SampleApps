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

require_once __DIR__ . '/SegmentationDetails.php';

use Att\Api\IMMN\IMMNSegmentationDetails;

final class IMMNTypeMetaData
{
    private $_subject;
    private $_isSegmented;
    private $_segmentationDetails;

    public function __construct($subject, $isSegmented, $segmentationDetails)
    {
        $this->_subject = $subject;
        $this->_isSegmented = $isSegmented;
        $this->_segmentationDetails = $segmentationDetails;
    }

    public function getSubject()
    {
        return $this->_subject;
    }

    public function isSegmented()
    {
        return $this->_isSegmented;
    }

    public function getSegmentationDetails()
    {
        return $this->_segmentationDetails;
    }

    public static function fromArray($arr)
    {
        $isSegmented = null;
        $segDetails = null;
        $subject = null;

        if (isset($arr['isSegmented']) && $arr['isSegmented']) {
            $isSegmented = true;
        }

        if (isset($arr['segmentationDetails'])) {
            $segDetailsArr = $arr['segmentationDetails'];
            $segDetails = IMMNSegmentationDetails::fromArray($segDetailsArr);
        }

        if (isset($arr['subject'])) {
            $subject = $arr['subject'];
        }

        return new IMMNTypeMetaData($subject, $isSegmented, $segDetails);
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
