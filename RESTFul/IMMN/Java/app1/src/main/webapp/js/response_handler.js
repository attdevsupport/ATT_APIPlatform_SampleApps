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

var pollForNotifications = function() {
    var delay = 3000;
    var sendRequest = function() {
        $.post(endpoints.getNotifications).done(function(response) {
            var notificationTable = $('#notificationTable');
            var data = JSON.parse(response);
            if ('stopPolling' in data) {
                $('#receivingNotifications').addClass('hidden');
                $('#createSubscriptionAlert').removeClass('hidden');
                notificationTable.html('');
                return;
            }
            setTimeout(sendRequest, delay);

            var notifications = response.notifications;
            var notificationTableCaption = 'Notification Payload:';
            var notificationTableHeaders = [
                'Subscription Id', 'Callback Data', 'Message Id',
                'Conversation Thread Id', 'Event Type', 'Queue', 'Text',
                'Truncated', 'Favorite', 'Unread'
            ];
            var htmlNotificationTable = generateTable(
                notificationTableHeaders, data,
                notificationTableCaption
            );
            notificationTable.html(htmlNotificationTable);
        });
    };
    setTimeout(sendRequest, delay);
};

var createSubscriptionSuccess = function(response) {
    genericSuccess(response);
    $('#receivingNotifications').removeClass('hidden');
    $('#createSubscriptionAlert').addClass('hidden');
    pollForNotifications();
};

var deleteSubscriptionSuccess = function(response) {
    genericSuccess(response);
    $('#receivingNotifications').addClass('hidden');
    $('#createSubscriptionAlert').removeClass('hidden');
};

var attapisuccess = {
    sendMsg: genericSuccess,
    createIndex: genericSuccess,
    getMsgList: genericSuccess,
    getMsg: genericSuccess,
    getMsgContent: genericSuccess,
    getDelta: genericSuccess,
    getMsgIndexInfo: genericSuccess,
    updateMsg: genericSuccess,
    delMsg: genericSuccess,
    getNotiDetails: genericSuccess,
    createSubscription: createSubscriptionSuccess,
    updateSubscription: genericSuccess,
    getSubscription: genericSuccess,
    deleteSubscription: deleteSubscriptionSuccess,
};

// vim: set ts=8 sw=4 sts=4 tw=79 ft=javascript et :
