# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'json'
require 'immutable_struct'


module Att
  module Codekit
    module Model

      class Contacts
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          root = (json["contact"] || json)

          contacts = Array.new
          Array(root).each do |contact|
            contacts << Contact.createFromParsedJson(contact)
          end
          contacts
        end
      end

      class Contact
        include Enumerable
        @@attributes = [:formatted_name, :first_name, :middle_name, :last_name, 
                        :prefix, :suffix, :nickname, :organization, :job_title, 
                        :anniversary, :gender, :spouse, :children, :hobby, 
                        :assistant, :phones, :addresses, :emails, :ims, 
                        :snapis, :weburls, :photo, :note, :contact_id, 
                        :creation_date, :modification_date ]

        @@attributes.each {|attr| attr_reader attr}

        def initialize(opt={})
          @formatted_name = (opt[:formattedName] || opt[:formatted_name])
          @first_name = (opt[:firstName] || opt[:first_name])
          @last_name = (opt[:lastName] || opt[:last_name])
          @middle_name = (opt[:middleName] || opt[:middle_name])
          @prefix = opt[:prefix]
          @suffix = opt[:suffix]
          @nickname = (opt[:nickName] || opt[:nickname])
          @organization = (opt[:organization] || opt[:org])
          @job_title = (opt[:jobTitle] || opt[:job_title])
          @anniversary = opt[:anniversary]
          @gender = opt[:gender]
          @spouse = opt[:spouse] 
          @children = opt[:children]
          @hobby = opt[:hobby]
          @assistant = opt[:assistant]
          @phones = opt[:phones]
          @addresses = opt[:addresses]
          @emails = opt[:emails]
          @ims = opt[:ims]
          @snapis = opt[:snapis]
          @weburls = opt[:weburls]
          @photo = opt[:photo]
          @contact_id = (opt[:contactId] || opt[:contact_id])
          @creation_date = (opt[:creation_date] || opt[:creationDate])
          @modification_date = (opt[:modification_date] || opt[:modificationDate])
        end

        def each
          self.each_pair do |name, value|
            yield value
          end
        end

        def each_pair
          @@attributes.each do |attr|
            value = self.send(attr)
            yield attr, value
          end
        end

        def base_contact
          contact = Hash.new
          contact[:formattedName] = @formatted_name unless @formatted_name.to_s.empty?
          contact[:firstName] = @first_name unless @first_name.to_s.empty?
          contact[:lastName] = @last_name unless @last_name.to_s.empty?
          contact[:middleName] = @middle_name unless @middle_name.to_s.empty?
          contact[:prefix] = @prefix unless @prefix.to_s.empty?
          contact[:suffix] = @suffix unless @suffix.to_s.empty?
          contact[:nickName] = @nickname unless @nickname.to_s.empty?
          contact[:organization] = @organization unless @organization.to_s.empty?
          contact[:anniversary] = @anniversary unless @anniversary.to_s.empty?
          contact[:jobTitle] = @job_title unless @job_title.to_s.empty?
          contact[:gender] = @gender unless @gender.to_s.empty?
          contact[:children] = @children unless @children.to_s.empty?
          contact[:spouse] = @spouse unless @spouse.to_s.empty?
          contact[:assistant] = @assistant unless @assistant.to_s.empty?
          contact[:hobby] = @hobby unless @hobby.to_s.empty?
          contact[:assistant] = @assistant unless @assistant.to_s.empty?

          contact[:addresses] = {
            :address => @addresses
          } unless @addresses.nil? 

          contact[:phones] = {
            :phone => @phones
          } unless @phones.nil?

          contact[:emails] = {
            :email => @emails
          } unless @emails.nil?

          contact[:ims] = {
            :im => @ims
          } unless @ims.nil?

          contact[:snapis] = {
            :snapis => @snapis
          } unless @snapis.nil?

          contact[:weburls] = {
            :webUrl => @weburls
          } unless @weburls.nil?

          contact[:photo] = @photo unless @photo.nil?

          contact
        end

        def to_json(*args)
          {
            :json_class => self.class.name,
            :contact => self.base_contact
          }.to_json(*args)
        end

        def self.json_create(o)
          self.createFromParsedJson(o)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end
        def self.createFromParsedJson(json)
          root = (json["contact"] || json)

          new({
            :first_name => root["firstName"],
            :last_name => root["lastName"],
            :middle_name => root["middleName"],
            :prefix => root["prefix"],
            :suffix => root["suffix"],
            :nickname => root["nickName"],
            :organization => root["organization"],
            :job_title => root["jobTitle"],
            :anniversary => root["anniversary"],
            :gender => root["gender"],
            :spouse => root["spouse"] ,
            :children => root["children"],
            :hobby => root["hobby"],
            :assistant => root["assistant"],
            :phones => Phones.createFromParsedJson(root["phones"]),
            :addresses => Addresses.createFromParsedJson(root["addresses"]),
            :emails => Emails.createFromParsedJson(root["emails"]),
            :ims => IMS.createFromParsedJson(root["ims"]),
            :snapis => Snapis.createFromParsedJson(root["snapis"]),
            :weburls => WebUrls.createFromParsedJson(root["weburls"]),
            :photo => Photo.createFromParsedJson(root["photo"]),
            :formatted_name => root["formattedName"],
            :contact_id => root["contactId"],
            :creation_date => root["creationDate"],
            :modification_date => root["modificationDate"],
          })
        end
      end

      class MyContactInfo < Contact
        def self.createFromParsedJson(json)
          root = (json["myInfo"] || json)
          super(root)
        end

        def to_json(*args)
          {
            :json_class => self.class.name,
            :myInfo => self.base_contact
          }.to_json(*args)
        end
      end

      class Addresses
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return Array.new if json.nil?

          root = (json["address"] || json)

          addresses = Array.new
          Array(root).each do |addr|
            addresses << Address.createFromParsedJson(addr)
          end
          addresses
        end
      end

      class Address 
        attr_reader :type, :preferred, :po_box, :address_line_1, 
          :address_line_2, :city, :state, :zipcode, :country

        def initialize(opts={})
          @type = opts[:type]
          @preferred = opts[:preferred]
          @po_box = opts[:po_box]
          @address_line_1 = opts[:address_line_1]
          @address_line_2 = opts[:address_line_2]
          @city = opts[:city]
          @state = opts[:state]
          @zipcode = opts[:zipcode]
          @country = opts[:country]
        end

        def preferred?
          !!@preferred
        end

        def to_json(*args)
          address = Hash.new
          address[:type] = self.type unless self.type.nil?
          address[:preferred] = self.preferred unless self.preferred.nil?
          address[:poBox] = self.po_box unless self.po_box.nil?

          unless self.address_line_1.nil?
            address[:addressLine1] = self.address_line_1 
          end
          unless self.address_line_2.nil?
            address[:addressLine2] = self.address_line_2
          end

          address[:city] = self.city unless self.city.nil?
          address[:state] = self.state unless self.state.nil?
          address[:zipcode] = self.zipcode unless self.zipcode.nil?
          address[:country] = self.country unless self.country.nil?

          address.to_json(*args)
        end

        def self.create_json(o)
          self.createFromParsedJson(o)
        end
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return nil if json.nil?

          root = json.is_a?(Array) ? json : (json["address"] || json)

          type = root["type"]
          pref = root["preferred"]
          po = root["poBox"]
          addr1 = root["addressLine1"]
          addr2 = root["addressLine2"]
          city = root["city"]
          state = root["state"]
          zip = root["zipcode"]
          country = root["country"]

          new(:type => type,
              :preferred => pref,
              :po_box => po,
              :address_line_1 => addr1,
              :address_line_2 => addr2,
              :city => city, 
              :state => state, 
              :zipcode => zip, 
              :country => country)
        end
      end

      class Emails
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return Array.new if json.nil?

          root = (json["email"] || json)

          emails = Array.new
          Array(root).each do |email|
            emails << Email.createFromParsedJson(email)
          end
          emails
        end
      end

      class Email 
        attr_reader :type, :email_address, :preferred

        def initialize(opts={})
          @type = opts[:type]
          @email_address = (opts[:email_address] || opts[:address])
          @preferred = opts[:preferred]
        end

        def address
          @email_address
        end

        def to_s
          str = "Type: #{@type}, #{@email_address}"
          @preferred == true ? "Preferred - #{str}" : str
        end

        def preferred?
          !!@preferred
        end

        def to_json(*args)
          email = Hash.new

          email[:type] = @type unless @type.nil?

          unless @email_address.nil?
            email[:emailAddress] = @email_address 
          end

          email[:preferred] = @preferred unless @preferred.nil?

          email.to_json(*args)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return nil if json.nil?

          root = json.is_a?(Array) ? json : (json["email"] || json)

          type = root["type"]
          addr = root["emailAddress"]
          pref = root["preferred"]

          new(:type => type, 
              :address => addr, 
              :preferred => pref)
        end
      end

      class IMS
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return Array.new if json.nil?

          root = (json["im"] || json)

          ims = Array.new
          Array(root).each do |im|
            ims << IM.createFromParsedJson(im)
          end
          ims
        end
      end

      class IM 
        attr_reader :type, :im_uri, :preferred

        def initialize(opts={})
          @type = opts[:type]
          @im_uri = (opts[:im_uri] || opts[:uri])
          @preferred = opts[:preferred]
        end

        def uri
          @im_uri
        end

        def to_s
          str = "Type: #{@type}, #{@im_uri}"
          @preferred == true ? "Preferred - #{str}" : str
        end

        def preferred?
          !!@preferred
        end

        def to_json(*args)
          im = Hash.new

          im[:type] = @type unless @type.nil?
          im[:imUri] = @im_uri unless @im_uri.nil?
          im[:preferred] = @preferred unless @preferred.nil?

          im.to_json(*args)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return nil if json.nil?

          root = json.is_a?(Array) ? json : (json["im"] || json)

          type = root["type"]
          uri = root["imUri"]
          pref = root["preferred"]

          new(:type => type, 
              :uri => uri,
              :preferred => pref)
        end
      end

      class Phones
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return Array.new if json.nil?

          root = (json["phone"] || json)

          phones = Array.new
          Array(root).each do |phone|
            phones << Phone.createFromParsedJson(phone)
          end
          phones
        end
      end

      class Phone 
        attr_reader :type, :number, :preferred

        def initialize(opts={})
          @type = opts[:type]
          @number = opts[:number]
          @preferred = opts[:preferred]
        end

        def to_json(*args)
          phone = Hash.new

          phone[:type] = @type unless @type.nil?
          phone[:number] = @number unless @number.nil?
          phone[:preferred] = @preferred unless @preferred.nil?

          phone.to_json(*args)
        end

        def to_s
          str = "Type: #{@type}, ##{@number}"
          @preferred == true ? "Preferred - #{str}" : str
        end

        def preferred?
          !!@preferred
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return nil if json.nil?

          root = json.is_a?(Array) ? json : (json["phone"] || json)

          type = root["type"]
          num = root["number"]
          pref = root["preferred"]

          new(:type => type, 
              :number => num,
              :preferred => pref)
        end

      end

      class WebUrls
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return Array.new if json.nil?

          root = (json["webUrl"] || json)

          weburls = Array.new
          Array(root).each do |weburl|
            weburls << WebUrl.createFromParsedJson(weburl)
          end
          weburls
        end
      end

      class WebUrl 
        attr_reader :type, :url, :preferred

        def initialize(opts={})
          @type = opts[:type]
          @url = opts[:url]
          @preferred = opts[:preferred]
        end

        def preferred?
          !!@preferred
        end

        def to_json(*args)
          weburl = Hash.new

          weburl[:type] = @type unless @type.nil?
          weburl[:url] = @url unless @url.nil?
          weburl[:preferred] = @preferred unless @preferred.nil?

          weburl.to_json(*args)
        end
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return nil if json.nil?

          root = json.is_a?(Array) ? json : (json["weburl"] || json)

          type = root["type"]
          url = root["url"]
          pref = root["preferred"]

          new(:type => type, 
              :url => url, 
              :preferred => pref)
        end
      end

      class Snapis
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return Array.new if json.nil?

          root = (json["snapis"] || json)

          snapis = Array.new
          Array(root).each do |snapi|
            snapis << Snapi.createFromParsedJson(snapi)
          end
          snapis
        end
      end

      class Snapi 
        attr_reader :type, :uri, :preferred

        def initialize(opts={})
          @type = opts[:type]
          @uri = opts[:uri]
          @preferred = opts[:preferred]
        end

        def preferred?
          !!@preferred
        end

        def to_json(*args)
          snapi = Hash.new

          snapi[:type] = @type unless @type.nil?
          snapi[:uri] = @uri unless @uri.nil?
          snapi[:preferred] = @preferred unless @preferred.nil?

          snapi.to_json(*args)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return nil if json.nil?

          root = json.is_a?(Array) ? json : (json["snapi"] || json)

          type = root["type"]
          uri = root["uri"]
          preferred = root["preferred"]

          new(:type => type,
              :uri => uri,
              :preferred => preferred)
        end
      end

      class Photo 
        attr_reader :encoding, :value

        def initialize(opts={})
          @encoding = opts[:encoding]
          @value = opts[:value]
        end

        def to_json(*args)
          photo = Hash.new

          photo[:encoding] = @encoding unless @encoding.nil?
          photo[:value] = @value unless @value.nil?

          photo.to_json(*args)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return nil if json.nil?

          root = json.is_a?(Array) ? json : (json["photo"] || json)

          encoding = root["encoding"]
          value = root["value"]

          new(:encoding => encoding,
              :value => value)
        end
      end

      class Groups
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return Array.new if json.nil?

          root = nil
          if json["groups"]
            root = json["groups"]["group"]
          else
            root = (json["group"] || json)
          end

          groups = Array.new
          root.each do |group|
            groups << Group.createFromParsedJson(group)
          end
          groups
        end
      end

      class Group 
        attr_reader :group_id, :group_name, :group_type

        def initialize(opts={})
          @group_id   = (opts[:group_id] || opts[:id])
          @group_name = (opts[:group_name] || opts[:name])  
          @group_type = (opts[:group_type] || opts[:type])
        end

        def id
          @group_id
        end

        def name
          @group_name
        end

        def type
          @group_type
        end

        def to_json(*args)
          group = Hash.new

          group[:groupId] = @group_id unless @group_id.nil?
          group[:groupName] = @group_name unless @group_name.nil?
          group[:groupType] = @group_type unless @group_type.nil?

          { 
            :json_class => self.class.name,
            :group => group 
          }.to_json(*args)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          return nil if json.nil?

          root = json

          gid = root["groupId"]
          gname = root["groupName"]
          gtype = root["groupType"]

          new(:group_id => gid,
              :group_name => gname,
              :group_type => gtype)
        end
      end

      class QuickContacts
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          root = json.is_a?(Array) ? json : (json["quickContact"] || json)

          qcontacts = Array.new
          Array(root).each do |q|
            qcontacts << QuickContact.createFromParsedJson(q)
          end
          qcontacts
        end
      end

      class QuickContact < ImmutableStruct.new(:contact_id, 
                                               :formatted_name,
                                               :first_name,
                                               :middle_name,
                                               :last_name,
                                               :prefix,
                                               :suffix,
                                               :nickname,
                                               :organization,
                                               :phone,
                                               :email,
                                               :im,
                                               :address
                                              )
        def id
          self.contact_id
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          root = json.is_a?(Array) ? json : (json["quickContact"] || json)

          cid = root["contactId"]
          org = root["organization"]
          form_name = root["formattedName"]
          fname = root["firstName"]
          mname = root["middleName"]
          lname = root["lastName"]
          prefix = root["prefix"]
          suffix = root["suffix"]
          nick = root["nickName"]

          phone = Phone.createFromParsedJson(root["phone"])
          email = Email.createFromParsedJson(root["email"])
          im = IM.createFromParsedJson(root["im"])
          address = Address.createFromParsedJson(root["address"])

          new(cid, form_name, fname, mname, lname, prefix, 
              suffix, nick, org, phone, email, im, address)
        end
      end

      class GroupsResultSet < ImmutableStruct.new(:total_records,
                                                  :total_pages,
                                                  :current_page_index,
                                                  :previous_page,
                                                  :next_page,
                                                  :groups
                                                 )
        def groups?
          !!self.groups
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          root = (json["resultSet"] || json)

          trecs = root["totalRecords"]
          tpages = root["totalPages"]
          cpage_index = root["currentPageIndex"]
          prev_page = root["previousPage"]
          next_page = root["nextPage"]

          if root["groups"]
            groups = Groups.createFromParsedJson(root["groups"])
          else
            groups = nil
          end

          new(trecs, tpages, cpage_index, prev_page, next_page, groups)
        end
      end

      class ContactsResultSet < ImmutableStruct.new(:total_records,
                                                    :total_pages,
                                                    :current_page_index,
                                                    :previous_page,
                                                    :next_page,
                                                    :contacts,
                                                    :quick_contacts
                                                   )
        def contacts?
          !!self.contacts
        end
        def quick_contacts?
          !!self.quick_contacts
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          root = json.is_a?(Array) ? json : (json["resultSet"] || json)

          trecs = root["totalRecords"]
          tpages = root["totalPages"]
          cpage_index = root["currentPageIndex"]
          prev_page = root["previousPage"]
          next_page = root["nextPage"]

          if root["contacts"]
            contact = Contacts.createFromParsedJson(root["contacts"])
          else
            contact = nil
          end

          if root["quickContacts"]
            quick = QuickContacts.createFromParsedJson(root["quickContacts"])
          else
            quick = nil
          end

          new(trecs, tpages, cpage_index, prev_page, next_page, contact, quick)
        end
      end

    end
  end
end

#  vim: set ts=8 sw=2 tw=80 et :
