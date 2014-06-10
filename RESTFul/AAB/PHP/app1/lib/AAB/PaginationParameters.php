<?php
namespace Att\Api\AAB;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * AAB Library
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
 * @package   AAB
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      http://developer.att.com
 */

/**
 * Class used to hold paginiation parameters.
 *
 * For a list of response values and their definitions, refer to
 * {@link https://developer.att.com/apis/address-book/docs}.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@
 * @link     https://developer.att.com/apis/address-book/docs
 */
class PaginationParameters
{
    /**
     * Sorting order.
     *
     * Acceptable values:
     * <ul>
     * <li>ASC - ascending</li>
     * <li>DESC - descending</li>
     * <ul>
     *
     * @var string
     */
    public $order = null;

    /**
     * Field name to be used for ordering result set.
     *
     * @var string
     */
    public $orderBy = null;

    /**
     * Limit value, must be greater than or equal to one.
     *
     * @var int
     */
    public $limit = null;

    /**
     * Offset value, must be greater than or equal to zero.
     *
     * @var int
     */
    public $offset = null;

    /**
     * Converts PaginationParameters object to an associative array.
     *
     * @return array PaginationParameters object as an array
     */
    public function toArray()
    {
        $arr = array();

        if ($this->order != null)
            $arr['order'] = $this->order;

        if ($this->orderBy != null)
            $arr['orderBy'] = $this->orderBy;

        if ($this->limit != null)
            $arr['limit'] = $this->limit;

        if ($this->offset != null)
            $arr['offset'] = $this->offset;

        return $arr;
    }

}

?>
