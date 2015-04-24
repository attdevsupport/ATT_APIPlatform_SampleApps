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
require 'base64'

# require codekit
require 'att/codekit'

include Att::Codekit

class TTS < Sinatra::Application
  get '/' do index; end
  get '/textToSpeech' do index; end
  post '/textToSpeech' do text_to_speech; end
  get  '/load' do load_data; end
  post '/save' do save_data; end

  configure do
    config_file 'config.yml'
    SCOPE = "TTS"
    Transport.proxy(settings.proxy)
    FILE_SUPPORT = (settings.tokens_file && !settings.tokens_file.strip.empty?)
    OAuth = Auth::ClientCred.new(settings.FQDN,
                                 settings.api_key,
                                 settings.secret_key)
    set :token, nil
  end

  before do
    if settings.token && settings.token.expired?
      begin 
        settings.token = OAuth.refreshToken(settings.token)
      rescue
        settings.token = OAuth.createToken(SCOPE)
      ensure
        Auth::OAuthToken.save(settings.tokens_file, settings.token) if FILE_SUPPORT
      end
    end
  end

  def index
    file_exists = FILE_SUPPORT && File.file?(settings.tokens_file)
    if settings.token.nil?
      if file_exists 
        settings.token = Auth::OAuthToken.load(settings.tokens_file)
      else
        settings.token = OAuth.createToken(SCOPE)
        Auth::OAuthToken.save(settings.tokens_file, settings.token) if FILE_SUPPORT
      end
    end
    erb :tts
  end

  def text_to_speech
    begin
      service = Service::TTSService.new(settings.FQDN, settings.token)

      xarg = params[:x_arg]
      type = params[:contentType]
      content = if type == 'text/plain'
                  if params[:plaintext].length > 250
                    raise Exception("Character limit of 250 reached")
                  end
                  params[:plaintext]
                else
                  params[:ssml]
                end

      audio = service.textToSpeech(content, :xargs => xarg, :type => type)

      {
        :success => true,
        :audio => {
          :type => 'audio/wav',
          :base64 => Base64.encode64(audio.data)
        }
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
      :authenticated => true,
      :server_time => Util::serverTime,
      :download => settings.download_link,
      :github => settings.github_link,
    }
    data[:savedData] = session[:savedData] unless session[:savedData].nil?
    data.to_json
  end
end 
