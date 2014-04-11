#!/usr/bin/env ruby
# This quickstart guide requires the Ruby codekit, which can be found at:
# https://github.com/attdevsupport/codekit-ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http://proxyaddress.com:port")

# Use the app account settings from developer.att.com for the following values.
# Make sure ADS is enabled for the App Key and App Secret.

# Enter the value from 'App Key' field
client_id = 'ENTER VALUE!'

# Enter the value from 'App Secret' field
client_secret = 'ENTER VALUE!'

# Set the fqdn to default of https://api.att.com
fqdn = 'https://api.att.com'

# Create service for requesting an OAuth access token
clientcred = Auth::ClientCred.new(fqdn, 
                                  client_id,
                                  client_secret)

# Get OAuth access token using the Advertising API scope
token = clientcred.createToken('ADS')

# Create service for interacting with the Advertising API
ads = Service::ADSService.new(fqdn, token)

# User agent (must be mobile)
user_agent = 'Mozilla/5.0 (Android; Mobile; rv:13.0) Gecko/13.0 Firefox/13.0'

# Random unique value
udid = '938382893239492349234923493249'

begin 
  # Send a request to the API Gateway for getting an advertisement using 'auto'
  # as the category.
  response = ads.getAds('auto', user_agent, udid)

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else
  # It's important to check that advertisements were returned since a
  # successful response does not guarantee advertisement content
  if response.hasAds?

    # Simply display the results
    puts "ClickUrl: #{response.clickurl}"
    puts "Content: #{response.content}"

  end
end
