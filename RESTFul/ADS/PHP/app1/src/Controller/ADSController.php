<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/ADS/ADSService.php';
require_once __DIR__ . '/../../lib/ADS/OptArgs.php';

use Att\Api\ADS\ADSService;
use Att\Api\ADS\OptArgs;
use Att\Api\Controller\APIController;

class ADSController extends APIController
{
    const RESULT_AD = 0;
    const ERROR_AD = 0;

    private function _init() {
        $inputs = array(
                'MMA', 'ageGroup', 'Premium', 'gender', 'category', 'over18',
                'zipCode', 'city', 'areaCode', 'country', 'latitude',
                'longitude', 'keywords'
                );
        $this->copyToSession($inputs);
    }

    private function _getOptArgs()
    {   
        $optArgs = new OptArgs();

        if (isset($_REQUEST['MMA']) && $_REQUEST['MMA'] != "") {
            $mma = $_REQUEST['MMA'];
            $mmaSplit = explode(' x ', $mma);

            $optArgs->setMaxWidth($mmaSplit[0]);
            $optArgs->setMinWidth($mmaSplit[0]);

            $optArgs->setMaxHeight($mmaSplit[1]);
            $optArgs->setMinHeight($mmaSplit[1]);
        }

        if (isset($_REQUEST['keywords']) && $_REQUEST['keywords'] != "") {
            $keywords = explode(',', $_REQUEST['keywords']);
            $optArgs->setKeywords($keywords);
        }

        if (isset($_REQUEST['ageGroup']) && $_REQUEST['ageGroup'] != "")
            $optArgs->setAgeGroup($_REQUEST['ageGroup']);

        if (isset($_REQUEST['gender']) && $_REQUEST['gender'] != "")
            $optArgs->setGender($_REQUEST['gender']);

        if (isset($_REQUEST['zipCode']) && $_REQUEST['zipCode'] != "")
            $optArgs->setZipCode($_REQUEST['zipCode']);
            
        if (isset($_REQUEST['city']) && $_REQUEST['city'] != "")
            $optArgs->setCity($_REQUEST['city']);
            
        if (isset($_REQUEST['areaCode']) && $_REQUEST['areaCode'] != "")
            $optArgs->setAreaCode($_REQUEST['areaCode']);

        if (isset($_REQUEST['country']) && $_REQUEST['country'] != "")
            $optArgs->setCountry($_REQUEST['country']);
            
        if (isset($_REQUEST['latitude']) && $_REQUEST['latitude'] != "")
            $optArgs->setLatitude($_REQUEST['latitude']);
            
        if (isset($_REQUEST['longitude']) && $_REQUEST['longitude'] != "")
            $optArgs->setLongitude($_REQUEST['longitude']);
    }

    public function __construct()
    {
        parent::__construct();
    }

    public function handleRequest()
    {
        if (!isset($_REQUEST['btnGetAds'])) {
            return;
        }

        try {
            $this->_init();

            $adsService= new ADSService($this->apiFQDN, $this->getFileToken());

            //$ua = $_SERVER['HTTP_USER_AGENT'];
            // TODO (pk9069): move values to config
            $category = $_REQUEST['category'];
            $optArgs = $this->_getOptArgs();
            $ua = 'Mozilla/5.0 (Android; Mobile; rv:13.0) Gecko/13.0 Firefox/13.0';
            $udid = md5('RANDOM TRUST ME');
            $result = $adsService->getAdvertisement(
                $category, $ua, $udid, $optArgs
            );

            if ($result == null) {
                $result = 'No Ads were returned';
            }

            $this->results[ADSController::RESULT_AD] = $result; 
        } catch (Exception $e) {
            $this->errors[ADSController::ERROR_AD] = $e->getMessage();
        }
    }

}
?>
