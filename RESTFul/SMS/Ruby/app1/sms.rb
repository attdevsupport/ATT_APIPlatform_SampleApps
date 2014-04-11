#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2014 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

require 'sinatra'
require 'sinatra/config_file'

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

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = "SMS"

RestClient.proxy = settings.proxy

configure do
  FILE_SUPPORT = (settings.tokens_file && !settings.tokens_file.strip.empty?)
  FILE_EXISTS = FILE_SUPPORT && File.file?(settings.tokens_file)

  OAuth = Auth::ClientCred.new(settings.FQDN, 
                               settings.api_key,
                               settings.secret_key)
  @@token = nil
end

#update listeners data before every request
before do 
  begin
    @status_listener = load_file "#{settings.status_file}"
    @message_listener = load_file "#{settings.message_file}"

    #check if token exists and create if necessary
    if @@token.nil?
      if FILE_EXISTS 
        @@token = Auth::OAuthToken.load(settings.tokens_file)
      else
        @@token = OAuth.createToken(SCOPE)
      end
      Auth::OAuthToken.save(settings.tokens_file, @@token) if FILE_SUPPORT
    end

    if @@token.expired?
      @@token = OAuth.refreshToken(@@token)
      Auth::OAuthToken.save(settings.tokens_file, @@token) if FILE_SUPPORT
    end

  rescue Exception => e
    @send_error = e.message
  end
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
    service = Service::SMSService.new(settings.FQDN, @@token)

    session[:sms1_address] = params[:address]
    #set true if  we're sending a notification 
    if params[:chkGetOnlineStatus].nil?
      notify = false
    else
      notify = true
    end

    @send = service.sendSms(session[:sms1_address], params[:message], notify)

    session[:sms_id] = @send.id unless notify

  rescue Exception => e
    @send_error = e.message
  end
  erb :sms
end

post '/getDeliveryStatus' do
  session[:sms_id] = params["messageId"]

  service = Service::SMSService.new(settings.FQDN, @@token)
  begin
    @status = service.getDeliveryStatus(session[:sms_id])
    @status_resource_url = service.getResourceUrl(session[:sms_id])

  rescue Exception => e
    @delivery_error = e.message
  end
  erb :sms
end

post '/getReceivedSms' do
  begin
    service = Service::SMSService.new(settings.FQDN, @@token)
    @messages = service.getReceivedMessages(settings.short_code_1)

  rescue Exception => e
    @received_error = e.message
  end
  erb :sms
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
