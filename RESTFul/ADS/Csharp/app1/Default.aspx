<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Ad_App1" %>

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
    <title>AT&amp;T Sample Application - Advertisement</title>

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
        <h3 class="text-center">Advertisement</h3>
      </div>
      <div class="row">
        <h5 class="text-center">
          This sample application showcases fetching advertisements based on user-selected parameters
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
          
            <div class="form-group">
              <label for="category">Category:</label>
              <select name="category" id="category" class="form-control" runat="server">
                <option value="auto">auto</option>
                <option value="business">business</option>
                <option value="chat">chat</option>
                <option value="communication">communication</option>
                <option value="community">community</option>
                <option value="entertainment">entertainment</option>
                <option value="finance">finance</option>
                <option value="games">games</option>
                <option value="health">health</option>
                <option value="local">local</option>
                <option value="maps">maps</option>
                <option value="medical">medical</option>
                <option value="movies">movies</option>
                <option value="music">music</option>
                <option value="news">news</option>
                <option value="other">other</option>
                <option value="personals">personals</option>
                <option value="photos">photos</option>
                <option value="shopping">shopping</option>
                <option value="shopping">shopping</option>
                <option value="social">social</option>
                <option value="sports">sports</option>
                <option value="technology">technology</option>
                <option value="tools">tools</option>
                <option value="travel">travel</option>
                <option value="tv">tv</option>
                <option value="video">video</option>
                <option value="weather">weather</option>
              </select>
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="MMA">MMA Size:</label>
              <select name="MMA" id="MMA" class="form-control" runat="server">
                <option value=""></option>
                <option value="120 x 20">120 x 20</option>
                <option value="168 x 28">168 x 28</option>
                <option value="216 x 36">216 x 36</option>
                <option value="300 x 50">300 x 50</option>
                <option value="300 x 250">300 x 250</option>
                <option value="320 x 50">320 x 50</option>
              </select>
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="ageGroup">Age Group:</label>
              <select name="ageGroup" id="ageGroup" class="form-control" runat="server">
                <option value=""></option>
                <option value="1-13">1-13</option>
                <option value="14-25">14-25</option>
                <option value="26-35">26-35</option>
                <option value="36-55">36-55</option>
                <option value="55-100">55-100</option>
              </select>
            </div><!--/.form-group-->
            <div class="form-group">
                <label>
                    Premium:</label>
                <br />
                <select name="Premium" id="Premium" class="form-control" runat="server">
                    <option value=""></option>
                    <option value="0">NonPremium</option>
                    <option value="1">Premium Only</option>
                    <option value="2">Both</option>
                </select>
             </div>

            <div class="form-group">
              <label for="gender">Gender:</label>
              <select name="gender" id="gender" class="form-control" runat="server">
                <option value=""></option>
                <option value="M">Male</option>
                <option value="F">Female</option>
              </select>
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="gender">Over 18 Ad Content:</label>
              <select name="over18" id="over18" class="form-control" runat="server">
                <option value=""></option>
                <option value="0">Deny Over 18</option>
                <option value="2">Only Over 18</option>
                <option value="3">Allow All Ads</option>
              </select>
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="zipCode">Zip Code:</label>
              <input type="text" class="form-control" runat="server" name="zipCode" id="zipCode" placeholder="Zip Code">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="city">City:</label>
              <input type="text" runat="server" class="form-control" name="city" id="city" placeholder="City">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="Area Code">Area Code:</label>
              <input type="text" class="form-control" runat="server" name="areaCode" id="areaCode" placeholder="Area Code">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="country">Country:</label>
              <input type="text" class="form-control" runat="server" name="country" id="country" placeholder="Country">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="latitude">Latitude:</label>
              <input type="text" class="form-control" runat="server" name="latitude" id="latitude" placeholder="Latitude">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="longitude">Longitude:</label>
              <input type="text" class="form-control" runat="server" name="longitude" id="longitude" placeholder="Longitude">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="keywords">Keywords:</label>
              <input type="text" class="form-control" runat="server" name="keywords" id="keywords" 
              data-toggle="tooltip" data-placement="bottom"
              data-title="Multiple keywords can be seperated by a comma (,)"
              placeholder="Keywords">
            </div><!--/.form-group-->
            <button type="submit" runat="server"  data-loading-text="Getting..." onserverclick="BtnGetADS_Click" class="btn btn-primary">Get Ads</button>
        </div><!--./col-md-12-->
          </div><!--./row-->
               <% if (!string.IsNullOrEmpty(getAdsSuccessResponse))
                       { %>
                    <div class="alert alert-success">
                        <strong>SUCCESS:</strong><br /><%= getAdsSuccessResponse%>
                    </div>
                    <% if (adRequestResponse != null && adRequestResponse.AdsResponse != null && adRequestResponse.AdsResponse.Ads != null)
                       { %>
                       <div class="table-responsive">
                        <table class="table table-condensed table-striped table-bordered"> 
                        <thead>
                            <tr>
                                <th>
                                    Parameter
                                </th>
                                <th>
                                    Value
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <% if (!string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.Type))
                               {%>
                            <tr>
                                <td data-value="Parameter">
                                    Type
                                </td>
                                <td data-value="Value">
                                    <%= adRequestResponse.AdsResponse.Ads.Type%>
                                </td>
                            </tr>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.ClickUrl))
                               {%>
                            <tr>
                                <td data-value="Parameter">
                                    ClickUrl
                                </td>
                                <td data-value="Value">
                                    <%= adRequestResponse.AdsResponse.Ads.ClickUrl%>
                                </td>
                            </tr>
                            <% } %>

                            <% if ( adRequestResponse.AdsResponse.Ads.ImageUrl != null && !string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.ImageUrl.Image))
                               {%>
                            <tr>
                                <td data-value="Parameter">
                                    ImageUrl
                                </td>
                                <td data-value="Value">
                                    <%= adRequestResponse.AdsResponse.Ads.ImageUrl.Image %>
                                </td>
                            </tr>
                            <% } %>

                            <% if (!string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.Text))
                               {%>
                            <tr>
                                <td data-value="Parameter">
                                    Text
                                </td>
                                <td data-value="Value">
                                    <%= adRequestResponse.AdsResponse.Ads.Text %>
                                </td>
                            </tr>
                            <% } %>

                            <% if (!string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.Content))
                               {%>
                            <tr>
                                <td data-value="Parameter">
                                    Content
                                </td>
                                <td data-value="Value">
                                    <%= adRequestResponse.AdsResponse.Ads.Content %>
                                </td>
                            </tr>
                            <% } %>
                        </tbody>
                    </table>
                    </div>
                    <%if ( ((adRequestResponse.AdsResponse.Ads.ImageUrl != null) &&
                           (!string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.ImageUrl.Image))) ||
                          !string.IsNullOrEmpty(adRequestResponse.AdsResponse.Ads.Text))
                      {%>
                    <asp:HyperLink ID="hplImage" runat="server" Target="_blank"></asp:HyperLink>
                    <%} %>
                    <%} %>
                    <%} %>
                    <% if (!string.IsNullOrEmpty(getAdsErrorResponse))
                       { %>
                    <div class="alert alert-danger">
                        <strong>ERROR:</strong>
                        <br />
                        <%=getAdsErrorResponse%>
                    </div>
                    <% } %>

      
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

    <!-- end of page_container -->
    </form>
</body>
</html>
