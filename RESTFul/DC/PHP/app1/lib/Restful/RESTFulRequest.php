<?php
namespace Att\Api\Restful;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

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
 * @author    pk9069
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

// CURL is required for this class to work 
if (!function_exists('curl_init')) {
    $err = 'RestfulRequest class requires the CURL extension for PHP.';
    throw new Exception($err);
}

require_once __DIR__ . '/HttpMultipart.php';
require_once __DIR__ . '/RestfulResponse.php';
require_once __DIR__ . '/RestfulEnvironment.php';
require_once __DIR__ . '/HttpPost.php';
require_once __DIR__ . '/HttpPut.php';
require_once __DIR__ . '/HttpGet.php';

use RuntimeException;
use Att\Api\OAuth\OAuthToken;

/**
 * Used for sending restful requests. 
 *
 * The CURL extension is required for this class to work. 
 *
 * This class follows the dependency inversion principle by applying a
 * varation of the adapter pattern. That is, this class wraps CURL and provides
 * a simplified interface. 
 * 
 * Example usage of this class can be found below:
 * <pre>
 * <code>
 * // set any proxy settings, if applicable
 * RestfulEnvironment::setProxy('proxy.host', 8080);
 * // choose URL to send request to
 * $endpoint = 'http://www.att.com';
 * // create a RestfULRequest
 * $req = new RestfulRequest($endpoint);
 * // set any headers
 * $req->setHeader('name', 'value');
 * // send request and get result
 * $result = $req->sendHttpGet();
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
 * @author   pk9069
 * @version  1.0
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 */
class RestfulRequest
{

    /**
     * HTTP Headers to send.
     */
    private $_headers;

    /**
     * Whether to send the request chunked.
     */
    private $_chunked;

    /**
     * URL to send request to.
     */
    private $_url;


    private $_options;

    private function _setInternalOptions($url) 
    {
        if ($this->_chunked) {
            $this->setHeader('Content-Transfer-Encoding', 'chunked');
        } 

        // get result as a string instead of printing it out
        $this->_options[CURLOPT_RETURNTRANSFER] = true;

        // set the URL
        $this->_options[CURLOPT_URL] = $url;

        // set any proxy settings, if applicable
        if (RestfulEnvironment::getProxyHost() != null) {
            $host = RestfulEnvironment::getProxyHost();
            $port = RestfulEnvironment::getProxyPort();

            $this->_options[CURLOPT_PROXY] = $host;
            $this->_options[CURLOPT_PROXYPORT] = $port;
        }

        if (RestfulEnvironment::getAcceptAllCerts()) {
            // TODO: set if true or false, not just on true
            $shouldAccept = RestfulEnvironment::getAcceptAllCerts();

            $this->_options[CURLOPT_SSL_VERIFYHOST] = !$shouldAccept;
            $this->_options[CURLOPT_SSL_VERIFYPEER] = !$shouldAccept;
        }

        // Add HTTP headers to CURL
        $headers = array();
        foreach ($this->_headers as $name => $value) {
            $headers[] = $name . ': ' . $value;
        }
        $this->_options[CURLOPT_HTTPHEADER] = $headers;
    }

    /**
     * Sends a Restful request using the previously supplied values.
     *
     * @return RestfulResponse HTTP response
     * @throws RuntimeException if there was an error sending request
     */
    private function _sendRequest($url) 
    {
        $connection = curl_init();
        $this->_setInternalOptions($url);

        curl_setopt_array($connection, $this->_options);
        $response = curl_exec($connection);
        if (!$response) {
            throw new RuntimeException(curl_error($connection));
        }

        $headers = curl_getinfo($connection);
        $responseCode = curl_getinfo($connection, CURLINFO_HTTP_CODE);

        return new RestfulResponse($response, $responseCode, $headers);
    }

    /**
     * Initializes a RestfulRequest object that will send a request to the
     * specified url.
     *
     * @param string $url URL to send request to
     */
    public function __construct($url) 
    {

        // TODO: Implement encoding

        $this->_chunked = false;
        $this->_url = $url;

        $this->_headers = array();

        $this->_options = null;
    }

    /**
     * Convenience method used to set the 'Authorization' header.
     *
     * @param OAuthToken $token token to use when setting authorization header.
     *
     * @return a reference to this, thereby allowing method chaining
     */
    public function setAuthorizationHeader(OAuthToken $token) 
    {
        $this->setHeader('Authorization', 'BEARER ' . $token->getAccesstoken());

        return $this;
    }

    /**
     * Sets an HTTP header that will be used during the request.
     *
     * @param string $name  header name
     * @param string $value value name
     *
     * @return a reference to this, thereby allowing method chaining
     */
    public function setHeader($name, $value) 
    {
        $this->_headers[$name] = $value;
        return $this;
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

    public function sendHttpPost(HttpPost $post = null)
    {
        $this->_options = array();

        $this->_options[CURLOPT_POST] = true;
        $this->_options[CURLOPT_HTTPGET] = false; 
        $this->_options[CURLOPT_PUT] = false;
        
        if ($post != null && $post->getBody() != null) {
            $this->_options[CURLOPT_POSTFIELDS] = $post->getBody();
        }

        return $this->_sendRequest($this->_url);
    }

    public function sendHttpMultipart(HttpMultipart $multipart)
    {
        $this->setHeader('Content-Type', $multipart->getContentType()); 

        $httpPost = new HttpPost();
        $httpPost->setBody($multipart->getMultipartRaw());

        return $this->sendHttpPost($httpPost);
    }

    public function sendHttpGet(HttpGet $get = null)
    {
        $this->_options = array();

        $this->_options[CURLOPT_HTTPGET] = true; 
        $this->_options[CURLOPT_POST] = false;
        $this->_options[CURLOPT_PUT] = false;
        
        $url = $this->_url;

        if ($get != null && $get->getQueryParameters() != null) {
            $url = $this->_url . '?' . $get->getQueryParameters();
        }

        return $this->_sendRequest($url);
    }

    public function sendHttpPut(HttpPut $put)
    {
        $this->_options = array();

        $this->_options[CURLOPT_HTTPGET] = false;
        $this->_options[CURLOPT_POST] = false;
        $this->_options[CURLOPT_PUT] = true;

        $fput = tmpfile();
        fwrite($fput, $put->getPutData());
        fseek($fput, 0);
        
        $this->_options[CURLOPT_INFILE] = $this->_fput;
        $this->_options[CURLOPT_INFILESIZE] = strlen($put->getPutData());

        $response = $this->_sendRequest($this->_url);

        // There's an issue where the file won't be closed if an exception is
        // thrown. 
        // TODO: fix this issue
        fclose($this->_fput);

        return $response;
    }

    public function sendHttpDelete()
    {
        $this->_options = array();

        $this->_options[CURLOPT_HTTPGET] = false;
        $this->_options[CURLOPT_POST] = false;
        $this->_options[CURLOPT_PUT] = false;

        $this->_options[CURLOPT_CUSTOMREQUEST] = 'DELETE';

        return $this->_sendRequest($this->_url);
    }

}

?>
