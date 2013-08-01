<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * TL Library
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2013. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category  API
 * @package   TL
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '../../Srvc/APIService.php';

/**
 * Used to interact with version 2 of the Terminal Location API.
 *
 * @category API
 * @package  TL
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/2/Location
 */
class TLService extends APIService
{
    /**
     * Creates a TLService object that can be used to interact with
     * the terminal location (TL) API.
     *
     * @param string     $FQDN  fully qualified domain name to which request is
                                sent.
     * @param OAuthToken $token OAuth token used for authorization.
     */
    public function __construct($FQDN, OAuthToken $token)
    {
        parent::__construct($FQDN, $token); 
    }

    /**
     * Sends an API request for getting a device's location. 
     *
     * In addition to returning the API response an associative array, this
     * method also adds an 'elapsedTime' key that contains the duration of the
     * request.
     *
     * The values for requested accuracy and acceptable accuracy are as follows:
     *
     * @param int    $rAccuracy specifies the requested accuracy in meters.
     *                          The acceptable values are 0 through 20000.
     *                          The technology used to find the location 
     *                          depends on the requested accuracy and is as 
     *                          follows:
     *                          <ul>
     *                          <li>0-800: Assited GPS (A-GPS)</li>
     *                          <li>801-9999: Enhanced Cell-Id (ECID)</li>
     *                          <li>10000-20000: Cell-Id (CID)</li>
     *                          </ul>
     * @param int    $aAccuracy specifies acceptable accuracy in meters. If API
     *                          request is unable to satisfy this parameter, a 
     *                          service error is returned. 
     * @param string $tolerance specifies the response time priority. The 
     *                          acceptable values are: 
     *                          <ul>
     *                          <li>NoDelay</li>
     *                          <li>LowDelay</li>
     *                          <li>DelayTolerant</li>
     *                          </ul>
     * 
     * @return array an associative array containing the API response and an 
     *               extra 'elapsedTime' key.
     */
    public function getLocation(
        $rAccuracy = 1000, $aAccuracy = 10000, $tolerance = 'NoDelay'
    ) {
        $endpoint = $this->FQDN . '/2/devices/location';

        $req = new RESTFulRequest($endpoint);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $req->addParam('requestedAccuracy', $rAccuracy);
        $req->addParam('acceptableAccuracy', $aAccuracy);
        $req->addParam('tolerance', $tolerance);
        $req->addAuthorizationHeader($this->token);

        $tNow = time();

        $result = $req->sendRequest();
        $responseArr = $this->parseResult($result); 

        $responseArr['elapsedTime'] = time() - $tNow;

        return $responseArr;
    }
}
?>
