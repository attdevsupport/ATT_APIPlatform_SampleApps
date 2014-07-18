#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'json'
require 'cgi'
require 'base64'
require 'sinatra'
require 'sinatra/config_file'
require 'rexml/document'

# require as a gem file load relative if fails
begin
  require 'att/codekit'
rescue LoadError
  # try relative, fall back to ruby 1.8 method if fails
  begin
    require_relative 'codekit/lib/att/codekit'
  rescue NoMethodError 
    require File.join(File.dirname(__FILE__), 'codekit/lib/att/codekit')
  end
end

include Att::Codekit
include Att::Codekit::Service

#default sessions are too small, use pool 
use Rack::Session::Pool

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'PAYMENT'

#Setup proxy used by att/codekit
Transport.proxy(settings.proxy)

# Initialize our service object that we will use to make requests.
# Doing this globally with Rack spawners such as Shotgun will
# make this call on every request resulting in poor performance.
configure do
  @@client = Auth::Client.new(settings.api_key, 
                              settings.secret_key)

  OAuth = Auth::ClientCred.new(settings.FQDN, 
                               @@client.id,
                               @@client.secret)
  @@token = nil
end

#if we have a payload generate notary
before do
  @show_subscription = false
  @show_notary = false
  @show_transaction = false

  sign_payload session[:payload] if session[:payload]
  @show_notary = true if session[:payload]

  updateTransactions(settings.transactions_file, :transactions)
  updateTransactions(settings.subscriptions_file, :subscription)
  updateTransactions(settings.notifications_file, :notifications)

  begin
    if @@token.nil?
      @@token = OAuth.createToken(SCOPE)
    end

    if @@token.expired?
      @@token = OAuth.refreshToken(@@token) 
    end
  rescue Exception => e
    @auth_error = e.message
  end
end

[ '/newSubscription','/getSubscriptionStatus','/getSubscriptionDetails',
  '/refundSubscription','/cancelSubscription','/callbackSubscription' ].each do |path|
  before path do
    @show_subscription = true
  end
end

[ '/newTransaction', '/getTransactionStatus', '/refundTransaction',
  '/acknowledgeNotifications', '/returnTransaction'].each do |path|
  before path do
    @show_transaction = true
  end
end

before '/notary' do
  @show_notary = true
end

get '/' do
  erb :payment
end

# Single pay URL handlers
post '/newTransaction'  do
  service = Service::PaymentService.new(settings.FQDN, @@token, @@client)

  amount = params[:product] == "1" ? settings.min_transaction_value : settings.max_transaction_value

  description = 'Word game 1'
  merch_transaction_id = 'User' + sprintf('%03d', rand(1000)) + 'Transaction' + sprintf('%04d', rand(10000))

  merch_product_id = settings.merch_prod_id

  #generate a payload to populate the notary
  session[:payload] = generate_transaction_payload(
    amount, 
    Categories::IN_APP_GAMES,
    description,
    merch_transaction_id,
    merch_product_id,
    settings.payment_redirect_url)

  #redirect is required per purchase to authenticate the purchase by user
  redirect service.newTransaction(
    amount,
    Categories::IN_APP_GAMES,
    description,
    merch_transaction_id,
    merch_product_id,
    settings.payment_redirect_url)
end

# Returning from oauth flow to confirm purchase
# get our purchase object and store it
get '/returnTransaction' do
  if params['TransactionAuthCode']
    begin
      service = Service::PaymentService.new(settings.FQDN, @@token, @@client)

      response = service.getTransaction(TransactionType::TransactionAuthCode,
                                        params['TransactionAuthCode'])

      new_transaction = JSON.parse(response.to_json)
      new_transaction[TransactionType::TransactionAuthCode] =
        params['TransactionAuthCode']

      updateTransactions(settings.transactions_file, :transactions,
                         new_transaction, settings.recent_transactions_stored)

      @transaction_status = new_transaction

    rescue RestClient::Exception => e
      @new_transaction_error = e.response 
    rescue Exception => e
      @new_transaction_error = e.message
    end
  end
  erb :payment
end

