
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

x_arg_val = URI.escape(settings.X_arg)

['/SpeechToText'].each do |path|
  before path do
    obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
  end
end

get '/' do
  drop_down_list
  erb :speech
end

post '/SpeechToText' do
  if params[:f1] != nil
    @type = (params[:f1][:filename]).to_s.split(".")[1]
    #Basic file extension check to ensure proper file types are uploaded
    #Some times browser's may recognize mime types are application/octet-stream if the system does not know about the files mime type
    if @type.to_s.eql?"wav"
      @type = "audio/wav"
      speech_to_text
    elsif @type.to_s.eql?"amr"
      @type = "audio/amr"
      speech_to_text
    elsif @type.to_s.eql?"amr-wb"
      @type = "audio/amr-wb"
      speech_to_text
    elsif @type.to_s.eql?"x-speex"
      @type = "audio/x-speex"
      speech_to_text
    elsif @type.to_s.eql?"spx"
      @type = "audio/spx"
      speech_to_text
    else
      drop_down_list
      @error = "Invalid file type, use audio/wav, audio/x-wav, audio/amr, audio/amr-wb or x-speex."
      return erb :speech
    end
  else
    speech_default_file
  end
end

def default_to_generic
  @speech_context = 'Generic'
  if params[:SpeechContext] == ""
    params[:SpeechContext] = @speech_context
  end
end

def drop_down_list
  @context_types = settings.speech_context.split(", ")
  @type_list = Array.new
  
  @context_types.each do |p|
    @type_list.push p
  end
end

def speech_to_text
  
  x_arg_val = URI.escape(settings.X_arg)

  drop_down_list
  default_to_generic

  temp_file = params[:f1][:tempfile]

  @file_contents = File.read(temp_file.path)

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

def speech_default_file

  x_arg_val = URI.escape(settings.X_arg)

  drop_down_list
  default_to_generic

  @filename = 'bostonSeltics.wav'
  @type = 'audio/wav'

  fullname = File.expand_path(File.dirname(File.dirname(__FILE__)))
  final = fullname + '/' + @filename
  @file_contents = File.read(final)

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
  return erb:speech
end
