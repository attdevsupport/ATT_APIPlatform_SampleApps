<?php
namespace Att\Api\Speech;

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

require_once __DIR__ . '/../Restful/HttpMultipart.php';

use Att\Api\Util\Util;
use Att\Api\Restful\HttpMultipart;
use RuntimeException;

/**
 * Used to handle the multipart aspect of a speech request.
 *
 * @category API
 * @package  Speech
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/Speech
 */
class SpeechMultipartBody extends HttpMultipart
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
    public function addXGrammarPart($fname, $nameParam="x-grammar")
    {
        $pheaders = array();
        $pheaders['Content-Disposition'] = 'form-data; name="'. $nameParam . '"';
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
