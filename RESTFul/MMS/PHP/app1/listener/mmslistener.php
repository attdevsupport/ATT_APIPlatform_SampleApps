<?php
require __DIR__ . '/../config.php';
require_once __DIR__ . '/../lib/Util/FileUtil.php';
require_once __DIR__ . '/../lib/Util/Util.php';


use Att\Api\Util\FileUtil;
use Att\Api\Util\Util;

$postBody = file_get_contents('php://input');
if (strpos($postBody, "<SenderAddress>tel:+") == 0) {
    exit();
}

if (!is_dir($imagesPath)) {
    mkdir($imagesPath);
}
$messages = FileUtil::loadArray($imagesDbPath);

$message = array();
preg_match("@<SenderAddress>tel:(.*)</SenderAddress>@i", $postBody ,$matches);
$addr = $matches[1];
$message["address"] = preg_replace('/\d\d\d($)/', '***${1}', $addr);
preg_match("@<subject>(.*)</subject>@i", $postBody, $matches);
$message["subject"] = $matches[1];
$message["date"] = Util::getServerTime();
$message["id"] = count($messages);
mkdir($imagesPath . '/' . $message["id"]);

$boundaryParts = explode("--Nokia-mm-messageHandler-BoUnDaRy", $postBody);
foreach ($boundaryParts as $mimePart){
    if (!preg_match("@BASE64@", $mimePart)) {
        continue;
    }

    $mmPart = explode("BASE64", $mimePart);
    $filename = null;
    $contentType = null;
    if (preg_match("@Name=([^;^\n]+)@i", $mmPart[0], $matches)){
        $filename = trim($matches[1]);
    }
    if (preg_match("@Content-Type:([^;^\n]+)@i", $mmPart[0], $matches)){
        $contentType = trim($matches[1]);
    }
    if ($contentType == null || $filename == null) {
        continue;
    }

    $base64Data = base64_decode($mmPart[1]);
    $filePath = $imagesPath . '/' . $message['id'] . '/' . $filename;
    $relativePath = $imagesPathName . '/' .$message['id'] . '/' . $filename;

    if (preg_match("@image@", $contentType)) {
        $message["image"] = $relativePath;
    }
    if (preg_match("@text@", $contentType)){
        $message["text"] = $relativePath;
    }

    if (!($fileHandle = fopen($filePath, 'w'))) {
        die("Unable to open $filePath");
    }
    fwrite($fileHandle, $base64Data);
    fclose($fileHandle);
}

// TODO: set a limit on the number of messages saved
$messages[] = $message;
FileUtil::saveArray($messages, $imagesDbPath);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
