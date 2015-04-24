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
      <div class="row">
        <div class="col-md-12">
          <form id="speechToTextCustom" name="speechToTextCustom">
            <div class="form-group">
              <label for="speechContext">Speech Context:</label>
              <select name="speechContext" id="speechContext" class="form-control">
                <option value="GenericHints">GenericHints</option>
                <option value="GrammarList">GrammarList</option>
              </select>
            </div>
            <div class="form-group">
              <label for="nameParam">Name Parameter</label>
              <select name="nameParam" id="nameParam" class="form-control">
                <option value="x-grammar">x-grammar</option>
                <option value="x-grammar-prefix">x-grammar-prefix</option>
                <option value="x-grammar-altgram">x-grammar-altgram</option>
              </select>
            </div><!--./form-group-->
            <div class="form-group">
              <label for="audioFile">Audio File <a id="linkPlay" href="#">(Play):</a></label>
              <select name="audioFile" id="audioFile" class="form-control">
                <option value="pizza-en-US.wav">pizza-en-US.wav</option>
              </select>
            </div><!--./form-group-->
            <div class="form-group">
              <label for="x_arg">X-Args</label>
              <textarea class="form-control" id="x_arg" type="text" name="x_arg" readonly="readonly" rows="1" 
                value="GrammarPenaltyPrefix=1.1,GrammarPenaltyGeneric=2.0,GrammarPenaltyAltgram=4.1"
                >GrammarPenaltyPrefix=1.1,GrammarPenaltyGeneric=2.0,GrammarPenaltyAltgram=4.1</textarea>
            </div><!--./form-group-->
            <div class="form-group">
              <label for="xDictionary">X-Dictionary</label>
              <textarea class="form-control" id="xDictionary" type="text" name="xDictionary" readonly="readonly"
                rows="4"></textarea>
            </div><!--./form-group-->
            <div class="form-group">
              <label for="xGrammar">X-Grammar</label>
              <textarea class="form-control" id="xGrammar" type="text" name="xGrammar" readonly="readonly" rows="4"></textarea>
            </div><!--./form-group-->
            <div class="form-group">
              <div class="alert">* Denotes optional parameters.</div>
            </div>
            <button type="submit" data-loading-text="Converting..." class="btn btn-primary">Speech to Text</button>
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
<!-- vim: set ts=2 sts=2 sw=2 cc=120 tw=120 et : -->
