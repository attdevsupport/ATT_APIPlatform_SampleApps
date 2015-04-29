<?php
namespace Att\Api\ADS;

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

require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '/ADSResponse.php';

use Att\Api\Restful\RestfulRequest;
use Att\Api\Restful\HttpGet;
use Att\Api\Srvc\APIService;
use Att\Api\Srvc\Service;
use Att\Api\OAuth\OAuthToken;

/**
 * Used to interact with version 1 of the Advertising API.
 *
 * For a list of acceptable values and their definitions, refer to 
 * {@link https://developer.att.com/docs/apis/rest/1/Advertising}.
 *
 * @category API
 * @package  ADS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Advertising
 */
class ADSService extends APIService
{

    /**
     * Convenience method to append any optional arguments to the specified
     * <var>$httpGet</var> object.
     *
     * @param HttpGet $httpGet HttpGet object to append to
     * @param OptArgs $optArgs optional arguments used for appending
     *
     * @return void
     */
    private function _appendOptArgs(HttpGet $httpGet, OptArgs $optArgs)
    {
        $keys = array(
            'AgeGroup', 'AreaCode', 'City', 'Country', 'Gender',
            'Keywords', 'Latitude', 'Longitude', 'MaxHeight', 'MaxWidth',
            'MinHeight', 'MinWidth', 'Type', 'ZipCode'
        );

        $keywords = $optArgs->getKeywords();
        if ($keywords !== null)
            $keywords = implode(',', $optArgs->getKeywords());

        $vals = array(
            $optArgs->getAgeGroup(), $optArgs->getAreaCode(),
            $optArgs->getCity(), $optArgs->getCountry(), $optArgs->getGender(),
            $keywords, $optArgs->getLatitude(), $optArgs->getLongitude(),
            $optArgs->getMaxHeight(), $optArgs->getMaxWidth(),
            $optArgs->getMinHeight(), $optArgs->getMinWidth(),
            $optArgs->getAdType(), $optArgs->getZipCode()
        );

        for ($i = 0; $i < count($keys); ++$i) {
            $key = $keys[$i];
            $val = $vals[$i];

            if ($val != null) {
                $httpGet->setParam($key, $val);
            }
        }
    }

    /**
     * Creates an ADSService object that can be used to interact with
     * the ADS API.
     *
     * @param string     $FQDN  fully qualified domain name to which requests 
     *                          will be sent
     * @param OAuthToken $token OAuth token used for authorization 
     */
    public function __construct($FQDN, OAuthToken $token)
    {
        parent::__construct($FQDN, $token); 
    }

    /**
     * Sends a request to the API for getting an advertisement. 
     * 
     * @param string       $category  category of this app.
     * @param string       $userAgent user agent string to send to API.
     * @param string       $udid      specifies a universially unique
     *                                identifier, which must be at least 30
     *                                characters in length.
     * @param OptArgs|null $optArgs   any optional values.
     *
     * @return null|ADSResponse null if no ads were returned, 
     *                          otherwise an ADSResponse object
     * @throws ServiceException if API request was not successful
     */
    public function getAdvertisement(
        $category, $userAgent, $udid, OptArgs $optArgs = null
    ) {

        $endpoint = $this->getFqdn() . '/rest/1/ads';

        $req = new RestfulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('User-agent', $userAgent)
            ->setHeader('Udid', $udid);

        $httpGet = new HttpGet();
        $httpGet->setParam('Category', $category);

        if ($optArgs != null)
            $this->_appendOptArgs($httpGet, $optArgs);

        $result = $req->sendHttpGet($httpGet);

        // no ads returned
        if ($result->getResponseCode() == 204) {
            return null;
        }

        // response as json array
        $jarr = Service::parseJson($result);

        return ADSResponse::fromArray($jarr);
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
