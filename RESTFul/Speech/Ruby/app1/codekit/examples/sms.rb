#!/usr/bin/env ruby
# This Quickstart Guide for the SMS API requires the Ruby code kit, 
# which can be found at: https://github.com/attdevsupport/codekit-ruby

# Make sure that the att-codekit has been installed, then require the class.
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional).
include Att::Codekit

# If a proxy is required, uncomment the following line to set the proxy.
# Transport.proxy("http://proxyaddress.com:port")

# Use the app account settings from developer.att.com for the following values.
# Make sure that the API scope is set to SMS for the SMS API before 
# retrieving the App Key and App Secret.

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

# Get OAuth access token using the API scope set to SMS.
token = clientcred.createToken('SMS')

# Create the service for interacting with the SMS API.
sms = Service::SMSService.new(fqdn, token)

# Set up the addresses where the message is sent.
addresses = "555-555-5555,444-555-5555"

# Alternatively, an array can be used to specify the addresses.
# addresses = [5555555555,"444-555-5555"]

# Send an SMS message to the specified addresses.
begin

  response = sms.sendSms(addresses, "Message from the ATT codekit SMS example")


rescue Service::ServiceException => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  puts "Sent SMS with ID: #{response.id}"
  puts "Resource url: #{response.resource_url}"

end

puts

# Check the status of the sent SMS message.
begin 

  status = sms.smsStatus(response.id)

rescue Service::ServiceException => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  puts "Status response:"
  puts "\tResource URI: #{status.resource_url}"
  puts "\tDelivery Info:"

  status.delivery_info.each do |info|
    puts "\t\t------------------------"
    puts "\t\tMessage ID: #{info.id}"
    puts "\t\tAddress: #{info.address}"
    puts "\t\tStatus: #{info.status}"
    puts "\t\t------------------------"
  end

end
