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

require __DIR__ . '/../config.php';
require_once __DIR__ . '/../lib/OAuth/OAuthToken.php';
require_once __DIR__ . '/../lib/OAuth/OAuthTokenService.php';
require_once __DIR__ . '/../lib/Util/Util.php';
require_once __DIR__ . '/../lib/Speech/SpeechService.php';

use Att\Api\OAuth\OAuthToken;
use Att\Api\OAuth\OAuthTokenService;
use Att\Api\Util\Util;
use Att\Api\Speech\SpeechService;

$arr = null;
try {
    $token = OAuthToken::loadToken($oauth_file);
    if ($token == null || $token->isAccessTokenExpired()) {
        $tokenSrvc = new OAuthTokenService($FQDN, $api_key, $secret_key);
        $token = $tokenSrvc->getTokenUsingScope($scope);
        $token->saveToken($oauth_file);
    }
    $speechSrvc = new SpeechService($FQDN, $token);
    $contentType = $_POST['contentType'];
    $plaintext = $_POST['plaintext'];
    $ssml = $_POST['ssml'];
    $xArg = $_POST['x_arg'];
    $txt = null;
    if ($contentType === 'text/plain') {
        $txt = $plaintext;
        if (strlen($txt) > 250) {
            throw new Exception("Character limit of 250 reached");
        }
    } else {
        $txt = $ssml;
    }
    $response = $speechSrvc->textToSpeech($contentType, $txt, $xArg);
    $arr = array(
        'success' => true,
        'audio' => array(
            'type' => 'audio/wav',
            'base64' => base64_encode($response)
        )
    );
} catch (Exception $e) {
    $arr = array(
        'success' => false,
        'text' => $e->getMessage()
    );
}

echo json_encode($arr);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
