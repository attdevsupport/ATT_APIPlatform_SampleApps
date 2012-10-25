if numberToDial == "undefined"

    messageToPlay = "http://wdev.code-api-att.com:8181/Tropo/music.mp3"
    say "Welcome to the A T and T. Call Management Services Sample Application."
	
	result20 = ask "Press a digit to listen what you pressed or Press pound to skip", 
	{
		:choices => "[1 DIGITS]",
		:terminator => '#',
		:timeout => 5.0,
		:mode => "dtmf",
		:interdigitTimeout => 5,
		:onChoice => lambda { |event|
			say "Thank you for entering"
		},
		:onBadChoice => lambda { |event|
			say "I am sorry, not able to get the digit"
		}
	}
	
	if result20 != ""
	{
		say result20
	}
	
	result21 = ask "Press a digit to join 1337 conferencing or Press pound to skip", 
	{
		:choices => "[1 DIGITS]",
		:terminator => '#',
		:timeout => 5.0,
		:mode => "dtmf",
		:interdigitTimeout => 5,
		:onChoice => lambda { |event|
			say "Thank you for joining conferenceing, to quit press star at any time"
		}
		conference "1337", 
		{
			:terminator => "*",
			:playTones => true,
			:onChoice => lambda { |event|
				say "Disconnecting from conference"
		}
	}
	
	result22 = ask "Press a digit to log the header value or Press pound to skip", 
	{
        :choices => "[1 DIGITS]",
		:terminator => '#',
		:timeout => 5.0,
		:mode => "dtmf",
		:interdigitTimeout => 5,
        	:onChoice => lambda { |event|
			if $currentCall.getHeader("to")
				say "Logged to header value"
				log "Your header value is " + currentCall.getHeader("to")
			else
				say "Could not find to header value"
				log "Your header value was not found"
			end
        }
	}
	
	result23 = ask "Press a ten digit phone number to send sms to the requested number or Press pound to skip", 
	{
        	:choices => "[1 DIGITS]",
		:terminator => '#',
		:timeout => 15.0,
		:mode => "dtmf",
		:interdigitTimeout => 5,
        	:onChoice => lambda { |event|
            	say "Sending message to"
        }
    }

    if result23 != ""
    {
        numbertest = result23.value
        say numbertest
        message "Message from AT&T Call Control Service Sample Application", 
		{
            :to => numbertest,
            :network => "SMS"
        }
    }
    
	result24 = ask "Press  ten digit phone number for which you are calling to reject this call or Press pound to skip", 
	{
        :choices => "[1 DIGITS]",
		:terminator => '#',
		:timeout => 120.0,
		:mode => "dtmf",
		:interdigitTimeout => 5,
        :onChoice => lambda { |event|
        }
    }
	
    if result24 != ""
    {
        numbertest = result24
        if callerID == numbertest
		{
            say "your calls will be rejected"
            say "Thank you, for using A T and T Call management Sample Application Demo.  Good Bye"
            reject
        }
        else
        {
            say "number not matched for reject feature"
            say "the current call id is"
            say $currentCall.callerID
            say "and you entered is "
            say numbertest
        }
    }
	
	result25 = ask "enter 10 digit phone number to transfer the call or Press pound to skip",
	{
        :choices => "[10 DIGITS]",
		:terminator => '#',
		:timeout => 120.0,
		:mode => "dtmf",
		:interdigitTimeout => 5,
        :onChoice => lambda { |event|
			say "transfering to "
		}
    }
	
    if result25 != ""
    {
        numbertest = result25
        say numbertest
        transfer [numbertest, "sip:12345678912@221.122.54.86"], {
            :playvalue => messageToPlay,
			:terminator => '*',
			:onTimeout => lambda { |event|
				say "Sorry, but nobody answered"
			}
	}
	
	result27 = ask "Press  ten digit phone number for which you are calling to wait this call or Press pound to skip", 
	{
		:choices => "[10 DIGITS]",
		:terminator => '#',
		:timeout => 120.0,
		:mode => "dtmf",
		:interdigitTimeout => 5,
        :onChoice => lambda { |event|
		}
		
    if result27 != ""
    {
        numbertest = result27
        callerID = $currentCall.callerID
        if callerID == numbertest 
		{
            say "entered number matched with the current call caller id"
            say "your calls will be kept for three seconds wait"
            wait(3000)
        }
        else
        {
            say "number not matched for wait feature"
            say " the current call id is"
            say $currentCall.callerID
            say "and you entered is "
            say numbertest
        }
    }
	
	result29 = ask "Press a digit to test the signalling or Press pound to skip", 
	{
        :choices => "[1 DIGITS]",
		:terminator => '#',
		:timeout => 5.0,
		:mode => "dtmf",
		:interdigitTimeout => 5,
			:onChoice => lambda { |event|
				say "Waiting for exit signal"
				say messageToPlay,
				{
					:allowSignals => "exit",
					:onSignal => lambda { |event|
						say "Received exit signal, hence music is paused.  Enjoy the music again."
				}
			}
			
			say "Waiting for stopHold signal"
			say messageToPlay, 
			{
				:allowSignals => "stopHold",
				:onSignal => lambda { |event|
					say "Received stop hold signal, hence music is paused. Enjoy the music again."
				}
			}
			
			say "Waiting for dequeue signal"
			say messageToPlay, 
			{
				:allowSignals => "dequeue",
				:onSignal => lambda { |event|
					say "Received dequeue signal, hence music is stopped."
				}
			}
		}
    }

    say "Thank you, for using A T and T. Call Management Service Sample Application Demo. Good Bye" 
    hangup
}
else
{
    call numberToDial
    say "Welcome to the A T and T. Call Management Services Sample Application."
	switch feature
	{
	
	case 'answer':
        say "thank you for using answering script function"
		break
	case 'ask':
	
        result9 = ask "Press four or five digits and Press pound when finished", 
		{
            :choices => "[4-5 DIGITS]",
            :terminator => '#',
			:timeout => 90.0,
			:mode => "dtmf",
			:interdigitTimeout => 5,
			:onChoice => lambda { |event|
                say "Thank you for entering"
            },
            :onBadChoice => lambda { |event|
                say "I am sorry, not able to get the four or five digits"
            }
        }
        if result9 != ""
        {
            say result9
        }
        break
	case 'call':
        say "thank you for using calling script function"
		break
	case 'conference':
        say "Thank you for joining conferenceing"
		say "1337"
        say "and to quit press star at any time"
        conference "1337", 
		{
            :terminator => '*',
            :playTones => true,
            :onChoice => lambda { |event|
                say "Disconnecting from conference"
            }
        }
        break
	case 'getHeader':
        if $currentCall.getHeader("to") 
		{
            say "Logged to header value"
            log "Your header value is " + $currentCall.getHeader("to")
        }
        else 
		{
            say "could not find to header value"
            log("Your header value was not found"
        }
        break
	case 'hangup':
		say "thank you for using hang script function"
		break
	case 'log':
        if $currentCall.getHeader("to")
		{
            say "Logged to header value"
            log "Your header value is " + $currentCall.getHeader("to")
        }
        else 
		{
            say "could not find to header value"
            log "Your header value was not found"
        }
        break
	case 'message':
		say "Sending message to"
	    say featurenumber
	    message "Message from AT&T Call Control Service Sample Application", 
		{
	        :to => featurenumber,
	        :network => "SMS"
	    }
	    break
	case 'reject':
        callerID = $currentCall.callerID
        if callerID == featurenumber
		{
            say "your calls will be rejected"
            say "Thank you, for using A T and T Call management Sample Application Demo.  Good Bye"
            reject
        }
        else
        {
            say "present id is"
            say $currentCall.callerID
            say "rejectnumber is "
            say featurenumber
            say "number not matched for reject feature"
        }
        break
	case 'say':
		say "thank you for using saying script function"
		break
	case 'transfer':
        say "transfering to "
        say featurenumber
        transfer [featurenumber, "sip:12345678912@221.122.54.86"], 
		{
            :playvalue => messageToPlay,
            :terminator => '*'
            :onTimeout => lambda { |event|
                say "Sorry, but nobody answered"
            }
        }
        break
	case 'wait':
        callerID = $currentCall.callerID
        if callerID == featurenumber
		{
            say "your calls will be kept for three seconds wait"
            wait(3000)
        }
        else
		{
			say "present id is"
			say $currentCall.callerID
			say "requested id is"
			say featurenumber
			say "number not matched for wait feature"
        }
        break
	}

    result4 = ask "Press a digit to test the signalling or Press pound to skip", 
	{
        :choices => "[1 DIGITS]",
        :terminator => '#',
        :timeout => 5.0,
        :mode => "dtmf",
        :interdigitTimeout => 5,
        :onChoice => lambda { |event|
			say "Waiting for exit signal"
			say messageToPlay, 
			{
				:allowSignals => "exit",
				:onSignal => function (event) {
					say "Received exit signal, hence music is paused.  Enjoy the music again."
            }
        }
			say "Waiting for stopHold signal"
			say messageToPlay, 
			{
				:allowSignals => "stopHold",
				:onSignal => lambda { |event|
					say "Received stop hold signal, hence music is paused. Enjoy the music again."
			}
        }
        say "Waiting for dequeue signal"
        say messageToPlay, 
		{
			:allowSignals => "dequeue",
			:onSignal => lambda { |event|
				say "Received dequeue signal, hence music is stopped."
            }
        }
        },
    }

    say "Thank you, for using A T and T. Call Management Service Sample Application Demo.  Good Bye"
    hangup
}