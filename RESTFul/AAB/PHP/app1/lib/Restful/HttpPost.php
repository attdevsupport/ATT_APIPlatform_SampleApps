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

final class HttpPost
{

    /**
     * Query parameters to send.
     */
    private $_params;

    /**
     * POST body, if applicable.
     */
    private $_body;

    public function __construct()
    {
        $this->_params = null;
        $this->_body = null;
    }

    /**
     * Sets a query parameter, which will be appended to the POST body. 
     * 
     * Warning: this method will overwrite any values set using setBody(). 
     *
     * @param string $name  name
     * @param string $value value
     *
     * @return a reference to this, thereby allowing method chaining
     */
    public function setParam($name, $value) 
    {

        // lazy init
        if ($this->_params == null) {
            $this->_params = array();
        }

        $this->_params[$name] = $value;
        return $this;
    }

    /** 
     * Sets raw body for HTTP POST.
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

    public function getBody()
    {
        if ($this->_params != null) {
            $query = http_build_query($this->_params); 
            $this->_body = $query;
        }

        return $this->_body;
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
