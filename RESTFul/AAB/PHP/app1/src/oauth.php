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
require_once __DIR__ . '/../lib/OAuth/OAuthException.php';

use Att\Api\OAuth\OAuthException;

try {
    envinit();
    getSessionToken();
    header("Location: " . INDEX_URI);
} catch (OAuthException $oe) {
    $error = $oe->getError();
    $errorDesc = $oe->getErrorDescription();

    if (isset($_SESSION['savedData'])) {
        $savedData = json_decode($_SESSION['savedData'], true);
        if (isset($savedData['redirecting'])) {
            unset($savedData['redirecting']);
            $_SESSION['savedData'] = json_encode($savedData);
        }
    }
}

?>
<!DOCTYPE html>
<html lang="en">
  <head></head>
  <body>
    Unable to authenticate. Click <a href="<?php echo INDEX_URI ?>">here</a> to continue.
  </body>
</html>

<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
