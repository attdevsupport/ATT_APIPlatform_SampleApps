<?php
require_once __DIR__ . '/lib/OAuth/OAuthTokenService.php';
require_once __DIR__ . '/lib/Payment/PaymentService.php';
require_once __DIR__ . '/lib/Util/FileUtil.php';

function getFileToken() 
{
    include __DIR__ . '/config.php';

    if (!isset($oauth_file)) {
        // set default if can't load
        $oauth_file = 'token.php';
    }

    $token = OAuthToken::loadToken($oauth_file);
    if ($token == null || $token->isAccessTokenExpired()) {
        $tokenSrvc = new OAuthTokenService(
            $FQDN,
            $api_key,
            $secret_key
        );
        $token = $tokenSrvc->getTokenUsingScope($scope);
        // save token for future use
        $token->saveToken($oauth_file);
    }

    return $token;
}

function getNotificationIds($xmlstr) {
    $xmlparser = xml_parser_create();
    xml_parse_into_struct($xmlparser, $xmlstr, $vals);
    xml_parser_free($xmlparser);
    $ids = array();
    foreach ($vals as $val) {
        if (is_string($val['tag']) 
                && strcmp($val['tag'], 'HUB:NOTIFICATIONID') == 0) {
            $ids[] = $val['value'];
        }
    }

    return $ids;
}

// TODO: Avoid accepting all certs
RESTFulRequest::setDefaultAcceptAllCerts(true);

$refundXML = file_get_contents('php://input');
$notificationIds = getNotificationIds($refundXML);

require __DIR__ . '/config.php';
$token = getFileToken();

foreach ($notificationIds as $notificationId) {
    // get notificatio info
    $req = new PaymentService($FQDN, $token);
    $refundNotification = $req->getNotificationInfo($notificationId);
    $savedNotifications = FileUtil::loadArray('notifications.db');
    $nResponse = $refundNotification['GetNotificationResponse'];

    $notification = array();
    $notification['NotificationId'] = $notificationId;
    foreach ($nResponse as $k => $v) {
        $notification[$k] = $v;
    }

    $savedNotifications[] = $notification;

    // limit on the number of entires
    // TODO: Get limit from config
    while (count($savedNotifications) > 5) { 
        array_shift($savedNotifications);
    }

    FileUtil::saveArray($savedNotifications, 'notifications.db');

    // obtained notification info; stop server from sending it again
    $req->deleteNotification($notificationId);
}

?>
