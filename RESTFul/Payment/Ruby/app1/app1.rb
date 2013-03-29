#!/usr/bin/ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

require 'rubygems'
require 'json'
require 'base64'
require 'sinatra'
require 'sinatra/config_file'
require 'att_payment_service'
#require File.join(File.dirname(__FILE__), 'common.rb')

use Rack::Session::Pool

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'PAYMENT'
RestClient.proxy = settings.proxy

# Include the payment service namespace
include AttCloudServices::Payment

# Initialize our service object that we will use to make requests.
# Doing this globally with Rack spawners such as Shotgun will
# make this call on every request resulting in poor performance.
configure do
  begin
    Service = PaymentService.new(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.payment_redirect_url, :tokens_file => settings.tokens_file)
  rescue => e
    @auth_error = e.message
    puts e
  end
end

['/newSubscription','/getSubscriptionStatus','/getSubscriptionDetails','/refundSubscription','/cancelSubscription','/callbackSubscription', '/refreshSubNotifications'].each do |path|
  before path do
    @show_subscription = true
  end
end

['/newTransaction', '/getTransactionStatus', '/refundTransaction','/acknowledgeNotifications', '/returnTransaction', '/refreshTransNotifications'].each do |path|
  before path do
    @show_transaction = true
  end
end

before '/notary' do
  @show_notary = true
end

before '/' do
  @show_subscription = false
  @show_notary = false
  @show_transaction = false
end

#if we have a payload generate notary
before '*' do
  sign_payload session[:payload] if session[:payload]
  @show_notary = true if session[:payload]
end

get '/' do
  updateTransactions(settings.transactions_file, :transactions)
  updateTransactions(settings.subscriptions_file, :subscription)
  erb :payment
end

# Single pay URL handlers
post '/newTransaction'  do
  amount = params[:product] == "1" ? settings.min_transaction_value : settings.max_transaction_value
  description = 'Word game 1'
  merch_transaction_id = 'User' + sprintf('%03d', rand(1000)) + 'Transaction' + sprintf('%04d', rand(10000))
  merch_product_id = 'wordGame1'
        
  #generate a payload to populate the notary
  session[:payload] = generate_transaction_payload(amount, Categories::IN_APP_GAMES, description, merch_transaction_id, merch_product_id,
                                       :redirect_uri => settings.payment_redirect_url)

  #redirect is required per purchase to authenticate the purchase by user
  redirect Service.newTransaction(amount, Categories::IN_APP_GAMES, description, merch_transaction_id, merch_product_id, 
                                  :redirect_uri => settings.payment_redirect_url)
end

# Returning from oauth flow to confirm purchase
# get our purchase object and store it
get '/returnTransaction' do
  if params['TransactionAuthCode']
    begin
      response = Service.getTransaction(TransactionType::TransactionAuthCode, params['TransactionAuthCode'])
      new_transaction = JSON.parse response
      new_transaction[TransactionType::TransactionAuthCode] = params['TransactionAuthCode']

      updateTransactions(settings.transactions_file, :transactions, new_transaction)

      @transaction_status = new_transaction

    rescue => e
      @new_transaction_error = e.message
    end
  end
  erb :payment
end

post '/getTransactionStatus'  do
  begin
    #we have three possible parameters, assign possible type
    response = Service.getTransaction(TransactionType::TransactionAuthCode, params['getTransactionAuthCode']) if params['getTransactionAuthCode']
    response ||= Service.getTransaction(TransactionType::TransactionId, params['getTransactionTID']) if params['getTransactionTID']
    response ||= Service.getTransaction(TransactionType::MerchantTransactionId, params['getTransactionMTID']) if params['getTransactionMTID']

    @transaction_status = JSON.parse response

  rescue => e
    @transaction_status_error = e.message
  end

  erb :payment
end

post '/refundTransaction'  do
  begin
    reason = "User was not happy with purchase"
    response = Service.refundTransaction(params['refundTransactionId'], RefundCodes::CP_None, reason)
    @refund = JSON.parse Service.getTransaction(TransactionType::TransactionId, params['refundTransactionId'])

  rescue => e
    @refund_error = e.message
  end

  erb :payment
end

post '/transactionListener'  do
  erb :payment
end

get '/acknowledgeNotifications' do
  erb :payment
end

# Subscription URL handlers
post '/newSubscription' do
  amount = params[:product] == "1" ? settings.min_subscription_value : settings.max_subscription_value
  description = 'Word game 1'
  merchant_product_id = 'wordGame1'
  sub_recurrances = '99999'
  merchant_transaction_id = 'User' + sprintf('%03d', rand(1000)) + 'Subscription' + sprintf('%04d', rand(10000))
  merchant_subscription_id_list = 'MSList' + sprintf('%04d', rand(10000))
  
  session[:payload] = generate_subscription_payload(amount, Categories::IN_APP_GAMES, description, merchant_transaction_id, 
                                   merchant_product_id, merchant_subscription_id_list, sub_recurrances, 
                                   :redirect_uri => settings.subscription_redirect_url)

  redirect Service.newSubscription(amount, Categories::IN_APP_GAMES, description, merchant_transaction_id, 
                                   merchant_product_id, merchant_subscription_id_list, sub_recurrances, 
                                   :redirect_uri => settings.subscription_redirect_url)
end

