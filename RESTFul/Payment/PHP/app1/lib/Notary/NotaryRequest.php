<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Notary Library
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
 * @category Notary 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '../../Common/RESTFulRequest.php';
require_once __DIR__ . '/NotaryArguments.php';
require_once __DIR__ . '/Notary.php';


class NotaryRequest extends RESTFulRequest {
    private $_clientId;
    private $_clientSecret;

    private function setHeaders() {
        $this->setHeader('Content-Type', 'application/json');
        $this->setHeader('Accept', 'application/json');
        $this->setHeader('Client_id', $this->_clientId);
        $this->setHeader('Client_secret', $this->_clientSecret);
    }

    public function __construct($url, $clientId, $clientSecret) {
        parent::__construct($url);
        $this->_clientId = $clientId;
        $this->_clientSecret = $clientSecret;
    }

    public function getNotary($payload) {
        $this->setHttpMethod(RESTFulRequest::HTTP_METHOD_POST);
        $this->setHeaders();
        $this->setBody($payload);
        $result = $this->sendRequest();
        $response = $result['response'];
        $responseCode = $result['responseCode'];
        if ($responseCode != 200 && $responseCode != 201) {
            throw new Exception($responseCode . ':' .  $response);
        }
        $jresponse = json_decode($response);
        return new Notary($jresponse->SignedDocument, $jresponse->Signature,
                $payload);
    }

    public function getSubscriptionNotary(SubscriptionNotaryArguments $args) {
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

    public function getTransactionNotary(TransactionNotaryArguments $args) {
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

?>
