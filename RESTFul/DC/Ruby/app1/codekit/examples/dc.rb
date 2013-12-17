#!/usr/bin/env ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http:/proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure DC is enabled for the app key/secret.

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
dc = Service::DCService.new(fqdn, token)

# Use exception handling to see if anything went wrong with the request
begin

  # Make api call to get authenticated device's capabilities
  response = dc.getDeviceCapabilities

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simple dump of results
  puts ""
  puts "Device ID: #{response.device_id.type_allocation_code}"
  puts "Device Capabilities:"
  response.capabilities.each do |cap, value|
    puts "\t#{cap}: #{value}"
  end
  
end
