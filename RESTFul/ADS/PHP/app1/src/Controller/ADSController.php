<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/ADS/ADSService.php';

class ADSController extends APIController
{
    const RESULT_AD = 0;
    const ERROR_AD = 0;

    private function saveInputsToSession()
    {
        $inputs = array('category', 'MMA', 'ageGroup', 'Premium', 'gender',
                'over18', 'zipCode', 'city', 'areaCode', 'country', 'latitude',
                'longitude', 'keywords');

        foreach ($inputs as $input) {
            if (isset($_REQUEST[$input])) {
                $_SESSION[$input] = $_REQUEST[$input];
            }
        }

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
            // save user input to session 
            $this->saveInputsToSession();

            $category = $_REQUEST['category'];

            $adsService= new ADSService($this->FQDN, $this->getFileToken());

            $inputs = array('MMA', 'ageGroup', 'Premium', 'gender',
                    'over18', 'zipCode', 'city', 'areaCode', 'country', 
                    'latitude', 'longitude', 'keywords');

            $keys = array('MMASize', 'AgeGroup', 'Premium', 'Gender', 'Over18',
                    'ZipCode', 'City', 'AreaCode', 'Country', 'Latitude', 
                    'Longitude', 'Keywords');

            $optVals = array();
            for ($i = 0; $i < count($inputs); ++$i) {
                $key = $keys[$i];
                $input = $inputs[$i];

                if (isset($_REQUEST[$input]) && $_REQUEST[$input] != "") {
                    $optVals[$key] = $_REQUEST[$input];
                }
            }

            $ua = $_SERVER['HTTP_USER_AGENT'];
            $udid = md5('RANDOM TRUST ME');
            $result = $adsService->getAdvertisement(
                $category, $ua, $udid, $optVals
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
