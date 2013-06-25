<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * IMMN Library
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
 * @package   IMMN 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Used to hold the IMMNHeader response of version 1 of the In-app Messaging 
 * from Mobile Number (IMMN) API.
 *
 * @category API
 * @package  IMMN
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/In-app%20Messaging%20from%20Mobile%20Number
 */
class IMMNHeader
{
    /**
     * Message id.
     *
     * @var string
     */
    private $_messageId;

    /**
     * Origin of message.
     *
     * @var string
     */
    private $_from;

    /**
     * Destination of message.
     *
     * @var string
     */
    private $_to;

    /**
     * Subject of message.
     *
     * @var string
     */
    private $_subject;

    /**
     * Text of message.
     *
     * @var string
     */
    private $_text;

    /**
     * MMS Content of message.
     *
     * @var string
     */
    private $_mmsContent;

    /**
     * Message id.
     *
     * @var string
     */
    private $_received;

    /**
     * If message is favorited.
     *
     * @var string
     */
    private $_favorite;

    /**
     * If message is read.
     *
     * @var string
     */
    private $_read;

    /**
     * Type of message.
     *
     * @var string
     */
    private $_type;

    /**
     * Direction of message.
     *
     * @var string
     */
    private $_direction;

    /**
     * Content name of message.
     *
     * @var string
     */
    private $_contentName;

    /**
     * Content type of message.
     *
     * @var string
     */
    private $_contentType;

    /**
     * Part number of message.
     *
     * @var string
     */
    private $_partNumber;

    /**
     * Creates an IMMNHeader object.
     *
     * @param array $arr an array of strings from which this object will be
     *                   created.
     */
    public function __construct($arr) 
    {
        // TODO: Move to explicit variable names

        $vals = array('MessageId', 'From', 'To', 'Subject', 'Text',
                'MmsContent', 'Received', 'Favorite', 'Read', 'Type', 'Direction',
                'ContentName', 'ContentType', 'PartNumber');
        foreach ($vals as $val) {
            if (!isset($arr[$val])) {
                $arr[$val] = null;
            }
        }

        $this->_messageId = $arr['MessageId'];
        $this->_from = $arr['From'];
        $this->_to = $arr['To'];
        $this->_subject = $arr['Subject'];
        $this->_text = $arr['Text'];
        $this->_mmsContent = $arr['MmsContent'];
        $this->_received = $arr['Received'];
        $this->_favorite = $arr['Favorite'];
        $this->_read = $arr['Read'];
        $this->_type = $arr['Type'];
        $this->_direction = $arr['Direction'];
    }

    /**
     * Gets message header id.
     *
     * @return string message header id
     */
    public function getMessageId()
    {
        return $this->_messageId;
    }

    /**
     * Gets from whom the message header was sent.
     *
     * @return string from 
     */
    public function getFrom() 
    {
        return $this->_from;
    }

    /**
     * Gets to whom the message header was sent.
     *
     * @return string to 
     */
    public function getTo() 
    {
        return $this->_to;
    }

    /**
     * Gets the subject of this message header.
     *
     * @return string subject of this message header 
     */
    public function getSubject()
    {
        return $this->_subject;
    }

    /**
     * Gets the text body of this message header.
     *
     * @return string text body
     */
    public function getText()
    {
        return $this->_text;
    }

    /**
     * Gets the MMS content of this message header.
     *
     * @return string MMS content
     */
    public function getMmsContent()
    {
        return $this->_mmsContent;
    }

    /**
     * Gets whether this message header was received.
     * 
     * @return string whether message header was received
     */
    public function getReceived()
    {
        return $this->_received;
    }

    /**
     * Gets whether this message header is a favorite message header.
     * 
     * @return string whether message header is a favorite message header. 
     */
    public function getFavorite()
    {
        return $this->_favorite;
    }

    /**
     * Gets whether this message header was read.
     * 
     * @return string whether message header was read. 
     */
    public function getRead()
    {
        return $this->_read;
    }

