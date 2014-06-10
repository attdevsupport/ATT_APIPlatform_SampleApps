<?php
namespace Att\Api\AAB;

final class Snapi
{
    private $_type;
    private $_uri;
    private $_preferred;

    public function __construct($type, $uri, $preferred)
    {
        $this->_type = $type;
        $this->_uri = $uri;
        $this->_preferred = $preferred;
    }

    public function getSnapiType()
    {
        return $this->_type;
    }

    public function getUri()
    {
        return $this->_uri;
    }

    public function isPreferred()
    {
        if (is_string($this->_preferred)) {
            $lpref = strtolower($this->_preferred);
            return $lpref == 'true';
        }

        return $this->_preferred;
    }

    public function toArray()
    {
        $arr = array();
        if ($this->getSnapiType() !== null)
            $arr['type'] = $this->getSnapiType();

        if ($this->getUri() !== null)
            $arr['uri'] = $this->getUri();

        if ($this->isPreferred() !== null)
            $arr['preferred'] = $this->isPreferred();

        return $arr;
    }

    public static function fromArray($arr)
    {
        $type = $arr['type'];
        $uri = $arr['uri'];
        $preferred = $arr['preferred'];
        
        return new Snapi($type, $uri, $preferred);
    }
}

?>
