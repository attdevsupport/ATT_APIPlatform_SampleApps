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
          <form id="speechToText" name="speechToText">
            <div class="form-group">
              <label for="speechContext">Speech Context:</label>
              <select name="speechContext" id="speechContext" class="form-control">
                <option value="Generic">Generic</option>
                <option value="TV">TV</option>
                <option value="BusinessSearch">BusinessSearch</option>
                <option value="Websearch">Websearch</option>
                <option value="SMS">SMS</option>
                <option value="Voicemail">Voicemail</option>
                <option value="QuestionAndAnswer">QuestionAndAnswer</option>
                <option value="Gaming">Gaming</option>
                <option value="SocialMedia">SocialMedia</option>
              </select>
            </div>
            <div class="form-group">
              <label for="speechFile">Audio File <a id="linkPlay" href="#">(Play)</a>:</label>
              <select name="speechFile" id="speechFile" class="form-control">
                <option value="boston_celtics.wav">boston_celtics.wav</option>
                <option value="california.amr">california.amr</option>
                <option value="coffee.amr">coffee.amr</option>
                <option value="doctors.wav">doctors.wav</option>
                <option value="nospeech.wav">nospeech.wav</option>
                <option value="samplerate_conflict_error.wav">samplerate_conflict_error.wav</option>
                <option value="this_is_a_test.spx">this_is_a_test.spx</option>
                <option value="too_many_channels_error.wav">too_many_channels_error.wav</option>
              </select>
            </div><!--./form-group-->
            <div class="form-group">
              <label for="x_arg">X-Arg*</label>
              <textarea class="form-control" id="x_arg" type="text" name="x_arg" readonly="readonly" rows="1" 
                value="ClientApp=NoteTaker">ClientApp=NoteTaker</textarea>
            </div><!--./form-group-->
            <div id="formSubContext" class="hidden form-group">
              <label for="x_subcontext">X-SpeechSubContext</label>
              <select name="x_subcontext" id="x_subcontext" class="form-control">
                <option value="AlphadigitList">AlphadigitList</option>
                <option value="AlphadigitSingle">AlphadigitSingle</option>
                <option value="AlphaList">AlphaList</option>
                <option value="AlphaSingle">AlphaSingle</option>
                <option value="Chat">Chat</option>
                <option value="DigitList">DigitList</option>
                <option value="DigitList_1-2">DigitList_1-2</option>
                <option value="DigitList_1-3">DigitList_1-3</option>
                <option value="DigitList_1-4">DigitList_1-4</option>
                <option value="DigitList_1-5">DigitList_1-5</option>
                <option value="DigitList_1-6">DigitList_1-6</option>
                <option value="DigitList_1-7">DigitList_1-7</option>
                <option value="DigitList_1-8">DigitList_1-8</option>
                <option value="DigitList_1-9">DigitList_1-9</option>
                <option value="DigitSingle">DigitSingle</option>
                <option value="DigitSingle_1-2">DigitSingle_1-2</option>
                <option value="DigitSingle_1-3">DigitSingle_1-3</option>
                <option value="DigitSingle_1-4">DigitSingle_1-4</option>
                <option value="DigitSingle_1-5">DigitSingle_1-5</option>
                <option value="DigitSingle_1-6">DigitSingle_1-6</option>
                <option value="DigitSingle_1-7">DigitSingle_1-7</option>
                <option value="DigitSingle_1-8">DigitSingle_1-8</option>
                <option value="DigitSingle_1-9">DigitSingle_1-9</option>
                <option value="DigitSingle_1-10">DigitSingle_1-10</option>
                <option value="PetCmd">PetCmd</option>
                <option value="Rpg">Rpg</option>
                <option value="RpgActn">RpgActn</option>
                <option value="RpgActnAccept">RpgActnAccept</option>
                <option value="RpgActnActiv">RpgActnActiv</option>
                <option value="RpgActnExam">RpgActnExam</option>
                <option value="RpgActnOpenclose">RpgActnOpenclose</option>
                <option value="RpgActnPushpull">RpgActnPushpull</option>
                <option value="RpgEmotes">RpgEmotes</option>
                <option value="RpgEmotesBasic">RpgEmotesBasic</option>
                <option value="RpgEmotesComm">RpgEmotesComm</option>
                <option value="RpgEmotesExpr">RpgEmotesExpr</option>
                <option value="RpgEmotesGreet">RpgEmotesGreet</option>
                <option value="RpgEmotesHibye">RpgEmotesHibye</option>
                <option value="RpgEmotesMoodact">RpgEmotesMoodact</option>
                <option value="RpgEmotesMoodstate">RpgEmotesMoodstate</option>
                <option value="RpgEmotesVerbal">RpgEmotesVerbal</option>
                <option value="RpgMagicSpell">RpgMagicSpell</option>
                <option value="RpgMotion">RpgMotion</option>
                <option value="RpgMotionGetupgetdownliedown">RpgMotionGetupgetdownliedown</option>
                <option value="RpgMotionGowait">RpgMotionGowait</option>
                <option value="RpgMotionMoveflyjumphide">RpgMotionMoveflyjumphide</option>
                <option value="RpgMotionRunwalkfollowcrawlstop">RpgMotionRunwalkfollowcrawlstop</option>
                <option value="RpgTeamCheer">RpgTeamCheer</option>
                <option value="SportChess">SportChess</option>
                <option value="SportFootballScrimmage">SportFootballScrimmage</option>
                <option value="SportGolf">SportGolf</option>
                <option value="SportGolfAction">SportGolfAction</option>
                <option value="SportGolfCaddie">SportGolfCaddie</option>
                <option value="SportGolfClub">SportGolfClub</option>
                <option value="SportRacecar">SportRacecar</option>
                <option value="SportRacecarCarinfo">SportRacecarCarinfo</option>
                <option value="SportRacecarPit">SportRacecarPit</option>
                <option value="SportRacecarRaceinfo">SportRacecarRaceinfo</option>
                <option value="Ui">Ui</option>
                <option value="UiBrowser">UiBrowser</option>
                <option value="UiContinueresumehelp">UiContinueresumehelp</option>
                <option value="UiDiscplayer">UiDiscplayer</option>
                <option value="UiHerethere">UiHerethere</option>
                <option value="UiHilo">UiHilo</option>
                <option value="UiHorvert">UiHorvert</option>
                <option value="UiMoreless">UiMoreless</option>
                <option value="UiRotateflip">UiRotateflip</option>
                <option value="UiUnderaboveover">UiUnderaboveover</option>
                <option value="UiUpdown">UiUpdown</option>
                <option value="War">War</option>
                <option value="WarAnarchy">WarAnarchy</option>
                <option value="WarCombat">WarCombat</option>
              </select>
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
