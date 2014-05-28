#!/usr/bin/env ruby
# This Quickstart Guide requires the Ruby code kit, 
# which can be found at: 
# https://github.com/attdevsupport/codekit-ruby

# Make sure that the att-codekit has been installed, then require the class.
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional).
include Att::Codekit

# If a proxy is required, uncomment the following line to set the proxy.
# Transport.proxy("http://proxyaddress.com:port")

# Use the app account settings from developer.att.com for the following values.
# Make sure that the API scope is set to MIM before retrieving the App Key and App Secret.

# Enter the value from 'App Key' field obtained at developer.att.com 
# in your app account.
client_id = 'ENTER VALUE!'

# Enter the value from 'App Secret' field obtained at developer.att.com 
# in your app account.
client_secret = 'ENTER VALUE!'

# Set the fully-qualified domain name to: https://api.att.com
fqdn = 'https://api.att.com'

# Set the redirect URI for returning after consent flow.
base_redirect_url = "http://localhost:4567"

# Create the service for requesting an OAuth access token.
authcode = Auth::AuthCode.new(fqdn, 
                              client_id,
                              client_secret)


# Authenticate the user. Note: This requires a Web browser.

# Obtain the url string that is used for consent flow.
consent_url = authcode.consentFlow(:redirect => base_redirect_url)

# Display the link with the consent flow URI. 
puts consent_url

# Wait for user input after spawning consent flow.
puts "Please input the code in the query parameters after doing consent flow:" 
code = gets.strip

# Get the OAuth access token using the OAuth authentication code.
token = authcode.createToken(code)

# Create a service for making the method request.
mim = Service::MIMService.new(fqdn, token)

# Obtain a list of the messages sent to the authenticated phone.
begin

  COUNT = 10

  msg_list = mim.getMessageList(COUNT)

rescue Service::ServiceException => e
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display the results.
  msg_list.each_pair do |attribute, value|
    puts "#{attribute}: \t#{value}"
  end
  
end

puts

# Obtain a message by ID.
begin

  # Note: this property also returns the first message in the list and can be used to
  # confirm the return value of the getMessage example that follows.
  # data as msg_list.messages.first
  
  # Get the first message from the previous msg_list.
  message = mim.getMessage(msg_list.messages.first.id)

rescue Service::ServiceException => e
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display the results.
  message.each_pair do |attribute, value|
    puts "#{attribute}: \t#{value}"
  end
  
end

puts

# Obtain the content of an MMS message.
begin

  # Note: You will be able to only obtain the content of an MMS message.
  
  content = mim.getMessageContent(msg_list.messages.first.id)

rescue Service::ServiceException => e
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display the results.
  puts "Content type: #{content.content_type}"
  puts "Content length: #{content.content_length}"
  puts "Image? #{content.image?}"
  puts "Audio? #{content.audio?}"
  puts "Video? #{content.video?}"
  puts "Text? #{content.text?}"
  puts "SMIL? #{content.smil?}"
  
end
