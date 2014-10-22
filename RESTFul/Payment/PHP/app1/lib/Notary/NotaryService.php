<?php
namespace Att\Api\Notary;

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
require_once __DIR__ . '/NotaryArguments.php';
require_once __DIR__ . '/Notary.php';

use Att\Api\Restful\HttpPost;
use Att\Api\Restful\RestfulRequest;
use Att\Api\Srvc\APIService;
use Att\Api\Srvc\Service;

/**
 * Used to interact with the Notary API.
 *
 * @category API
 * @package  Notary 
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     http://developer.att.com
 */
class NotaryService extends Service
{
    /** 
     * Fully qualified domain name.
     */
    private $_fqdn;

    /**
     * Client id.
     */
    private $_clientId;

    /**
     * Client secret.
     */
    private $_clientSecret;

    /** 
     * Adds the required http headers to the specified request object.
     *
     * @param RestfulRequest $req restful request object
     *
     * @return void
     */
    private function _setHeaders($req)
    {
        $req->setHeader('Content-Type', 'application/json');
        $req->setHeader('Accept', 'application/json');
        $req->setHeader('Client_id', $this->_clientId);
        $req->setHeader('Client_secret', $this->_clientSecret);
    }

    /**
     * Creates a NotaryService object used to send api requests to generate
     * notaries.
     *
     * @param string $fqdn         fully qualified domain name
     * @param string $clientId     client id
     * @param string $clientSecret client secret
     */
    public function __construct($fqdn, $clientId, $clientSecret)
    {
        $this->_fqdn = $fqdn;
        $this->_clientId = $clientId;
        $this->_clientSecret = $clientSecret;
    }

    /**
     * Sends a request to the api for generating a signed document and 
     * signature, both of which will be contained within the returned notary 
     * object. 
     * 
     * @param string $payload payload used to generate signature and signed 
     *                        document
     *
     * @return Notary notary generated from payload
     * @throws ServiceException if api response isn't successful
     */
    public function getNotary($payload)
    {
        $endpoint = $this->_fqdn . '/Security/Notary/Rest/1/SignedPayload';

        $req = new RestfulRequest($endpoint);

        $httpPost = new HttpPost();
        $this->_setHeaders($req);
        $httpPost->setBody($payload);

        $result = $req->sendHttpPost($httpPost);
        $responseArr = Service::parseJson($result);

        return new Notary(
            $responseArr['SignedDocument'],
            $responseArr['Signature'],
            $payload
        );
    }

    /**
     * Sends a request to the api for generating a signed document and 
     * signature, both of which will be contained within the returned notary 
     * object. The notary object can then be used for creating a new 
     * subscription.
     *
     * @param SubscriptionNotaryArguments $args arguments for generating notary 
     *
     * @return Notary notary generated from payload
     * @throws ServiceException if api response isn't successful
     */
    public function getSubscriptionNotary(SubscriptionNotaryArguments $args)
    {
        $vars = array(
            'Amount' => $args->getAmount(),
            'Category' => $args->getCategory(),
            'Channel' => $args->getChannel(),
            'Description' => $args->getDescription(),
            'MerchantTransactionId' => $args->getMerchantTransactionId(),
            'MerchantProductId' => $args->getMerchantProductId(),
            'MerchantPaymentRedirectUrl' => $args->getMerchantRedirectUrl(),
            'MerchantSubscriptionIdList' => $args->getMerchantSubscriptionIdList(),
            'IsPurchaseOnNoActiveSubscription' => $args->isPurchaseOnNoActiveSubscription(),
            'SubscriptionRecurrences' => $args->getSubscriptionRecurrences(),
            'SubscriptionPeriod' => $args->getSubscriptionPeriod(),
            'SubscriptionPeriodAmount' => $args->getSubscriptionPeriodAmount()
        );
        $payload = json_encode($vars);
        return $this->getNotary($payload);
    }

    /**
     * Sends a request to the api for generating a signed document and 
     * signature, both of which will be contained within the returned notary 
     * object. The notary object can then be used for creating a new 
     * transaction.
     *
     * @param TransactionNotaryArguments $args arguments for generating notary 
     *
     * @return Notary notary generated from payload
     * @throws ServiceException if api response isn't successful
     */
    public function getTransactionNotary(TransactionNotaryArguments $args)
    {
        $vars = array(
            'Amount' => $args->getAmount(),
            'Category' => $args->getCategory(),
            'Channel' => $args->getChannel(),
            'Description' => $args->getDescription(),
            'MerchantTransactionId' => $args->getMerchantTransactionId(),
            'MerchantProductId' => $args->getMerchantProductId(),
            'MerchantPaymentRedirectUrl' => $args->getMerchantRedirectUrl()
        );
        $payload = json_encode($vars);
        return $this->getNotary($payload);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
