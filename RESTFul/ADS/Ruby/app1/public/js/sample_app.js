/*
 * Copyright 2015 AT&T
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

// define a custom hook for loading any app-specific values
load_hook = function(data) {
    var addEntry = function(indexId, panelId) {
        var indexValue = $(indexId).val();
        var id = panelId + indexValue++;
        var elem = $(id);
        if (elem.length) {
            elem.removeClass('hidden');
            $(indexId).val(indexValue);
        } else {
            alert("Exceeded max allowed");
        }
    };

    var removeEntry = function(indexId, panelId) {
        var indexValue = $(indexId).val();
        var id = panelId + (--indexValue);
        var elem = $(id);
        if (elem.length) {
            elem.addClass('hidden');
            $(indexId).val(indexValue);
        }
    };

    var entryMappings = {
        /* Create Contact Mappings */
        '#createPhoneIndex': {
            'addButton': '#btnCreatePhones',
            'removeButton': '#btnUnCreatePhones',
            'panel': '#createPhonePanel',
        },
        '#createIMIndex': {
            'addButton': '#btnCreateIM',
            'removeButton': '#btnUnCreateIM',
            'panel': '#createIMPanel',
        },
        '#createAddressIndex': {
            'addButton': '#btnCreateAddresses',
            'removeButton': '#btnUnCreateAddresses',
            'panel': '#createAddressPanel',
        },
        '#createEmailIndex': {
            'addButton': '#btnCreateEmails',
            'removeButton': '#btnUnCreateEmails',
            'panel': '#createEmailPanel',
        },
        '#createWeburlIndex': {
            'addButton': '#btnCreateWeburls',
            'removeButton': '#btnUnCreateWeburls',
            'panel': '#createWeburlPanel',
        },

        /* Update Contact Mappings */
        '#updatePhoneIndex': {
            'addButton': '#btnUpdatePhones',
            'removeButton': '#btnUnUpdatePhones',
            'panel': '#updatePhonePanel',
        },
        '#updateIMIndex': {
            'addButton': '#btnUpdateIM',
            'removeButton': '#btnUnUpdateIM',
            'panel': '#updateIMPanel',
        },
        '#updateAddressIndex': {
            'addButton': '#btnUpdateAddresses',
            'removeButton': '#btnUnUpdateAddresses',
            'panel': '#updateAddressPanel',
        },
        '#updateEmailIndex': {
            'addButton': '#btnUpdateEmails',
            'removeButton': '#btnUnUpdateEmails',
            'panel': '#updateEmailPanel',
        },
        '#updateWeburlIndex': {
            'addButton': '#btnUpdateWeburls',
            'removeButton': '#btnUnUpdateWeburls',
            'panel': '#updateWeburlPanel',
        },

        /* Update MyInfo Mappings */
        '#myInfoPhoneIndex': {
            'addButton': '#btnMyInfoPhones',
            'removeButton': '#btnUnMyInfoPhones',
            'panel': '#myInfoPhonePanel',
        },
        '#myInfoIMIndex': {
            'addButton': '#btnMyInfoIM',
            'removeButton': '#btnUnMyInfoIM',
            'panel': '#myInfoIMPanel',
        },
        '#myInfoAddressIndex': {
            'addButton': '#btnMyInfoAddresses',
            'removeButton': '#btnUnMyInfoAddresses',
            'panel': '#myInfoAddressPanel',
        },
        '#myInfoEmailIndex': {
            'addButton': '#btnMyInfoEmails',
            'removeButton': '#btnUnMyInfoEmails',
            'panel': '#myInfoEmailPanel',
        },
        '#myInfoWeburlIndex': {
            'addButton': '#btnMyInfoWeburls',
            'removeButton': '#btnUnMyInfoWeburls',
            'panel': '#myInfoWeburlPanel',
        },
    };

    var handleMapping = function(key, entry) {
        var indexValue = $(key).val();
        $(key).val(0);
        for (var i = 0; i < indexValue; ++i) {
            addEntry(key, entry['panel']);
        }
        
        $(entry['addButton']).click(function(evt) {
            evt.preventDefault();
            addEntry(key, entry['panel']);
        });
        $(entry['removeButton']).click(function(evt) {
            evt.preventDefault();
            removeEntry(key, entry['panel']);
        });
    };

    for (var key in entryMappings) {
        var entry = entryMappings[key];
        handleMapping(key, entry);
    }

    if ('savedData' in data && 'redirecting' in data.savedData) {
        var tabMapping = {
            'createContact': 'create-contact',
            'updateContact': 'update-contact',
            'deleteContact': 'delete-contact',
            'getContacts': 'get-contacts',
            'getMyInfo': 'get-my-info',
            'updateMyInfo': 'update-my-info',
            'createGroup': 'create-group',
            'updateGroup': 'update-group',
            'deleteGroup': 'delete-group',
            'getGroups': 'get-groups',
            'getGroupContacts': 'get-group-contacts',
            'addContactsToGroup': 'add-contacts-to-group',
            'removeContactsFromGroup': 'remove-contacts-from-group',
            'getContactGroups': 'get-contact-groups',
        };

        var redirecting = data.savedData.redirecting;
        var activeTab = tabMapping[redirecting];

        $('a[href=#' + activeTab + ']').tab('show');
    }
};

// vim: set ts=8 sw=4 sts=4 tw=79 ft=javascript et :
