<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="IMMN_App1" %>
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
 <%--   <script src="js/config.js"></script>--%>
    <%--<script src="js/form_handler.js"></script>--%>
    <%--<script src="js/response_handler.js"></script>--%>
    <%--<script src="js/sample_app.js"></script>--%>

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
 
    <script src="scripts/utils.js" type="text/javascript"></script>
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
        <form id="form1" runat="server">
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
          <div role="tabpanel" id="tabs">
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
                      data-toggle="tab" aria-controls="create-subscription">Webhooks: Create Subscription</a>
                  </li>
                  <li>
                    <a href="#update-subscription" tabindex="-1" role="tab" id="update-subscription-tab"
                      data-toggle="tab" aria-controls="update-subscription">Webhooks: Update Subscription</a>
                  </li>
                  <li>
                    <a href="#get-subscription" tabindex="-1" role="tab" id="get-subscription-tab" data-toggle="tab"
                      aria-controls="get-subscription">Webhooks: Get Subscription</a>
                  </li>
                  <li>
                    <a href="#delete-subscription" tabindex="-1" role="tab" id="delete-subscription-tab"
                      data-toggle="tab" aria-controls="delete-subscription">Webhooks: Delete Subscription</a>
                  </li>
                  <li>
                    <a href="#view-notification-details" tabindex="-1" role="tab" id="view-notification-details-tab"
                      data-toggle="tab" aria-controls="view-notification-details">Webhooks: View Notifications</a>
                  </li>
                </ul>
              </li>
            </ul>
            <!-- Tab panes -->
              <%--<asp:HiddenField ID="selected_tab" runat="server" />--%>

            <div class="tab-content">
              <div role="tabpanel" class="tab-pane active" id="send-msg">
                <%--<-- <form id="sendMsg" runat="server"> -->--%>
                      <div class="inputFields">
                          <label >
                            Address
                            <br />
                            <asp:TextBox ID="Address" class="form-control" data-toggle="tooltip" data-placement="bottom"
                      data-title="Format must be one of: tel:15555555" name="message" placeholder="Address" runat="server"></asp:TextBox>
                          </label>
                          <br/>
                            <label>
                                <asp:CheckBox ID="groupCheckBox" runat="server" name="groupCheckBox" /> Group
                            </label>
                          <br/>
                            <label>
                                Message: <i>max 200 characters are allowed</i>
                                <br/>
                                <asp:TextBox ID="message" class="form-control" name="message" placeholder="ATT IMMN sample message" maxlength="200" runat="server"></asp:TextBox>
                            </label>
                          <br/>
                            <label>
                                Subject: <i>max 30 characters are allowed</i>
                                <br/>
                                    <asp:TextBox ID="subject" class="form-control"  runat="server" name="subject" placeholder="Subject : ATT IMMN APP"  maxlength="30"></asp:TextBox>
                            </label>
                          <br/>
                            <label>
                                Attachment:
                            </label>
                          <br/>
                            <select name="attachment" class="form-control" id="attachment" runat="server">
                                 <option>None</option>
                            </select><br />
                            <asp:Button ID="sendMessage" runat="server" class="btn btn-primary" Text="Send Message" OnClick="Button1_Click" />
                        </div>
                        <% if (!string.IsNullOrEmpty(sendMessageSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>
                            <%=sendMessageSuccessResponse.ToString() %>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(sendMessageErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=sendMessageErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%--</form>--%>
              </div>
              <div role="tabpanel" class="tab-pane" id="create-msg-index">
                <%--<form id="createIndex" runat="server"> -->--%>
                  		<div class="inputFields">
                        <label>
                            Create Message Index:
                            <br />
                            <br />
                            
                            <asp:Button ID="createIndexId" class="btn btn-primary" runat="server" Text="Create Index" OnClick="createMessageIndex_Click"/>
					    </div>
                        </label>
					        
                        <% if (!string.IsNullOrEmpty(createMessageIndexSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>
                            <%=createMessageIndexSuccessResponse.ToString()%>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(createMessageIndexErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=createMessageIndexErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%--</form>--%>
              </div>
              <div role="tabpanel" class="tab-pane" id="get-msg-list">
                <%--<form id="getMsgList" runat="server"> -->--%>
                  <label>Get Message List (Displays last 5 messages from the list):</label>
                  <br />
                  <br />
                        <div class="inputFields">
                            <label>
                                <asp:CheckBox ID="CheckBox1" runat="server" name="groupCheckBox" /> Filter by favorite
                            </label>
                            <br/>
                             <label>
                                <asp:CheckBox ID="CheckBox2" runat="server" name="groupCheckBox" /> Filter by unread flag
                            </label>
                            <br/>
                             <label>
                                <asp:CheckBox ID="CheckBox3" runat="server" name="groupCheckBox" /> Filter by incoming flag
                            </label>
                            <br/>
                            <label>
                                Filter by recipients:
                                <br />
							<asp:TextBox ID="FilterKeyword" runat="server" name="FilterKeyword" placeholder="555-555-5555"></asp:TextBox>
                            </label>
                            <br />
                            <br />
                            <asp:Button ID="getMessageLst" class="btn btn-primary" runat="server" Text="Get Message List" OnClick="getMessageList_Click" />
                        </div>
                        <br/>			
                        <% if (!string.IsNullOrEmpty(getMessageListSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>
                            
                        </div>
                        <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered">
                          <thead>
							<tr>
							  <th>limit</th>
							  <th>offset</th>
							  <th>total</th>
							  <th>state</th>
							  <th>cache status</th>
							  <th>failed messages</th>							  
							</tr>
						  </thead>
						  <tbody>
							<tr>
							  <td><%= csGetMessageListDetailsResponse.limit%></td>
							  <td><%= csGetMessageListDetailsResponse.offset%></td>
							  <td><%= csGetMessageListDetailsResponse.total%></td>
							  <td><%= csGetMessageListDetailsResponse.state%></td>
							  <td><%= csGetMessageListDetailsResponse.cacheStatus%></td>
							  <td><%= csGetMessageListDetailsResponse.failedMessages%></td>							  
							</tr>
						  </tbody>
						</table>
						</div>
						<% 
						int count = 1;
						foreach (var message in csGetMessageListDetailsResponse.messages)
						{ 
						if(count > 5)
						{
						break;
						}
                        %>
                        Message <%=count%>
						<br/> 
                        <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered">
						  <thead>
							<tr>
							  <th>message id</th>
							  <th>from</th>
							  <th>recipients</th>
							  <th>text</th>
							  <th>timestamp</th>
							  <th>isFavorite</th>
							  <th>isUnread</th>
							  <th>isIncoming</th>
							  <th>type</th>
							</tr>
						  </thead>
						  <tbody>
							<tr>
							  <td><%= message.messageId%></td>
							  <td><%= message.from.value%></td>
							  <td><%= message.recipients[0].value%></td>
							  <td><%= message.text%></td>
							  <td><%= message.timeStamp%></td>
							  <td><%= message.isFavorite%></td>
							  <td><%= message.isUnread%></td>
							  <td><%= message.isIncoming%></td>
							  <td><%= message.type%></td>
							</tr>
						  </tbody>
						</table> 
                        </div> 
                        <%
                        count = count+1; 
                        } %> 
						<%                       
                        } %>
                        <% if (!string.IsNullOrEmpty(getMessageListErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=getMessageListErrorResponse.ToString()%>
                        </div>
                        <% } %>				        
                        <br />
                <%--<-- </form> -->--%>
              </div> <!--./get-msg-list-->
              <div role="tabpanel" class="tab-pane" id="get-msg">
<%--                < <form id="getMsg" runat="server"> -->--%>
                        <label>Get Message </label>
                        <div class="inputFields">
                            <label>Message Id
                                <br />
                            <asp:TextBox ID="MessageId" value="" runat="server" name="MessageId" placeholder="Message ID"></asp:TextBox>
                            </label>
                            <br />
                            <asp:Button ID="btnGetMessage" class="btn btn-primary" runat="server" Text="Get Message" OnClick="getMessage_Click" />
                        </div>
                        <% if (!string.IsNullOrEmpty(getMessageSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>      
                        </div>
                        <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered">
                          <thead>
							<tr>
							  <th>message id</th>
							  <th>from</th>
							  <th>recipients</th>
							  <th>text</th>
							  <th>timestamp</th>
							  <th>isFavorite</th>
							  <th>isUnread</th>
							  <th>isIncoming</th>
							  <th>type</th>
							</tr>
						  </thead>
						  <tbody>
							<tr>
							  <td><%= getMessageDetailsResponse.messageId%></td>
							  <td><%= getMessageDetailsResponse.from.value%></td>
							  <td><%= getMessageDetailsResponse.recipients[0].value%></td>
							  <td><%= getMessageDetailsResponse.text%></td>
							  <td><%= getMessageDetailsResponse.timeStamp%></td>
							  <td><%= getMessageDetailsResponse.isFavorite%></td>
							  <td><%= getMessageDetailsResponse.isUnread%></td>
							  <td><%= getMessageDetailsResponse.isIncoming%></td>
							  <td><%= getMessageDetailsResponse.type%></td>
							</tr>
						  </tbody>
						</table>
                        </div> 
                        <% } %>
                        <% if (!string.IsNullOrEmpty(getMessageErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=getMessageErrorResponse.ToString()%>
                        </div>
                        <% } %>
                        <br />
<%--                <-- </form> -->--%>
              </div> <!--./get-msg-->
              <div role="tabpanel" class="tab-pane" id="get-msg-content">
<%--                <-- <form id="getMsgContent" runat="server"> -->--%>
                        <label>Get Message Content:</label>
                        <div class="inputFields">
                            <label>Message Id
                            <br />
                            <asp:TextBox ID="MessageIdForContent" class="form-control" runat="server" name="MessageIdForContent" placeholder="Message ID"></asp:TextBox>
                            </label>
                            <br />
                            <label>Part Number
                            <br />
                            <asp:TextBox ID="PartNumberForContent" class="form-control" runat="server" name="PartNumberForContent" placeholder="Part Number"></asp:TextBox>
                            </label>
                            <br />
                            <asp:Button ID="getMessageContent" class="btn btn-primary" runat="server" Text="Get Message Content" OnClick="GetMessageContentByIDnPartNumber" />
                        </div>
                        <% if (!string.IsNullOrEmpty(getMessageContentSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>                                                  
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(getMessageContentErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=getMessageContentErrorResponse.ToString()%>
                        </div>
                        <% } %>
                        <br />
                <%--<-- </form> -->--%>
              </div> <!--./get-msg-content-->
              <div role="tabpanel" class="tab-pane" id="get-delta">
                <%--<-- <form id="getDelta" runat="server"> -->--%>
                        <label>Get Delta: </label>
                        <br />
                        <div class="inputFields">
                            <label>Message State
                            <br />
                            <asp:TextBox ID="MessageIdForDelta" class="form-control" runat="server" name="MessageIdForDelta" placeholder="Message State"  ></asp:TextBox>
                            </label>
                            <br />
                            <asp:Button ID="btnGetDelta" class="btn btn-primary" runat="server" Text="Get Delta" OnClick="getDelta_Click" />
                        </div>
                        <% if (!string.IsNullOrEmpty(deltaSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>                            
                        </div>
                        <b>Delta Type : TEXT</b>
                        <% int count = 0;
                                foreach (var deltaRes in csDeltaResponse.delta)
                               { 
                               if (deltaRes.type.Equals("TEXT", StringComparison.InvariantCultureIgnoreCase)) {
                               %>
                                <div class="table-responsive">
                                <table class="table table-condensed table-striped table-bordered">                                                           
								  <thead>
									<tr>
									  <th>DeltaOperation</th>
									  <th>MessageId</th>
									  <th>Favorite</th>
									  <th>Unread</th>									  
									</tr>
								  </thead>
                                <tbody>
                                    <%  
									int i = 0; 
									int addsize = deltaRes.adds.Count;										
									while(addsize > 0 && i < 6 && deltaRes.adds[i] != null)
                                     { %>                                 
                                    <tr>
									  <td>ADD</td>
									  <td><%= deltaRes.adds[i].messageId%></td>
									  <td><%= deltaRes.adds[i].isFavorite%></td>
									  <td><%= deltaRes.adds[i].isUnread%></td>										  
					                </tr>
									<%
									i = i+1; 								
                                    } %>
                                     <%  
									i = 0; 
									int updsize = deltaRes.updates.Count;										
									while(updsize > 0 && i < 6 && deltaRes.updates[i] != null)
                                     { %>                                 
                                    <tr>
									  <td>UPDATE</td>
									  <td><%= deltaRes.updates[i].messageId%></td>
									  <td><%= deltaRes.updates[i].isFavorite%></td>
									  <td><%= deltaRes.updates[i].isUnread%></td>										  
					                </tr>
									<%
									i = i+1; 								
                                    } %>
                                     <%  
									i = 0; 
									int delsize = deltaRes.deletes.Count;										
									while(delsize > 0 && i < 6 && deltaRes.deletes[i] != null)
                                     { %>                                 
                                    <tr>
									  <td>DELETE</td>
									  <td><%= deltaRes.deletes[i].messageId%></td>
									  <td><%= deltaRes.deletes[i].isFavorite%></td>
									  <td><%= deltaRes.deletes[i].isUnread%></td>										  
					                </tr>
									<%
									i = i+1; 								
                                    } %>
                                    </tbody>
                            </table>
                            </div>
                         <% } %>
                        
                        <% 
                        count = count + 1;
                        } %>
                        <b>Delta Type : MMS</b>
                        <% count = 0;
                                foreach (var deltaRes in csDeltaResponse.delta)
                               { 
                               if (deltaRes.type.Equals("TEXT", StringComparison.InvariantCultureIgnoreCase)) {
                               %>
                                <div class="table-responsive">
                                <table class="table table-condensed table-striped table-bordered">                                                           
								  <thead>
									<tr>
									  <th>DeltaOperation</th>
									  <th>MessageId</th>
									  <th>Favorite</th>
									  <th>Unread</th>									  
									</tr>
								  </thead>
                                <tbody>
                                    <%  
									int i = 0; 
									int addsize = deltaRes.adds.Count;										
									while(addsize != 0 && i < 6 && deltaRes.adds[i] != null)
                                     { %>                                 
                                    <tr>
									  <td>ADD</td>
									  <td><%= deltaRes.adds[i].messageId%></td>
									  <td><%= deltaRes.adds[i].isFavorite%></td>
									  <td><%= deltaRes.adds[i].isUnread%></td>										  
					                </tr>
									<%
									i = i+1; 								
                                    } %>
                                     <%  
									i = 0; 
									int updsize = deltaRes.updates.Count;										
									while(updsize != 0 && i < 6 && deltaRes.updates[i] != null)
                                     { %>                                 
                                    <tr>
									  <td>UPDATE</td>
									  <td><%= deltaRes.updates[i].messageId%></td>
									  <td><%= deltaRes.updates[i].isFavorite%></td>
									  <td><%= deltaRes.updates[i].isUnread%></td>										  
					                </tr>
									<%
									i = i+1; 								
                                    } %>
                                     <%  
									i = 0; 
									int delsize = deltaRes.deletes.Count;										
									while(delsize != 0 && i < 6 && deltaRes.deletes[i] != null)
                                     { %>                                 
                                    <tr>
									  <td>DELETE</td>
									  <td><%= deltaRes.deletes[i].messageId%></td>
									  <td><%= deltaRes.deletes[i].isFavorite%></td>
									  <td><%= deltaRes.deletes[i].isUnread%></td>										  
					                </tr>
									<%
									i = i+1; 								
                                    } %>
                                    </tbody>
                            </table>
                            </div>
                         <% } %>
                        
                        <% 
                        count = count + 1;
                        } 						 
                        } %>
                        
                        <% if (!string.IsNullOrEmpty(deltaErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=deltaErrorResponse.ToString()%>
                        </div>
                        <% } %>
                        <br />
                <%--!-- </form> -->--%>
              </div> <!--./get-delta-->
              <div role="tabpanel" class="tab-pane" id="get-msg-index-info">
                <%--!-- <form id="getMsgIndexInfo" runat="server"> -->--%>
                        <label>Get Message Index Info:</label>
                        <br />
                        <div class="inputFields">
                            <asp:Button ID="getMessageIndexInfo" class="btn btn-primary" runat="server" Text="Get Message Index Info" OnClick="getMessageIndex_Click" />
                        </div>
                        <% if (!string.IsNullOrEmpty(messageIndexSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>                            
                        </div>
                        <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered">
                          <thead>
							<tr>
							  <th>Status</th>
							  <th>State</th>
							  <th>Message Count</th>
							</tr>
						  </thead>
						  <tbody>
							<tr>
							  <td data-value="Username"><%= getMessageIndexInfoResponse.status%></td>
							  <td data-value="Password"><%= getMessageIndexInfoResponse.state%></td>
							  <td data-value="https url"><%= getMessageIndexInfoResponse.messageCount%></td>							
							</tr>
						  </tbody>
						</table>
                        </div>  
                        <% } %>
                        <% if (!string.IsNullOrEmpty(messageIndexErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=messageIndexErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%--!-- </form>->--%>
              </div> <!--./get-msg-index-info-->
              <div role="tabpanel" class="tab-pane" id="update-msg">
                <%--!-- <form id="updateMsg" runat="server"> -->--%>
				        <label>Update Message(s)</label>
					    <div class="inputFields">
					        <label>Message Id
                            <br />
                            <asp:TextBox ID="updateMessageId" class="form-control" runat="server" name="updateMessageId" placeholder="Message ID" data-toggle="tooltip" data-placement="bottom"
                      data-title="More than one message ID can be separated by a comma (,)" ></asp:TextBox>
                            </label>
                            <br />
						    <label> Change Status : 
                            <br />
                            <input type="radio" id="read" runat="server" name="changeStatus" value="Read" checked /> Read
                            <input type="radio" id="unread" runat="server" name="changeStatus" value="UnRead" /> UnRead
					        </label>
                            <br />
                            <asp:Button ID="btnUpdateMessage" class="btn btn-primary" runat="server" Text="Update Message(s)" OnClick="updateMessage_Click" />
					    </div>
                        <% if (!string.IsNullOrEmpty(updateMessageSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>
                            <%=updateMessageSuccessResponse.ToString()%>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(updateMessageErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=updateMessageErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%--!-- </form> -->--%>
              </div>
              <div role="tabpanel" class="tab-pane" id="delete-msg">
                <%--!-- <form id="delMsg" runat="server"> -->--%>
				        <label>Delete Message(s)</label>
                        <br />
					    <div class="inputFields">
					        <label>Message Id
                            <br />
                            <asp:TextBox ID="deleteMessageId" class="form-control" runat="server" name="deleteMessageId" placeholder="Message ID" data-toggle="tooltip" data-placement="bottom"
                      data-title="More than one message ID can be separated by a comma (,)"></asp:TextBox>
                            </label>
                            <br />
                            <asp:Button ID="deleteMessages" class="btn btn-primary" runat="server" Text="Delete Message(s)" OnClick="deleteMessage_Click" />                              												
					    </div>
                        <% if (!string.IsNullOrEmpty(deleteMessageSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>
                            <%=deleteMessageSuccessResponse.ToString()%>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(deleteMessageErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=deleteMessageErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%---- </form> -->--%>
              </div>
              <div role="tabpanel" class="tab-pane" id="create-subscription">
                <%---- <form id="createSubscription" runat="server"> -->--%>
                            <label>Create Subscription:</label>
                        <br/>
                        <br/>
                        <div class="inputFields">
							<label>Callback Data
                            <br />
							<asp:TextBox ID="callbackText" class="form-control" runat="server" name="FilterKeyword" placeholder="Callback Data"></asp:TextBox>
                            </label>
                            <br />
                            <label>
                                <asp:CheckBox ID="subscribeCheckBox1" runat="server" name="subscribeCheckBox1" /> TEXT
                            </label>
                            <br />
                             <label>
                                <asp:CheckBox ID="subscribeCheckBox2" runat="server" name="subscribeCheckBox2" /> MMS
                            </label>
                            <br />
                            
                            <asp:Button ID="Button1" class="btn btn-primary" runat="server" Text="Create Subscription" OnClick="createSubscription_Click" />
                        </div>
                        <% if (!string.IsNullOrEmpty(createSubscriptionSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>      
                        </div>
                        <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered">
                          <thead>
							<tr>
							  <th>subscription Id</th>
							  <th>expiresIn</th>
							</tr>
						  </thead>
						  <tbody>
							<tr>
							  <td><%= createSubscriptionResponse.subscriptionId%></td>
							  <td><%= createSubscriptionResponse.expiresIn%></td>
							</tr>
						  </tbody>
						</table> 
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(createSubscriptionErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=createSubscriptionErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%--!-- </form> -->--%>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="update-subscription">
                <%--!-- <form id="updateSubscription" runat="server">-->--%>
                            <label>Update Subscription:</label>
                        <br/>                
                        <br/>
                        <div class="inputFields">
							<label>Callback Data
                            <br />
							<asp:TextBox ID="callbackText2" class="form-control" runat="server" name="FilterKeyword" placeholder="Callback Data"></asp:TextBox>
                            </label>
                            <br/>
                            <label>
                                <asp:CheckBox ID="subscribeCheckBox3" runat="server" name="subscribeCheckBox3" /> TEXT
                            </label>
                            <br/>
                            
                             <label>
                                <asp:CheckBox ID="subscribeCheckBox4" runat="server" name="subscribeCheckBox4" /> MMS
                            </label>
                            
                            <br />
                            <asp:Button ID="Button2" class="btn btn-primary" runat="server" Text="Update Subscription" OnClick="updateSubscription_Click" />
                        </div>
                        <% if (!string.IsNullOrEmpty(updateSubscriptionSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>      
                        </div>
                        <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered">
                          <thead>
							<tr>
							  <th>subscription id</th>
							  <th>expiresIn</th>
							</tr>
						  </thead>
						  <tbody>
							<tr>
							  <td><%= updateSubscriptionResponse.subscriptionId%></td>
							  <td><%= updateSubscriptionResponse.expiresIn%></td>
							</tr>
						  </tbody>
						</table> 
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(updateSubscriptionErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=updateSubscriptionErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%--!-- </form> -->--%>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="get-subscription">
                <%--!-- <form id="getSubscription" runat="server"> -->--%>
                  <label>Get Subscription:</label>
                  <br />
                        <div class="inputFields">
                            <asp:Button ID="Button4" class="btn btn-primary" runat="server" Text="Get Subscription" OnClick="getSubscription_Click" />
                        </div>
                        <% if (!string.IsNullOrEmpty(getSubscriptionSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>      
                        </div>
                        <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered">
                          <thead>
							<tr>
							  <th>Subscription Id</th>
							  <th>Expires In</th>
							  <th>Queues</th>
							  <th>Callback Data</th>
							</tr>
						  </thead>
						  <tbody>
							<tr>
							  <td><%= getSubscriptionResponse.subscriptionId%></td>
							  <td><%= getSubscriptionResponse.expiresIn%></td>
							  <td><%= getSubscriptionResponseFilters%></td>
							  <td><%= getSubscriptionResponse.callbackData%></td>
							</tr>
						  </tbody>
						</table> 
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(getSubscriptionErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=getSubscriptionErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%--!-- </form> -->--%>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="delete-subscription">
                <%--!-- <form id="deleteSubscription" runat="server"> -->--%>
                  <label>Delete Subscription:</label>
                  <br />
                        <div class="inputFields">
                            <asp:Button ID="Button3" class="btn btn-primary" runat="server" Text="Delete Subscription" OnClick="deleteSubscription_Click" />
                        </div>
                        <% if (!string.IsNullOrEmpty(deleteSubscriptionSuccessResponse))
                           { %>
                        <div class="alert alert-success" align="left">
                            <strong>SUCCESS:</strong>      
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(deleteSubscriptionErrorResponse))
                           { %>
                        <div class="alert alert-danger">
                            <strong>ERROR:</strong><br />
                            <%=deleteSubscriptionErrorResponse.ToString()%>
                        </div>
                        <% } %>
                <%--!-- </form> -->--%>
              </div><!--./tab-pane-->
              <div role="tabpanel" class="tab-pane" id="view-notification-details">
                <%--!-- <form id="viewNotificationDetails" runat="server"> -->--%>
                        <label>Notification Details:</label>
                  <br />
                  <div class="alert alert-info">
                    Note: Webhooks requires apps to create a channel for receiving notifications. This app-specific
                    resource has already been created for this sample app.
                  </div>
                  <div id="channelTable"></div>
                  <div id="createSubscriptionAlert" class="alert alert-info">
                    Webhooks requires apps to create subscriptions for customers' message inbox in order to
                    receive notifications. Create one using the tab option for 'Webhooks: Create Subscription'
                  </div>
                  <%--<div id="receivingNotifications" class="hidden">--%>
                    <div class="alert alert-info">
                      Note: Webhooks will provide a stream of notifications if a subscription and the user's inbox are
                      both active. For seeing notifications, you will have to receive / delete messages on the phone used
                      to authorize this sample app. You will only see notifications for the phone you used.
                    </div>
                      <br />
                        <label>Notification Payload
                        </label>
                        <div class="inputFields">
                        <% if (notificationObjArray != null && notificationObjArray.Count != 0)
                        {%>
                        <%  foreach (var notificationObject in notificationObjArray)
                        {%>
                        <% if (notificationObject != null && notificationObject.messageNotifications != null && notificationObject.messageNotifications.subscriptionNotifications.Count != 0)
                            {%>
                            <div class="alert alert-success" align="left">
                                <strong>SUCCESS:</strong>
                            </div>
                            <div class="table-responsive">
                            <table class="table table-condensed table-striped table-bordered">
                                <thead>
                                    <th>subscriptionId</th>
                                    <th>callbackData</th>
                                    <th>messageId</th>
                                    <th>Conversation Thread Id</th>
                                    <th>Event Type</th>
                                    <th>Queue</th>
                                    <th>Text</th>
                                    <th>isTruncated</th>
                                    <th>isFavourite</th>
                                    <th>isUnread</th>
                                </thead>
                                <tbody>
                                    <%  foreach (var notificationEvent in notificationObject.messageNotifications.subscriptionNotifications[0].notificationevents)
                                        {%>
                                    <tr>
                                        <td data-value="subscriptionId">
                                            <%= notificationObject.messageNotifications.subscriptionNotifications[0].subscriptionId %>
                                        </td>
                                        <td data-value="callbackData">
                                            <%= notificationObject.messageNotifications.subscriptionNotifications[0].callbackData %>
                                        </td>
                                        <td data-value="messageId">
                                            <%= notificationEvent.messageId %>
                                        </td>
                                        <td data-value="Conversation Thread Id">
                                            <%= notificationEvent.conversationThreadId %>
                                        </td>
                                        <td data-value="Event Type">
                                            <%= notificationEvent.eventType %>
                                        </td>
                                        <td data-value="Queue">
                                            <%= notificationEvent.@event %>
                                        </td>
                                        <td data-value="Text">
                                            <%= notificationEvent.text %>
                                        </td>
                                        <td data-value="isTruncated">
                                            <%= notificationEvent.isTextTruncated %>
                                        </td>
                                        <td data-value="isFavourite">
                                            <%= notificationEvent.isFavourite %>
                                        </td>
                                        <td data-value="isUnread">
                                            <%= notificationEvent.isUnread %>
                                        </td>
                                    </tr>
                                    <% } %>
                                </tbody>
                            </table>
                            </div>
                        <% } %>
                        <% } %>                                
                        <% } %>                                
                        <button id="Button6" type="submit" class="btn btn-primary" name="refreshNotifications" runat="server" onserverclick="getNotifications_Click">Refresh</button>
                        </div>
                        <br/>
                  <%--</div>--%><!--./hidden-->
                <%--<-- </form> -->--%>
              </div><!--./tab-pane-->
            </div>
          </div>
        </div><!--./col-md-12-->
      <%--</div><!--./row-->--%>
            
             </form>
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

      	<% if (!string.IsNullOrEmpty(showSendMsg))
           { %>
        <script type="text/javascript">   $('#tabs a[href="#send-msg"]').tab('show');</script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showCreateMessageIndex))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#create-msg-index"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetMessageList))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-msg-list"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetMessage))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-msg"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetMessageContent))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-msg-content"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetDelta))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-delta"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetMessageIndexInfo))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-msg-index-info"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showUpdateMessage))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#update-msg"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showDeleteMessage))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#delete-msg"]').tab('show'); </script>
        <% } %>
	  <% if (!string.IsNullOrEmpty(showCreateSubscription))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#create-subscription"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showUpdateSubscription))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#update-subscription"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetSubscription))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-subscription"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showDeleteSubscription))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#delete-subscription"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showWebHookNotifications))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#view-notification-details"]').tab('show'); </script>
        <% } %>
        

  </body>
</html>
<!-- vim: set ts=2 sts=2 sw=2 et : -->
