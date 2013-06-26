<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Speech Library
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
 * @package   Speech 
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

require_once __DIR__ . '/../Restful/Multipart.php';

/**
 * Used to handle the multipart aspect of a speech request.
 *
 * @category API
 * @package  Speech
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/Speech
 */
class SpeechMultipartBody extends MultipartBody
{

    /**
     * Creates a SpeechMultipartBody object.
     */
    public function __construct()
    {
        parent::__construct();
    }

    /** 
     * Gets the content type of the entire multipart. This method should be 
     * used to set the HTTP header 'Content-Type'.
     *
     * @return string content type.
     */
    public function getContentType()
    {
        $ctype = 'multipart/x-srgs-audio; boundary=' . $this->getBoundary();
        return $ctype;
    }
    
    /**
     * Convenience method for adding dictionary part.
     *
     * @param string $fname file location of dictionary.
     *
     * @return void
     */
    public function addXDictionaryPart($fname)
    {
        $pheaders = array();
        $pheaders['Content-Disposition'] = 'form-data; name="x-dictionary"; ' 
            . 'filename="speech_alpha.pls';
        $pheaders['Content-Type'] = 'application/pls+xml';

        $fileResource = fopen($fname, 'r');
        if (!$fileResource) {
            throw new RuntimeException('Could not open file ' . $fname);
        }
        $str = fread($fileResource, filesize($fname));
        fclose($fileResource);
        $this->addPart($pheaders, $str);   
    }

    /**
     * Convenience method for adding grammar part.
     *
     * @param string $fname file location of grammar part.
     *
     * @return void
     */
    public function addXGrammarPart($fname)
    {
        $pheaders = array();
        $pheaders['Content-Disposition'] = 'form-data; name="x-grammar"';
        $pheaders['Content-Type'] = 'application/srgs+xml';

        $fileResource = fopen($fname, 'r');
        if (!$fileResource) {
            throw new RuntimeException('Could not open file ' . $fname);
        }
        $str = fread($fileResource, filesize($fname));
        fclose($fileResource);

        $this->addPart($pheaders, $str);   
    }

    /**
     * Convenience method for adding file part.
     *
     * @param string $fname file location of file part.
     *
     * @return void
     */
    public function addFilePart($fname)
    {
        // base name
        $bname = basename($fname);

        $cDisposition = 'form-data;'; 
        $cDisposition .= ' name="x-voice";';
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
}

?>
