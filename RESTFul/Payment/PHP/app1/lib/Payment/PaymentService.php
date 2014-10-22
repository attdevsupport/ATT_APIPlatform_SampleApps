<?php
namespace Att\Api\Payment;

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

require_once __DIR__ . '../../Notary/NotaryService.php';
require_once __DIR__ . '../../Srvc/APIService.php';

use Att\Api\OAuth\OAuthToken;
use Att\Api\Restful\HttpPut;
use Att\Api\Restful\RestfulRequest;
use Att\Api\Notary\Notary;
use Att\Api\Srvc\APIService;
use Att\Api\Srvc\Service;

/**
 * Used to interact with version 3 of the Payment API.
 *
 * @category API
 * @package  Payment
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/Payment
 */
class PaymentService extends APIService
{

    /** 
     * Gets the redirect URL.
     * 
     * @param string  $FQDN    fully qualified domain name
     * @param string  $cid     client id
     * @param Notary  $notary  notary
     * @param boolean $isTrans true if transaction, false if subscription
     *
     * @return string redirect URL
     */
    private static function _getURL(
        $FQDN, $cid, Notary $notary, $isTrans = true
    ) {

        $type = $isTrans ? 'Transactions' : 'Subscriptions';

        $url = $FQDN . '/rest/3/Commerce/Payment/' . $type;
        $signedDoc = $notary->getSignedDocument();
        $signature = $notary->getSignature();
        $url .= '?clientid=' . $cid . '&SignedPaymentDetail=' . $signedDoc 
            . '&Signature=' . $signature;

        return $url;
    }

    /**
     * Internal function used for handling common information requests, such
     * as getting a transaction or subscription status.
     *
     * @param string $url url to send request to
     *
     * @return array api response as an array of key-value pairs
     * @throws ServiceException if api request was not successful
     */
    private function _getInfo($url)
    {
        $req = new RestfulRequest($url);
        $result = $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken())
            ->sendHttpGet();

