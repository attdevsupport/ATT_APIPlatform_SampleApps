//
// AT&T Call Management Services Sample Script
//
// Language: JavaScript
//


var welcomeMessage = "Welcome to the A T and T Call Managment Services Sample Application.";
var goodbyeMessage = "Thank you, for using A T and T. Call Management Service Sample Application Demo.  Good Bye";
var holdMusic = "";

if (typeof numberToDial === 'undefined') {

    //
    // Obtain incoming callerId (if number is not blocked), strip out leading characters
    // to result in 10 digit inbound number.
    //
    var callerID = currentCall.callerID.replace("+1", "");

    if (callerID == '2044884744') {
        reject();
    }

    say(welcomeMessage);

    ask("Press a digit to listen what you pressed or Press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout: 5,
        onChoice: function (event) {
            say("Thank you for entering  " + event.value);
        },
        onBadChoice: function (event) {
            say("I am sorry, not able to get the digit");
        }
    });

    ask("Press a digit to join 1337 conferencing or Press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout: 5,
        onChoice: function (event) {
            say("Thank you for joining conferenceing, to quit press star at any time");
            conference("1337", {
                terminator: "*",
                playTones: true,
                onChoice: function (event) {
                    say("Disconnecting from conference");
                }
            });
        }
    });

    ask("Press a digit to log the header value or Press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout: 5,
        onChoice: function (event) {
            if (currentCall.getHeader("to")) {
                say("Logged to header value");
                log("Your header value is " + currentCall.getHeader("to"));
            }
            else {
                say("could not find to header value");
                log("Your header value was not found");
            }
        }
    });

    ask("Enter your 10 digit phone number to test the reject feature or press pound to skip.", {
        choices: "[10 DIGITS]",
        terminator: "#",
        timeout: 120.0,
        mode: "dtmf",
        interdigitTimeout: 10,
        onChoice: function (event) {
            var numbertest = event.value;
            say("You entered " + numbertest);
            if (callerID == numbertest) {
                say("Number entered matches. Rejecting your call now.");
                say(goodbyeMessage);
                hangup();
            }
            else {
                say("Numbers do not match. You are calling from ");
                say("<speak><say-as interpret-as = 'vxml:digits'>" + callerID + "</say-as></speak>");
                say("and the number you entered is ");
                say("<speak><say-as interpret-as = 'vxml:digits'>" + numbertest + "</say-as></speak>");
            }
        }
    });

    ask("Press a ten digit phone number to send sms to the requested number or Press pound to skip", {
        choices: "[10 DIGITS]",
        terminator: "#",
        timeout: 120.0,
        mode: "dtmf",
        interdigitTimeout: 10,
        onChoice: function (event) {
            var numbertest = event.value;
            say("Sending message to ");
            say("<speak><say-as interpret-as = 'vxml:digits'>" + numbertest + "</say-as></speak>");
            message("Message from AT&T Call Control Service Sample Application", {
                to: numbertest,
                network: "SMS"
            });
        }
    });


    ask("Enter a 10 digit phone number to transfer the call to or press pound to skip", {
        choices: "[10 DIGITS]",
        terminator: "#",
        timeout: 120.0,
        mode: "dtmf",
        interdigitTimeout: 10,
        onChoice: function (event) {
            var numbertest = event.value;
            say("Transferring you to ");
            say("<speak><say-as interpret-as = 'vxml:digits'>" + numbertest + "</say-as></speak>");
            transfer(numbertest, {
                playvalue: holdMusic,
                terminator: "*",
                onTimeout: function (event) {
                    say("I'm sorry, but nobody answered");
                }
            });
        }

    });

    //	say("Your event name is " + result25.name); badChoice

    ask("Press  ten digit phone number for which you are calling to waited this call or Press pound to skip", {
        choices: "[10 DIGITS]",
        terminator: "#",
        timeout: 120.0,
        mode: "dtmf",
        interdigitTimeout: 10,
        onChoice: function (event) {
            var numbertest = event.value;

            if (callerID == numbertest) {
                say("entered number matched with the current call caller id");
                say("your calls will be kept for three seconds waited");
                wait(3000);
            }
            else {
                say("number not matched for waited feature");
                say(" the current call id is");
                say(callerID);
                say("and you entered is ");
                say(numbertest);
            }
        }
    });

    ask("Press a digit to test the signalling or press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout: 5,
        onChoice: function (event) {
            say("Waiting for exit signal");
            say(messageToPlay, {
                allowSignals: "exit",
                onSignal: function (event) {
                    say("Received exit signal, hence music is paused. Enjoy the music again.");
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
        }
    });
}
else {

    call(numberToDial);
    say(welcomeMessage);

    //
    // As this is an OUTBOUND call, caller ID is not available and must be manually
    // set to the number we are dialing.
    //
    var callerID = numberToDial; // currentCall.callerID.replace("+1", "");
    say("I am calling " + callerID);

    if (typeof feature !== 'undefined') {

        switch (feature) {
            case 'ask':
                var result9 = ask("Press four or five digits and press pound when finished", {
                    choices: "[4-5 DIGITS]",
                    terminator: "#",
                    timeout: 90.0,
                    mode: "dtmf",
                    interdigitTimeout: 5,
                    onChoice: function (event) {
                        say("Thank you for entering " + event.value);
                    },
                    onBadChoice: function (event) {
                        say("I am sorry, not able to get the four or five digits");
                    }
                });
                break;
            case 'conference':
                say("Thank you for joining conference 1337.");
                say("To quit press star key at any time");
                conference("1337", {
                    terminator: "*",
                    playTones: true,
                    onChoice: function (event) {
                        say("Disconnecting from conference");
                    }
                });
                break;
            case 'reject':
                if (typeof featurenumber === 'undefined') {
                    say("Feature number is not provided as part of create session A P I request");
                }
                else {
                    if (callerID == featurenumber) {
                        say("your calls will be rejected");
                        say("Thank you, for using A T and T Call management Sample Application Demo.  Good Bye");
                        reject();
                    }
                    else {
                        say("Feature number provided is ");
                        say("<speak><say-as interpret-as = 'vxml:digits'>" + featurenumber + "</say-as></speak>");
                        say("Numbers do not match. Reject feature test skipped.");
                    }
                }
                break;
            case 'transfer':
                if (typeof featurenumber === 'undefined') {
                    say("Feature number is not provided as part of create session A P I request");
                }
                else {
                    say("Transferring call to ");
                    say("<speak><say-as interpret-as = 'vxml:digits'>" + featurenumber + "</say-as></speak>");
                    transfer([featurenumber, "sip:12345678912@221.122.54.86"], {
                        playvalue: messageToPlay,
                        terminator: "*",
                        onTimeout: function (event) {
                            say("Sorry, but nobody answered");
                        }
                    });
                }
                break;
            case 'wait':
                if (typeof featurenumber === 'undefined') {
                    say("Feature number is not provided as part of create session A P I request");
                }
                else {
                    if (callerID == featurenumber) {
                        say("Your call will be placed in a waiting state for 3 seconds.");
                        wait(3000);
                    }
                    else {
                        say("Feature number provided is ");
                        say("<speak><say-as interpret-as = 'vxml:digits'>" + featurenumber + "</say-as></speak>");
                        say("Numbers do not match. Wait feature test skipped.");
                    }
                }
                break;
            case 'message':
                if (typeof featurenumber === 'undefined') {
                    say("Feature number is not provided as part of create session A P I request");
                }
                else {
                    say("Sending message to");
                    say("<speak><say-as interpret-as = 'vxml:digits'>" + featurenumber + "</say-as></speak>");
                    message("Message from AT&T Call Control Service Sample Application", {
                        to: featurenumber,
                        network: "SMS"
                    });
                }
                break;
        }
    }

    if (typeof messageToPlay === 'undefined') {
        say("The MessageToPlay parameter was not provided. Using default music file.");
        var messageToPlay = holdMusic;
    }

    ask("Press a digit to test the signaling or press pound to skip", {
        choices: "[1 DIGITS]",
        terminator: "#",
        timeout: 5.0,
        mode: "dtmf",
        interdigitTimeout: 5,
        onChoice: function (event) {

            say("Waiting for exit signal");
            say(messageToPlay, {
                allowSignals: "exit",
                onSignal: function (event) {
                    say("Exit signal received...");
                }
            });

            say("Waiting for stopHold signal");
            say(messageToPlay, {
                allowSignals: "stopHold",
                onSignal: function (event) {
                    say("Stop, Hold Signal received...");
                }
            });

            say("Waiting for dequeue signal");
            say(messageToPlay, {
                allowSignals: "dequeue",
                onSignal: function (event) {
                    say("Dee Queue Signal received.");
                }
            });
        }
    });
}

say(goodbyeMessage);
hangup();
