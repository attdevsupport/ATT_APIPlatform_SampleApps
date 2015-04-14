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

// define a custom hook for loading iam values
load_hook = function(data) {
    var channelTable = $('#channelTable');
    var channelTableCaption = 'Channel Details:';
    var channelTableHeaders = [
        'Channel Id', 'Channel Type', 'Max Events per Notification',
        'Notification Content Type'
    ];
    var notificationChannel = data.notificationChannel;
    var channelTableValues = [
        notificationChannel.channelId, notificationChannel.channelType,
        notificationChannel.maxEvents, notificationChannel.contentType
    ];
    var htmlChannelTable = generateTable(
        channelTableHeaders, [channelTableValues],
        channelTableCaption
    );
    channelTable.html(htmlChannelTable);

    if (data.subscriptionActive === true) {
        $('#receivingNotifications').removeClass('hidden');
        $('#createSubscriptionAlert').addClass('hidden');
        pollForNotifications();
    }

    if ('savedData' in data && 'redirecting' in data.savedData) {
        var tabMapping = {
            'sendMsg': 'send-msg',
            'createIndex': 'create-msg-index',
            'getMsgList': 'get-msg-list',
            'getMsg': 'get-msg',
            'getMsgContent': 'get-msg-content',
            'getDelta': 'get-delta',
            'getMsgIndexInfo': 'get-msg-index-info',
            'updateMsg': 'update-msg',
            'delMsg': 'delete-msg',
            'getNotiDetails': 'get-notification-details',
            'createSubscription': 'create-subscription',
            'updateSubscription': 'update-subscription',
            'getSubscription': 'get-subscription',
            'deleteSubscription': 'delete-subscription',
        };

        var redirecting = data.savedData.redirecting;
        var activeTab = tabMapping[redirecting];

        $('a[href=#' + activeTab + ']').tab('show');
    }
};

// vim: set ts=8 sw=4 sts=4 tw=79 ft=javascript et :
