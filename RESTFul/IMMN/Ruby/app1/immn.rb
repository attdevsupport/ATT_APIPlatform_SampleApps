#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'base64'
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

#regular sessions cause a problem with chrome
use Rack::Session::Pool

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

ATTACH_DIR = File.join(File.dirname(__FILE__), 'public/attachments')

#Setup proxy used by att/codekit
Transport.proxy(settings.proxy)

SCOPE = ["IMMN", "MIM"]

configure do
  PARAMS = [ :address, :message, :subject, :attachment, :groupCheckBox, 
             :favorite, :state, :incoming, :unread, :keyword, :messageId, 
             :partNumber, :readflag, :queues ]

  MSG_LIST_COUNT = 5

  OAuth = Auth::AuthCode.new(settings.FQDN,
                             settings.api_key,
                             settings.secret_key)
end

# Setup filters for saving session, 
# loading select box data and catching the oauth redirect code
before do
  save_session unless session[:consenting]
  load_attachments


  begin
    if params[:code] && session[:token].nil?
      session[:token] = OAuth.createToken(params[:code])
    end
    if session[:token] && session[:token].expired?
      session[:token] = OAuth.refreshToken(session[:token])
    end
  rescue Exception => e
    @oauth_error = e.message
  end
end

# Setup filter for clearing session after service calls are made.
# This is required to prevent populating the form inputs of different forms.
after do
  clear_session unless session[:consenting]
end

# Setup filter for catching authentication 
# only on actions that require authentication
['/sendMessage', 
 '/createMessageIndex',
 '/getMessageList', 
 '/getMessage', 
 '/getMessageContent', 
 '/getDelta', 
 '/getMessageIndexInfo',
 '/updateMessage',
 '/deleteMessage',
 '/getNotifyDetails' 
].each do |action|
  before action do
    if session[:token].nil?
      #remove the leading / from action if redirect ends with /
      suburl = settings.redirect_url.end_with?("/") ? action[1..-1] : action

      session[:consenting] = true
      redirect_url = "#{settings.redirect_url}#{suburl}"
      redirect OAuth.consentFlow(:scope => SCOPE, :redirect => redirect_url) 
    else
      session[:consenting] = false
    end
  end
end

get '/' do
  if params[:error]
    @send_error = params[:error]
  end
  erb :immn
end

get '/sendMessage' do send_message; end
post '/sendMessage' do send_message; end
def send_message
  begin
    service = Service::IMMNService.new(settings.FQDN, session[:token])

    if session[:attachment] 
      attachment  = File.join(ATTACH_DIR, session[:attachment])
      unless File.file? attachment
        attachment = nil
      end
    end

    # Sending in all optional parameters, service takes care of nil items
    @send_message = service.sendMessage(session[:address], 
                                        :message => session[:message],
                                        :subject => session[:subject],
                                        :attachments => attachment,
                                        :group => session[:groupCheckBox])
  rescue Exception => e
    @send_message_error = e.message
  ensure
    session[:toggle_div] = :sendMsg
  end
  erb :immn
end

get '/createMessageIndex' do create_message_index; end
post '/createMessageIndex' do create_message_index; end
def create_message_index
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])

    @create_index = service.createIndex

  rescue Exception => e
    @create_index_error = e.message
  ensure
    session[:toggle_div] = :createMsg
  end
  erb :immn
end

get '/getMessageList' do get_message_list; end
post '/getMessageList' do get_message_list; end
def get_message_list
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])
    
    unread = true if session[:unread] 
    fav = true if session[:favorite] 
    inc = true if session[:incoming] 

    @msg_list = service.getMessageList(MSG_LIST_COUNT,
                                       :isUnread => unread,
                                       :isFavorite => fav,
                                       :isIncoming => inc,
                                       :keyword => session[:keyword])
  rescue Exception => e
    @msg_list_error = e.message
  ensure
    session[:toggle_div] = :getMsg
  end
  erb :immn
end

get '/getMessage' do get_message; end
post '/getMessage' do get_message; end
def get_message
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])

    @get_msg = service.getMessage(session[:messageId])

  rescue Exception => e
    @get_msg_error = e.message
  ensure
    session[:toggle_div] = :getMsg
  end
  erb :immn
end

get '/getMessageContent' do get_message_content; end
post '/getMessageContent' do get_message_content; end
def get_message_content
  # make sure our required arguments are present
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])

    @msg_content = service.getMessageContent(session[:messageId], 
                                             session[:partNumber])

  rescue Exception => e
    @msg_content_error = e.message
  ensure
    session[:toggle_div] = :getMsg
  end
  erb :immn
end

get '/getDelta' do get_delta; end
post '/getDelta' do get_delta; end
def get_delta
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])

    @get_delta = service.getDelta(session[:state])

  rescue Exception => e
    @get_delta_error = e.message
  ensure
    session[:toggle_div] = :getMsg
  end
  erb :immn
end

get '/getMessageIndexInfo' do get_message_index_info; end
post '/getMessageIndexInfo' do get_message_index_info; end
def get_message_index_info
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])

    @msg_index_info = service.getIndexInfo

  rescue Exception => e
    @msg_index_info_error = e.message
  ensure
    session[:toggle_div] = :getMsg
  end
  erb :immn
end


get '/updateMessage' do update_message; end
post '/updateMessage' do update_message; end
def update_message
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])

    unread_flag = ( session[:readflag] == 'unread' )

    msgids = session[:messageId].split(",")

    @update_msg = service.updateReadFlag(msgids, unread_flag)

  rescue Exception => e
    @update_msg_error = e.message
  ensure
    session[:toggle_div] = :updateMsg
  end
  erb :immn
end

get '/deleteMessage' do delete_message; end
post '/deleteMessage' do delete_message; end
def delete_message
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])

    msgids = session[:messageId].split(",")

    @delete_msg = service.deleteMessage(msgids)

  rescue Exception => e
    @delete_msg_error = e.message
  ensure
    session[:toggle_div] = :delMsg
  end
  erb :immn
end

get '/getNotifyDetails' do get_notify_details; end
post '/getNotifyDetails' do get_notify_details; end
def get_notify_details
  begin
    service = Service::MIMService.new(settings.FQDN, session[:token])

    queues = session[:queues]

    @notification_details = service.getNotificationDetails(queues)

  rescue Exception => e
    @notification_details_error = e.message
  ensure
    session[:toggle_div] = :getMsgNot
  end
  erb :immn
end

def load_attachments
  # make first box blank
  @attachments = [""] 
  Dir.entries(ATTACH_DIR).sort.each do |x|
    #add file filter directories with one or more '.'
    @attachments.push x unless x.match /\A\.+/
  end
end

def save_session
  PARAMS.each do |x|
    session[x] = params[x] 
  end
end

def clear_session
  PARAMS.each do |x|
    session[x] = nil
  end
end
