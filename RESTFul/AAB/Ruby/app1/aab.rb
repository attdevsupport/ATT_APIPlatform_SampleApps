#!/usr/bin/env ruby

# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'base64'
require 'sinatra'
require 'sinatra/config_file'

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

include Att::Codekit
include Att::Codekit::Model

#regular sessions cause a problem with large amounts of data
use Rack::Session::Pool

config_file 'config.yml'

set :port, settings.port

IMAGE_DIR = File.join(File.dirname(__FILE__), 'public/photos')

#Setup proxy used by att/codekit
Transport.proxy(settings.proxy)

SCOPE = "AAB"

configure do
  OAuth = Auth::AuthCode.new(settings.FQDN,
                             settings.api_key,
                             settings.secret_key)

  AUTHENTICATING = :from_auth

  special_case = Struct.new(:name, :parameters)

  CONTACT_PARAMS = [ :firstName, :middleName, :lastName, :prefix, :suffix,
                     :nickname, :organization, :jobTitle, :anniversary,
                     :gender, :spouse, :children, :hobby, :assistant, 
                     :attachPhotoMyInf, :contactId,
                     special_case.new(:phone, [:number, :type]),
                     special_case.new(:im, [:uri, :type]),
                     special_case.new(:email, [:emailAddress, :type]),
                     special_case.new(:weburl, [:url, :type]),
                     special_case.new(:address, 
                                      [:pobox, :pobox, :addressLine1, 
                                       :addressLine2, :city, :country, 
                                       :state, :zip, :type])
  ]

  SEARCH_PARAMS = [ :searchVal ]
  GROUP_PARAMS = [ :groupId, :groupName, :groupType, :order, :contactIds ]
end

# Setup filters for saving session, 
# loading select box data and catching the oauth redirect code
before do
  load_attachments

  begin
    if params[:code] && session[:token].nil?
      session[:token] = OAuth.createToken(params[:code])
    end
    if session[:token] && session[:token].expired?
      session[:token] = OAuth.refreshToken(session[:token])
    end
  rescue Exception => e
    @oauth_error = e.message
  end
end

get '/' do
  if params[:error]
    @error = params[:error]
  end
  erb :aab
end

get '/createContact' do create_contact; end
post '/createContact' do create_contact; end
def create_contact
  save_session(CONTACT_PARAMS) 
  authenticate("createContact")

  # Note that saving the session creates our complex objects such as phones
  contact = Contact.new(
    :first_name => session[:firstName],
    :last_name => session[:lastName],
    :middle_name => session[:middleName],
    :prefix => session[:prefix],
    :suffix => session[:suffix],
    :nickname => session[:nickname],
    :organization => session[:organization],
    :job_title => session[:jobTitle],
    :anniversary => session[:anniversary],
    :gender => session[:gender],
    :spouse => session[:spouse] ,
    :children => session[:children],
    :hobby => session[:hobby],
    :assistant => session[:assistant],
    :phones => session[:phone],
    :addresses => session[:address],
    :emails => session[:email],
    :ims => session[:im],
    :weburls => session[:weburl],
    :photo => session[:photo]
  )

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @create_contact = service.createContact(contact)
  rescue Exception => e
    @contact_error = e
    puts e.backtrace
  end
  clear_session(CONTACT_PARAMS) 
  erb :aab
end

get '/updateContact' do update_contact; end
post '/updateContact' do update_contact; end
def update_contact
  save_session(CONTACT_PARAMS) 
  authenticate("updateContact")

  # Note that saving the session creates our complex objects such as phones
  contact = Contact.new(
    :first_name => session[:firstName],
    :last_name => session[:lastName],
    :middle_name => session[:middleName],
    :prefix => session[:prefix],
    :suffix => session[:suffix],
    :nickname => session[:nickname],
    :organization => session[:organization],
    :job_title => session[:jobTitle],
    :anniversary => session[:anniversary],
    :gender => session[:gender],
    :spouse => session[:spouse] ,
    :children => session[:children],
    :hobby => session[:hobby],
    :assistant => session[:assistant],
    :phones => session[:phone],
    :addresses => session[:address],
    :emails => session[:email],
    :ims => session[:im],
    :weburls => session[:weburl],
    :photo => session[:photo]
  )

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @success_contact = service.updateContact(session[:contactId], contact)
  rescue Exception => e
    @contact_error = e
    puts e.backtrace
  end
  clear_session(CONTACT_PARAMS) 
  erb :aab
end


get '/searchContact' do search_contact; end
post '/searchContact' do search_contact; end
def search_contact
  save_session(SEARCH_PARAMS)
  authenticate("searchContact")

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @contact_resultset = service.getContacts(
      :search => session[:searchVal]
    )
  rescue Exception => e
    @contact_error = e
    puts e.backtrace
  end
  clear_session(SEARCH_PARAMS)
  erb :aab
end

get '/createGroup' do create_group; end
post '/createGroup' do create_group; end
def create_group
  save_session(GROUP_PARAMS)
  authenticate("createGroup")

  service = Service::AABService.new(settings.FQDN, session[:token])

  group = Group.new(:name => session[:groupName])

  begin
    @create_group = service.createGroup(group)
  rescue Exception => e
    @group_error = e
    puts e.backtrace
  end
  clear_session(GROUP_PARAMS)
  erb :aab
end

get '/updateGroup' do update_group; end
post '/updateGroup' do update_group; end
def update_group
  save_session(GROUP_PARAMS)
  authenticate("updateGroup")

  service = Service::AABService.new(settings.FQDN, session[:token])

  group = Group.new(:name => session[:groupName])

  begin
    @success_group = service.updateGroup(session[:groupId], group)
  rescue Exception => e
    @group_error = e
    puts e.backtrace
  end
  clear_session(GROUP_PARAMS)
  erb :aab
