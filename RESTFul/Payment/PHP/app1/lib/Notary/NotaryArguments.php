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

class NotaryArguments {
    const CATEGORY_IN_APP_GAMES = 1;
    const CATEGORY_IN_APP_VIRTUAL_GOODS = 2;
    const CATEGORY_IN_APP_OTHER = 3;
    const CATEGORY_APPLICATION_GAMES = 4;
    const CATEGORY_APPLICATION_OTHER = 5;

    private $_amt;
    private $_category;
    private $_channel;
    private $_desc;
    private $_mTransId;
    private $_mProductId;
    private $_mRedirectUrl;

    public function __construct() {
        $this->_amt = "0.00";
        $this->_category = 5;
        $this->_channel = "MOBILE_WEB";
        $this->_desc = "Description";
        $this->_mTransId = 'T' . time();
        $this->_mProductId = "0";
        $this->_mRedirectUrl = "localhost";
    }

    public function setAmount($amt) {
        $this->_amt = $amt;
        return $this;
    }

    public function setCategory($category) {
        $this->_category = $category;
        return $this;
    }

    //    Can only be MOBILE_WEB for now
    public function setChannel($channel) {
        $this->_channel = $channel;
        return $this;
    }

    public function setDescription($desc) {
        $this->_desc = $desc;
        return $this;
    }
    
    public function setMerchantTransactionId($mTransId) {
        $this->_mTransId= $mTransId;
        return $this;
    }

    public function setMerchantProductId($mProductId) {
        $this->_mProductId= $mProductId;
        return $this;
    }

    public function setMerchantRedirectUrl($mRedirectUrl) {
        $this->_mRedirectUrl= $mRedirectUrl;
        return $this;
    }

    public function getAmount() {
        return $this->_amt;
    }

    public function getCategory() {
        return $this->_category;
    }

    public function getChannel() {
        return $this->_channel;
    }

    public function getDescription() {
        return $this->_desc;
    }
    
    public function getMerchantTransactionId() {
        return $this->_mTransId;
    }

    public function getMerchantProductId() {
        return $this->_mProductId;
    }

    public function getMerchantRedirectUrl() {
        return $this->_mRedirectUrl;
    }
}

class TransactionNotaryArguments extends NotaryArguments {
    //wrapper
}

class SubscriptionNotaryArguments extends NotaryArguments {
    private $_idList;

    public function __construct() {
        parent::__construct();
        $this->_idList = 'S' . time();
    }

    public function getMerchantSubscriptionIdList() {
        return $this->_idList;
    }

    public function isPurchaseOnNoActiveSubscription() {
        return false;
    }

    public function getSubscriptionRecurrences() {
        return 99999;
    }

    public function getSubscriptionPeriod() {
        return "MONTHLY";
    }

    public function getSubscriptionPeriodAmount() {
        return 1;
    }
}

?>
