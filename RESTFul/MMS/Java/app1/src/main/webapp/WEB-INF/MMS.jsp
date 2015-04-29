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
    <title>AT&amp;T Sample Application - MMS</title>

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
        <h3 class="text-center">MMS</h3>
      </div>
      <div class="row">
        <h5 class="text-center">
        This sample application showcases sending MMS messages, checking status for sent MMS messages, and receiving
        notifications.
        </h5>
      </div>
      <hr>
      <div class="inline-row">
        <a class="btn btn-warning" id="github" href="#">Github</a>
        <a class="btn btn-warning" id="download" href="#">Download</a>
      </div><!--./row-->
      <hr>
      <div class="row">
        <div class="col-md-12">
          <div role="tabpanel">
            <!-- Nav tabs -->
            <ul class="nav nav-tabs" role="tablist">
              <li role="presentation" class="active">
                <a href="#send-mms" aria-controls="send-mms" role="tab" data-toggle="tab">Send MMS</a>
              </li>
              <li role="presentation">
                <a href="#get-status" aria-controls="get-status" role="tab"
                  data-toggle="tab">Get Delivery Status</a>
              </li>
              <li role="presentation">
                <a href="#receive-status" aria-controls="receive-status" role="tab"
                  data-toggle="tab">Receive Delivery Status</a>
              </li>
              <li role="presentation">
                <a href="#receive-msg" aria-controls="receive-msg" role="tab"
                  data-toggle="tab">Receive MMS</a>
              </li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content">
              <div role="tabpanel" class="tab-pane active" id="send-mms">
                <form id="sendMMS">
                  <div class="form-group">
                    <label for="address">Address</label>
                    <input type="text" class="form-control" name="address" id="address"
                      data-toggle="tooltip" data-placement="bottom"
                      data-title="Format must be one of: tel:+12065550199, tel:12065550199, tel:2065550199"
                      placeholder="tel:+12065550199">
                  </div>
                  <div class="checkbox">
                    <label>
                      <input name="receiveStatus" type="checkbox"> Receive Delivery Status
                    </label>
                  </div>
                  <div class="form-group">
                    <label for="sendMsgInput">Message:</label>
                    <select name="sendMsgInput" id="sendMsgInput" class="form-control">
                      <option>MMS Sample Message</option>
                    </select>
                  </div>
                  <div class="form-group">
                    <label for="attachmentInput">Attachment:</label>
                    <select name="attachmentInput" id="attachmentInput" class="form-control">
                      <option>None</option>
                      <option>att.gif</option>
                      <option>coupon.jpg</option>
                    </select>
                  </div>
                  <button type="submit" data-loading-text="Sending..." class="btn btn-primary">Send Message</button>
                </form>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="get-status">
                <form id="getDeliveryStatus">
                  <div class="form-group">
                    <label for="msgId">Message Id</label>
                    <input type="text" class="form-control" name="msgId" id="msgId" placeholder="Message Id">
                  </div>
                  <button type="submit" data-loading-text="Getting..." class="btn btn-primary">Get Delivery Status</button>
                </form>
              </div><!--/.tab-pane-->
              <div role="tabpanel" class="tab-pane" id="receive-status">
                <div class="form-group">
                  <label>Listening for any delivery status notifications...</label>
                  <div class="alert alert-info">
                    Note: The last three digits of the 'Address' column have been filtered
                  </div>
                  <div class="progress">
                    <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100"
                      aria-valuemin="0" aria-valuemax="100"
                      style="width: 100%"> <span class="sr-only">Receiving...</span>
                    </div>
                  </div><!--./progress-->
                </div>
                <div id="statusTable"></div>
              </div><!--/.tab-pane-->
              <div role="tabpanel" class="tab-pane" id="receive-msg">
                <div class="form-group">
                  <label>Listening for any messages sent to shortcode <span id="notificationShortcode"></span>...</label>
                  <div class="alert alert-info">
                    Note: The last three digits of the 'Sent From' column have been filtered
                  </div>
                  <div class="progress">
                    <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100"
                      aria-valuemin="0" aria-valuemax="100"
                      style="width: 100%"> <span class="sr-only">Receiving...</span>
                    </div>
                  </div><!--./progress-->
                </div>
                <div id="mmsImages"></div>
              </div><!--/.tab-pane-->
            </div><!--/.tab-content-->
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
