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
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Speech to Text Custom</title>

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
    <script type="text/javascript">
        function load() {
            var selectedValue = document.getElementById("SpeechContext").value;
            if (selectedValue == "GenericHints") {
                document.getElementById("nameParam").disabled = false;
            } else {
                document.getElementById("nameParam").disabled = true;
                var choices = document.getElementById("nameParam");
                choices.options[0].selected = true;
            }
        }
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

    <!--[if lt IE 9]>
      <script src="https://lprod.code-api-att.com/public_files/js/html5shiv.min.js"></script>
      <script src="https://lprod.code-api-att.com/public_files/js/respond.min.js"></script>
    <![endif]-->
  </head>
<body onload="load()">
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
        <h3 class="text-center">Speech to Text Custom</h3>
      </div>
      <div class="row">
        <h5 class="text-center">
          This sample application showcases transcribing audio data files into text-based output using developer
          customized grammar, hints, or both. 
        </h5>
      </div>
      <hr>
      <div class="inline-row">
        <a class="btn btn-warning" id="github" href="#">Github</a>
        <a class="btn btn-warning" id="download" href="#">Download</a>
      </div><!--./row-->
      <hr>
        <form id="form1" runat="server">
        <div id="content" class="content">
            <div class="formBox" id="formBox">
                <div id="formContainer" class="formContainer">
                    <div id="formData">
                        <label>
                            Speech Context:
                        </label>
                        <asp:DropDownList ID="SpeechContext" class="form-control" runat="server"  onchange="enableNameParam(this,'nameParam')">
                        </asp:DropDownList>
                        <label>Name Parameter:</label>
                        <asp:DropDownList ID="nameParam"  class="form-control" name="nameParam" runat="server">
                        </asp:DropDownList>
                        <div class="form-group">
                            <label for="audio_file">Audio File <a id="linkPlay" href="#">(Play):</a></label>
                                <br />
                            <asp:DropDownList ID="audio_file" class="form-control" runat="server">
                            </asp:DropDownList>
                        </div>
                        <label>
                            X-Args
                        </label>
                        <br />
                        <asp:TextBox ID="x_arg" class="form-control" readonly="true" runat="server" name="x_arg"></asp:TextBox>
                        <br />
                        <label>
                            X-Dictionary
                        </label>
                        <br />
                        <asp:TextBox ID="x_dictionary" class="form-control" readonly="true" Enabled="False" runat="server" TextMode="MultiLine" rows="4" name="x_dictionary"></asp:TextBox>
                        <br />
                        <label>
                            X-Grammer
                        </label>
                        <br />
                        <asp:TextBox ID="x_grammer" class="form-control" readonly="true" Enabled="False" runat="server" TextMode="MultiLine" rows="4" name="x_grammer"></asp:TextBox>
                        <br />
                        
                        <div class="hidden">
                        <label>
                            MIME Data:
                        </label>
                        
                        <asp:TextBox ID="mimeData" class="form-control" type="text" runat="server" TextMode="MultiLine" Enabled="False" Rows="4" name="mimeData"></asp:TextBox>
                        <br />
                        </div>
                        <div class="form-group">
                            <div class="alert alert-info">* Denotes optional parameters.</div>
                        </div>
                        <button id="SpeechToTextCustom" onserverclick="BtnSubmit_Click" runat="server" class="btn btn-primary" name="SpeechToTextCustom"
                            type="submit">
                            Speech to Text</button>
                    </div>
                </div>
            </div>
          <% if (!string.IsNullOrEmpty (speechSuccessMessage)){ %>
            <div class="alert alert-info" align="left">
              <strong>SUCCESS:</strong>
              <br />
                </div>
              <label>Speech Response:</label>
            <div class="table-responsive">
            <table id="kvp" class="table table-condensed table-striped table-bordered">
              <thead>
                <tr>
                  <th>Parameter</th>
                  <th>Value</th>
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
                </div>
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
    <!-- modal for playing audio files-->
    <div class="modal fade" id="playModal" tabindex="-1" role="dialog" aria-labelledby="playFiles"
      aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span
              aria-hidden="true">&times;</span></button>
            <h4 class="modal-title" id="playFiles">Select Audio File to Play:</h4>
          </div><!--/.modal-header-->
          <div class="modal-body">
            <div class="row">
              <a target="_blank" href="audio/pizza-en-US.wav"><p class="text-center">pizza-en-US.wav</p></a>
            </div><!--./row-->
          </div><!--/.modal-body-->
          <div class="modal-footer">
            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
          </div>
        </div>
      </div>
    </div><!--/.modal-->
    
</body>
</html>
