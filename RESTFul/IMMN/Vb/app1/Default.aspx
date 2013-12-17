<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Mobo_App1" %>

<!DOCTYPE html>
<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<!--[if lt IE 7]> <html class="ie6" lang="en"> <![endif]-->
<!--[if IE 7]>    <html class="ie7" lang="en"> <![endif]-->
<!--[if IE 8]>    <html class="ie8" lang="en"> <![endif]-->
<!--[if gt IE 8]><!-->
<html lang="en">
<!--<![endif]-->
<head>
    <title>AT&amp;T Sample Application - In App Messaging</title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <script type="text/javascript">
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-33466541-1']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script');
            ga.type = 'text/javascript';
            ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl'
                                      : 'http://www')
                                      + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(ga, s);
        })();
    </script>
    <script src="scripts/utils.js" type="text/javascript"></script>
</head>
<body>
    <div id="pageContainer">
        <div id="header">
            <div class="logo" id="top">
            </div>
            <div id="menuButton" class="hide">
                <a id="jump" href="#nav">Main Navigation</a>
            </div>
            <ul class="links" id="nav">
                <li><a href="#" target="_blank">Full Page<img src="images/max.png" alt="" /></a> <span
                    class="divider">|&nbsp;</span> </li>
                <li><a id="sourceLink" runat="server" href="<%$ AppSettings:SourceLink %>" target="_blank">
                    Source<img src="images/opensource.png" alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="downloadLink" runat="server" href="<%$ AppSettings:DownloadLink %>" target="_blank">
                    Download<img src="images/download.png" alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="helpLink" runat="server" href="<%$ AppSettings:HelpLink %>" target="_blank">
                    Help </a></li>
                <li id="back"><a href="#top">Back to top</a></li>
            </ul>
        </div>
        <form id="form1" runat="server">
        <div id="content">
            <div id="contentHeading">
                <h1>
                    AT&amp;T Sample Application - In App Messaging from mobile number</h1>
                <div class="border">
                </div>
                <div id="introtext">
                    <div>
                        <b>Server Time:</b>
                        <%= [String].Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) & " UTC"%>
                    </div>
                    <div>
                        <b>Client Time:</b>
                        <script language="JavaScript" type="text/javascript">
                            var myDate = new Date();
                            document.write(myDate);
                        </script>
                    </div>
                    <div>
                        <b>User Agent:</b>
                        <script language="JavaScript" type="text/javascript">
                            document.write("" + navigator.userAgent);
                        </script>
                    </div>
                </div>
            </div>
            <div class="lightBorder"></div>
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <a id="sendMsgToggle" href="javascript:toggle('sendMsg','sendMsgToggle', 'Send Message');">Send Message </a>
                    <div id="sendMsg" class="toggle">
                        <br />
                        <label><i>Max message size shall be 1MB or less</i></label><br/><br/>
                        <h2>Operation : Send Message</h2>
                        <div class="inputFields">
                            <asp:TextBox ID="Address" name="message" placeholder="Address" runat="server"></asp:TextBox>
                            <label>
                                Group:
                                <asp:CheckBox ID="groupCheckBox" runat="server" name="groupCheckBox" />
                            </label>
                            <label>
                                Message: <i>max 200 characters are allowed</i>
                                <asp:TextBox ID="message" name="message" placeholder="ATT IMMN sample message" maxlength="200" runat="server"></asp:TextBox>
                            </label>
                            <label>
                                Subject: <i>max 30 characters are allowed</i>
                                    <asp:TextBox ID="subject" runat="server" name="subject" placeholder="Subject : ATT IMMN APP"  maxlength="30"></asp:TextBox>
                            </label>
                            <label>
                                Attachment:
                            </label>
                            <select name="attachment" id="attachment" runat="server">
                                 <option>None</option>
                            </select><br />
                            <asp:Button ID="sendMessage" class="submit" runat="server" Text="Send Message" OnClick="Button1_Click" />
                        </div>
                        <% If Not String.IsNullOrEmpty(sendMessageSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>
                            <%=sendMessageSuccessResponse.ToString() %>
                        </div>
                        <%End If %>
                        <% If Not String.IsNullOrEmpty(sendMessageErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=sendMessageErrorResponse.ToString()%>
                        </div>
                        <% End If%>
                    </div>
                    <!-- end of sendMessages -->
                     <div class="lightBorder"></div>
						<label>
						  <b>Note:</b> In order to use the following features, you must be subscribed to 
						  <a href="http://messages.att.net">AT&amp;T Messages</a>
						</label>
						<div class="lightBorder"></div>
				     <a id="createMsgToggle" href="javascript:toggle('createMsg','createMsgToggle', 'Create Message Index ');">Create Message Index </a>
				     <div class="toggle" id="createMsg">
				        <br />
				        <h2>Create Message Index</h2>
					    <div class="inputFields">
                            <asp:Button ID="createIndexId" runat="server" Text="Create Index" OnClick="createMessageIndex_Click"/>
					    </div>
					        <label>* - <i>Call <b>Create Message Index</b> method to create an index cache for the subscribers inbox</i></label>
                        <% If Not String.IsNullOrEmpty(createMessageIndexSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>
                            <%=createMessageIndexSuccessResponse.ToString()%>
                        </div>
                        <% End If %>
                        <% If Not String.IsNullOrEmpty(createMessageIndexErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=createMessageIndexErrorResponse.ToString()%>
                        </div>
                        <% End If %>
                     </div>
                    <div class="lightBorder">
                    </div>
                    <a id="getMsgToggle" href="javascript:toggle('getMsg','getMsgToggle', 'Get Message ');">Get Message </a>
                    <div id="getMsg" class="toggle"><br />
                    <h2>Get Message List <i>(Displays last 5 messages from the list)</i></h2>
                        <div class="inputFields">
                            <label>
                                <asp:CheckBox ID="CheckBox1" runat="server" name="groupCheckBox" /> Filter by favorite
                            </label>
                             <label>
                                <asp:CheckBox ID="CheckBox2" runat="server" name="groupCheckBox" /> Filter by unread flag
                            </label>
                             <label>
                                <asp:CheckBox ID="CheckBox3" runat="server" name="groupCheckBox" /> Filter by incoming flag
                            </label>
							<asp:TextBox ID="FilterKeyword" runat="server" name="FilterKeyword" placeholder="555-555-5555"></asp:TextBox>
                            <asp:Button ID="getMessageLst" runat="server" Text="Get Message List" OnClick="getMessageList_Click" />
                        </div>
                        <br/>			
                        <% If Not String.IsNullOrEmpty(getMessageListSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>
                            
                        </div>
                        
                        <table>
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
						
						<% 
						Dim count As Integer = 1
						For Each message As Message In csGetMessageListDetailsResponse.messages
						If count > 5 Then
							Exit For
						End If
                        %>
                        Message <%=count%>
						<br/> 
						<table>
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
							  <td><%= message.recipients%></td>
							  <td><%= message.text%></td>
							  <td><%= message.timeStamp%></td>
							  <td><%= message.isFavorite%></td>
							  <td><%= message.isUnread%></td>
							  <td><%= message.isIncoming%></td>
							  <td><%= message.type%></td>
							</tr>
						  </tbody>
						</table> 
                        <%
                        count = count + 1
                        Next %> 
						<%                       
                        End If %>
                        <% If Not String.IsNullOrEmpty(getMessageListErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=getMessageListErrorResponse.ToString()%>
                        </div>
                        <% End If %>				        
                        <br />
                        <h2>Get Message </h2>
                        <div class="inputFields">
                            <asp:TextBox ID="MessageId" value="" runat="server" name="MessageId" placeholder="Message ID"></asp:TextBox>
                            <asp:Button ID="btnGetMessage" runat="server" Text="Get Message" OnClick="getMessage_Click" />
                        </div>
                        <% If Not String.IsNullOrEmpty(getMessageSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>      
                        </div>
                        <table>
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
							  <td><%= getMessageDetailsResponse.recipients%></td>
							  <td><%= getMessageDetailsResponse.text%></td>
							  <td><%= getMessageDetailsResponse.timeStamp%></td>
							  <td><%= getMessageDetailsResponse.isFavorite%></td>
							  <td><%= getMessageDetailsResponse.isUnread%></td>
							  <td><%= getMessageDetailsResponse.isIncoming%></td>
							  <td><%= getMessageDetailsResponse.type%></td>
							</tr>
						  </tbody>
						</table> 
                        <% End If %>
                        <% If Not String.IsNullOrEmpty(getMessageErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=getMessageErrorResponse.ToString()%>
                        </div>
                        <% End If %>
                        <br />
                        <h2>Get Message Content</h2>
                        <div class="inputFields">
                            <asp:TextBox ID="MessageIdForContent" runat="server" name="MessageIdForContent" placeholder="Message ID"></asp:TextBox>
                            <asp:TextBox ID="PartNumberForContent" runat="server" name="PartNumberForContent" placeholder="Part Number"></asp:TextBox>
                            <asp:Button ID="getMessageContent" runat="server" Text="Get Message Content" OnClick="GetMessageContentByIDnPartNumber" />
                        </div>
                        <% If Not String.IsNullOrEmpty(getMessageContentSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>                                                  
                        </div>
                        <% End If %>
                        <% If Not String.IsNullOrEmpty(getMessageContentErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=getMessageContentErrorResponse.ToString()%>
                        </div>
                        <% End If %>
                        <br />
                        <h2>Get Delta </h2>
                        <div class="inputFields">
                            <asp:TextBox ID="MessageIdForDelta" runat="server" name="MessageIdForDelta" placeholder="Message State"  ></asp:TextBox>
                            <asp:Button ID="btnGetDelta" runat="server" Text="Get Delta" OnClick="getDelta_Click" />
                        </div>
                        <% If Not String.IsNullOrEmpty(deltaSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>                            
                        </div>
                        <b>Delta Type : TEXT</b>
                        <% Dim count As Integer = 0
							Dim deltaRes As Delta
							For Each deltaRes In csDeltaResponse.delta
							If deltaRes.type.Equals("TEXT", StringComparison.InvariantCultureIgnoreCase) Then						
                               %>                                                           
								<table>
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
									Dim i As Integer = 0
									Dim addsize As Integer = deltaRes.adds.Count
									While addsize > 0 AndAlso i < 6 AndAlso deltaRes.adds(i) IsNot Nothing %>                                 
                                    <tr>
									  <td>ADD</td>
									  <td><%= deltaRes.adds(i).messageId%></td>
									  <td><%= deltaRes.adds(i).isFavorite%></td>
									  <td><%= deltaRes.adds(i).isUnread%></td>										  
					                </tr>
									<%
									i = i + 1 								
                                    End While %>
                                     <%  
									i = 0
									Dim updsize As Integer = deltaRes.updates.Count
									While updsize > 0 AndAlso i < 6 AndAlso deltaRes.updates(i) IsNot Nothing %>                                 
                                    <tr>
									  <td>UPDATE</td>
									  <td><%= deltaRes.updates(i).messageId%></td>
									  <td><%= deltaRes.updates(i).isFavorite%></td>
									  <td><%= deltaRes.updates(i).isUnread%></td>										  
					                </tr>
									<%
									i = i + 1 								
                                    End While %>
                                     <%  
									i = 0
									Dim delsize As Integer = deltaRes.deletes.Count
									While delsize > 0 AndAlso i < 6 AndAlso deltaRes.deletes(i) IsNot Nothing %>                                 
                                    <tr>
									  <td>DELETE</td>
									  <td><%= deltaRes.deletes(i).messageId%></td>
									  <td><%= deltaRes.deletes(i).isFavorite%></td>
									  <td><%= deltaRes.deletes(i).isUnread%></td>										  
					                </tr>
									<%
									i = i + 1 								
                                    End While %>
                                    </tbody>
                            </table>
                         <% End If %>
                        
                        <% 
                        count = count + 1
                        Next %>
                        <b>Delta Type : MMS</b>
                        <% count = 0
						For Each deltaRes In csDeltaResponse.delta
						If deltaRes.type.Equals("TEXT", StringComparison.InvariantCultureIgnoreCase) Then
                               %>                                                           
								<table>
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
									Dim i As Integer = 0
									Dim addsize As Integer = deltaRes.adds.Count
									While addsize <> 0 AndAlso i < 6 AndAlso deltaRes.adds(i) IsNot Nothing %>                                 
                                    <tr>
									  <td>ADD</td>
									  <td><%= deltaRes.adds(i).messageId%></td>
									  <td><%= deltaRes.adds(i).isFavorite%></td>
									  <td><%= deltaRes.adds(i).isUnread%></td>										  
					                </tr>
									<%
									i = i+1 								
                                    End While %>
                                     <%  
									i = 0
									Dim updsize As Integer = deltaRes.updates.Count
									While updsize <> 0 AndAlso i < 6 AndAlso deltaRes.updates(i) IsNot Nothing %>                                 
                                    <tr>
									  <td>UPDATE</td>
									  <td><%= deltaRes.updates(i).messageId%></td>
									  <td><%= deltaRes.updates(i).isFavorite%></td>
									  <td><%= deltaRes.updates(i).isUnread%></td>										  
					                </tr>
									<%
									i = i+1 								
                                    End While %>
                                     <%  
									i = 0
									Dim delsize As Integer = deltaRes.deletes.Count
									While delsize <> 0 AndAlso i < 6 AndAlso deltaRes.deletes(i) IsNot Nothing %>                                 
                                    <tr>
									  <td>DELETE</td>
									  <td><%= deltaRes.deletes(i).messageId%></td>
									  <td><%= deltaRes.deletes(i).isFavorite%></td>
									  <td><%= deltaRes.deletes(i).isUnread%></td>										  
					                </tr>
									<%
									i = i+1 								
                                    End While %>
                                    </tbody>
                            </table>
                         <% End If %>
                        
                        <% 
                        count = count + 1
                        Next 						 
                        End If %>
                        
                        <% If Not String.IsNullOrEmpty(deltaErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=deltaErrorResponse.ToString()%>
                        </div>
                        <% End If %>
                        <br />
                        <h2>Get Message Index Info</h2>
                        <div class="inputFields">
                            <asp:Button ID="getMessageIndexInfo" runat="server" Text="Get Message Index Info" OnClick="getMessageIndex_Click" />
                        </div>
                        <% If Not String.IsNullOrEmpty(messageIndexSuccessResponse) Then
                        %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>                            
                        </div>
                        <table>
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
                        <% End If %>
                        <% If Not String.IsNullOrEmpty(messageIndexErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=messageIndexErrorResponse.ToString()%>
                        </div>
                        <% End If %>
                    
                </div>
                    
                    <div class="lightBorder"></div>
                    <a id="updateMsgToggle" 
				        href="javascript:toggle('updateMsg','updateMsgToggle', 'Update Message ');">Update Message </a>
				    <div class="toggle" id="updateMsg">
				        <br/>
				        <h2>Update Message/Messages</h2>
					    <div class="inputFields">
					        <label><i>More than one message ID's can be separated by comma(,) separator</i></label>
                            <asp:TextBox ID="updateMessageId" runat="server" name="updateMessageId" placeholder="Message ID"  ></asp:TextBox>
						    <label> Change Status : 
                            <input type="radio" id="read" runat="server" name="changeStatus" value="Read" Checked /> Read
                            <input type="radio" id="unread" runat="server" name="changeStatus" value="UnRead" /> UnRead
					        </label>
                            <asp:Button ID="btnUpdateMessage" runat="server" Text="Update Message/Messages" OnClick="updateMessage_Click" />
					    </div>
                        <% If Not String.IsNullOrEmpty(updateMessageSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>
                            <%=updateMessageSuccessResponse.ToString()%>
                        </div>
                        <% End If %>
                        <% If Not String.IsNullOrEmpty(updateMessageErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=updateMessageErrorResponse.ToString()%>
                        </div>
                        <% End If %>
                     </div>

                     <div class="lightBorder"></div>
				     <a id="delMsgToggle" 
				      href="javascript:toggle('delMsg','delMsgToggle', 'Delete Message ');">Delete Message </a>
				     <div class="toggle" id="delMsg">
				        <br/>
				        <h2>Delete Message/Messages</h2>
					    <div class="inputFields">
					        <label><i>More than one message ID's can be separated by comma(,) separator</i></label>
                            <asp:TextBox ID="deleteMessageId" runat="server" name="deleteMessageId" placeholder="Message ID"></asp:TextBox>
                            <asp:Button ID="deleteMessages" runat="server" Text="Delete Message/Messages" OnClick="deleteMessage_Click" />                              												
					    </div>
                        <% If Not String.IsNullOrEmpty(deleteMessageSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>
                            <%=deleteMessageSuccessResponse.ToString()%>
                        </div>
                        <% End If %>
                        <% If Not String.IsNullOrEmpty(deleteMessageErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=deleteMessageErrorResponse.ToString()%>
                        </div>
                        <% End If %>
                     </div>

           			 
                     <div class="lightBorder"></div>
                     <a id="getMsgNotToggle" 
				                href="javascript:toggle('getMsgNot','getMsgNotToggle', 'Get Notification Connection Details ');">Get Notification Connection Details </a>
				     <div class="toggle" id="getMsgNot">
				        <br />
				        <h2>Get Notification Connection Details</h2>
					    <div class="inputFields">
						    <label> Notification Subscription : 
                                <input type="radio" id="notificationText" runat="server" name="notifciationDetails" value="TEXT" checked/> TEXT
                                <input type="radio" id="notificationMms" runat="server" name="notifciationDetails" value="MMS" /> MMS
					        </label>
                            <asp:Button ID="GetNotificationDetailsId" runat="server" Text="Get Details" OnClick="getNotificationConnectionDetails_Click" />
                        </div>
                        <% If Not String.IsNullOrEmpty(getNotificationConnectionDetailsSuccessResponse) Then %>
                        <div class="successWide">
                            <strong>SUCCESS:</strong>                                                     
                        </div>
                        <label> Connection Details </label><br />
                        <table>
						  <thead>
							<tr>
							  <th>Username</th>
							  <th>Password</th>
							  <th>https url</th>
							  <th>wss url</th>
							  <th>queues</th>
							</tr>
						  </thead>
						  <tbody>
							<tr>
							  <td data-value="Username"><%= getNotificationConnectionDetailsResponse.username%></td>
							  <td data-value="Password"><%= getNotificationConnectionDetailsResponse.password%></td>
							  <td data-value="https url"><%= getNotificationConnectionDetailsResponse.httpsUrl%></td>
							  <td data-value="wss url"><%= getNotificationConnectionDetailsResponse.wssUrl%></td>
							  <td data-value="queues"><%= getNotificationConnectionDetailsResponse.queues%></td>
							</tr>
						  </tbody>
						</table>                        
                        <% End If %>
                        <% If Not String.IsNullOrEmpty(getNotificationConnectionDetailsErrorResponse) Then %>
                        <div class="errorWide">
                            <strong>ERROR:</strong><br />
                            <%=getNotificationConnectionDetailsErrorResponse.ToString()%>
                        </div>
                        <% End If %>
                 </div>    
                 
             </div>
               <!-- END HEADER CONTENT RESULTS -->
            </div>
            <!-- SAMPLE APP CONTENT ENDS HERE! -->
            </div>
        </form>
        <br style="clear: both" />
        <div class="border">
        </div>
        <div id="footer">
            <div id="powered_by">
                Powered by AT&amp;T Cloud Architecture
            </div>
            <p>
                The Application hosted on this site are working examples intended to be used for
                reference in creating products to consume AT&amp;T Services and not meant to be
                used as part of your product. The data in these pages is for test purposes only
                and intended only for use as a reference in how the services perform.
                <br />
                <br />
                For download of tools and documentation, please go to <a href="https://developer.att.com/"
                    target="_blank">https://developer.att.com</a>
                <br />
                For more information contact <a href="mailto:developer.support@att.com">developer.support@att.com</a>
                <br />
                <br />
                &#169; 2013 AT&amp;T Intellectual Property. All rights reserved. <a href="https://developer.att.com/"
                    target="_blank">https://developer.att.com</a>
            </p>
        </div>
        <!-- end of footer -->
    </div>
    <script type="text/javascript">        setup();</script>
    <% If Not String.IsNullOrEmpty(showSendMsg) Then 
      %>
        <script type="text/javascript">        toggle('sendMsg','sendMsgToggle', 'Send Message');</script>
        <% End If %>
        <% If Not String.IsNullOrEmpty(showGetMessage) Then
           %>
        <script type="text/javascript">        toggle('getMsg','getMsgToggle', 'Get Message ');</script>
        <% End If %>
        <% If Not String.IsNullOrEmpty(showUpdateMessage) Then
            %>
        <script type="text/javascript">        toggle('updateMsg','updateMsgToggle', 'Update Message ');</script>
        <% End If %>
         <% If Not String.IsNullOrEmpty(showDeleteMessage) Then
           %>
        <script type="text/javascript">        toggle('delMsg','delMsgToggle', 'Delete Message ');</script>
        <% End If %>
         <% If Not String.IsNullOrEmpty(showCreateMessageIndex) Then
            %>
        <script type="text/javascript">        toggle('createMsg','createMsgToggle', 'Create Message Index ');</script>
        <% End If %>
         <% If Not String.IsNullOrEmpty(showGetNotificationConnectionDetails) Then
            %>
        <script type="text/javascript">        toggle('getMsgNot','getMsgNotToggle', 'Get Notification Connection Details ');</script>
        <% End If %>
</body>
</html>
