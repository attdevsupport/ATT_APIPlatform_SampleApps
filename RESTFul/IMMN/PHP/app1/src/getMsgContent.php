<?php
/*
 * Copyright 2015 AT&T
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
session_start();

require_once __DIR__ . '/common.php';
require_once __DIR__ . '/../lib/IMMN/IMMNService.php';

use Att\Api\IMMN\IMMNService;

$arr = null;
try {
    envinit();
    $immnSrvc = new IMMNService(getFqdn(), getSessionToken());

    $msgId = $_POST['contentMsgId'];
    $partNumber = $_POST['contentPartNumber'];
    $content = $immnSrvc->getMessageContent($msgId, $partNumber);

    $arr = array('success' => true);
    $splitType = explode('/', $content->getContentType());
    $ctype = strtolower($splitType[0]);
    if ($ctype == 'text' || $ctype == 'plain') {
        $arr['text'] = 'Message Content: ' . $content->getContent();
    } else if ($ctype == 'image') {
        $arr['image'] = array(
            'type' => $content->getContentType(),
            'base64' => base64_encode($content->getContent())
        );
    } else if ($ctype == 'video') {
        $arr['video'] = array(
            'type' => $content->getContentType(),
            'base64' => base64_encode($content->getContent())
        );
    } else if ($ctype == 'audio') {
        $arr['audio'] = array(
            'type' => $content->getContentType(),
            'base64' => base64_encode($content->getContent())
        );
    }
} catch (Exception $e) {
    $arr = array(
        'success' => false,
        'text' => $e->getMessage()
    );
}

echo json_encode($arr);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
