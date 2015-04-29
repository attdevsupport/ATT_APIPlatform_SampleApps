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
          <form id="getAds" name="getAds">
            <div class="form-group">
              <label for="category">Category:</label>
              <select name="category" id="category" class="form-control">
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
              <label for="mmaSize">MMA Size:</label>
              <select name="mmaSize" id="mmaSize" class="form-control">
                <option value="">None</option>
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
              <select name="ageGroup" id="ageGroup" class="form-control">
                <option value="">None</option>
                <option value="1-13">1-13</option>
                <option value="14-25">14-25</option>
                <option value="26-35">26-35</option>
                <option value="36-55">36-55</option>
                <option value="55-100">55-100</option>
              </select>
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="gender">Gender:</label>
              <select name="gender" id="gender" class="form-control">
                <option value="">None</option>
                <option value="M">Male</option>
                <option value="F">Female</option>
              </select>
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="zipCode">Zip Code:</label>
              <input type="text" class="form-control" name="zipCode" id="zipCode" placeholder="Zip Code">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="city">City:</label>
              <input type="text" class="form-control" name="city" id="city" placeholder="Zip Code">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="areaCode">Area Code:</label>
              <input type="text" class="form-control" name="areaCode" id="areaCode" placeholder="Area Code">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="country">Country:</label>
              <input type="text" class="form-control" name="country" id="country" placeholder="Country">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="latitude">Latitude:</label>
              <input type="text" class="form-control" name="latitude" id="latitude" placeholder="Latitude">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="longitude">Longitude:</label>
              <input type="text" class="form-control" name="longitude" id="longitude" placeholder="Longitude">
            </div><!--/.form-group-->
            <div class="form-group">
              <label for="keywords">Keywords:</label>
              <input type="text" class="form-control" name="keywords" id="keywords" 
              data-toggle="tooltip" data-placement="bottom"
              data-title="Multiple keywords can be seperated by a comma (,)"
              placeholder="Keywords">
            </div><!--/.form-group-->
            <button type="submit" data-loading-text="Getting..." class="btn btn-primary">Get Ads</button>
          </form>
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
<!-- vim: set ts=2 sts=2 sw=2 cc=120 tw=120 et : -->
