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
# Make sure that the API scope is set to AAB for the Address Book API
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

contact_name = "Isaac"
group_name = "Example"

contact_id = ""
group_id = ""

# Create a new contact for our Address Book
puts 
puts "Create:"
begin
  # Create a contact object
  contact = Model::Contact.new(:first_name => contact_name,
                               :last_name => "Newton")

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
puts "Updating contact's last name"
begin
  contact = Model::Contact.new(:last_name => "Asimov")
  response = address_book.updateContact(contact_id, contact)
rescue Service::ServiceException => e
  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"
else
  # If we reach this point then the create was successful
  puts "\tSuccessfully updated contact to: #{contact_name} #{contact.last_name}"
end

# Create a new group
puts
puts "Creating new group"
begin
  group = Model::Group.new(:name => "Example")
  response = address_book.createGroup(group)
  # the contact id will be contained as the last element of the location
  group_id = response.location.split("/")[-1]
rescue Service::ServiceException => e
  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"
else
  # If we reach this point then the create was successful
  puts "\tSuccessfully created group: #{group.name}"
  puts "\tat the location: #{response.location}"
end

puts
puts "Adding contact to group"
begin
  response = address_book.updateContact(group_id, contact_id)
rescue Service::ServiceException => e
  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"
else
  # If we reach this point then the create was successful
  puts "\tSuccessfully updated contact to: #{contact_name} #{contact.last_name}"
end

puts
puts "Obtaining group contacts"
begin
  response = address_book.getGroupContacts(group_id)
rescue Service::ServiceException => e
  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"
else
  puts "Contacts in group - #{group_id}:"
  response.each do |id|
    puts "\t#{id}"
  end
end

puts 
puts "Search:"
# Search for the created contact
begin
  contact_results = address_book.getContacts(:search => contact_name)
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
puts "Removing contact from group"
begin
  response = address_book.removeContactFromGroup(group_id, contact_id)
rescue Service::ServiceException => e
  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"
end

puts 
puts "Delete group:"
# Delete the created contact
begin
 response = address_book.deleteGroup(group_id)
rescue Service::ServiceException => e
  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"
else
  puts "\tGroup #{group_id} successfully deleted"
end

puts 
puts "Delete contact:"
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

puts 
puts "Get My Info:"
begin
  me = address_book.getMyInfo
rescue Service::ServiceException => e
  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"
else
  me.each_pair do |key, value|
    puts "\t#{key}, #{value}"
  end
end

puts 
puts "Update My Info:"
begin
  updated_me = Model::MyContactInfo.new(:suffix => "Esquire")
  response = address_book.updateMyInfo(updated_me)
rescue Service::ServiceException => e
  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"
else
  response.each_pair do |key, value|
    puts "\t#{key}, #{value}"
  end
end
