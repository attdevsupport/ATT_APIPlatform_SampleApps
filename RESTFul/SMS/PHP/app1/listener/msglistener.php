<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

require_once __DIR__ . '/../config.php';
require_once __DIR__ . '/../lib/Util/FileUtil.php';

use Att\Api\Util\FileUtil;

$postBody = file_get_contents('php://input');
$messageInfo = json_decode($postBody, true);
$msgsarr = FileUtil::loadArray(MSGS_FILE);
// TODO: Put value into config
while (count($msgsarr) > 5) {
    array_shift($msgsarr); 
}
$msgsarr[] = $messageInfo;

FileUtil::saveArray($msgsarr, MSGS_FILE);

?>
