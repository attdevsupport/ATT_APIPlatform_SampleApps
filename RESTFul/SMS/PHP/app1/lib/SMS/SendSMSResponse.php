<?php
namespace Att\Api\SMS;

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
 * Immutable class that holds a Send SMS response.
 *
 * @category API
 * @package  SMS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@
 * @link     https://developer.att.com/docs/apis/rest/3/SMS
 */
final class SendSMSResponse
{
    private $_messageId;

    private $_resourceUrl;

    public function __construct($messageId, $resourceUrl)
    {
        $this->_messageId = $messageId;
        $this->_resourceUrl = $resourceUrl;
    }

    public function getMessageId()
    {
        return $this->_messageId;
    }

    public function getResourceUrl()
    {
        return $this->_resourceUrl;
    }

    public static function fromArray($arr)
    {
        $outboundResponse = $arr['outboundSMSResponse'];

        // TODO: throw exception if required field isn't set
        $msgId = $outboundResponse['messageId'];
        
        $resourceUrl = null;
        if (isset($outboundResponse['resourceReference'])) {
            $resourceRef = $outboundResponse['resourceReference'];

            if (isset($resourceRef['resourceURL']))
                $resourceUrl = $resourceRef['resourceURL'];
        }

        return new SendSMSResponse($msgId, $resourceUrl);
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
