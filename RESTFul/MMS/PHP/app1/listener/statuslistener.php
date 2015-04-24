<?php
require_once __DIR__ . '/../lib/Util/FileUtil.php';
require __DIR__ . '/../config.php';

use Att\Api\Util\FileUtil;


$postBody = file_get_contents('php://input');

$fname = $statusFile;
$messageInfo = json_decode($postBody, true);
if ($messageInfo != null) {
    $statusarr = FileUtil::loadArray($fname);
    $statusarr[] = $messageInfo;
    while (count($statusarr) > $statusLimit) {
        array_shift($statusarr); 
    }
    FileUtil::saveArray($statusarr, $fname);
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
