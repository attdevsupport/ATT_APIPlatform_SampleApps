require 'json'
require 'immutable_struct'
require 'rexml/document'

module Att
  module Codekit
    module Model
      #@author kh455g
      class PaymentNotification < ImmutableStruct.new(:type, 
                                                      :timestamp, 
                                                      :effective,
                                                      :network_operator_id, 
                                                      :owner_id,
                                                      :purchase_date, 
                                                      :product_id,
                                                      :instance_id, 
                                                      :min_id, 
                                                      :old_min_id,
                                                      :sequence_number,
                                                      :purchase_activity_id,
                                                      :vendor_purchase_id, 
                                                      :reason_code,
                                                      :reason_message)


        def self.from_xml(xmlstr)
          xml_doc = REXML::Document.new xmlstr
          root = xml_doc.root
          attrs = root.attributes
          elem = root.elements

          type = attrs['type']
          timestamp = attrs['timestamp']
          effective = attrs['effective']

          network_operator_id = elem['networkOperatorId'].text unless elem['networkOperatorId'].nil?
          owner_id = elem['ownerIdentifier'].text unless elem['ownerIdentifier'].nil?
          purchase_date = elem['purchaseDate'].text unless elem['purchaseDate'].nil?
          product_id = elem['productIdentifier'].text unless elem['productIdentifier'].nil?
          instance_id = elem['instanceIdentifier'].text unless elem['instanceIdentifier'].nil?
          min_id = elem['minIdentifier'].text unless elem['minIdentifier'].nil?
          old_min_id = elem['oldMinIdentifier'].text unless elem['oldMinIdentifier'].nil?
          sequence_number = elem['sequenceNumber'].text unless elem['sequenceNumber'].nil?
          purchase_activity_id = elem['purchaseActivityIdentifier'].text unless elem['purchaseActivityIdentifier'].nil?
          vendor_purchase_id = elem['vendorPurchaseIdentifier'].text unless elem['vendorPurchaseIdentifier'].nil?
          reason_code = elem['reasonCode'].text unless elem['reasonCode'].nil?
          reason_message = elem['reasonMessage'].text unless elem['reasonMessage'].nil?
          
          new(type, timestamp, effective, network_operator_id, owner_id,
              purchase_date, product_id, instance_id, min_id, old_min_id,
              sequence_number, purchase_activity_id, vendor_purchase_id,
              reason_code, reason_message)
        end

        # Serializes instance to a json string
        def to_json(*a)
          {
            "json_class" => self.class.name,
            "data" => self.values
          }.to_json(*a)
        end

        # Decodes a json serialized object into a payment notification
        def self.json_create(o)
          new(*o["data"])
        end
      end

    end
  end
end
