<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */
require_once __DIR__ . '/../lib/Util/FileUtil.php';

use Att\Api\Util\FileUtil;

$path = __DIR__;
$fname = $path . '/status.db';

$postBody = file_get_contents('php://input');

$asArray = true;
$messageInfo = json_decode($postBody, $asArray);
$statusarr = FileUtil::loadArray($fname);
$statusarr[] = $messageInfo;
// TODO: Put value into config
while (count($statusarr) > 5) {
    array_shift($statusarr); 
}
FileUtil::saveArray($statusarr, $fname);

?>
