#!/usr/bin/env ruby

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
require 'att/codekit'

#simplify our namespace
include Att::Codekit

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'ADS'
RestClient.proxy = settings.proxy

configure do
  begin
    VALID_PARAMS = [:Category, :MMA]
    oauth = Auth::ClientCred.new(settings.FQDN,
                                 settings.api_key,
                                 settings.secret_key,
                                 SCOPE, 
                                 :tokens_file => settings.tokens_file)
    Ads = Service::ADService.new(oauth)
  rescue Exception => e
    @error = e.message
  end
end

get '/' do
  erb :ads
end

post '/getAds' do
  begin
    args = Hash.new

    VALID_PARAMS.each do |p|
      args[p] = params[p].strip unless params[p].nil? or params[p].strip.empty?
    end

    heads = {
      :User_Agent => @env["HTTP_USER_AGENT"].to_s, 
      :UDID => "012266005922565000000000000000"
    }

    response = Ads.get_ads(args, heads) 

    case response.code
    when 204
      @no_ads = 'No Ads were returned'
    when 200,201
      puts response
      @result = JSON.parse(response)["AdsResponse"]["Ads"]
    else
      @error = response
    end

  rescue RestClient::Exception => e
    @error = e.response 
  rescue Exception => e
    @error = e.message
  end
  erb :ads
end

