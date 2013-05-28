#!/usr/bin/ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

require 'rubygems'
require 'json'
require 'rest_client'
require 'sinatra'
require 'open-uri'
require 'sinatra/config_file'
require 'att_dc_service'

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

RestClient.proxy = settings.proxy

include AttCloudServices

before do
  #add special headers for IE
  headers "P3P" => "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\""

  #create our service if isn't present
  if session[:dc].nil?
    session[:dc] = DC::DCService.new(settings.FQDN, 
                                 settings.api_key, 
                                 settings.secret_key,
                                 DC::SCOPE,
                                 :grant_type => GrantType::AUTHORIZATION,
                                 :redirect_uri => settings.redirect_url) 
  end
end

get '/' do
  begin
    #if there's a code we're returning from authenticaion
    if params[:code] 
      @result = JSON.parse(session[:dc].getDeviceCapabilties(params[:code]))

    #something went wrong with authentication display error
    elsif params[:error]
      @error = params[:error] + ": " + params[:error_description]

    #Authenticate if neccessary
    else
      redirect session[:dc].consentFlow
    end
  rescue => e
    @error = e.message
  ensure
    return erb :dc
  end
end
