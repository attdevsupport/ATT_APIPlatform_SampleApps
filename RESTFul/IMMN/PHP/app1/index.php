<!DOCTYPE html>
<!--
Copyright 2015 AT&T

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
-->
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>In-App Messaging</title>

    <!-- jquery and bootstrap js -->
    <script src="https://lprod.code-api-att.com/public_files/js/jquery.min.js"></script>
    <script src="https://lprod.code-api-att.com/public_files/js/bootstrap.min.js"></script>
    <!-- custom js -->
    <script src="js/config.js"></script>
    <script src="js/form_handler.js"></script>
    <script src="js/response_handler.js"></script>
    <script src="js/sample_app.js"></script>

    <!-- bootstrap css -->
    <link rel="stylesheet" href="https://lprod.code-api-att.com/public_files/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://lprod.code-api-att.com/public_files/css/bootstrap-theme.min.css">
    <!-- custom css -->
    <link href="https://lprod.code-api-att.com/public_files/css/custom.css" rel="stylesheet">

    <script type="text/javascript">
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-33466541-1']);
        _gaq.push(['_trackPageview']);

        (function () {
             var ga = document.createElement('script');
             ga.type = 'text/javascript';
             ga.async = true;
             ga.src = ('https:' == document.location.protocol ? 'https://ssl'
                                         : 'https://www')
                                         + '.google-analytics.com/ga.js';
             var s = document.getElementsByTagName('script')[0];
             s.parentNode.insertBefore(ga, s);
         })();
    </script>

    <!--[if lt IE 9]>
      <script src="https://lprod.code-api-att.com/public_files/js/html5shiv.min.js"></script>
      <script src="https://lprod.code-api-att.com/public_files/js/respond.min.js"></script>
    <![endif]-->
  </head>
  <body>
    <div class="container">
      <div class="row">
        <div class="header">
          <ul class="nav nav-pills pull-left">
            <li>
              <a class="brand" href="https://developer.att.com">
                <img alt="AT&amp;T Developer" src="https://developer.att.com/static-assets/images/logo-developer.png">
              </a>
            </li>
          </ul>
        </div><!--./header-->
      </div><!--./row-->
      <div class="row">
        <h3 class="text-center">In-App Messaging</h3>
      </div>
      <div class="row">
        <h5 class="text-center">This sample application showcases sending, receiving, updating, and deleting MMS and SMS messages on behalf of a specific user.</h5>
      </div>
      <hr>
      <div class="inline-row">
        <a class="btn btn-warning" id="github" href="#">Github</a>
        <a class="btn btn-warning" id="download" href="#">Download</a>
      </div><!--./row-->
      <hr>
      <div class="row">
        <div class="alert alert-info">
          Note: All features except for Send Message require a subscription to
          <a href="http://messages.att.net">AT&amp;T Messages</a>
        </div>
      </div><!--./row-->
      <div class="row">
        <div class="col-md-12">
          <!-- TODO: finish this portion -->
          <div role="tabpanel">
            <!-- Nav tabs -->
            <ul class="nav nav-tabs" role="tablist">
              <li role="presentation" class="active">
                <a href="#send-msg" aria-controls="send-msg" role="tab" data-toggle="tab">Send Message</a>
              </li>
              <li role="presentation">
                <a href="#create-msg-index" aria-controls="create-msg-index" role="tab"
                  data-toggle="tab">Message Index</a>
              </li>
              <li role="presentation" class="dropdown">
                <a href="#" id="messages-tab" class="dropdown-toggle" data-toggle="dropdown"
                  aria-controls="messages-tab-contents">Get Message <span class="caret"></span></a>
                <ul class="dropdown-menu" role="menu" aria-labelledby="messages-tab" id="messages-tab-contents">
                  <li>
                    <a href="#get-msg-list" tabindex="-1" role="tab" id="get-msg-list-tab" data-toggle="tab"
                      aria-controls="get-msg-list">Get Message List</a>
                  </li>
                  <li>
                    <a href="#get-msg" tabindex="-1" role="tab" id="get-msg-tab" data-toggle="tab"
                      aria-controls="get-msg">Get Message</a>
                  </li>
                  <li>
                    <a href="#get-msg-content" tabindex="-1" role="tab" id="get-msg-content-tab" data-toggle="tab"
                      aria-controls="get-msg-content">Get Message Content</a>
                  </li>
                  <li>
                    <a href="#get-delta" tabindex="-1" role="tab" id="get-delta-tab" data-toggle="tab"
                      aria-controls="get-delta">Get Delta</a>
                  </li>
                  <li>
                    <a href="#get-msg-index-info" tabindex="-1" role="tab" id="get-msg-index-info-tab"
                      data-toggle="tab" aria-controls="get-msg-index-info">Get Message Index Info</a>
                  </li>
                </ul>
              </li>
              <li role="presentation">
                <a href="#update-msg" aria-controls="update-msg" role="tab" data-toggle="tab">Update Message</a>
              </li>
              <li role="presentation">
                <a href="#delete-msg" aria-controls="delete-msg" role="tab" data-toggle="tab">Delete Message</a>
              </li>
              <li role="presentation" class="dropdown">
                <a href="#" id="notifications-tab" class="dropdown-toggle" data-toggle="dropdown"
                  aria-controls="notification-tab-contents">Notifications <span class="caret"></span></a>
                <ul class="dropdown-menu" role="menu"
                  aria-labelledby="notifications-tab" id="notification-tab-contents">
                  <li>
                    <a href="#create-subscription" tabindex="-1" role="tab" id="create-subscription-tab"
                      data-toggle="tab" aria-controls="create-subscription">Create Subscription</a>
                  </li>
                  <li>
                    <a href="#update-subscription" tabindex="-1" role="tab" id="update-subscription-tab"
                      data-toggle="tab" aria-controls="update-subscription">Update Subscription</a>
                  </li>
                  <li>
                    <a href="#get-subscription" tabindex="-1" role="tab" id="get-subscription-tab" data-toggle="tab"
                      aria-controls="get-subscription">Get Subscription</a>
                  </li>
                  <li>
                    <a href="#delete-subscription" tabindex="-1" role="tab" id="delete-subscription-tab"
                      data-toggle="tab" aria-controls="delete-subscription">Delete Subscription</a>
                  </li>
                  <li>
                    <a href="#view-notification-details" tabindex="-1" role="tab" id="view-notification-details-tab"
                      data-toggle="tab" aria-controls="view-notification-details">View Notifications</a>
                  </li>
                </ul>
              </li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content">
              <div role="tabpanel" class="tab-pane active" id="send-msg">
                <form id="sendMsg">
                  <div class="form-group">
                    <label for="address">Address</label>
                    <input type="text" class="form-control" name="address" id="address"
                      data-toggle="tooltip" data-placement="bottom"
                      data-title="Format must be one of: tel:15555555"
                      placeholder="Address">
                  </div>
                  <div class="checkbox">
                    <label>
                      <input name="groupCheckbox" type="checkbox"> Group
                    </label>
                  </div>
                  <div class="form-group">
                    <label for="sendMsgInput">Message (max 200 characters allowed):</label>
                    <input type="text" class="form-control" name="sendMsgInput" id="sendMsgInput"
                      placeholder="Sample Message">
                  </div>
                  <div class="form-group">
                    <label for="sendSubjectInput">Subject (max 30 characters allowed):</label>
                    <input type="text" class="form-control" name="sendSubjectInput" id="sendSubjectInput"
                      placeholder="Sample Subject">
                  </div>
                  <button type="submit" data-loading-text="Sending..." class="btn btn-primary">Send Message</button>
                </form>
              </div>
              <div role="tabpanel" class="tab-pane" id="create-msg-index">
                <form id="createIndex">
                  <div class="form-group">
                    <label>Create Message Index:</label>
                  </div>
                  <button type="submit" data-loading-text="Creating..." class="btn btn-primary">Create Index</button>
                </form>
              </div>
              <div role="tabpanel" class="tab-pane" id="get-msg-list">
                <form id="getMsgList">
                  <label>Get Message List (Displays last 5 messages from the list):</label>
                  <div class="checkbox">
                    <label>
                      <input name="favorite" type="checkbox"> Filter by favorite
                    </label>
                  </div>
                  <div class="checkbox">
                    <label>
                      <input name="unread" type="checkbox"> Filter by unread
                    </label>
                  </div>
                  <div class="checkbox">
                    <label>
                      <input name="incoming" type="checkbox"> Filter by incoming
                    </label>
                  </div>
                  <div class="form-group">
                    <label for="filterByRecipients">Filter by recipients:</label>
                    <input type="text" class="form-control" name="keyword" id="filterByRecipients"
                      placeholder="555-555-5555, etc...">
                  </div>
                  <button type="submit" data-loading-text="Getting..." class="btn btn-primary">Get Message List</button>
                </form>
              </div> <!--./get-msg-list-->
              <div role="tabpanel" class="tab-pane" id="get-msg">
                <form id="getMsg">
                  <div class="form-group">
                    <label>Get Message:</label>
                  </div>
                  <div class="form-group">
                    <label for="getMsgId">Message Id</label>
                    <input type="text" class="form-control" id="getMsgId" name="getMsgId" placeholder="Message Id">
                  </div>
                  <button type="submit" data-loading-text="Getting..." class="btn btn-primary">Get Message</button>
                </form>
              </div> <!--./get-msg-->
              <div role="tabpanel" class="tab-pane" id="get-msg-content">
                <form id="getMsgContent">
                  <div class="form-group">
                    <label>Get Message Content:</label>
                  </div>
                  <div class="form-group">
                    <label for="contentMsgId">Message Id</label>
                    <input type="text" class="form-control" name="contentMsgId" id="contentMsgId"
                      placeholder="Message Id">
                  </div>
                  <div class="form-group">
                    <label for="contentPartNumber">Part Number</label>
                    <input type="text" class="form-control" name="contentPartNumber"  id="contentPartNumber"
                      placeholder="Part Number">
                  </div>
                  <button type="submit" data-loading-text="Getting..."
                    class="btn btn-primary">Get Message Content</button>
                </form>
              </div> <!--./get-msg-content-->
              <div role="tabpanel" class="tab-pane" id="get-delta">
                <form id="getDelta">
                  <div class="form-group">
                    <label>Get Delta:</label>
                  </div>
                  <div class="form-group">
                    <label for="msgState">Message State</label>
                    <input type="text" class="form-control" name="msgState" id="msgState" placeholder="Message State">
                  </div>
                  <button type="submit" data-loading-text="Getting..." class="btn btn-primary">Get Delta</button>
                </form>
              </div> <!--./get-delta-->
              <div role="tabpanel" class="tab-pane" id="get-msg-index-info">
                <form id="getMsgIndexInfo">
                  <div class="form-group">
                    <label>Get Message Index Info:</label>
                  </div>
                  <button type="submit" data-loading-text="Getting..."
                    class="btn btn-primary">Get Message Index Info</button>
                </form>
              </div> <!--./get-msg-index-info-->
              <div role="tabpanel" class="tab-pane" id="update-msg">
                <form id="updateMsg">
                  <div class="form-group">
                    <label>Update Message(s):</label>
                  </div>
                  <div class="form-group">
                    <label for="updateMsgId">Message Id</label>
                    <input type="text" class="form-control" name="updateMsgId" id="updateMsgId"
                    placeholder="Message Id" data-toggle="tooltip" data-placement="bottom"
                      data-title="More than one message ID can be separated by a comma (,)">
                  </div>
                  <div class="form-group">
                    <label for="updateStatus">Change Status:</label>
                    <select name="updateStatus" id="updateStatus" class="form-control">
                      <option>Read</option>
                      <option>Unread</option>
                    </select>
                  </div>
                  <button type="submit" data-loading-text="Updating..."
                    class="btn btn-primary">Update Message(s)</button>
                </form>
              </div>
              <div role="tabpanel" class="tab-pane" id="delete-msg">
                <form id="delMsg">
                  <div class="form-group">
                    <label>Delete Message(s):</label>
                  </div>
                  <div class="form-group">
                    <label for="deleteMsgId">Message Id</label>
                    <input type="text" class="form-control" name="deleteMsgId" id="deleteMsgId"
                      placeholder="Message Id" data-toggle="tooltip" data-placement="bottom"
                      data-title="More than one message ID can be separated by a comma (,)">
                  </div>
                  <button type="submit" data-loading-text="Deleting..."
                    class="btn btn-primary">Delete Message(s)</button>
                </form>
              </div>
              <div role="tabpanel" class="tab-pane" id="create-subscription">
                <form id="createSubscription">
                  <div class="form-group">
                    <label>Create Subscription:</label>
                  </div>
                  <div class="form-group">
                    <label for="callbackData">Callback Data</label>
                    <input type="text" class="form-control" name="callbackData" id="callbackData"
                      placeholder="Callback Data">
                  </div>
                  <div class="form-group">
                    <div class="checkbox">
                      <label>
                        <input name="subscriptionText" type="checkbox"> Text
                      </label>
                    </div>
                    <div class="checkbox">
                      <label>
                        <input name="subscriptionMms" type="checkbox"> MMS
                      </label>
                    </div>
                  </div>
                  <button type="submit" data-loading-text="Creating..."
                    class="btn btn-primary">Create Subscription</button>
                </form>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="update-subscription">
                <form id="updateSubscription">
                  <div class="form-group">
                    <label>Update Subscription:</label>
                  </div>
                  <div class="form-group">
                    <label for="callbackData">Callback Data</label>
                    <input type="text" class="form-control" name="updateCallbackData" id="updateCallbackData"
                      placeholder="Callback Data">
                  </div>
                  <div class="form-group">
                    <div class="checkbox">
                      <label>
                        <input name="updateSubscriptionText" type="checkbox"> Text
                      </label>
                    </div>
                    <div class="checkbox">
                      <label>
                        <input name="updateSubscriptionMms" type="checkbox"> MMS
                      </label>
                    </div>
                  </div>
                  <button type="submit" data-loading-text="Updating..."
                    class="btn btn-primary">Update Subscription</button>
                </form>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="get-subscription">
                <form id="getSubscription">
                  <div class="form-group">
                    <label>Get Subscription:</label>
                  </div>
                  <button type="submit" data-loading-text="Getting..."
                    class="btn btn-primary">Get Subscription</button>
                </form>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="delete-subscription">
                <form id="deleteSubscription">
                  <div class="form-group">
                    <label>Delete Subscription:</label>
                  </div>
                  <button type="submit" data-loading-text="Deleting..."
                    class="btn btn-primary">Delete Subscription</button>
                </form>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="view-notification-details">
                <form id="viewNotificationDetails">
                  <div class="form-group">
                    <label>Notification Details Subscription:</label>
                  </div>
                  <div class="alert alert-info">
                    Note: Webhooks requires apps to create a channel for receiving notifications. This app-specific
                    resource has already been created for this sample app.
                  </div>
                  <div id="channelTable"></div>
                  <div id="createSubscriptionAlert" class="alert alert-danger">
                    Webhooks requires apps to create subscriptions for customers' message inbox in order to
                    receive notifications. Create one using the tab option for 'Create Subscription'
                  </div>
                  <div id="receivingNotifications" class="hidden">
                    <div class="alert alert-info">
                      Note: Webhooks will provide a stream of notifications if a subscription and the user's inbox are
                      both active. For seeing notifications, you will have to receive / delete messages on the phone used
                      to authorize this sample app. You will only see notifications for the phone you used.
                    </div>
                    <div class="form-group">
                      <label>Receiving...</label>
                      <div class="progress">
                        <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100"
                          aria-valuemin="0" aria-valuemax="100"
                          style="width: 100%"> <span class="sr-only">Receiving...</span>
                        </div>
                      </div><!--./progress-->
                    </div>
                    <div id="notificationTable"></div>
                  </div><!--./hidden-->
                </form>
              </div><!--./tab-pane-->
            </div>
          </div>
        </div><!--./col-md-12-->
      </div><!--./row-->
      <div class="row">
        <div class="col-md-12">
          <div class="hidden" id="response"></div>
        </div><!--./col-md-12-->
      </div><!--./row-->
      <hr>
      <div class="row"><div class="col-md-12"><b>Server Time:&nbsp;</b><span id="serverTime"></span></div></div>
      <div class="row"><div class="col-md-12"><b>Client Time:</b> <script>document.write("" + new Date());</script></div></div>
      <div class="row"><div class="col-md-12"><b>User Agent:</b> <script>document.write("" + navigator.userAgent);</script></div></div>
      <hr>
      <div class="footer text-muted">
        <div class="row">
          <div class="col-sm-12 text-left">
            <p>
              <small>
                The application hosted on this site is a working example
                intended to be used for reference in creating products to
                consume AT&amp;T Services and not meant to be used as part of
                your product. The data in these pages is for test purposes only
                and intended only for use as a reference in how the services
                perform.
              </small>
            </p>
          </div> <!--./col-->
        </div> <!--./row-->
        <hr>
        <div class="row">
          <div class="text-left col-sm-6">
            <div class="col-sm-1">
              <a class="brand" href="https://developer.att.com" target="_blank">
                <img alt="AT&amp;T Developer" src="https://developer.att.com/static-assets/images/logo-globe.png">
              </a>
            </div>
            <div class="col-sm-11">
              <p>
                <small>
                  <a href="https://www.att.com/gen/general?pid=11561" target="_blank">Terms of Use</a>
                  <a href="https://www.att.com/gen/privacy-policy?pid=2506" target="_blank">Privacy Policy</a>
                  <a href="https://developer.att.com/support" target="_blank">Contact Us</a>
                  <br>
                  &#169; 2015 AT&amp;T Intellectual Property. All rights reserved.
                </small>
              </p>
            </div>
          </div>
          <div class="col-sm-6 left-border">
            <p class="text-right">
              <small>
                AT&amp;T, the AT&amp;T logo and all other AT&amp;T marks
                contained herein are trademarks of
                <br>
                AT&amp;T Intellectual Property and/or AT&amp;T affiliated
                companies. AT&amp;T 36USC220506
              </small>
            </p>
          </div>
        </div><!--./row-->
      </div><!--./footer-->
    </div><!--./container-->

    <!-- enable bootstrap custom tootips -->
    <script>$(function () { $('[data-toggle="tooltip"]').tooltip() });</script>

  </body>
</html>
<!-- vim: set ts=2 sts=2 sw=2 et : -->
