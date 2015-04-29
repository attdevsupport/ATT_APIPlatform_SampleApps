<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Speech_App1" %>
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
        <h3 class="text-center">Speech to Text</h3>
      </div>
      <div class="row">
        <h5 class="text-center">
          This sample application showcases transcribing audio data files into text-based output
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
          <form id="speechToText" name="speechToText" runat="server" >
            <div class="form-group">
                <label>
                  Speech Context:
                </label>
                <asp:DropDownList ID="SpeechContext" class="form-control" runat="server" AutoPostBack="true">
                </asp:DropDownList>
             </div>
            <div class="form-group">
                <label>
                   Audio File<a id="linkPlay" href="#">(Play)</a>:
                </label>
                <asp:DropDownList ID="audio_file" class="form-control" runat="server">
                </asp:DropDownList>
            </div><!--./form-group-->
            <div id="form-group">
                <label>Send Chunked:</label>
                <asp:CheckBox ID="chkChunked" class="form-control" runat="server" />
            </div>
              <br />
            <div class="form-group">
            <label>
                X-Arg*:
            </label>
            <asp:TextBox ID="x_arg" class="form-control" type="text" name="x_arg" readonly="true" rows="1" runat="server"  Enabled="False" value="ClientApp=NoteTaker" ></asp:TextBox>
            <br />
            </div><!--./form-group-->
              <br/>
            <div id="formSubContext" class="hidden form-group">
                <label>X-SpeechSubContext</label>
                <asp:TextBox ID="x_subContext" runat="server" class="form-control" TextMode="MultiLine" Enabled="False" Rows="4" name="x_subContext"></asp:TextBox>
                <br />
            </div><!--./form-group-->
            <div class="form-group">
              <div class="alert alert-info">* Denotes optional parameters.</div>
            </div>
            <button id="btnSubmit" onserverclick="BtnSubmit_Click" runat="server" class="btn btn-primary" name="SpeechToText"
                            type="submit">
                            Speech to Text</button>
                          <br clear="all" />
          <% if (!string.IsNullOrEmpty (speechSuccessMessage)){ %>
            <div class="alert alert-info" align="left">
              <strong>SUCCESS:</strong>
              <br />
                </div>
              <label>Speech Response:</label>
              <div class="table-responsive">
            <table class="table table-condensed table-striped table-bordered">
              <thead>
                <tr>
                  <th >Parameter</th>
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
                      <% } %>
                      <% if (speechResponseData.Recognition.Info != null)
                         {
                             if (!string.IsNullOrEmpty(speechResponseData.Recognition.Info.version))
                             {
                            %>
                             <tr>
                                <td class="cell" align="center"><em>version</em></td>
                                <td class="cell" align="center"><em><%= speechResponseData.Recognition.Info.version%></em></td>
                            </tr>
                            <%} %>
                            <% if (!string.IsNullOrEmpty(speechResponseData.Recognition.Info.actionType))
                             {
                            %>
                             <tr>
                                <td class="cell" align="center"><em>actionType</em></td>
                                <td class="cell" align="center"><em><%= speechResponseData.Recognition.Info.actionType%></em></td>
                            </tr>
                            <%} %>
                            <% int count = 0; 
                               if ( speechResponseData.Recognition.Info.metrics != null) {
                               foreach (KeyValuePair<string, string> keyitem in speechResponseData.Recognition.Info.metrics)
                               {
                                   if (count == 0)
                                   {%>
                                        <tr>
                                            <td class="cell" align="center"><em>metrics</em></td>
                                            <td class="cell" align="center"><em></em></td>
                                        </tr> 
                                        <%}%>                              
                            <tr>
                                <td class="cell" align="center"><em><%=keyitem.Key%></em></td>
                                <td class="cell" align="center"><em><%=keyitem.Value%></em></td>
                            </tr>
                            <%count++;
                               } count = 0;}%>
                            <% if ( speechResponseData.Recognition.Info.interpretation != null) {
                               foreach (KeyValuePair<string, string> keyitem in speechResponseData.Recognition.Info.interpretation)
                               {
                                   if (count == 0)
                                   {%>
                                        <tr>
                                            <td class="cell" align="center"><em>interpretation</em></td>
                                            <td class="cell" align="center"><em></em></td>
                                        </tr> 
                                        <%}%>                              
                            <tr>
                                <td class="cell" align="center"><em><%=keyitem.Key%></em></td>
                                <td class="cell" align="center"><em><%=keyitem.Value%></em></td>
                            </tr>
                            <%count++;
                               } count = 0;}%>
                            <% if (!string.IsNullOrEmpty(speechResponseData.Recognition.Info.recognized))
                             {
                            %>
                             <tr>
                                <td class="cell" align="center"><em>recognized</em></td>
                                <td class="cell" align="center"><em><%= speechResponseData.Recognition.Info.recognized%></em></td>
                            </tr>
                            <%} %>
                            <% if (speechResponseData.Recognition.Info.search != null)
                               {
								   if ( speechResponseData.Recognition.Info.search.meta != null) {
                                   foreach (KeyValuePair<string, string> keyitem in speechResponseData.Recognition.Info.search.meta)
                                   {
                                       if (count == 0)
                                       {%>
                                        <tr>
                                            <td class="cell" align="center"><em>meta</em></td>
                                            <td class="cell" align="center"><em></em></td>
                                        </tr> 
                                        <%}%>                              
                            <tr>
                                <td class="cell" align="center"><em><%=keyitem.Key%></em></td>
                                <td class="cell" align="center"><em><%=keyitem.Value%></em></td>
                            </tr>
                            <%count++;
                                   } count = 0;}%>
									
                                   <% if ( speechResponseData.Recognition.Info.search.programs != null) {
                                   foreach (KeyValuePair<string, string> keyitem in speechResponseData.Recognition.Info.search.programs)
                                   {
                                       if (count == 0)
                                       {%>
                                        <tr>
                                            <td class="cell" align="center"><em>programs</em></td>
                                            <td class="cell" align="center"><em></em></td>
                                        </tr> 
                                        <%}%>                              
                            <tr>
                                <td class="cell" align="center"><em><%=keyitem.Key%></em></td>
                                <td class="cell" align="center"><em><%=keyitem.Value%></em></td>
                            </tr>
                            <%count++;
                                   } count = 0;}%>

                                   <% if ( speechResponseData.Recognition.Info.search.showTimes != null) {
                                   foreach (KeyValuePair<string, string> keyitem in speechResponseData.Recognition.Info.search.showTimes)
                                   {
                                       if (count == 0)
                                       {%>
                                        <tr>
                                            <td class="cell" align="center"><em>showTimes</em></td>
                                            <td class="cell" align="center"><em></em></td>
                                        </tr> 
                                        <%}%>                              
                            <tr>
                                <td class="cell" align="center"><em><%=keyitem.Key%></em></td>
                                <td class="cell" align="center"><em><%=keyitem.Value%></em></td>
                            </tr>
                            <%count++;
                                   } count = 0;}%>

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
        <%--</div>--%>
          </form>
        </div><!--./col-md-12-->
      </div><!--./row-->
      <div class="row">
        <div class="col-md-12">
          <div class="hidden" id="response"></div>
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
              <a target="_blank" href="audio/boston_celtics.wav"><p class="text-center">boston_celtics.wav</p></a>
            </div><!--./row-->
            <div class="row">
              <a target="_blank" class="text-center" href="audio/california.amr">
                <p class="text-center">california.amr</p>
              </a>
            </div><!--./row-->
            <div class="row">
              <a target="_blank" href="audio/coffee.amr"><p class="text-center">coffee.amr</p></a>
            </div><!--./row-->
            <div class="row">
              <a target="_blank" href="audio/doctors.wav"><p class="text-center">doctors.wav</p></a>
            </div><!--./row-->
            <div class="row">
              <a target="_blank" href="audio/nospeech.wav"><p class="text-center">nospeech.wav</p></a>
            </div><!--./row-->
            <div class="row">
              <a target="_blank" href="audio/samplerate_conflict_error.wav">
                <p class="text-center">samplerate_conflict_error.wav</p>
              </a>
            </div><!--./row-->
            <div class="row">
              <a target="_blank" href="audio/this_is_a_test.spx"><p class="text-center">this_is_a_test.spx</p></a>
            </div><!--./row-->
            <div class="row">
              <a target="_blank" href="audio/too_many_channels_error.wav">
                <p class="text-center">too_many_channels_error.wav</p>
              </a>
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
<!-- vim: set ts=2 sts=2 sw=2 cc=120 tw=120 et : -->