        return Service::parseJson($result);
    }

    /**
     * Internal function used for sending common transaction operation 
     * statuses, such as refunding a transaction or cancelling a subscription.
     *
     * @param string $rReasonTxt     reason for refunding
     * @param string $rReasonCode    reason code for refunding
     * @param string $transOptStatus transaction operation status 
     *                               (e.g. Refunded). 
     * @param string $url            URL used for sending request
     * 
     * @return string api response
     * @throws ServiceException if api request was not successful
     */
    private function _sendTransOptStatus(
        $rReasonTxt, $rReasonCode, $transOptStatus, $url
    ) {

        $req = new RestfulRequest($url);

        $bodyArr = array(
            'TransactionOperationStatus' => $transOptStatus,
            'RefundReasonCode' => $rReasonCode,
            'RefundReasonText' => $rReasonTxt,
        );
        $httpPut = new HttpPut(json_encode($bodyArr));

        $req->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Content-Type', 'application/json');

        $result = $req->sendHttpPut($httpPut);

        return Service::parseJson($result);
    }

    /**
     * Creates a PaymentService object with the following FQDN and following
     * access token.
     * 
     * @param string     $FQDN  fully qualified domain name 
     * @param OAuthToken $token token to use for authorization
     */
    public function __construct($FQDN, OAuthToken $token = null)
    {
        parent::__construct($FQDN, $token);
    }

    /**
     * Sends an API request for getting transaction status. 
     *
     * For getting status, a type and its value are used, where type can be one
     * of:
     * <ul>
     * <li>TransactionAuthCode</li>
     * <li>TransactionId</li>
     * <li>MerchantTransactionId</li>
     * </ul>
     *
     * @param string $type  type used for getting status
     * @param string $value the value of the specified type
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function getTransactionStatus($type, $value)
    {
        $urlPath = '/rest/3/Commerce/Payment/Transactions/' . $type . '/' 
            . $value;

        $url = $this->getFqdn() . $urlPath;

        return $this->_getInfo($url);
    }

    /**
     * Sends an API request for getting subscription status. 
     *
     * For getting status, a type and its value are used, where type can be one
     * of:
     * <ul>
     * <li>SubscriptionAuthCode</li>
     * <li>MerchantTransactionId</li>
     * <li>SubscriptionId</li>
     * </ul>
     *
     * @param string $type  type used for getting status
     * @param string $value the value of the specified type
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function getSubscriptionStatus($type, $value)
    {
        $urlPath = '/rest/3/Commerce/Payment/Subscriptions/' . $type . '/' 
            . $value;

        $url = $this->getFqdn() . $urlPath;

        return $this->_getInfo($url);
    }

    /**
     * Sends an API request for getting details about a subscription.
     * 
     * @param string $merchantSId merchant subscription id
     * @param string $consumerId  consumer id 
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function getSubscriptionDetails($merchantSId, $consumerId)
    {
        $urlPath =  '/rest/3/Commerce/Payment/Subscriptions/' . $merchantSId 
            . '/Detail/' . $consumerId;

        $url = $this->getFqdn() . $urlPath;

        return $this->_getInfo($url);
    }

    /**
     * Sends an API request for cancelling a subscription.
     * 
     * @param string $subId      subscription id
     * @param string $reasonTxt  reason for cancelling
     * @param int    $reasonCode reason code for cancelling (defaults to 1)
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function cancelSubscription($subId, $reasonTxt, $reasonCode = 1)
    {
        $urlPath = '/rest/3/Commerce/Payment/Transactions/' . $subId;
        $url = $this->getFqdn() . $urlPath; 
            
        $type = 'SubscriptionCancelled';
        return $this->_sendTransOptStatus($reasonTxt, $reasonCode, $type, $url);
    }

    /**
     * Sends an API request for refunding a subscription.
     * 
     * @param string $subId      subscription id
     * @param string $reasonTxt  reason for refunding 
     * @param int    $reasonCode reason code for refunding (defaults to 1)
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function refundSubscription($subId, $reasonTxt, $reasonCode = 1)
    {
        $urlPath = '/rest/3/Commerce/Payment/Transactions/' . $subId;
        $url = $this->getFqdn() . $urlPath;

        $type = 'Refunded';
        return $this->_sendTransOptStatus($reasonTxt, $reasonCode, $type, $url);
    }

    /**
     * Sends an API request for refunding a transaction.
     * 
     * @param string $transId    transaction id
     * @param string $reasonTxt  reason for refunding 
     * @param int    $reasonCode reason code for refunding (defaults to 1)
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function refundTransaction($transId, $reasonTxt, $reasonCode = 1)
    {
        $urlPath = '/rest/3/Commerce/Payment/Transactions/' . $transId;
        $url = $this->getFqdn() . $urlPath;

        $type = 'Refunded';
        return $this->_sendTransOptStatus($reasonTxt, $reasonCode, $type, $url);
    }

    /**
     * Sends an API request for getting information about a notification.
     * 
     * @param string $notificationId notification id
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function getNotificationInfo($notificationId)
    {
        $urlPath = '/rest/3/Commerce/Payment/Notifications/' . $notificationId;
        $url = $this->getFqdn() . $urlPath;

        return $this->_getInfo($url);
    }

    /**
     * Sends an API request for removing the notification from the api server, 
     * thereby causing any future calls to getNotificationInfo() to fail. Also,
     * prevents the api from sending any further notifications. 
     *
     * Unless this method is called, the api will keep sending the same 
     * notification id indefinitely.
     * 
     * @param string $notificationId notification id
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function deleteNotification($notificationId)
    {
        $urlPath = '/rest/3/Commerce/Payment/Notifications/' . $notificationId;
        $url = $this->getFqdn() . $urlPath;

        $req = new RESTFulRequest($url);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_PUT);
        $req->setHeader('Accept', 'application/json');
        $req->addAuthorizationHeader($this->token);

        $result = $req->sendRequest();
        return $this->parseResult($result);
    }
    
    /**
     * Redirects the browser to the consent flow page for creating a new 
     * transaction.
     *
     * @param string $FQDN     fully qualified domain name
     * @param string $clientId client id
     * @param Notary $notary   notary
     *
     * @return void
     */
    public static function newTransaction($FQDN, $clientId, Notary $notary)
    {
        $url = PaymentService::_getURL($FQDN, $clientId, $notary, true);
        header('Location: ' . $url);
    }

    /**
     * Redirects the browser to the consent flow page for creating a new 
     * subscription.
     *
     * @param string $FQDN     fully qualified domain name
     * @param string $clientId client id
     * @param Notary $notary   notary
     *
     * @return void
     */
    public static function newSubscription($FQDN, $clientId, Notary $notary)
    {
        $url = PaymentService::_getURL($FQDN, $clientId, $notary, false);
        header('Location: ' . $url);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
