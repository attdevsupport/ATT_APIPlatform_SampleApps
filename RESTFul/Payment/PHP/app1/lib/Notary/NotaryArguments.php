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

/**
 * Base class used to hold the arguments for generating a signed document and 
 * signature.
 *
 * @category API
 * @package  Notary 
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     http://developer.att.com
 */
abstract class NotaryArguments
{
    const CATEGORY_IN_APP_GAMES = 1;
    const CATEGORY_IN_APP_VIRTUAL_GOODS = 2;
    const CATEGORY_IN_APP_OTHER = 3;
    const CATEGORY_APPLICATION_GAMES = 4;
    const CATEGORY_APPLICATION_OTHER = 5;

    /**
     * Price.
     */
    private $_amt;

    /**
     * Payment category.
     */
    private $_category;

    /**
     * Payment channel.
     */
    private $_channel;

    /**
     * Payment description.
     */
    private $_desc;

    /**
     * Merchant transaction id.
     */
    private $_mTransId;

    /**
     * Merchant product id.
     */
    private $_mProductId;

    /**
     * Merchant redirect url.
     */
    private $_mRedirectUrl;

    /**
     * Creates a NotaryArguments object with the following default values:
     * <ul>
     * <li>Amount: 0.00</li>
     * <li>Category: CATEGORY_APPLICATION_OTHER</li>
     * <li>Channel: MOBILE_WEB</li>
     * <li>Description: Description</li>
     * <li>Merchant Transaction Id: T + current time</li>
     * <li>Merchant Product Id: 0</li>
     * <li>Merchant Redirect URL: localhost</li>
     * </ul>
     */
    public function __construct()
    {
        $this->_amt = "0.00";
        $this->_category = 5;
        $this->_channel = "MOBILE_WEB";
        $this->_desc = "Description";
        $this->_mTransId = 'T' . time();
        $this->_mProductId = "0";
        $this->_mRedirectUrl = "localhost";
    }

    /**
     * Sets the amount.
     * 
     * @param string $amt amount
     *
     * @return NotaryArguments a reference to this (used for method chaining)
     */
    public function setAmount($amt)
    {
        $this->_amt = $amt;
        return $this;
    }

    /**
     * Sets the category. 
     * 
     * The following options are currently supported.
     * <ul>
     * <li>CATEGORY_IN_APP_GAMES</li>
     * <li>CATEGORY_IN_APP_VIRTUAL_GOODS</li>
     * <li>CATEGORY_IN_APP_OTHER</li>
     * <li>CATEGORY_APPLICATION_GAMES</li>
     * <li>CATEGORY_APPLICATION_OTHER</li>
     * </ul>
     *
     * @param const $category category
     *
     * @return NotaryArguments a reference to this (used for method chaining)
     */
    public function setCategory($category)
    {
        $this->_category = $category;
        return $this;
    }

    /**
     * Sets the channel. Only MOBILE_WEB is currently supported.
     *
     * @param string $channel channel
     *
     * @return NotaryArguments a reference to this (used for method chaining)
     */
    public function setChannel($channel)
    {
        $this->_channel = $channel;
        return $this;
    }

    /**
     * Sets description.
     * 
     * @param string $desc description
     *
     * @return NotaryArguments a reference to this (used for method chaining)
     */
    public function setDescription($desc)
    {
        $this->_desc = $desc;
        return $this;
    }
    
    /**
     * Sets the merchant transaction id.
     *
     * @param string $mTransId merchant transaction id
     *
     * @return NotaryArguments a reference to this (used for method chaining)
     */
    public function setMerchantTransactionId($mTransId)
    {
        $this->_mTransId= $mTransId;
        return $this;
    }

    /**
     * Sets the merchant product id.
     *
     * @param string $mProductId merchant product id
     *
     * @return NotaryArguments a reference to this (used for method chaining)
     */
    public function setMerchantProductId($mProductId)
    {
        $this->_mProductId= $mProductId;
        return $this;
    }

    /**
     * Sets the merchant redirect URL.
     *
     * @param string $mRedirectUrl merchant redirect URL
     *
     * @return NotaryArguments a reference to this (used for method chaining)
     */
    public function setMerchantRedirectUrl($mRedirectUrl)
    {
        $this->_mRedirectUrl= $mRedirectUrl;
        return $this;
    }

    /**
     * Gets amount.
     *
     * @return string amount
     * @return NotaryArguments a reference to this (used for method chaining)
     */
    public function getAmount()
    {
        return $this->_amt;
    }

    /**
     * Gets category.
     *
     * @return string category.
     */
    public function getCategory()
    {
        return $this->_category;
    }

    /**
     * Gets channel.
     *
     * @return string channel
     */
    public function getChannel()
    {
        return $this->_channel;
    }

    /**
     * Gets description.
     *
     * @return string description
     */
    public function getDescription()
    {
        return $this->_desc;
    }
    
    /** 
     * Gets merchant transaction id.
     *
     * @return string merchant transaction id
     */
    public function getMerchantTransactionId()
    {
        return $this->_mTransId;
    }

    /**
     * Gets merchant product id.
     *
     * @return string merchant product id
     */
    public function getMerchantProductId()
    {
        return $this->_mProductId;
    }

    /**
     * Gets merchant redirect URL
     *
     * @return string merchant redirect URL
     */
    public function getMerchantRedirectUrl()
    {
        return $this->_mRedirectUrl;
    }
}

/**
 * Class used to hold the arguments for generating a signed document and 
 * signature to use with payment transactions.
 *
 * @category API
 * @package  Notary 
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     http://developer.att.com
 */
class TransactionNotaryArguments extends NotaryArguments
{
    /* currently is just a wrapper */
}

/**
 * Class used to hold the arguments for generating a signed document and 
 * signature to use with payment subscriptions.
 *
 * @category API
 * @package  Notary 
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     http://developer.att.com
 */
class SubscriptionNotaryArguments extends NotaryArguments
{
    /**
     * Generated merchant subscription id list.
     */
    private $_idList;

    /** 
     * Creates a SubscriptionNotaryArguments object.
     * 
     * Merchant subscription id list is set to P + current time.
     */
    public function __construct()
    {
        parent::__construct();
        $this->_idList = 'P' . time();
    }

    /**
     * Gets the merchant subscription id list.
     *
     * @return string id list
     */
    public function getMerchantSubscriptionIdList()
    {
        return $this->_idList;
    }

    /**
     * Gets whether the purchase has no active subscription.
     * 
     * @return boolean true if no active subscription, false otherwise
     */
    public function isPurchaseOnNoActiveSubscription()
    {
        return false;
    }

    /**
     * Gets the number of subscription recurrances.
     *
     * @return int subscription reccurences
     */
    public function getSubscriptionRecurrences()
    {
        return 99999;
    }

    /**
     * Gets subscription period.
     * 
     * @return string subscription period.
     */
    public function getSubscriptionPeriod()
    {
        return "MONTHLY";
    }

    /**
     * Gets how many times to charge during subscription period.
     *
     * @return int subscription period amount
     */
    public function getSubscriptionPeriodAmount()
    {
        return 1;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
