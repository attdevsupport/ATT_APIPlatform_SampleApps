#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

require 'rubygems'
require 'sinatra'
require 'sinatra/config_file'
require 'base64'
require 'att/codekit'

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
