
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

SCOPE = 'DC'

# obtain an OAuth access token if necessary
def authorize
  if session[:dc_access_token].nil? then
   redirect "#{settings.auth_code_url}client_id=#{settings.api_key}&scope=DC&redirect_uri=#{settings.redirect_url}"
  else
   redirect '/GetDeviceCapabilities'
  end
end

def get_device_capabilties
   
  response = RestClient.get "#{settings.getdc_url}", :Authorization => "BEARER #{session[:dc_access_token]}", :Content_Type => 'application/json', :Accept => 'application/json'
   
  @result = JSON.parse(response)

rescue => e
  @error = e.message
ensure
  return erb :dc
end

def get_access_token

  if params[:error] != nil
     session[:access_error] = params[:error]
     session[:error_type] = params[:error_description]
     redirect '/'
  else
     response = RestClient.post "#{settings.access_token_url}", :grant_type => "authorization_code", :client_id => settings.api_key, :client_secret => settings.secret_key, :code => params[:code]
     from_json = JSON.parse response
     session[:dc_access_token] = from_json['access_token']
     redirect '/GetDeviceCapabilities'
  end
end

get '/auth/callback' do
  get_access_token
end

get '/' do
  if session[:access_error] != nil
    return erb :dc
  else
    authorize
  end
end

get '/GetDeviceCapabilities' do
  get_device_capabilties
end
