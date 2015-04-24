#!/usr/bin/env ruby

# Copyright 2014 AT&T
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

require 'sinatra'
require 'sinatra/config_file'
require 'securerandom'
require 'open-uri'

# require codekit
require 'att/codekit'

#simplify our namespace
include Att::Codekit

class ADS < Sinatra::Application
  get '/' do index; end
  get '/load' do load_data; end
  post '/save' do save_data; end
  post '/getAds' do get_ads; end

  configure do
    enable :sessions
    disable :protection
    config_file 'config.yml'
    #Setup proxy used by att/codekit
    Transport.proxy(settings.proxy)

    SCOPE = 'ADS'
    OAuth = Auth::ClientCred.new(settings.FQDN,
                                 settings.api_key,
                                 settings.secret_key)

    set :tokens_file, Dir.tmpdir + "/ruby_ads_token"
    File.new(settings.tokens_file, 'w').close unless File.exists? settings.tokens_file

    set :token, nil
  end

  before '/getAds' do
    begin
      #check if token exists and create if necessary
      settings.token = Auth::OAuthToken.load(settings.tokens_file) if settings.token.nil?
      if settings.token.nil?
        settings.token = OAuth.createToken(SCOPE)
      elsif settings.token.expired?
        settings.token = OAuth.refreshToken(settings.token) 
      end
    rescue Exception => e
      halt 401, { :success => false, :text => e.message }.to_json
    end
  end

  def index
    erb :ads
  end

  def get_ads
    begin
      service = Service::ADSService.new(settings.FQDN, settings.token)

      opts = {
        :AgeGroup => params[:ageGroup],
        :AreaCode => params[:areaCode], 
        :City => params[:city],
        :Country => params[:country], 
        :Gender => params[:gender], 
        :Latitude => params[:latitude], 
        :Longitude => params[:longitude],
        :ZipCode => params[:zipCode], 
      }

      mma = params[:mmaSize]
      if mma && !mma.empty? 
        size = mma.split("x")
        width = size[0].strip
        height = size[1].strip
        opts[:MaxWidth] = width
        opts[:MinWidth] = width
        opts[:MaxHeight] = height
        opts[:MinHeight] = height
      end
      if params[:keywords]
        keywords = params[:keywords].split(",")
        opts[:Keywords]  = keywords.map { |k| k.strip }
      end

      category = params[:category]
      user_agent = @env["HTTP_USER_AGENT"].to_s
      if session[:user_udid].nil?
        session[:user_udid] = SecureRandom.uuid.gsub("-","") 
      end
      udid = session[:user_udid]

      ad = service.getAds(category, user_agent, udid, opts) 

      jbody = { :success => true }
      if ad.has_ads?
        jbody[:table] = [{
          :caption => 'Ads Response:',
          :headers => ['Type', 'ClickUrl'],
          :values => [[ad.type, ad.clickurl]]
        }]
      else
        jbody[:text] = 'No Ads were returned'
      end
      jbody.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  ##################################
  ####### Save data in forms #######
  ##################################
  def save_data
    session['savedData'] = JSON.parse(params['data'])
    ""
  end

  def load_data
    data = {
      :authenticated => true,
      :server_time => Util::serverTime,
      :download => settings.download_link,
      :github => settings.github_link,
    }
    data[:savedData] = session[:savedData] unless session[:savedData].nil?
    data.to_json
  end
end
