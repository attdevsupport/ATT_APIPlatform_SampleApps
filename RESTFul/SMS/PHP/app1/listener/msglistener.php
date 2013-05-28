<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../lib/Util/FileUtil.php';

$path = __DIR__;
$fname = $path . '/msgs.db';

$postBody = file_get_contents('php://input');

$asArray = true;
$messageInfo = json_decode($postBody, $asArray);
$msgsarr = FileUtil::loadArray($fname);
// TODO: Put value into config
while (count($msgsarr) > 5) {
    array_shift($msgsarr); 
}
$msgsarr[] = $messageInfo;

FileUtil::saveArray($msgsarr, $fname);

?>
