<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="MMS_App1" %>

<!DOCTYPE html>
<!-- 
* Copyright 2014 AT&T
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
-->
<!--[if lt IE 7]> <html class="ie6" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="ie7" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="ie8" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
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
<body onload="setup()">

    <form id="form1" runat="server">
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
          <div role="tabpanel" id="tabs">
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
              <%--</li>
                </ul>--%>
            <!-- Tab panes -->
            <div class="tab-content">
              <div role="tabpanel" class="tab-pane active" id="send-mms">
                  <div class="inputFields">
                        <label>Address</label> 
                        <br />
                        <input type="text" name="address" class="form-control" data-toggle="tooltip" data-placement="bottom"
                      data-title="Format must be one of: tel:15555555"
                      placeholder="tel:15551234" runat="server" id="address" />
                      <br />
                      <label>
                            <asp:CheckBox ID="chkGetOnlineStatus" name="chkGetOnlineStatus" runat="server" title="If Checked, Delivery status is sent to the listener, use feature 3 to view the status" />
                            Receive Delivery Status<br />
                        </label>
                      <br />
                        <label>
                            Message:
                            <asp:DropDownList name="subject" class="form-control" id="subject" runat="server">
                            <asp:ListItem>MMS Sample Message</asp:ListItem>
                            </asp:DropDownList>
                        </label>
                      <br />
                        <label>
                            Attachment:
                            <asp:DropDownList ID="attachment" class="form-control" runat="server" name="attachment">
                            </asp:DropDownList>
                        </label>
                        <br />
                        <asp:Button ID="sendMms" runat="server" class="btn btn-primary" Text="Send Message" OnClick="SendMessage_Click" />
                   </div>
                  <% if (!string.IsNullOrEmpty(sendMessageResponseError))
                       { %>
                    <div class="errorWide">
                        <strong>ERROR: </strong>
                        <br />
                        <%= sendMessageResponseError %>
                    </div>
                    <% } %>
                    <% if (!string.IsNullOrEmpty(sendMessageResponseSuccess))
                       { %>
                    <div class="successWide">
                        <div class="alert alert-info" align="left">
                            <strong>SUCCESS:</strong>
                            <br />
                        </div>  
                        <strong>messageId: </strong>
                        <%= sendMMSResponseData.outboundMessageResponse.messageId%>
                        <% if (sendMMSResponseData.outboundMessageResponse.resourceReference != null)
                            {%>
                        <br />
                        <strong>resourceURL: </strong>
                        <%= sendMMSResponseData.outboundMessageResponse.resourceReference.resourceURL%>
                        <%} %>
                    </div>
                    <% } %>
                    <div class="lightBorder">
                    </div>
                
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="get-status">
                <div class="inputFields">
                    <label>Message Id</label>
                    <input maxlength="20" class="form-control" name="mmsId" placeholder="Message Id" runat="server" id="mmsId" />
                    <br/>
                    <asp:Button ID="getStatus" runat="server" class="btn btn-primary" Text="Get Delivery Status" OnClick="GetStatus_Click" />
                    </div>
                     <% if (!string.IsNullOrEmpty(getDeliveryStatusResponseError))
                       { %>
                    <div class="errorWide">
                        <strong>ERROR: </strong>
                        <br />
                        <%= getDeliveryStatusResponseError %>
                    </div>
                    <%} %>
                    <% if (!string.IsNullOrEmpty(getDeliveryStatusResponseSuccess))
                       { %>
                            <div class="successWide">
                                <div class="alert alert-info" align="left">
                                <strong>SUCCESS:</strong>
                                <br />
                                </div>
                                <strong>ResourceUrl: </strong>
                                <%=getMMSDeliveryStatusResponseData.DeliveryInfoList.ResourceURL%><br />
                            </div>
                            <div class="table-responsive">
                            <table class="table table-condensed table-striped table-bordered">
                                <thead>
                                    <tr>
                                        <th>
                                            Id
                                        </th>
                                        <th>
                                            Address
                                        </th>
                                        <th>
                                            DeliveryStatus
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <% foreach (DeliveryInfo delinfo in getMMSDeliveryStatusResponseData.DeliveryInfoList.DeliveryInfo)
                                       {%>
                                    <tr>
                                        <td data-value="Id">
                                            <%=delinfo.Id %>
                                        </td>
                                        <td data-value="Address">
                                            <%= delinfo.Address %>
                                        </td>
                                        <td data-value="DeliveryStatus">
                                            <%= delinfo.Deliverystatus %>
                                        </td>
                                    </tr>
                                    <% }%>
                                </tbody>
                            </table>
                                </div>
                    <% } %>                        
              </div><!--/.tab-pane-->
              <div role="tabpanel" class="tab-pane" id="receive-status">
                  <asp:Button ID="receiveStatusBtn" runat="server" class="btn btn-primary" Text="Refresh Notifications" OnClick="receiveStatusBtn_Click" />
                  <br />
                  <br />
                  <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th>
                                                messageId
                                            </th>
                                            <th>
                                                address
                                            </th>
                                            <th>
                                                deliveryStatus
                                            </th>
                                        </tr>
                                    </thead>
                            <% if (receiveMMSDeliveryStatusResponseData != null && receiveMMSDeliveryStatusResponseData.Count > 0)
                               { %>
                                    <tbody>
                                        <% foreach (deliveryInfoNotification deinfo in receiveMMSDeliveryStatusResponseData)
                                           { %>
                                        <tr>
                                            <td data-value="messageId">
                                                <%=deinfo.messageId%>
                                            </td>
                                            <td data-value="address">
                                                <%=deinfo.deliveryInfo.address%>
                                            </td>
                                            <td data-value="deliveryStatus">
                                                <%=deinfo.deliveryInfo.deliveryStatus%>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                            <% } %>
                                </table>
                      </div>
                                <br />
              </div><!--/.tab-pane-->
              <div role="tabpanel" class="tab-pane" id="receive-msg">
                  <asp:Button ID="receiveMessageBtn" runat="server" class="btn btn-primary" Text="Refresh Rececived Messages" OnClick="receiveMessageBtn_Click" />
                        <div id="webGallery">
                        <label>
                            Web gallery of MMS photos sent to short code</label>
                        <p>
                            Photos sent to short code
                            <%= ListenerShortCode %>
                            :
                            <%= totalImages.ToString() %></p>
                        <% foreach (ImageData imgdata in imageList)
                           {%>
                        <img src= "<%= imgdata.path %>" width="150" border="0" alt="image" /><br />
                        <strong>Sent from:&nbsp;</strong><%= imgdata.senderAddress %><br />
                        <strong>On:&nbsp;</strong><%= imgdata.date %><br />
                        <strong>Text:&nbsp;</strong><%= imgdata.text  %><br />
                        <% } %>
                    </div>
                  <br />
                  <br />
                <div id="statusTable"></div>
              </div><!--/.tab-pane-->
            </div><!--/.tab-content-->
          </div>
        </div><!--./col-md-12-->
      </div><!--./row-->
         <hr>
      <div class="row"><div class="col-md-12"><b>Server Time:&nbsp;</b><span id="serverTime"></span><%= String.Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) + " UTC" %></div></div>
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

        <% if (!string.IsNullOrEmpty(showSendMsg))
           { %>
        <script type="text/javascript">   $('#tabs a[href="#send-mms"]').tab('show');</script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetStatus))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-status"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showReceiveStatus))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#receive-status"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showReceiveMessage))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#receive-msg"]').tab('show'); </script>
        <% } %>
        
    </form>
</body>
</html>
