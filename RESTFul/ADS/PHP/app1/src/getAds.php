<?php
/*
 * Copyright 2015 AT&T
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
session_start();

require __DIR__ . '/../config.php';
require_once __DIR__ . '/common.php';
require_once __DIR__ . '/../lib/ADS/ADSService.php';
require_once __DIR__ . '/../lib/ADS/OptArgs.php';

use Att\Api\ADS\ADSService;
use Att\Api\ADS\OptArgs;
use Att\Api\Controller\APIController;

function parseArgs() {
    $optArgs = new OptArgs();

    if (isset($_POST['mmaSize']) && $_POST['mmaSize'] != "") {
        $mma = $_POST['mmaSize'];
        $mmaSplit = explode(' x ', $mma);

        $optArgs->setMaxWidth($mmaSplit[0]);
        $optArgs->setMinWidth($mmaSplit[0]);

        $optArgs->setMaxHeight($mmaSplit[1]);
        $optArgs->setMinHeight($mmaSplit[1]);
    }

    if (isset($_POST['keywords']) && $_POST['keywords'] != "") {
        $keywords = explode(',', $_POST['keywords']);
        $optArgs->setKeywords($keywords);
    }

    if (isset($_POST['ageGroup']) && $_POST['ageGroup'] != "")
        $optArgs->setAgeGroup($_POST['ageGroup']);

    if (isset($_POST['gender']) && $_POST['gender'] != "")
        $optArgs->setGender($_POST['gender']);

    if (isset($_POST['zipCode']) && $_POST['zipCode'] != "")
        $optArgs->setZipCode($_POST['zipCode']);
        
    if (isset($_POST['city']) && $_POST['city'] != "")
        $optArgs->setCity($_POST['city']);
        
    if (isset($_POST['areaCode']) && $_POST['areaCode'] != "")
        $optArgs->setAreaCode($_POST['areaCode']);

    if (isset($_POST['country']) && $_POST['country'] != "")
        $optArgs->setCountry($_POST['country']);
        
    if (isset($_POST['latitude']) && $_POST['latitude'] != "")
        $optArgs->setLatitude($_POST['latitude']);
        
    if (isset($_POST['longitude']) && $_POST['longitude'] != "")
        $optArgs->setLongitude($_POST['longitude']);

    return $optArgs;
}

$arr = null;
try {
    envinit();
    $adsService = new ADSService(getFqdn(), getFileToken());

    $category = $_POST['category'];
    $optArgs = parseArgs();

    $result = $adsService->getAdvertisement(
        $category, $userAgent, $udid, $optArgs
    );
    if ($result === null) {
        $arr = array(
            'success' => true,
            'text' => 'No Ads were returned',
        );
    } else {
        $arr = array(
            'success' => true,
            'tables' => array(
                array(
                    'caption' => 'Ads Response:',
                    'headers' => array('Type', 'ClickUrl'),
                    'values' => array(
                        array($result->getAdsType(), $result->getClickUrl())
                    )
                ),
            ),
        );
    }
} catch (Exception $e) {
    $arr = array(
        'success' => false,
        'text' => $e->getMessage()
    );
}

echo json_encode($arr);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
