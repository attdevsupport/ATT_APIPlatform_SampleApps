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
require 'sinatra/config_file'
require 'cgi'
require 'att/codekit'

include Att::Codekit

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = "TL"

configure do 
  OAuth = Auth::AuthCode.new(settings.FQDN,
                             settings.api_key,
                             settings.secret_key,
                             SCOPE,
                             settings.redirect_url)
end

before do
  session[:tl] = Service::TLService.new(OAuth) if session[:tl].nil?
end

get '/' do
  begin
    #something went wrong with authentication display error
    if params[:error] then
      @error = params[:error] + ": " + params[:error_description]
      #if code is present then get location
    elsif params[:code] then
      response = session[:tl].getLocation(params[:code],
                                          :req_accuracy => params[:requestedAccuracy],
                                          :accept_accuracy => params[:acceptableAccuracy],
                                          :tolerance => params[:tolerance])
      @result = JSON.parse(response)
    end


  rescue RestClient::Exception => e
    @error = e.response 
  rescue Exception => e
    @error = e.message
  end
  erb :tl
end

post '/submit' do
  begin
    if session[:tl].authenticated?
      #get location if authenticated
      response = session[:tl].getLocation(:req_accuracy => params[:requestedAccuracy],
                                          :accept_accuracy => params[:acceptableAccuracy],
                                          :tolerance => params[:tolerance])
      @result = JSON.parse(response)
    else
      #save params and authenticate
      session[:requestedAccuracy] = params[:requestedAccuracy]
      session[:acceptableAccuracy] = params[:acceptableAccuracy]
      session[:tolerance] = params[:tolerance]
      redirect session[:tl].consentFlow
    end

  rescue RestClient::Exception => e
    @error = e.response 
  rescue Exception => e
    @error = e.message
  end
  erb :tl
end

