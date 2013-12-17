<?php
namespace Att\Api\IMMN;

final class IMMNMessageIndexInfo
{
    private $_status;
    private $_state;
    private $_msgCount;

    public function __construct($status, $state, $msgCount)
    {
        $this->_status = $status;
        $this->_state = $state;
        $this->_msgCount = $msgCount;
    }

    public function getStatus()
    {
        return $this->_status;
    }

    public function getState()
    {
        return $this->_state;
    }
    
    public function getMessagecount()
    {
        return $this->_msgCount;
    }

    public static function fromArray($arr)
    {
        $msgIndexInfoArr = $arr['messageIndexInfo'];

        $status = $msgIndexInfoArr['status'];
        $state = $msgIndexInfoArr['state'];
        $msgCount = $msgIndexInfoArr['messageCount'];

        return new IMMNMessageIndexInfo($status, $state, $msgCount);
    }
}

?>
