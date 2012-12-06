# Utility method for saying individual characters in a string
sayNumber = lambda { |text|
	text.split("").each do |c|
    say c
  end	
}

messageToPlay = "http://wdev.code-api-att.com:8181/Tropo/music.mp3"

if (defined? $numberToDial).nil?
  say "Welcome to the A T and T Call Management sample application demo."

  result20 = ask("Press a digit to listen to what you press or press pound to skip.",
    {
      :choices => "[1 DIGITS]",
      :terminator => "#",
      :timeout => 5.0,
      :mode => "dtmf",
      :interdigitTimeout => 5,
      :onChoice => lambda { |event|
        say "Thank you for entering"
      },
      :onBadChoice => lambda { |event|
        say "Sorry, I am not able to get the digit."
      }
    })
 
  if result20.value != ""
    say result20.value
  end

  result21 = ask("Press a digit to join 1337 conferencing or press pound to skip.",
    { 
      :choices => "[1 DIGITS]",
      :terminator => "#",
      :timeout => 5.0,
      :mode => "dtmf",
      :interdigitTimeout => 5,
      :onChoice => lambda { |event|
        say "Thank you for joining 1337 conferencing. To quit, press star at any time."
        conference("1337", 
          {
            :terminator => "*",
            :playTones => true,
            :onChoice => lambda { |event| 
              say "Disconnecting from conference."
            }
          })
      }
    }
  )

  result24 = ask("Enter a ten digit phone number from which you are calling to reject this call or press pound to skip.",
    {
      :choices => "[10 DIGITS]",
      :terminator => '#',
      :timeout => 120.0,
      :mode => "dtmf",
      :interdigitTimeout => 10,
      :onChoice => lambda { |event|
      }
    }
  )
  
  if result24.value != ""
    numbertest = result24.value
    strippedCID = $currentCall.callerID[2..-1] 
    if strippedCID .eql? numbertest
      say "Your calls will be rejected."
      say "Thank you for using A T and T Call Management sample application demo. Goodbye."
      reject
    else
      say "Number not matched for reject feature."
      say "The current caller I D is "
      sayNumber.call strippedCID
      say " and the number you entered is"
      sayNumber.call numbertest
    end
  end

  result25 = ask("Enter a 10 digit phone number to transfer the call or press pound to skip.",
    {
      :choices => "[10 DIGITS]",
      :terminator => "#",
      :timeout => 120.0,
      :mode => "dtmf",
      :interdigitTimeout => 10,
      :onChoice => lambda { |event| 
      }
    })

  if result25.value != ""
    numbertest = result25.value
    transfernumb = "+1" + numbertest
    say "Transfering to "
    
    sayNumber.call numbertest
  
    transfer(transfernumb, 
      {
        :playvalue => messageToPlay,
        :terminator => "*",
        :onCallFailure => lambda { |event|
          say "Unable to transfer. Call failed."
        },
        :onTimeout => lambda { |event|
          say "Unable to transfer. Nobody answered."
        }
      }
    )
  end 

  result27 = ask("Enter the ten digit phone number from which you are calling to wait this call or press pound to skip.", 
    {
      :choices => "[10 DIGITS]", 
      :terminator => "#",
      :timeout => 120.0,
      :mode => "dtmf",
      :interdigitTimeout => 10,
      :onChoice => lambda { |event| 
      }
    })
 
  if result27.value != ""
    numbertest = result27.value
    strippedCID = $currentCall.callerID[2..-1] 
    if strippedCID .eql? numbertest
      say "Entered number matched with current caller I D."
      say "Will now set the call to wait three seconds."
      wait(3000)
    else
      say "Number not matched for wait feature."
      say "The current caller I D is "
      
      sat " and the number you entered is"
      sayNumber.call numbertest
    end
  end
  
  result29 = ask("Press a digit to test the signaling or press pound to skip.",
    {
      :choices => "[1 DIGITS]",
      :terminator => "#",
      :timeout => 5.0,
      :mode => "dtmf",
      :interdigitTimeout => 5,
      :onChoice => lambda { |event|
        say "Waiting for exit signal"
        say(messageToPlay, 
        {
          :allowSignals => "exit",
          :onSignal => lambda { |event1|
            say "Received exit signal, hence the music is paused. Enjoy the music again."
          }
        })
        say "Waiting for stopHold signal"
        say(messageToPlay, 
        {
          :allowSignals => "stopHold",
          :onSignal => lambda { |event2| 
            say "Received stop hold signal, hence the music is paused. Enjoy the music again."
          }
        })
        say "Waiting for deueue signal"
        say(messageToPlay, 
        {
          :allowSignals => "dequeue",
          :onSignal => lambda { |event3| 
            say "Received dequeue signal, hence the music is stopped."
          }
        })
      }
    })

  say "Thank you for using A T and T Call Management sample application demo. Goodbye."
  hangup
