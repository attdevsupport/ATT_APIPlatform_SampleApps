#!/usr/bin/env ruby
# This quickstart guide requires the Ruby codekit, which can be found at:
# https://github.com/attdevsupport/codekit-ruby

# Make sure the att-codekit has been installed then require the class
require 'att/codekit'

# Include the name spaces to reduce the code required (Optional)
include Att::Codekit
include Att::Codekit::Service

# Uncomment to set a proxy if required
# Transport.proxy("http://proxyaddress.com:port")

# Use the application settings from developer.att.com for the following values.
# Make sure Payment is enabled for the App key and App Secret.

# Enter the value from 'App Key' field
client_id = 'ENTER VALUE!'

# Enter the value from 'App Secret' field
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
payment = PaymentService.new(fqdn, token, client)

# Define how much the product costs 
AMOUNT = 1

# Give the item a description
DESC = 'example game xtreme'

# Define a uniq merchant transaction id
MERCH_TRANS_ID = "example#{rand(1_000_000)}"

# Set the merchant product id
MERCH_PROD_ID = 'exampleGame'

# Set the url to redirect to after authorizing a payment
REDIRECT = 'http://localhost:123123/auth'

# Consent Flow is required per purchase to authenticate the purchase by user
# Note: Completing consent flow will charge the phone used to autenticate
puts payment.newTransaction(AMOUNT,
                            Categories::IN_APP_GAMES,
                            DESC,
                            MERCH_TRANS_ID,
                            MERCH_PROD_ID,
                            REDIRECT)

# Wait for user input after spawning auth flow
puts
puts "Please input the TransactionAuthCode from the query "
puts "parameters after doing consent flow:"
code = gets.strip

# Get the transaction details
begin
  
  # Obtain the status of the transaction via authentication code
  transaction = payment.getTransactionByAuthCode(code)

rescue Exception => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simply display all the values
  transaction.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

puts

# Refund the transaction made
begin
  reason_code = RefundCodes::CP_Other
  reason_text = "Testing out the Payment API"
  
  refund = payment.refundTransaction(transaction.id, 
                                     reason_code,
                                     reason_text)

rescue Exception => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simply display all the values
  refund.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

puts

MERCH_SUB_ID = "ExampleMerchSubID"
RECURRANCES = '99999'

# Consent Flow is required per purchase to authenticate the purchase by user
# Note: Completing consent flow will charge the phone used to autenticate
puts payment.newSubscription(AMOUNT,
                             Categories::IN_APP_GAMES,
                             DESC,
                             MERCH_TRANS_ID,
                             MERCH_PROD_ID,
                             MERCH_SUB_ID,
                             RECURRANCES,
                             REDIRECT)

# Wait for user input after spawning auth flow
puts "Please input the TransactionAuthCode from the query "
puts "parameters after doing consent flow:"
code = gets.strip

# Get the subscription status
begin

  # Obtain the status via authentication code
  sub_status = payment.getSubscriptionByAuthCode(code)

rescue Exception => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simply display all the values
  sub_status.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

# Get the subscription details
begin

  # Obtain the details via consumer id and merchant subscription id
  details = payment.getSubscriptionDetails(sub_status.consumer_id,
                                           sub_status.merchant_subscription_id)

rescue Exception => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simply display all the values
  details.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

# Cancel and refund the subscription
begin

  reason_code = RefundCodes::Subscriber_Did_Not_Use
  reason_text = "This was an example payment subscription test"

  refund = payment.refundSubscription(sub_status.id, reason_code, reason_text)

rescue Exception => e

  # There was an error in execution print what happened
  puts "There was an error, the api returned the following error code:"
  puts "#{e.message}"

else

  # Simply display all the values
  refund.each_pair do |key, value|
    puts "#{key}: #{value}"
  end

end

#  vim: set ts=8 sw=2 tw=79 et :
