<?php
namespace Att\Api\AAB;

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

/**
 * Class used to hold paginiation parameters.
 *
 * For a list of response values and their definitions, refer to
 * {@link https://developer.att.com/apis/address-book/docs}.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
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

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
