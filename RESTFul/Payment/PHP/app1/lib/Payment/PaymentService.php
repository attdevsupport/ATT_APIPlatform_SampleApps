<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Payment Library
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
 * @package   Payment
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '../../Notary/NotaryService.php';
require_once __DIR__ . '../../Srvc/APIService.php';

/**
 * Used to interact with version 3 of the Payment API.
 *
 * @category API
 * @package  Payment
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
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
        $req = new RESTFulRequest($url);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $req->setHeader('Accept', 'application/json');
        $req->addAuthorizationHeader($this->token);

        $result = $req->sendRequest();
        return $this->parseResult($result);
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

        $req = new RESTFulRequest($url);
        $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_PUT);
        $req->setHeader('Accept', 'application/json');
        $req->setHeader('Content-Type', 'application/json');
        $req->addAuthorizationHeader($this->token);

        $bodyArr = array(
                'TransactionOperationStatus' => $transOptStatus,
                'RefundReasonCode' => $rReasonCode,
                'RefundReasonText' => $rReasonTxt,
                );

        $req->setPutData(json_encode($bodyArr));
        $result = $req->sendRequest();
        return $this->parseResult($result);
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

        $url = $this->FQDN . $urlPath;

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

        $url = $this->FQDN . $urlPath;

        return $this->_getInfo($url);
    }

    /**
     * Sends an API request for getting details about a subscription.
     * 
     * @param string $merchantSID merchant subscription id
     * @param string $consumerId  consumer id 
     *
     * @return array api response
     * @throws ServiceException if api request was not successful
     */
    public function getSubscriptionDetails($merchantSID, $consumerId)
    {
        $urlPath =  '/rest/3/Commerce/Payment/Subscriptions/' . $merchantSID 
            . '/Detail/' . $consumerId;

        $url = $this->FQDN . $urlPath;

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
        $url = $this->FQDN . $urlPath; 
            
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
        $url = $this->FQDN . $urlPath;

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
        $url = $this->FQDN . $urlPath;

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
        $url = $this->FQDN . $urlPath;

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
        $url = $this->FQDN . $urlPath;

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
?>
