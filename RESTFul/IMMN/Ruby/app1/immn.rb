#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'rubygems'
require 'json'
require 'base64'
require 'rest_client'
require 'sinatra'
require 'sinatra/config_file'
require 'att/codekit'

include Att::Codekit

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

ATTACH_DIR = File.join(File.dirname(__FILE__), 'public/attachments')

RestClient.proxy = settings.proxy

SCOPE = ["IMMN", "MIM"]

configure do
  OAuth = Auth::AuthCode.new(settings.FQDN,
                             settings.api_key,
                             settings.secret_key,
                             SCOPE,
                             settings.redirect_url)
end

before do
  load_attachments
  session[:immn] = Service::IMMNService.new(OAuth) if session[:immn].nil?
  session[:mim] = Service::MIMService.new(OAuth) if session[:mim].nil?
end

get '/' do
  begin
    #if code is present in params then we're returning from authentication
    session[:immn].updateAccessToken(params[:code]) unless params[:code].nil?
    session[:mim].updateAccessToken(params[:code]) unless params[:code].nil?

    if (session[:immn].authenticated? || session[:mim].authenticated?)
      if session[:sending] then
        sendMessage(session[:address], session[:subject], session[:message], session[:attachment], session[:groupCheckBox], true)
      elsif session[:getting] then
        getMessageHeaders(session[:headerCountTextBox], session[:indexCursorTextBox], true)
      elsif session[:getting_content] then
        getMessageContent(session[:MessageId], session[:PartNumber], true)
      end
    end

  rescue RestClient::Exception => e
    @send_error = e.response 
  rescue Exception => e
    @send_error = e.message
  end
  erb :immn
end

#send message
post '/submit' do
  begin
    # if auth'd then send the message
    if session[:immn].authenticated? then
      sendMessage(params[:address], params[:subject], params[:message], params[:attachment], params[:groupCheckBox])
      # otherwise save our session and do consent flow
    else
      storeSendingParams
      redirect session[:immn].consentFlow
    end
  rescue => e
    @send_error = e.message
  end
end

post '/submitGetHeaders' do
  begin
    if session[:mim].authenticated?
      getMessageHeaders(params[:headerCountTextBox], params[:indexCursorTextBox])
    else
      storeGetHeadersParams
      redirect session[:mim].consentFlow
    end
  rescue RestClient::Exception => e
    @get_error = e.response 
  rescue => e
    @get_error = e.message
  end
  erb :immn
end

post '/submitGetHeaderContent' do
  begin
    if session[:mim].authenticated?
      getMessageContent(params[:MessageId], params[:PartNumber])
    else
      storeGetHeaderContentParams
      redirect session[:mim].consentFlow
    end

  rescue RestClient::Exception => e
    @get_error = e.response 
  rescue Exception => e
    @get_error = e.message
  end
  erb :immn
end

# Takes care of parsing our send responses
def sendMessage(address, subject, msg, attachment, group, clear_requested=false)
  begin
    clearSendingParams if clear_requested

    #if we don't have an attachment selected make it nil
    if attachment == "None"
      attachment = nil 
    else
      attachment = File.join(ATTACH_DIR, attachment).to_s
    end

    response = session[:immn].sendMessage(address, subject, msg, attachment, group)

    @send_result = JSON.parse(response)["Id"]

  rescue RestClient::Exception => e
    @send_error = e.response 
  rescue => e
    @send_error = e.message
  end
  erb :immn
end

# perform the API call for getting message headers
def getMessageHeaders(count, index, clear_requested=false)
  begin
    clearGetHeadersParams if clear_requested
    response = session[:mim].getMessageHeaders(count, index)

    @get_result = JSON.parse(response)

  rescue RestClient::Exception => e
    @get_error = e.response 
  rescue => e
    @get_error = e.message
  end
  erb :immn
end

def getMessageContent(message_id, part_number, clear_requested=false)
  begin
    clearGetHeaderContentParams if clear_requested
    response = session[:mim].getMessageContent(message_id, part_number)

    @content_result = response

    headers = response.headers[:content_type]
    content_string = headers.split("; ")
    @image_string = headers.split("/")
    @image = @image_string[0]
    @image_content = content_string[0]

  rescue RestClient::Exception => e
    @get_error = e.response 
  rescue => e
    @get_error = e.message
  end
  erb :immn
end

#load all attachments present
def load_attachments
  @attachments = Array.new
  Dir.entries(ATTACH_DIR).sort.each do |x|
    #add file unless it starts with a . or more
    @attachments.push x unless x.match /\A\.+/
  end
end

def storeSendingParams
  session[:address] = params[:address] if params[:address]
  session[:message] = params[:message] if params[:message]
  session[:subject] = params[:subject] if params[:subject]
  session[:attachment] = params[:attachment] if params[:attachment]
  session[:groupCheckBox] = params[:groupCheckBox] if params[:groupCheckBox]
  session[:sending] = true
end

def clearSendingParams
  session[:address] = nil
  session[:message] = nil
  session[:subject] = nil
  session[:attachment] = nil
  session[:sending] = false
end

def storeGetHeadersParams
  session[:getting] = true
  session[:headerCountTextBox] = params[:headerCountTextBox]
  session[:indexCursorTextBox] = params[:indexCursorTextBox]
end

def clearGetHeadersParams
  session[:getting] = false
  session[:headerCountTextBox] = nil
  session[:indexCursorTextBox] = nil
end

def storeGetHeaderContentParams
  session[:getting_content] = true
  session[:MessageId] = params[:MessageId]
  session[:PartNumber] = params[:PartNumber]
end

def clearGetHeaderContentParams
  session[:getting_content] = false
  session[:MessageId] = nil
  session[:PartNumber] = nil
end

