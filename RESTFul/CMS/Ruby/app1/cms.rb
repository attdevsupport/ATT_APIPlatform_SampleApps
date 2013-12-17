#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'rubygems'
require 'sinatra'
require 'open-uri'
require 'sinatra/config_file'
require 'cgi'

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

SCOPE = 'CMS'

#An array of all the methods our script supports
SCRIPT_METHODS = settings.script_methods.split(",")
SCRIPT_FILE = "cms_files/First.rb"

#Set the proxy that's used in att/codekit
RestClient.proxy = settings.proxy

#setup our oauth service
configure do
  FILE_SUPPORT = (settings.tokens_file && !settings.tokens_file.strip.empty?)
  FILE_EXISTS = FILE_SUPPORT && File.file?(settings.tokens_file)

  OAuth = Auth::ClientCred.new(settings.FQDN, 
                               settings.api_key, 
                               settings.secret_key)
  @@token = nil
end

#load our file for display before anything
before do
  read_script
  begin
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
    @error = e.message
  end
end

# On start of application clear any session id value and read scripts.
get '/' do
  clear_session_variables
  erb :cms
end

post '/CreateSession' do
  begin
    create_session unless params[:btnCreateSession].nil? 
  rescue Exception => e
    @error = e.message
  end
  erb :cms
end

post '/SendSignal' do
  begin
    service = Service::CMSService.new(settings.FQDN, @@token)

    if params[:scriptType].nil?
      @signal_result = service.sendSignal(session[:cms_id], params[:signal])
    end
  rescue Exception => e
    @signal_error = e.message
  end
  erb :cms
end

def clear_session_variables
  session[:txtNumberToDial] = nil
  session[:txtMessageToPlay] = nil
  session[:txtNumber] = nil
  session[:cms_id] = nil
end

# Function to get contents of selected script file.
def getContentsFromFile filetoread
  return File.read(Dir.pwd + '/' + filetoread)
end

def read_script
  # Populate Feature 1: Outbound voice/message Session text area with contents of Script.rb script. 
  @outbound = getContentsFromFile SCRIPT_FILE
end

# Function to create session for outbound voice and messaging.
def create_session
  # Script variables for First.rb.
  numberToDial = session[:txtNumberToDial] = CGI.escapeHTML(params[:txtNumberToDial])
  messageToPlay = session[:txtMessageToPlay] = CGI.escapeHTML(params[:txtMessageToPlay])
  numberVar = session[:txtNumber] = CGI.escapeHTML(params[:txtNumber])
  scriptType = session[:scriptType] = CGI.escapeHTML(params[:scriptType])


  # Pass the script variable for First.rb in request body.
  requestbody = {
    'feature' => scriptType, 
    'messageToPlay' => messageToPlay,
    'numberToDial' => numberToDial.to_s,
    'featurenumber' => numberVar.to_s, 
    'smsCallerID' => settings.phoneNumber.to_s,
  }

  begin
    service = Service::CMSService.new(settings.FQDN, @@token)

    @result =  service.create_session(requestbody)

    session[:cms_id] = @result.id
  rescue Exception => e
    @error = e.message 
  end
end

