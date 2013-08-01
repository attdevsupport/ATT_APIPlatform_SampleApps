#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

require 'rubygems'
require 'json'
require 'rest_client'
require 'sinatra'
require 'sinatra/config_file'
require 'base64'
require 'att_tts_service'

include AttCloudServices

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

TEXT_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/text")

RestClient.proxy = settings.proxy

configure do
  Service = TTS::TtsService.new(settings.FQDN,
                                settings.api_key,
                                settings.secret_key,
                                TTS::SCOPE,
                                :tokens_file => settings.tokens_file)
end

before do
  @ssml_content = load_content("SSMLWithPhoneme.txt")
  @text_content = load_content("PlainText.txt")
end


get '/' do
  erb :tts
end

post '/TextToSpeech' do
  begin
    type = params[:ContentType]
    content = case type
              when "application/ssml+xml" then load_content("SSMLWithPhoneme.txt")
              when "text/plain" then load_content("PlainText.txt")
              end

    response = Service.textToSpeech(params[:x_arg], content, type)
    #the binary audio file
    @audio = response
  rescue => e
    @error = e.response
  ensure
    return erb :tts
  end
end

def load_content(text_file)
  File.readlines(File.join(TEXT_DIR,text_file))
end
