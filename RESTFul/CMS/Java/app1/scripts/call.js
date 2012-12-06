
if (typeof numberToDial === 'undefined')
{
	if (typeof messageToPlay == 'undefined')
		var messageToPlay = "http://wdev.code-api-att.com:8181/Tropo/music.mp3";
	
    say("Welcome to the A T and T. Call Management Services Sample Application.");

    var result20 = ask("Press a digit to listen what you pressed or Press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout: 5,
        onChoice: function (event) {
            say("Thank you for entering");
        },
        onBadChoice: function (event) {
            say("I am sorry, not able to get the digit");
        }
    });
    if (result20.value != "")
    {
        say (result20.value);
    }

    var result21 = ask("Press a digit to join 1337 conferencing or Press pound to skip", {
    choices: "[1 DIGITS]",
    terminator: "#",
    timeout: 5.0,
    mode: "dtmf",
    interdigitTimeout:5,
    onChoice: function (event) {
        say ("Thank you for joining conferenceing, to quit press star at any time");
        conference("1337", {
            terminator: "*",
            playTones: true,
            onChoice: function (event) {
                say("Disconnecting from conference");
                }
            });
        },
    });

    var result22 = ask("Press a digit to log the header value or Press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout:5,
        onChoice: function (event) {
            if (currentCall.getHeader("to")) {
                say("Logged to header value");
                log("Your header value is " + currentCall.getHeader("to"));
            }
            else {
                say("could not find to header value");
                log("Your header value was not found");
            }
        },
    });

    var result23 = ask("Press a ten digit phone number to send sms to the requested number or Press pound to skip", {
        choices: "[10 DIGITS]",
        terminator: "#",
        timeout: 120.0,
        mode: "dtmf",
        interdigitTimeout:10,
        onChoice: function (event) {
            say("Sending message to");
        },
    });

    if (result23.value != "")
    {
        var numbertest = result23.value;
        say(numbertest);
        message("Message from AT&T Call Control Service Sample Application", {
            to: numbertest,
            network: "SMS"
        });
    }

    var result24 = ask("Press  ten digit phone number for which you are calling to reject this call or Press pound to skip", {
        choices: "[10 DIGITS]",
        terminator: "#",
        timeout: 120.0,
        mode: "dtmf",
        interdigitTimeout:10,
        onChoice: function (event) {
        },
    });
    if ( result24.value != "")
    {
        var numbertest = result24.value;
        if (callerID == numbertest) {
            say("your calls will be rejected");
            say("Thank you, for using A T and T Call management Sample Application Demo.  Good Bye");
            reject();
        }
        else
        {
            say("number not matched for reject feature");
            say(" the current call id is");
            say(currentCall.callerID);
            say( "and you entered is ");
            say(numbertest);
        }
    }


    var result25 = ask("enter 10 digit phone number to transfer the call or Press pound to skip", {
        choices: "[10 DIGITS]",
        terminator: "#",
        timeout: 120.0,
        mode: "dtmf",
        interdigitTimeout:10,
        onChoice: function (event) {
        say("transfering to ");
        },
    });
    if (result25.value != "")
    {
        var numbertest = result25.value;
        say(numbertest);
        transfer([numbertest, "sip:12345678912@221.122.54.86"], {
            playvalue: messageToPlay,
            terminator: "*",
            onTimeout: function (event) {
                say("Sorry, but nobody answered");
            }
        });
     }

    var result27 = ask("Press  ten digit phone number for which you are calling to wait this call or Press pound to skip", {
    choices: "[10 DIGITS]",
    terminator: "#",
    timeout: 120.0,
    mode: "dtmf",
    interdigitTimeout:10,
    onChoice: function (event) {
        },
    });
    if (result27.value != "")
    {
        var numbertest = result27.value
        var callerID = currentCall.callerID;
        if (callerID == numbertest) {
                say("entered number matched with the current call caller id");
                say("your calls will be kept for three seconds wait");
            wait(3000);
        }
        else
        {
            say("number not matched for wait feature");
            say(" the current call id is");
            say(currentCall.callerID);
            say( "and you entered is ");
            say(numbertest);
        }
    }
    var result29 = ask("Press a digit to test the signalling or Press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout:5,
        onChoice: function (event) {
        say("Waiting for exit signal");
        say(messageToPlay, {
            allowSignals: "exit",
            onSignal: function (event) {
                say("Received exit signal, hence music is paused.  Enjoy the music again.");
            }
        });
        say("Waiting for stopHold signal");
        say(messageToPlay, {
        allowSignals: "stopHold",
        onSignal: function (event) {
            say("Received stop hold signal, hence music is paused. Enjoy the music again.");
            }
        });
        say("Waiting for dequeue signal");
        say(messageToPlay, {
        allowSignals: "dequeue",
        onSignal: function (event) {
            say("Received dequeue signal, hence music is stopped.");
            }
        });
        },
    });

    say("Thank you, for using A T and T. Call Management Service Sample Application Demo.  Good Bye"); 
    hangup();
}
else
{
    call (numberToDial);
    say("Welcome to the A T and T. Call Management Services Sample Application.");
	switch(feature)
	{
	case 'answer':
        say("thank you for using answering script function");
		break;
	case 'ask':
        var result9 = ask("Press four or five digits and Press pound when finished", {
            choices: "[4-5 DIGITS]",
            terminator: "#",
            timeout: 90.0,
            mode: "dtmf",
            interdigitTimeout: 5,
            onChoice: function (event) {
                say("Thank you for entering");
            },
            onBadChoice: function (event) {
                say("I am sorry, not able to get the four or five digits");
            }
        });
        if (result9.value != "")
        {
            say( result9.value);
        }
        break;
        case 'message':
          if ( typeof featurenumber === 'undefined') {
              say("Feature number is not provided as part of create session A P I request");
          } else {
              say("Sending message to");
              say("<speak><say-as interpret-as = 'vxml:digits'>" + featurenumber + "</say-as></speak>");
                  message("Message from AT&T Call Control Service Sample Application", {
                            to: featurenumber,
                            network: "SMS"
                        });
          }
          break;
	case 'call':
        say("thank you for using calling script function");
		break;
	case 'conference':
        say ("Thank you for joining conferenceing");
        say ("1337");
        say("and to quit press star at any time");
        conference("1337", {
            terminator: "*",
            playTones: true,
            onChoice: function (event) {
                say("Disconnecting from conference");
            }
        });
        break;
	case 'getHeader':
        if (currentCall.getHeader("to")) {
            say("Logged to header value");
            log("Your header value is " + currentCall.getHeader("to"));
        }
        else {
            say("could not find to header value");
            log("Your header value was not found");
        }
        break;
	case 'hangup':
		say("thank you for using hang script function");
		break;
	case 'log':
        if (currentCall.getHeader("to")) {
            say("Logged to header value");
            log("Your header value is " + currentCall.getHeader("to"));
        }
        else {
            say("could not find to header value");
            log("Your header value was not found");
        }
        break;
	case 'reject':
        var callerID = currentCall.callerID;
        if (callerID == featurenumber) {
            say("your calls will be rejected");
            say("Thank you, for using A T and T Call management Sample Application Demo.  Good Bye");
            reject();
        }
        else
        {
            say("present id is");
            say(currentCall.callerID);
            say("rejectnumber is ")
            say(featurenumber);
            say("number not matched for reject feature");
        }
        break;
	case 'say':
		say("thank you for using saying script function");
		break;
	case 'transfer':
        say("transfering to ");
        say(featurenumber);
        transfer([featurenumber, "sip:12345678912@221.122.54.86"], {
            playvalue: messageToPlay,
            terminator: "*",
            onTimeout: function (event) {
                say("Sorry, but nobody answered");
            }
        });
        break;
	case 'wait':
        var callerID = currentCall.callerID;
        if (callerID == featurenumber) {
            say("your calls will be kept for three seconds wait");
            wait(3000);
        }
        else{
        say("present id is");
        say(currentCall.callerID);
        say("requested id is");
        say(featurenumber);
        say("number not matched for wait feature");
        }
        break;
	}

    var result4 = ask("Press a digit to test the signalling or Press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout:5,
        onChoice: function (event) {
        say("Waiting for exit signal");
        say(messageToPlay, {
            allowSignals: "exit",
            onSignal: function (event) {
                say("Received exit signal, hence music is paused.  Enjoy the music again.");
            }
        });
        say("Waiting for stopHold signal");
        say(messageToPlay, {
        allowSignals: "stopHold",
        onSignal: function (event) {
            say("Received stop hold signal, hence music is paused. Enjoy the music again.");
            }
        });
        say("Waiting for dequeue signal");
        say(messageToPlay, {
        allowSignals: "dequeue",
        onSignal: function (event) {
            say("Received dequeue signal, hence music is stopped.");
            }
        });
        },
    });

    say("Thank you, for using A T and T. Call Management Service Sample Application Demo.  Good Bye"); 
    hangup();
}

