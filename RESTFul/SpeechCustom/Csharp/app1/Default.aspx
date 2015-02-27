<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Speech_App1" %>

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
<!--<![endif]-->
<head>
    <title>AT&amp;T Sample Speech Application - Speech to Text Custom </title>
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta id="viewport" name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link rel="stylesheet" type="text/css" href="style/common.css" />
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
        function enableNameParam(list, nameParam) {
            var selectedValue = list.options[list.selectedIndex].value;
            if (selectedValue == "GenericHints") {
                document.getElementById("nameParam").disabled = false;
            } else {
                document.getElementById("nameParam").disabled = true;
                var choices = document.getElementById("nameParam");
                choices.options[0].selected = true;
            }
        }
    </script>
</head>
<body>
    <div id="pageContainer" class="pageContainer">
        <div id="header">
            <div class="logo" id="top">
            </div>
            <div id="menuButton" class="hide">
                <a id="jump" href="#nav">Main Navigation</a>
            </div>
            <ul class="links" id="nav">
               
                <li><a id="SourceLink" runat="server" target="_blank">Source<img src="images/source.png"
                    alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="DownloadLink" runat="server" target="_blank">Download<img src="images/download.png"
                    alt="" />
                </a><span class="divider">|&nbsp;</span> </li>
                <li><a id="HelpLink" runat="server" target="_blank">Help </a></li>
                <li id="back"><a href="#top">Back to top</a></li>
            </ul>
        </div>
        <form id="form1" runat="server">
        <div id="content" class="content">
            <div id="contentHeading" class="contentHeading">
                <h1>
                    AT&amp;T Sample Application - Speech to Text Custom</h1>
                <div id="introtext">
                    <div>
                        <b>Server Time:&nbsp;</b><%= String.Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) + " UTC" %>
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
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <div id="formData">
                        <h3>
                            Speech Context:
                        </h3>
                        <asp:DropDownList ID="SpeechContext" runat="server" onchange="enableNameParam(this,'nameParam')">
                        </asp:DropDownList>
                        <h3>Name Parameter:</h3>
                        <asp:DropDownList ID="nameParam" name="nameParam" runat="server">
                        </asp:DropDownList>
                        <h3>
                            Audio File:
                        </h3>
                        <asp:DropDownList ID="audio_file" runat="server">
                        </asp:DropDownList>
                        <br />
                        <h3>
                            X-Arg:
                        </h3>
                        <asp:Label ID="x_arg" runat="server" name="x_arg"></asp:Label>
                        <br />
                        <h3>
                            MIME Data:
                        </h3>
                        <asp:TextBox ID="mimeData" type="text" runat="server" TextMode="MultiLine" Enabled="False" Rows="4" name="mimeData"></asp:TextBox>
                        <br />

                        <button id="SpeechToTextCustom" onserverclick="BtnSubmit_Click" runat="server" name="SpeechToTextCustom"
                            type="submit">
                            Submit</button>
                    </div>
                </div>
            </div>
          <% if (!string.IsNullOrEmpty (speechSuccessMessage)){ %>
            <div class="successWide" align="left">
              <strong>SUCCESS:</strong>
              <br />
              Response parameters listed below.
            </div>
            <table id="kvp" class="kvp">
              <thead>
                <tr>
                  <th class="label">Parameter</th>
                  <th class="label">Value</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td class="cell" align="center"><em>ResponseId</em></td>
                  <td class="cell" align="center"><em><%= speechResponseData.Recognition.ResponseId %></em></td>
                </tr>
                <tr>
                  <td class="cell" align="center"><em>Status</em></td>
                  <td class="cell" align="center"><em><%= speechResponseData.Recognition.Status %></em></td>
                </tr>
                <%
                  if ((speechResponseData.Recognition.NBest != null) && (speechResponseData.Recognition.NBest.Count > 0)) {
                   foreach (NBest nbest in speechResponseData.Recognition.NBest){  %>
                      <tr>
                        <td class="cell" align="center"><em>Hypothesis</em></td>
                        <td class="cell" align="center"><em><%= nbest.Hypothesis %></em></td>
                      </tr>
                      <tr>
                        <td class="cell" align="center"><em>LanguageId</em></td>
                        <td class="cell" align="center"><em><%= nbest.LanguageId%></em></td>
                      </tr>
                      <tr>
                        <td class="cell" align="center"><em>Confidence</em></td>
                        <td class="cell" align="center"><em><%= nbest.Confidence  %></em></td>
                      </tr>
                      <tr>
                        <td class="cell" align="center"><em>Grade</em></td>
                        <td class="cell" align="center"><em><%= nbest.Grade  %></em></td>
                      </tr>
                      <tr>
                        <td class="cell" align="center"><em>ResultText</em></td>
                        <td class="cell" align="center"><em><%= nbest.ResultText %></em></td>
                      </tr>
                      <tr>
                        <td class="cell" align="center"><em>Words</em></td>
                        <td class="cell" align="center"><em><%= string.Join(", ", nbest.Words.ToArray())%></em></td>
                      </tr>
                      <tr>
                        <td class="cell" align="center"><em>WordScores</em></td>
                        <td class="cell" align="center"><em><%= string.Join(", ", nbest.WordScores.ToArray())%></em></td>
                      </tr>
                           <% if (nbest.NluHypothesis != null)
                              { %>
                                                 <tr>
                        <td class="cell" align="center"><em>NluHypothesis</em></td>
                        <td class="cell" align="center"><em></em></td>
                      </tr>
                           <% foreach (outComposite comp in nbest.NluHypothesis.OutComposite.ToList())
                              { %>
                                                 <tr>
                        <td class="cell" align="center"><em>Grammar</em></td>
                        <td class="cell" align="center"><em><%= comp.Grammar%></em></td>
                      </tr>
                                                                       <tr>
                        <td class="cell" align="center"><em>Out</em></td>
                        <td class="cell" align="center"><em><%= comp.Out%></em></td>
                      </tr>
                           <%} %>
                           <%} %>
                     <%} %>
                      <%} %>
                    </tbody>
                  </table>
                <% } %>
                <% if (!string.IsNullOrEmpty(speechErrorMessage)){ %>
                  <div class="errorWide">
                    <strong>ERROR:</strong>
                    <br />
                    <%= speechErrorMessage  %>
                  </div>
                <% } %>
        </div>
        </form>
        <div id="footer">
            <div id="ft">
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
                    For more information please go to <a href="https://developer.att.com/support"
                        target="_blank">https://developer.att.com/support</a>
                    <br />
                    <br />
                    © 2014 AT&amp;T Intellectual Property. All rights reserved. <a href="https://developer.att.com/"
                        target="_blank">https://developer.att.com</a>
                </p>
            </div>
            <!-- end of ft -->
        </div>
        <!-- end of footer -->
    </div>
</body>
</html>
