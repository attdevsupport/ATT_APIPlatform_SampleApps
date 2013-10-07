<?php
namespace Att\Api\Restful;

class RestfulEnvironment
{
    /**
     * Value to use for proxy host when a new RestfulRequest object is created.
     *
     * @var string
     */
    private static $_proxyHost = null;

    /**
     * Value to use for proxy port when a new RestfulRequest object is created.
     *
     * @var int
     */
    private static $_proxyPort = -1;

    /**
     * Value to use for whether to accept invalid or self-signed certificates
     * when a new RestfulRequest object is created
     *
     * @var boolean
     */
    private static $_acceptAllCerts = false;

    /**
     * Sets any proxy values.
     *
     * @param string $host host name
     * @param int    $port port
     * 
     * @return void
     */
    public static function setProxy($host, $port) 
    {
        RestfulEnvironment::$_proxyHost = $host;
        RestfulEnvironment::$_proxyPort = $port;
    }

    /**
     * Sets whether to accept all certificates.
     * 
     * Useful for handling self-signed certificates, but should not be used on
     * production.
     *
     * @param boolean $shouldAccept true if to accept all certificates, false
     *                              otherwise
     * 
     * @return void
     */
    public static function setAcceptAllCerts($shouldAccept)
    {
        RestfulEnvironment::$_acceptAllCerts = $shouldAccept;
    }

    public static function getProxyHost()
    {
        return RestfulEnvironment::$_proxyHost;
    }

    public static function getProxyPort()
    {
        return RestfulEnvironment::$_proxyPort;
    }

    public static function getAcceptAllCerts()
    {
        return RestfulEnvironment::$_acceptAllCerts;
    }
}

?>
