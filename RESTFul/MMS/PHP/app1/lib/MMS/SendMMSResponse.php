<?php
namespace Att\Api\MMS;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * MMS Library
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
 * @package   MMS 
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class that holds a send MMS response.
 *
 * @category API
 * @package  MMS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/MMS
 */
final class SendMMSResponse
{
    private $_messageId;

    private $_resourceUrl;

    public function __construct($messageId, $resourceUrl)
    {
        $this->_messageId = $messageId;
        $this->_resourceUrl = $resourceUrl;
    }

    public function getMessageId()
    {
        return $this->_messageId;
    }

    public function getResourceUrl()
    {
        return $this->_resourceUrl;
    }

    public static function fromArray($arr)
    {
        $outboundResponse = $arr['outboundMessageResponse'];

        // TODO: throw exception if required field isn't set
        $msgId = $outboundResponse['messageId'];
        
        $resourceUrl = null;
        if (isset($outboundResponse['resourceReference'])) {
            $resourceRef = $outboundResponse['resourceReference'];

            if (isset($resourceRef['resourceURL']))
                $resourceUrl = $resourceRef['resourceURL'];
        }

        return new SendMMSResponse($msgId, $resourceUrl);
    }
}
