<?php
namespace Att\Api\CMS;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/**
 * CMS Library
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
 * @category  API
 * @package   CMS 
 * @author    pk9069
 * @copyright 2013 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Immutable class used to hold a create session response.
 *
 * @category API
 * @package  CMS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Call%20Management%20(Beta)
 */
final class CSResponse
{
    /**
     * Whether creating session was a success.
     *
     * @var bool
     */
    private $_success;

    /**
     * Id of created session.
     *
     * @var string
     */
    private $_id;

    /**
     * Creates an object to hold the response after creating a session
     *
     * @param boolean $success whether creating session was successful
     * @param string  $id      id of session
     */
    public function __construct($success, $id)
    {
        $this->_success = $success;
        $this->_id = $id;
    }


    /**
     * Gets whether creating the success was a success.
     * 
     * @return boolean whether creating session was a success
     */
    public function getSuccess()
    {
        return $this->_success;
    }


    /**
     * Gets the newly created session's id.
     *
     * @return string session id
     */
    public function getId()
    {
        return $this->_id;
    }

    /**
     * Creates a CSResponse object using the specified array.
     *
     * @param array $arr array to use for creating object
     *
     * @return CSResponse CSResponse object
     */
    public static function fromArray($arr)
    {
        // TODO: validate required field

        $success = $arr['success'];
        $id = $arr['id'];

        return new CSResponse($success, $id);
    }

}
?>
