<?php
namespace Att\Api\Restful;

class HttpGet
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

?>
