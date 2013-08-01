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
require 'att_speechcustom_service'

include AttCloudServices

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

RestClient.proxy = settings.proxy

AUDIO_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/audio")
TEMPLATE_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/template")


configure do
  Service = SpeechCustom::SpeechCustomService.new(settings.FQDN, 
                                            settings.api_key, 
                                            settings.secret_key, 
                                            SpeechCustom::SCOPE, 
                                            :tokens_file => settings.tokens_file)
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
      nametype = params[:nameParam]
    else
      nametype = "filename"
    end

    dict_part = {
      :headers => {"Content-Disposition" => 'form-data; name="x-dictionary"; ' + nametype.to_s + '="speech_alpha.pls"',
                  "Content-Type" => "application/pls+xml"
                 },
      :data => dict 
    }

    grammar_part = {
      :headers => {"Content-Disposition" => 'form-data; name="x-grammar"',
                  "Content-Type" => "application/srgs+xml"
                 },
      :data => grammar
    }

    afile_part = {
      :headers => {"Content-Disposition" =>  'form-data; name="x-voice"; ' + nametype.to_s + '="pizza-en-US.wav"',
                   "Content-Type" => 'audio/wav',
                   "Content-Transfer-Encoding" => 'binary'
                  },
      :data => afile
    }

    multipart = [dict_part, grammar_part, afile_part]

    response = Service.speechToText(settings.X_arg, multipart, params[:SpeechContext])

    @result = JSON.parse(response)
  rescue => e
    @error = e.message
  ensure
    return erb :speech
  end
end


def load_files
  dictionary = File.read(File.join(TEMPLATE_DIR, "x-dictionary.txt"))
  grammar = File.read(File.join(TEMPLATE_DIR, "x-grammar.txt"))
  audio_file = File.read(File.join(AUDIO_DIR, "pizza-en-US.wav")) 

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

