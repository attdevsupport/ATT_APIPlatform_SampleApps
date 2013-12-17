#!/usr/bin/env ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http:/proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure TL is enabled for the app key/secret.

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

# Create a service for making the API call
tl = Service::TLService.new(fqdn, token)

# Use exception handling to see if anything went wrong with the request
begin

  # Make api call to get location with default parameters
  response = tl.getDeviceLocation

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simple dump of results
  puts ""
  puts "accuracy: #{response.accuracy}"
  puts "latitude: #{response.latitude}"
  puts "longitude: #{response.longitude}"
  puts "timestamp: #{response.timestamp}"
  
end
