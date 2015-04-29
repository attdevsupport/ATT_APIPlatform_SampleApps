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
    var delay = 5000;
    var sendRequest = function() {
        $.post(endpoints.loadNotifications).done(function(response) {
            var receiveStatus = $('#receiveStatusTable');
            var receiveMessages = $('#receiveMessagesTable');
            var jbody = JSON.parse(response);
            var deliveryStatus = jbody.deliveryStatus;
            var messages = jbody.messages;
            setTimeout(sendRequest, delay);

            var deliveryStatusCaption = 'Delivery Status:';
            var deliveryStatusTableHeaders = [
                'Message ID', 'Address', 'Delivery Status', 
            ];
            var htmlDeliveryStatusTable = generateTable(
                deliveryStatusTableHeaders, deliveryStatus,
                deliveryStatusCaption
            );
            var messagesCaption = 'Received Messages:';
            var messagesTableHeaders = [
                'Message ID', 'Date Time', 'Sender Address', 
                'Destination Address', 'Message',
            ];
            var htmlMessagesTable = generateTable(
                messagesTableHeaders, messages, messagesCaption
            );
            receiveStatus.html(htmlDeliveryStatusTable);
            receiveMessages.html(htmlMessagesTable);
        });
    };
    setTimeout(sendRequest, delay);
};

var attapisuccess = {
    sendSms: genericSuccess,
    getDeliveryStatus: genericSuccess,
    getMessages: genericSuccess,
};

// vim: set ts=8 sw=4 sts=4 tw=79 ft=javascript et :
