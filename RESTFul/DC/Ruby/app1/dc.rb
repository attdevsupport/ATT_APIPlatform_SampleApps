#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

require 'rubygems'
require 'sinatra'
require 'open-uri'
require 'sinatra/config_file'
require 'att/codekit'

include Att::Codekit

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = "DC"

RestClient.proxy = settings.proxy

configure do
  OAuth = Auth::AuthCode.new(settings.FQDN, 
                             settings.api_key, 
                             settings.secret_key)
end

before do
  #add special headers for IE
  headers "P3P" => "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\""

  #create our service if isn't present
  if params[:code] && session[:token].nil?
    session[:token] = OAuth.createToken(params[:code]) 
  end
end

get '/' do
  begin
    #if there's a code we're returning from authenticaion
    if session[:token]
      @result = Service::DCService.new(settings.FQDN, session[:token]).getDeviceCapabilities

    #something went wrong with authentication display error
    elsif params[:error]
      @error = params[:error] + ": " 

    #Authenticate if neccessary
    else
      redirect OAuth.consentFlow(:scope => SCOPE, :redirect => settings.redirect_url)
    end
  rescue Exception => e
    @error = e.message
  end
  erb :dc
end
