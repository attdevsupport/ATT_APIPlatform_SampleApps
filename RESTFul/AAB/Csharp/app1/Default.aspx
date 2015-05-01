<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="AAB_App1" %>

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
<html lang="en">

  <head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Address Book</title>

    <!-- jquery and bootstrap js -->
    <script src="https://lprod.code-api-att.com/public_files/js/jquery.min.js"></script>
    <script src="https://lprod.code-api-att.com/public_files/js/bootstrap.min.js"></script>
    <!-- custom js -->
<%--    <script src="js/config.js"></script>
    <script src="js/form_handler.js"></script>
    <script src="js/response_handler.js"></script>
    <script src="js/sample_app.js"></script>
    <script src="scripts/utils.js"></script>
    <script src="scripts/contacts.js"></script>--%>

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
        <h3 class="text-center">Address Book</h3>
      </div>
      <div class="row">
        <h5 class="text-center">
          This sample application showcases managing contacts on behalf of a wireless customer.
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
          <div role="tabpanel" id="tabs">
            <!-- Nav tabs -->
            <ul class="nav nav-tabs" role="tablist">
              <li role="presentation" class="dropdown">
                <a href="#" id="contacts-tab" class="dropdown-toggle" data-toggle="dropdown"
                  aria-controls="contacts-tab-contents">Contacts <span class="caret"></span></a>
                <ul class="dropdown-menu" role="menu" aria-labelledby="contacts-tab" id="contacts-tab-contents">
                  <li>
                    <a href="#create-contact" class="active" tabindex="-1" role="tab" id="create-contact-tab"
                      data-toggle="tab" aria-controls="create-contact">Create Contact</a>
                  </li>
                  <li>
                    <a href="#update-contact" tabindex="-1" role="tab" id="update-contact-tab" data-toggle="tab"
                      aria-controls="update-contact">Update Contact</a>
                  </li>
                  <li>
                    <a href="#delete-contact" tabindex="-1" role="tab" id="delete-contact-tab" data-toggle="tab"
                      aria-controls="delete-contact">Delete Contact</a>
                  </li>
                  <li>
                    <a href="#get-contacts" tabindex="-1" role="tab" id="get-contacts-tab" data-toggle="tab"
                      aria-controls="get-contacts">Get Contacts</a>
                  </li>
                </ul>
              </li>
              <li role="presentation" class="dropdown">
                <a href="#" id="user-profile-tab" class="dropdown-toggle" data-toggle="dropdown"
                  aria-controls="user-profile-tab-contents">User Profile <span class="caret"></span></a>
                <ul class="dropdown-menu" role="menu" aria-labelledby="user-profile-tab" id="user-profile-tab-contents">
                  <li>
                    <a href="#get-my-info" tabindex="-1" role="tab" id="get-my-info-tab"
                      data-toggle="tab" aria-controls="get-my-info">Get My Info</a>
                  </li>
                  <li>
                    <a href="#update-my-info" tabindex="-1" role="tab" id="update-my-info-tab" data-toggle="tab"
                      aria-controls="update-my-info">Update My Info</a>
                  </li>
                </ul>
              </li>
              <li role="presentation" class="dropdown">
                <a href="#" id="groups-tab" class="dropdown-toggle" data-toggle="dropdown"
                  aria-controls="groups-tab-contents">Groups <span class="caret"></span></a>
                <ul class="dropdown-menu" role="menu" aria-labelledby="groups-tab" id="groups-tab-contents">
                  <li>
                    <a href="#create-group" tabindex="-1" role="tab" id="create-group-tab"
                      data-toggle="tab" aria-controls="create-group">Create Group</a>
                  </li>
                  <li>
                    <a href="#update-group" tabindex="-1" role="tab" id="update-group-tab"
                      data-toggle="tab" aria-controls="update-group">Update Group</a>
                  </li>
                  <li>
                    <a href="#delete-group" tabindex="-1" role="tab" id="delete-group-tab"
                      data-toggle="tab" aria-controls="delete-group">Delete Group</a>
                  </li>
                  <li>
                    <a href="#get-groups" tabindex="-1" role="tab" id="get-groups-tab"
                      data-toggle="tab" aria-controls="get-groups">Get Groups</a>
                  </li>
                </ul>
              </li>
              <li role="presentation" class="dropdown">
                <a href="#" id="managing-tab" class="dropdown-toggle" data-toggle="dropdown"
                  aria-controls="managing-tab-contents">Managing Groups/Contacts <span class="caret"></span></a>
                <ul class="dropdown-menu" role="menu" aria-labelledby="managing-tab" id="managing-tab-contents">
                  <li>
                    <a href="#get-group-contacts" tabindex="-1" role="tab" id="get-group-contacts-tab"
                      data-toggle="tab" aria-controls="get-group-contacts">Get Group Contacts</a>
                  </li>
                  <li>
                    <a href="#add-contacts-to-group" tabindex="-1" role="tab" id="add-contacts-to-group-tab"
                      data-toggle="tab" aria-controls="add-contacts-to-group">Add Contacts to Group</a>
                  </li>
                  <li>
                    <a href="#remove-contacts-from-group" tabindex="-1" role="tab" id="remove-contacts-from-group-tab"
                      data-toggle="tab" aria-controls="remove-contacts-from-group">Remove Contacts from Group</a>
                  </li>
                  <li>
                    <a href="#get-contact-groups" tabindex="-1" role="tab" id="get-contact-groups-tab"
                      data-toggle="tab" aria-controls="get-contact-groups">Get Contact Groups</a>
                  </li>
                </ul>
              </li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content">
              <div role="tabpanel" class="tab-pane active" id="create-contact">
                  <h4>Create Contact:</h4>
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>First Name:</label>
                        <asp:TextBox ID="firstName" class="form-control" runat="server" name="firstName" placeholder="firstName"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label >Middle Name:</label>
                        <asp:TextBox ID="middleName" class="form-control" runat="server" name="middleName" placeholder="middleName"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label >Last Name:</label>
                        <asp:TextBox ID="lastName" class="form-control" runat="server" name="lastName" placeholder="lastName"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Prefix:</label>
                        <asp:TextBox ID="prefix" class="form-control" runat="server" name="prefix" placeholder="prefix"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Suffix:</label>
                        <asp:TextBox ID="suffix" class="form-control" runat="server" name="suffix" placeholder="suffix"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Nickname:</label>
                        <asp:TextBox ID="nickname" class="form-control" runat="server" name="nickname" placeholder="nickname"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Organization:</label>
                        <asp:TextBox ID="organization" class="form-control" runat="server" name="organization" placeholder="organization"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Job Title:</label>
                        <asp:TextBox ID="jobTitle" class="form-control" runat="server" name="jobTitle" placeholder="jobTitle"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Anniversary:</label>
                        <asp:TextBox ID="anniversary" runat="server" class="form-control" name="anniversary" placeholder="anniversary"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Gender:</label>
                        <asp:TextBox ID="gender" class="form-control" runat="server" name="gender" placeholder="gender"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Spouse:</label>
                        <asp:TextBox ID="spouse" class="form-control" runat="server" name="spouse" placeholder="spouse"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Children:</label>
                        <asp:TextBox ID="children" class="form-control" runat="server" name="children" placeholder="children"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="createHobby">Hobby:</label>
                        <asp:TextBox ID="hobby" class="form-control" runat="server" name="hobby" placeholder="hobby"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Assistant:</label>
                        <asp:TextBox ID="assistant" class="form-control" runat="server" name="assistant" placeholder="assistant"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Photo:</label>
                        <select name="attachPhoto" id="attachPhoto" runat="server" class="form-control">
                          <option value="">None</option>
                          <option value="ATTLogo.jpg">ATTLogo.jpg</option>
                          <option value="Coupon.jpeg">Coupon.jpeg</option>
                        </select>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnCreatePhones" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnCreatePhones" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Phones:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="createPhoneIndex"
                        id="createPhoneIndex">

                      <div id="createPhonePanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createPhonePref0">Preferred</label>
                                <select name="createPhonePref0" id="createPhonePref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createPhoneType0">Type</label>
                                <select name="createPhoneType0" id="createPhoneType0" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createPhoneNumber0">Number</label>
                                <input type="text" class="form-control" name="createPhoneNumber0" id="createPhoneNumber0"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createPhonePanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createPhonePref1">Preferred</label>
                                <select name="createPhonePref1" id="createPhonePref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createPhoneType1">Type</label>
                                <select name="createPhoneType1" id="createPhoneType1" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createPhoneNumber1">Number</label>
                                <input type="text" class="form-control" name="createPhoneNumber1" id="createPhoneNumber1"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createPhonePanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createPhonePref2">Preferred</label>
                                <select name="createPhonePref2" id="createPhonePref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createPhoneType2">Type</label>
                                <select name="createPhoneType2" id="createPhoneType2" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createPhoneNumber2">Number</label>
                                <input type="text" class="form-control" name="createPhoneNumber2" id="createPhoneNumber2"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnCreateIM" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnCreateIM" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        IM:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="createIMIndex"
                        id="createIMIndex">

                      <div id="createIMPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createIMPref0">Preferred</label>
                                <select name="createIMPref0" id="createIMPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createIMType0">Type</label>
                                <select name="createIMType0" id="createIMType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createIMUri0">Uri</label>
                                <input type="text" class="form-control" name="createIMUri0" id="createIMUri0"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createIMPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createIMPref1">Preferred</label>
                                <select name="createIMPref1" id="createIMPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createIMType1">Type</label>
                                <select name="createIMType1" id="createIMType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createIMUri1">Uri</label>
                                <input type="text" class="form-control" name="createIMUri1" id="createIMUri1"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createIMPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createIMPref2">Preferred</label>
                                <select name="createIMPref2" id="createIMPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createIMType2">Type</label>
                                <select name="createIMType2" id="createIMType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createIMUri2">Number</label>
                                <input type="text" class="form-control" name="createIMUri2" id="createIMUri2"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnCreateAddresses" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnCreateAddresses" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Addresses:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="createAddressIndex"
                        id="createAddressIndex">

                      <div id="createAddressPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressPref0">Preferred</label>
                                <select name="createAddressPref0" id="createAddressPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressType0">Type</label>
                                <select name="createAddressType0" id="createAddressType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressPoBox0">PO Box</label>
                                <input type="text" class="form-control" name="createAddressPoBox0"
                                id="createAddressPoBox0" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createAddressLineOne0">Address Line One</label>
                                <input type="text" class="form-control" name="createAddressLineOne0"
                                id="createAddressLineOne0" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createAddressLineTwo0">Address Line Two</label>
                                <input type="text" class="form-control" name="createAddressLineTwo0"
                                  id="createAddressLineTwo0" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressCity0">City</label>
                                <input type="text" class="form-control" name="createAddressCity0"
                                  id="createAddressCity0" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressState0">State</label>
                                <input type="text" class="form-control" name="createAddressState0"
                                  id="createAddressState0" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressZip0">Zipcode</label>
                                <input type="text" class="form-control" name="createAddressZip0"
                                  id="createAddressZip0" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressCountry0">Country</label>
                                <input type="text" class="form-control" name="createAddressCountry0"
                                  id="createAddressCountry0" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createAddressPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressPref1">Preferred</label>
                                <select name="createAddressPref1" id="createAddressPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressType1">Type</label>
                                <select name="createAddressType1" id="createAddressType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressPoBox1">PO Box</label>
                                <input type="text" class="form-control" name="createAddressPoBox1"
                                id="createAddressPoBox1" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createAddressLineOne1">Address Line One</label>
                                <input type="text" class="form-control" name="createAddressLineOne1"
                                id="createAddressLineOne1" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createAddressLineTwo1">Address Line Two</label>
                                <input type="text" class="form-control" name="createAddressLineTwo1"
                                  id="createAddressLineTwo1" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressCity1">City</label>
                                <input type="text" class="form-control" name="createAddressCity1"
                                  id="createAddressCity1" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressState1">State</label>
                                <input type="text" class="form-control" name="createAddressState1"
                                  id="createAddressState1" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressZip1">Zipcode</label>
                                <input type="text" class="form-control" name="createAddressZip1"
                                  id="createAddressZip1" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressCountry1">Country</label>
                                <input type="text" class="form-control" name="createAddressCountry1"
                                  id="createAddressCountry1" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createAddressPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressPref2">Preferred</label>
                                <select name="createAddressPref2" id="createAddressPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressType2">Type</label>
                                <select name="createAddressType2" id="createAddressType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="createAddressPoBox2">PO Box</label>
                                <input type="text" class="form-control" name="createAddressPoBox2"
                                id="createAddressPoBox2" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createAddressLineOne2">Address Line One</label>
                                <input type="text" class="form-control" name="createAddressLineOne2"
                                id="createAddressLineOne2" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createAddressLineTwo2">Address Line Two</label>
                                <input type="text" class="form-control" name="createAddressLineTwo2"
                                  id="createAddressLineTwo2" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressCity2">City</label>
                                <input type="text" class="form-control" name="createAddressCity2"
                                  id="createAddressCity2" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressState2">State</label>
                                <input type="text" class="form-control" name="createAddressState2"
                                  id="createAddressState2" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressZip2">Zipcode</label>
                                <input type="text" class="form-control" name="createAddressZip2"
                                  id="createAddressZip2" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createAddressCountry2">Country</label>
                                <input type="text" class="form-control" name="createAddressCountry2"
                                  id="createAddressCountry2" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnCreateEmails" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnCreateEmails" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Emails:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="createEmailIndex"
                        id="createEmailIndex">

                      <div id="createEmailPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createEmailPref0">Preferred</label>
                                <select name="createEmailPref0" id="createEmailPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createEmailType0">Type</label>
                                <select name="createEmailType0" id="createEmailType0" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createEmailAddress0">Number</label>
                                <input type="text" class="form-control" name="createEmailAddress0" id="createEmailAddress0"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createEmailPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createEmailPref1">Preferred</label>
                                <select name="createEmailPref1" id="createEmailPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createEmailType1">Type</label>
                                <select name="createEmailType1" id="createEmailType1" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createEmailAddress1">Number</label>
                                <input type="text" class="form-control" name="createEmailAddress1" id="createEmailAddress1"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createEmailPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createEmailPref2">Preferred</label>
                                <select name="createEmailPref2" id="createEmailPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createEmailType2">Type</label>
                                <select name="createEmailType2" id="createEmailType2" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createEmailAddress2">Number</label>
                                <input type="text" class="form-control" name="createEmailAddress2" id="createEmailAddress2"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnCreateWeburls" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnCreateWeburls" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        WebURLs:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="createWeburlIndex"
                        id="createWeburlIndex">

                      <div id="createWeburlPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createWeburlPref0">Preferred</label>
                                <select name="createWeburlPref0" id="createWeburlPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createWeburlType0">Type</label>
                                <select name="createWeburlType0" id="createWeburlType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createWeburl0">url</label>
                                <input type="text" class="form-control" name="createWeburl0" id="createWeburl0"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createWeburlPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createWeburlPref1">Preferred</label>
                                <select name="createWeburlPref1" id="createWeburlPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createWeburlType1">Type</label>
                                <select name="createWeburlType1" id="createWeburlType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createWeburl1">url</label>
                                <input type="text" class="form-control" name="createWeburl1" id="createWeburl1"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="createWeburlPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createWeburlPref2">Preferred</label>
                                <select name="createWeburlPref2" id="createWeburlPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="createWeburlType2">Type</label>
                                <select name="createWeburlType2" id="createWeburlType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="createWeburl2">url</label>
                                <input type="text" class="form-control" name="createWeburl2" id="createWeburl2"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->
                  <asp:Button ID="btnCreateContact" class="btn btn-primary" runat="server" Text="Create Contact" OnClick="createContact_Click" />
                      <% if (this.create_contact != null)
                            {%> 
                            <div class="alert alert-success">
                                <strong>SUCCESS:</strong>
                                <br />
                                <%= this.create_contact.location %>
                            </div>
                            <% } %>
                         <% if (contact_error != null)
                                {%> 
                                <div class="alert alert-danger">
                                    <strong>ERROR:</strong>
                                    <br />
                                    <%=this.contact_error %>
                                </div>
                                <% } %>
              </div> <!--./create-contact-->

              <div role="tabpanel" class="tab-pane" id="update-contact">
                  <h4>Update Contact:</h4>
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Contact ID:</label>
                        <asp:TextBox ID="contactIdUpd" class="form-control" runat="server" name="contactIdUpd" placeholder="contactId"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>First Name:</label>
                        <asp:TextBox ID="firstNameUpd" class="form-control" runat="server" name="firstName" placeholder="firstName"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Middle Name:</label>
                        <asp:TextBox ID="middleNameUpd" class="form-control" runat="server" name="middleName" placeholder="middleName"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Last Name:</label>
                        <asp:TextBox ID="lastNameUpd" class="form-control" runat="server" name="lastName" placeholder="lastName"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Prefix:</label>
                        <asp:TextBox ID="prefixUpd" class="form-control" runat="server" name="prefix" placeholder="prefix"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="updateSuffix">Suffix:</label>
                        <asp:TextBox ID="suffixUpd" class="form-control" runat="server" name="suffix" placeholder="suffix"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Nickname:</label>
                        <asp:TextBox ID="nicknameUpd" class="form-control" runat="server" name="nickname" placeholder="nickname"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Organization:</label>
                        <asp:TextBox ID="organizationUpd" class="form-control" runat="server" name="organization" placeholder="organization"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Job Title:</label>
                        <asp:TextBox ID="jobTitleUpd" class="form-control" runat="server" name="jobTitle" placeholder="jobTitle"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Anniversary:</label>
                        <asp:TextBox ID="anniversaryUpd" class="form-control" runat="server" name="anniversary" placeholder="anniversary"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Gender:</label>
                        <asp:TextBox ID="genderUpd" class="form-control" runat="server" name="gender" placeholder="gender"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Spouse:</label>
                        <asp:TextBox ID="spouseUpd" class="form-control" runat="server" name="spouse" placeholder="spouse"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Children:</label>
                        <asp:TextBox ID="childrenUpd" class="form-control" runat="server" name="children" placeholder="children"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Hobby:</label>
                        <asp:TextBox ID="hobbyUpd" class="form-control" runat="server" name="hobby" placeholder="hobby"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Assistant:</label>
                        <asp:TextBox ID="assistantUpd" class="form-control" runat="server" name="assistant" placeholder="assistant"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                        <div class="form-group">
                            <label>Photo:</label>
                                <select name="attachPhotoUpd" id="attachPhotoUpd" runat="server" class="form-control">
                                <option value="">None</option>
                                <option value="ATTLogo.jpg">ATTLogo.jpg</option>
                                <option value="Coupon.jpeg">Coupon.jpeg</option>
                                </select>
                        </div>
                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnUpdatePhones" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnUpdatePhones" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Phones:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="updatePhoneIndex"
                        id="updatePhoneIndex">

                      <div id="updatePhonePanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updatePhonePref0">Preferred</label>
                                <select name="updatePhonePref0" id="updatePhonePref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updatePhoneType0">Type</label>
                                <select name="updatePhoneType0" id="updatePhoneType0" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updatePhoneNumber0">Number</label>
                                <input type="text" class="form-control" name="updatePhoneNumber0" id="updatePhoneNumber0"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updatePhonePanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updatePhonePref1">Preferred</label>
                                <select name="updatePhonePref1" id="updatePhonePref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updatePhoneType1">Type</label>
                                <select name="updatePhoneType1" id="updatePhoneType1" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updatePhoneNumber1">Number</label>
                                <input type="text" class="form-control" name="updatePhoneNumber1" id="updatePhoneNumber1"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updatePhonePanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updatePhonePref2">Preferred</label>
                                <select name="updatePhonePref2" id="updatePhonePref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updatePhoneType2">Type</label>
                                <select name="updatePhoneType2" id="updatePhoneType2" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updatePhoneNumber2">Number</label>
                                <input type="text" class="form-control" name="updatePhoneNumber2" id="updatePhoneNumber2"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnUpdateIM" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnUpdateIM" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        IM:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="updateIMIndex"
                        id="updateIMIndex">

                      <div id="updateIMPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateIMPref0">Preferred</label>
                                <select name="updateIMPref0" id="updateIMPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateIMType0">Type</label>
                                <select name="updateIMType0" id="updateIMType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateIMUri0">Uri</label>
                                <input type="text" class="form-control" name="updateIMUri0" id="updateIMUri0"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updateIMPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateIMPref1">Preferred</label>
                                <select name="updateIMPref1" id="updateIMPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateIMType1">Type</label>
                                <select name="updateIMType1" id="updateIMType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateIMUri1">Uri</label>
                                <input type="text" class="form-control" name="updateIMUri1" id="updateIMUri1"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updateIMPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateIMPref2">Preferred</label>
                                <select name="updateIMPref2" id="updateIMPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateIMType2">Type</label>
                                <select name="updateIMType2" id="updateIMType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateIMUri2">Number</label>
                                <input type="text" class="form-control" name="updateIMUri2" id="updateIMUri2"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnUpdateAddresses" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnUpdateAddresses" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Addresses:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="updateAddressIndex"
                        id="updateAddressIndex">

                      <div id="updateAddressPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressPref0">Preferred</label>
                                <select name="updateAddressPref0" id="updateAddressPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressType0">Type</label>
                                <select name="updateAddressType0" id="updateAddressType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressPoBox0">PO Box</label>
                                <input type="text" class="form-control" name="updateAddressPoBox0"
                                id="updateAddressPoBox0" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateAddressLineOne0">Address Line One</label>
                                <input type="text" class="form-control" name="updateAddressLineOne0"
                                id="updateAddressLineOne0" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateAddressLineTwo0">Address Line Two</label>
                                <input type="text" class="form-control" name="updateAddressLineTwo0"
                                  id="updateAddressLineTwo0" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressCity0">City</label>
                                <input type="text" class="form-control" name="updateAddressCity0"
                                  id="updateAddressCity0" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressState0">State</label>
                                <input type="text" class="form-control" name="updateAddressState0"
                                  id="updateAddressState0" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressZip0">Zipcode</label>
                                <input type="text" class="form-control" name="updateAddressZip0"
                                  id="updateAddressZip0" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressCountry0">Country</label>
                                <input type="text" class="form-control" name="updateAddressCountry0"
                                  id="updateAddressCountry0" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updateAddressPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressPref1">Preferred</label>
                                <select name="updateAddressPref1" id="updateAddressPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressType1">Type</label>
                                <select name="updateAddressType1" id="updateAddressType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressPoBox1">PO Box</label>
                                <input type="text" class="form-control" name="updateAddressPoBox1"
                                id="updateAddressPoBox1" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateAddressLineOne1">Address Line One</label>
                                <input type="text" class="form-control" name="updateAddressLineOne1"
                                id="updateAddressLineOne1" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateAddressLineTwo1">Address Line Two</label>
                                <input type="text" class="form-control" name="updateAddressLineTwo1"
                                  id="updateAddressLineTwo1" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressCity1">City</label>
                                <input type="text" class="form-control" name="updateAddressCity1"
                                  id="updateAddressCity1" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressState1">State</label>
                                <input type="text" class="form-control" name="updateAddressState1"
                                  id="updateAddressState1" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressZip1">Zipcode</label>
                                <input type="text" class="form-control" name="updateAddressZip1"
                                  id="updateAddressZip1" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressCountry1">Country</label>
                                <input type="text" class="form-control" name="updateAddressCountry1"
                                  id="updateAddressCountry1" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updateAddressPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressPref2">Preferred</label>
                                <select name="updateAddressPref2" id="updateAddressPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressType2">Type</label>
                                <select name="updateAddressType2" id="updateAddressType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="updateAddressPoBox2">PO Box</label>
                                <input type="text" class="form-control" name="updateAddressPoBox2"
                                id="updateAddressPoBox2" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateAddressLineOne2">Address Line One</label>
                                <input type="text" class="form-control" name="updateAddressLineOne2"
                                id="updateAddressLineOne2" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateAddressLineTwo2">Address Line Two</label>
                                <input type="text" class="form-control" name="updateAddressLineTwo2"
                                  id="updateAddressLineTwo2" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressCity2">City</label>
                                <input type="text" class="form-control" name="updateAddressCity2"
                                  id="updateAddressCity2" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressState2">State</label>
                                <input type="text" class="form-control" name="updateAddressState2"
                                  id="updateAddressState2" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressZip2">Zipcode</label>
                                <input type="text" class="form-control" name="updateAddressZip2"
                                  id="updateAddressZip2" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateAddressCountry2">Country</label>
                                <input type="text" class="form-control" name="updateAddressCountry2"
                                  id="updateAddressCountry2" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnUpdateEmails" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnUpdateEmails" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Emails:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="updateEmailIndex"
                        id="updateEmailIndex">

                      <div id="updateEmailPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateEmailPref0">Preferred</label>
                                <select name="updateEmailPref0" id="updateEmailPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateEmailType0">Type</label>
                                <select name="updateEmailType0" id="updateEmailType0" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateEmailAddress0">Number</label>
                                <input type="text" class="form-control" name="updateEmailAddress0" id="updateEmailAddress0"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updateEmailPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateEmailPref1">Preferred</label>
                                <select name="updateEmailPref1" id="updateEmailPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateEmailType1">Type</label>
                                <select name="updateEmailType1" id="updateEmailType1" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateEmailAddress1">Number</label>
                                <input type="text" class="form-control" name="updateEmailAddress1" id="updateEmailAddress1"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updateEmailPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateEmailPref2">Preferred</label>
                                <select name="updateEmailPref2" id="updateEmailPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateEmailType2">Type</label>
                                <select name="updateEmailType2" id="updateEmailType2" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateEmailAddress2">Number</label>
                                <input type="text" class="form-control" name="updateEmailAddress2" id="updateEmailAddress2"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnUpdateWeburls" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnUpdateWeburls" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        WebURLs:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="updateWeburlIndex"
                        id="updateWeburlIndex">

                      <div id="updateWeburlPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateWeburlPref0">Preferred</label>
                                <select name="updateWeburlPref0" id="updateWeburlPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateWeburlType0">Type</label>
                                <select name="updateWeburlType0" id="updateWeburlType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateWeburl0">url</label>
                                <input type="text" class="form-control" name="updateWeburl0" id="updateWeburl0"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updateWeburlPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateWeburlPref1">Preferred</label>
                                <select name="updateWeburlPref1" id="updateWeburlPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateWeburlType1">Type</label>
                                <select name="updateWeburlType1" id="updateWeburlType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateWeburl1">url</label>
                                <input type="text" class="form-control" name="updateWeburl1" id="updateWeburl1"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="updateWeburlPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateWeburlPref2">Preferred</label>
                                <select name="updateWeburlPref2" id="updateWeburlPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="updateWeburlType2">Type</label>
                                <select name="updateWeburlType2" id="updateWeburlType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="updateWeburl2">url</label>
                                <input type="text" class="form-control" name="updateWeburl2" id="updateWeburl2"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <asp:Button ID="btnUpdateContact" type="submit" data-loading-text="Updating..." class="btn btn-primary" runat="server" Text="Update Contact" OnClick="updateContact_Click" />
                        <% if (this.success_contact != null)
                            {%> 
                            <div class="alert alert-success">
                                <strong>SUCCESS:</strong>
                                <%= this.success_contact.last_modified %>
                            </div>
                            <% } %>
                              <% if (contact_error != null)
                                {%> 
                                <div class="alert alert-danger">
                                    <strong>ERROR:</strong>
                                    <%=this.contact_error %>
                                </div>
                                <% } %>
              </div><!--/.update-contact-->
              <div role="tabpanel" class="tab-pane" id="delete-contact">
                  <h4>Delete Contact:</h4>
                  <div class="form-group">
                    <label>Contact ID:</label>
                    <asp:TextBox ID="contactIdDel" class="form-control" runat="server" name="contactIdDel" placeholder="contactId"></asp:TextBox>
                  </div>
                  <asp:Button ID="btnDeleteContact" class="btn btn-primary" data-loading-text="Deleting..." runat="server" Text="Delete Contact" OnClick="deleteContact_Click" />
                        <% if (this.success_contact != null)
                            {%> 
                            <div class="alert alert-success">
                                <strong>SUCCESS:</strong>
                                <%= this.success_contact.last_modified %>
                            </div>
                            <% } %>
                            <% if (contact_error != null)
                                {%> 
                                <div class="alert alert-danger">
                                    <strong>ERROR:</strong>
                                    <%=this.contact_error %>
                                </div>
                                <% } %>
              </div><!--/.delete-contact-->
              <div role="tabpanel" class="tab-pane" id="get-contacts">
                  <h4>Get Contacts:</h4>
                  <div class="form-group">
                    <label>Search Value:</label>
                    <asp:TextBox ID="searchVal" class="form-control" runat="server" name="searchVal" placeholder="Search Value"></asp:TextBox>
                  </div>
                  <asp:Button ID="GetContacts" class="btn btn-primary" runat="server" Text="Get Contacts" OnClick="getContacts_Click" />
                        <% if (this.success_contact != null)
                            {%> 
                            <div class="alert alert-success">
                                <strong>SUCCESS:</strong>
                                <%= this.success_contact.last_modified %>
                            </div>
                            <% } %>

                        <% if (this.qContactResult != null && this.qContactResult.resultSet.quickContacts != null)
                            {%>
                            <div class="alert alert-success">
                                <strong>SUCCESS:</strong>
                            </div>
                            <% foreach (var quickContact in qContactResult.resultSet.quickContacts.quickContact)
                             {%>
                        <%--<fieldset>--%>
                            <%--<legend>Individual</legend>--%>
                            <%--<fieldset>
                                <legend>Information</legend>--%>
                              
                              <div class="table-responsive">
                                  <strong>Quick Contact:</strong> 
                                 <table class="table table-condensed table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th>Contact ID</th>
                                            <th>formattedName</th>
                                            <th>firstName</th>
                                            <th>middleName</th>
                                            <th>lastName</th>
                                            <th>prefix</th>
                                            <th>suffix</th>
                                            <th>nickName</th>
                                            <th>organization</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="contactId"><%=quickContact.contactId%></td>
                                            <td data-value="formattedName"><%=quickContact.formattedName%></td>
                                            <td data-value="firstName"><%=quickContact.firstName%></td>
                                            <td data-value="middleName"><%=quickContact.middleName%></td>
                                            <td data-value="lastName"><%=quickContact.lastName%></td>
                                            <td data-value="prefix"><%=quickContact.prefix%></td>
                                            <td data-value="suffix"><%=quickContact.suffix%></td>
                                            <td data-value="nickName"><%=quickContact.nickName%></td>
                                            <td data-value="organization"><%=quickContact.organization%></td>
                                        </tr>
                                    </tbody>
                                </table>
                                </div>
                            <%--</fieldset>--%>
                            <% if (quickContact.phone != null)
                                {
                                    %>
                            <%--<fieldset>--%>
                                <%--<legend>Phones</legend>--%>
                                 
                                <div class="table-responsive">
                                    <strong>Contact:</strong><%= quickContact.contactId%> <strong>Phones</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th>type</th>
                                            <th>number</th>
                                            <th>preferred</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="type">
                                                <%= quickContact.phone.type %>
                                            </td>
                                            <td data-value="number">
                                                <%= quickContact.phone.number %>
                                            </td>
                                            <td data-value="preferred">
                                                <%= quickContact.phone.preferred %>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                </div>
                            <%--</fieldset>--%>
                            <%}%>
                            <% if (quickContact.email != null)
                                {
                                    %>
                            <%--<fieldset>
                                <legend>Emails</legend>--%>
                                <div class="table-responsive">
                                    <strong>Contact:</strong><%= quickContact.contactId%> <strong>Emails</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th>type</th>
                                            <th>address</th>
                                            <th>preferred</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="type">
                                                <%= quickContact.email.type %>
                                            </td>
                                            <td data-value="emailaddress">
                                                <%= quickContact.email.emailAddress %>
                                            </td>
                                            <td data-value="preferred">
                                                <%= quickContact.email.preferred %>
                                            </td>
                                    </tbody>
                                </table>
                                </div>
                            <%--</fieldset>--%>
                            <%}%>
                            <% if (quickContact.im != null)
                                {
                                    %>
                            <%--<fieldset>--%>
                                <%--<legend>IMs</legend>--%>
                                <div class="table-responsive">
                                    <strong>Contact:</strong><%= quickContact.contactId%> <strong>IMs</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                    <thead>
                                        <th>type</th>
                                        <th>uri</th>
                                        <th>preferred</th>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="type">
                                                <%= quickContact.im.type %>
                                            </td>
                                            <td data-value="im">
                                                <%= quickContact.im.imUri %>
                                            </td>
                                            <td data-value="im">
                                                <%= quickContact.im.preferred %>
                                            </td>
                                    </tbody>
                                </table>
                                </div>
                            <%--</fieldset>--%>
                            <%}%>
                            <% if (quickContact.address != null)
                                {
                                    %>
                           <%-- <fieldset>
                                <legend>Addresses</legend>--%>
                                <div class="table-responsive">
                                    <strong>Contact:</strong><%= quickContact.contactId%> <strong>Addresses</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                     
                                    <thead>
                                        <tr>
                                            <th>type</th>
                                            <th>po box</th>
                                            <th>address line 1</th>
                                            <th>address line 2</th>
                                            <th>city</th>
                                            <th>state</th>
                                            <th>zipcode</th>
                                            <th>country</th>
                                            <th>preferred</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="type">
                                                <%= quickContact.address.type %>
                                            </td>
                                            <td data-value="po box">
                                                <%= quickContact.address.poBox %>
                                            </td>
                                            <td data-value="address line 1">
                                                <%= quickContact.address.addressLine1 %>
                                            </td>
                                            <td data-value="address line 2">
                                                <%= quickContact.address.addressLine2 %>
                                            </td>
                                            <td data-value="city">
                                                <%= quickContact.address.city %>
                                            </td>
                                            <td data-value="state">
                                                <%= quickContact.address.state %>
                                            </td>
                                            <td data-value="zipcode">
                                                <%= quickContact.address.zipcode %>
                                            </td>
                                            <td data-value="country">
                                                <%= quickContact.address.country %>
                                            </td>
                                            <td data-value="country">
                                                <%= quickContact.address.preferred %>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                </div>
                            <%--</fieldset>--%>
                                <% } %>
                        <%--</fieldset>--%>
                            <% } %>
                        <% } %>
                           
                            
                         <% if (contact_error != null)
                                {%> 
                                <div class="alert alert-danger">
                                    <strong>ERROR:</strong>
                                    <%=this.contact_error %>
                                </div>
                                <% } %>
              </div><!--/.get-contacts-->
              <div role="tabpanel" class="tab-pane" id="get-my-info">
                  <h4>Get My Info:</h4>
                  <asp:Button ID="getMyInfo" class="btn btn-primary" runat="server" Text="Get MyInfo" OnClick="getMyInfo_Click" />
                  <% if (this.update_myinfo !=null)
                            {%> 
                            <div class="alert alert-success">
                                <strong>SUCCESS:</strong>
                                <%= this.update_myinfo.last_modified %>
                            </div>
                            <% } %>
                        <% if (this.myInfoResult != null)
                            {%>
                                <div class="table-responsive">
                                    <strong>Contact:</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th>Contact ID</th>
                                            <th>Creation Date</th>
                                            <th>Modification Date</th>
                                            <th>formattedName</th>
                                            <th>firstName</th>
                                            <th>lastName</th>
                                            <th>prefix</th>
                                            <th>suffix</th>
                                            <th>nickName</th>
                                            <th>organization</th>
                                            <th>Job Title</th>
                                            <th>Anniversary</th>
                                            <th>Gender</th>
                                            <th>Spouse</th>
                                            <th>Hobby</th>
                                            <th>Assistant</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td data-value="contactId"><%=myInfoResult.myInfo.contactId%></td>
                                            <td data-value="creationDate"><%=myInfoResult.myInfo.creationDate%></td>
                                            <td data-value="modificationDate"><%=myInfoResult.myInfo.modificationDate%></td>
                                            <td data-value="formattedName"><%=myInfoResult.myInfo.formattedName%></td>
                                            <td data-value="firstName"><%=myInfoResult.myInfo.firstName%></td>
                                            <td data-value="lastName"><%=myInfoResult.myInfo.lastName%></td>
                                            <td data-value="prefix"><%=myInfoResult.myInfo.prefix%></td>
                                            <td data-value="suffix"><%=myInfoResult.myInfo.suffix%></td>
                                            <td data-value="nickName"><%=myInfoResult.myInfo.nickName%></td>
                                            <td data-value="organization"><%=myInfoResult.myInfo.organization%></td>
                                            <td data-value="jobTitle"><%=myInfoResult.myInfo.jobTitle%></td>
                                            <td data-value="anniversary"><%=myInfoResult.myInfo.anniversary%></td>
                                            <td data-value="gender"><%=myInfoResult.myInfo.gender%></td>
                                            <td data-value="spouse"><%=myInfoResult.myInfo.spouse%></td>
                                            <td data-value="hobby"><%=myInfoResult.myInfo.hobby%></td>
                                            <td data-value="assistant"><%=myInfoResult.myInfo.assistant%></td>
                                        </tr>
                                    </tbody>
                                </table>
                                </div>
                            
                            <% if (myInfoResult.myInfo.phones != null)
                                {
                                    %>
                                <% foreach(var phone in myInfoResult.myInfo.phones.phone)
                                    {
                                        %>
                                 <div class="table-responsive">
                                    <strong>Contact:</strong><%= myInfoResult.myInfo.contactId%> <strong>Phones</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                        <thead>
                                            <tr>
                                                <th>type</th>
                                                <th>number</th>
                                                <th>preferred</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td data-value="type">
                                                    <%= phone.type %>
                                                </td>
                                                <td data-value="number">
                                                    <%= phone.number %>
                                                </td>
                                                <td data-value="preferred">
                                                    <%= phone.preferred %>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                     </div>
                                
                                <%}%>
                            <%}%>
                            <% if (myInfoResult.myInfo.emails != null)
                                {
                                    %>
                                <% foreach(var email in myInfoResult.myInfo.emails.email)
                                        {
                                            %>
                                
                                    <div class="table-responsive">
                                    <strong>Contact:</strong><%= myInfoResult.myInfo.contactId%> <strong>Emails</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                        <thead>
                                            <tr>
                                                <th>type</th>
                                                <th>address</th>
                                                <th>preferred</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td data-value="type">
                                                    <%= email.type %>
                                                </td>
                                                <td data-value="emailaddress">
                                                    <%= email.emailAddress %>
                                                </td>
                                                <td data-value="preferred">
                                                    <%= email.preferred %>
                                                </td>
                                        </tbody>
                                    </table>
                                </div>
                                <%}%>
                            <%}%>
                            <% if (myInfoResult.myInfo.ims != null)
                                {
                                    %>
                                <% foreach(var im in myInfoResult.myInfo.ims.im)
                                            {
                                                %>
                                
                                    <div class="table-responsive">
                                    <strong>Contact:</strong><%= myInfoResult.myInfo.contactId%> <strong>IMs</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                        <thead>
                                            <th>type</th>
                                            <th>uri</th>
                                            <th>preferred</th>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td data-value="type">
                                                    <%= im.type %>
                                                </td>
                                                <td data-value="im">
                                                    <%= im.imUri %>
                                                </td>
                                                <td data-value="preferred">
                                                    <%= im.preferred %>
                                                </td>
                                        </tbody>
                                    </table>
                                </div>
                                <%}%>
                            <%}%>
                            <% if (myInfoResult.myInfo.addresses != null)
                                {
                                    %>
                                <% foreach(var address in myInfoResult.myInfo.addresses.address)
                                                {
                                                    %>
                                
                                    <div class="table-responsive">
                                    <strong>Contact:</strong><%= myInfoResult.myInfo.contactId%> <strong>Addresses</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                        <thead>
                                            <tr>
                                                <th>type</th>
                                                <th>po box</th>
                                                <th>address line 1</th>
                                                <th>address line 2</th>
                                                <th>city</th>
                                                <th>state</th>
                                                <th>zipcode</th>
                                                <th>country</th>
                                                <th>preferred</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td data-value="type">
                                                    <%= address.type %>
                                                </td>
                                                <td data-value="po box">
                                                    <%= address.poBox %>
                                                </td>
                                                <td data-value="address line 1">
                                                    <%= address.addressLine1 %>
                                                </td>
                                                <td data-value="address line 2">
                                                    <%= address.addressLine2 %>
                                                </td>
                                                <td data-value="city">
                                                    <%= address.city %>
                                                </td>
                                                <td data-value="state">
                                                    <%= address.state %>
                                                </td>
                                                <td data-value="zipcode">
                                                    <%= address.zipcode %>
                                                </td>
                                                <td data-value="country">
                                                    <%= address.country %>
                                                </td>
                                                <td data-value="preferred">
                                                    <%= address.preferred %>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <% } %>
                            <% } %>
                        
                        <% } %>
                        <% if (this.myinfo_error != null)
                            {%> 
                            <div class="alert alert-danger">
                                <strong>ERROR:</strong>
                                <%= this.myinfo_error %>
                            </div>
                            <% } %>
              </div><!--/.get-my-info-->
              <div role="tabpanel" class="tab-pane" id="update-my-info">
                  <h4>Update My Info:</h4>
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoFirstName">First Name:</label>
                        <asp:TextBox ID="firstNameMyInf" class="form-control" runat="server" name="firstName" placeholder="firstName"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoMiddleName">Middle Name:</label>
                        <input type="text" class="form-control" name="myInfoMiddleName" id="myInfoMiddleName">
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoLastName">Last Name:</label>
                        <asp:TextBox ID="lastNameMyInf" class="form-control" runat="server" name="lastName" placeholder="lastName"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoPrefix">Prefix:</label>
                        <asp:TextBox ID="prefixMyInf" class="form-control" runat="server" name="prefix" placeholder="prefix"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoSuffix">Suffix:</label>
                        <asp:TextBox ID="suffixMyInf" class="form-control" runat="server" name="suffix" placeholder="suffix"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoNickname">Nickname:</label>
                        <asp:TextBox ID="nicknameMyInf" class="form-control" runat="server" name="nickname" placeholder="nickname"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoOrganization">Organization:</label>
                        <asp:TextBox ID="organizationMyInf" class="form-control" runat="server" name="organization" placeholder="organization"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoJobTitle">Job Title:</label>
                        <asp:TextBox ID="jobTitleMyInf" class="form-control" runat="server" name="jobTitle" placeholder="jobTitle"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoAnniversary">Anniversary:</label>
                        <asp:TextBox ID="anniversaryMyInf" class="form-control" runat="server" name="anniversary" placeholder="anniversary"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoGender">Gender:</label>
                        <asp:TextBox ID="genderMyInf" class="form-control" runat="server" name="gender" placeholder="gender"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoSpouse">Spouse:</label>
                        <asp:TextBox ID="spouseMyInf" class="form-control" runat="server" name="spouse" placeholder="spouse"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoChildren">Children:</label>
                        <asp:TextBox ID="childrenMyInf" class="form-control" runat="server" name="children" placeholder="children"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoHobby">Hobby:</label>
                        <asp:TextBox ID="hobbyMyInf" class="form-control" runat="server" name="hobby" placeholder="hobby"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label for="myInfoAssistant">Assistant:</label>
                        <asp:TextBox ID="assistantMyInf" class="form-control" runat="server" name="assistant" placeholder="assistant"></asp:TextBox>
                      </div>
                    </div>
                      <div class="col-md-4">
                        <div class="form-group">
                            <label>Photo:</label>
                                <select name="attachPhotoMyInf" id="attachPhotoMyInf" runat="server" class="form-control">
                                <option value="">None</option>
                                <option value="ATTLogo.jpg">ATTLogo.jpg</option>
                                <option value="Coupon.jpeg">Coupon.jpeg</option>
                                </select>
                        </div>
                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnMyInfoPhones" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnMyInfoPhones" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Phones:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="myInfoPhoneIndex"
                        id="myInfoPhoneIndex">

                      <div id="myInfoPhonePanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoPhonePref0">Preferred</label>
                                <select name="myInfoPhonePref0" id="myInfoPhonePref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoPhoneType0">Type</label>
                                <select name="myInfoPhoneType0" id="myInfoPhoneType0" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoPhoneNumber0">Number</label>
                                <input type="text" class="form-control" name="myInfoPhoneNumber0" id="myInfoPhoneNumber0"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoPhonePanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoPhonePref1">Preferred</label>
                                <select name="myInfoPhonePref1" id="myInfoPhonePref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoPhoneType1">Type</label>
                                <select name="myInfoPhoneType1" id="myInfoPhoneType1" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoPhoneNumber1">Number</label>
                                <input type="text" class="form-control" name="myInfoPhoneNumber1" id="myInfoPhoneNumber1"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoPhonePanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoPhonePref2">Preferred</label>
                                <select name="myInfoPhonePref2" id="myInfoPhonePref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoPhoneType2">Type</label>
                                <select name="myInfoPhoneType2" id="myInfoPhoneType2" class="form-control">
                                  <option>WORK,VOICE</option>
                                  <option>CELL</option>
                                  <option>HOME,CELL</option>
                                  <option>WORK,CELL</option>
                                  <option>VOICE</option>
                                  <option>HOME,VOICE</option>
                                  <option>WORK,VOICE</option>
                                  <option>FAX</option>
                                  <option>HOME,FAX</option>
                                  <option>WORK,FAX</option>
                                  <option>VIDEO</option>
                                  <option>HOME,VIDEO</option>
                                  <option>WORK,VIDEO</option>
                                  <option>PAGER</option>
                                  <option>CAR</option>
                                  <option>OTHER</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoPhoneNumber2">Number</label>
                                <input type="text" class="form-control" name="myInfoPhoneNumber2" id="myInfoPhoneNumber2"
                                  placeholder="2065551299">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->
                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnMyInfoIM" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnMyInfoIM" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        IM:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="myInfoIMIndex"
                        id="myInfoIMIndex">

                      <div id="myInfoIMPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoIMPref0">Preferred</label>
                                <select name="myInfoIMPref0" id="myInfoIMPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoIMType0">Type</label>
                                <select name="myInfoIMType0" id="myInfoIMType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoIMUri0">Uri</label>
                                <input type="text" class="form-control" name="myInfoIMUri0" id="myInfoIMUri0"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoIMPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoIMPref1">Preferred</label>
                                <select name="myInfoIMPref1" id="myInfoIMPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoIMType1">Type</label>
                                <select name="myInfoIMType1" id="myInfoIMType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoIMUri1">Uri</label>
                                <input type="text" class="form-control" name="myInfoIMUri1" id="myInfoIMUri1"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoIMPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoIMPref2">Preferred</label>
                                <select name="myInfoIMPref2" id="myInfoIMPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoIMType2">Type</label>
                                <select name="myInfoIMType2" id="myInfoIMType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>AIM</option>
                                  <option>ICQ</option>
                                  <option>JABBER</option>
                                  <option>MSN</option>
                                  <option>YAHOO</option>
                                  <option>WV</option>
                                  <option>SKYPE</option>
                                  <option>GTALK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoIMUri2">Number</label>
                                <input type="text" class="form-control" name="myInfoIMUri2" id="myInfoIMUri2"
                                  placeholder="ABC">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnMyInfoAddresses" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnMyInfoAddresses" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Addresses:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="myInfoAddressIndex"
                        id="myInfoAddressIndex">

                      <div id="myInfoAddressPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressPref0">Preferred</label>
                                <select name="myInfoAddressPref0" id="myInfoAddressPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressType0">Type</label>
                                <select name="myInfoAddressType0" id="myInfoAddressType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressPoBox0">PO Box</label>
                                <input type="text" class="form-control" name="myInfoAddressPoBox0"
                                id="myInfoAddressPoBox0" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoAddressLineOne0">Address Line One</label>
                                <input type="text" class="form-control" name="myInfoAddressLineOne0"
                                id="myInfoAddressLineOne0" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoAddressLineTwo0">Address Line Two</label>
                                <input type="text" class="form-control" name="myInfoAddressLineTwo0"
                                  id="myInfoAddressLineTwo0" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressCity0">City</label>
                                <input type="text" class="form-control" name="myInfoAddressCity0"
                                  id="myInfoAddressCity0" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressState0">State</label>
                                <input type="text" class="form-control" name="myInfoAddressState0"
                                  id="myInfoAddressState0" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressZip0">Zipcode</label>
                                <input type="text" class="form-control" name="myInfoAddressZip0"
                                  id="myInfoAddressZip0" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressCountry0">Country</label>
                                <input type="text" class="form-control" name="myInfoAddressCountry0"
                                  id="myInfoAddressCountry0" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoAddressPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressPref1">Preferred</label>
                                <select name="myInfoAddressPref1" id="myInfoAddressPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressType1">Type</label>
                                <select name="myInfoAddressType1" id="myInfoAddressType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressPoBox1">PO Box</label>
                                <input type="text" class="form-control" name="myInfoAddressPoBox1"
                                id="myInfoAddressPoBox1" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoAddressLineOne1">Address Line One</label>
                                <input type="text" class="form-control" name="myInfoAddressLineOne1"
                                id="myInfoAddressLineOne1" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoAddressLineTwo1">Address Line Two</label>
                                <input type="text" class="form-control" name="myInfoAddressLineTwo1"
                                  id="myInfoAddressLineTwo1" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressCity1">City</label>
                                <input type="text" class="form-control" name="myInfoAddressCity1"
                                  id="myInfoAddressCity1" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressState1">State</label>
                                <input type="text" class="form-control" name="myInfoAddressState1"
                                  id="myInfoAddressState1" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressZip1">Zipcode</label>
                                <input type="text" class="form-control" name="myInfoAddressZip1"
                                  id="myInfoAddressZip1" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressCountry1">Country</label>
                                <input type="text" class="form-control" name="myInfoAddressCountry1"
                                  id="myInfoAddressCountry1" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoAddressPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressPref2">Preferred</label>
                                <select name="myInfoAddressPref2" id="myInfoAddressPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressType2">Type</label>
                                <select name="myInfoAddressType2" id="myInfoAddressType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-4">
                              <div class="form-group">
                                <label for="myInfoAddressPoBox2">PO Box</label>
                                <input type="text" class="form-control" name="myInfoAddressPoBox2"
                                id="myInfoAddressPoBox2" placeholder="POBOX">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoAddressLineOne2">Address Line One</label>
                                <input type="text" class="form-control" name="myInfoAddressLineOne2"
                                id="myInfoAddressLineOne2" placeholder="Address Line">
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoAddressLineTwo2">Address Line Two</label>
                                <input type="text" class="form-control" name="myInfoAddressLineTwo2"
                                  id="myInfoAddressLineTwo2" placeholder="Address Line">
                              </div>
                            </div>
                          </div><!--/.row-->
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressCity2">City</label>
                                <input type="text" class="form-control" name="myInfoAddressCity2"
                                  id="myInfoAddressCity2" placeholder="City">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressState2">State</label>
                                <input type="text" class="form-control" name="myInfoAddressState2"
                                  id="myInfoAddressState2" placeholder="State">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressZip2">Zipcode</label>
                                <input type="text" class="form-control" name="myInfoAddressZip2"
                                  id="myInfoAddressZip2" placeholder="Zipcode">
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoAddressCountry2">Country</label>
                                <input type="text" class="form-control" name="myInfoAddressCountry2"
                                  id="myInfoAddressCountry2" placeholder="Country">
                              </div>
                            </div>
                          </div><!--/.row-->
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnMyInfoEmails" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnMyInfoEmails" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        Emails:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="myInfoEmailIndex"
                        id="myInfoEmailIndex">

                      <div id="myInfoEmailPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoEmailPref0">Preferred</label>
                                <select name="myInfoEmailPref0" id="myInfoEmailPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoEmailType0">Type</label>
                                <select name="myInfoEmailType0" id="myInfoEmailType0" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoEmailAddress0">Number</label>
                                <input type="text" class="form-control" name="myInfoEmailAddress0" id="myInfoEmailAddress0"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoEmailPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoEmailPref1">Preferred</label>
                                <select name="myInfoEmailPref1" id="myInfoEmailPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoEmailType1">Type</label>
                                <select name="myInfoEmailType1" id="myInfoEmailType1" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoEmailAddress1">Number</label>
                                <input type="text" class="form-control" name="myInfoEmailAddress1" id="myInfoEmailAddress1"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoEmailPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoEmailPref2">Preferred</label>
                                <select name="myInfoEmailPref2" id="myInfoEmailPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoEmailType2">Type</label>
                                <select name="myInfoEmailType2" id="myInfoEmailType2" class="form-control">
                                  <option>INTERNET</option>
                                  <option>WORK</option>
                                  <option>HOME</option>
                                  <option>INTERNETWORK</option>
                                  <option>INTERNETHOME</option>
                                  <option>EMPTY</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoEmailAddress2">Number</label>
                                <input type="text" class="form-control" name="myInfoEmailAddress2" id="myInfoEmailAddress2"
                                  placeholder="someone@example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->

                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <button id="btnMyInfoWeburls" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
                        </button>
                        <button id="btnUnMyInfoWeburls" type="button" class="btn btn-default" aria-label="Left Align">
                          <span class="glyphicon glyphicon-minus" aria-hidden="true"></span>
                        </button>
                        WebURLs:
                      </div>
                      <input value="0" type="text" class="hidden form-control" name="myInfoWeburlIndex"
                        id="myInfoWeburlIndex">

                      <div id="myInfoWeburlPanel0" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoWeburlPref0">Preferred</label>
                                <select name="myInfoWeburlPref0" id="myInfoWeburlPref0" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoWeburlType0">Type</label>
                                <select name="myInfoWeburlType0" id="myInfoWeburlType0" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoWeburl0">url</label>
                                <input type="text" class="form-control" name="myInfoWeburl0" id="myInfoWeburl0"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoWeburlPanel1" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoWeburlPref1">Preferred</label>
                                <select name="myInfoWeburlPref1" id="myInfoWeburlPref1" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoWeburlType1">Type</label>
                                <select name="myInfoWeburlType1" id="myInfoWeburlType1" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoWeburl1">url</label>
                                <input type="text" class="form-control" name="myInfoWeburl1" id="myInfoWeburl1"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                      <div id="myInfoWeburlPanel2" class="hidden panel panel-default">
                        <div class="panel-body">
                          <div class="row">
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoWeburlPref2">Preferred</label>
                                <select name="myInfoWeburlPref2" id="myInfoWeburlPref2" class="form-control">
                                  <option>True</option>
                                  <option>False</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-3">
                              <div class="form-group">
                                <label for="myInfoWeburlType2">Type</label>
                                <select name="myInfoWeburlType2" id="myInfoWeburlType2" class="form-control">
                                  <option>HOME</option>
                                  <option>WORK</option>
                                </select>
                              </div>
                            </div>
                            <div class="col-md-6">
                              <div class="form-group">
                                <label for="myInfoWeburl2">url</label>
                                <input type="text" class="form-control" name="myInfoWeburl2" id="myInfoWeburl2"
                                  placeholder="example.com">
                              </div>
                            </div>
                          </div>
                        </div><!--/.panel-body-->
                      </div><!--./panel-->

                    </div>
                  </div><!--/.row-->
                  <asp:Button ID="btnUpdateMyinfo" data-loading-text="Updating..." class="btn btn-primary" runat="server" Text="Update MyInfo" OnClick="updateMyInfo_Click" />
                <% if (this.update_myinfo !=null)
                            {%> 
                            <div class="alert alert-success">
                                <strong>SUCCESS:</strong>
                                <%= this.update_myinfo.last_modified %>
                            </div>
                            <% } %>
                        <% if (this.myinfo_error != null)
                            {%> 
                            <div class="alert alert-danger">
                                <strong>ERROR:</strong>
                                <%= this.myinfo_error %>
                            </div>
                            <% } %>
              </div><!--/.update-my-info-->
              <div role="tabpanel" class="tab-pane" id="create-group">
                  <h4>Create Group:</h4>
                  <div class="row">
                    <div class="col-md-6">
                      <div class="form-group">
                        <label for="createGroupName">Group Name:</label>
                        <asp:TextBox ID="groupName" class="form-control" runat="server" name="groupName" placeholder="groupName"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-6">
                      <div class="form-group">
                        <label for="createGroupType">Group Type:</label>
                        <asp:TextBox ID="groupType" class="form-control" runat="server" name="groupType" placeholder="USER" readonly="true"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <asp:Button ID="btnCreatGroup" class="btn btn-primary" data-loading-text="Creating..." runat="server" Text="Create Group" OnClick="createGroup_Click" />
                <% if (this.create_group!=null)
                                {%> 
                                <div class="alert alert-success">
                                    <strong>SUCCESS:</strong>
                                    <%= this.create_group.location %>
                                </div>
                                <% } %>
                                <% if (this.success_group != null)
                                {%> 
                                <div class="alert alert-success">
                                    <strong>SUCCESS:</strong>
                                    <%= this.success_group.last_modified %>
                                </div>
                                <% } %>
                                <% if (this.group_error != null)
                                {%> 
                                <div class="alert alert-danger">
                                    <strong>ERROR:</strong>
                                    <%= this.group_error %>
                                </div>
                                <% } %>
              </div><!--/.create-group-->
              <div role="tabpanel" class="tab-pane" id="update-group">
                
                  <h4>Update Group:</h4>
                  <div class="row">
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Group Id:</label>
                        <asp:TextBox ID="groupIdUpd" class="form-control" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Group Name:</label>
                        <asp:TextBox ID="groupNameUpd" class="form-control" runat="server" name="groupName" placeholder="groupName"></asp:TextBox>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="form-group">
                        <label>Group Type:</label>
                        <asp:TextBox ID="groupTypeUpd" class="form-control" runat="server" name="groupType" placeholder="USER" readonly="true"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <asp:Button ID="btnUpdateGrp" class="btn btn-primary" data-loading-text="Updating..." runat="server" Text="Update Group" OnClick="updateGroup_Click" />
                <% if (this.create_group!=null)
                                {%> 
                                <div class="alert alert-success">
                                    <strong>SUCCESS:</strong>
                                    <%= this.create_group.location %>
                                </div>
                                <% } %>
                                <% if (this.success_group != null)
                                {%> 
                                <div class="alert alert-success">
                                    <strong>SUCCESS:</strong>
                                    <%= this.success_group.last_modified %>
                                </div>
                                <% } %>
                                <% if (this.group_error != null)
                                {%> 
                                <div class="alert alert-danger">
                                    <strong>ERROR:</strong>
                                    <%= this.group_error %>
                                </div>
                                <% } %>
              </div><!--/.update-group-->
              <div role="tabpanel" class="tab-pane" id="delete-group">
                
                  <h4>Delete Group:</h4>
                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <label>Group Id:</label>
                        <asp:TextBox ID="groupIdDel" class="form-control" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <asp:Button ID="btnDeleteGroup" class="btn btn-primary" data-loading-text="Deleting..." runat="server" Text="Delete Group" OnClick="deleteGroup_Click" />
                <% if (this.create_group!=null)
                                {%> 
                                <div class="alert alert-success">
                                    <strong>SUCCESS:</strong>
                                    <%= this.create_group.location %>
                                </div>
                                <% } %>
                                <% if (this.success_group != null)
                                {%> 
                                <div class="alert alert-success">
                                    <strong>SUCCESS:</strong>
                                    <%= this.success_group.last_modified %>
                                </div>
                                <% } %>
                                <% if (this.group_error != null)
                                {%> 
                                <div class="alert alert-danger">
                                    <strong>ERROR:</strong>
                                    <%= this.group_error %>
                                </div>
                                <% } %>
              </div><!--/.delete-group-->
              <div role="tabpanel" class="tab-pane" id="get-groups">
                
                  <h4>Get Groups:</h4>
                  <div class="form-group">
                    <label>Group Name:</label>
                    <asp:TextBox ID="getGroupName" class="form-control" runat="server" name="groupName" placeholder="groupName"></asp:TextBox>
                  </div>
                  <div class="form-group">
                    <label>Order:</label>
                    <asp:ListBox ID="order" runat="server" CssClass="form-control">
                                                                <asp:ListItem Value="ASC">Ascending</asp:ListItem>
                                                                <asp:ListItem Value="DESC">Descending</asp:ListItem>
                                                            </asp:ListBox>
                  </div>
                  <asp:Button ID="btnGetGroups" data-loading-text="Searching..." class="btn btn-primary" runat="server" Text="Get Group" OnClick="getGroups_Click" />
                <% if (this.create_group!=null)
                                {%> 
                                <div class="alert alert-success">
                                    <strong>SUCCESS:</strong>
                                    <%= this.create_group.location %>
                                </div>
                                <% } %>
                                <% if (this.success_group != null)
                                {%> 
                                <div class="alert alert-success">
                                    <strong>SUCCESS:</strong>
                                    <%= this.success_group.last_modified %>
                                </div>
                                <% } %>
                                <% if (this.group_error != null)
                                {%> 
                                <div class="alert alert-danger">
                                    <strong>ERROR:</strong>
                                    <%= this.group_error %>
                                </div>
                                <% } %>
                        
                        
                                <% if (this.groupResult != null && this.groupResult.resultSet.totalRecords != "0")
                                    {%>
                                    <div class="alert alert-success">
                                        <strong>SUCCESS:</strong>
                                    </div>
                                    <div class="table-responsive">
                                    <strong>Groups:</strong>
                                 <table class="table table-condensed table-striped table-bordered">
                                        <thead>
                                            <th>groupId</th>
                                            <th>groupName</th>
                                            <th>groupType</th>
                                        </thead>
                                        <tbody>
                                            <%  foreach (var group in groupResult.resultSet.groups.group)
                                                {%>
                                            <tr>
                                                <td data-value="groupId">
                                                    <%= group.groupId %>
                                                </td>
                                                <td data-value="groupName">
                                                    <%= group.groupName %>
                                                </td>
                                                <td data-value="groupType">
                                                    <%= group.groupType %>
                                                </td>
                                            </tr>
                                            <% } %>
                                        </tbody>
                                    </table>
                                    </div>
                                <% } %>
              </div><!--/.delete-group-->
              <div role="tabpanel" class="tab-pane" id="get-group-contacts">
                
                  <h4>Get Group Contacts:</h4>
                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <label for="getContactsGroupId">Group Id:</label>
                        <asp:TextBox ID="groupIdContacts" class="form-control" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <asp:Button ID="groupIdContactsBtn" data-loading-text="Getting..." class="btn btn-primary" runat="server" Text="Get Group Contacts" OnClick="groupIdContacts_Click" />
                <% if (contactIdResult != null)
                                        {%>
                                        <div class="alert alert-success">
                                            <strong>SUCCESS:</strong>
                                        </div>
                                        <div class="table-responsive">
                                         <strong>Contacts:</strong>
                                         <table class="table table-condensed table-striped table-bordered">
                                            <thead>
                                                <th>Contact Id</th>
                                            </thead>
                                            <tbody>
                                                <% foreach (var id in contactIdResult.contactIds.id)
                                                    { %>
                                                <tr>
                                                    <td data-value="Contact Id">
                                                        <%= id %>
                                                    </td>
                                                </tr>
                                                <% } %>
                                            </tbody>
                                        </table>
                                        </div>
                                        <% } %>
                                        <% if (this.manage_groups_error != null)
                                            { %>
                                        <div class="alert alert-danger">
                                            <strong>Error</strong><br />
                                            <%= this.manage_groups_error %>
                                        </div>
                                        <% } %>
              </div><!--/.get-group-contacts-->
              <div role="tabpanel" class="tab-pane" id="add-contacts-to-group">
                
                  <h4>Add Contacts to Group:</h4>
                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <label for="addContactsGroupId">Group Id:</label>
                        <asp:TextBox ID="groupIdAddDel" class="form-control" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                      </div>
                      <div class="form-group">
                        <label for="addContactIds">Contact Ids:</label>
                          <asp:TextBox ID="addContactsGrp" class="form-control" runat="server" name="contactIds" data-toggle="tooltip" data-placement="bottom"
                          data-title="More than one contact id may be seperated by a comma (,)" placeholder="id1,id2"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <asp:Button ID="btnGroupIdContactsAdd" data-loading-text="Adding..." class="btn btn-primary" runat="server" Text="Add Contacts to Group" OnClick="addContctsToGroup_Click" />
                <% if (contactIdResult != null)
                                        {%>
                                        <div class="alert alert-success">
                                            <strong>SUCCESS:</strong>
                                        </div>
                                        
                                        <% } %>
                                        <% if (this.manage_groups != null)
                                            {%>
                                            <div class="alert alert-success">
                                                <strong>SUCCESS:</strong>
                                                <%= this.manage_groups.last_modified %>
                                            </div>
                                        <% } %>
                                        <% if (this.manage_groups_error != null)
                                            { %>
                                        <div class="alert alert-danger">
                                            <strong>Error</strong><br />
                                            <%= this.manage_groups_error %>
                                        </div>
                                        <% } %>
              </div><!--/.add-contacts-to-group-->
              <div role="tabpanel" class="tab-pane" id="remove-contacts-from-group">
                
                  <h4>Remove Contacts from Group:</h4>
                  <div class="row">
                    <div class="col-md-12">
                      <div class="form-group">
                        <label for="removeContactsGroupId">Group Id:</label>
                        <asp:TextBox ID="groupIdRemDel" class="form-control" runat="server" name="groupId" placeholder="groupId"></asp:TextBox>
                      </div>
                      <div class="form-group">
                        <label for="removeContactIds">Contact Ids:</label>
                          <asp:TextBox ID="remContactsGrp" class="form-control" runat="server" name="contactIds" data-toggle="tooltip" data-placement="bottom"
                          data-title="More than one contact id may be seperated by a comma (,)" placeholder="id1,id2"></asp:TextBox>
                      </div>
                    </div>
                  </div><!--/.row-->
                  <asp:Button ID="btnGroupIdContactsRem" data-loading-text="Removing..." class="btn btn-primary" runat="server" Text="Remove Contacts from Group" OnClick="removeContctsFromGroup_Click" />
                <% if (contactIdResult != null)
                                        {%>
                                        <div class="alert alert-success">
                                            <strong>SUCCESS:</strong>
                                        </div>
                                        <% } %>
                                        <% if (this.manage_groups != null)
                                            {%>
                                            <div class="alert alert-success">
                                                <strong>SUCCESS:</strong>
                                                <%= this.manage_groups.last_modified %>
                                            </div>
                                        <% } %>
                                        <% if (this.manage_groups_error != null)
                                            { %>
                                        <div class="alert alert-danger">
                                            <strong>Error</strong><br />
                                            <%= this.manage_groups_error %>
                                        </div>
                                        <% } %>
              </div><!--/.remove-contacts-from-group-->
              <div role="tabpanel" class="tab-pane" id="get-contact-groups">
                
                  <h4>Get Contact Groups:</h4>
                  <div class="form-group">
                    <label for="getGroupsContactId">Contact Id</label>
                    <asp:TextBox ID="contactsIdGroups" class="form-control" runat="server" name="contactId" placeholder="contactId"></asp:TextBox>
                  </div>
                  <asp:Button ID="btnContactsIdGroups" data-loading-text="Getting..." class="btn btn-primary" runat="server" Text="Get Contact Groups" OnClick="getContactGroups_Click" />
                             <% if (this.contactGroupResult != null)
                                            {%>
                                        <div class="alert alert-success">
                                            <strong>SUCCESS:</strong>
                                        </div>
                                        <div class="table-responsive">
                                         <strong>Groups:</strong>
                                         <table class="table table-condensed table-striped table-bordered">
                                            <thead>
                                                <th>groupId</th>
                                                <th>groupName</th>
                                                <th>groupType</th>
                                            </thead>
                                            <tbody>
                                                <%  foreach (var group in contactGroupResult.resultSet.groups.group)
                                                    {%>
                                                <tr>
                                                    <td data-value="groupId">
                                                        <%= group.groupId %>
                                                    </td>
                                                    <td data-value="groupName">
                                                        <%= group.groupName %>
                                                    </td>
                                                    <td data-value="groupType">
                                                        <%= group.groupType %>
                                                    </td>
                                                </tr>
                                                <% } %>
                                            </tbody>
                                        </table>
                                        </div>
                                        <% } %>
                                        <% if (this.manage_groups != null)
                                            {%>
                                            <div class="alert alert-success">
                                                <strong>SUCCESS:</strong>
                                                <%= this.manage_groups.last_modified %>
                                            </div>
                                        <% } %>
                                        <% if (this.manage_groups_error != null)
                                            { %>
                                        <div class="alert alert-danger">
                                            <strong>Error</strong><br />
                                            <%= this.manage_groups_error %>
                                        </div>
                                        <% } %>
              </div><!--/.get-contact-groups-->
            </div>
          </div>
        </div><!--./col-md-12-->
      </div><!--./row-->
      <div class="row">
        <div class="col-md-12">
          <div class="hidden" id="response"></div>
        </div><!--./col-md-12-->
      </div><!--./row-->
      <hr>
      <div class="row"><div class="col-md-12"><b>Server Time:&nbsp;</b><span id="serverTime"></span><%= String.Format("{0:ddd, MMMM dd, yyyy HH:mm:ss}", DateTime.UtcNow) + " UTC" %></div></div>
      <div class="row"><div class="col-md-12"><b>Client Time:</b> <script>                                                                      document.write("" + new Date());</script></div></div>
      <div class="row"><div class="col-md-12"><b>User Agent:</b> <script>                                                                     document.write("" + navigator.userAgent);</script></div></div>
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
    <script>        $(function () { $('[data-toggle="tooltip"]').tooltip() });</script>

        <script>
            var addEntry = function (indexId, panelId) {
                var indexValue = $(indexId).val();
                var id = panelId + indexValue++;
                var elem = $(id);
                if (elem.length) {
                    elem.removeClass('hidden');
                    $(indexId).val(indexValue);
                } else {
                    alert("Exceeded max allowed");
                }
            };

            var removeEntry = function (indexId, panelId) {
                var indexValue = $(indexId).val();
                var id = panelId + (--indexValue);
                var elem = $(id);
                if (elem.length) {
                    elem.addClass('hidden');
                    $(indexId).val(indexValue);
                }
            };

            var entryMappings = {
                /* Create Contact Mappings */
                '#createPhoneIndex': {
                    'addButton': '#btnCreatePhones',
                    'removeButton': '#btnUnCreatePhones',
                    'panel': '#createPhonePanel',
                },
                '#createIMIndex': {
                    'addButton': '#btnCreateIM',
                    'removeButton': '#btnUnCreateIM',
                    'panel': '#createIMPanel',
                },
                '#createAddressIndex': {
                    'addButton': '#btnCreateAddresses',
                    'removeButton': '#btnUnCreateAddresses',
                    'panel': '#createAddressPanel',
                },
                '#createEmailIndex': {
                    'addButton': '#btnCreateEmails',
                    'removeButton': '#btnUnCreateEmails',
                    'panel': '#createEmailPanel',
                },
                '#createWeburlIndex': {
                    'addButton': '#btnCreateWeburls',
                    'removeButton': '#btnUnCreateWeburls',
                    'panel': '#createWeburlPanel',
                },

                /* Update Contact Mappings */
                '#updatePhoneIndex': {
                    'addButton': '#btnUpdatePhones',
                    'removeButton': '#btnUnUpdatePhones',
                    'panel': '#updatePhonePanel',
                },
                '#updateIMIndex': {
                    'addButton': '#btnUpdateIM',
                    'removeButton': '#btnUnUpdateIM',
                    'panel': '#updateIMPanel',
                },
                '#updateAddressIndex': {
                    'addButton': '#btnUpdateAddresses',
                    'removeButton': '#btnUnUpdateAddresses',
                    'panel': '#updateAddressPanel',
                },
                '#updateEmailIndex': {
                    'addButton': '#btnUpdateEmails',
                    'removeButton': '#btnUnUpdateEmails',
                    'panel': '#updateEmailPanel',
                },
                '#updateWeburlIndex': {
                    'addButton': '#btnUpdateWeburls',
                    'removeButton': '#btnUnUpdateWeburls',
                    'panel': '#updateWeburlPanel',
                },

                /* Update MyInfo Mappings */
                '#myInfoPhoneIndex': {
                    'addButton': '#btnMyInfoPhones',
                    'removeButton': '#btnUnMyInfoPhones',
                    'panel': '#myInfoPhonePanel',
                },
                '#myInfoIMIndex': {
                    'addButton': '#btnMyInfoIM',
                    'removeButton': '#btnUnMyInfoIM',
                    'panel': '#myInfoIMPanel',
                },
                '#myInfoAddressIndex': {
                    'addButton': '#btnMyInfoAddresses',
                    'removeButton': '#btnUnMyInfoAddresses',
                    'panel': '#myInfoAddressPanel',
                },
                '#myInfoEmailIndex': {
                    'addButton': '#btnMyInfoEmails',
                    'removeButton': '#btnUnMyInfoEmails',
                    'panel': '#myInfoEmailPanel',
                },
                '#myInfoWeburlIndex': {
                    'addButton': '#btnMyInfoWeburls',
                    'removeButton': '#btnUnMyInfoWeburls',
                    'panel': '#myInfoWeburlPanel',
                },
            };

            var handleMapping = function (key, entry) {
                var indexValue = $(key).val();
                $(key).val(0);
                for (var i = 0; i < indexValue; ++i) {
                    addEntry(key, entry['panel']);
                }

                $(entry['addButton']).click(function (evt) {
                    evt.preventDefault();
                    addEntry(key, entry['panel']);
                });
                $(entry['removeButton']).click(function (evt) {
                    evt.preventDefault();
                    removeEntry(key, entry['panel']);
                });
            };

            for (var key in entryMappings) {
                var entry = entryMappings[key];
                handleMapping(key, entry);
            }

        </script>
 <% if (!string.IsNullOrEmpty(showCreateContact))
           { %>
        <script type="text/javascript">   $('#tabs a[href="#create-contact"]').tab('show');</script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showUpdateContact))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#update-contact"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showDeleteContact))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#delete-contact"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetContacts))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-contacts"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetMyInfo))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-my-info"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showUpdateMyInfo))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#update-my-info"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showCreateGroup))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#create-group"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showUpdateGroup))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#update-group"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showDeleteGroup))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#delete-group"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetGroups))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-groups"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetGroupContacts))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-group-contacts"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showAddContactsToGroup))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#add-contacts-to-group"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showRemoveContactsfrmGroup))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#remove-contacts-from-group"]').tab('show'); </script>
        <% } %>
        <% if (!string.IsNullOrEmpty(showGetContactGroups))
           { %>
        <script type="text/javascript">        $('#tabs a[href="#get-contact-groups"]').tab('show'); </script>
        <% } %>
        
    </form>
</body>
</html>
