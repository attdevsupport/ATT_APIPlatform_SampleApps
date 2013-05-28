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
require 'att_sms_service'

include AttCloudServices

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

RestClient.proxy = settings.proxy

configure do
  Service = SMS::SmsService.new(settings.FQDN, 
                                settings.api_key,
                                settings.secret_key,
                                SMS::SCOPE,
                                settings.tokens_file)
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
  begin
    session[:sms1_address] = params[:address]
    #set true if  we're sending a notification 
    if params[:chkGetOnlineStatus].nil?
      notify = false
    else
      notify = true
    end

    response = Service.sendSms(session[:sms1_address], params[:message], notify)

    json = JSON.parse(response)

    @sms_id =  json['outboundSMSResponse']['messageId']
    @resource_url = json['outboundSMSResponse']['resourceReference']['resourceURL'] 
    session[:sms_id] = @sms_id unless notify
  rescue => e
    @send_error = e.response
  ensure
    return erb :sms
  end
end

post '/getDeliveryStatus' do
  session[:sms_id] = params["messageId"]

  begin
    response = Service.getDeliveryStatus(session[:sms_id])
    @resource_url = Service.getResourceUrl(session[:sms_id])

    delivery_info_list = JSON.parse(response)['DeliveryInfoList']
    @delivery_info = delivery_info_list['DeliveryInfo']

  rescue => e
    @delivery_error = e.response
  ensure
    return erb :sms
  end
end

post '/getReceivedSms' do
  begin
    response = Service.getReceivedMessages(settings.short_code_1)

    @message_list = JSON.parse(response).fetch 'InboundSmsMessageList'

    @messages_in_batch = @message_list['NumberOfMessagesInThisBatch']
    @messages_pending  = @message_list['TotalNumberOfPendingMessages']
    @messages_inbound  = @message_list['InboundSmsMessage']

  rescue => e
    @received_error = e.response
  ensure
    return erb :sms
  end
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
