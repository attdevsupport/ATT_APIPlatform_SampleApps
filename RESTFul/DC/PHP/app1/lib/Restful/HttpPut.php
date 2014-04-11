<?php
namespace Att\Api\Restful;

class HttpPut
{
    private $_putData;

    public function __construct($putData)
    {
        $this->_putData = $putData;
    }

    public function getPutData()
    {
        return $this->_putData;
    }

}

?>
