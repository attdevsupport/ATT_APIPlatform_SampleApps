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

require_once __DIR__ . '/DeliveryInfo.php';

/**
 * Immutable class that holds a an SMS Status.
 *
 * @category API
 * @package  SMS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/SMS
 */
final class DeliveryStatus
{
    private $_resourceUrl;

    private $_deliveryInfoList;

    public function __construct($resourceUrl, $deliveryInfoList)
    {
        $this->_resourceUrl = $resourceUrl;
        $this->_deliveryInfoList = $deliveryInfoList;
    }

    public function getResourceUrl()
    {
        return $this->_resourceUrl;
    }

    public function getDeliveryInfoList()
    {
        return $this->_deliveryInfoList;
    }

    public static function fromArray($arr)
    {
        // TODO: throw exception if required fields are not set
        
        $infoList = $arr['DeliveryInfoList'];
        $resourceUrl = $infoList['ResourceUrl'];

        $dInfoListModel = array();

        $deliveryInfoList = $infoList['DeliveryInfo'];
        foreach($deliveryInfoList as $deliveryInfo) {
            $dInfoModel = DeliveryInfo::fromArray($deliveryInfo);

            $dInfoListModel[] = $dInfoModel;
        }

        return new DeliveryStatus($resourceUrl, $dInfoListModel);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
