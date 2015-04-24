#!/usr/bin/env ruby

# Copyright 2015 AT&T
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
require 'open-uri'
require 'sinatra/config_file'

# require codekit
require 'att/codekit'

include Att::Codekit


class DC < Sinatra::Application
  get '/' do index; end
  get  '/load' do load_data; end
  post '/save' do save_data; end
  post '/getDeviceCapabilities' do get_capabilities; end
  get '/authorization' do auth; end

  configure do
    enable :sessions
    config_file 'config.yml'
    disable :protection

    #Setup proxy used by att/codekit
    Transport.proxy(settings.proxy)

    SCOPE = "DC"
    AuthService = Auth::AuthCode.new(settings.FQDN, 
                                     settings.api_key, 
                                     settings.secret_key,
                                     :scope => SCOPE,
                                     :redirect => settings.redirect_url)
  end

  def auth
    begin
      base_url = settings.redirect_url.split("authorization")[0]
      if params[:code].nil?
        raise Exception.new('Authentication Code not present')
      end
      session[:token] = AuthService.createToken(params[:code]) 
      redirect to(base_url)
    rescue Exception => e
      session[:savedData][:redirecting] = nil
      @main_page = base_url
      erb :error
    end
  end

  def index
    erb :dc
  end

  def get_capabilities
    begin
      service = Service::DCService.new(settings.FQDN, session[:token])
      response = service.getDeviceCapabilities

      dc = response.capabilities

      {
        :success => true,
        :tables => [{
          :caption => 'Device Capabilities',
          :headers => [
            'TypeAllocationCode', 'Name', 'Vendor', 'Model', 'FirmwareVersion',
            'UaProf', 'MmsCapable', 'AssistedGps', 'LocationTechnology',
            'DeviceBrowser', 'WapPushCapable' 
          ],
          :values => [[ 
            response.device_id.type_allocation_code, dc.name, dc.vendor,
            dc.model, dc.firmware_version, dc.uaprof, dc.mms_capable,
            dc.assisted_gps, dc.location_technology, dc.device_browser,
            dc.wap_push_capable 
          ]]
        }]
      }.to_json
    rescue Exception => e
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
      :authenticated => !!session[:token],
      :redirect_url => AuthService.consentFlow,
      :server_time => Util::serverTime,
      :download => settings.download_link,
      :github => settings.github_link,
    }
    data[:savedData] = session[:savedData] unless session[:savedData].nil?
    data.to_json
  end
end