    /**
     * Gets message header's type.
     * 
     * @return string message header's type. 
     */
    public function getType()
    {
        return $this->_type;
    }

    /**
     * Gets direction of this message header.
     * 
     * @return string message header's direction. 
     */
    public function getDirection()
    {
        return $this->_direction;
    }

    /**
     * Convenience method that builds a IMMNHeader object from the specified
     * JSON string.
     *
     * @param string $jsonStr JSON string
     * 
     * @return IMMNHeader IMMNHeader object built using the specified JSON 
     *                    string
     */
    public static function buildFromJSON($jsonStr)
    {
        $arr = json_decode($jsonStr, true);
        return new IMMNHeader($arr); 
    }
}

/**
 * Used to hold a IMMNBody response of version 1 of the In-app Messaging 
 * from Mobile Number (IMMN) API.
 *
 * @category API
 * @package  IMMN
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/In-app%20Messaging%20from%20Mobile%20Number
 */
class IMMNBody
{
    /**
     * Message body content type.
     *
     * @var string
     */
    private $_contentType;

    /**
     * Message body.
     * 
     * @var string
     */
    private $_data;

    /**
     * Creates an IMMBody object with the specified content type and content
     * data.
     *
     * @param string $contentType content type
     * @param string $data        content data
     */
    public function __construct($contentType, $data)
    {
        $this->_contentType = $contentType; 
        $this->_data = $data;
    }

    /**
     * Gets content type.
     *
     * @return string content type
     */
    public function getContentType()
    {
        return $this->_contentType;
    }

    /**
     * Gets content data.
     *
     * @return string content data
     */
    public function getData() 
    {
        return $this->_data;
    }
}

/**
 * Imuttable class used to hold a IMMNBody response of version 1 of the In-app 
 * Messaging from Mobile Number (IMMN) API.
 *
 * @category API
 * @package  IMMN
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/In-app%20Messaging%20from%20Mobile%20Number
 */
class IMMNResponse
{
    /**
     * Array of IMMN headers.
     *
     * @var array
     */
    private $_headers;

    /** 
     * Index cursor used to sent request.
     *
     * @var string
     */
    private $_indexCursor;

    /**
     * Number of IMMN headers contained in this IMMN Response object.
     *
     * @var int
     */
    private $_headerCount;

    /**
     * Creates an IMMNResponse object with the specified headers, index cursor, 
     * and header count.
     *
     * @param array  $headers     array of IMMNHeader objects
     * @param string $indexCursor index cursor
     * @param int    $headerCount header count
     */
    public function __construct($headers, $indexCursor, $headerCount) 
    {
        $this->_headers = $headers;
        $this->_indexCursor = $indexCursor;
        $this->_headerCount = $headerCount;
    }

    /**
     * Gets IMMN headers.
     * 
     * @return array an array of IMMNHeader objects
     */
    public function getHeaders() 
    {
        return $this->_headers;
    }

    /**
     * Gets index cursor.
     *
     * @return string index cursor
     */
    public function getIndexCursor() 
    { 
        return $this->_indexCursor;
    }

    /**
     * Gets header count.
     *
     * @return int header count 
     */
    public function getHeaderCount() 
    {
        return $this->_headerCount;
    }

    /**
     * Convenience method that can be used to build an IMMNResponse object from
     * a JSON string.
     *
     * @param string $jsonStr JSON string 
     *
     * @return IMMNResponse IMMNResponse object built from JSON string
     */
    public static function buildFromJSON($jsonStr) 
    {
        $jsonObj = json_decode($jsonStr);
        $headersList = $jsonObj->MessageHeadersList;
        $indexCursor = $headersList->IndexCursor;
        $headerCount = $headersList->HeaderCount;
        $jheaders = $headersList->Headers;
        $headers = array();
        foreach ($jheaders as $jheader) {
            $header = IMMNHeader::buildFromJSON(json_encode($jheader)); 
            $headers[] = $header;
        }

        return new IMMNResponse($headers, $indexCursor, $headerCount);
    }
}
