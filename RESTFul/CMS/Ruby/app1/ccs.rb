
# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

#!/usr/bin/ruby
require 'rubygems'
require 'json'
require 'rest_client'
require 'sinatra'
require 'open-uri'
require 'sinatra/config_file'
require File.join(File.dirname(__FILE__), 'common.rb')

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'CCS'

# Obtain an OAuth access token if necessary.
['/CreateSession', '/SendSignal'].each do |path|
  before path do
    obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
  end
end

# On start of application clear any session id value and read scripts.
get '/' do
  clear_session_variables
  read_script
  erb :ccs
end

post '/CreateSession' do
  read_script
  if params[:btnCreateSession] != nil
  numberScript = params[:txtNumberToDial]
    if numberScript.empty?
      read_script
      @error = 'You must enter in a telephone number or sip address.'
      return erb :ccs
    else
      read_script
      create_session
      return erb :ccs
    end
  else
    erb :ccs
  end
end

post '/SendSignal' do
  if params[:scriptType].nil?
    send_signal
    read_script
    return erb :ccs
  else
    read_script
    return erb :ccs
  end
end

def clear_session_variables
  session[:txtNumberToDial] = nil
  session[:txtMessageToPlay] = nil
  session[:txtNumber] = nil
  session[:ccs_id] = nil
end

# Function to get contents of selected script file.
def getContentsFromFile filetoread
  return File.read Dir.pwd + '/' + filetoread
end

def read_script
  # Populate Feature 1: Outbound voice/message Session text area with contents of Script.rb script. 
  @firstscript = 'First.rb'
  @outbound = getContentsFromFile @firstscript
end

# Function to create session for outbound voice and messaging.
def create_session

  # Script variables for First.rb.
  numberToDial = session[:txtNumberToDial] = params[:txtNumberToDial]
  messageToPlay = session[:txtMessageToPlay] = params[:txtMessageToPlay]
  numberVar = session[:txtNumber] = params[:txtNumber]
  scriptType = session[:scriptType] = params[:scriptType]
  
  # Pass the script variable for First.rb in request body.
  requestbody = '{"feature":"' + scriptType + '","messageToPlay":"' + messageToPlay + '","numberToDial":"' + numberToDial.to_s + '","featurenumber":"' + numberVar.to_s + '"}'
  
  # Resource URL for Create Session.
  url = "#{settings.FQDN}/rest/1/Sessions"
RestClient.post url, "#{requestbody}", :Authorization => "Bearer #{@access_token}", :Content_Type => 'application/json', :Accept => 'application/json' do |response, request, result, &block|
  	@r = response
  end

  if @r.code == 200
    @result = JSON.parse @r
    session[:ccs_id] = @result['id']
  else
    @error = @r
  end
end

# Function which sends signal to either exit, hold or dequeue session.
def send_signal
  
  # Resource URL for Send Signal.
  url = "#{settings.FQDN}/rest/1/Sessions/#{session[:ccs_id]}/Signals"
  
  # Pass the signal paramater in request body.
  requestBody = '{"signal":"' + params[:signal] + '"}'
  
  response = RestClient.post url, "#{requestBody}", :Authorization => "Bearer #{@access_token}", 
  :Content_Type => 'application/json', :Accept => 'application/json'
  
  @signal_result = JSON.parse response
  
rescue => e
  @signal_error = e.message
ensure
  return erb :ccs
end
