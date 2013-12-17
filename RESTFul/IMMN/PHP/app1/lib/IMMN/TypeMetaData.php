<?php
namespace Att\Api\IMMN;

require_once __DIR__ . '/SegmentationDetails.php';

use Att\Api\IMMN\IMMNSegmentationDetails;

final class IMMNTypeMetaData
{
    private $_subject;
    private $_isSegmented;
    private $_segmentationDetails;

    public function __construct($subject, $isSegmented, $segmentationDetails)
    {
        $this->_subject = $subject;
        $this->_isSegmented = $isSegmented;
        $this->_segmentationDetails = $segmentationDetails;
    }

    public function getSubject()
    {
        return $this->_subject;
    }

    public function isSegmented()
    {
        return $this->_isSegmented;
    }

    public function getSegmentationDetails()
    {
        return $this->_segmentationDetails;
    }

    public static function fromArray($arr)
    {
        $isSegmented = null;
        $segDetails = null;
        $subject = null;

        if (isset($arr['isSegmented']) && $arr['isSegmented']) {
            $isSegmented = true;
        }

        if (isset($arr['segmentationDetails'])) {
            $segDetailsArr = $arr['segmentationDetails'];
            $segDetails = IMMNSegmentationDetails::fromArray($segDetailsArr);
        }

        if (isset($arr['subject'])) {
            $subject = $arr['subject'];
        }

        return new IMMNTypeMetaData($subject, $isSegmented, $segDetails);
    }

}

?>
