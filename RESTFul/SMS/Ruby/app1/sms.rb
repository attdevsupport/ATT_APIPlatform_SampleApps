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

# require codekit
require 'att/codekit'

include Att::Codekit

class SMS < Sinatra::Application
  get '/' do index; end
  get '/load' do load_data; end
  post '/save' do save_data; end
  post '/sendSms' do send_sms; end
  post '/getDeliveryStatus' do delivery_status; end
  post '/getMessages' do get_messages; end
  post '/loadNotifications' do load_notifications; end

  post '/statusListener' do 
    request.body.rewind
    input = JSON.parse(request.body.read)
    if input.is_a?(Hash) && input.include?("deliveryInfoNotification")
      handle_inbound settings.status_file
    end
  end
  post '/messageListener' do 
    request.body.rewind
    input = JSON.parse(request.body.read)
    if input.is_a?(Hash) && input.include?("DestinationAddress")
      handle_inbound settings.messages_file
    end
  end

  configure do
    enable :sessions
    config_file 'config.yml'
    SCOPE = "SMS"
    Transport.proxy(settings.proxy)

    # create files if doesn't exist
    set :tokens_file, Dir.tmpdir + "/ruby_sms_token"
    File.new(settings.tokens_file, 'w').close if !File.exists? settings.tokens_file
    set :status_file, Dir.tmpdir + "/ruby_sms_status"
    File.new(settings.status_file, 'w').close if !File.exists? settings.status_file
    set :messages_file, Dir.tmpdir + "/ruby_sms_messages"
    File.new(settings.messages_file, 'w').close if !File.exists? settings.messages_file

    OAuth = Auth::ClientCred.new(settings.FQDN, 
                                 settings.api_key,
                                 settings.secret_key)
    set :token, nil
  end

  #update listeners data before every request
  before do 
    begin
      #check if token exists and create if necessary
      settings.token = Auth::OAuthToken.load(settings.tokens_file) if settings.token.nil?
      if settings.token.nil?
        settings.token = OAuth.createToken(SCOPE)
        Auth::OAuthToken.save(settings.tokens_file, settings.token) 
      elsif settings.token.expired?
        settings.token = OAuth.refreshToken(settings.token)
        Auth::OAuthToken.save(settings.tokens_file, settings.token) 
      end

    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def index
    erb :sms
  end

  def send_sms
    begin
      service = Service::SMSService.new(settings.FQDN, settings.token)

      # set notify to a boolean based on checkbox exists or not in request
      notify = !!params[:deliveryNotificationStatus]
      message = "AT&T Sample Message"
      sms = service.sendSms(params[:address], message, notify)

      {
        :success => true,
        :tables => [{ 
          :caption => "Message sent successfully!",
          :headers => ["Message ID", "Resource URL"],
          :values => [[sms.id, sms.resource_url || '-']]
        }]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def delivery_status
    begin
      service = Service::SMSService.new(settings.FQDN, settings.token)

      msgid = params[:messageId]
      status = service.getDeliveryStatus(msgid)
      delivery_info = status.delivery_info

      infos = Array.new
      delivery_info.each do |info|
        infos << [info.id, info.address, info.status]
      end
      {
        :success => true,
        :tables => [{
          :caption => "Resource URL: #{status.resource_url}",
          :headers => ["Message ID", "Address", "Delivery Status"],
          :values => infos
        }]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_messages
    begin
      service = Service::SMSService.new(settings.FQDN, settings.token)
      message_list = service.getReceivedMessages(settings.short_code_check)

      values = Array.new
      message_list.messages.each do |msg|
        values << [msg.id, msg.sender, msg.message]
      end
      {
        :success => true,
        :tables => [
          {
            :caption => "Message List Information:",
            :headers => ["Number Of Messages In This Batch", 
                         "Resource Url", 
                         "Total Number Of Pending Messages"],
            :values => [[message_list.count, message_list.url,
                        message_list.pending]]
          },
          {
            :caption => "Messages",
            :headers => ["Message ID", "Sender Address", "Message"],
            :values => values
          }
        ]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def load_notifications
    status = load_json_file settings.status_file
    statuses = Array.new
    status.each do |s|
      info = s["deliveryInfoNotification"]
      dinfo = info["deliveryInfo"]
      statuses << [
        info["messageId"], dinfo["address"], dinfo["deliveryStatus"]
      ]
    end

    messages = load_json_file settings.messages_file
    msgs = Array.new
    messages.each do |msg|
      msgs << [
        msg["MessageId"], msg["DateTime"], msg["SenderAddress"],
        msg["DestinationAddress"], msg["Message"]
      ]
    end
    {
      :messages => msgs,
      :deliveryStatus => statuses,
    }.to_json
  end

  def load_json_file(path)
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

  def handle_inbound(path)
    request.body.rewind
    input = request.body.read

    File.open(path, 'w') do |f|
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
      :short_code_check => settings.short_code_check,
      :short_code_received => settings.short_code_received,
    }
    data[:savedData] = session[:savedData] unless session[:savedData].nil?
    data.to_json
  end

end
