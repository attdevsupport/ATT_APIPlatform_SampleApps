<?php
require_once __DIR__ . '/lib/Payment/NotificationDetails.php';
require_once __DIR__ . '/lib/Util/FileUtil.php';

use Att\Api\Payment\NotificationDetails;
use Att\Api\Util\FileUtil;

$rawXml = file_get_contents('php://input');
$details = NotificationDetails::fromXml($rawXml);

$arr = array(
    'type' => $details->getNotificationType(),
    'timestamp' => $details->getTimestamp(),
    'effective' => $details->getEffective(),
    'networkOperatorId' => $details->getNetworkOperatorId(),
    'ownerIdentifier'   => $details->getOwnerIdentifier(),
    'purchaseDate'      => $details->getPurchaseDate(),
    'productIdentifier' => $details->getProductIdentifier(),
    'purchaseActivityIdentifier' => $details->getPurchaseActivityIdentifier(),
    'instanceIdentifier' => $details->getInstanceIdentifier(),
    'minIdentifier'     => $details->getMinIdentifier(),
    'oldMinIdentifier' => $details->getOldMinIdentifier(),
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
    array_shift($notifications);
}
FileUtil::saveArray($notifications, 'notifications.db');

?>
