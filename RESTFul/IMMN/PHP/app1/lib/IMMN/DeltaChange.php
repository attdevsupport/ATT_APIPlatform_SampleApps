<?php
namespace Att\Api\IMMN;

final class IMMNDeltaChange
{
    private $_msgId;
    private $_favorite;
    private $_unread;

    public function __construct($msgId, $favorite, $unread)
    {
        $this->_msgId = $msgId;
        $this->_favorite = $favorite;
        $this->_unread = $unread;
    }

    public function getMessageId()
    {
        return $this->_msgId;
    }

    public function isFavorite()
    {
        return $this->_favorite;
    }

    public function isUnread()
    {
        return $this->_unread;
    }

    public function toArray()
    {
        $arr = array();
        $arr['messageId'] = $this->getMessageId();

        if (isset($arr['isFavorite'])) {
            $arr['isFavorite'] = $this->isFavorite();
        }
        if (isset($arr['isUnread'])) {
            $arr['isUnread'] = $this->isUnread();
        }

        return $arr;
    }

    public static function fromArray($arr)
    {
        $msgId = $arr['messageId'];
        $favorite = $arr['isFavorite'];
        $unread = $arr['isUnread'];

        return new IMMNDeltaChange($msgId, $favorite, $unread);
    }
}

?>
