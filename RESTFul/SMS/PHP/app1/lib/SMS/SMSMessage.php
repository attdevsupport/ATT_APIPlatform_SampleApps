<?php
namespace Att\Api\SMS;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * SMS Library
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2014. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category  API
 * @package   SMS 
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class that holds an SMS message.
 *
 * @category API
 * @package  SMS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/SMS
 */
final class SMSMessage
{
    private $_msgId;

    private $_msg;

    private $_senderAddr;


    public function __construct($msgId, $msg, $senderAddr)
    {
        $this->_msgId = $msgId;
        $this->_msg = $msg;
        $this->_senderAddr = $senderAddr;
    }

    public function getMessageId()
    {
        return $this->_msgId;
    }
    public function getMessage()
    {
        return $this->_msg;
    }
    public function getSenderAddress()
    {
        return $this->_senderAddr;
    }

    public static function fromArray($arr)
    {
        $msgId = $arr['MessageId'];
        $msg = $arr['Message'];
        $senderAddr = $arr['SenderAddress'];

        return new SMSMessage($msgId, $msg, $senderAddr);
    }
}

?>
