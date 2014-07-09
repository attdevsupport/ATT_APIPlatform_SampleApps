#!/usr/bin/env ruby
# This quickstart guide requires the Ruby code kit, which can be found at:
# https://github.com/attdevsupport/codekit-ruby

# Make sure that the att-codekit has been installed, then require the class.
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional).
include Att::Codekit

# If a proxy is required, uncomment the following line to set the proxy.
# Transport.proxy("http://proxyaddress.com:port")

# To use OAuth 2.0 you must first set up an app account using developer.att.com.

# Enter the API scopes that you want to access with the OAuth access token. 
# API scopes must be from app accounts enabled above. 
# Note: It is not required to specify all the API scopes that are enabled, only 
# those that the OAuth access token should access.
SCOPE = ['SCOPE_ONE','SCOPE_TWO']

# Enter the value from 'App Key' field obtained at developer.att.com 
# in your app account.
CLIENT_ID = 'ENTER VALUE!'

# Enter the value from 'App Secret' field obtained at developer.att.com 
# in your app account.
CLIENT_SECRET = 'ENTER VALUE!'

# Set the fully-qualified domain name to: https://api.att.com
FQDN = 'https://api.att.com'

# An OAuth access token can be accessed using the following:
#   Client Credentials : Does not require user authentication to use the API.
#   Authorization Code : Requires user authentication to use the API.

# Client credentials flow.
begin 
  # Create the client credential service for requesting an OAuth access token.
  client_cred = Auth::ClientCred.new(FQDN, 
                                     CLIENT_ID,
                                     CLIENT_SECRET)

  # Get the OAuth access token using the API scopes.
  token = client_cred.createToken(SCOPE)

# Display any error codes returned by the API Gateway.
rescue Service::ServiceException => e
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else
  # Display the token's contents.
  puts "Client Credentials"
  puts "\tOAuth access token: #{token.access_token}"
  puts "\tRefresh Token: #{token.refresh_token}"
  puts "\tIs the OAuth access token expired? #{token.expired?}"

  puts "\tJSON format: #{token.to_json}"
end

# Add padding space between outputs.
puts or puts 

# Authorization code flow.
begin 

  # Create the Authorization Code service used to obtain the OAuth authorization code.
  auth_code = Auth::AuthCode.new(FQDN,
                                 CLIENT_ID,
                                 CLIENT_SECRET)

  # Authenticate the user. Note: This requires a Web browser.

  # Obtain the URI string that is used for consent flow.
  consent_url = auth_code.consentFlow(:redirect => "http://localhost",
                                      :scope => SCOPE)

  # Display the link with the consent flow URL. 
  puts "Consent flow URI: #{consent_url}"

  # Wait for user input after spawning consent flow.
  puts "Please input the OAuth authorization code in the query parameters after doing consent flow:"
  code = gets.strip

  # Get the OAuth access token using the OAuth authentication code .
  token = auth_code.createToken(code)

rescue Service::ServiceException => e
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else
  # Display the contents of the OAuth access token.
  puts "Authorization Code"
  puts "\tOAuth access token: #{token.access_token}"
  puts "\tRefresh Token: #{token.refresh_token}"
  puts "\tIs the OAuth access token expired? #{token.expired?}"

  puts "\tJSON format: #{token.to_json}"
end
