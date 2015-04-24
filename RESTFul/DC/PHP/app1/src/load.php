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

require_once __DIR__ . '/../config.php';
require_once __DIR__ . '/common.php';
require_once __DIR__ . '/../lib/Util/Util.php';

use Att\Api\Util\Util;

$arr = array(
    'authenticated' => isSessionAuthenticated(),
    'redirect_url' => getCodeLocation(),
    'server_time' => Util::getServerTime(),
    'download' => LINK_DOWNLOAD,
    'github' => LINK_GITHUB,
);

if (isset($_SESSION['savedData'])) {
    $arr['savedData'] = json_decode($_SESSION['savedData'], true);
}

echo json_encode($arr);

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
