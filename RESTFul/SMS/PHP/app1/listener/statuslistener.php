<?php

require_once __DIR__ . '/../config.php';
require_once __DIR__ . '/../lib/Util/FileUtil.php';

use Att\Api\Util\FileUtil;

$postBody = file_get_contents('php://input');

$asArray = true;
$messageInfo = json_decode($postBody, $asArray);
$statusarr = FileUtil::loadArray(STATUS_FILE);
$statusarr[] = $messageInfo;
// TODO: Put value into config
while (count($statusarr) > 5) {
    array_shift($statusarr); 
}
FileUtil::saveArray($statusarr, STATUS_FILE);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */
?>
