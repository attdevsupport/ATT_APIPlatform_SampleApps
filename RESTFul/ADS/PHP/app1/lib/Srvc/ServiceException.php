<?php
namespace Att\Api\Srvc;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * Service Library.
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2014. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category  API
 * @package   Service
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

use Exception;

/**
 * Basic class for storing any exceptions related to API requests.
 * 
 * @category API
 * @package  Service
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @link     http://developer.att.com
 * @see      Exception
 */
final class ServiceException extends Exception
{
    /**
     * Http status code returned by API after unsuccessful request.
     *
     * @var int
     */
    private $_errorCode;

    /** 
     * Http response body returned by API after unsuccessful request.
     *
     * @var string
     */
    private $_errorResponse;


    /**
     * Creates a new ServiceException object.
     *
     * @param string    $errBody  http status code as a result of an 
     *                            unsuccessful API request   
     * @param string    $errCode  http message body as a result of an 
     *                            unsuccessful API request
     * @param string    $msg      optional exception message
     * @param int       $code     optional exception code
     * @param Exception $previous optional previous exception
     */
    public function __construct(
        $errBody, $errCode, $msg = null, $code = 0, Exception $previous = null
    ) {

        if (!isset($msg)) {
            $msg = $errCode . ':' . $errBody; 
        }

        parent::__construct($msg, $code, $previous);
        $this->_errorCode = $errCode;
        $this->_errorResponse = $errBody;
    }

    /**
     * Gets the API http code associated with this exception. 
     * 
     * @return int http error code
     */
    public function getErrorCode()
    {
        return $this->_errorCode;
    }

    /**
     * Gets the API http response body associated with this exception.
     * 
     * @return string http error response body 
     */
    public function getErrorResponse() 
    {
        return $this->_errorResponse;
    }
}

?>
