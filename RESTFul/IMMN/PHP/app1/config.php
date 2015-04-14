<?php
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
define('INDEX_URI', 'http://localhost/immn/index.php');

// Mandatory parameter that specifies the authorization redirect URL. This is
// URL that the consent flow will redirect to.
define('AUTHORIZE_REDIRECT_URI', 'http://localhost/immn/src/oauth.php');

// Mandatory parameter. Scope to use when requesting access token.
define('SCOPE', 'IMMN,MIM');

// Mandatory parameter. Scope to use when requesting a Webhooks OAuth token.
define('NOTIFICATION_SCOPE', 'NOTIFICATIONCHANNEL');

// Mandatory parameter. File to save access token after request. File/folder 
// must have write permissions.
define('OAUTH_FILE', sys_get_temp_dir(). '/immnoauthtoken.php');

// Folder to use when sending any attachments.
define('ATTACHMENTS', 'attachments');

// Path to file used to save notifications. File/folder must have write
// permissions. Point the listener URL to the 'src/notifications.php' file to
// receive notifications.
define('NOTIFICATION_FILE', sys_get_temp_dir() . '/immn_notifications.db');

// This sample app requires that the notification channel is created
// before hosting the sample app. The configuration values are specified below
define('CHANNEL_ID', 'ENTER_VALUE'); // Created channel id
define('CHANNEL_TYPE', 'ENTER_VALUE'); // Created channel type
define('MAX_EVENTS', 'ENTER_VALUE'); // Created channel max events
define('CONTENT_TYPE', 'application/json'); // Created channel content type

// Expiry parameter to pass in during a call to 'Create Subscription' and to
// 'Update Subscription.'
define('EXPIRES_IN', 3600);

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
