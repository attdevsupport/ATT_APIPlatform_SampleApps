#!/usr/bin/env ruby
# This quickstart guide requires the Ruby codekit, which can be found at:
# https://github.com/attdevsupport/codekit-ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http://proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure IMMN is enabled for the app key/secret.

# Enter the value from 'App Key' field
client_id = 'ENTER VALUE!'

# Enter the value from 'Secret' field
client_secret = 'ENTER VALUE!'

# Set the fqdn to default of https://api.att.com
fqdn = 'https://api.att.com'

# Set the redirect url for returning after consent flow
base_redirect_url = "http://localhost:4567"

# Create service for requesting an OAuth token
authcode = Auth::AuthCode.new(fqdn, 
                              client_id,
                              client_secret)


# Authenticate the user. note: this requires a browser

# Obtain the url string that will be used for consent flow
consent_url = authcode.consentFlow(:redirect => base_redirect_url)

# display a link with the consent flow url 
puts consent_url

# Wait for user input after spawning consent flow
puts "Please input the code in the query parameters after doing consent flow:" 
code = gets.strip

# Get the token using the authentication code
token = authcode.createToken(code)

# Create a service for making the API call
mim = Service::MIMService.new(fqdn, token)

# Obtain a list of messages sent to authenticated phone
begin

  COUNT = 10

  msg_list = mim.getMessageList(COUNT)

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simple dump of results
  msg_list.each_pair do |attribute, value|
    puts "#{attribute}: \t#{value}"
  end
  
end

puts

# Obtain a message by ID
begin

  # Note: this is redundant and only an example; message will contain the same
  # data as msg_list.messages.first
  
  # Get the first message from above msg_list
  message = mim.getMessage(msg_list.messages.first.id)

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simple dump of results
  message.each_pair do |attribute, value|
    puts "#{attribute}: \t#{value}"
  end
  
end

puts

# Obtain content of an mms message
begin

  # Note: you will only be able to obtain the content of an mms message
  
  content = mim.getMessageContent(msg_list.messages.first.id)

rescue Service::ServiceException => e
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simple dump of results
  puts "Content type: #{content.content_type}"
  puts "Content length: #{content.content_length}"
  puts "Image? #{content.image?}"
  puts "Audio? #{content.audio?}"
  puts "Video? #{content.video?}"
  puts "Text? #{content.text?}"
  puts "SMIL? #{content.smil?}"
  
end