post '/getTransactionStatus'  do
  begin
    service = Service::PaymentService.new(settings.FQDN, @@token, @@client)

    #we have three possible parameters, assign possible type
    response = service.getTransaction(
      TransactionType::TransactionAuthCode, 
      params['getTransactionAuthCode']) if params['getTransactionAuthCode']

    response ||= service.getTransaction(
      TransactionType::TransactionId, 
      params['getTransactionTID']) if params['getTransactionTID']

    response ||= service.getTransaction(
      TransactionType::MerchantTransactionId, 
      params['getTransactionMTID']) if params['getTransactionMTID']

    @transaction_status = JSON.parse(response.to_json)

  rescue RestClient::Exception => e
    @transaction_status_error = e.response 
  rescue Exception => e
    @transaction_status_error = e.message
  end
  erb :payment
end

post '/refundTransaction'  do
  begin
    service = Service::PaymentService.new(settings.FQDN, @@token, @@client)

    reason = "User was not happy with purchase"
    response = service.refundTransaction(params['refundTransactionId'],
                                         RefundCodes::CP_None, reason)

    response = service.getTransaction(TransactionType::TransactionId,
                                      params['refundTransactionId'])

    @refund = JSON.parse(response.to_json)

  rescue RestClient::Exception => e
    @refund_error = e.response 
  rescue Exception => e
    @refund_error = e.message
  end
  erb :payment
end

post '/notificationListener' do
  service = Service::PaymentService.new(settings.FQDN, @@token, @@client)

  input = request.env["rack.input"].read

  notification = Model::PaymentNotification.from_xml(input)
  updateTransactions(settings.notifications_file, :notifications, 
                     notification, settings.recent_notifications_stored)

  #return a string to satisfy the post request
  "success"
end

# Subscription URL handlers
post '/newSubscription' do
  service = Service::PaymentService.new(settings.FQDN, @@token, @@client)

  amount = params[:product] == "1" ? settings.min_subscription_value : settings.max_subscription_value
  description = 'Word game 1'
  merchant_product_id = settings.merch_prod_id
  sub_recurrances = '99999'

  merchant_transaction_id = 'User' + sprintf('%03d', rand(1000)) +
    'Subscription' + sprintf('%04d', rand(10000))

  merchant_subscription_id = sprintf('%10d', rand(1000)) #settings.merch_sub_id

  session[:payload] = generate_subscription_payload(
    amount, 
    Categories::IN_APP_GAMES,
    description,
    merchant_transaction_id,
    merchant_product_id,
    merchant_subscription_id,
    sub_recurrances,
    settings.subscription_redirect_url)

  redirect service.newSubscription(
    amount,
    Categories::IN_APP_GAMES,
    description,
    merchant_transaction_id, 
    merchant_product_id,
    merchant_subscription_id,
    sub_recurrances, 
    settings.subscription_redirect_url)
end

get '/callbackSubscription' do
  if params['SubscriptionAuthCode']
    begin
      service = Service::PaymentService.new(settings.FQDN, @@token, @@client)
      response = service.getSubscription(SubscriptionType::SubscriptionAuthCode,
                                         params['SubscriptionAuthCode'])

      new_subscription = JSON.parse(response.to_json)
      new_subscription[SubscriptionType::SubscriptionAuthCode] = params['SubscriptionAuthCode']

      updateTransactions(settings.subscriptions_file, :subscription,
                         new_subscription, settings.recent_transactions_stored)

      @subscription_status= new_subscription

    rescue RestClient::Exception => e
      @new_subscription_error = e.response 
    rescue Exception => e
      @new_subscription_error = e
    end
  end
  erb :payment
end

post '/getSubscriptionStatus' do
  begin
    service = Service::PaymentService.new(settings.FQDN, @@token, @@client)
    if params['getSubscriptionAuthCode'] then
      response = service.getSubscription(
        SubscriptionType::SubscriptionAuthCode, 
        params['getSubscriptionAuthCode']) 
    end

    if params['getSubscriptionTID'] then
      response ||= service.getSubscription(
        SubscriptionType::SubscriptionId, params['getSubscriptionTID']) 
    end

    if params['getSubscriptionMTID'] then
      response ||= service.getSubscription(
        SubscriptionType::MerchantTransactionId, params['getSubscriptionMTID']) 
    end

    @subscription_status = JSON.parse(response.to_json)

  rescue RestClient::Exception => e
    @subscription_status_error = e.response 
  rescue Exception => e
    @subscription_status_error = e
  end
  erb :payment
end

