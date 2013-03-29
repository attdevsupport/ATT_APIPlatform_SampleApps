#!/usr/bin/ruby

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
require File.join(File.dirname(__FILE__), 'common.rb')

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

RestClient.proxy = settings.proxy

SCOPE = 'STTC'
AUDIO_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/audio")
TEMPLATE_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/template")

#set xarg to an empty string if it's nil
if settings.X_arg then
	X_arg = settings.X_arg
else
	X_arg = ''
end

x_arg_val = URI.escape(X_arg)

['/', '/SpeechToText'].each do |path|
	before path do
		drop_down_list
    dict, grammar, audio = load_files
    @mime_data = "x-dictionary:\n" + dict + "\n\nx-grammar:\n" + grammar
		obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
	end
end

get '/' do
	erb :speech
end

post '/SpeechToText' do
  speech_to_text
	return erb :speech
end

def speech_to_text
	x_arg_val = URI.escape(X_arg)

	url = "#{settings.FQDN}/speech/v3/speechToTextCustom"

  boundary = rand(1_000_000).to_s
  payload = generate_multipart(boundary, *load_files)

  response = RestClient.post url, payload, 
    :Authorization => "Bearer #{@access_token}", 
    :X_arg => "#{x_arg_val}", 
    :X_SpeechContext => params[:SpeechContext], 
    :Content_Type => "multipart/x-srgs-audio; boundary=\"#{boundary}\"",
    :Accept => 'application/json'

  @result = JSON.parse(response)

rescue => e
  @error = e.response
ensure
	return erb :speech
end

def load_files
  dictionary = File.read(File.join(TEMPLATE_DIR, "x-dictionary.txt"))
  grammar = File.read(File.join(TEMPLATE_DIR, "x-grammar.txt"))
	audio_file = File.open(File.join(AUDIO_DIR, "pizza-en-US.wav")) {|f| f.read}

  return [dictionary, grammar, audio_file]
end

def generate_multipart(boundary, dictionary, grammar, audio)
  body = "--" +"#{boundary}\r\n"
  body += "Content-Disposition: form-data; name=\"x-dictionary\"; filename=\"speech_alpha.pls\r\n"
  body += "Content-Type: application/pls+xml\r\n\r\n"
  body += "#{dictionary}\r\n\r\n"

  body += "--" +"#{boundary}\r\n"
  body += "Content-Disposition: form-data; name=\"x-grammar\"\r\n"
  body += "Content-Type: application/srgs+xml\r\n\r\n"
  body += "#{grammar}\r\n\r\n"

  body += "--" +"#{boundary}\r\n"
  body += "Content-Disposition: form-data; name=\"x-voice\"; filename=\"pizza-en-US.wav\"\r\n"
  body += "Content-Type: audio/wav\r\n"
  body += "Content-Transfer-Encoding: binary\r\n\r\n"
  body += "#{audio}\r\n\r\n"
  body += "#{boundary}--\r\n"

  body
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

