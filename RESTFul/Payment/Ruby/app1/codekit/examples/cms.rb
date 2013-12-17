#!/usr/bin/env ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http:/proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure CMS is enabled for the app key/secret.

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

# Get OAuth token using the CMS scope
token = clientcred.createToken('CMS')

# Create service for interacting with the CMS api
cms = Service::CMSService.new(fqdn, token)

# Create an args hash map that supplies the variables/values for the cms script
args = {
  :var1 => "This is the value of var1",
  :var2 => "This is the value of var2"
}


# Use exception handling to see if anything went wrong with the request
begin

  # Send a request to the API for creating a session with the provided
  # variables
  response = cms.createSession(args)

  # Send a signal to the created session
  sigresponse = cms.sendSignal(response.id, "exit")

rescue Service::ServiceException => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # API calls were successful. Print simple output of received responses
  puts "Was session with id #{response.id} successful? #{response.success?}"
  puts "Signal call returned with status: #{sigresponse.status}"

end
