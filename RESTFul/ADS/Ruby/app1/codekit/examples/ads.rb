#!/usr/bin/env ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http:/proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure ADS is enabled for the app key/secret.

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

# Get OAuth token using the ADS scope
token = clientcred.createToken('ADS')

# Create service for interacting with the ADS api
ads = Service::ADSService.new(fqdn, token)

# User agent (must be mobile)
user_agent = 'Mozilla/5.0 (Android; Mobile; rv:13.0) Gecko/13.0 Firefox/13.0'

# Random unique value
udid = '938382893239492349234923493249'

begin 
  # Send a request to the API for getting an advertisement using 'auto' as the
  # category.
  response = ads.getAds('auto', user_agent, udid)

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else
  # It's important to check that ads were returned since a successful response
  # does not guarantee ad content
  if response.hasAds?

    # Simply display the results
    puts "ClickUrl: #{response.clickurl}"
    puts "Content: #{response.content}"

  end
end
