#!/usr/bin/env ruby
# This Quickstart Guide for the Payment API requires the Ruby code kit, 
# which can be found at: https://github.com/attdevsupport/codekit-ruby

# Make sure that the att-codekit has been installed, then require the class.
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional).
include Att::Codekit
include Att::Codekit::Service

# If a proxy is required, uncomment the following line to set the proxy.
# Transport.proxy("http://proxyaddress.com:port")

# Use the application settings from developer.att.com for the following values.
# Make sure that the API scope is set to PAYMENT for the Payment API before 
# retrieving the App Key and App Secret.

# Enter the value from 'App Key' field obtained at developer.att.com 
# in your app account.
client_id = 'ENTER VALUE!'

# Enter the value from 'App Secret' field obtained at developer.att.com 
# in your app account.
client_secret = 'ENTER VALUE!'

# Set the fully-qualified domain name to: https://api.att.com
fqdn = 'https://api.att.com'

# Set up a client object for use with the Payment API.
client = Auth::Client.new(client_id, client_secret)

# Create the service for requesting an OAuth access token.
clientcred = Auth::ClientCred.new(fqdn, 
                                  client.id,
                                  client.secret)

# Get the OAuth access token using the API scope set to PAYMENT.
token = clientcred.createToken('PAYMENT')

# Create the service for interacting with the Payment API.
payment = PaymentService.new(fqdn, token, client)

# Specify how much the product costs.
AMOUNT = 1

# Specify a description of the item.
DESC = 'example game xtreme'

# Specify a unique merchant transaction ID.
MERCH_TRANS_ID = "example#{rand(1_000_000)}"

# Specify the merchant product ID.
MERCH_PROD_ID = 'exampleGame'

# Set the URI to redirect to after authorizing a payment.
REDIRECT = 'http://localhost:123123/auth'

# A consent flow is required to perform authentication for each purchase. 
# Completing the consent flow will charge the purchase to the phone used for 
# autenticateauthentication. 
puts payment.newTransaction(AMOUNT,
                            Categories::IN_APP_GAMES,
                            DESC,
                            MERCH_TRANS_ID,
                            MERCH_PROD_ID,
                            REDIRECT)

# Wait for user input after spawning Authorization flow.
puts
puts "Please input the TransactionAuthCode from the query "
puts "parameters after doing consent flow:"
code = gets.strip

# Get the transaction details.
begin
  
  # Obtain the status of the transaction using the OAuth authorization code.
  transaction = payment.getTransactionByAuthCode(code)

rescue Exception => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display all the values.
  transaction.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

puts

# Refund the test transaction that was just made.
begin
  reason_code = RefundCodes::CP_Other
  reason_text = "Testing out the Payment API"
  
  refund = payment.refundTransaction(transaction.id, 
                                     reason_code,
                                     reason_text)

rescue Exception => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display all of the values.
  refund.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

puts

MERCH_SUB_ID = "ExampleMerchSubID"
RECURRANCES = '99999'

# A consent flow is required to perform authentication for each subscription 
# purchase. Completing the consent flow will charge the purchase to the phone 
# used for authentication.
puts payment.newSubscription(AMOUNT,
                             Categories::IN_APP_GAMES,
                             DESC,
                             MERCH_TRANS_ID,
                             MERCH_PROD_ID,
                             MERCH_SUB_ID,
                             RECURRANCES,
                             REDIRECT)

# Wait for user input after spawning authorization flow.
puts "Please input the TransactionAuthCode from the query "
puts "parameters after doing consent flow:"
code = gets.strip

# Get the subscription status.
begin

  # Obtain the status using the OAuth authorization code.
  sub_status = payment.getSubscriptionByAuthCode(code)

rescue Exception => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display all of the values.
  sub_status.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

# Get the subscription details.
begin

  # Obtain the details using the consumer ID and merchant subscription ID.
  details = payment.getSubscriptionDetails(sub_status.consumer_id,
                                           sub_status.merchant_subscription_id)

rescue Exception => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display all of the values.
  details.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

# Cancel and refund the subscription.
begin

  reason_code = RefundCodes::Subscriber_Did_Not_Use
  reason_text = "This was an example payment subscription test"

  refund = payment.refundSubscription(sub_status.id, reason_code, reason_text)

rescue Exception => e

  # Display any error codes returned by the API Gateway.
  puts "There was an error, the API Gateway returned the following error code:"
  puts "#{e.message}"

else

  # Display all of the values.
  refund.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

#  vim: set ts=8 sw=2 tw=79 et :
