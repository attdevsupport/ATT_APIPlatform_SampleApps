<?php
/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

require_once __DIR__ . '/../../lib/Controller/APIController.php';
require_once __DIR__ . '/../../lib/AAB/AABService.php';
require_once __DIR__ . '/../../lib/AAB/Contact.php';
require_once __DIR__ . '/../../lib/AAB/ContactCommon.php';
require_once __DIR__ . '/../../lib/AAB/Group.php';
require_once __DIR__ . '/../../lib/AAB/PaginationParameters.php';

use Att\Api\AAB\AABService;
use Att\Api\AAB\Contact;
use Att\Api\AAB\ContactCommon;
use Att\Api\AAB\Group;
use Att\Api\AAB\PaginationParameters;
use Att\Api\Controller\APIController;

/* Result and error constants */
define('C_OAUTH_ERROR', 0);
define('C_CREATE_CONTACT', 1);
define('C_CONTACT_SUCCESS', 2);
define('C_GET_CONTACTS', 3);
define('C_CONTACT_ERROR', 4);
define('C_MY_INFO', 5);
define('C_UPDATE_MY_INFO', 6);
define('C_CREATE_GROUP', 7);
define('C_UPDATE_GROUP', 8);
define('C_DELETE_GROUP', 9);
define('C_GET_GROUPS', 10);
define('C_GROUP_ERROR', 11);
define('C_GET_GROUP_CONTACTS', 12);
define('C_ADD_GROUP_CONTACTS', 13);
define('C_ADD_CONTACTS_TO_GROUP', 14);
define('C_REMOVE_CONTACTS_FROM_GROUP', 15);
define('C_GET_CONTACT_GROUPS', 16);
define('C_MANAGE_GROUPS_ERROR', 16);

class AABController extends APIController
{
    private function _getContactArrayData() {
        $arr = array();
        if (isset($_SESSION['phone'])) {
            $phoneSession = $_SESSION['phone'];
            $phonesArr = array();
            for ($i = 0; $i < count($phoneSession); $i += 2) {
                $number = $phoneSession[$i];
                $type = $phoneSession[$i + 1];
                $preferred = false;
                if (isset($_SESSION['phonePref'])) {
                    $preferred = ($i / 2 == $_SESSION['phonePref']);
                }
                $phonesArr[] = array(
                    'number' => $number['number'],
                    'type' => $type['type'],
                    'preferred' => $preferred
                );
            }
            $arr['phones'] = array('phone' => $phonesArr);
        }
        if (isset($_SESSION['im'])) {
            $imSession = $_SESSION['im'];
            $imsArr = array();
            for ($i = 0; $i < count($imSession); $i += 2) {
                $type = $imSession[$i + 1];
                $uri = $imSession[$i];
                $preferred = false;
                if (isset($_SESSION['imPref'])) {
                    $preferred = ($i / 2 == $_SESSION['imPref']);
                }
                $imsArr[] = array(
                    'imUri' => $uri['uri'],
                    'type' => $type['type'],
                    'preferred' => $preferred
                );
            }
            $arr['ims'] = array('im' => $imsArr);
        }
        if (isset($_SESSION['address'])) {
            $addressSession = $_SESSION['address'];
            $addressesArr = array();
            for ($i = 0; $i < count($addressSession); $i += 8) {
                $pobox = $addressSession[$i];
                $addressLine1 = $addressSession[$i + 1];
                $addressLine2 = $addressSession[$i + 2];
                $city = $addressSession[$i + 3];
                $state = $addressSession[$i + 4];
                $zip = $addressSession[$i + 5];
                $country = $addressSession[$i + 6];
                $type = $addressSession[$i + 7];
                $preferred = false;
                if (isset($_SESSION['addressPref'])) { 
                    $preferred = ($i / 8 == $_SESSION['addressPref']);
                }
                $addressesArr[] = array(
                    'addressLine1' => $addressLine1['addressLine1'],
                    'addressLine2' => $addressLine2['addressLine2'],
                    'city' => $city['city'],
                    'country' => $country['country'],
                    'poBox' => $pobox['pobox'],
                    'state' => $state['state'],
                    'zip' => $zip['zip'],
                    'type' => $type['type'],
                    'preferred' => $preferred
                );
            }
            $arr['addresses'] = array('address' => $addressesArr);
        }
        if (isset($_SESSION['email'])) {
            $emailSession = $_SESSION['email'];
            $emailsArr = array();
            for ($i = 0; $i < count($emailSession); $i += 2) {
                $emailAddr = $emailAddr[$i];
                $type = $emailSession[$i + 1];
                $preferred = false;
                if (isset($_SESSION['emailPref'])) {
                    $preferred = ($i / 2 == $_SESSION['emailPref']);
                }
                $emailsArr[] = array(
                    'emailAddress' => $emailAddr['email_address'],
                    'type' => $type['type'],
                    'preferred' => $preferred
                );
            }
            $arr['emails'] = array('email' => $emailsArr);
        }
        if (isset($_SESSION['weburl'])) {
            $weburlSession = $_SESSION['weburl'];
            $weburlsArr = array();
            for ($i = 0; $i < count($weburlSession); $i += 2) {
                $url = $weburlSession[$i];
                $type = $weburlSession[$i + 1];
                if (isset($_SESSION['weburlPref'])) {
                    $preferred = ($i / 2 == $_SESSION['weburlPref']);
                }
                $weburlsArr[] = array(
                    'url' => $url['url'],
                    'type' => $type['type'],
                    'preferred' => $preferred
                );
            }
            $arr['weburls'] = array('webUrl' => $weburlsArr);
        }
        return $arr;
    }
    
