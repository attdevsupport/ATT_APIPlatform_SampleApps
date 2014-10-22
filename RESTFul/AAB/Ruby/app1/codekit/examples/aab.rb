#!/usr/bin/env ruby
# This Quickstart Guide for the Address Book API requires the Ruby code kit, 
# which can be found at: https://github.com/attdevsupport/codekit-ruby

# Make sure that the att-codekit has been installed, then require the class.
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional).
include Att::Codekit

# If a proxy is required, uncomment the following line to set the proxy.
# Transport.proxy("http://proxyaddress.com:port")

# Use the app account settings from developer.att.com for the following values.
# Make sure that the API scope is set to Address Book for the Address Book API
# before retrieving the App Key and App Secret.

# Enter the value from 'App Key' field obtained at developer.att.com 
# in your app account.
client_id = 'ENTER VALUE!'

# Enter the value from 'App Secret' field obtained at developer.att.com 
# in your app account.
client_secret = 'ENTER VALUE!'

# Set the fully-qualified domain name to: https://api.att.com
fqdn = 'https://api.att.com'

# Set the redirect URI for returning after consent flow.
redirect_url = "http://localhost:4567"

# Create the service for requesting an OAuth access token.
authcode = Auth::AuthCode.new(fqdn, 
                              client_id,
                              client_secret)

# Authenticate the user. Note: This requires a Web browser.

# Obtain the url string that is used for consent flow.
consent_url = authcode.consentFlow(:redirect => redirect_url)

# Display a link with the consent flow URI. 
puts consent_url

# Wait for user input after spawning consent flow.
puts "Please input the code in the query parameters after doing consent flow:" 
code = gets.strip

# Get the OAuth access token using the OAuth authentication code.
token = authcode.createToken(code)
 
# Create the service for making the method request.
address_book = Service::AABService.new(fqdn, token)

puts 
puts "Create:"

contact_id = ""

# Create a new contact for our Address Book
begin

  # Create a contact object
  contact = Model::Contact.new(:first_name => "Alf",
                               :last_name => "Alfa")

  response = address_book.createContact(contact)

  # the contact id will be contained as the last element of the location
  contact_id = response.location.split("/")[-1]

rescue Service::ServiceException => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # If we reach this point then the create was successful
  puts "\tSuccessfully created contact: #{contact.first_name} #{contact.last_name}"
  puts "\tat the location: #{response.location}"

end

puts 
puts "Search:"
# Search for the created contact
begin

  contact_results = address_book.getContacts(:search => "Alf Alfa")

rescue Service::ServiceException => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  contact_results.each_pair do |attr, value|
    puts "\t#{attr}: #{value}"
  end
end

puts 
puts "Delete:"
# Delete the created contact
begin

  contact_results = address_book.deleteContact(contact_id)

rescue Service::ServiceException => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  puts "\tContact #{contact_id} successfully deleted!"

end
