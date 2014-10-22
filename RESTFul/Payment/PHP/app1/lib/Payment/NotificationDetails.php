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

use \SimpleXMLElement;

/**
 * Immutable class that holds a Notification Details response.
 *
 * @category API
 * @package  Payment
 * @author   mp748d
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com
 */
final class NotificationDetails 
{
    /**
     * Notification type.
     *
     * @var string
     */
    private $_type;

    /**
     * Notification timestamp.
     *
     * @var string
     */
    private $_timestamp;

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
     * Subscriber's previous 10 digit MSISDN.
     *
     * @var string|null
     */
    private $_oldMinIdentifier;
    
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
     * Effective timestamp.
     *
     * @var string
     */
    private $_effective;
    
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
     * Gets the old min identifier, or null if none.
     *
     * @return string|null min identifier or null
     */
    public function getOldMinIdentifier() 
    {
        return $this->_oldMinIdentifier;
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
     * Gets notification type.
     *
     * @return string notification type
     */
    public function getNotificationType() 
    {
        return $this->_type;
    }

    /**
     * Gets notification timestamp.
     *
     * @return string notification timestamp
     */
    public function getTimestamp() 
    {
        return $this->_timestamp;
    }

    /**
     * Gets effective timestamp.
     *
     * @return string effective timestamp
     */
    public function getEffective() 
    {
        return $this->_effective;
    }
    
    /**
     * Disallow instances via default constructor.
     */
    private function __construct() 
    {
        $this->_effective = null;
        $this->_oldMinIdentifier = null;
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
            // attributes
            $attrs = $xmlobj->attributes();
            $details->_type = $attrs['type']->__toString();
            $details->_timestamp = $attrs['timestamp']->__toString();
            if (isset($attrs['effective'])) {
                $details->_effective = $attrs['effective']->__toString();
            }

            // child values
            $details->_networkOperatorId = $xmlobj->networkOperatorId->__toString();
            $details->_ownerIdentifier   = $xmlobj->ownerIdentifier->__toString();
            $details->_purchaseDate      = $xmlobj->purchaseDate->__toString();
            $details->_productIdentifier = $xmlobj->productIdentifier->__toString();
            $details->_purchaseActivityIdentifier = $xmlobj->purchaseActivityIdentifier->__toString();
            $details->_instanceIdentifier = $xmlobj->instanceIdentifier->__toString();
            $details->_minIdentifier     = $xmlobj->minIdentifier->__toString();
            if (property_exists($xmlobj, 'oldMinIdentifier')) {
                $details->_oldMinIdentifier = $xmlobj->oldMinIdentifier->__toString();
            }
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
