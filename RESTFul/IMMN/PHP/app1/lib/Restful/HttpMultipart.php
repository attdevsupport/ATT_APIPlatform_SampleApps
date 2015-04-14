<?php
namespace Att\Api\Restful;

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

require_once __DIR__ . '/../Util/Util.php';

use Att\Api\Util\Util;
use RuntimeException;

/**
 * This class represents a multipart request, which will be sent using the HTTP
 * verb POST.
 * 
 * @category Network
 * @package  Restful
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @link     http://developer.att.com
 */
class HttpMultipart
{
    /**
     * Raw HTTP data.
     */
    private $_rawoutput;

    /**
     * Multipart boundary.
     */
    private $_boundary;

    /**
     * Startpart of multipart body.
     */
    private $_startpart;

    /**
     * Number of parts
     */
    private $_nParts;
    
    /**
     * Gets content identifier.
     *
     * @return string content identifier
     */
    private function _getContentId()
    {
        if ($this->_nParts == 0) {
            return $this->_startpart;
        }
        
        return 'partid' . $this->_nParts;
    }

    /**
     * Adds a part header (e.g. 'Content-Encoding: binary').
     *
     * @param string $name  name to add
     * @param string $value value to add
     *
     * @return void
     */
    private function _addPartHeader($name, $value)
    {
        $this->_rawoutput .= $name . ': ' . $value . "\r\n"; 
    }

    /**
     * Adds the body for one of the parts.
     * 
     * @param string $body body to add
     *
     * @return void
     */
    private function _addPartBody($body)
    {
        $this->_rawoutput .= "\r\n";
        $this->_rawoutput .= $body;
        $this->_rawoutput .= "\r\n\r\n";
    }

    /**
     * Gets the boundary used for creating the multipart body.
     *
     * @return string boundary used.
     */
    protected function getBoundary()
    {
        return $this->_boundary;
    }

    /**
     * Gets the startpart used for creating the multipart body.
     *
     * @return string startpart used.
     */
    protected function getStartpart()
    {
        return $this->_startpart;
    }

    /** 
     * Create a multipart body object, which can then be set in the POST body.
     */
    public function __construct()
    {  
        $this->_boundary = 'BOUNDARY' . md5(time());
        $this->_startpart = '<startpart>';
        $this->_nParts = 0;
        $this->_rawoutput = '';
    }

    /**
     * Adds a part to this multipart request using the specified headers and
     * body.
     *
     * @param array  $headers an associative array of header strings
     * @param string $body    body of the part
     *
     * @return void
     */
    public function addPart($headers, $body)
    {
        if (!isset($headers['Content-ID'])) {
            $headers['Content-ID'] = $this->_getContentId();
        }

        $this->_nParts++;
        $this->_rawoutput .= '--' . $this->_boundary . "\r\n"; 

        foreach ($headers as $name => $value) {
            $this->_addPartHeader($name, $value);
        }

        $this->_addPartBody($body);
    }

    /**
     * Gets multipart object as a raw string. This string can be set directly
     * into the HTTP POST body.
     *
     * @return string raw multipart output 
     */
    public function getMultipartRaw()
    {
        $output = $this->_rawoutput;
        $output .= '--' . $this->_boundary . '--' . "\r\n";
        return $output;
    }

    /** 
     * Gets the content type of the entire multipart. This method should be 
     * used to set the HTTP header 'Content-Type'.
     *
     * @return string content type
     */
    public function getContentType()
    {
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
     * @param string $fname file location
     *
     * @return void
     */
    public function addFilePart($fname)
    {
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
     * @param string $jsonstr JSON string
     *
     * @return void
     */
    public function addJSONPart($jsonstr) 
    {
        $pheaders = array();
        $pheaders['Content-Type'] = 'application/json';
    
        $this->addPart($pheaders, $jsonstr);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
