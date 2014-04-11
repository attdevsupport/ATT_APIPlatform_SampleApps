#!/usr/bin/env ruby
# This quickstart guide requires the Ruby codekit, which can be found at:
# https://github.com/attdevsupport/codekit-ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http://proxyaddress.com:port")

# To use OAuth you must first setup an app using developer.att.com 
# Enable the apps you wish to have access to with the OAuth token

# Enter the scopes that you want the token to be able to access. 
# Scopes must be from apps enabled above. Note that it's not required to specify
# all the scopes enabled, only those that the token should have access to.
SCOPE = ['SCOPE_ONE','SCOPE_TWO']

# Enter the value from 'App Key' field obtained at developer.att.com
CLIENT_ID = 'ENTER VALUE!'

# Enter the value from 'Secret' field obtained at developer.att.com
CLIENT_SECRET = 'ENTER VALUE!'

# Set the fqdn to default of https://api.att.com
FQDN = 'https://api.att.com'

# There are currently two ways to obtain a token.
# 1. Client Credentials - Does not require user authentication to use the api
# 2. Authorization Code - Requires user authentication to use the api

# Client credentials flow
begin 
  # Create client credential service for requesting an OAuth token
  client_cred = Auth::ClientCred.new(FQDN, 
                                     CLIENT_ID,
                                     CLIENT_SECRET)

  # Get OAuth token using the the scope of the apps
  token = client_cred.createToken(SCOPE)

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else
  # Print out the token's contents.
  puts "Client Credentials"
  puts "\tAccess Token: #{token.access_token}"
  puts "\tRefresh Token: #{token.refresh_token}"
  puts "\tIs the token expired? #{token.expired?}"

  puts "\tJSON format: #{token.to_json}"
end

# just padding space between outputs
puts or puts 

# Authorization code flow
begin 

  # Create an AuthCode service to obtain the consent flow url and token
  auth_code = Auth::AuthCode.new(FQDN,
                                 CLIENT_ID,
                                 CLIENT_SECRET)

  # Authenticate the user. note: this requires a browser

  # Obtain the url string that will be used for consent flow
  consent_url = auth_code.consentFlow(:redirect => "http://localhost",
                                      :scope => SCOPE)

  # display a link with the consent flow url 
  puts "Consent flow url: #{consent_url}"

  # Wait for user input after spawning consent flow
  puts "Please input the code in the query parameters after doing consent flow:"
  code = gets.strip

  # Get OAuth token using the the scope of the apps
  token = auth_code.createToken(code)

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else
  # Print out the token's contents.
  puts "Authorization Code"
  puts "\tAccess Token: #{token.access_token}"
  puts "\tRefresh Token: #{token.refresh_token}"
  puts "\tIs the token expired? #{token.expired?}"

  puts "\tJSON format: #{token.to_json}"
end
