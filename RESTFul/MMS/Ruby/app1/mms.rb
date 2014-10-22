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
require 'cgi'

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

#include namespace
include Att::Codekit

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'MMS'
ATTACH_DIR=File.join(File.expand_path(File.dirname(__FILE__)), "public/attachments")

RestClient.proxy = settings.proxy

#setup our mms service for the application
configure do
  FILE_SUPPORT = (settings.tokens_file && !settings.tokens_file.strip.empty?)
  FILE_EXISTS = FILE_SUPPORT && File.file?(settings.tokens_file)

  OAuth = Auth::ClientCred.new(settings.FQDN,
                               settings.api_key,
                               settings.secret_key)
  @@token = nil
end

before do 
  begin
    Dir.mkdir(settings.momms_image_dir) unless File.directory?(settings.momms_image_dir)
    Dir.mkdir(settings.momms_data_dir) unless File.directory?(settings.momms_data_dir)
    display_images
    drop_down_list
    @status_listener = load_file "#{settings.status_file}"

    #check if token exists and create if necessary
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
    @send_error = e.message
  end
end

get '/' do
  session[:mms_id] = nil
  erb :mms
end

post '/statusListener' do
  handle_inbound "#{settings.status_file}"
end

post '/refreshStatus' do
  erb :mms
end

post '/sendMms' do
  begin
    service = Service::MMSService.new(settings.FQDN, @@token)
    session[:mms1_address] = params[:address]
    session[:mms1_subject] = params[:subject]
    session[:selected_file] = params[:attachment]

    notify = false
    notify = true if params[:chkGetOnlineStatus]

    if params[:attachment] and params[:attachment].empty?
      attachment = nil
    else
      attachment  = File.join(ATTACH_DIR, params[:attachment])
    end

    @send = service.sendMms(params[:address], params[:subject], attachment, notify)

    session[:mms_id] = @send.id unless notify

  rescue Exception => e
    @send_error = e.message
  end
  erb :mms
end

post '/getStatus' do
  begin 
    service = Service::MMSService.new(settings.FQDN, @@token)
    session[:mms_id] = params[:mmsId]

    @status = service.getDeliveryStatus(session[:mms_id])

  rescue Exception => e
    @delivery_error = e.message
  end
  erb :mms
end

# use this URL to clear token file
get '/clear' do
  File.delete settings.tokens_file if File.exists? settings.tokens_file
  redirect '/'
end

def drop_down_list
  @attachments = Array.new
  #insert a blank attachment
  @attachments << ""
  Dir.entries(ATTACH_DIR).sort.each do |x|
    @attachments.push x unless x.match /\A\.+/
  end
end

def load_attachment filename
  return File.read(File.join(ATTACH_DIR,filename))
end

# # # # # # # # #
# MMS LISTENER  #
# # # # # # # # #
post '/mmslistener' do
  mms_listener
end

def display_images
  data = get_image_data

  @images_total = data[:image_count]
  @images_list = data[:image_list]
end

def mms_listener
  service = Service::MMSService.new(settings.FQDN, @@token)
  input   = request.env["rack.input"].read
  random  = rand(10000000).to_s
  service.handleInput(input) do |sender, date, text, type, image|
    File.open("#{settings.momms_image_dir}/#{random}.#{type}", 'w') { |f| f.puts image }
    File.open("#{settings.momms_data_dir}/#{random}.#{type}.txt", 'w') { |f| f.puts sender, date, text } 
  end
end

def get_image_data
  images = Array.new

  Dir.entries(settings.momms_image_dir).each do |entry|
    rel_path = File.join(settings.momms_image_dir, entry)
    if File.file? rel_path
      data = File.join(settings.momms_data_dir, entry + ".txt");
      if File.exists? data
        File.open(data, "r") do |f|
          images.push({:path => rel_path.sub("public/",""), :senderAddress => f.gets.strip, :date => f.gets.strip, :text => f.gets.strip}) 
        end
      end
    end
  end
  return {:image_count => images.length, :image_list => images}
end

def load_file(file)
  data = Array.new
  if File.exists? file 
    File.open(file, 'r') do |f|
      begin
        f.flock(File::LOCK_EX)
        f.each_line {|line| data << JSON.parse(line)}
      ensure
        f.flock(File::LOCK_UN)
      end
    end
  end
  data
end

def handle_inbound(file)
  input = request.env["rack.input"].read

  if !File.exists? file
    File.new(file, 'w')
  end

  contents = File.readlines(file)

  File.open(file, 'w') do |f|
    begin
      f.flock(File::LOCK_EX)
      #remove the first line if we're over limit
      if contents.size > settings.listener_limit - 1 
        offset = 1
      else
        offset = 0
      end
      f.puts contents[offset, contents.size] 
      f.puts JSON.parse(input).to_json
    ensure
      f.flock(File::LOCK_UN)
    end
  end
end
