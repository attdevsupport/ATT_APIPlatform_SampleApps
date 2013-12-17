#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'rubygems'
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
                             :redirect => settings.redirect_url)
end

before do
  if params[:code] 
    token = OAuth.createToken(params[:code])
    session[:tl] = Service::TLService.new(settings.FQDN, token) 
  elsif session[:tl] && session[:tl].token.expired?
    token = OAuth.refreshToken(session[:tl].token)
    session[:tl] = Service::TLService.new(settings.FQDN, token)
  end
end

get '/' do
  begin
    #something went wrong with authentication display error
    if params[:error] 
      @error = params[:error] + ": " + params[:error_description]

    elsif session[:tl] && session[:back_from_consent]
      @result = session[:tl].getLocation(:req_accuracy => session[:requestedAccuracy],
                                         :accept_accuracy => session[:acceptableAccuracy],
                                         :tolerance => session[:tolerance])
      clear_session
    end

  rescue Exception => e
    @error = e.message
  end
  erb :tl
end

post '/submit' do
  begin
    if session[:tl]
      @result = session[:tl].getLocation(:req_accuracy => params[:requestedAccuracy],
                                         :accept_accuracy => params[:acceptableAccuracy],
                                         :tolerance => params[:tolerance])
    else
      save_session
      redirect OAuth.consentFlow(:scope => SCOPE)
    end

  rescue Exception => e
    @error = e.message
  end
  erb :tl
end

def save_session
  session[:requestedAccuracy] = params[:requestedAccuracy]
  session[:acceptableAccuracy] = params[:acceptableAccuracy]
  session[:tolerance] = params[:tolerance]
  session[:back_from_consent] = true
end

def clear_session
  session[:requestedAccuracy] = nil
  session[:acceptableAccuracy] = nil
  session[:tolerance] = nil
  session[:back_from_consent] = nil
end