end

get '/deleteGroup' do delete_group; end
post '/deleteGroup' do delete_group; end
def delete_group
  save_session(GROUP_PARAMS)
  authenticate("deleteGroup")

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @success_group = service.deleteGroup(session[:groupId])
  rescue Exception => e
    @group_error = e
    puts e.backtrace
  end
  clear_session(GROUP_PARAMS)
  erb :aab
end

get '/getGroups' do get_groups; end
post '/getGroups' do get_groups; end
def get_groups
  save_session(GROUP_PARAMS)
  authenticate("getGroups")

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @groups = service.getGroups(:name => session[:groupName],
                                :order => session[:order])
  rescue Exception => e
    @group_error = e
    puts e.backtrace
  end
  clear_session(GROUP_PARAMS)
  erb :aab
end

get '/getGroupContacts' do get_group_contacts; end
post '/getGroupContacts' do get_group_contacts; end
def get_group_contacts
  save_session(GROUP_PARAMS)
  authenticate("getGroupContacts")

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @group_contact_ids = service.getGroupContacts(session[:groupId])
  rescue Exception => e
    @manage_groups_error = e
    puts e.backtrace
  end
  clear_session(GROUP_PARAMS)
  erb :aab
end

get '/addContactsToGroup' do add_contacts; end
post '/addContactsToGroup' do add_contacts; end
def add_contacts
  save_session(GROUP_PARAMS)
  authenticate("addContactsToGroup")

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @manage_groups = service.addContactToGroup(session[:groupId], 
                                               session[:contactIds])
  rescue Exception => e
    @manage_groups_error = e
    puts e.backtrace
  end
  clear_session(GROUP_PARAMS)
  erb :aab
end

get '/rmContactsFromGroup' do rm_contacts; end
post '/rmContactsFromGroup' do rm_contacts; end
def rm_contacts
  save_session(GROUP_PARAMS)
  authenticate("rmContactsFromGroup")

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @manage_groups = service.addContactToGroup(session[:groupId], 
                                               session[:contactIds])
  rescue Exception => e
    @manage_groups_error = e
    puts e.backtrace
  end
  clear_session(GROUP_PARAMS)
  erb :aab
end

get '/getContactGroups' do get_contact_groups; end
post '/getContactGroups' do get_contact_groups; end
def get_contact_groups
  save_session([:contactId])
  authenticate("getContactGroups")

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @contact_groups = service.getContactGroups(session[:contactId])
  rescue Exception => e
    @manage_groups_error = e
    puts e.backtrace
  end
  clear_session([:contactId])
  erb :aab
end

get '/updateMyInfo' do update_myinfo; end
post '/updateMyInfo' do update_myinfo; end
def update_myinfo
  save_session(CONTACT_PARAMS) 
  authenticate("updateMyInfo")

  # Note that saving the session creates our complex objects such as phones
  me = MyContactInfo.new(
    :first_name => session[:firstName],
    :last_name => session[:lastName],
    :middle_name => session[:middleName],
    :prefix => session[:prefix],
    :suffix => session[:suffix],
    :nickname => session[:nickname],
    :organization => session[:organization],
    :job_title => session[:jobTitle],
    :anniversary => session[:anniversary],
    :gender => session[:gender],
    :spouse => session[:spouse] ,
    :children => session[:children],
    :hobby => session[:hobby],
    :assistant => session[:assistant],
    :phones => session[:phone],
    :addresses => session[:address],
    :emails => session[:email],
    :ims => session[:im],
    :weburls => session[:weburl],
    :photo => session[:photo]
  )

  service = Service::AABService.new(settings.FQDN, session[:token])

  begin
    @update_myinfo = service.updateMyInfo(me)
  rescue Exception => e
    @myinfo_error = e
    puts e.backtrace
  end
  clear_session(CONTACT_PARAMS) 
  erb :aab
end

def authenticate(url)
  session[AUTHENTICATING] = true
  if session[:token].nil?
    #remove the leading / from action if redirect ends with /
    suburl = settings.redirect_url.end_with?("/") ? url : "/#{url}"

    redirect_url = "#{settings.redirect_url}#{suburl}"
    redirect OAuth.consentFlow(:scope => SCOPE, :redirect => redirect_url) 
  end
  session[AUTHENTICATING] = false
end

def load_attachments
  # make first box blank
  @attachments = [""] 
  Dir.entries(IMAGE_DIR).sort.each do |x|
    #add file, filter names with one or more leading '.'
    @attachments.push x unless x.match /\A\.+/
  end
end

def clear_session(parameters)
  parameters.each do |p|
    if p.is_a? Struct
      session[p.name] = nil
    else
      session[p] = nil
    end
  end
end

def save_session(parameters)
  return if session[AUTHENTICATING]
  parameters.each do |p|
    if p.is_a? Struct
      if params[p.name]
        session[p.name] = create_object(p.name, p.parameters)
      end
    else
      session[p] = params[p]
    end
  end
end

def create_object(name, parameters)
  objs = Array.new

  #special objects are mapped via html forms with the following notation.
  #    address[0]{parameter => value}
  #    address[1]{parameter => value}
  params[name].each_index do |i|

    # item will equal be the {parameter => value} as described above
    item = params[name][i]

    opts = Hash.new

    # Test if the current index is the preferred index
    opts[:preferred] = params["#{name}Pref"].to_i == i

    parameters.each do |p|
      opts[p] = item[p]
    end

    objs << case name
    when :phone
      Phone.new(opts)
    when :address
      Address.new(opts)
    when :im
      IM.new(opts)
    when :email
      Email.new(opts)
    when :weburl
      WebUrl.new(opts)
    end
  end

  objs
end

#  vim: set ts=8 sw=2 tw=80 et :
