# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

#!/usr/bin/ruby
require 'rubygems'
require 'json'
require 'base64'
require 'rest_client'
require 'sinatra'
require 'sinatra/config_file'
require File.join(File.dirname(__FILE__), 'common.rb')

enable :sessions

SCOPE = 'MIM,IMMN'

ATTACH_DIR = File.join(File.dirname(__FILE__), 'public/attachments')

config_file 'config.yml'

set :port, settings.port
set :protection, :except => :frame_options

def authorize
	# obtain an access token if necessary
	if session[:access_token] then
		redirect "#{settings.base_url}/getMessageHeader"
	else
		redirect "#{settings.FQDN}/oauth/authorize?client_id=#{settings.api_key}&scope=#{SCOPE}&redirect_uri=#{settings.redirect_url}"
	end
end

# perform the API call for getting message headers
def get_message_header
	if params[:indexCursorTextBox].nil? then
		url = "#{settings.FQDN}/rest/1/MyMessages?HeaderCount=" + session[:headerCountTextBox]
	else
		url = "#{settings.FQDN}/rest/1/MyMessages?HeaderCount=" + session[:headerCountTextBox] + "&IndexCursor" + session[:indexCursorTextBox]
	end

	RestClient.get url, :Authorization => "Bearer #{session[:access_token]}", :Accept => 'application/json', :Content_Type => 'application/json' do |response, request, code, &block|
		@r = response
	end

	if @r.code == 200
		@get_result = JSON.parse @r
	else
		@get_error = @r
	end

rescue => e
	@get_error = e.message
ensure
	return erb :immn
end

def get_message_content
	url = "#{settings.FQDN}/rest/1/MyMessages/" + session[:MessageId] + "/" + session[:PartNumber]

	RestClient.get url, :Authorization => "Bearer #{session[:access_token]}", :Accept => 'application/json', :Content_Type => 'application/json' do |response, request, code, &block|
		@r = response
	end

	if @r.code == 200
		@content_result = @r
		@headers = @content_result.headers[:content_type]
		content_string = @headers.split("; ")
		@image_string = @headers.split("/")
		@image = @image_string[0]
		@image_content = content_string[0]

	else
		@get_error = @r
	end

rescue => e
	@get_error = e.message
ensure
	return erb :immn
end


#													#
####  IMMN STARTS HERE ####
#													#

# perform the API call for sending messages
def send_messages
	if ((params[:attachment] && params[:attachment] != "None") || params[:groupCheckBox] != "false") then
		attachment = params[:attachment]
		addresses = ''
		invalidaddresses = ''

		params[:address].each do |p|
			if (p.match('^\d{10}$'))
				addresses += 'Addresses=tel:' + p + '&'
			elsif (p.scan('/^[^@]*@[^@]*\.[^@]*$/'))
				addresses += 'Addresses=' + p + '&'
			else
				invalidaddresses += p
			end
		end

		att_idx = 0

		@split = "----=_Part_0_#{((rand*10000000) + 10000000).to_i}.#{((Time.new.to_f) * 1000).to_i}"
		@contents = []

		data = "Content-Type: application/x-www-form-urlencoded; charset=UTF-8"
		data += "\nContent-Transfer-Encoding: 8bit"
		data += "\nContent-Disposition: form-data; name=\"root-fields\""
		data += "\nContent-ID: <startpart>"
		data += "\n\n"

		if params[:address].length > 0
			data += "#{addresses}" + 'Subject='+ "#{params[:subject]}" + '&Text='+ "#{params[:message]}" + '&Group='"#{params[:groupCheckBox]}"
		end

		data += "\n\n"
		@contents << data

		if attachment != "None" then
			temp = File.join(ATTACH_DIR, attachment)
			file = File.open(temp, "rb")

			data = "Content-Disposition: form-data; name=\"#{attachment[:name]}\"; filename=\"#{attachment}\""
			data += "\nContent-Type: file"
			data += "\nContent-ID: file"
			data += "\nContent-Transfer-Encoding: binary "
			@file_contents = File.read(file.path)
			attachment = @file_contents

			data += "\n\n#{attachment}"
			data += "\n"

			@contents << data

			file.close
			att_idx += 1
		end

		mimeContent = "--#{@split}\n" + @contents.join("--#{@split}\n") + "--#{@split}--\n"

		RestClient.post "#{settings.FQDN}/rest/1/MyMessages", "#{mimeContent}", :Authorization => "Bearer #{session[:access_token]}", :Accept => 'application/json', :Content_Type => 'multipart/related; type="application/x-www-form-urlencoded"; start="<startpart>"; boundary="' + @split + '"' do |response, request, code, &block|
			@mmsresp = response
		end

		if @mmsresp.code == 200
			session[:mms_id] = JSON.parse(@mmsresp)["Id"]
			@send_result = session[:mms_id]
		else
			@send_error = @mmsresp
			session[:mms_error_id] = @send_error
		end

	else
		addresses = ''
		invalidaddresses = ''
		params[:address].each do |p|
			if (p.match('^\d{8}$'))
				addresses += 'Addresses=short:' + p + '&'
			elsif (p.match('^\d{10}$'))
				addresses += 'Addresses=tel:' + p + '&'
			elsif (p.scan('/^[^@]*@[^@]*\.[^@]*$/'))
				addresses += 'Addresses=' + p + '&'
			else
				invalidaddresses += p
			end
		end

		if params[:address].length > 0
			data = "#{addresses}" + 'Subject='+ "#{params[:subject]}" + '&Text='+ "#{params[:message]}" + '&Group=false'
		end

		RestClient.post "#{settings.FQDN}/rest/1/MyMessages", "#{data}", :Authorization => "Bearer #{session[:access_token]}", :Accept => 'application/json', :Content_Type => 'application/x-www-form-urlencoded' do |response, request, code, &block|
			@smsresp = response
		end

		if @smsresp.code == 200
			session[:sms_id] = JSON.parse(@smsresp)["Id"]
			@send_result = session[:sms_id]
		else
			@send_error = @smsresp
			session[:sms_error_id] = @send_error   
		end
	end
	erb :immn
