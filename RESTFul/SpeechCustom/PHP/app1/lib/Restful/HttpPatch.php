<?php
namespace Att\Api\Restful;

class HttpPatch
{

    /**
     * PATCH body, if applicable.
     */
    private $_body;

    public function __construct($body)
    {
        $this->_body = $body;
    }

    public function getBody()
    {
        return $this->_body;
    }
}

?>
