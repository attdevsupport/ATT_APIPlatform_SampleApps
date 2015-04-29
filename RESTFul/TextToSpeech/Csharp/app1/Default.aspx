<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="TTS_App1" %>

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
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Speech to Text</title>

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
        <h3 class="text-center">Text To Speech</h3>
      </div>
      <div class="row">
        <h5 class="text-center">
          This sample application showcases transcribing text into binary audio data and applying a male or female
          synthesized voice.
        </h5>
      </div>
      <hr>
      <div class="inline-row">
        <a class="btn btn-warning" id="github" href="#">Github</a>
        <a class="btn btn-warning" id="download" href="#">Download</a>
      </div><!--./row-->
      <hr>
        <form id="form1" runat="server">
<div class="container">
             <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <div id="formData">
                        <label>
                            Content Type:
                        </label>
                        <asp:DropDownList ID="ContentType" name="ContentType" class="form-control" runat="server">
                            <asp:ListItem value="text/plain" Selected="True" Text="text/plain" />
                            <asp:ListItem value="application/ssml+xml" Text="application/ssml+xml" />
                        </asp:DropDownList>
                        <label>Content:</label>
                          <asp:TextBox ID="plaintext" type="text" class="form-control" runat="server" name="plaintext" TextMode="MultiLine" maxlength="250"
                  rows="4" data-toggle="tooltip" data-placement="top"
                  data-title="Limit of 250 characters"></asp:TextBox><br/>
                        <label>application/ssml</label>
                        <br />
                          <asp:TextBox  ID="ssml" type="text" runat="server" name="ssml" TextMode="MultiLine" Enabled="False" Rows="4"></asp:TextBox >
                        <br />
                        <label>
                            X-Arg*
                        </label>
                        <br />
                        <asp:TextBox ID="x_arg" runat="server" class="form-control" type="text" readonly="true" value="AppId=12345,OrgId=54321,VoiceName=mike"></asp:TextBox>
                        <br />
                        <button id="btnSubmit" onserverclick="BtnSubmit_Click" class="btn btn-primary" runat="server" name="TextToSpeechButton"
                            type="submit">
                            Submit</button>
                        <br />
                        <div class="form-group">
                            <div class="alert alert-info">* Denotes optional parameters.</div>
                        </div>
                        <% if (!string.IsNullOrEmpty(TTSSuccessMessage)){%>
                        <div class="successWide" align="left">
                            <strong>SUCCESS:</strong>
                            <audio controls="controls" autobuffer="autobuffer" autoplay="autoplay" runat="server" id="audioPlay">
                            </audio>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(TTSErrorMessage)){%>
                        <div class="errorWide">
                            <strong>ERROR:</strong>
                            <br>
                            <%= TTSErrorMessage%>
                        </div>
                        <% } %>
                    </div>
                </div>
            </div>
        </div>
        </form>
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
    </div>
</body>
</html>
