<?php
namespace Att\Api\IMMN;

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

require_once __DIR__ . '../../Srvc/APIService.php';

require_once __DIR__ . '/DeltaResponse.php';
require_once __DIR__ . '/MessageList.php';
require_once __DIR__ . '/MessageContent.php';
require_once __DIR__ . '/MessageIndexInfo.php';
require_once __DIR__ . '/NotificationCD.php';


use Att\Api\Restful\RestfulRequest;
use Att\Api\Srvc\Service;
use Att\Api\Srvc\ServiceException;
use Att\Api\Srvc\APIService;
use Att\Api\OAuth\OAuthToken;
use Att\Api\Restful\HttpGet;
use Att\Api\Restful\HttpPost;
use Att\Api\Restful\HttpPut;
use Att\Api\Restful\HttpMultipart;

use Att\Api\IMMN\DeltaResponse;
use Att\Api\IMMN\IMMNMessageList;
use Att\Api\IMMN\IMMNMessageContent;
use Att\Api\IMMN\IMMNMessageIndexInfo;
use Att\Api\IMMN\IMMNNotificactionCD;

/**
 * Used to interact with version 2 of the In-app Messaging from Mobile Number 
 * (IMMN) API.
 *
 * @category API
 * @package  IMMN
 * @author   pk9069
 * @license  http://www.apache.org/licenses/LICENSE-2.0
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/1/In-app%20Messaging%20from%20Mobile%20Number
 */
class IMMNService extends APIService
{

    /**
     * Creates an IMMNService object that can be used to interact with
     * the IMMN API.
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
     * Sends a message to the specified addresses. 
     *
     * @param array       $addresses strings that holds the addresses to which 
     *                               the specified messages will be sent. 
     * @param string|null $text      text body of message or null if none
     * @param string|null $subject   subject of message or null if none
     * @param array|null  $fnames    file names of attachments or null if none 
     * @param bool|null   $isGroup   whether to send as broadcast or null to
     *                               use default
     *
     * @return string message id 
     * @throws ServiceException if API request was not successful
     */
    public function sendMessage(
        $addresses, $text, $subject, $fnames = null, $isGroup = null
    ) {

        $endpoint = $this->getFqdn() . '/myMessages/v2/messages';

        $req = new RESTFulRequest($endpoint);
        $req
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json')
            ->setAuthorizationHeader($this->getToken());

        $vals = array('addresses' => $addresses);
        $vals['isGroup'] = $isGroup ? 'true' : 'false';

        if ($text !== null) { $vals['text'] = $text; }
        if ($subject !== null) { $vals['subject'] = $subject; }

        $messageRequest = array('messageRequest' => $vals);

        $jvals = json_encode($messageRequest);

        $result = null;
        if ($fnames == null) { // no attachments; send basic POST
            $req->setHeader('Content-Type', 'application/json');
            $httpPost = new HttpPost();
            $httpPost->setBody($jvals);
            $result = $req->sendHttpPost($httpPost);
        } else { // attachments; send as multipart 
            $mpart = new HttpMultipart();
            $mpart->addJSONPart($jvals); 
            foreach ($fnames as $fname) {
                $mpart->addFilePart($fname);
            }
            $result = $req->sendHttpMultipart($mpart);
        }

        $responseArr = Service::parseJson($result);
        return $responseArr['id'];
    }

    public function getMessageList($limit, $offset, $msgIds=null,
        $isUnread=null, $type=null, $keyword=null, $isIncoming=null, $isFav=null
    ) {

        $endpoint = $this->getFqdn() . '/myMessages/v2/messages';

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken());

        $httpGet = new HttpGet();

        $httpGet
            ->setParam('limit', $limit)
            ->setParam('offset', $offset);
        
        if ($msgIds != null) {
            $msgIdsStr = implode(',', $msgIds);
            $httpGet->setParam('messageIds', $msgIdsStr);
        }

        if ($isUnread !== null) {
            $httpGet->setParam('isUnread', $isUnread ? 'true' : 'false');
        }
        if ($isIncoming !== null) {
            $httpGet->setParam('isIncoming', $isIncoming ? 'true' : 'false');
        }
        if ($isFav !== null) {
            $httpGet->setParam('isFavorite', $isFav ? 'true' : 'false');
        }
        
        if ($type != null) $httpGet->setParam('type', $type);
        if ($keyword != null) $httpGet->setParam('keyword', $keyword);

        $result = $req->sendHttpGet($httpGet);
        $arr = Service::parseJson($result);

