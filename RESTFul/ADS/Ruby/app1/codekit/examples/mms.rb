#!/usr/bin/env ruby
# This Quickstart Guide for the MMS API requires the Ruby code kit, 
# which can be found at: 
# https://github.com/attdevsupport/codekit-ruby

# Make sure that the att-codekit has been installed, then require the class.
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional).
include Att::Codekit

# If a proxy is required, uncomment the following line to set the proxy.
# Transport.proxy("http://proxyaddress.com:port")

# Use the app account settings from developer.att.com for the following values.
# Make sure that the API scope is set to MMS for the MMS API 
# before retrieving the App Key and App Secret.

# Enter the value from 'App Key' field obtained at developer.att.com 
# in your app account.
client_id = 'ENTER VALUE!'

# Enter the value from 'App Secret' field obtained at developer.att.com 
# in your app account.
client_secret = 'ENTER VALUE!'

# Set the fully-qualified domain name to: https://api.att.com
fqdn = 'https://api.att.com'

# Create the service for requesting an OAuth access token.
clientcred = Auth::ClientCred.new(fqdn, 
                                  client_id,
                                  client_secret)

# Get the OAuth access token using the API scope set to MMS.
token = clientcred.createToken('MMS')

# Create the service for interacting with the MMS API.
mms = Service::MMSService.new(fqdn, token)

# Specify the addresses where the file is sent. 
addresses = "555-555-5555,444-555-5555"

# Alternatively, addresses can be specified using an array.
# addresses = [5555555555,"444-555-5555"]

# Specify the path to the file that is sent as an attachment.
ATTACHMENT = '/temp/somefile.png'



# Send the MMS message.
begin

  # Send the message to the specified addresses.
  response = mms.sendMms(addresses, "Example", ATTACHMENT)

rescue Service::ServiceException => e

  # Print any error code returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display information about the sent message.
  puts "Sent MMS with id: #{response.id}"
  puts "#{response.id} has the resource url: #{response.resource_url}"

end

puts 

# Check the MMS status.
begin

  status = mms.mmsStatus(response.id)

rescue Service::ServiceException => e

  # Print any error code returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  puts "Status response:"
  puts "\tResource URL: #{status.resource_url}"
  puts "\tDelivery Info:"

  status.delivery_info.each do |info|
    puts "\t\t------------------------"
    puts "\t\tMessage ID: #{info.id}"
    puts "\t\tAddress: #{info.address}"
    puts "\t\tStatus: #{info.status}"
    puts "\t\t------------------------"
  end
end

puts

# The following is commented out, but demonstrates how to accept and parse
# an MMS message that is received over http.
#
## The following will need to be implemented inside a listener url.
## The URI must be setup on the Developer Program website in your app account.
## 
## Handle a received MMS message.
#begin
#
#  # You must obtain the raw input of the request. This will change depending
#  # on the framework that you use, but most likely it will be obtained using 
#  # rack.input.
#  #
#  # Sinatra is required for the following command.
#  input = request.env["rack.input"].read
#
#  mms_message = mms.parseReceivedMms(input)
#
#rescue Service::ServiceException => e
#
#  # Print any error code returned by the API Gateway.
#  puts "There was an error, the API Gateway returned the following error code:"
#  puts "#{e.message}"
#
#else 
#
#  puts "Sender: #{mms_message.sender}"
#  puts "Date: #{mms_message.date}"
#  puts "Text: #{mms_message.text}"
#  puts "Type: #{mms_message.type}"
#  puts "Attachment: #{mms_message.attachment}"
#
#end
