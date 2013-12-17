<?php
namespace Att\Api\IMMN;

final class IMMNMmsContent
{
    private $_contentName;
    private $_contentUrl;
    private $_contentType;
    private $_type;

    public function __construct($contentName, $contentType, $contentUrl, $type)
    {
        $this->_contentName = $contentName;
        $this->_contentType = $contentType;
        $this->_contentUrl = $contentUrl;
        $this->_type = $type;
    }

    public function getContentName()
    {
        return $this->_contentName;
    }

    public function getContentUrl()
    {
        return $this->_contentUrl;
    }

    public function getContentType()
    {
        return $this->_contentType;
    }

    public function getMessageType()
    {
        return $this->_type;
    }

    public static function fromArray($arr)
    {
        $cname = $arr['contentName'];
        $ctype = $arr['contentType'];
        $curl = $arr['contentUrl'];
        $type = $arr['type'];

        $content = new IMMNMmsContent($cname, $ctype, $curl, $type);
    }

}

?>
