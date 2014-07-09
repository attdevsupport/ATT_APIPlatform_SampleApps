<?php
require_once __DIR__ . '/lib/OAuth/OAuthTokenService.php';
require_once __DIR__ . '/lib/Payment/NotificationDetails.php';
require_once __DIR__ . '/lib/Payment/PaymentService.php';
require_once __DIR__ . '/lib/Restful/RestfulEnvironment.php';
require_once __DIR__ . '/lib/Util/FileUtil.php';

use Att\Api\OAuth\OAuthTokenService;
use Att\Api\Payment\NotificationDetails;
use Att\Api\Restful\RestfulEnvironment;
use Att\Api\Util\FileUtil;

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

// TODO: Avoid accepting all certs
RestfulEnvironment::setAcceptAllCerts(true);

$rawXml = file_get_contents('php://input');
$details = NotificationDetails::fromXml($rawXml);

$arr = array(
    'networkOperatorId' => $details->getNetworkOperatorId(),
    'ownerIdentifier'   => $details->getOwnerIdentifier(),
    'purchaseDate'      => $details->getPurchaseDate(),
    'productIdentifier' => $details->getProductIdentifier(),
    'purchaseActivityIdentifier' => $details->getPurchaseActivityIdentifier(),
    'instanceIdentifier' => $details->getInstanceIdentifier(),
    'minIdentifier'     => $details->getMinIdentifier(),
    'sequenceNumber'    => $details->getSequenceNumber(),
    'reasonCode'        => $details->getReasonCode(),
    'reasonMessage'     => $details->getReasonMessage(),
    'vendorPurchaseIdentifier' => $details->getVendorPurchaseIdentifier()
);

$notifications = FileUtil::loadArray('notifications.db');
$notifications[] = $arr;
// limit on the number of entires
// TODO: Get limit from config
while (count($notifications) > 5) {
    array_shift($savedNotifications);
}
FileUtil::saveArray($notifications, 'notifications.db');

?>
