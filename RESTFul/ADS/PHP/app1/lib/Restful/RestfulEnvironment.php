<?php
namespace Att\Api\Restful;

/*
 * Copyright 2014 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

final class RestfulEnvironment
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
