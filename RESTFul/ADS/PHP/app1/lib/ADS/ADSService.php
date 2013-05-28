<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * ADS Library
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
 * @package   ADS 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&T License
 * @link      http://developer.att.com
 */
require_once __DIR__ . '../../Srvc/APIService.php';

/**
 * Used to interact with version 1 of the Advertising API.
 *
 * For a list of acceptable values and their definitions, refer to 
 * {@link https://developer.att.com/docs/apis/rest/1/Advertising}.
 *
 * @category API
 * @package  ADS
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Advertising
 */
class ADSService extends APIService
{
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
     * @param string $category  category of this app.
     * @param string $userAgent user agent string to send to API.
     * @param string $udid      specifies a universially unique identifier,
     *                          which must be at least 30 characters in length.
     * @param array  $optVals   any optional values as an associative array.
     *
     * @return null|array null if no ads were returned, 
     *                    otherwise an array of key-value pairs
     * @throws ServiceException if API request was not successful
     */
    public function getAdvertisement(
        $category, $userAgent, $udid, $optVals = array()
    ) {
        $endpoint = $this->FQDN . '/rest/1/ads';

        $req = new RESTFulRequest($endpoint); 
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $req->addAuthorizationHeader($this->token);
        $req->addParam('Category', $category);
        foreach ($optVals as $key => $val) {
            $req->addParam($key, $val);
        }

        $req->setHeader('User-Agent', $userAgent);
        $req->setHeader('UDID', $udid);

        $result = $req->sendRequest();
        // no ads returned
        if ($result->getResponseCode() == 204) {
            return null;
        }

        return $this->parseResult($result);
    }

}
?>
