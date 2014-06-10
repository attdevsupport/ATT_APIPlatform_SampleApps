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

require_once __DIR__ . '../../Srvc/APIService.php';
require_once __DIR__ . '/../Restful/HttpPatch.php';
require_once __DIR__ . '/Contact.php';
require_once __DIR__ . '/ContactCommon.php';
require_once __DIR__ . '/ContactsResultSet.php';
require_once __DIR__ . '/GroupsResultSet.php';

use InvalidArgumentException;
use Att\Api\OAuth\OAuthToken;
use Att\Api\Restful\HttpGet;
use Att\Api\Restful\HttpPatch;
use Att\Api\Restful\HttpPost;
use Att\Api\Restful\RestfulRequest;
use Att\Api\Srvc\APIService;
use Att\Api\Srvc\Service;
use Att\Api\Srvc\ServiceException;

/**
 * Used to interact with version 1 of the Address Book API Gateway.
 *
 * @category API
 * @package  AAB
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@
 * @link     https://developer.att.com/docs/apis/rest/1/Advertising
 */
class AABService extends APIService
{

    /**
     * Creates an AABService object that can be used to interact with
     * the Address Book API Gateway.
     *
     * @param string     $fqdn  fully qualified domain name to which requests
     *                          will be sent
     * @param OAuthToken $token OAuth token used for authorization
     */
    public function __construct($fqdn, OAuthToken $token)
    {
        parent::__construct($fqdn, $token);
    }

    /**
     * Sends a request to the API gateway for creating a contact.
     *
     * @param ContactCommon $contact contact information to create
     *
     * @return string location of created resource
     * @throws ServiceException if request was not successful
     */
    public function createContact(ContactCommon $contact)
    {
        $endpoint = $this->getFqdn() . '/addressBook/v1/contacts';

        $req = new RestfulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Content-Type', 'application/json');

        $httpPost = new HttpPost();

        $arrBody = array('contact' => $contact->toArray());
        $httpPost->setBody(json_encode($arrBody));

        $result = $req->sendHttpPost($httpPost);

        $code = $result->getResponseCode();

        if ($code != 201 && $code != 204) {
            throw new ServiceException($result->getResponseBody(), $code);
        }

        return $result->getHeader('location');
    }

    /**
     * Sends a request to the API gateway for getting a contact.
     *
     * @param string      $contactId contact id used to get contact
     * @param string|null $xFields   x-fields to send or null if none
     *
     * @return Contact|QuickContact returned contact
     * @throws ServiceException if request was not successful
     */
    public function getContact($contactId, $xFields=null)
    {
        $subUrl = '/addressBook/v1/contacts/' . $contactId;
        $endpoint = $this->getFqdn() . $subUrl;

        $req = new RestfulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        if ($xFields != null) {
            $req->setHeader('x-fields', $xFields);
        }

        $result = $req->sendHttpGet();

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);

