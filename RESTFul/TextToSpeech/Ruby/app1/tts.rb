#!/usr/bin/ruby

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
require File.join(File.dirname(__FILE__), 'common.rb')

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'TTS'
TEXT_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/text")

RestClient.proxy = settings.proxy

before '*' do
  #load files 
  load_content

  #obtain oauth tokens
  obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
end

get '/' do
  erb :tts
end

post '/TextToSpeech' do
  text_to_speech params[:ContentType], params[:x_arg]
end

def text_to_speech(type, x_args="")
  x_arg_val = URI.escape(x_args)

  url = "#{settings.FQDN}/speech/v3/textToSpeech"

  content = case type
            when "application/ssml+xml" then @ssml_content
            when "text/plain" then @text_content
            end

  response = RestClient.post url, "#{content}",
    :Authorization => "Bearer #{@access_token}", 
    :Accept => "audio/x-wav",
    :X_arg => "#{x_arg_val}", 
    :Content_Type => "#{type}"

  #the binary audio file
  @audio = response

rescue => e
  @error = e.response
ensure
  return erb :tts
end

def load_content
  @text_content = File.readlines(File.join(TEXT_DIR,"PlainText.txt"))
  @ssml_content = File.readlines(File.join(TEXT_DIR, "SSMLWithPhoneme.txt"))
end
