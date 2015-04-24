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
require 'sinatra/config_file'

# require codekit
require 'att/codekit'

include Att::Codekit

class Speech < Sinatra::Application
  get '/' do index; end
  get '/speechToText' do index; end
  post '/speechToText' do speech_to_text; end
  get  '/load' do load_data; end
  post '/save' do save_data; end

  configure do
    enable :sessions
    config_file 'config.yml'
    SCOPE = "SPEECH"
    Transport.proxy(settings.proxy)
    FILE_SUPPORT = (settings.tokens_file && !settings.tokens_file.strip.empty?)
    AUDIO_DIR = File.join(File.expand_path(File.dirname(__FILE__)),
                          "public/audio")

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
    erb :speech
  end

  def speech_to_text
    begin
      service = Service::SpeechService.new(settings.FQDN, settings.token)
      filename = params[:speechFile]

      allowed = ['boston_celtics.wav', 'california.amr', 'coffee.amr',
                 'samplerate_conflict_error.wav', 'this_is_a_test.spx',
                 'doctors.wav', 'nospeech.wav', 'too_many_channels_error.wav']

      if !allowed.include?(filename.to_s)
        raise Exception.new('Invalid speech file specified')
      end

      audio_file = File.join(AUDIO_DIR, filename)
      x_arg = params[:x_arg]
      chunked = params[:chunked]
      context = params[:speechContext]
      subcontext = params[:x_subcontext]

      result = service.speechToText(audio_file, 
                                    :xargs => x_arg, 
                                    :context => context, 
                                    :subcontext => subcontext,
                                    :chunked => chunked)
      values = []
      headers = nil
      nbest = result.nbest
      if nbest.empty?
        headers = ['ResponseId', 'Status']
        values << [result.id, result.status]
      else
        headers = ['ResponseId', 'Status', 'Hypothesis', 'LanguageId',
                   'Confidence', 'Grade', 'ResultText', 'Words', 'WordScores']
        nbest.each do |best|
          values << [result.id, result.status, best.hypothesis, best.language,
                     best.confidence, best.grade, best.result, best.words,
                     best.scores]
        end
      end
      {
        :success => true,
        :tables => [{
          :caption => 'Speech Response:',
          :headers => headers,
          :values => values
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
      :authenticated => true,
      :server_time => Util::serverTime,
      :download => settings.download_link,
      :github => settings.github_link,
    }
    data[:savedData] = session[:savedData] unless session[:savedData].nil?
    data.to_json
  end
end