get '/callbackSubscription' do
  if params['SubscriptionAuthCode']
    begin
      response = Service.getSubscription(SubscriptionType::SubscriptionAuthCode, params['SubscriptionAuthCode'])
      new_subscription = JSON.parse response
      new_subscription[SubscriptionType::SubscriptionAuthCode] = params['SubscriptionAuthCode']

      updateTransactions(settings.subscriptions_file, :subscription, new_subscription)

      @subscription_status= new_subscription

    rescue => e
      @new_subscription_error = e
    end
  end
  erb :payment
end

post '/getSubscriptionStatus' do
  begin
    response = Service.getSubscription(SubscriptionType::SubscriptionAuthCode, params['getSubscriptionAuthCode']) if params['getSubscriptionAuthCode']
    response ||= Service.getSubscription(SubscriptionType::SubscriptionId, params['getSubscriptionTID']) if params['getSubscriptionTID']
    response ||= Service.getSubscription(SubscriptionType::MerchantTransactionId, params['getSubscriptionMTID']) if params['getSubscriptionMTID']

    @subscription_status = JSON.parse response

  rescue => e
    @subscription_status_error = e
  end

  erb :payment
end

post '/getSubscriptionDetails' do
  consumer_id = params['getSDetailsConsumerId']
  merch_id = params['getSDetailsMSID']

  if consumer_id then
    session[:subscription].each do |sub|
      if sub['ConsumerId'] == consumer_id
        merch_id = sub['MerchantSubscriptionId']
        break
      end
    end
  elsif merch_id 
    session[:subscription].each do |sub|
      if sub['MerchantSubscriptionId'] == merch_id
        consumer_id = sub['ConsumerId']
        break
      end
    end
  end

  begin
    response = Service.getSubscriptionDetails(consumer_id, merch_id)

    @subscription_detail= JSON.parse response

  rescue => e
    @subscription_detail_error = e
  end

  erb :payment
end

post '/cancelSubscription' do
  begin
    if params['cancelSubscriptionId']
      reason = "User was not happy with service"
      response = Service.cancelSubscription(params['cancelSubscriptionId'], RefundCodes::CP_None, reason)
      @cancel_subscription = JSON.parse response
    end
  rescue => e
    @cancel_subscription_error = e
  end
  erb :payment
end

post '/refundSubscription' do
  begin
    if params['refundSubscriptionId']
      reason = "User was not happy with service"
      response = Service.refundSubscription(params['refundSubscriptionId'], RefundCodes::CP_None, reason)
      @refund_subscription = JSON.parse response
    end
  rescue => e
    @refund_subscription_error = e
  end
  erb :payment
end

get '/acknowledgeNotifications' do
  erb :payment
end

post '/refreshTransNotifications' do
  erb :payment
end

post '/refreshSubNotifications' do
  erb :payment
end

post '/subscriptionListener'  do
  erb :payment
end

post '/notary' do
  sign_payload params['payload']
  erb :payment
end

def sign_payload(payload)
  begin
    response = Service.signPayload(payload)

    from_json = JSON.parse response

    @payload = payload
    @signed_doc = from_json['SignedDocument']
    @signature = from_json['Signature']

  rescue => e
    @notary_error = e
  end
end

# Using file locking to update one at a time
#
#@param file [String] path/name of the file to save to
#@param type [Symbol] the type we want to save :transactions or :subscription
#@param new [Hash] a transaction or subscription descriptor
def updateTransactions(file, type, new=nil)
  File.open(type.to_s + "_lock", File::CREAT|File::RDONLY) do |lock|
    begin
      lock.flock(File::LOCK_EX)
      File.open(file, File::RDONLY|File::CREAT) do |read|
        session[type] = Array.new

        read.each do |line|
          session[type].push JSON.parse line
        end

        if new
          session[type].push new unless session[type].include? new
        end
      end

      #only store x amount of transactions (set in config)
      session[type].delete_at 0 if session[type].length > settings.recent_transactions_stored

      File.open(file, 'w') do |out|
        session[type].each do |array|
          out.puts JSON.generate(array)
        end
      end

    ensure
      lock.flock(File::LOCK_UN)
    end
  end
end

def generate_transaction_payload(amount, category, desc, merch_trans_id, merch_prod_id, opts={})
  redirect_uri = (opts[:redirect_uri] || @redirect_uri)
  channel = (opts[:channel] || "MOBILE_WEB")

  payload = {
    :Amount => amount,
    :Category => category.to_i,
    :Description => desc,
    :MerchantTransactionId => merch_trans_id,
    :MerchantProductId => merch_prod_id,
    :MerchantPaymentRedirectUrl => redirect_uri,
    :Channel => channel,
  }.to_json
end

def generate_subscription_payload(amount, category, desc, merch_trans_id, merch_prod_id, merch_sub_id_list, sub_recurrances, opts={})
  redirect_uri = (opts[:redirect_uri] || @redirect_uri)
  sub_period_amount = (opts[:sub_period_amount] || 1) 
  sub_period = (opts[:sub_period] || 'MONTHLY')
  is_purchase_on_no_active_sub = (opts[:iponas] || false)
  channel = (opts[:channel] || "MOBILE_WEB")

  payload = {
    :Amount => amount,
    :Category => category,
    :Description => desc,
    :MerchantTransactionId => merch_trans_id,
    :MerchantProductId => merch_prod_id,
    :MerchantSubscriptionIdList => merch_sub_id_list,
    :SubscriptionRecurrences => sub_recurrances,
    :MerchantPaymentRedirectUrl => redirect_uri,
    :SubscriptionPeriodAmount => sub_period_amount,
    :SubscriptionPeriod => sub_period,
    :IsPurchaseOnNoActiveSubscription => is_purchase_on_no_active_sub,
    :Channel => channel,
  }.to_json
end