else
  call $numberToDial
  say "Welcome to the A T and T Call Management sample application demo."
  case
    when ($feature .eql? "ask")
      say "The ask feature has been selected."
      result9 = ask("Press four or five digits and press pound when finished.", 
        {
          :choices => "[4-5 DIGITS]",
          :terminator => "#",
          :timeout => 90.0,
          :mode => "dtmf",
          :interdigitTimeout => 5,
          :onChoice => lambda { |event|
            say "Thank you for entering"
          },
          :onBadChoice => lambda { |event|
            say "Sorry, I am not able to get the four or five digits you pressed."
          }
        }
      )
      if result9.value != ""
        sayNumber.call result9.value
      end
    when ($feature .eql? "conference")
      say "Thank you for joining 1337 conferencing. To quit, press star at any time."
      conference("1337",
        {
          :terminator => "*",
          :playTones => true,
          :onChoice => lambda { |event|
            say "Disconnecting from conference."
          }
        }
      )
    when ($feature .eql? "reject")
      callID = $currentCall.callerID
      if callID .eql? $featurenumber
        say "Your calls will be rejected."
        say "Thank you for using A T and T Call Management application demo. Goodbye."
        reject
        hangup
      else
        say "Number not matched for reject feature."
        say "The present number is"
        sayNumber.call callID 
        say "and requested number is"
        sayNumber.call $featurenumber 
      end
    when ($feature .eql? "transfer")
      say "Transfering to"
      sayNumber.call $featurenumber
      transfer($featurenumber,
        {
          :playvalue => messageToPlay,
          :terminator => "*",
          :onCallFailure => lambda { |event|
            say "Unable to transfer. Call failed."
          },
          :onTimeout => lambda { |event|
            say "Unable to transfer. Nobody answered."
          }
        }
      )
    when ($feature .eql? "wait")
      callID = $currentCall.callerID
      if callID .eql? $featurenumber 
        say "Present number matched with requested number."
        say "Will now set the call to wait three seconds."
        wait(3000)
      else
        say "Number not matched for wait feature."
        say "The present number is"
        sayNumber.call callID 
        say "and the requested number is"
        sayNumber.call $featurenumber 
      end
  end

  result4 = ask("Press a digit to test the signaling or press pound to skip.",
    {
      :choices => "[1 DIGITS]",
      :terminator => "#",
      :timeout => 5.0,
      :mode => "dtmf",
      :interdigitTimeout => 5,
      :onChoice => lambda { |event|
        say "Waiting for exit signal"
        say(messageToPlay,
        {
          :allowSignals => "exit",
          :onSignal => lambda { |event1|
            say "Received exit signal, hence the music is paused. Enjoy the music again."
          }
        })
        say "Waiting for stopHold signal"
        say(messageToPlay,
        {
          :allowSignals => "stopHold",
          :onSignal => lambda { |event2|
            say "Received stop hold signal, hence the music is paused. Enjoy the music again."
          }
        })
        say "Waiting for dequeue signal"
        say(messageToPlay,
        {
          :allowSignals => "dequeue",
          :onSignal => lambda { |event3|
            say "Received dequeue signal, hence the music is stopped."
          }
        })
      }
    })

  say "Thank you for using the A T and T Call Management sample application demo. Goodbye."
  hangup

end
