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

var load_hook = null;

var escapeHtml = function(str) {
    return $('<div/>').text(str).html()
};

// TODO: move HTML string concatenation to use jquery functions to generate
// HTML

var generateTable = function(headers, values, caption) {
    var table = '<div class="table-responsive">';
    table += '<table class="table table-condensed table-striped table-bordered">';
    table += '<caption>' + escapeHtml(caption) + '</caption>';
    table += '<thead>';
    table += '<tr>';
    for (var i in headers) {
        var name = headers[i];
        table += '<th>' + escapeHtml(name) + '</th>';
    }
    table += '</tr>';
    table += '</thead>';
    table += '<tbody>';
    for (var i in values) {
        var value = values[i];
        table += '<tr>';
        for (var j in value) {
            var entry = value[j];
            table += '<th>' + escapeHtml(entry) + '</th>';
        }
        table += '</tr>';
    }
    table += '</tbody>';
    table += '</table>';
    table += '</div';

    return table;
}

var genericSuccess = function(response) {
    var successHtml = '<hr><div class="alert alert-success">Success: ';
    if ('text' in response) {
        successHtml += '<div>';
        successHtml += escapeHtml(response.text);
        successHtml += '</div>';
    }
    successHtml += '</div>';
    if ('tables' in response) {
        var tables = response['tables'];
        for (var i in tables) {
            var table = tables[i];
            var headers = table['headers'];
            var values = table['values'];
            var caption = table['caption'];
            var tablestr = generateTable(headers, values, caption);
            successHtml += tablestr;
        }
    }
    if ('image' in response) {
        var type = response.image.type;
        var base64 = response.image.base64;
        successHtml += '<div>';
        successHtml += '<img src="data:' + type + ';base64,' + base64 + '" />';
        successHtml += '</div>';
    }
    if ('video' in response) {
        var type = response.video.type;
        var base64 = response.video.base64;
        successHtml += '<div>';
        successHtml += '<video controls="controls" autobuffer="autobuffer" autoplay="autoplay">';
        successHtml += '<source src="data:' + type + ';base64,' + base64 + '" />';
        successHtml += '</video>';
        successHtml += '</div>';
    }
    if ('audio' in response) {
        var type = response.audio.type;
        var base64 = response.audio.base64;
        successHtml += '<div>';
        successHtml += '<audio controls="controls" autobuffer="autobuffer" autoplay="autoplay">';
        successHtml += '<source src="data:' + type + ';base64,' + base64 + '" />';
        successHtml += '</audio>';
        successHtml += '</div>';
    }
    $('#response').removeClass('hidden');
    $('#response').html(successHtml);
};

var genericError = function(response) {
    var errHtml = '<hr><div class="alert alert-danger">Error: ';
    errHtml += escapeHtml(response.text);
    errHtml += '</div>';
    $('#response').removeClass('hidden');
    $('#response').html(errHtml);
};

$(document).ready(function() {
    var authenticated = false;
    var redirect_url = null;
    var redirecting = null;
    var submit_button = null;

    var save = function(data, success_callback) {
        $("form").each(function(index) {
            var form_id = $(this).attr('id');
            data[form_id] = $(this).serializeArray();
        });
        $.post(endpoints.save, {data: JSON.stringify(data)})
        .done(function(response) {
            success_callback();
        });
    };

    var load = function() {
        $.get(endpoints.load)
        .done(function(response) {
            var data = JSON.parse(response);
            redirect_url = data.redirect_url;
            authenticated = data.authenticated;
            if ('savedData' in data && 'redirecting' in data.savedData) {
                redirecting = data.savedData.redirecting;
            }
            $('#serverTime').html(data.server_time);
            $('#download').attr('href', data.download);
            $('#github').attr('href', data.github);
            if ('savedData' in data) {
                var savedData = data.savedData;
                $.each(savedData, function(form, form_values) {
                    if ($.type(form_values) === "array") {
                        $.each(form_values, function(i, obj) {
                            var elem = '#' + form + ' [name="' + obj.name + '"]';
                            elem = $(elem);
                            if(elem.is(':checkbox')) { 
                                elem.attr('checked', true);
                            } else {
                                elem.val(obj.value);
                            }
                        });
                    }
                });
            }
            if (load_hook !== null) {
                load_hook(data);
            }
        })
        .always(function() {
            if (redirecting) {
                // resubmit the form by id stored in redirecting var
                $("#" + redirecting).submit();
                redirecting = null;
            }
        });
    };

    /************************\
    ** Form submit overides **
    \************************/

    $("form").submit(function(event) {
        var form = $(this);
        var form_id = form.attr('id');
        var button = form.find(":submit");
        if (submit_button != null)
            submit_button.button('reset');
        submit_button = button.button('loading');
        $('#response').addClass('hidden');

        var data = {};
        if (!authenticated) {
            data['redirecting'] = form_id;
        }
        save(data, function() {
            if(!authenticated) {
                if (redirect_url) {
                    redirecting = form_id;
                    window.location.assign(redirect_url);
                } else {
                    alert("invalid redirect url");
                }
            } else {
                $.post(endpoints[form_id], form.serialize())
                .done(function(response) {
                    submit_button.button('reset');
                    submit_button = null;
                    var r = JSON.parse(response);
                    if (r.success) {
                        attapisuccess[form_id](r);
                    } else {
                        genericError(r);
                    }
                });
            }
        });
        event.preventDefault();
    });

    // Load data on page load
    load();
});

// vim: set ts=8 sw=4 sts=4 tw=79 ft=javascript et :
