<?php
namespace Att\Api\IMMN;

require_once __DIR__ . '/DeltaChange.php';

use Att\Api\IMMN\IMMNDeltaChange;

final class Delta
{
    private $_type;
    private $_adds;
    private $_deletes;
    private $_updates;

    public function __construct($type, $adds, $deletes, $updates)
    {
        $this->_type = $type;
        $this->_adds = $adds;
        $this->_deletes = $deletes;
        $this->_updates = $updates;
    }

    public function getDeltaType()
    {
        return $this->_type;
    }

    public function getAdds()
    {
        return $this->_adds;
    }

    public function getDeletes()
    {
        return $this->_deletes;
    }

    public function getUpdates()
    {
        return $this->_updates;
    }

    private static function getDeltaChanges($arr)
    {
        $deltaChanges = array();
        foreach($arr as $deltaChangeArr) {
            $deltaChange = IMMNDeltaChange::fromArray($deltaChangeArr);
            $deltaChanges[] = $deltaChange;
        }

        return $deltaChanges;
    }

    public static function fromArray($arr)
    {
        $type = $arr['type'];
        $adds = Delta::getDeltaChanges($arr['adds']); 
        $deletes = Delta::getDeltaChanges($arr['deletes']); 
        $updates = Delta::getDeltaChanges($arr['updates']); 

        return new Delta($type, $adds, $deletes, $updates);
    }
    
}

?>
