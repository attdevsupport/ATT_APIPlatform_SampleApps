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
# Make sure MMS is enabled for the app key/secret.

# Enter the path of a file to send
ATTACHMENT = '/tmp/somefile.png'

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

# Get OAuth token using the MMS scope
token = clientcred.createToken('MMS')

# Create service for interacting with the MMS api
mms = Service::MMSService.new(fqdn, token)

# Setup the addresses that we want to send 
addresses = "555-555-5555,444-555-5555"

# Alternatively we can use an array
# addresses = [5555555555,"444-555-5555"]

# Send an MMS message to the specified address(es)
begin

  # Send a message to the addresses specified
  response = mms.sendMms(addresses, "Example", ATTACHMENT)

rescue Service::ServiceException => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  puts "Sent MMS with id: #{response.id}"
  puts "#{response.id} has the resource url: #{response.resource_url}"

end

puts 

# Check the MMS status
begin

  status = mms.mmsStatus(response.id)

rescue Service::ServiceException => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
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

# The following is commented out but demonstrates how to accept and parse
# an mms message received over http
#
## The following will need to be implemented inside a listener url.
## The url must be setup in the developer portal.
## 
## Handle a received mms
#begin
#
#  # You must obtain the raw input of the request. This will change depending
#  # on the framework you use, but most likely it will be obtained using 
#  # rack.input
#  #
#  # Via sinatra:
#  input = request.env["rack.input"].read
#
#  mms_message = mms.parseReceivedMms(input)
#
#rescue Service::ServiceException => e
#
#  # There was an error in execution print what happened
#  puts "There was an error, the api returned the following error code:"
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
