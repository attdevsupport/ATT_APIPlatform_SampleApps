<?php

namespace Att\Api\Payment;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

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
 * @author    mp748d
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

use \SimpleXMLElement;

/**
 * Immutable class that holds a Notification Details response.
 *
 * @category API
 * @package  Payment
 * @author   mp748d
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com
 */
final class NotificationDetails 
{
    /**
     * Network operator Id.
     *
     * @var string
     */
    private $_networkOperatorId;
    
    /**
     * Owner identifier, AT&T MAG ID for the scubscriber.
     *
     * @var string
     */
    private $_ownerIdentifier;
    
    /**
     * Purchase date.
     *
     * @var string
     */
    private $_purchaseDate;
    
    /**
     * Product identifier.
     *
     * @var string
     */
    private $_productIdentifier;
    
    /**
     * Purchase activity identifier.
     *
     * @var string
     */
    private $_purchaseActivityIdentifier;
    
    /**
     * Instance identifier.
     *
     * @var string
     */
    private $_instanceIdentifier;
    
    /**
     * Subscriber's 10 digit MSISDN.
     *
     * @var string
     */
    private $_minIdentifier;
    
    /**
     * sequence number, a sequential, unique (per CP), positive number that identifies each message.
     *
     * @var string
     */
    private $_sequenceNumber;
    
    /**
     * Reason code for revocation.
     *
     * @var string
     */
    private $_reasonCode;
    
    /**
     * Reason message.
     *
     * @var string
     */
    private $_reasonMessage;
    
    /**
     * Vendor purchase identifier.
     *
     * @var string
     */
    private $_vendorPurchaseIdentifier;
    
    /**
      * Gets the network operator ID.
      *
      * @return string network operator ID
      */
    public function getNetworkOperatorId() 
    {
        return $this->_networkOperatorId;
    }
    
    /**
     * Gets the owner identifier.
     *
     * @return string owner identifier
     */
    public function getOwnerIdentifier() 
    {
        return $this->_ownerIdentifier;
    }
    
    /**
     * Gets the purchase date.
     *
     * @return string purchase date
     */
    public function getPurchaseDate() 
    {
        return $this->_purchaseDate;
    }
    
    /**
     * Gets the product identifier.
     *
     * @return string product identifier
     */
    public function getProductIdentifier() 
    {
        return $this->_productIdentifier;
    }
    
    /**
     * Gets the purchase activity identifier.
     *
     * @return string purchase activity identifier
     */
    public function getPurchaseActivityIdentifier() 
    {
        return $this->_purchaseActivityIdentifier;
    }
    
    /**
     * Gets the instance identifier.
     *
     * @return string instance identifier
     */
    public function getInstanceIdentifier() 
    {
        return $this->_instanceIdentifier;
    }
    
    /**
     * Gets the min identifier.
     *
     * @return string min identifier
     */
    public function getMinIdentifier() 
    {
        return $this->_minIdentifier;
    }
    
    /**
     * Gets the sequence number.
     *
     * @return string purchase date
     */
    public function getSequenceNumber() 
    {
        return $this->_sequenceNumber;
    }
    
    /**
     * Gets the reason code.
     *
     * @return string reason code
     */
    public function getReasonCode() 
    {
        return $this->_reasonCode;
    }
    
    /**
     * Gets the reason message.
     *
     * @return string reason message
     */
    public function getReasonMessage() 
    {
        return $this->_reasonMessage;
    }
    
    /**
     * Gets the vendor purchase identifier.
     *
     * @return string vendor purchase identifier
     */
    public function getVendorPurchaseIdentifier() 
    {
        return $this->_vendorPurchaseIdentifier;
    }
    
    /**
     * Disallow instances via default constructor.
     */
    private function __construct() 
    {
    }

    /**
     * Creates a NotificationDetails object from the specified XML.
     *
     * @param string $xml to use for creating an NotificationDetails object
     * 
     * @return NotificationDetails NotificationDetails object
     * @throws ServiceException if xml contains unexpected values
     */
    public static function fromXml($xml) 
    {
        $details = new NotificationDetails();
        
        $xmlobj = new SimpleXMLElement($xml);
        try {
            $details->_networkOperatorId = $xmlobj->networkOperatorId->__toString();
            $details->_ownerIdentifier   = $xmlobj->ownerIdentifier->__toString();
            $details->_purchaseDate      = $xmlobj->purchaseDate->__toString();
            $details->_productIdentifier = $xmlobj->productIdentifier->__toString();
            $details->_purchaseActivityIdentifier = $xmlobj->purchaseActivityIdentifier->__toString();
            $details->_instanceIdentifier = $xmlobj->instanceIdentifier->__toString();
            $details->_minIdentifier     = $xmlobj->minIdentifier->__toString();
            $details->_sequenceNumber    = $xmlobj->sequenceNumber->__toString();
            $details->_reasonCode        = $xmlobj->reasonCode->__toString();
            $details->_reasonMessage     = $xmlobj->reasonMessage->__toString();
            $details->_vendorPurchaseIdentifier = $xmlobj->vendorPurchaseIdentifier->__toString();
        } catch (XMLException $xmlexception) {
                throw new ServiceException($xmlexception);
        }
        return $details;
    }
}

?>
