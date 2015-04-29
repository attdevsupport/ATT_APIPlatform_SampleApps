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

# require codekit
require 'att/codekit'

# Simplify the namespace
include Att::Codekit
include Att::Codekit::Model

class AAB < Sinatra::Application
  get '/' do index; end
  get  '/load' do load_data; end
  get  '/authorization' do authorize; end
  post '/save' do save_data; end
  post '/createContact' do create_contact; end
  post '/updateContact' do update_contact; end
  post '/deleteContact' do delete_contact; end
  post '/getContacts' do search_contact; end
  post '/createGroup' do create_group; end
  post '/updateGroup' do update_group; end
  post '/deleteGroup' do delete_group; end
  post '/getGroups' do get_groups; end
  post '/getGroupContacts' do get_group_contacts; end
  post '/addContactsToGroup' do add_contacts; end
  post '/removeContactsFromGroup' do rm_contacts; end
  post '/getContactGroups' do get_contact_groups; end
  post '/getMyInfo' do get_myinfo; end
  post '/updateMyInfo' do update_myinfo; end

  use Rack::Session::Pool, :key => 'aab.ruby.session', :path => '/'

  configure do
    config_file 'config.yml'

    #Setup proxy used by att/codekit
    Transport.proxy(settings.proxy)

    SCOPE = "AAB"
    IMAGE_DIR = File.join(File.dirname(__FILE__), 'public/photos')

    AuthService = Auth::AuthCode.new(settings.FQDN,
                               settings.api_key,
                               settings.secret_key,
                               :scope => SCOPE,
                               :redirect => settings.redirect_url)
    set :token, nil
  end

  # Setup filters for saving session, 
  # loading select box data and catching the oauth redirect code
  before do
    begin
      if session[:token] && session[:token].expired?
        session[:token] = AuthService.refreshToken(session[:token])
      end
    rescue Exception => e
      { :success => false, :text => e.message }.to_json
    end
  end

  def authorize
    begin
      base_url = settings.redirect_url.split("authorization")[0]
      if params[:code].nil?
        raise Exception.new('Authentication Code not present')
      end
      session[:token] = AuthService.createToken(params[:code]) 
      redirect to(base_url)
    rescue Exception => e
      session[:savedData][:redirecting] = nil
      @main_page = base_url
      erb :error
    end
  end

  def index
    erb :aab
  end

  def create_contact
    service = Service::AABService.new(settings.FQDN, session[:token])

    phones = []
    get_complex_params(:createPhone, :Number) do |number, pref, type|
      phones << Phone.new(:number => number, :preferred => pref, :type => type)
    end
    emails = []
    get_complex_params(:createEmail, :Address) do |address, pref, type|
      emails << Email.new(:address => address, :preferred => pref, :type => type)
    end
    ims = []
    get_complex_params(:createIM, :Uri) do |uri, pref, type|
      ims << IM.new(:uri => uri, :preferred => pref, :type => type)
    end
    weburls = []
    get_complex_params(:createWeburl) do |url, pref, type|
      weburls << WebUrl.new(:url => url, :preferred => pref, :type => type)
    end
    addresses = []
    get_addresses(:createAddress) do |addr|
      addresses << addr
    end

    # Create a contact object
    new_contact = Contact.new(
      :first_name => params[:createFirstName],
      :last_name => params[:createLastName],
      :middle_name => params[:createMiddleName],
      :prefix => params[:createPrefix],
      :suffix => params[:createSuffix],
      :nickname => params[:createNickname],
      :organization => params[:createOrganization],
      :job_title => params[:createJobTitle],
      :anniversary => params[:createAnniversary],
      :gender => params[:createGender],
      :spouse => params[:createSpouse] ,
      :children => params[:createChildren],
      :hobby => params[:createHobby],
      :assistant => params[:createAssistant],
      :phones => phones,
      :addresses => addresses,
      :emails => emails,
      :ims => ims,
      :weburls => weburls,
    )

    begin
      created = service.createContact(new_contact)

      {
        :success => true,
        :text => created.location
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def update_contact
    service = Service::AABService.new(settings.FQDN, session[:token])

    phones = []
    get_complex_params(:updatePhone, :Number) do |number, pref, type|
      phones << Phone.new(:number => number, :preferred => pref, :type => type)
    end
    emails = []
    get_complex_params(:updateEmail, :Address) do |address, pref, type|
      emails << Email.new(:address => address, :preferred => pref, :type => type)
    end
    ims = []
    get_complex_params(:updateIM, :Uri) do |uri, pref, type|
      ims << IM.new(:uri => uri, :preferred => pref, :type => type)
    end
    weburls = []
    get_complex_params(:updateWeburl) do |url, pref, type|
      weburls << WebUrl.new(:url => url, :preferred => pref, :type => type)
    end
    addresses = []
    get_addresses(:updateAddress) do |addr|
      addresses << addr
    end
    update_contact = Contact.new(
      :first_name => params[:updateFirstName],
      :last_name => params[:updateLastName],
      :middle_name => params[:updateMiddleName],
      :prefix => params[:updatePrefix],
      :suffix => params[:updateSuffix],
      :nickname => params[:updateNickname],
      :organization => params[:updateOrganization],
      :job_title => params[:updateJobTitle],
      :anniversary => params[:updateAnniversary],
      :gender => params[:updateGender],
      :spouse => params[:updateSpouse] ,
      :children => params[:updateChildren],
      :hobby => params[:updateHobby],
      :assistant => params[:updateAssistant],
      :phones => phones,
      :addresses => addresses,
      :emails => emails,
      :ims => ims,
      :weburls => weburls,
    )

    begin
      updated = service.updateContact(params[:updateContactId], update_contact)

      {
        :success => true,
        :text => 'Successfully updated contact.'
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def delete_contact
    service = Service::AABService.new(settings.FQDN, session[:token])
    begin
      service.deleteContact(params[:deleteContactId])
      {
        :success => true,
        :text => 'Successfully deleted contact.'
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def search_contact
    service = Service::AABService.new(settings.FQDN, session[:token])

    begin
      results = service.getContacts(:search => session[:contactsSearchValue])
      contacts = results.contacts
      qcontacts = results.quick_contacts

      tables = []
      if contacts
        contacts.each do |contact|
          tables << make_contact_table(contact)
          contact_id = contact.contact_id
          tables << make_phones_table(contact_id, contact.phones) unless contact.phones.empty?
          tables << make_emails_table(contact_id, contact.emails) unless contact.emails.empty?
          tables << make_ims_table(contact_id, contact.ims) unless contact.ims.empty?
          tables << make_addresses_table(contact_id, contact.addresses) unless contact.addresses.empty?
          tables << make_weburls_table(contact_id, contact.weburls) unless contact.weburls.empty?
        end
      end

      if qcontacts
        qcontacts.each do |qcontact|
          tables << make_qcontact_table(qcontact)
          contact_id = qcontact.contact_id
          tables << make_phones_table(contact_id, qcontact.phone) unless qcontact.phone.nil?
          tables << make_emails_table(contact_id, qcontact.email) unless qcontact.email.nil?
          tables << make_ims_table(contact_id, qcontact.im) unless qcontact.im.nil?
          tables << make_addresses_table(contact_id, qcontact.address) unless qcontact.address.nil?
        end
      end
      
      {
        :success => true,
        :tables => tables
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def create_group
    service = Service::AABService.new(settings.FQDN, session[:token])

    group = Group.new(:name => params[:createGroupName])

    begin
      created = service.createGroup(group)

      {
        :success => true,
        :text => created.location
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def update_group
    service = Service::AABService.new(settings.FQDN, session[:token])

    group = Group.new(:name => params[:updateGroupName])

    begin
      updated = service.updateGroup(params[:updateGroupId], group)
      {
        :success => true,
        :text => 'Successfully updated group'
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def delete_group
    service = Service::AABService.new(settings.FQDN, session[:token])

    begin
      deleted = service.deleteGroup(params[:deleteGroupId])
      {
        :success => true,
        :text => 'Successfully deleted group'
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_groups
    service = Service::AABService.new(settings.FQDN, session[:token])

    begin
      results = service.getGroups(:name => params[:getGroupName],
                                  :order => params[:getGroupOrder])
      table = {
        :caption => 'Groups:',
        :headers => ['groupId', 'groupName', 'groupType'],
        :values => []
      }
      if results.groups?
        results.groups.each do |group|
          table[:values] << [ group.id, group.name, group.type ]
        end
      end
      {
        :success => true,
        :tables => [table]
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_group_contacts
    service = Service::AABService.new(settings.FQDN, session[:token])

    begin
      gid = params[:getContactsGroupId]
      raise Exception.new('Group Id must not be empty') if gid.empty?
      ids = service.getGroupContacts(gid)
      {
        :success => true,
        :tables => [{
          :caption => 'Contacts:',
          :headers => ['contactId'],
          :values => [ids],
        }]
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def add_contacts
    service = Service::AABService.new(settings.FQDN, session[:token])
    begin
      gid = params[:addContactsGroupId]
      cid = params[:addContactIds]
      if gid.empty?
        raise Exception.new('Group Id must not be empty')
      end
      if cid.empty?
        raise Exception.new('Contact Ids must not be empty')
      end
      added = service.addContactToGroup(gid, cid.split(','))
      {
        :success => true,
        :text =>  'Successfully added contacts to group.'
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def rm_contacts
    service = Service::AABService.new(settings.FQDN, session[:token])

    begin
      gid = params[:removeContactsGroupId]
      cid = params[:removeContactIds]
      if gid.empty?
        raise Exception.new('Group Id must not be empty')
      end
      if cid.empty?
        raise Exception.new('Contact Ids must not be empty')
      end
      removed = service.removeContactFromGroup(gid, cid.split(','))
      {
        :success => true,
        :text => 'Successfully removed contacts from group.'
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_contact_groups
    service = Service::AABService.new(settings.FQDN, session[:token])
    begin
      cid = params[:getGroupsContactId]
      if cid.empty?
        raise Exception.new('Contact Id must not be empty')
      end
      results = service.getContactGroups(cid)
      table = {
        :caption => 'Groups:',
        :headers => ['groupId', 'groupName', 'groupType'],
        :values => []
      }
      if results.groups?
        results.groups.each do |group|
          table[:values] << [group.id, group.name, group.type]
        end
      end
      {
        :success => true,
        :tables => [table]
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_myinfo
    service = Service::AABService.new(settings.FQDN, session[:token])

    begin
      myinfo = service.getMyInfo()
      tables = []
      tables << make_contact_table(myinfo)
      contact_id = myinfo.contact_id
      tables << make_phones_table(contact_id, myinfo.phones) unless myinfo.phones.empty?
      tables << make_emails_table(contact_id, myinfo.emails) unless myinfo.emails.empty?
      tables << make_ims_table(contact_id, myinfo.ims) unless myinfo.ims.empty?
      tables << make_addresses_table(contact_id, myinfo.addresses) unless myinfo.addresses.empty?
      tables << make_weburls_table(contact_id, myinfo.weburls) unless myinfo.weburls.empty?
      {
        :success => true,
        :tables => tables
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def update_myinfo
    phones = []
    get_complex_params(:myInfoPhone, :Number) do |number, pref, type|
      phones << Phone.new(:number => number, :preferred => pref, :type => type)
    end
    emails = []
    get_complex_params(:myInfoEmail, :Address) do |address, pref, type|
      emails << Email.new(:address => address, :preferred => pref, :type => type)
    end
    ims = []
    get_complex_params(:myInfoIM, :Uri) do |uri, pref, type|
      ims << IM.new(:uri => uri, :preferred => pref, :type => type)
    end
    weburls = []
    get_complex_params(:myInfoWeburl) do |url, pref, type|
      weburls << WebUrl.new(:url => url, :preferred => pref, :type => type)
    end
    addresses = []
    get_addresses(:myInfoAddress) do |addr|
      addresses << addr
    end

    me = MyContactInfo.new(
      :first_name => params[:myInfoFirstName],
      :last_name => params[:myInfoLastName],
      :middle_name => params[:myInfoMiddleName],
      :prefix => params[:myInfoPrefix],
      :suffix => params[:myInfoSuffix],
      :nickname => params[:myInfoNickname],
      :organization => params[:myInfoOrganization],
      :job_title => params[:myInfoJobTitle],
      :anniversary => params[:myInfoAnniversary],
      :gender => params[:myInfoGender],
      :spouse => params[:myInfoSpouse] ,
      :children => params[:myInfoChildren],
      :hobby => params[:myInfoHobby],
      :assistant => params[:myInfoAssistant],
      :phones => phones,
      :addresses => addresses,
      :emails => emails,
      :ims => ims,
      :weburls => weburls,
    )

    service = Service::AABService.new(settings.FQDN, session[:token])

    begin
      updated = service.updateMyInfo(me)
      {
        :success => true,
        :text => 'Successfully updated MyInfo'
      }.to_json
    rescue Exception => e
      puts e.backtrace
      { :success => false, :text => e.message }.to_json
    end
  end

  def get_complex_params(param_name, data_name=nil)
    count = params["#{param_name}Index"].to_i
    count.times do |i|
      data = params["#{param_name}#{data_name}#{i}"]
      pref = !!params["#{param_name}Pref#{i}"]
      type = params["#{param_name}Type#{i}"]
      yield(data, pref, type) unless data.to_s.empty?
    end
  end

  def get_addresses(param_name)
    count = params["#{param_name}Index"].to_i
    count.times do |i|
      pref = !!params["#{param_name}Pref#{i}"]
      type = params["#{param_name}Type#{i}"]
      pobox = params["#{param_name}PoBox#{i}"]
      addr1 = params["#{param_name}LineOne#{i}"]
      addr2 = params["#{param_name}LineTwo#{i}"]
      city = params["#{param_name}City#{i}"]
      state = params["#{param_name}State#{i}"]
      zip = params["#{param_name}Zip#{i}"]
      country = params["#{param_name}Country#{i}"]
      addr = Address.new(:type => type, :preferred => pref, :po_box => pobox,
                         :address_line_1 => addr1, :address_line_2 => addr2,
                         :city => city, :state => state, :zipcode => zip, 
                         :country => country)
      yield(addr) unless addr1.to_s.empty?
    end
  end

  def make_contact_table(c)
    {
      :caption => 'Contact:',
      :headers => [ 'contactId', 'creationDate', 'modificationDate',
                    'formattedName', 'firstName', 'lastName', 'prefix',
                    'suffix', 'nickName', 'organization', 'jobTitle',
                    'anniversary', 'gender', 'spouse', 'hobby', 'assistant' ],
      :values => [[ c.contact_id, c.creation_date, c.modification_date,
                    c.formatted_name, c.first_name, c.last_name, c.prefix,
                    c.suffix, c.nickname, c.organization, c.job_title,
                    c.anniversary, c.gender, c.spouse, c.hobby, c.assistant ]]
    }
  end

  def make_qcontact_table(q)
    table = {
      :caption => 'Quick Contact:',
      :headers => ['contactId', 'formattedName', 'firstName', 'middleName',
                   'lastName', 'prefix', 'suffix', 'nickName', 'organization'],
      :values => [[ q.contact_id, q.formatted_name, q.first_name,
                    q.middle_name, q.last_name, q.prefix, q.suffix, q.nickname,
                    q.organization ]]
    }
  end

  def make_phones_table(id, phones)
    table = { :caption => "Contact (#{id}) Phones:",
              :headers => [ 'type', 'number', 'preferred' ] }
    values = []
    Array(phones).each do |phone|
     values << [ phone.type, phone.number, phone.preferred || '-' ]
    end
    table[:values] = values
    table
  end

  def make_emails_table(id, emails)
    table = { :caption => "Contact (#{id}) Emails:",
              :headers => [ 'type', 'address', 'preferred' ] }
    values = []
    Array(emails).each do |email|
     values << [ email.type, email.address, email.preferred || '-' ]
    end
    table[:values] = values
    table
  end

  def make_ims_table(id, ims)
    table = { :caption => "Contact (#{id}) Ims:",
              :headers => [ 'type', 'uri', 'preferred' ] }
    values = []
    Array(ims).each do |im|
     values << [ im.type, im.uri, im.preferred || '-' ]
    end
    table[:values] = values
    table
  end

  def make_weburls_table(id, weburls)
    table = { :caption => "Contact (#{id}) Weburls:",
              :headers => [ 'type', 'url', 'preferred' ] }
    values = []
    Array(weburls).each do |weburl|
     values << [ weburl.type, weburl.url, weburl.preferred || '-' ]
    end
    table[:values] = values
    table
  end

  def make_addresses_table(id, addrs)
    table = { 
      :caption => "Contact (#{id}) Addresses:",
      :headers => [ 'type', 'preferred', 'poBox', 'addressLine1',
                    'addressLine2', 'city', 'state', 'zipcode', 'country' ]
    }
    values = []
    Array(addrs).each do |addr|
      values << [ addr.type, addr.preferred || '-', addr.address_line_1,
                  addr.address_line_2, addr.city, addr.state, addr.zipcode,
                  addr.country ]
    end
    table[:values] = values
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
    }
    data[:savedData] = session[:savedData] unless session[:savedData].nil?
    data.to_json
  end
end
#  vim: set ts=8 sw=2 sts=2 tw=79 ft=ruby et :
