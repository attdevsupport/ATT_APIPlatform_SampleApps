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
require 'uri'
require 'sinatra/config_file'
require 'att/codekit'

include Att::Codekit

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = "SPEECH"

RestClient.proxy = settings.proxy

AUDIO_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/audio")

configure do
  oauth = Auth::ClientCred.new(settings.FQDN,
                               settings.api_key,
                               settings.secret_key,
                               SCOPE, 
                               :tokens_file => settings.tokens_file)

  Speech = Service::SpeechService.new(oauth)
end

before do 
  drop_down_list
  load_submitted
end

get '/' do
  erb :speech
end

post '/SpeechToText' do
  begin
    audio_file = File.join(AUDIO_DIR, params[:audio_file])

    chunked = params[:chkChunked]
    context = params[:SpeechContext]

    response = Speech.toText(audio_file, 
                             :xargs => settings.X_arg, 
                             :context => context, 
                             :chunked => chunked)

    @result = JSON.parse(response)

  rescue RestClient::Exception => e
    @error = e.response 
  rescue Exception => e
    @error = e.message
  end
  erb :speech
end

def drop_down_list
  @context_types = settings.speech_context.split(", ")
  @type_list = Array.new
  @audio_file_list = Array.new

  @context_types.each do |p|
    @type_list.push p
  end

  Dir.entries(AUDIO_DIR).sort.each do |x|
    @audio_file_list.push x unless x.match /\A\.+/
  end
end

def load_submitted
  @selected_chunked = params[:chkChunked]

  if params[:SpeechContext] then
    @selected_type = params[:SpeechContext]
  end

  if params[:audio_file] then
    @selected_file = params[:audio_file]
  end
end

