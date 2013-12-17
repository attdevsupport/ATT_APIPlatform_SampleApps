#!/usr/bin/env ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http:/proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure IMMN is enabled for the app key/secret.

# Enter the value from 'App Key' field
client_id = 'ENTER VALUE!'

# Enter the value from 'Secret' field
client_secret = 'ENTER VALUE!'

# Set the fqdn to default of https://api.att.com
fqdn = 'https://api.att.com'

# Set the redirect url for returning after consent flow
redirect_url = "http://localhost:4567"

# Create service for requesting an OAuth token
authcode = Auth::AuthCode.new(fqdn, 
                              client_id,
                              client_secret,
                              :redirect => redirect_url)


# Authenticate the user. note: this requires a browser

# Obtain the url string that will be used for consent flow
consent_url = authcode.consentFlow

# display a link with the consent flow url 
puts consent_url

# Wait for user input after spawning consent flow
puts "Please input the code in the query parameters after doing consent flow:" 
code = gets.strip

# Get the token using the authentication code
token = authcode.createToken(code)

# Setup the addresses that we want to send 
addresses = "555-555-5555,444-555-5555"

# Alternatively we can use an array
# addresses = [5555555555,"444-555-5555"]

# Create a service for making the API call
immn = Service::IMMNService.new(fqdn, token)

# Use exception handling to see if anything went wrong with the request
begin

  # Make api calls to send a sms and mms
  sms = immn.sendMessage(addresses, :message => "This is a sms message from immn example")
  mms = immn.sendMessage(addresses, :subject => "This is a mms message from immn example")

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simple dump of results
  puts "The response id of the sms message was: #{sms.id}"
  puts "The response id of the mms message was: #{mms.id}"
  
end
