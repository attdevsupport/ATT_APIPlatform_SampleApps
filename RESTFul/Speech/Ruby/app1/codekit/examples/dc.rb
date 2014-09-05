#!/usr/bin/env ruby
# This Quickstart Guide for the Device Capabilities API requires the Ruby 
# code kit, which can be found at: 
# https://github.com/attdevsupport/codekit-ruby

# Make sure that the att-codekit has been installed, then require the class.
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional).
include Att::Codekit

# If a proxy is required, uncomment the following line.
# Transport.proxy("http://proxyaddress.com:port")

# Use the app account settings from developer.att.com for the following values.
# Make sure that the API scope is set to DC for the Device Capabilities API 
# before retrieving the App Key and App Secret.

# Enter the value from 'App Key' field obtained at developer.att.com 
# in your app account.
client_id = 'ENTER VALUE!'

# Enter the value from 'App Secret' field obtained at developer.att.com 
# in your app account.
client_secret = 'ENTER VALUE!'

# Set the fully-qualified domain name to: https://api.att.com
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

# Create the service for making the method request.
dc = Service::DCService.new(fqdn, token)

# Use exception handling to see if anything went wrong with the method request.
begin

  # Make the method request to get the capabilities of the authenticated device.
  response = dc.getDeviceCapabilities

rescue Service::ServiceException => e
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display of results.
  puts ""
  puts "Device ID: #{response.device_id.type_allocation_code}"
  puts "Device Capabilities:"
  response.capabilities.each do |cap, value|
    puts "\t#{cap}: #{value}"
  end
  
end
