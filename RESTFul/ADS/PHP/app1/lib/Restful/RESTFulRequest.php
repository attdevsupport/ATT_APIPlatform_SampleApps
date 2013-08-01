<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Restful Library.
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2013. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2013 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category  Network
 * @package   Restful
 * @author    Pavel Kazakov <pk9069@att.com>
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

// CURL is required for this class to work 
if (!function_exists('curl_init')) {
    $err = 'RESTFulRequest class requires the CURL extension for PHP.';
    throw new Exception($err);
}

require_once __DIR__ . '/Multipart.php';
require_once __DIR__ . '/RESTFulResponse.php';

/**
 * Used for sending restful requests. 
 * 
 * This class follows the dependency inversion principle by applying a
 * varation of the adapter pattern. That is, this class wraps CURL and provides
 * a simplified interface. Since this class is a wrapper for CURL, the CURL
 * extension is required for this class to work. 
 * 
 * Example usage of this class can be found below:
 * <pre>
 * <code>
 * // choose URL to send request to
 * $endpoint = 'http://www.att.com';
 * // create a RESTFULRequest
 * $req = new RESTFulRequest($endpoint);
 * // set any proxy settings, if applicable
 * $req->setProxy('proxy.entp.attws.com', 8080);
 * // set http method
 * $req->setHttpMethod(RESTFulRequest::HTTP_METHOD_GET);
 * // set any headers
 * $req->setHeader('name', 'value');
 * // add any get params
 * $req->addParam('param', 'value');
 * // send request and get result
 * $result = $req->sendRequest();
 * // parse result
 * if ($result->getResponseCode() == 200) {
 *     print('Success!');
 *     print("\n");
 *     print($result->getResponseBody());
 * } else {
 *     print('Failed');
 * }
 * </code>
 * </pre>
 * 
 * @category Network
 * @package  Restful 
 * @author   Pavel Kazakov <pk9069@att.com>
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
class RESTFulRequest
{
    const HTTP_METHOD_GET = 0;
    const HTTP_METHOD_POST = 1;
    const HTTP_METHOD_PUT = 2;

    /**
     * Value to use for proxy host when a new RESTFulRequest object is created.
     *
     * @var string
     */
    private static $_defaultProxyHost = null;

    /**
     * Value to use for proxy port when a new RESTFulRequest object is created.
     *
     * @var int
     */
    private static $_defaultProxyPort = -1;

    /**
     * Value to use for whether to accept invalid or self-signed certificates
     * when a new RESTFulRequest object is created
     *
     * @var boolean
     */
    private static $_defaultAcceptAllCerts = false;

    // TODO: Implement
    /**
     * Encoding to be used when sending request. 
     */
    private $_encoding;

    /**
     * The HTTP Method to use when sending the request. E.g. POST.
     */
    private $_httpMethod;

    /**
     * HTTP Headers to send.
     */
    private $_headers;

    /**
     * CURL options to use.
     */
    private $_options;

    /**
     * Whether to send the request chunked.
     */
    private $_chunked;

    /**
     * Query parameters to send.
     */
    private $_params;

    /**
     * POST body, if applicable.
     */
    private $_body;

    /**
     * PUT data, if applicable.
     */
    private $_putData;

    /**
     * Temporary file used for sending PUT data.
     */
    private $_fput;

    /**
     * URL to send request to.
     */
    private $_url;

    /**
     * Only used in multipart requests.
     */
    private $_multipart;

    /**
     * Internal function used to add any needed headers.
     *
     * @return void
     */
    private function _addInternalHeaders() 
    {
        if ($this->isMultipartRequest()) {
            $contentType = $this->_multipart->getContentType();
            $this->setHeader('Content-Type', $contentType); 

            $this->_body = $this->_multipart->getMultipartRaw();
        }

        //TODO: Modify to prevent curl from auto-appending content length
        if ($this->_chunked) {
            $this->setHeader('Content-Transfer-Encoding', 'chunked');
        } 
    }

    /**
     * Internal function used to add CURL options.
     *
     * @return void
     */
    private function _addInternalOptions() 
    {
        if ($this->_httpMethod == RESTFulRequest::HTTP_METHOD_GET) {
            $this->_options[CURLOPT_HTTPGET] = true; 
            $this->_options[CURLOPT_POST] = false;
            $this->_options[CURLOPT_PUT] = false;
        } else if ($this->_httpMethod == RESTFULRequest::HTTP_METHOD_POST) {
            $this->_options[CURLOPT_HTTPGET] = false; 
            $this->_options[CURLOPT_POST] = true;
            $this->_options[CURLOPT_PUT] = false;
            $this->_options[CURLOPT_POSTFIELDS] = $this->_params;
        } else if ($this->_httpMethod == RESTFulRequest::HTTP_METHOD_PUT) {
            $this->_options[CURLOPT_HTTPGET] = false;
            $this->_options[CURLOPT_POST] = false;
            $this->_options[CURLOPT_PUT] = true;

            $this->_fput = tmpfile();
            fwrite($this->_fput, $this->_putData);
            fseek($this->_fput, 0);
            $this->_options[CURLOPT_INFILE] = $this->_fput;
            $this->_options[CURLOPT_INFILESIZE] = strlen($this->_putData);
        }

        if ($this->_params != null) {
            $query = http_build_query($this->_params); 

            if ($this->_httpMethod == RESTFulRequest::HTTP_METHOD_GET) {
                $this->_url .= '?' . $query;
            } else if ($this->_httpMethod == RESTFulRequest::HTTP_METHOD_POST) {
                $this->_body = $query;
            }
        }

        // Set the URL
        $this->_options[CURLOPT_URL] = $this->_url;

        // Add any additional HTTP headers to array
        $this->_addInternalHeaders();

        // Add HTTP headers to CURL
        $headers = array();
        foreach ($this->_headers as $name => $value) {
            $headers[] = $name . ': ' . $value;
        }
        $this->_options[CURLOPT_HTTPHEADER] = $headers;

        // Add POST body, if applicable 
        if ($this->_body != null) {
            $this->_options[CURLOPT_POSTFIELDS] = $this->_body;
        }
    }

    /**
     * Initializes a RESTFulRequest object that will send a request to the
     * specified url.
     *
     * @param string $url URL to send request to
     */
    public function __construct($url) 
    {
        $this->_httpMethod = RESTFULRequest::HTTP_METHOD_GET;
        $this->_encoding = 'UTF-8';
        $this->_options = array();
        $this->_chunked = false;
        $this->_params = array();
        $this->_body = null;
        $this->_url = $url;
        $this->_fput = null;

        // only used in multipart requests
        $this->_multipart = null;

        $this->_headers = array();

        // Get result as a string instead of printing it out
        $this->_options[CURLOPT_RETURNTRANSFER] = true;
        
        $this->setProxy(self::$_defaultProxyHost, self::$_defaultProxyPort);
        $this->setAcceptAllCerts(self::$_defaultAcceptAllCerts);
    }

    /**
     * Sets the HTTP method to use when sending request.
     * 
     * Only the following HTTP methods currently are supported:
     * <ul>
     * <li>GET</li>
     * <li>POST</li>
     * <li>PUT</li>
     * </ul>
     * 
     * @param int $method http method to use when sending request
     *
     * @throws InvalidArgumentException if method is not supported
     * @return void
     */
    public function setHttpMethod($method) 
    {
        if ($method != RESTFulRequest::HTTP_METHOD_GET 
            && $method != RESTFulRequest::HTTP_METHOD_POST 
            && $method != RESTFulRequest::HTTP_METHOD_PUT
        ) {

            throw new InvalidArgumentException('Invalid HTTP method.');
        }

        $this->_httpMethod = $method;
    }

    /**
     * Convenience method used to add the 'Authorization' header.
     *
     * @param OAuthToken $token token to use when setting authorization header.
     *
     * @return void
     */
    public function addAuthorizationHeader(OAuthToken $token) 
    {
        $this->setHeader('Authorization', 'BEARER ' . $token->getAccesstoken());
    }

    /**
     * Convenience method for adding query parameters to request. 
     * For POST, the parameters will be set in the POST body.
     * For GET, the parameters will be appended to the URL.
     * 
     * Warning: For POST requests, this method will overwrite any values set 
     * using setBody(). 
     *
     * @param string $name  name
     * @param string $value value
     *
     * @return void
     */
    public function addParam($name, $value) 
    {
        $this->_params[$name] = $value;
    }

    /** 
     * Set raw body for HTTP POST.
     * 
     * If intending to set query parameters as the POST body, use the
     * {@link #addParam()} method instead.
     *
     * @param string $body body to use in an HTTP POST request.
     *
     * @return void
     */
    public function setBody($body) 
    {
        $this->_body = $body;
    }

    /**
     * Sets a multipart object to use for a multipart request.
     * 
     * By setting a multipart object, this request becomes a multipart request.
     *
     * @param MultipartBody $multipart multipart object to use 
     *
     * @return void
     */
    public function setMultipart(MultipartBody $multipart) 
    {
        $this->_multipart = $multipart;
    }

    /**
     * Gets whether this request is a multipart request.
     * 
     * @return boolean true if this is a multipart reqeust, false otherwise
     */
    public function isMultipartRequest() 
    {
        return $this->_multipart != null;
    }

    /**
     * Sets an HTTP header that will be used during the request.
     *
     * @param string $name  header name
     * @param string $value value name
     *
     * @return void
     */
    public function setHeader($name, $value) 
    {
        $this->_headers[$name] = $value;
    }

    /**
     * Sets any PUT data to be sent in a PUT request.
     * 
     * @param string $putData put data to use in PUT request
     *
     * @return void
     */
    public function setPutData($putData) 
    {
        $this->_putData = $putData;
    }


    /**
     * Sets whether to send this request chunked.
     *
     * @param boolean $chunked whether to send this request chunked
     *
     * @return void
     */
    public function setChunked($chunked) 
    {
        $this->_chunked = $chunked;
    }

    /**
     * Set to use a proxy.
     *
     * @param string $host host name
     * @param int    $port port
     * 
     * @return void
     */
    public function setProxy($host, $port) 
    {
        $this->_options[CURLOPT_PROXY] = $host;
        $this->_options[CURLOPT_PROXYPORT] = $port;
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
    public function setAcceptAllCerts($shouldAccept)
    {
        $this->_options[CURLOPT_SSL_VERIFYHOST] = !$shouldAccept;
        $this->_options[CURLOPT_SSL_VERIFYPEER] = !$shouldAccept;
    }

    /**
     * Sends a RESTFul request using the previously supplied values.
     *
     * @return RESTFulResponse HTTP response
     * @throws RuntimeException if there was an error sending request
     */
    public function sendRequest() 
    {
        $connection = curl_init();
        $this->_addInternalOptions();
        curl_setopt_array($connection, $this->_options);
        $response = curl_exec($connection);
        if (!$response) {
            throw new RuntimeException(curl_error($connection));
        }

        if ($this->_fput != null) {
            fclose($this->_fput);
            $this->_fput = null;
        }

        $headers = curl_getinfo($connection);
        $responseCode = curl_getinfo($connection, CURLINFO_HTTP_CODE);

        return new RESTFulResponse($response, $responseCode, $headers);
    }

    /**
     * Sets any proxy settings that will be used when a new RESTFulRequest 
     * object is created.
     *
     * @param string $proxyHost proxy host or null if no proxy 
     * @param int    $proxyPort proxy port or -1 if no proxy
     *
     * @return void
     */
    public static function setDefaultProxy($proxyHost, $proxyPort) 
    {
        self::$_defaultProxyHost = $proxyHost;
        self::$_defaultProxyPort = $proxyPort;
    }

    /**
     * Sets whether to accept invalid or self-signed certificates when a new
     * RESTFulRequest object is created.
     *
     * @param boolean $accept true if to accept invalid or self-signed 
     *                        certificates, false otherwise
     *
     * @return void
     */
    public static function setDefaultAcceptAllCerts($accept) 
    {
        self::$_defaultAcceptAllCerts = $accept;
    }
}
?>