    private function _handleCreateContact()
    {
        $scalarVals = array(
            'firstName', 'middleName', 'lastName', 'prefix', 'suffix', 'nickname',
            'organization', 'jobTitle','anniversary', 'gender', 'spouse', 'children',
            'hobby', 'assistant'
        );
        $arrayVals = array(
            'phone', 'phonePref', 'im', 'imPref', 'address', 'addressPref',
            'email', 'emailPref', 'weburl', 'weburlPref'
        );
        $vnames = array_merge($scalarVals, $arrayVals);
        $vnames[] = 'createContact';

        $this->copyToSession($vnames);
        if (!isset($_SESSION['createContact'])) {
            return;
        }
        $reqParams = $this->_getContactArrayData();
        foreach ($scalarVals as $val) {
            if (isset($_SESSION[$val]) && strlen($_SESSION[$val]) > 0) {
                $reqParams[$val] = $_SESSION[$val];
            }
        }

        try {
            $cc = ContactCommon::fromArray($reqParams);
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $location = $aabSrvc->createContact($cc);
            $this->results[C_CREATE_CONTACT] = $location;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_CONTACT_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleUpdateContact()
    {
        $scalarVals = array(
            'contactId', 'firstName', 'middleName', 'lastName', 'prefix',
            'suffix', 'nickname', 'organization', 'jobTitle','anniversary',
            'gender', 'spouse', 'children', 'hobby', 'assistant'
        );
        $arrayVals = array(
            'phone', 'phonePref', 'im', 'imPref', 'address', 'addressPref',
            'email', 'emailPref', 'weburl', 'weburlPref'
        );
        $vnames = array_merge($scalarVals, $arrayVals);
        $vnames[] = 'updateContact';

        $this->copyToSession($vnames);
        if (!isset($_SESSION['updateContact'])) {
            return;
        }
        $reqParams = $this->_getContactArrayData();
        foreach ($scalarVals as $val) {
            if (isset($_SESSION[$val]) && strlen($_SESSION[$val]) > 0) {
                $reqParams[$val] = $_SESSION[$val];
            }
        }

        try {
            $cc = Contact::fromArray($reqParams);
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $location = $aabSrvc->updateContact($cc);
            $this->results[C_CONTACT_SUCCESS] = true;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_CONTACT_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleDeleteContact()
    {
        $vnames = array('contactId', 'deleteContact');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['deleteContact'])) {
            return;
        }
        $contactId = $_SESSION['contactId'];

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $location = $aabSrvc->deleteContact($contactId);
            $this->results[C_CONTACT_SUCCESS] = true;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_CONTACT_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleGetContacts()
    {
        $vnames = array('searchVal', 'getContacts');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['getContacts'])) {
            return;
        }
        $searchVal = $_SESSION['searchVal'];

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $resultSet = $aabSrvc->getContacts(null, null, $searchVal);
            $this->results[C_GET_CONTACTS] = $resultSet;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_CONTACT_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleGetMyInfo()
    {
        $vnames = array('getMyInfo');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['getMyInfo'])) {
            return;
        }
        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $result = $aabSrvc->getMyInfo();
            $this->results[C_MY_INFO] = $result;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_MY_INFO] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleUpdateMyInfo()
    {
        $scalarVals = array(
            'firstName', 'middleName', 'lastName', 'prefix', 'suffix',
            'nickname', 'organization', 'jobTitle','anniversary', 'gender',
            'spouse', 'children', 'hobby', 'assistant'
        );
        $arrayVals = array(
            'phone', 'phonePref', 'im', 'imPref', 'address', 'addressPref',
            'email', 'emailPref', 'weburl', 'weburlPref'
        );
        $vnames = array_merge($scalarVals, $arrayVals);
        $vnames[] = 'updateMyInfo';

        $this->copyToSession($vnames);
        if (!isset($_SESSION['updateMyInfo'])) {
            return;
        }
        $reqParams = $this->_getContactArrayData();
        foreach ($scalarVals as $val) {
            if (isset($_SESSION[$val]) && strlen($_SESSION[$val]) > 0) {
                $reqParams[$val] = $_SESSION[$val];
            }
        }

        try {
            $cc = ContactCommon::fromArray($reqParams);
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $location = $aabSrvc->updateMyInfo($cc);
            $this->results[C_UPDATE_MY_INFO] = true;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_MY_INFO] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleCreateGroup()
    {
        $vnames = array('groupName', 'groupType', 'createGroup');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['createGroup'])) {
            return;
        }
        $gname = $_SESSION['groupName'];
        $group = new Group($gname);

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $location = $aabSrvc->createGroup($group);
            $this->results[C_CREATE_GROUP] = $location;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_GROUP_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleUpdateGroup()
    {
        $vnames = array('groupId', 'groupName', 'updateGroup');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['updateGroup'])) {
            return;
        }
        $gname = $_SESSION['groupName'];
        $gid = $_SESSION['groupId'];
        $group = new Group($gname, $gid);

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $aabSrvc->updateGroup($group);
            $this->results[C_UPDATE_GROUP] = true;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_GROUP_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleDeleteGroup()
    {
        $vnames = array('groupId', 'deleteGroup');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['deleteGroup'])) {
            return;
        }
        $gid = $_SESSION['groupId'];

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $aabSrvc->deleteGroup($gid);
            $this->results[C_DELETE_GROUP] = true;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_GROUP_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleGetGroups()
    {
        $vnames = array('getGroupName', 'getGroups', 'order');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['getGroups'])) {
            return;
        }
        $gname = $_SESSION['getGroupName'];
        $order = $_SESSION['order'];
        $params = new PaginationParameters();
        $params->order = $order;

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $result = $aabSrvc->getGroups($params, $gname);
            $this->results[C_GET_GROUPS] = $result;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_GROUP_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleGetGroupContacts()
    {
        $vnames = array('getGroupContacts', 'groupId');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['getGroupContacts'])) {
            return;
        }
        $gid = $_SESSION['groupId'];

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $result = $aabSrvc->getGroupContacts($gid);
            $this->results[C_GET_GROUP_CONTACTS] = $result;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_MANAGE_GROUPS_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleAddContactsToGroup()
    {
        $vnames = array('addContactsToGroup', 'groupId', 'contactIds');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['addContactsToGroup'])) {
            return;
        }
        $gid = $_SESSION['groupId'];
        $cids = $_SESSION['contactIds'];
        $cids = explode(",", $cids);

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $aabSrvc->addContactsToGroup($gid, $cids);
            $this->results[C_ADD_CONTACTS_TO_GROUP] = true;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_MANAGE_GROUPS_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleRemoveContactsFromGroup()
    {
        $vnames = array('removeContactsFromGroup', 'groupId', 'contactIds');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['removeContactsFromGroup'])) {
            return;
        }
        $gid = $_SESSION['groupId'];
        $cids = $_SESSION['contactIds'];
        $cids = explode(",", $cids);

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $aabSrvc->removeContactsFromGroup($gid, $cids);
            $this->results[C_REMOVE_CONTACTS_FROM_GROUP] = true;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_MANAGE_GROUPS_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    private function _handleGetContactGroups()
    {
        $vnames = array('getContactGroups', 'contactId');

        $this->copyToSession($vnames);
        if (!isset($_SESSION['getContactGroups'])) {
            return;
        }
        $cid = $_SESSION['contactId'];

        try {
            $aabSrvc = new AABService($this->apiFQDN, $this->getSessionToken());
            $result = $aabSrvc->getContactGroups($cid);
            $this->results[C_GET_CONTACT_GROUPS] = $result;
            $this->clearSession($vnames);
        } catch (Exception $e) {
            $this->errors[C_MANAGE_GROUPS_ERROR] = $e->getMessage();
            $this->clearSession($vnames);
        }
    }

    public function __construct()
    {
        parent::__construct();
    }
    
    public function handleRequest()
    {
        /* contacts */
        $this->_handleCreateContact();
        $this->_handleUpdateContact();
        $this->_handleDeleteContact();
        $this->_handleGetContacts();

        /* my user profile */
        $this->_handleGetMyInfo();
        $this->_handleUpdateMyInfo();

        /* groups */
        $this->_handleCreateGroup();
        $this->_handleUpdateGroup();
        $this->_handleDeleteGroup();
        $this->_handleGetGroups();

        /* manage groups/contacts */
        $this->_handleGetGroupContacts();
        $this->_handleAddContactsToGroup();
        $this->_handleRemoveContactsFromGroup();
        $this->_handleGetContactGroups();
    }
}

?>
