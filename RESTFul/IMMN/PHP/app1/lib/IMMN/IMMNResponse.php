<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * IMMN API Library
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
 * @category IMMNAPI 
 * @package IMMN 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

/**
 * Holds a single message header.
 * 
 * @package IMMN
 */
class IMMNHeader {
    private $_messageId;
    private $_from;
    private $_to;
    private $_subject;
    private $_text;
    private $_mmsContent;
    private $_received;
    private $_favorite;
    private $_read;
    private $_type;
    private $_direction;
    private $_contentName;
    private $_contentType;
    private $_partNumber;

    /**
     * Creates an IMMNHeader object.
     *
     * @param $arr array an array of strings from which this object will be
     * created.
     */
    public function __construct($arr) {
        $vals = array('MessageId', 'From', 'To', 'Subject', 'Text',
                'MmsContent', 'Received', 'Favorite', 'Read', 'Type', 'Direction',
                'ContentName', 'ContentType', 'PartNumber');
        foreach ($vals as $val) {
            if (!isset($arr[$val])) {
                $arr[$val] = NULL;
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
    public function getMessageId() {
        return $this->_messageId;
    }

    /**
     * Gets from whom the message header was sent.
     *
     * @return string from 
     */
    public function getFrom() {
        return $this->_from;
    }

    /**
     * Gets to whom the message header was sent.
     *
     * @return string to 
     */
    public function getTo() {
        return $this->_to;
    }

    /**
     * Gets the subject of this message header.
     *
     * @return string subject of this message header 
     */
    public function getSubject() {
        return $this->_subject;
    }

    /**
     * Gets the text body of this message header.
     *
     * @return string text body
     */
    public function getText() {
        return $this->_text;
    }

    /**
     * Gets the MMS content of this message header.
     *
     * @return string MMS content
     */
    public function getMmsContent() {
        return $this->_mmsContent;
    }

    /**
     * Gets whether this message header was received.
     * 
     * @return string whether message header was received
     */
    public function getReceived() {
        return $this->_received;
    }

    /**
     * Gets whether this message header is a favorite message header.
     * 
     * @return string whether message header is a favorite message header. 
     */
    public function getFavorite() {
        return $this->_favorite;
    }

    /**
     * Gets whether this message header was read.
     * 
     * @return string whether message header was read. 
     */
    public function getRead() {
        return $this->_read;
    }

    /**
     * Gets message header's type.
     * 
     * @return string message header's type. 
     */
    public function getType() {
        return $this->_type;
    }

    /**
     * Gets direction of this message header.
     * 
     * @return string message header's direction. 
     */
    public function getDirection() {
        return $this->_direction;
    }

    /**
     * Convenience method that builds a IMMNHeader object from the specified
     * JSON string.
     *
     * @param $jsonStr string JSON string
     * @return IMMNHeader IMMNHeader object built using the specified JSON 
     * string
     */
    public static function buildFromJSON($jsonStr) {
        $arr = json_decode($jsonStr, true);
        return new IMMNHeader($arr); 
    }
}

/**
 * Holds a single message body.
 * 
 * @package IMMN
 */
class IMMNBody {
    private $_contentType;
    private $_data;

    /**
     * Creates an IMMBody object with the specified content type and content
     * data.
     *
     * @param $contentType string content type
     * @param $data string content data
     */
    public function __construct($contentType, $data) {
        $this->_contentType = $contentType; 
        $this->_data = $data;
    }

    /**
     * Gets content type.
     *
     * @return string content type
     */
    public function getContentType() {
        return $this->_contentType;
    }

    /**
     * Gets content data.
     *
     * @return string content data
     */
    public function getData() {
        return $this->_data;
    }
}

/**
 * Holds IMMN response variables returned by AT&T's IMMN API.
 *
 * @package IMMN
 */
class IMMNResponse {
    private $_headers;
    private $_indexCursor;
    private $_headerCount;

    /**
     * Creates an IMMNResponse object with the specified headers, indexCursor, 
     * and headerCount.
     *
     * @param $headers array an array of IMMNHeader objects
     * @param $indexCursor string index cursor
     * @param $headerCount int header count
     */
    public function __construct($headers, $indexCursor, $headerCount) {
        $this->_headers = $headers;
        $this->_indexCursor = $indexCursor;
        $this->_headerCount = $headerCount;
    }

    /**
     * Gets IMMN headers.
     * 
     * @return array an array of IMMNHeader objects
     */
    public function getHeaders() {
        return $this->_headers;
    }

    /**
     * Gets index cursor.
     *
     * @return string index cursor
     */
    public function getIndexCursor() { 
        return $this->_indexCursor;
    }

    /**
     * Gets header count.
     *
     * @return int header count 
     */
    public function getHeaderCount() {
        return $this->_headerCount;
    }

    /**
     * Convenience method that can be used to build an IMMNResponse object from
     * a JSON string.
     *
     * @param $jsonStr string JSON string 
     * @return IMMNResponse IMMNResponse object built from JSON string
     */
    public static function buildFromJSON($jsonStr) {
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
