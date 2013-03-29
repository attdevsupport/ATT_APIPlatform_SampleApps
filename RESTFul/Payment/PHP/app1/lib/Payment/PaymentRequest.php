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
 * @category Payment 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '../../Common/RESTFulRequest.php';
require_once __DIR__ . '../../Notary/NotaryRequest.php';
require_once __DIR__ . '../../OAuth/OAuthToken.php';
require_once __DIR__ . '../../OAuth/OAuthTokenRequest.php';

class PaymentRequest extends RESTFulRequest {
    private $_token;

    private static function getURL($fqdn, $cid, Notary $notary, 
            $isTrans = true) {
        $type = $isTrans ? 'Transactions' : 'Subscriptions';

        $url = $fqdn . '/rest/3/Commerce/Payment/' . $type;
        $signedDoc = $notary->getSignedDocument();
        $signature = $notary->getSignature();
        $url .= '?clientid=' . $cid . '&SignedPaymentDetail=' . $signedDoc 
            . '&Signature=' . $signature;

        return $url;
    }

    private function getInfo() {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
        $this->setHeader('Accept', 'application/json');
        $this->addAuthorizationHeader($this->_token);

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        return json_decode($response, true);
    }

    private function sendTransOptStatus($rReasonTxt, $rReasonCode, 
            $transOptStatus) {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_PUT);
        $this->setHeader('Accept', 'application/json');
        $this->setHeader('Content-Type', 'application/json');
        $this->addAuthorizationHeader($this->_token);

        $bodyArr = array(
                'TransactionOperationStatus' => $transOptStatus,
                'RefundReasonCode' => $rReasonCode,
                'RefundReasonText' => $rReasonTxt,
                );

        $this->setPutData(json_encode($bodyArr));
        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        return json_decode($response, true);
    }

    /**
     * Creates a PaymentRequest object with the following url and following
     * access token.
     * 
     * @param $url URL to send requests to
     * @param $token OAuthToken token to use for authorization
     */
    public function __construct($url, OAuthToken $token = NULL) {
        parent::__construct($url);
        $this->_token = $token;
    }

    public function getTransactionStatus() {
        return $this->getInfo();
    }

    public function getSubscriptionStatus() {
        return $this->getInfo();
    }

    public function getSubscriptionDetails() {
        return $this->getInfo();
    }

    public function cancelSubscription($reasonTxt, $reasonCode = 1) {
        $type = 'SubscriptionCancelled';
        return $this->sendTransOptStatus($reasonTxt, $reasonCode, $type);
    }

    public function refundSubscription($reasonTxt, $reasonCode = 1) {
        return $this->sendTransOptStatus($reasonTxt, $reasonCode, 'Refunded');
    }

    public function refundTransaction($reasonTxt, $reasonCode = 1) {
        return $this->sendTransOptStatus($reasonTxt, $reasonCode, 'Refunded');
    }

    public function getNotificationInfo() {
        return $this->getInfo();
    }

    public function deleteNotification() {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_PUT);
        $this->setHeader('Accept', 'application/json');
        $this->addAuthorizationHeader($this->_token);

        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        return json_decode($response, true);
    }

    public static function newTransaction($fqdn, $clientId, Notary $notary) {
        $url = PaymentRequest::getURL($fqdn, $clientId, $notary, true);
        header('Location: ' . $url);
    }

    public static function newSubscription($fqdn, $clientId, Notary $notary) {
        $url = PaymentRequest::getURL($fqdn, $clientId, $notary, false);
        header('Location: ' . $url);
    }
}
?>
