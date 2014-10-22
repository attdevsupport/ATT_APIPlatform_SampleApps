<?php
namespace Att\Api\MMS;

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

require_once __DIR__ . '../../Restful/HttpMultipart.php';
require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '../../Util/Util.php';
require_once __DIR__ . '/Status.php';
require_once __DIR__ . '/SendMMSResponse.php';

use Att\Api\OAuth\OAuthToken;
use Att\Api\Restful\HttpMultipart;
use Att\Api\Restful\RestfulRequest;
use Att\Api\Srvc\APIService;
use Att\Api\Srvc\Service;

/**
 * Used to interact with version 3 of the MMS API.
 *
 * @category API
 * @package  MMS
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/MMS
 */
class MMSService extends APIService
{

    /**
     * Creates a MMSService object that can be used to interact with
     * the MMS API.
     *
     * @param string     $FQDN  fully qualified domain name to which requests 
     *                          will be sent
     * @param OAuthToken $token OAuth token used for authorization 
     */
    public function __construct($FQDN, OAuthToken $token) 
    {
        parent::__construct($FQDN, $token); 
    }

    /**
     * Sends a request to the API for sending MMS to the specified address.
     *
     * @param string      $addr                 address to which MMS will be 
     *                                          sent.
     * @param string|null $subject              subject of MMS.
     * @param string|null $priority             priority of MMS
     * @param boolean     $notifyDeliveryStatus whether the API should send a
     *                                          notification after MMS has been 
     *                                          sent.
     *
     * @return SendMMSResponse API response.
     * @throws ServiceException if API request was not successful.
     */
    public function sendMMS($addr, $fnames, $subject = null, $priority = null, 
        $notifyDeliveryStatus = false
    ) {
        $endpoint = $this->getFqdn() . '/mms/v3/messaging/outbox';

        $req = new RESTFULRequest($endpoint);

        $req
            ->setHeader('Content-Type', 'application/json')
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken());

        $outboundRequest = array(
            'address' => $addr,
            'notifyDeliveryStatus' => $notifyDeliveryStatus
        );

        if ($subject != null) {
            $outboundRequest['subject'] = $subject;
        }
        if ($priority != null) {
            $outboundRequest['priority'] = $priority;
        }

        $vals = array('outboundMessageRequest' => $outboundRequest);
        $jvals = json_encode($vals);

        $mpart = new HttpMultipart();
        $mpart->addJSONPart($jvals);
        foreach ($fnames as $fname) {
            $mpart->addFilePart($fname);
        }

        $result = $req->sendHttpMultipart($mpart);

        $arr = Service::parseJson($result);
        return SendMMSResponse::fromArray($arr);
    }

    /**
     * Sends a request to the API for getting delivery status of an MMS.
     *
     * @param string $mmsId MMS Id for which to get status.
     *
     * @return MMSStatusResponse API response.
     * @throws ServiceException if API request was not successful.
     */
    public function getMMSStatus($mmsId) 
    {
        $endpoint = $this->getFqdn() . '/mms/v3/messaging/outbox/' . $mmsId;              

        $req = new RESTFULRequest($endpoint);

        $result = $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken())
            ->sendHttpGet();

        $arr = Service::parseJson($result);
        return Status::fromArray($arr);
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
