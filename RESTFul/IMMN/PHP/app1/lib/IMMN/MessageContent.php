<?php
namespace Att\Api\IMMN;

final class IMMNMessageContent
{
    private $_contentType;
    private $_contentLength;
    private $_content;

    private function __construct()
    {
    }

    public function getContentType()
    {
        return $this->_contentType;
    }

    public function getContentLength()
    {
        return $this->_contentLength;
    }

    public function getContent()
    {
        return $this->_content;
    }

    public static function fromArray($arr)
    {
        $immnMsgContent = new IMMNMessageContent();

        $immnMsgContent->_contentType = $arr['contentType'];
        $immnMsgContent->_contentLength = $arr['contentLength'];
        $immnMsgContent->_content = $arr['content'];

        return $immnMsgContent;
    }
}
