<?php
namespace Att\Api\SMS;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/**
 * SMS Library
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
 * @package   SMS 
 * @author    pk9069
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */
require_once __DIR__ . '/SMSMessage.php';

/**
 * Immutable class that holds a Get SMS response.
 *
 * @category API
 * @package  SMS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/SMS
 */
final class GetSMSResponse
{
    private $_numberOfMsgs;

    private $_resourceUrl;

    private $_pendingMsgs;

    private $_smsMessageList;

    public function __construct(
        $numberOfMsgs, $resourceUrl, $pendingMsgs, $smsMessageList
    ) {
        $this->_numberOfMsgs = $numberOfMsgs;

        $this->_resourceUrl = $resourceUrl;

        $this->_pendingMsgs = $pendingMsgs;

        $this->_smsMessageList = $smsMessageList;

    }

    public function getNumberOfMessages()
    {
        return $this->_numberOfMsgs;
    }

    public function getResourceUrl()
    {
        return $this->_resourceUrl;
    }

    public function getNumberOfPendingMessages()
    {
        return $this->_pendingMsgs;
    }

    public function getMessages()
    {
        return $this->_smsMessageList;
    }

    public static function fromArray($arr)
    {
        $inboundList = $arr['InboundSmsMessageList'];

        $numberOfMsgs = $inboundList['NumberOfMessagesInThisBatch'];
        $resourceUrl = $inboundList['ResourceUrl'];
        $pendingMsgs = $inboundList['TotalNumberOfPendingMessages'];

        $smsMsgs = array();
        $inboundSmsMessage = $inboundList['InboundSmsMessage'];
        foreach ($inboundSmsMessage as $inboundMsg) {
            $smsMsgs[] = SMSMessage::fromArray($inboundMsg);
        }

        return new GetSMSResponse(
            $numberOfMsgs, $resourceUrl, $pendingMsgs, $smsMsgs
        );
    }
}

?>
