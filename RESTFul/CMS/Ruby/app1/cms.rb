#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'rubygems'
require 'json'
require 'rest_client'
require 'sinatra'
require 'open-uri'
require 'sinatra/config_file'
require 'cgi'
require 'att/codekit'

include Att::Codekit

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'CMS'

#An array of all the methods our script supports
SCRIPT_METHODS = settings.script_methods.split(",")
RestClient.proxy = settings.proxy

#setup our oauth service
configure do
  begin
    oauth = Auth::ClientCred.new(settings.FQDN, 
                                 settings.api_key, 
                                 settings.secret_key,
                                 SCOPE,
                                 :tokens_file => settings.tokens_file)
    CMS = Service::CMSService.new(oauth)
  rescue RestClient::Exception => e
    @error = e.response 
  rescue Exception => e
    @error = e.message
  end
end

#load our file for display before anything
before do
  read_script
end

# On start of application clear any session id value and read scripts.
get '/' do
  clear_session_variables
  erb :cms
end

post '/CreateSession' do
  begin
    create_session unless params[:btnCreateSession].nil? 
  rescue RestClient::Exception => e
    @error = e.response 
  rescue Exception => e
    @error = e.message
  end
  erb :cms
end

post '/SendSignal' do
  begin
    if params[:scriptType].nil?
      response = CMS.send_signal(session[:cms_id], params[:signal])
      @signal_result = JSON.parse response
    end
  rescue RestClient::Exception => e
    @signal_error = e.response 
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
  @firstscript = 'First.rb'
  @outbound = getContentsFromFile @firstscript
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

  response =  CMS.create_session(requestbody)

  if response.code == 200 || response.code == 201
    @result = JSON.parse response
    session[:cms_id] = @result['id']
  else
    @error = r
  end
end

