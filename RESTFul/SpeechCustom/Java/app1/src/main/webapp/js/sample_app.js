/*
 * Copyright 2015 AT&T
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
 */

// define a custom hook for loading any custom values specific to
// this sample app
load_hook = function(data) {
    var xgrammar = data.x_grammar;
    var xdictionary = data.x_dictionary;
    $('#xGrammar').val(xgrammar);
    $('#xDictionary').val(xdictionary);
    $('#linkPlay').click(function(evt) {
        evt.preventDefault();
        $('#playModal').modal('show');
    });
    var grammarContext = function() {
        context = $('#speechContext').val();
        if (context == "GrammarList") {
            $('#nameParam > option:selected').prop('selected', false);
            $('#nameParam > option:first-child').prop('selected', true);
            $('#nameParam').prop('disabled', true);
        } else {
            $('#nameParam').prop('disabled', false);
        }
    };
    grammarContext();
    $('#speechContext').change(function(){
        grammarContext();
    });
};

// vim: set ts=8 sw=4 sts=4 tw=79 ft=javascript et :
