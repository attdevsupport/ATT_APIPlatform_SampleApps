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

SCOPE = "STTC"

RestClient.proxy = settings.proxy

AUDIO_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/audio")
TEMPLATE_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/template")


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
  dict, grammar, afile = load_files
  @mime_data = "x-dictionary:\n" + dict + "\n\nx-grammar:\n" + grammar 
end

get '/' do
  erb :speech
end

post '/SpeechToText' do
  begin
    dict, grammar, afile = load_files

    if params[:nameParam] and params[:SpeechContext] == "GenericHints"
      grammar_type = params[:nameParam]
    else
      grammar_type = nil 
    end

    response = Speech.speechToText(afile, dict, grammar, 
                                   :context => params[:SpeechContext], 
                                   :grammar => grammar_type,
                                   :xargs => settings.X_arg)

    @result = JSON.parse(response)

  rescue RestClient::Exception => e
    @error = e.response 
  rescue Exception => e
    @error = e.message
  end
  erb :speech
end


def load_files
  dictionary = File.join(TEMPLATE_DIR, "x-dictionary.txt")
  grammar = File.join(TEMPLATE_DIR, "x-grammar.txt")
  audio_file = File.join(AUDIO_DIR, "pizza-en-US.wav")

  return [dictionary, grammar, audio_file]
end

def drop_down_list
  @context_types = settings.speech_context.split(",")
  @type_list = Array.new
  @audio_file_list = Array.new

  @context_types.each do |p|
    @type_list.push p
  end

  Dir.entries(AUDIO_DIR).sort.each do |x|
    @audio_file_list.push x unless x.match /\A\.+/
  end
end

