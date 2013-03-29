<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 foldmethod=marker: */

/**
 * Common Library
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
 * @category RESTFul 
 * @copyright AT&T Intellectual Property
 * @license http://developer.att.com/sdk_agreement/
 */

require_once __DIR__ . '/Multipart.php';

/**
 * Class containing common information for sending RESTFul requests.
 * 
 * @package Common 
 */
abstract class RESTFulRequest {
    const HTTP_METHOD_GET = 0;
    const HTTP_METHOD_POST = 1;

    // Encoding to be used when sending request 
    // TODO: Implement
    private $_encoding;

    // Content type to use when sending request
    private $_contentType;

    // The HTTP Method to use when sending the request. E.g. POST
    private $_httpMethod;

    // HTTP Headers to send
    private $_headers;

    // CURL options to use
    private $_options;

    // Whether to send the request chunked
    private $_chunked;

    // Query parameters to send
    private $_params;

    // POST body, if applicable
    private $_body;

    // URL to send request to
    private $_url;

    // only used in multipart requests
    private $_multipart;

    /**
     * Internal function used to add any needed headers.
     */
    private function addInternalHeaders() {
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
     */
    private function addInternalOptions() {
        if ($this->_httpMethod == RESTFulRequest::HTTP_METHOD_GET) {
            $this->_options[CURLOPT_HTTPGET] = true; 
            $this->_options[CURLOPT_POST] = false;
        } else if ($this->_httpMethod == RESTFULRequest::HTTP_METHOD_POST) {
            $this->_options[CURLOPT_HTTPGET] = false; 
            $this->_options[CURLOPT_POST] = true;
            $this->_options[CURLOPT_POSTFIELDS] = $this->_params;
        }

        if ($this->_params != NULL) {
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
        $this->addInternalHeaders();

        // Add HTTP headers to CURL
        $headers = array();
        foreach ($this->_headers as $name => $value) {
            $headers[] = $name . ': ' . $value;
        }
        $this->_options[CURLOPT_HTTPHEADER] = $headers;

        // Add POST body, if applicable 
        if ($this->_body != NULL) {
            $this->_options[CURLOPT_POSTFIELDS] = $this->_body;
        }
    }

    /**
     * Sets the HTTP method to use when sending request. Only GET and POST are
     * currently supported.
     * 
     * @param int method to use when sending request.
     * @param throws InvalidArgumentException if method is not supported
     */
    protected function setHttpMethod($method) {
        if ($method != RESTFulRequest::HTTP_METHOD_GET &&
                $method != RESTFulRequest::HTTP_METHOD_POST) {

            throw new InvalidArgumentException('Invalid HTTP method.');
        }

        $this->_httpMethod = $method;
    }

    /**
     * Convenience method used to add the 'Authorization' header.
     *
     * @param $token OAuthToken token to use when setting authorization header.
     */
    protected function addAuthorizationHeader(OAuthToken $token) {
        $this->setHeader('Authorization', 'BEARER ' . $token->getAccesstoken());
    }

    /* Convenience method for adding query parameters to request. 
     * For POST, the parameters will be set in the POST body.
     * For GET, the parameters will be appended to the URL.
     * <p>
     * Warning: For POST requests, this method will overwrite any values set 
     * using setBody(). 
     *
     * @param $name string name
     * @param $value string value
     */
    protected function addParam($name, $value) {
        $this->_params[$name] = $value;
    }

    /* Set raw body for HTTP POST. If intending to set query parameters as
     * the POST body, use the addParam() method instead.
     *
     * @param $body string body to use in an HTTP POST request.
     */
    protected function setBody($body) {
        $this->_body = $body;
    }

    /**
     * Sets a multipart object to use for a multipart request. By setting a 
     * multipart object, this request becomes a multipart request.
     *
     * @param $multipart Multipart multipart object to use 
     */
    protected function setMultipart(Multipart $multipart) {
        $this->_multipart = $multipart;
    }

    /**
     * Gets whether this request is a multipart request.
     * 
     * @return boolean true if this is a multipart reqeust, false otherwise
     */
    protected function isMultipartRequest() {
        return $this->_multipart != NULL;
    }

    /**
     * Sets an HTTP header that will be used during the request.
     *
     * @param $name string header name
     * @param $value string value name
     */
    protected function setHeader($name, $value) {
        $this->_headers[$name] = $value;
    }

    /**
     * Initializes a RESTFulRequest object that will send a request to the
     * specified url.
     *
     * @param $url string URL to send request to
     */
    public function __construct($url) {
        $this->_httpMethod = RESTFULRequest::HTTP_METHOD_GET;
        $this->_encoding = 'UTF-8';
        $this->_options = array();
        $this->_chunked = false;
        $this->_params = array();
        $this->_body = NULL;
        $this->_url = $url;

        // only used in multipart requests
        $this->_multipart = NULL;

        $this->_headers = array();

        // Get result as a string instead of printing it out
        $this->_options[CURLOPT_RETURNTRANSFER] = true;
    }

    /**
     * Sets whether to send this request chunked.
     *
     * @param boolean whether to send this request chunked
     */
    public function setChunked($chunked) {
        $this->_chunked = $chunked;
    }

    /**
     * Set to use a proxy.
     *
     * @param $host string host name
     * @param $port int port
     */
    public function setProxy($host, $port) {
        $this->_options[CURLOPT_PROXY] = $host;
        $this->_options[CURLOPT_PROXYPORT] = $port;
    }

    /**
     * Sets whether to accept all certificates. Useful for testing if API server
     * is using self-signed certificates.
     */
    public function setAcceptAllCerts($shouldAccept) {
        $this->_options[CURLOPT_SSL_VERIFYHOST] = !$shouldAccept;
        $this->_options[CURLOPT_SSL_VERIFYPEER] = !$shouldAccept;
    }

    /**
     * Sends the request using the previously supplied values.
     * Returns an array with keys 'response', 'responseCode', and 'headers'.
     *
     * @return array an array of strings. 
     * @throws RuntimeException if there was an error sending request
     */
    public function sendRequest() {
        $connection = curl_init();
        $this->addInternalOptions();
        curl_setopt_array($connection, $this->_options);
        $response = curl_exec($connection);
        if (!$response) {
            throw new RuntimeException(curl_error($connection));
        }
        $headers = curl_getinfo($connection);
        $responseCode = curl_getinfo($connection, CURLINFO_HTTP_CODE);

        $results = array(
                'response' => $response, 
                'responseCode' => $responseCode,
                'headers' => $headers
                );

        return $results;
    }
}
?>