        if (isset($arr['quickContact'])) {
            return ContactCommon::fromArray($arr);
        } else {
            return Contact::fromArray($arr);
        }
    }

    /**
     * Sends a request to the API gateway for getting contacts.
     *
     * @param PaginationParameters $pParams paginiations parameters
     * @param string|null          $search  search text
     *
     * @return ContactResultSet contact result set
     * @throws ServiceException if request was not successful
     */
    public function getContacts($xFields=null,
        PaginationParameters $pParams=null, $search=null
    ) {

        $endpoint = $this->getFqdn() . '/addressBook/v1/contacts';

        $req = new RestfulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        if ($xFields != null) {
            $req->setHeader('x-fields', $xFields);
        }

        $httpGet = new HttpGet();

        if ($pParams != null) {
            $httpGet->setParams($pParams->toArray());
        }
        if ($search != null) {
            $httpGet->setParam('search', $search);
        }

        $result = $req->sendHttpGet($httpGet);

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);

        return ContactsResultSet::fromArray($arr);
    }

    /**
     * Sends a request to the API gateway for getting contact groups.
     *
     * @param string               $contactId contact id to get groups for
     * @param PaginationParameters $params    paginiations parameters
     *
     * @return GroupsResultSet returns contact groups
     * @throws ServiceException if request was not successful
     */
    public function getContactGroups(
        $contactId, PaginationParameters $params=null
    ) {

        $subUrl = "/addressBook/v1/contacts/$contactId/groups";
        $endpoint = $this->getFqdn() . $subUrl;

        $req = new RestfulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $httpGet = new HttpGet();
        if ($params != null) {
            $httpGet->setParams($params->toArray());
        }

        $result = $req->sendHttpGet($httpGet);

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);

        return GroupsResultSet::fromArray($arr);
    }

    /**
     * Sends a request to the API gateway for updating a contact.
     *
     * @param Contact $contact contact to update contact id used to get contact
     *
     * @return void
     * @throws ServiceException if request was not successful
     */
    public function updateContact(Contact $contact)
    {
        if ($contact->getContactId() == null) {
            throw new InvalidArgumentException('Contact id must not be null.');
        }

        $contactId = urlencode($contact->getContactId());
        $subUrl = '/addressBook/v1/contacts/' . $contactId;
        $endpoint = $this->getFqdn() . $subUrl;

        $req = new RestfulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $body = json_encode(array('contact' => $contact->toArray()));
        $httpPatch = new HttpPatch($body);
        $result = $req->sendHttpPatch($httpPatch);

        $code = $result->getResponseCode();

        if ($code != 201 && $code != 204) {
            throw new ServiceException($code, $result->getResponseBody());
        }
    }

    /**
     * Sends a request to the API gateway for deleting a contact.
     *
     * @param string $contactId contact id to delete
     *
     * @return void
     * @throws ServiceException if request was not successful
     */
    public function deleteContact($contactId)
    {
        $fqdn = $this->getFqdn();
        $endpoint = $fqdn . '/addressBook/v1/contacts/' . $contactId;

        $req = new RestfulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json');

        $result = $req->sendHttpDelete();

        $code = $result->getResponseCode();

        if ($code != 201 && $code != 204) {
            throw new ServiceException($code, $result->getResponseBody());
        }
    }

    /**
     * Sends a request to the API gateway for creating a group.
     *
     * @param Group $group group information to create
     *
     * @return string location of created resource
     * @throws ServiceException if request was not successful
     */
    public function createGroup(Group $group)
    {
        $endpoint = $this->getFqdn() . '/addressBook/v1/groups';

        $req = new RestfulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $httpPost = new HttpPost();
        $httpPost->setBody(json_encode($group->toArray()));

        $result = $req->sendHttpPost($httpPost);

        $code = $result->getResponseCode();
        if ($code != 201 && $code != 204) {
            throw new ServiceException($code, $result->getResponseBody());
        }
        return $result->getHeader('location');
    }

    /**
     * Sends a request to the API gateway for getting groups.
     *
     * @param PaginationParameters $pParams   paginiations parameters
     * @param string|null          $groupName group name used in search, if any
     *
     * @return ResultSet returned groups
     * @throws ServiceException if request was not successful
     */
    public function getGroups(PaginationParameters $pParams=null,
        $groupName=null
    ) {
        $endpoint = $this->getFqdn() . '/addressBook/v1/groups';

        $httpGet = new HttpGet();

        if ($pParams != null) {
            $httpGet->setParams($pParams->toArray());
        }
        if ($groupName != null) {
            $httpGet->setParam('groupName', $groupName);
        }

        $req = new RestfulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $result = $req->sendHttpGet($httpGet);

        if ($result->getResponseCode() == 204) {
            return null;
        }

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);

        return GroupsResultSet::fromArray($arr);
    }

    /**
     * Sends a request to the API gateway for deleting a group.
     *
     * @param string $groupId group id to delete
     *
     * @return void
     * @throws ServiceException if request was not successful
     */
    public function deleteGroup($groupId)
    {
        $groupId = urlencode($groupId);
        $endpoint = $this->getFqdn() . "/addressBook/v1/groups/$groupId";

        $req = new RestfulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $result = $req->sendHttpDelete();
        $code = $result->getResponseCode();
        if ($code != 204) {
            throw new ServiceException($result->getResponseBody(), $code);
        }
    }

    /**
     * Sends a request to the API gateway for updating a group.
     *
     * @param Group $group group to update
     *
     * @return void
     * @throws ServiceException if request was not successful
     */
    public function updateGroup(Group $group)
    {
        if ($group->getGroupId() == null) {
            throw new InvalidArgumentException('Group id must not be null.');
        }

        $subUrl = '/addressBook/v1/groups/' . urlencode($group->getGroupId());
        $endpoint = $this->getFqdn() . $subUrl;

        $req = new RESTFulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $body = json_encode($group->toArray());
        $result = $req->sendHttpPatch(new HttpPatch($body));

        $code = $result->getResponseCode();
        if ($code != 204) {
            throw new ServiceException($result->getResponseBody(), $code);
        }
    }

    /**
     * Sends a request to the API gateway adding contacts to a group.
     *
     * @param string $groupId    group id to add contacts to
     * @param array  $contactIds contact ids to add to group
     *
     * @return void
     * @throws ServiceException if request was not successful
     */
    public function addContactsToGroup($groupId, $contactIds)
    {
        $groupId = urlencode($groupId);
        $contactIds = urlencode(implode(',', $contactIds));

        $subUrl =
            "/addressBook/v1/groups/$groupId/contacts?contactIds=$contactIds";
        $endpoint = $this->getFqdn() . $subUrl;

        $req = new RESTFulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $result = $req->sendHttpPost();

        $code = $result->getResponseCode();
        if ($code != 204) {
            throw new ServiceException($result->getResponseBody(), $code);
        }
    }

    /**
     * Sends a request to the API gateway removing contacts from a group.
     *
     * @param string $groupId    group id to remove contacts from
     * @param array  $contactIds contact ids remove from group
     *
     * @return void
     * @throws ServiceException if request was not successful
     */
    public function removeContactsFromGroup($groupId, $contactIds)
    {
        $groupId = urlencode($groupId);
        $contactIds = urlencode(implode(',', $contactIds));

        $subUrl =
            "/addressBook/v1/groups/$groupId/contacts?contactIds=$contactIds";
        $endpoint = $this->getFqdn() . $subUrl;

        $req = new RESTFulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $result = $req->sendHttpDelete();

        $code = $result->getResponseCode();
        if ($code != 204) {
            throw new ServiceException($result->getResponseBody(), $code);
        }
    }

    /**
     * Sends a request to the API gateway for getting contacts owned by a group.
     *
     * @param string               $groupId group id
     * @param PaginationParameters $params  paginiation params
     *
     * @return array contact ids
     * @throws ServiceException if request was not successful
     */
    public function getGroupContacts($groupId, PaginationParameters $params=null)
    {
        $groupId = urlencode($groupId);
        $subUrl = "/addressBook/v1/groups/$groupId/contacts";

        $endpoint = $this->getFqdn() . $subUrl;

        $req = new RESTFulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json');

        $httpGet = new HttpGet();
        if ($params != null) {
            $httpGet->setParams($params->toArray());
        }

        $result = $req->sendHttpGet($httpGet);

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);
        $contactName = $arr['contactIds'];
        $contactArr = $contactName['id'];

        return $contactArr;
    }

    /**
     * Sends a request to the API gateway for getting subscriber's personal
     * contact card.
     *
     * @return Contact contact information for subscriber
     * @throws ServiceException if request was not successful
     */
    public function updateMyInfo(ContactCommon $contact)
    {
        $endpoint = $this->getFqdn() . '/addressBook/v1/myInfo';

        $req = new RestfulRequest($endpoint);

        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json')
            ->setHeader('Content-Type', 'application/json');

        $body = json_encode(array('myInfo' => $contact->toArray()));
        $httpPatch = new HttpPatch($body);
        $result = $req->sendHttpPatch($httpPatch);

        $code = $result->getResponseCode();

        if ($code != 201 && $code != 204) {
            throw new ServiceException($code, $result->getResponseBody());
        }
    }

    /**
     * Sends a request to the API gateway for getting subscriber's personal
     * contact card.
     *
     * @return Contact contact information for subscriber
     * @throws ServiceException if request was not successful
     */
    public function getMyInfo()
    {
        $endpoint = $this->getFqdn() . '/addressBook/v1/myInfo';

        $req = new RESTFulRequest($endpoint);
        $req
            ->setAuthorizationHeader($this->getToken())
            ->setHeader('Accept', 'application/json');

        $result = $req->sendHttpGet();

        $successCodes = array(200);
        $arr = Service::parseJson($result, $successCodes);

        return Contact::fromArray($arr['myInfo']);
    }

}

?>
