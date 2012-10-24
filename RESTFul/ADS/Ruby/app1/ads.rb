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

SCOPE = 'ADS'

['/GetAds'].each do |path|
  before path do
    obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
  end
end

get '/' do
  erb :ads
end

post '/GetAds' do
  get_ads
end

def get_ads
   
   @invalidParams = []

   url = "#{settings.FQDN}/rest/1/ads?Category=#{params[:Category]}"
   
   unless params[:Gender].empty?
    url += "&Gender=#{params[:Gender]}"
   end
   
   unless params[:zipCode].empty?
    if params[:zipCode].to_i != 0
     url += "&ZipCode=#{params[:zipCode]}"
	else
	 a = 'Invalid Zip Code'
	 @invalidParams << a
	end
   end
   
   unless params[:areaCode].empty?
    if params[:areaCode].to_i != 0
     url += "&AreaCode=#{params[:areaCode]}"
    else
	 b = 'Invalid Area Code'
	 @invalidParams << b
	end
   end
   
   unless params[:City].empty?
    url += "&City=#{params[:City]}"
   end
   
   unless params[:Country].empty?
    url += "&Country=#{params[:Country]}"
   end
   
   unless params[:Longitude].empty?
    if params[:Longitude].to_f != 0
     url += "&Longitude=#{params[:Longitude]}"
    else
	 c = 'Invalid Longitude'
	 @invalidParams << c
	end
   end
   
   unless params[:Latitude].empty?
    if params[:Latitude].to_f != 0
     url += "&Latitude=#{params[:Latitude]}"
    else
	 d = 'Invalid Latitude'
	 @invalidParams << d
	end
   end
   
   if params[:MMASize] != nil
    if params[:MMASize] == "120 x 20"
	 url += "&MinWidth=20&MinHeight=20&MaxHeight=120&MaxWidth=120"
	elsif params[:MMASize] == "168 x 28"
	 url += "&MinWidth=28&MinHeight=28&MaxHeight=168&MaxWidth=168"
	elsif params[:MMASize] == "216 x 36"
	 url += "&MinWidth=36&MinHeight=36&MaxHeight=216&MaxWidth=216"
	elsif params[:MMASize] == "300 x 50"
	 url += "&MinWidth=50&MinHeight=50&MaxHeight=300&MaxWidth=300"
	elsif params[:MMASize] == "300 x 250"
	 url += "&MinWidth=250&MinHeight=250&MaxHeight=300&MaxWidth=300"
	elsif params[:MMASize] == "320 x 50"
	 url += "&MinWidth=50&MinHeight=50&MaxHeight=320&MaxWidth=320"
	end
   end
   
   unless params[:AgeGroup].empty?
    url += "&AgeGroup=#{params[:AgeGroup]}"
   end
   
   unless params[:Over18].empty?
    url += "&Over18=#{params[:Over18]}"
   end
   
   unless params[:Keywords].empty?
    url += "&Keywords=#{params[:Keywords]}"
   end
   
   unless params[:Premium].empty?
    url += "&Premium=#{params[:Premium]}"
   end
   
   if settings.adType != nil
    url += "&Type=#{settings.adType}"
   end
   
   if @invalidParams.any?
    @error = ''
    @error
   else
   
   response = RestClient.get url, :Authorization => "BEARER #{@access_token}", :User_Agent => "#{@env["HTTP_USER_AGENT"]}", :UDID => "012266005922565000000000000000", :Content_Type => 'application/json', :Accept => 'application/json'
   
   if response.code == 204
    @no_ads = 'No Ads are returned'
    @no_ads
   else
    @result = JSON.parse(response)
   end
   end

rescue => e
  @error_gateway = e.message
ensure
  return erb :ads
end
