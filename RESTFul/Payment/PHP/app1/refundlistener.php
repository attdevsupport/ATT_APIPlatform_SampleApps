<?php
require_once __DIR__ . '/lib/Payment/PaymentRequest.php';
require_once __DIR__ . '/lib/Util/FileUtil.php';

function getToken() {
    require __DIR__ . '/config.php';
    // Try loading token from file
    $token = OAuthToken::loadToken($oauth_file);

    // No token saved or token is expired... send token request
    if (!$token || $token->isAccessTokenExpired()) {
        $URL = $FQDN . '/oauth/token';
        $id = $api_key;
        $secret = $secret_key;
        $tokenRequest = new OAuthTokenRequest($URL, $id, $secret);
        $token = $tokenRequest->getTokenUsingScope($scope);

        // Save token for future use 
        $token->saveToken($oauth_file);
    }

    return $token;
}

function getNotificationId($xmlstr) {
    $xmlparser = xml_parser_create();
    xml_parse_into_struct($xmlparser, $xmlstr, $vals);
    xml_parser_free($xmlparser);
    foreach ($vals as $val) {
        if (is_string($val['tag']) 
                && strcmp($val['tag'], 'HUB:NOTIFICATIONID') == 0) {
            return $val['value'];
        }
    }

    throw new Exception('Notification Id not found!');
}

$refundXML = file_get_contents('php://input');
$notificationId = getNotificationId($refundXML);

require __DIR__ . '/config.php';
$token = getToken();

// get notificatio info
$req = new PaymentRequest($FQDN . '/rest/3/Commerce/Payment/Notifications/' 
        . $notificationId, $token);
$refundNotification = $req->getNotificationInfo();
 FileUtil::saveArray($refundNotification, 'blah.db');

$nResponse = $refundNotification['GetNotificationResponse'];
if (empty($nResponse['MerchantSubscriptionId'])) {
    // transaction
    $savedRefundTrans = FileUtil::loadArray('refundtrans.db');
    $refundTrans = array(
            'NotificationId' => $notificationId,
            'NotificationType' => $nResponse['NotificationType'],
            'TransactionId' => $nResponse['OriginalTransactionId']
            );  
    $savedRefundTrans[] = $refundTrans;
    FileUtil::saveArray($savedRefundTrans, 'refundtrans.db');
} else {
    // subscription
    $savedRefundSubs = FileUtil::loadArray('refundsubs.db');

    $refundSub = array(
            'NotificationId' => $notificationId,
            'NotificationType' => $nResponse['NotificationType'],
            'TransactionId' => $nResponse['OriginalTransactionId']
            );  
    $savedRefundSubs[] = $refundSub;
    FileUtil::saveArray($savedRefundSubs, 'refundsubs.db');
}

// obtained notification info, stop server from sending it again
$req->deleteNotification();

?>
