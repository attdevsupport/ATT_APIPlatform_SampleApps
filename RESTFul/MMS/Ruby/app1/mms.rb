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
require 'cgi'
require File.join(File.dirname(__FILE__), 'common.rb')

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'MMS'
ATTACH_DIR=File.join(File.expand_path(File.dirname(__FILE__)), "public/attachments")

RestClient.proxy = settings.proxy

# setup filter fired before reaching our urls
# this is to ensure we are o-authenticated before actual action (like sendMms)

# autonomous version

['/sendMms', '/getStatus', '/sendCoupon','/checkStatus', ].each do |path|
  before path do
    obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
  end
end

before "/*" do |path|
  Dir.mkdir(settings.momms_image_dir) unless File.directory?(settings.momms_image_dir)
  Dir.mkdir(settings.momms_data_dir) unless File.directory?(settings.momms_data_dir)
  display_images
  drop_down_list
  @status_listener = load_file "#{settings.status_file}"
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
  session[:mms1_address] = params[:address]
  session[:mms1_subject] = params[:subject]

  notify = false
  notify = true if params[:chkGetOnlineStatus]

  #TODO: update for notifications
  send_mms params[:address], params[:subject], notify
end

post '/getStatus' do
  session[:mms_id] = params[:mmsId]
  get_delivery_status session[:mms_id]
end

# use this URL to clear token file
get '/clear' do
  File.delete settings.tokens_file if File.exists? settings.tokens_file
  redirect '/'
end

def send_mms(address, subject, notify=false)
  address = address.gsub("-","").split(",")
  addresses = Array.new

  address.each do |a|
    addresses << 'tel:'  + a.strip unless(a.include? "tel:" or a.include? "@")
  end

  # send in array if more than one otherwise string
  if addresses.size > 1
    address = addresses
  else
    address = addresses[0]
  end

  att_idx = 0

  @split = "----=_Part_0_#{((rand*10000000) + 10000000).to_i}.#{((Time.new.to_f) * 1000).to_i}"
  @contents = []

  payload = {
    :outboundMessageRequest => { :address => address, :subject => subject, :notifyDeliveryStatus => notify}
  }

  result = "Content-Type: application/json"
  result += "\nContent-ID: <startpart>"
  result += "\nContent-Disposition: form-data; name=\"root-fields\""
  result += "\n\n"
  result += payload.to_json
  result += "\n"

  @contents << result

  param = params[:attachment]
  if param && !param.empty?
    session[:selected] = param
    file_path = File.join(ATTACH_DIR,param)
    File.open(file_path, "rb") do |file|

      result = "Content-Type: image/gif"
      content_id = "<attachment#{att_idx}>"

      result += "\nContent-ID: #{content_id}"
      result += "\nContent-Transfer-Encoding: base64 "
      result += "\nContent-Disposition: attachment; name=\"\"; filename=\"\""
      attachment = Base64.encode64(file.read)

      result += "\n\n#{attachment}"
      result += "\n"

      @contents << result

    end
    att_idx += 1
  end

  mimeContent = "--#{@split}\n" + @contents.join("--#{@split}\n") + "--#{@split}--\n"


  response = RestClient.post "#{settings.FQDN}/mms/v3/messaging/outbox?", "#{mimeContent}", :Authorization => "Bearer #{@access_token}", :Accept => 'application/json', :Content_Type => 'multipart/form-data; type="application/json"; start=""; boundary="' + @split + '"'

  @mms_id = JSON.parse(response)['outboundMessageResponse']['messageId']
  @mms_url = JSON.parse(response)['outboundMessageResponse']['resourceReference']['resourceURL']
  session[:mms_id] = @mms_id unless notify

rescue => e
  @send_error = e.response
ensure
  return erb :mms
end

def get_delivery_status mmsid
  response = RestClient.get "#{settings.FQDN}/mms/v3/messaging/outbox/#{mmsid}?", :Authorization => "Bearer #{@access_token}", :Accept => 'application/json'

  delivery_info_list = JSON.parse(response)['DeliveryInfoList']
  @delivery_info = delivery_info_list['DeliveryInfo']
  @mms_url = delivery_info_list['ResourceUrl']

rescue => e
  @delivery_error = e.response
ensure
  return erb :mms
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
  input   = request.env["rack.input"].read
  address = /\<SenderAddress\>tel:([0-9\+]+)<\/SenderAddress>/.match(input)[1]
  parts   = input.split "--Nokia-mm-messageHandler-BoUnDaRy"
  body    = parts[2].split "BASE64"
  type    = /Content\-Type: image\/([^;]+)/.match(body[0])[1];
  date    = Time.now.utc

  random  = rand(10000000).to_s

  File.open("#{settings.momms_image_dir}/#{random}.#{type}", 'w') { |f| f.puts Base64.decode64 body[1] }

  # TODO: tokenizer stuff

  text = parts.length > 4 ? Base64.decode64(parts[3].split("BASE64")[1]).strip : ""
  File.open("#{settings.momms_data_dir}/#{random}.#{type}.txt", 'w') { |f| f.puts address, date, text } 
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
  if File.exists? file then
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
