#!/usr/bin/env ruby
# This Quickstart Guide for the In-App Messaging API requires the Ruby 
# code kit, which can be found at: 
# https://github.com/attdevsupport/codekit-ruby

# Make sure that the att-codekit has been installed, then require the class.
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional).
include Att::Codekit

# If a proxy is required, uncomment the following line.
# Transport.proxy("http://proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure that the API scope is set to IMMN for the In-App Messaging API 
# before retrieving the App Key and App Secret.

# Enter the value from 'App Key' field obtained at developer.att.com 
# in your app account.
client_id = 'ENTER VALUE!'

# Enter the value from 'App Secret' field obtained at developer.att.com 
# in your app account.
client_secret = 'ENTER VALUE!'

# Set the fully-qualified domain name to: https://api.att.com.
fqdn = 'https://api.att.com'

# Set the redirect URI for returning after consent flow.
redirect_url = "http://localhost:4567"

# Create the service for requesting an OAuth access token.
authcode = Auth::AuthCode.new(fqdn, 
                              client_id,
                              client_secret,
                              :redirect => redirect_url)


# Authenticate the user. Note: This requires a Web browser.

# Obtain the url string that is used for consent flow.
consent_url = authcode.consentFlow

# Display a link with the consent flow URI. 
puts consent_url

# Wait for user input after spawning consent flow.
puts "Please input the code in the query parameters after doing consent flow:" 
code = gets.strip

# Get the OAuth access token using the OAuth authentication code.
token = authcode.createToken(code)

# Specify the addresses where the message is sent.
addresses = "555-555-5555,444-555-5555"

# Alternatively, the addresses can be specified using an array.
# addresses = [5555555555,"444-555-5555"]

# Create the service for making the method request.
immn = Service::IMMNService.new(fqdn, token)

# Use exception handling to see if anything went wrong with the request.
begin

  # Make method requests to send SMS and MMS messages.
  sms = immn.sendMessage(addresses, :message => "This is a SMS message from the In-App Messaging example")
  mms = immn.sendMessage(addresses, :subject => "This is a MMS message from the In-App Messaging example")

rescue Service::ServiceException => e
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display of results.
  puts "The response id of the sms message was: #{sms.id}"
  puts "The response id of the mms message was: #{mms.id}"
  
end
