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

final class HttpGet
{
    
    /**
     * Query parameters to send.
     */
    private $_params;

    public function __construct()
    {
        $this->_params = null;
    }

    /**
     * Sets a query parameter, which will be added to the URL query parameters. 
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
     * Sets query parameters using an array.
     *
     * @param array $arr associative array containing query parameters.
     */
    public function setParams($arr)
    {
        foreach($arr as $name => $value) {
            $this->setParam($name, $value);
        }
    }

    public function getQueryParameters()
    {
        if ($this->_params == null) {
            return null;
        }

        $query = http_build_query($this->_params); 
        return $query;
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