end

def prepare_and_send
	if params[:groupCheckBox].nil?
		params[:groupCheckBox] = "false"
	end

	session[:sms_id] = nil
	session[:mms_id] = nil
	session[:sms_error_id] = nil
	session[:mms_error_id] = nil

	addresses = params[:address].strip.split ","
	params[:entered_address] = params[:address]

	params[:address] = Array.new
	addresses.each do |address|
		a = parse_address(address)
		if a
			params[:address] << a
		end
	end

	if params[:address].length > 0
		send_messages
	else
		@send_error = 'Please enter in a valid phone number'
		return erb :immn
	end
end

#load all attachments present
def load_attachments
	@attachments = Array.new
	Dir.entries(ATTACH_DIR).sort.each do |x|
		@attachments.push x unless x.match /\A\.+/
	end
end

#																	 #
#### BEGIN SINATRA URL HANDLING ####
#																	 #

#before every link, load attachments
before do
	load_attachments
end

get '/' do
	#if code is present in params then we're returning from authentication
	if params[:code] then
		response = RestClient.post "#{settings.FQDN}/oauth/access_token?", :grant_type => "authorization_code", :client_id => settings.api_key, :client_secret => settings.secret_key, :code => params[:code]
		from_json = JSON.parse(response.to_str)
		session[:access_token] = from_json['access_token']
		if session[:sending] then
			session[:sending] = false
			redirect "#{settings.base_url}/sendMessage"
		elsif session[:getting] then
			session[:getting] = false
			redirect "#{settings.base_url}/getMessageHeader"
		end
	end
	return erb :immn
end

get '/sendMessage' do
	if session[:sms_id] != nil then
		@send_result = session[:sms_id]
	elsif session[:mms_id] != nil then
		@send_result = session[:mms_id]
	elsif session[:sms_error_id] != nil then
		@send_error = session[:sms_error_id]
	elsif session[:mms_error_id] != nil then
		@send_error = session[:mms_error_id]
	end
	#restore parameters
	params[:address] = session[:address] if session[:address]
	params[:message] = session[:message] if session[:message]
	params[:subject] = session[:subject] if session[:subject]
	#clear session variables
	session[:address] = nil
	session[:message] = nil
	session[:subject] = nil
	prepare_and_send
	erb :immn
end

post '/submit' do
	session[:sending] = true
	if session[:access_token] then
		prepare_and_send
	else
		#save parameters
		session[:address] = params[:address] if params[:address]
		session[:message] = params[:message] if params[:message]
		session[:subject] = params[:subject] if params[:subject]
		redirect "#{settings.FQDN}/oauth/authorize?client_id=#{settings.api_key}&scope=#{SCOPE}&redirect_uri=#{settings.redirect_url}"
	end
end

get '/getMessageHeader' do
	get_message_header
end

post '/submitGetHeaders' do
	session[:getting] = true
	session[:headerCountTextBox] = params[:headerCountTextBox]
	session[:indexCursorTextBox] = params[:indexCursorTextBox]
	authorize
end

post '/submitGetHeaderContent' do
	session[:getting] = true
	session[:MessageId] = params[:MessageId]
	session[:PartNumber] = params[:PartNumber]
	get_message_content
end
