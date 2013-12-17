<?php
namespace Att\Api\IMMN;

// immn notification connection details
final class IMMNNotificactionCD
{
    private $_uname;
    private $_pw;
    private $_httpsUrl;
    private $_wssUrl;
    private $_queues;

    public function __construct($uname, $pw, $httpsUrl, $wssUrl, $queues)
    {
        $this->_uname = $uname;
        $this->_pw = $pw;
        $this->_httpsUrl = $httpsUrl;
        $this->_wssUrl = $wssUrl;
        $this->_queues = $queues;
    }

    public function getUsername()
    {
        return $this->_uname;
    }

    public function getPassword()
    {
        return $this->_pw;
    }

    public function getHttpsUrl()
    {
        return $this->_httpsUrl;
    }

    public function getWssUrl()
    {
        return $this->_wssUrl;
    }

    public function getQueues()
    {
        return $this->_queues;
    }

    public static function fromArray($arr)
    {
        $notificationCDArr = $arr['notificationConnectionDetails'];

        $uname = $notificationCDArr['username'];
        $pw = $notificationCDArr['password'];
        $httpsUrl = $notificationCDArr['httpsUrl'];
        $wssUrl = $notificationCDArr['wssUrl'];
        $queues = $notificationCDArr['queues'];

        return new IMMNNotificactionCD(
            $uname, $pw, $httpsUrl, $wssUrl, $queues
        );
    }

}
