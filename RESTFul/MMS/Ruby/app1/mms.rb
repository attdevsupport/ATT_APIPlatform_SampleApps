#!/usr/bin/env ruby

# Copyright 2015 AT&T
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

# require codekit
require 'att/codekit'

#include namespace
include Att::Codekit

class MMS < Sinatra::Application
  get '/' do index; end
  get  '/load' do load_data; end
  post '/save' do save_data; end
  post '/sendMMS' do send_mms; end
  post '/getDeliveryStatus' do get_status; end
  post '/getNotifications' do get_notifications; end
  post '/statusListener' do save_status; end
  post '/mmslistener' do mms_listener; end

  #setup our mms service for the application
  configure do
    enable :sessions
    disable :protection
    config_file 'config.yml'

    SCOPE = 'MMS'
    Transport.proxy(settings.proxy)

    # create files if they don't exist
    set :tokens_file, File.join(Dir.tmpdir, "ruby_mms_token")
    File.new(settings.tokens_file, 'w').close unless File.exists? settings.tokens_file
    set :status_file, File.join(Dir.tmpdir, "ruby_mms_status")
    File.new(settings.status_file, 'w').close unless File.exists? settings.status_file
    set :messages_file, File.join(Dir.tmpdir, "ruby_mms_messages")
    File.new(settings.messages_file, 'w').close unless File.exists? settings.messages_file

    MMSMessage = Struct.new(:id, :date, :sender, :text, :path)
    IMAGE_DIR = File.join(File.dirname(__FILE__), 'public/attachments/')
    OAuth = Auth::ClientCred.new(settings.FQDN,
                                 settings.api_key,
                                 settings.secret_key)
    set :token, nil
    set :messages, []
  end

  ['/sendMMS', '/getDeliveryStatus'].each do |path| 
    before path do 
      begin
        #check if token exists and create if necessary
        if settings.token.nil? && File.exists?(settings.tokens_file)
          settings.token = Auth::OAuthToken.load(settings.tokens_file)
        end
        if settings.token.nil?
          settings.token = OAuth.createToken(SCOPE)
          Auth::OAuthToken.save(settings.tokens_file, settings.token)
        elsif settings.token.expired?
          settings.token = OAuth.refreshToken(settings.token)
          Auth::OAuthToken.save(settings.tokens_file, settings.token)
        end
      rescue Exception => e
        halt 401, { :success => false, :text => e.message }.to_json
      end
    end
  end

  def index
    erb :mms
  end

  def send_mms
    begin
      service = Service::MMSService.new(settings.FQDN, settings.token)
      address = params[:address]
      subject = params[:sendMsgInput]
      attachment_name = params[:attachmentInput]
      notify = !!params[:receiveStatus]

      allowed_files = ['None', 'att.gif', 'coupon.jpg']
      if !allowed_files.include? attachment_name
        raise Exception.new('Invalid attachment file specified')
      end

      attachment = image_path_by_id(attachment_name) unless attachment_name == 'None'

      sent = service.sendMms(address, subject, Array(attachment), notify)

      {
        :success => true,
        :tables => [{
          :headers => ['MessageId', 'ResourceURL'],
          :values => [[ sent.id, sent.resource_url || '-' ]]
        }]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_status
    begin 
      service = Service::MMSService.new(settings.FQDN, settings.token)
      mms_id = params[:msgId]

      status = service.getDeliveryStatus(mms_id)
      
      values = []
      status.delivery_info.each do |info|
        values << [info.id, info.address, info.status]
      end

      {
        :success => true,
        :text => "ResourceURL: #{status.resource_url}",
        :tables => [{
          :caption => 'Status:',
          :headers => ['MessageId', 'Address', 'DeliveryStatus'],
          :values => values
        }]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_notifications
    msgs = []
    settings.messages.each do |msg|
      msgs << {
        :id => msg.id,
        :date => msg.date,
        :address => msg.sender, 
        :text => msg.text || '-',
        :subject => '-',
        :image => msg.path
      }
    end
    statuses = []
    ss = load_status
    ss.each do |s|
      delivery = s['deliveryInfoNotification']
      msg_id = delivery['messageId']
      info = delivery['deliveryInfo']
      addr = info['address']
      status = info['deliveryStatus']
      statuses << [msg_id, addr, status]
    end
    { :mmsNotifications => msgs, :statusNotifications => statuses }.to_json
  end

  def mms_listener
    input = request.env["rack.input"].read
    msgs = settings.messages
    id = (msgs.length > 0 ? msgs.last.id + 1 : 0)
    mms = parse_mms(input)
    attach_path = write_binary_to_id(mms.attachment, id)
    msgs << MMSMessage.new(id, mms.date, mms.sender, mms.text, attach_path)
    if msgs.length > settings.listener_limit
      old_file = image_path_by_id(msgs.first.id)
      File.delete old_file if File.exists?(old_file)
      msgs = msgs.shift 
    end
  end

  def write_binary_to_id(binary, id)
    path = image_path_by_id(id)
    File.open(path, 'wb') do |f|
      begin
        f.flock(File::LOCK_EX)
        f.write(binary)
      ensure
        f.flock(File::LOCK_UN)
      end
      path.split("public/")[-1]
    end
  end

  def image_path_by_id(id)
    File.join(IMAGE_DIR, id.to_s)
  end

  def load_status
    path = settings.status_file
    data = Array.new
    File.open(path, 'r') do |f|
      begin
        f.flock(File::LOCK_SH)
        f.each_line {|line| data << JSON.parse(line)}
      ensure
        f.flock(File::LOCK_UN)
      end
    end
    data
  end

  def save_status
    request.body.rewind
    input = request.body.read
    path = settings.status_file

    File.open(path, 'w+') do |f|
      begin
        f.flock(File::LOCK_EX)
        contents = f.readlines
        #remove the first line if we're over limit
        if contents.size > settings.listener_limit - 1 
          offset = 1
        else
          offset = 0
        end
        f.rewind
        f.puts contents[offset, contents.size] 
        f.puts JSON.parse(input).to_json
      ensure
        f.flock(File::LOCK_UN)
      end
    end
  end

  def parse_mms(input)
    boundary = "--#{/content-type:.*;\s+boundary="([^"]+)"/i.match(input)[1]}"
    sender = /\<SenderAddress\>tel:([0-9\+]+)<\/SenderAddress>/.match(input)[1]
    date    = Time.now.utc
    parts   = input.split(boundary)

    attach_parts = parts[-2].split("\n\n")
    attach_type = /Content\-Type: image\/([^;]+)/i.match(attach_parts[0])[1];
    attach = Base64.decode64 attach_parts[-1]

    text = parts.length > 4 ? Base64.decode64(parts[2].split("\n\n")[1]).strip : ""

    Model::MMSMessage.new(sender, date, text, attach_type, attach)
  end

  ##################################
  ####### Save data in forms #######
  ##################################
  def save_data
    session['savedData'] = JSON.parse(params['data'])
    ""
  end

  def load_data
    data = {
      :authenticated => true,
      :server_time => Util::serverTime,
      :download => settings.download_link,
      :github => settings.github_link,
      :notificationShortcode => settings.shortcode,
    }
    data[:savedData] = session[:savedData] unless session[:savedData].nil?
    data.to_json
  end
end
