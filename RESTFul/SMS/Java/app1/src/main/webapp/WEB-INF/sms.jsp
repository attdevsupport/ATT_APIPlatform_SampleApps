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
    <title>AT&amp;T Sample Application - Basic SMS Service</title>

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
        <h3 class="text-center">Basic SMS Service</h3>
      </div>
      <div class="row">
        <h5 class="text-center">The SMS API sends SMS messages to one or more AT&T Wireless mobile phones in a single
          request. This API contains methods for sending and receiving messages and querying for the status of
          previously submitted SMS messages. For Mobile Originating messages, the SMS API allows you to poll the
          delivery status of the message or request that delivery status notifications be sent to a registered callback
          listener URI as soon as the message arrives.</h5>
      </div>
      <div class="row"><div class="col-xs-12"><hr></div></div>
      <div class="inline-row">
        <div class="col-lg-12">
          <div class="col-lg-2 col-sm-0 col-xs-0"></div>
          <div class="col-lg-4 col-sm-6 col-xs-12">
            <a class="btn btn-block btn-warning" id="github" href="#">Github</a>
          </div>
          <div class="col-lg-4 col-sm-6 col-xs-12">
            <a class="btn btn-block btn-warning" id="download" href="#">Download</a>
          </div>
          <div class="col-lg-2 col-sm-0 col-xs-0"></div>
        </div><!--/.col-lg-12-->
      </div><!--/.row-->
      <div class="row"><div class="col-xs-12"><hr></div></div>
      <div class="row">
        <div class="col-md-12">
          <div role="tabpanel">
            <!-- Nav tabs -->
            <ul class="nav nav-tabs" role="tablist">
              <li role="presentation" class="active">
                <a href="#send-sms" aria-controls="send-sms" role="tab" data-toggle="tab">Send SMS</a>
              </li>
              <li role="presentation">
                <a href="#delivery-status" aria-controls="delivery-status" role="tab" data-toggle="tab">
                  Get Delivery Status</a>
              </li>
              <li role="presentation">
                <a href="#messages" aria-controls="messages" role="tab" data-toggle="tab">Get Messages</a>
              </li>
              <li role="presentation" class="dropdown">
                <a href="#" id="notifications-tab" class="dropdown-toggle" data-toggle="dropdown"
                    aria-controls="notification-tab-contents">Notifications <span class="caret"></span></a>
                <ul class="dropdown-menu" role="menu" aria-labelledby="notifications-tab"
                    id="notification-tab-contents">
                  <li>
                    <a href="#receive-delivery-status" tabindex="-1" role="tab" data-toggle="tab"
                        aria-controls="receive-delivery-status">Delivery Status</a>
                  </li>
                  <li>
                    <a href="#receive-messages" tabindex="-1" role="tab" data-toggle="tab"
                        aria-controls="receive-messages">Messages</a>
                  </li>
                </ul>
              </li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content">
              <div role="tabpanel" class="tab-pane active" id="send-sms">
                <form id="sendSms">
                  <div class="form-group">
                    <label for="address">Addresses</label>
                    <input type="text" class="form-control" name="address" data-toggle="tooltip" 
                        data-placement="bottom" data-title="Multiple numbers can be submitted comma separated. Format of
                        each address must be: tel:15555555" placeholder="AT&amp;T mobile numbers">
                  </div>
                  <div class="form-group">
                    <label for="message">Message</label>
                    <textarea class="form-control" type="text" name="message" maxlength=120 rows="1">AT&amp;T Sample message</textarea>
                  </div><!--./form-group-->
                  <div class="checkbox">
                    <label>
                      <input name="deliveryNotificationStatus" type="checkbox">Receive Delivery Status Notification
                    </label>
                  </div>
                  <button type="submit" data-loading-text="Sending..." class="btn btn-primary">Send SMS</button>
                </form>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="delivery-status">
                <form id="getDeliveryStatus">
                  <div class="form-group">
                    <label for="messageId">Message ID</label>
                    <input type="text" class="form-control" name="messageId" data-toggle="tooltip" 
                        data-placement="bottom" data-title="Specifies the unique identifier for the SMS message that is
                        returned by the API Gateway." placeholder="Message ID">
                  </div>
                  <button type="submit" data-loading-text="Checking Status..." class="btn btn-primary">
                    Get Delivery Status
                  </button>
                </form>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="messages">
                <label>Get messages from short code <span id="shortCodePoll"></span></label>
                <form id="getMessages">
                  <button type="submit" data-loading-text="Checking Messages..." class="btn btn-primary">
                    Get Messages
                  </button>
                </form>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="receive-delivery-status">
                <label>Receiving delivery status...</label>
                <div class="progress">
                  <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100"
                      aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
                </div><!--./progress-->
                <span class="sr-only">Receiving...</span>
                <div id="receiveStatusTable"></div>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="receive-messages">
                <label>Receiving messages from short code <span id="shortCodeReceiveMessages"></span></label>
                <div class="progress">
                  <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100"
                      aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
                </div><!--./progress-->
                <span class="sr-only">Receiving...</span>
                <div id="receiveMessagesTable"></div>
              </div><!--./tab-pane-->
            </div><!--./tab-content-->
          </div> <!--tabpanel-->
        </div><!--./col-md-12-->
      </div><!--./row-->
      <div class="row">
        <div class="col-md-12">
          <div class="hidden" id="response"></div>
        </div><!--./col-md-12-->
      </div><!--./row-->
      <div class="row"><div class="col-xs-12"><hr></div></div>
      <div class="row"><div class="col-md-12"><b>Server Time:&nbsp;</b><span id="serverTime"></span></div></div>
      <div class="row"><div class="col-md-12"><b>Client Time:</b> <script>document.write("" + new Date());</script></div></div>
      <div class="row"><div class="col-md-12"><b>User Agent:</b> <script>document.write("" + navigator.userAgent);</script></div></div>
        <div class="row"><div class="col-xs-12"><hr></div></div>
      <div class="footer text-muted">
        <div class="row">
          <div class="col-sm-12 text-left">
            <p>
              <small>
                The application hosted on this site is a working example intended to be used for reference in creating
                products to consume AT&amp;T Services and not meant to be used as part of your product. The data in
                these pages is for test purposes only and intended only for use as a reference in how the services
                perform.
              </small>
            </p>
          </div> <!--./col-->
        </div> <!--./row-->
        <div class="row"><div class="col-xs-12"><hr></div></div>
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
                AT&amp;T, the AT&amp;T logo and all other AT&amp;T marks contained herein are trademarks of
                <br>
                AT&amp;T Intellectual Property and/or AT&amp;T affiliated companies. AT&amp;T 36USC220506
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
<!-- vim: set ts=2 sts=2 sw=2 cc=120 tw=120 et : -->
