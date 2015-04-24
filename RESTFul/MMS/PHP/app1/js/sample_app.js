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
var savedMmsNotifications = {};

var pollForNotifications = function() {
    var delay = 10000;
    var sendRequest = function() {
        $.post(endpoints.getNotifications).done(function(response) {

            var statusTable = $('#statusTable');
            var data = JSON.parse(response);

            var statusNotifications = data.statusNotifications;
            var statusTableCaption = 'Status:';
            var statusTableHeaders = [
                'Message Id', 'Address', 'Delivery Status',
            ];
            var statusNotificationTable = generateTable(
                statusTableHeaders, statusNotifications,
                statusTableCaption
            );
            statusTable.html(statusNotificationTable);

            var mmsNotifications = data.mmsNotifications;
            for (var i = 0; i < mmsNotifications.length; ++i) {
                var mmsNotification = mmsNotifications[i];
                var id = mmsNotification.id;
                if (id in savedMmsNotifications) {
                    continue;
                }
                savedMmsNotifications[id] = mmsNotification;

                mmsTableHeaders = [
                    'Sent From', 'On', 'Text'
                ];
                mmsTableValues = [
                    [mmsNotification.address, mmsNotification.date, mmsNotification.text]
                ]
                mmsTableCaption = 'MMS Notification:'
                var mmsImagesHtml = generateTable(
                    mmsTableHeaders, mmsTableValues, mmsTableCaption
                );
                mmsImagesHtml += ('<div><img src="' + mmsNotification.image 
                    + '" width="150" border="0" /></div><hr>');

                $('#mmsImages').append(mmsImagesHtml);
            }
            setTimeout(sendRequest, delay);
        });
    };
    sendRequest();
};

// define a custom hook for loading any custom values specific to
// this sample app
load_hook = function(data) {
    $('#notificationShortcode').html(data.notificationShortcode);
    pollForNotifications();
};

// vim: set ts=8 sw=4 sts=4 tw=79 ft=javascript et :