        return IMMNMessageList::fromArray($arr);
    }

    public function getMessage($msgId)
    {
        $msgId = urlencode($msgId);
        $endpoint = $this->getFqdn() . '/myMessages/v2/messages/' . $msgId;

        $req = new RestfulRequest($endpoint);

        $result = $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken())
            ->sendHttpGet();

        $arr = Service::parseJson($result);
        $msgArr = $arr['message'];

        return IMMNMessage::fromArray($msgArr);
    }

    public function getMessageContent($msgId, $partId)
    { 
        $msgId = urlencode($msgId);
        $partId = urlencode($partId);
        $endpoint = $this->getFqdn() . '/myMessages/v2/messages/' . $msgId
            .'/parts/'. $partId;

        $req = new RestfulRequest($endpoint);

        $result = $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken())
            ->sendHttpGet();

        $responseCode = $result->getResponseCode();
        if ($responseCode != 200) {
            throw new ServiceException($responseCode, $result->getResponseBody());
        }

        $arr = array();
        $arr['contentType'] = $result->getHeader('content-type');
        $arr['contentLength'] = (int) ($result->getHeader('content-length'));
        $arr['content'] = $result->getResponseBody();

        return IMMNMessageContent::fromArray($arr);
    }

    public function getMessagesDelta($state)
    {  

        $endpoint = $this->getFqdn() . '/myMessages/v2/delta';

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken());

        $httpGet = new HttpGet();
        $httpGet->setParam('state', $state);

        $result = $req->sendHttpGet($httpGet);
        $arr = Service::parseJson($result);

        return DeltaResponse::fromArray($arr);
    }

    public function updateMessages($immnDeltaChanges)
    {
        $endpoint = $this->getFqdn() . '/myMessages/v2/messages';

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json')
            ->setAuthorizationHeader($this->getToken());
         
        $msgsArr = array();
        foreach ($immnDeltaChanges as $immnDeltaChange) {
            $msgsArr[] = $immnDeltaChange->toArray();
        }
        
        $jMsgsArr = json_encode(array('messages' => $msgsArr));

        $result = $req->sendHttpPut(new HttpPut($jMsgsArr));

        if ($result->getResponseCode() != 204) {
            $body = $result->getResponseBody();
            throw new ServiceException($result->getResponseCode(), $body);
        }
    }

    public function updateMessage($msgId, $isUnread=null, $isFavorite=null)
    {
        $msgId = urlencode($msgId);
        $endpoint = $this->getFqdn() . '/myMessages/v2/messages/' . $msgId;

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json')
            ->setAuthorizationHeader($this->getToken());
         
        $vals = array();

        if ($isUnread !== null) { 
            $vals['isUnread'] = $isUnread ? 'true' : 'false';
        }

        if ($isFavorite !== null)
            $vals['isFavorite'] = $isFavorite ? 'true' : 'false';

        $jbody = json_encode(array('message' => $vals));

        $result = $req->sendHttpPut(new HttpPut($jbody));

        if ($result->getResponseCode() != 204) {
            $body = $result->getResponseBody();
            throw new ServiceException($result->getResponseCode(), $body);
        }

    }

    public function deleteMessages($msgIds)
    {
        $msgIdsStr = implode(',', $msgIds);
        $query = http_build_query(array('messageIds' => $msgIdsStr));

        $endpoint = $this->getFqdn() . '/myMessages/v2/messages?' . $query;

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken());

        $result = $req->sendHttpDelete();

        if ($result->getResponseCode() != 204) {
            $body = $result->getResponseBody();
            throw new ServiceException($result->getResponseCode(), $body);
        }
    }
    
    public function deleteMessage($msgId)
    {
        $endpoint = $this->getFqdn() . '/myMessages/v2/messages/' . $msgId;

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken());

        $result = $req->sendHttpDelete();

        if ($result->getResponseCode() != 204) {
            $body = $result->getResponseBody();
            throw new ServiceException($result->getResponseCode(), $body);
        }
    }

    public function createMessageIndex()
    {
        $endpoint = $this->getFqdn() . '/myMessages/v2/messages/index';

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken());

        $httpPost = new HttpPost();
        $httpPost->setBody(' '); //empty body
        $result = $req->sendHttpPost($httpPost);

        if ($result->getResponseCode() != 202) {
            $body = $result->getResponseBody();
            throw new ServiceException($result->getResponseCode(), $body);
        }
    }

    public function getMessageIndexInfo()
    {
        $endpoint = $this->getFqdn() . '/myMessages/v2/messages/index/info';

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken());

        $result = $req->sendHttpGet();

        $arr = Service::parseJson($result);
        return IMMNMessageIndexInfo::fromArray($arr);
    }

    public function getNotificationConnectionDetails($queues) {
        $endpoint = $this->getFqdn() 
            . '/myMessages/v2/notificationConnectionDetails';

        $req = new RestfulRequest($endpoint);

        $req
            ->setHeader('Accept', 'application/json')
            ->setAuthorizationHeader($this->getToken());
        
        $httpGet = new HttpGet();
        $httpGet->setParam('queues', $queues);

        $result = $req->sendHttpGet($httpGet);
        $arr = Service::parseJson($result);

        return IMMNNotificactionCD::fromArray($arr);
    }

}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
