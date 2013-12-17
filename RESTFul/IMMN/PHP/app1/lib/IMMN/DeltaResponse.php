<?php
namespace Att\Api\IMMN;

require_once __DIR__ . '/Delta.php';

use Att\Api\IMMN\Delta;

final class DeltaResponse
{
    private $_state;
    private $_deltas;

    private function __construct($state, $deltas)
    {
        $this->_state = $state;
        $this->_deltas = $deltas;
    }

    public function getState()
    {
        return $this->_state;
    }

    public function getDeltas()
    {
        return $this->_deltas;
    }

    public static function fromArray($arr)
    {
        $deltasResponseArr = $arr['deltaResponse'];
        $state = $deltasResponseArr['state'];

        $deltas = array();
        $deltasArr = $deltasResponseArr['delta'];
        foreach($deltasArr as $deltaArr) {
            $delta = Delta::fromArray($deltaArr);
            $deltas[] = $delta;
        }

        return new DeltaResponse($state, $deltas);
    }
}

?>
