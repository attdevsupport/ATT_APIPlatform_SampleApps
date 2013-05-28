#!/usr/bin/ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'rubygems'
require 'json'
require 'rest_client'
require 'sinatra'
require 'sinatra/config_file'
require 'cgi'
require 'att_tl_service'

include AttCloudServices

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

before do
  if session[:tl].nil?
    session[:tl] = TL::TlService.new(settings.FQDN,
                                     settings.api_key,
                                     settings.secret_key,
                                     TL::SCOPE,
                                     :redirect_uri => settings.redirect_url,
                                     :grant_type => GrantType::AUTHORIZATION)
  end
end

get '/' do
  begin
    #something went wrong with authentication display error
    if params[:error] then
      @error = params[:error] + ": " + params[:error_description]
      #if code is present then get location
    elsif params[:code] then
      get_location(session[:requestedAccuracy],
                   session[:acceptableAccuracy],
                   session[:tolerance],
                   params[:code])
    end
  rescue => e
    @error = e.message
  ensure 
    return erb :tl
  end
end

post '/submit' do
  if session[:tl].authenticated?
    #get location if authenticated
    get_location(params[:requestedAccuracy],
                 params[:acceptableAccuracy],
                 params[:tolerance])
  else
    #save params and authenticate
    session[:requestedAccuracy] = params[:requestedAccuracy]
    session[:acceptableAccuracy] = params[:acceptableAccuracy]
    session[:tolerance] = params[:tolerance]
    redirect session[:tl].consentFlow
  end
end

def get_location(ra, aa, tol, code=nil)
  response = session[:tl].getDeviceLocation(code, ra, aa, tol)
  @result = JSON.parse(response)
end

