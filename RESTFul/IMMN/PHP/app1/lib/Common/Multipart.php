<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Common Library
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
 * @category RESTFul 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/../Util/Util.php';

/**
 * In a multiepart request, this class represents the POST body of that
 * multipart request.
 * 
 * @package Common 
 */
class Multipart {
    // raw HTTP data
    private $_rawoutput;

    private $_boundary;
    private $_startpart;

    // number of parts
    private $_nParts;
    
    /**
     * Gets content identifier.
     *
     * @return string content identifier
     */
    private function getContentId() {
        if ($this->_nParts == 0) {
            return $this->_startpart;
        }
        
        return 'partid' . $this->_nParts;
    }
    /**
     * Adds a part header (e.g. 'Content-Encoding: binary').
     *
     * @param $name string name to add
     * @param $value value to add
     */
    private function addPartHeader($name, $value) {
       $this->_rawoutput .= $name . ': ' . $value . "\r\n"; 
    }

    /**
     * Adds the body of this part.
     * 
     * @param $body string body to add
     */
    private function addPartBody($body) {
        $this->_rawoutput .= "\r\n";
        $this->_rawoutput .= $body;
        $this->_rawoutput .= "\r\n\r\n";
    }

    /** 
     * Create a multipart object, which can then be set in the POST body.
     */
    public function __construct() {  
        $this->_boundary = 'BOUNDARY' . md5(time());
        $this->_startpart = '<startpart>';
        $this->_nParts = 0;
        $this->_rawoutput = '';
    }

    /**
     * Adds a part to this multipart request using the specified headers and
     * body.
     *
     * @param $headers array an array of header strings
     * @param $body string body of the part
     */
    public function addPart($headers, $body) {
        if (!isset($headers['Content-ID'])) {
            $headers['Content-ID'] = $this->getContentId();
        }

        $this->_nParts++;
        $this->_rawoutput .= '--' . $this->_boundary . "\r\n"; 

        foreach ($headers as $name => $value) {
            $this->addPartHeader($name, $value);
        }

        $this->addPartBody($body);
    }

    /**
     * Gets multipart object as a raw string. This string can be set directly
     * into the HTTP POST body.
     *
     * @return string raw multipart output 
     */
    public function getMultipartRaw() {
        $output = $this->_rawoutput;
        $output .= '--' . $this->_boundary . '--' . "\r\n";
        return $output;
    }

    /** 
     * Gets the content type of the entire multipart. This method should be 
     * used to set the HTTP header 'Content-Type'.
     */
    public function getContentType() {
            $contentType = 'multipart/related;'; 
            $contentType .= ' start="' . $this->_startpart . '";'; 
            $contentType .= ' boundary="' . $this->_boundary . '";'; 

            //TODO: move away from hardcoding type
            $contentType .= ' type="application/json"'; 

            return $contentType;
    }

    /** 
     * Convenience method for adding a file to this multipart object.
     *
     * @param string file location
     */
    public function addFilePart($fname) {
        // base name
        $bname = basename($fname);

        $partName = 'file' . $this->_nParts;
        $cDisposition = 'attachment;'; 
        $cDisposition .= ' name="' . $partName . '";';
        $cDisposition .= ' filename="' . $bname . '"';

        // part headers
        $pheaders = array();
        $pheaders['Content-Disposition'] = $cDisposition;
        $pheaders['Content-Type'] = Util::getFileMIMEType($fname);
        $pheaders['Content-Transfer-Encoding'] = 'binary';

        $fileResource = fopen($fname, 'r');
        if (!$fileResource) {
            throw new RuntimeException('Could not open file ' . $fname);
        }
        $fileBinary = fread($fileResource, filesize($fname));
        fclose($fileResource);

        $this->addPart($pheaders, $fileBinary);
    }

    /** 
     * Convenience method for adding a JSON part to this multipart object.
     *
     * @param $jsonstr string JSON string
     */
    public function addJSONPart($jsonstr) {
        $pheaders = array();
        $pheaders['Content-Type'] = 'application/json';
    
        $this->addPart($pheaders, $jsonstr);
    }
}
?>
