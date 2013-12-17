<?php
namespace Att\Api\IMMN;

final class IMMNSegmentationDetails
{
    private $_segmentationMsgRefNumber;
    private $_totalNumberOfParts;
    private $_thisPartNumber;

    public function __construct($refNumber, $numParts, $partNumb)
    {
        $this->_segmentationMsgRefNumber = $refNumber;
        $this->_totalNumberOfParts = $numParts;
        $this->_thisPartNumber = $partNumb;
    }

    public function getSegmentationMsgRefNumber()
    {
        return $this->_segmentationMsgRefNumber;
    }

    public function getTotalNumberOfParts()
    {
        return $this->_totalNumberOfParts;
    }

    public function getThisPartNumber()
    {
        return $this->_thisPartNumber;
    }

    public static function fromArray($arr)
    {
        $refNumber = $arr['segmentationMsgRefNumber'];
        $numParts = $arr['totalNumberOfParts'];
        $partNumb = $arr['thisPartNumber'];

        return new IMMNSegmentationDetails($refNumber, $numParts, $partNumb);
    }

}

?>
