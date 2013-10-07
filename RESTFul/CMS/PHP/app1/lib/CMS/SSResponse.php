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
 * Immutable class used to hold a send signal response.
 *
 * @category API
 * @package  CMS
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/Call%20Management%20(Beta)
 */
final class SSResponse
{
    /**
     * Status of sending signal.
     *
     * @var string
     */
    private $_status;

    /**
     * Creates object.
     *
     * @param string $status status of sending signal
     */
    public function __construct($status)
    {
        $this->_status = $status;
    }

    /**
     * Gets the status of sending a signal.
     *
     * Return value will be one of the following values:
     * <ul>
     * <li>QUEUED</li>
     * <li>NOTFOUND</li>
     * <li>FAILED</li>
     * </ul>
     * 
     * @return string status
     */
    public function getStatus()
    {
        return $this->_status;
    }

    /**
     * Creates a SSResponse object using the specified array.
     *
     * @param array $arr array to use for creating object
     *
     * @return SSResponse SSResponse object
     */
    public static function fromArray($arr)
    {
        // TODO: validate http status

        return new SSResponse($arr['status']);
    }

}
?>
