#!/usr/bin/env ruby
# This quickstart guide requires the Ruby codekit, which can be found at:
# https://github.com/attdevsupport/codekit-ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http://proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure SPEECH is enabled for the app key/secret.

# Enter the value from 'App Key' field
client_id = 'ENTER VALUE!'

# Enter the value from 'Secret' field
client_secret = 'ENTER VALUE!'

# Set the fqdn to default of https://api.att.com
fqdn = 'https://api.att.com'

# Create service for requesting an OAuth token
clientcred = Auth::ClientCred.new(fqdn, 
                                  client_id,
                                  client_secret)

# Get OAuth token using the SPEECH scope
token = clientcred.createToken('SPEECH')

# Add an audio file to convert to text
AUDIO = "/path/to/audio/file"

# Create service for interacting with the SPEECH api
speech = Service::SpeechService.new(fqdn, token)

# Use exception handling to see if anything went wrong with the request
begin

  # Send a message to the addresses specified
  response = speech.toText(AUDIO)

rescue Service::ServiceException => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simple dump of all the data returned
  puts "Converted Speech with status response: #{response.status}"
  puts "Speech ID: #{response.id}"
  puts
  puts "NBest values:"
  response.nbest.each do |n|
    puts "---------------"
    n.each_pair do |name, value|
      if name == :nlu_hypothesis
        puts "\tNLU Hypothesis:"
        value.each do |v|
          v.each_pair { |x, y| puts "\t\t#{x}: #{y}" }
        end
      else
        puts "\t#{name}: #{value}"
      end
    end
  end

end
