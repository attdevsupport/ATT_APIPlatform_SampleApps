#!/usr/bin/ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

require 'rubygems'
require 'json'
require 'rest_client'
require 'sinatra'
require 'sinatra/config_file'
require File.join(File.dirname(__FILE__), 'common.rb')

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'SMS'
RestClient.proxy = settings.proxy

# setup filter fired before reaching our urls
# this is to ensure we are o-authenticated before actual action (like sendSms)

# autonomous version

['/sendSms', '/getDeliveryStatus', '/getReceivedSms'].each do |path|
  before path do
    obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
  end
end

#update listeners data before every request
before "/*" do |path|
  @status_listener = load_file "#{settings.status_file}"
  @message_listener = load_file "#{settings.message_file}"
end

get '/' do
  session[:sms_id] = nil

  if !File.directory? settings.mosms_file_dir then
    Dir.mkdir settings.mosms_file_dir
  end

  erb :sms
end

post '/sendSms' do
  session[:sms1_address] = params[:address]
  send_sms
end

post '/getDeliveryStatus' do
  session[:sms_id] = params["messageId"]
  get_delivery_status params["messageId"]
end

post '/getReceivedSms' do
  get_received_sms
end

post '/statusListener' do
  handle_inbound "#{settings.status_file}"
end

post '/messageListener' do
  handle_inbound "#{settings.message_file}"
end

get '/refreshStatus' do
  erb :sms
end

get '/receiveMessages' do
  erb :sms
end

# use this URL to clear files
get '/clear' do
  File.delete settings.tokens_file if File.exists? settings.tokens_file
  File.delete settings.status_file if File.exists? settings.status_file
  File.delete settings.message_file if File.exists? settings.message_file
  redirect '/'
end

def send_sms
  address = session[:sms1_address].gsub("-","").split(",")
  addresses = Array.new

  address.each do |a|
    addresses << 'tel:'  + a.strip unless(a.include? "tel:" or a.include? "@")
  end

  # send in array if more than one otherwise string
  if addresses.size > 1
    address = addresses
  else
    address = addresses[0]
  end

  #set true if  we're sending a notification 
  if params[:chkGetOnlineStatus].nil?
    doNotify = false
  else
    doNotify = true
  end

  payload = {
    :outboundSMSRequest => { :address => address, :message => params[:message], :notifyDeliveryStatus => doNotify.to_s }
  }.to_json

  response = RestClient.post "#{settings.FQDN}/sms/v3/messaging/outbox",  payload, :Content_Type => "application/json",
    :Accept => "application/json", :Authorization => "Bearer #{@access_token}"

  json = JSON.parse(response)

  @sms_id =  json['outboundSMSResponse']['messageId']
  @resource_url = json['outboundSMSResponse']['resourceReference']['resourceURL'] 
  session[:sms_id] = @sms_id unless doNotify
rescue => e
  @send_error = e.response
ensure
  return erb :sms
end

def get_delivery_status(sms_id)
  url = "#{settings.FQDN}/sms/v3/messaging/outbox/#{sms_id}"
  response = RestClient.get url,
    :Authorization => "Bearer #{@access_token}", :Accept => "application/json"

  delivery_info_list = JSON.parse(response)['DeliveryInfoList']
  @delivery_info = delivery_info_list['DeliveryInfo']

rescue => e
  @delivery_error = e.response
ensure
  return erb :sms
end

def get_received_sms
  response = RestClient.get "#{settings.FQDN}/sms/v3/messaging/inbox/#{settings.short_code_1}", 
  :Authorization => "Bearer #{@access_token}", :Accept => "application/json"

  @message_list = JSON.parse(response).fetch 'InboundSmsMessageList'

  @messages_in_batch = @message_list['NumberOfMessagesInThisBatch']
  @messages_pending  = @message_list['TotalNumberOfPendingMessages']
  @messages_inbound  = @message_list['InboundSmsMessage']

rescue => e
  @received_error = e.response
ensure
  return erb :sms
end

def load_file(file)
  data = Array.new
  if File.exists? file then
    File.open(file, 'r') do |f|
      begin
        f.flock(File::LOCK_EX)
        f.each_line {|line| data << JSON.parse(line)}
      ensure
        f.flock(File::LOCK_UN)
      end
    end
  end
  data
end

def handle_inbound(file)
  input = request.env["rack.input"].read

  if !File.exists? file
    File.new(file, 'w')
  end

  contents = File.readlines(file)

  File.open(file, 'w') do |f|
    begin
      f.flock(File::LOCK_EX)
      #remove the first line if we're over limit
      if contents.size > settings.listener_limit - 1 
        offset = 1
      else
        offset = 0
      end
      f.puts contents[offset, contents.size] 
      f.puts JSON.parse(input).to_json
    ensure
      f.flock(File::LOCK_UN)
    end
  end
end
