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

require 'base64'
require 'sinatra'
require 'sinatra/config_file'
require 'json'

# require as a gem file load relative if fails
begin
  require 'att/codekit'
rescue LoadError
  # load bundled codekit if not installed via gems
  require_relative 'codekit/lib/att/codekit'
end

include Att::Codekit

class IAM < Sinatra::Application
  get '/' do index; end
  get  '/load' do load_data; end
  post '/save' do save_data; end
  post '/sendMsg' do send_message; end
  post '/createIndex' do create_message_index; end
  post '/getMsgList' do get_message_list; end
  post '/getMsg' do get_message; end
  post '/getMsgContent' do get_message_content; end
  post '/getDelta' do get_delta; end
  post '/getMsgIndexInfo' do get_message_index_info; end
  post '/updateMsg' do update_message; end
  post '/delMsg' do delete_message; end
  post '/getNotiDetails' do get_notify_details; end
  post '/createSubscription' do create_subscription; end
  post '/updateSubscription' do update_subscription; end
  post '/getSubscription' do get_subscription; end
  post '/deleteSubscription' do delete_subscription; end
  post '/getNotifications' do get_notifications; end
  post '/notification' do notification; end

  #########################################
  ####### Configure the Application #######
  #########################################
  configure do
    enable :sessions
    disable :protection
    config_file 'config.yml'

    #Setup proxy used by att/codekit
    Transport.proxy(settings.proxy)

    SCOPE = ["IMMN", "MIM"]
    CHANNEL_SCOPE = "NOTIFICATIONCHANNEL"
    MSG_LIST_COUNT = 5
    ClientCred = Auth::ClientCred.new(settings.FQDN,
                                      settings.api_key,
                                      settings.secret_key)
    AuthService = Auth::AuthCode.new(settings.FQDN,
                                     settings.api_key,
                                     settings.secret_key,
                                     :scope => SCOPE,
                                     :redirect => settings.redirect_url)
    set :channel_token,  nil
    set :channel, nil
  end

  ##########################################
  ####### Setup filters for all urls #######
  ##########################################
  [ 
    '/save', '/load', '/sendMsg', '/createIndex', '/getMsgList', '/getMsg',
    '/getMsgContent', '/getDelta', '/getMsgIndexInfo', '/updateMsg', '/delMsg',
    '/getNotiDetails', '/createSubscription', '/updateSubscription',
    '/getSubscription', '/deleteSubscription', '/getNotifications' 
  ].each do |path|
    before path do
      begin
        # refresh token if it exist and is expired
        if session[:token] && session[:token].expired?
          session[:token] = AuthService.refreshToken(session[:token])
        end

      rescue Exception => e
        e.backtrace
        halt 200, { :success => false, :text => e.message }.to_json
      end
    end
  end

  before '/load' do
    begin
      load_channel
    rescue Exception => e
      puts e.backtrace
      halt 200, { :success => false, :text => e.message }.to_json
    end
  end

  def index
    begin
      load_channel
    rescue Exception => e
      puts e.backtrace
    end
    erb :immn
  end

  def load_channel
    if settings.channel_token.nil?
      settings.channel_token = ClientCred.createToken(CHANNEL_SCOPE)
    elsif settings.channel_token.expired?
      begin
        settings.channel_token = ClientCred.refreshToken(settings.channel_token)
      rescue
        settings.channel_token = ClientCred.createToken(CHANNEL_SCOPE)
      end
    end

    channel = load_file(settings.channel_file)
    settings.channel = JSON.load(channel) unless channel.empty?

    if settings.channel.nil?
      service = Service::Webhooks.new(settings.FQDN, settings.channel_token)
      begin
        # Try creating a new channel, delete if it exists
        c = service.createMIMNotificationChannel("application/json")
        settings.channel = service.getNotificationChannel(c.channel_id)
      rescue Exception => e
        jbody = JSON.parse(e.message)
        chanid = jbody["RequestError"]["PolicyException"]["Variables"].split(':')[-1].strip
        settings.channel = service.getNotificationChannel(chanid)
      ensure
        save_file(settings.channel_file, settings.channel.to_json)
      end
    end
  end

  get '/authorization' do
    begin
      session[:token] = AuthService.createToken(params[:code]) if params[:code]
    rescue Exception => e
      halt 200, e.message
    ensure
      base_url = settings.redirect_url.gsub("authorization","")
      redirect to(base_url)
    end
  end

  def send_message
    begin
      service = Service::IMMNService.new(settings.FQDN, session[:token])

      address = params[:address]
      group_checkbox = params[:groupCheckbox]
      message = params[:sendMsgInput]
      subject = params[:sendSubjectInput]

      # Sending in all optional parameters, service takes care of nil items
      response = service.sendMessage(address, 
                                     :message => message,
                                     :subject => subject, 
                                     :group => group_checkbox)

      { :success => true, :text => "id: #{response.id}" }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def create_message_index
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      service.createIndex

      { :success => true, :text => "Message index created." }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_message_list
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      # Set these to true(present) or false(nil)
      unread = !!params[:unread] 
      fav = !!params[:favorite] 
      inc = !!params[:incoming] 

      keyword = params[:keyword]

      msg_list = service.getMessageList(MSG_LIST_COUNT,
                                        :isUnread => unread,
                                        :isFavorite => fav,
                                        :isIncoming => inc,
                                        :keyword => session[:keyword])

      messages = []
      msg_list.messages.each do |msg|
        messages << replace_nils([ msg.id, msg.from, msg.recipients,
                                   msg.text, msg.timestamp, msg.favorite,
                                   msg.unread, msg.incoming, msg.type ])
      end

      # Format return in json to properly display
      { 
        :success => true, 
        :tables => [
          {
            :caption => 'Details:',
            :headers => [ 
              'Limit', 'Offset', 'Total', 'Cache Status', 
              'Failed Messages', 'State' 
            ],
            :values => [[ 
              msg_list.limit, msg_list.offset, msg_list.total,
              msg_list.cache_status, msg_list.failed_messages,
              msg_list.state 
            ]],
          },
          {
            :caption => 'Messages:',
            :headers => [
              'Message ID', 'From', 'Recipients', 'Text', 'Timestamp',
              'Favorite', 'Unread', 'Incoming', 'Type' 
            ],
            :values => messages,
          }
        ]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_message
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      message = service.getMessage(params[:getMsgId])

      # Format return in json to properly display
      { 
        :success => true, 
        :tables => [{
          :caption => 'Messages:',
          :headers => [
            'Message ID', 'From', 'Recipients', 'Text', 'Timestamp',
            'Favorite', 'Unread', 'Incoming', 'Type' 
          ],
          :values => [replace_nils([ message.id, message.from,
                                     message.recipients, message.text,
                                     message.timestamp, message.favorite,
                                     message.unread, message.incoming,
                                     message.type ])],
        }]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_message_content
    # make sure our required arguments are present
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      msg_id = params[:contentMsgId]
      part_number = params[:contentPartNumber]
      msg_content = service.getMessageContent(msg_id, part_number)

      r = { :success => true }
      if msg_content.text?
        r[:text] = "Message Content: #{msg_content.attachment}"
      elsif msg_content.video?
        r[:video] = { :type => msg_content.content_type,
                      :base64 => Base64.encode64(msg_content.attachment) }
      elsif msg_content.audio?
        r[:audio] = { :type => msg_content.content_type,
                      :base64 => Base64.encode64(msg_content.attachment) }
      elsif msg_content.image?
        r[:image] = { :type => msg_content.content_type,
                      :base64 => Base64.encode64(msg_content.attachment) }
      end
      r.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_delta
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      state = params[:msgState]
      response = service.getDelta(state)

      tables = []
      response.deltas.each do |delta|
        values = []
        delta.adds.each do |add|
          values << ['Add', add.id, add.favorite, add.unread]
        end
        delta.deletes.each do |delete|
          values << ['Delete', delete.id, delete.favorite, delete.unread]
        end
        delta.updates.each do |update|
          values << ['Update', update.id, update.favorite, update.unread]
        end
        tables << {
          'caption' => "Delta Type: #{delta.type}",
          'headers' => ['Delta Operation', 'MessageId', 'Favorite', 'Unread'],
          'values' => values,
        }
      end

      { :success => true, :tables => tables }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_message_index_info
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      index_info = service.getIndexInfo

      { 
        :success => true,
        :tables => [{
          :caption => 'Message Index Info:',
          :headers => ['Status', 'State', 'Message Count'],
          :values => [[index_info.status, index_info.state,
                       index_info.message_count]],
        }]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def update_message
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      unread = (params[:updateStatus] == 'Read' ? false : true)
      msgids = params[:updateMsgId].split(",")

      service.updateReadFlag(msgids, unread)

      { :success => true, :text => 'Message(s) Updated' }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def delete_message
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      msgids = params[:deleteMsgId].split(",")
      delete_msg = service.deleteMessage(msgids)

      { :success => true, :text => 'Message(s) Deleted' }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_notify_details
    begin
      service = Service::MIMService.new(settings.FQDN, session[:token])

      queues = params[:notificationType]
      details = service.getNotificationDetails(queues)

      {
        :success => true,
        :tables => [{
          :caption => 'Connection Details:',
          :headers => [:Username, :Password, :"Https Url", :"Wss Url", :Queues],
          :values => [[ details.username, details.password, details.https_url, 
                        details.wss_url, details.queues ]]
        }]
      }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def create_subscription
    begin
      if !!session[:subscription]
        raise Exception.new('You must first delete your existing subscription.')
      end

      events = []
      events << 'TEXT' unless params[:subscriptionText].nil?
      events << 'MMS' unless params[:subscriptionMms].nil?
      if events.empty?
        raise Exception.new("You must select at least one of Text or MMS")
      end

      service = Service::Webhooks.new(settings.FQDN, session[:token])

      callback_data = params[:callbackData]
      new_sub = Model::NotificationSubscription.new(events, callback_data,
                                                    settings.sub_expires_in)
      sub = service.createNotificationSubscription(settings.channel.channel_id,
                                                   new_sub)
      session[:subscription] = sub
      { :success => true, :text => 'Subscription created.' }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def update_subscription
    begin 
      current_sub = session[:subscription]
      unless !!current_sub
        raise Exception.new('You must first create a subscription.')
      end
      events = []
      events << 'TEXT' unless params[:updateSubscriptionText].nil?
      events << 'MMS' unless params[:updateSubscriptionMms].nil?
      if events.empty?
        raise Exception.new("You must select at least one of Text or MMS")
      end

      subid = current_sub.subscription_id
      service = Service::Webhooks.new(settings.FQDN, session[:token])

      callback_data = params[:updateCallbackData]
      callback_data = nil if callback_data.empty?
      update_sub = Model::NotificationSubscription.new(events, callback_data,
                                                       settings.sub_expires_in)
      sub = service.updateNotificationSubscription(settings.channel.channel_id,
                                                   subid, update_sub)
      session[:subscription] = sub
      { :success => true, :text => 'Subscription updated.' }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_subscription
    begin
      sub = session[:subscription]
      unless !!sub
        raise Exception.new('You must first create a subscription.')
      end

      service = Service::Webhooks.new(settings.FQDN, session[:token])
      details = service.getNotificationSubscription(settings.channel.channel_id,
                                                    sub.subscription_id)
      session[:subscription] = details
      {
        :success => true,
        :tables => [
          {
            :caption => 'Subscription Details',
            :headers => [ 'Subscription Id', 'Expires In', 
                          'Queues', 'Callback Data' ],
            :values => [[ details.subscription_id, details.expires_in, 
                          details.events, details.callback_data ]]
          }
        ]
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def delete_subscription
    begin
      sub = session[:subscription]
      unless !!sub
        raise Exception.new('You must first create a subscription.')
      end

      subid = sub.subscription_id
      service = Service::Webhooks.new(settings.FQDN, settings.channel_token)
      service.deleteNotificationSubscription(settings.channel.channel_id, subid)
      { :success => true, :text => 'Subscription deleted' }.to_json
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    ensure
      session[:subscription] = nil
    end
  end

  def get_notifications
    values = []
    sub = session[:subscription]
    unless !!sub
      halt 200, { :stopPolling => true}.to_json
    end
    readfile = load_file(settings.notifications_file)
    if !readfile.empty?
      notifications = JSON.parse(readfile) unless readfile.empty?
      usernotifs = notifications[sub.subscription_id] || []
      usernotifs.each do |notif|
        callback = notif['callbackData']
        events = notif['notificationEvents']
        events.each do |event|
          values << replace_nils([
            sub.subscription_id,
            callback,
            event['messageId'],
            event['conversationThreadId'],
            event['eventType'],
            event['text'],
            event['event'],
            event['isTextTruncated'],
            event['isFavorite'],
            event['isUnread'],
          ])
        end
      end
    end
    values.to_json
  end

  def notification
    begin
      if request.content_type == "application/json"
        request.body.rewind
        body = request.body.read
        readfile = load_file(settings.notifications_file)

        if readfile.empty?
          notifications = {}
        else
          notifications = JSON.parse(readfile)
        end
        notifs = JSON.parse(body)['messageNotifications']['subscriptionNotifications']
        notifs.each do |notif|
          subid = notif['subscriptionId']
          notifications[subid] ||= [] 
          notifications[subid] << notif
        end
        save_file(settings.notifications_file, notifications.to_json)
      end
    rescue Exception => e
      puts e.message
      puts e.backtrace
    else
      ""
    end
  end

  def replace_nils(arr)
    replaced = []
    arr.each do |item|
      if item.nil?
        replaced << "-"
      else
        replaced << item
      end
    end
    replaced
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
      :authenticated => !!session[:token],
      :redirect_url => AuthService.consentFlow,
      :server_time => Util::serverTime,
      :download => settings.download_link,
      :github => settings.github_link,
      :notificationChannel => {
        :channelId => settings.channel.channel_id,
        :channelType => settings.channel.channel_type,
        :maxEvents => settings.channel.max_events,
        :contentType => settings.channel.content_type
      },
      :subscriptionActive => !!session[:subscription],
    }
    data[:savedData] = session[:savedData] unless session[:savedData].nil?
    data.to_json
  end

  def save_file(file, json_data)
    if File.exist? file
      File.open(file, 'w') do |f|
        begin
          f.flock(File::LOCK_EX)
          f.puts json_data
        ensure
          f.flock(File::LOCK_UN)
        end
      end
    end
  end

  def load_file(file)
    if File.exist? file
      File.open(file,'r') do |f|
        begin
          f.flock(File::LOCK_EX)
          return f.read
        ensure
          f.flock(File::LOCK_UN)
        end
      end
    end
  end
end

#  vim: set ts=8 sw=2 sts=2 tw=79 ft=ruby et :
