#!/usr/bin/env ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http:/proxyaddress.com:port")

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

# Use exception handling to see if anything went wrong with the request
begin

  # Send a message to the addresses specified
  response = mms.sendMms(addresses, "Example", ATTACHMENT)
  status = mms.mmsStatus(response.id)

rescue Service::ServiceException => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  puts "Sent MMS with id: #{response.id}"
  puts "#{response.id} has the resource url: #{response.resource_url}"
  puts ""
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