post '/getSubscriptionDetails' do
  service = Service::PaymentService.new(settings.FQDN, @@token, @@client)

  merch_id = params['getSDetailsMSID']

  begin
    consumer_id = ""
    session[:subscription].each do |sub|
      if sub['MerchantSubscriptionId'] == merch_id
        consumer_id = sub['ConsumerId']
        break
      end
    end

    response = service.getSubscriptionDetails(consumer_id, merch_id)

    @subscription_detail= JSON.parse(response.to_json)

  rescue RestClient::Exception => e
    @subscription_detail_error = e.response 
  rescue Exception => e
    @subscription_detail_error = e
  end
  erb :payment
end

post '/cancelSubscription' do
  begin
    service = Service::PaymentService.new(settings.FQDN, @@token, @@client)
    if params['cancelSubscriptionId']
      reason = "User was not happy with service"

      response = service.cancelSubscription(params['cancelSubscriptionId'],
                                            RefundCodes::CP_None, reason)

      @cancel_subscription = JSON.parse(response.to_json)
    end

  rescue RestClient::Exception => e
    @cancel_subscription_error = e.response 
  rescue Exception => e
    @cancel_subscription_error = e
  end
  erb :payment
end

post '/refundSubscription' do
  begin
    service = Service::PaymentService.new(settings.FQDN, @@token, @@client)
    if params['refundSubscriptionId']
      reason = "User was not happy with service"

      response = service.refundSubscription(params['refundSubscriptionId'],
                                            RefundCodes::CP_None, reason)

      @refund_subscription = JSON.parse(response.to_json)
    end

  rescue RestClient::Exception => e
    @refund_subscription_error = e.response 
  rescue Exception => e
    @refund_subscription_error = e
  end
  erb :payment
end

post '/refreshNotifications' do
  @show_notifications = true
  erb :payment
end

post '/notary' do
  sign_payload params['payload']
  erb :payment
end

def sign_payload(payload)
  begin
    service = Service::PaymentService.new(settings.FQDN, @@token, @@client)
    response = service.signPayload(payload)

    from_json = JSON.parse(response)

    @payload = payload
    @signed_doc = from_json['SignedDocument']
    @signature = from_json['Signature']

  rescue RestClient::Exception => e
    @notary_error = e.response 
  rescue Exception => e
    @notary_error = e
  end
end

# Using file locking to update one at a time
#
# @param file [String] path/name of the file to save to
# @param type [Symbol] the type we want to save :transactions, :subscription or 
#   :notifications
# @param newdata [Hash] a transaction or subscription descriptor
# @param store_amount [Integer] the amount of transactions to store
def updateTransactions(file, type, newdata=nil, store_amount=5)
  File.open(type.to_s + "_lock", File::CREAT|File::RDONLY) do |lock|
    begin
      lock.flock(File::LOCK_EX)
      File.open(file, File::RDONLY|File::CREAT) do |read|
        session[type] = Array.new

        read.each do |line|
          obj = JSON.load(line)
          session[type].push(obj) 
        end

        if newdata
          session[type].push newdata unless session[type].include? newdata
        end
      end

      #only store x amount of transactions 
      session[type].delete_at 0 if session[type].length > store_amount

      File.open(file, 'w') do |out|
        session[type].each do |obj|
          out.puts(obj.to_json)
        end
      end

    ensure
      lock.flock(File::LOCK_UN)
    end
  end
end

#used to populate notary, service takes care of this.
def generate_transaction_payload(amount, category, desc, merch_trans_id, 
                                 merch_prod_id, redirect_uri, opts={})
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

#used to populate notary, service takes care of this.
def generate_subscription_payload(amount, category, desc, merch_trans_id, 
                                  merch_prod_id, merch_sub_id, 
                                  sub_recurrances, redirect_uri, opts={})
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
    :MerchantSubscriptionIdList => merch_sub_id,
    :SubscriptionRecurrences => sub_recurrances,
    :MerchantPaymentRedirectUrl => redirect_uri,
    :SubscriptionPeriodAmount => sub_period_amount,
    :SubscriptionPeriod => sub_period,
    :IsPurchaseOnNoActiveSubscription => is_purchase_on_no_active_sub,
    :Channel => channel,
  }.to_json
end

def raw(string)
    CGI.unescapeHTML(string)
end
