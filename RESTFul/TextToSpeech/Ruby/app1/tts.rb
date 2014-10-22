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

# require as a gem file load relative if fails
begin
  require 'att/codekit'
rescue LoadError
  # try relative, fall back to ruby 1.8 method if fails
  begin
    require_relative 'codekit/lib/att/codekit'
  rescue NoMethodError 
    require File.join(File.dirname(__FILE__), 'codekit/lib/att/codekit')
  end
end

include Att::Codekit

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = "TTS"

TEXT_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/text")

RestClient.proxy = settings.proxy

configure do
  FILE_SUPPORT = (settings.tokens_file && !settings.tokens_file.strip.empty?)
  FILE_EXISTS = FILE_SUPPORT && File.file?(settings.tokens_file)

  OAuth = Auth::ClientCred.new(settings.FQDN,
                               settings.api_key,
                               settings.secret_key)
  @@token = nil
end

before do
  @ssml_content = load_content("SSMLWithPhoneme.txt")
  @text_content = load_content("PlainText.txt")

  begin
    if @@token.nil?
      if FILE_EXISTS 
        @@token = Auth::OAuthToken.load(settings.tokens_file)
      else
        @@token = OAuth.createToken(SCOPE)
      end
      Auth::OAuthToken.save(settings.tokens_file, @@token) if FILE_SUPPORT
    end

    if @@token.expired?
      @@token = OAuth.refreshToken(@@token)
      Auth::OAuthToken.save(settings.tokens_file, @@token) if FILE_SUPPORT
    end

  rescue Exception => e
    @error = e.message
  end
end


get '/' do
  erb :tts
end

post '/TextToSpeech' do
  begin
    service = Service::TTSService.new(settings.FQDN, @@token)

    type = params[:ContentType]
    content = case type
              when "application/ssml+xml" then load_content("SSMLWithPhoneme.txt")
              when "text/plain" then load_content("PlainText.txt")
              end

    @audio = service.textToSpeech(content, 
                                  :xargs => params[:x_arg],
                                  :type => type)

  rescue Exception => e
    @error = e.message
  end
  erb :tts
end

def load_content(text_file)
  File.readlines(File.join(TEXT_DIR,text_file))
end
