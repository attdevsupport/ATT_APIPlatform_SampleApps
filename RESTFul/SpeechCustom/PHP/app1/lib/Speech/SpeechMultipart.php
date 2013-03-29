<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../Common/Multipart.php';

class SpeechMultipart extends Multipart {
    public function __construct() {
        parent::__construct();
    }

    /** 
     * Gets the content type of the entire multipart. This method should be 
     * used to set the HTTP header 'Content-Type'.
     */
    public function getContentType() {
        $ctype = 'multipart/x-srgs-audio; boundary=' . $this->getBoundary();
        return $ctype;
    }
    
    public function addXDictionaryPart($fname) {
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

    public function addXGrammarPart($fname) {
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

    public function addFilePart($fname) {
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
