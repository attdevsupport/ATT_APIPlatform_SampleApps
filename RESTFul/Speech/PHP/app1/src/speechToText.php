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
    $speechContext = $_POST['speechContext'];
    $speechFile = $_POST['speechFile'];
    $xArg = $_POST['x_arg'];
    $subContext = $_POST['x_subcontext'];
    $chunked = isset($_POST['chunked']) ? true : null;

    $allowedFiles = array(
        'boston_celtics.wav', 'california.amr', 'coffee.amr', 'doctors.wav',
        'nospeech.wav', 'samplerate_conflict_error.wav', 'this_is_a_test.spx',
        'too_many_channels_error.wav', 'boston_celtics.wav'
    );
    if (!in_array($speechFile, $allowedFiles, true)) {
        throw new Exception('Invalid speech file specified');
    } 
    $flocation = $audioFolder . '/' . $speechFile; 
    if ($speechContext !== 'Gaming') {
        $subContext = null;
    }
    $response = $speechSrvc->speechToText(
        $flocation, $speechContext, $subContext, $xArg, $chunked
    );
    $headers = null;
    $values = null;
    $nbest = $response->getNBest();
    if ($nbest !== null) {
        $headers = array(
            'ResponseId', 'Status', 'Hypothesis', 'LanguageId', 'Confidence',
            'Grade', 'ResultText', 'Words', 'WordScores'
        );
        $values = array(
            $response->getResponseId(), $response->getStatus(),
            $nbest->getHypothesis(), $nbest->getLanguageId(),
            $nbest->getConfidence(), $nbest->getGrade(),
            $nbest->getResultText(), json_encode($nbest->getWords()),
            json_encode($nbest->getWordScores()),
        );
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
                'values' => array($values),
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
