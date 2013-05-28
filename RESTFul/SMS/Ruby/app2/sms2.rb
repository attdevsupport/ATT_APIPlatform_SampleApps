
# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

#!/usr/bin/ruby
require 'rubygems'
require 'json'
require 'rest_client'
require 'sinatra'
require 'sinatra/config_file'
require File.join(File.dirname(__FILE__), 'common.rb')

enable :sessions

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

SCOPE = 'SMS'

['/receiveSms'].each do |path|
  before path do
    obtain_tokens(settings.FQDN, settings.api_key, settings.secret_key, SCOPE, settings.tokens_file)
  end
end

get '/' do
  session[:football_votes] ||= 0
  session[:baseball_votes] ||= 0
  session[:basketball_votes] ||= 0
  
  loadTallys

  erb :sms2
end

post '/smslistener' do
  sms_listener
end

get '/receiveSms' do
  receive_sms
end
  
# use this URL to clear token file
get '/clear' do
  File.delete settings.tokens_file if File.exists? settings.tokens_file
  redirect '/'
end

#update our tallys from the current votes
def loadTallys
  File.open 'tally1.txt', 'r+' do |f|
    session[:football_votes] = f.read.to_i
  end
  File.open 'tally2.txt', 'r+' do |f|
    session[:baseball_votes] = f.read.to_i
  end
  File.open 'tally3.txt', 'r+' do |f|
    session[:basketball_votes]= f.read.to_i
  end
  @received_total = session[:football_votes]+session[:baseball_votes]+session[:basketball_votes]
end

def receive_sms
  @votes = Array.new
  if File.exists? settings.votes_file 
    File.open settings.votes_file, 'r' do |f| 
      while (line = f.gets) do
        words = line.split
        msg_info = Hash.new
        msg_info[:date_time] = words[0]
        msg_info[:message_id] = words[1] 
        msg_info[:message] = words[2]
        msg_info[:sender] = words[3]
        msg_info[:destination] = words[4]
        
        @invalid_messages = []
        
        text = msg_info[:message]
        if text.downcase.eql? 'football'
           session[:football_votes] += 1
           @votes.push msg_info
        elsif text.downcase.eql? 'baseball' 
           session[:baseball_votes] += 1
           @votes.push msg_info
        elsif text.downcase.eql? 'basketball' 
           session[:basketball_votes] += 1
           @votes.push msg_info
        else 
           @invalid_messages.push msg_info
        end
        
        @received_total = session[:football_votes]+session[:baseball_votes]+session[:basketball_votes]

        File.open 'tally1.txt', 'w' do |fi|
          fi.write(session[:football_votes])
        end
        File.open 'tally2.txt', 'w' do |fi|
          fi.write(session[:baseball_votes])
        end
        File.open 'tally3.txt', 'w' do |fi|
          fi.write(session[:basketball_votes])
        end
      end
    end
  #files has been read so delete, such that we don't keep history
  File.delete settings.votes_file if File.exists? settings.votes_file
  else 
    loadTallys
  end
  rescue => e
  @received_error = e.response
ensure
  return erb :sms2
end


def sms_listener
  input = request.env["rack.input"].read

  File.open("#{settings.mosms_file_dir}/notifications", 'a+') { |f| f.puts input }
  
  sms_list = JSON.parse input
  
  @date_time = sms_list['DateTime']
  @message_id = sms_list['MessageId']
  @message = sms_list['Message']
  @sender = sms_list['SenderAddress']
  @destination = sms_list['DestinationAddress']
  
  #Append sms to file
  File.open("#{settings.mosms_file_dir}/vote_data", 'a+') { |f| f.puts @date_time + ' ' + @message_id + ' ' + @message + ' ' + @sender + ' ' + @destination }
  
  ensure
  return erb :sms2
end

