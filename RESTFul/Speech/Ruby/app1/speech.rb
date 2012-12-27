
# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

#!/usr/bin/ruby
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

SCOPE = 'SPEECH'
AUDIO_DIR = File.join(File.expand_path(File.dirname(__FILE__)), "public/audio")

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
		obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
	end
end

get '/' do
	erb :speech
end

post '/SpeechToText' do
	if params[:audio_file] then
		load_audio_file params[:audio_file]
	end
	return erb :speech
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
	
	load_submitted
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

def default_to_generic
	@speech_context = 'Generic'
	if params[:SpeechContext] == ""
		params[:SpeechContext] = @speech_context
	end
end


def speech_to_text
	x_arg_val = URI.escape(X_arg)

	default_to_generic

	url = "#{settings.FQDN}/rest/2/SpeechToText"

	if params[:chkChunked] != nil
		response = RestClient.post url, "#{@file_contents}", :Authorization => "Bearer #{@access_token}", :Content_Transfer_Encoding => 'chunked', :X_arg => "#{x_arg_val}", :X_SpeechContext => params[:SpeechContext], :Content_Type => "#{@type}" , :Accept => 'application/json'
		@result = JSON.parse response
	else
		response = RestClient.post url, "#{@file_contents}", :Authorization => "Bearer #{@access_token}", :X_arg => "#{x_arg_val}", :X_SpeechContext => params[:SpeechContext], :Content_Type => "#{@type}" , :Accept => 'application/json'
		@result = JSON.parse response
	end

rescue => e
	if e.response.nil?
		@error = e.message
	else
		@error = e.response
	end
ensure
	return erb :speech
end

def load_audio_file filename
	@type = get_filetype filename

	@file_contents = File.read(File.join(AUDIO_DIR,filename))

	speech_to_text
end

def get_filetype filename
	#Basic file extension check to ensure proper file types are uploaded
	#Some times browser's may recognize mime types are application/octet-stream if the system does not know about the files mime type
	extension = filename.split(".")[1]
	@type = case extension
	when "wav" then	"audio/wav"
	when "amr" then	"audio/amr"
	when "amr-wb" then "audio/amr-wb"
	when "x-speex" then "audio/x-speex"
	when "spx" then	"audio/x-speex"
	else @error = "Invalid file type, use audio/wav, audio/x-wav, audio/amr, audio/amr-wb or x-speex."
	end
end
