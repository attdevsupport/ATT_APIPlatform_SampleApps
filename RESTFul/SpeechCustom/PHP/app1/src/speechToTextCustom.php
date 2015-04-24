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
    $context = $_REQUEST['speechContext'];
    $nameParam = $_REQUEST['nameParam'];
    $filename = $_REQUEST['audioFile'];
    $xArgs = $_REQUEST['x_arg'];
    $flocation = $audioFolder . '/' . $filename; 
    $path = __DIR__ . '/../template/';
    $xGrammar = $path . 'x-grammar.txt';
    $xDictionary = $path . 'x-dictionary.txt';
    $srvc = new SpeechService($FQDN, $token);
    $response = $srvc->speechToTextCustom(
        $context, $flocation, $xGrammar, $xDictionary, $xArgs
    );
    $headers = null;
    $values = array();
    $recognition = $response['Recognition'];
    if (isset($recognition['NBest'])) {
        $nbests = $recognition['NBest'];
        foreach ($nbests as $nbest) {
            $headers = array(
                'ResponseId', 'Status', 'Hypothesis', 'LanguageId',
                'Confidence', 'Grade', 'ResultText', 'Words', 'WordScores'
            );
            $values[] = array(
                $recognition['ResponseId'], $recognition['Status'],
                $nbest['Hypothesis'], $nbest['LanguageId'],
                $nbest['Confidence'], $nbest['Grade'],
                $nbest['ResultText'], json_encode($nbest['Words']),
                json_encode($nbest['WordScores']),
            );
        }
    } else {
        $headers = array(
            'ResponseId', 'Status',
        );
        $values = array(
            $response->getResponseId(), $response->getStatus(),
        );
    }

    $arr = array(
        'success' => true,
        'tables' => array(
            array(
                'caption' => 'Speech Response:',
                'headers' => $headers,
                'values' => $values,
            ),
        ),
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
