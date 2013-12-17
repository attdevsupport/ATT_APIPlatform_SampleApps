#!/usr/bin/env ruby

# require json to decode response
require 'json'

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit

# Uncomment to set a proxy if required
# Transport.proxy("http:/proxyaddress.com:port")

# Use the app settings from developer.att.com for the following values.
# Make sure Payment is enabled for the app key/secret.

# Enter the value from 'App Key' field
client_id = 'ENTER VALUE!'

# Enter the value from 'Secret' field
client_secret = 'ENTER VALUE!'

# Set the fqdn to default of https://api.att.com
fqdn = 'https://api.att.com'


# Setup a client object for use with payment service
client = Auth::Client.new(client_id, client_secret)

# Create service for requesting an OAuth token
clientcred = Auth::ClientCred.new(fqdn, 
                                  client.id,
                                  client.secret)

# Get OAuth token using the Payment scope
token = clientcred.createToken('PAYMENT')

# Create service for interacting with the Payment api
payment = Service::PaymentService.new(fqdn, token, client)

# Define how much the product costs 
AMOUNT = 1

# Give the item a description
DESC = 'example game xtreme'

# Define a uniq merchant transaction id
MERCH_TRANS_ID = 'example' + rand(1_000_000)

# Set the merchant product id
MERCH_PROD_ID = 'exampleGame'

# Set the url to redirect to after authorizing payment
REDIRECT = 'http://localhost/handleAuth'

# Redirect is required per purchase to authenticate the purchase by user
puts payment.newTransaction(
  AMOUNT,
  Service::Categories::IN_APP_GAMES,
  DESC,
  MERCH_TRANS_ID,
  MERCH_PROD_ID,
  REDIRECT)

# Wait for user input after spawning auth flow
puts "Please input the TransactionAuthCode from the query parameters after doing consent flow:" 
code = gets.strip

# Use exception handling to see if anything went wrong with the request
begin
  
  # Get the transaction details
  response = payment.getTransaction(Service::TransactionType::TransactionAuthCode, code)

rescue Exception => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  status = JSON.parse response

  status.each do |key, value|
    puts "#{key}: #{value}"
  end

end
