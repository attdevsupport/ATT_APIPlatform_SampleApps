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

// Mandatory parameter that should be set to the registered application's
// 'API Key' value.
define('CLIENT_ID', 'ENTER_VALUE');

// Mandatory parameter that should be set to the registered application's
// 'Secret Key' value.
define('CLIENT_SECRET', 'ENTER_VALUE');

// Fully Qualified Domain Name. Mandatory parameter that points to the location
// of AT&T's API.
define('FQDN', 'https://api.att.com');

// Mandatory parameter that specifies the location of the index page.
define('INDEX_URI', 'http://localhost/aab/index.php');

// Mandatory parameter. File to save access token after request. File/folder
// must have write permissions.
define('OAUTH_FILE', sys_get_temp_dir() . '/aaboauthtoken.php');

// Mandatory parameter that specifies the authorization redirect URL. This is
// URL that the consent flow will redirect to.
define('AUTHORIZE_REDIRECT_URI', 'http://localhost/aab/src/oauth.php');

// Mandatory parameter. Scope to use when requesting access token.
define('SCOPE', 'AAB');

// URL Link for this sample app's download page.
define('LINK_DOWNLOAD', '#');

// URL link for this sample app's Github page.
define('LINK_GITHUB', '#');

// Optional parameters. Set any proxy settings to use.
// define('PROXY_HOST', 'proxy.host');
// define('PROXY_PORT', 8080);

// Optional parameter. Sets whether to accept all certificates, such as
// self-signed certificates. Useful for testing but should not be used on
// production.
// define('ACCEPT_ALL_CERTS', false);
?>
