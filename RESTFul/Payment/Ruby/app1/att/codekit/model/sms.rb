require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      # Holds the response for sending a sms
      class SMSResponse < ImmutableStruct.new(:id, :resource_url)
        # @!attribute [r] id
        #   @return [String] the message id
        # @!attribute [r] resource_url 
        #   @return [String] the url which can be used for status

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [SMSResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [SMSResponse] a parsed object
        def self.createFromParsedJson(json)
          root = json["outboundSMSResponse"]

          id = root["messageId"]
          ref = root["resourceReference"]
          url = ref["resourceURL"] rescue nil

          new(id, url)
        end
      end

      # Contains an array of info objects and resource url
      class SMSStatus < ImmutableStruct.new(:delivery_info, :resource_url)
        # @!attribute [r] delivery_info
        #   @return [Array<#SMSDeliveryInfo>] a list of info objects that the 
        #     message was sent to
        # @!attribute [r] resource_url 
        #   @return [String] the url which can be used for status

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [SMSStatus] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [SMSStatus] a parsed object
        def self.createFromParsedJson(json)
          root = json["DeliveryInfoList"]

          info_list = SMSDeliveryInfo.createFromParsedJson(root["DeliveryInfo"])
          url = root["ResourceUrl"]

          new(info_list, url)
        end
      end

      # Individual delivery info object
      class SMSDeliveryInfo < ImmutableStruct.new(:id, :address, :status)
        # @!attribute [r] id
        #   @return [String] the sms message id
        # @!attribute [r] address
        #   @return [String] address the message was sent to
        # @!attribute [r] status 
        #   @return [String] String representation of the message status

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Array<SMSDeliveryInfo>] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Array<SMSDeliveryInfo>] a parsed object
        def self.createFromParsedJson(json)
          list = Array.new
          Array(json).each do |info|
            id = info["Id"]
            addr = info["Address"]
            status = info["DeliveryStatus"]

            list << new(id, addr, status)
          end
          list
        end
      end

      # Container for receiving sms messages
      class SMSMessageList < ImmutableStruct.new(:messages, :count, :pending, :url)
        # @!attribute [r] messages
        #   @return [Array<#SMSMessage>] list of message objects
        # @!attribute [r] count 
        #   @return [Integer] number of messages received
        # @!attribute [r] pending 
        #   @return [Integer] number of messages pending
        # @!attribute [r] url 
        #   @return [String] the resource url used to obtain the status

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [SMSMessageList] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [SMSMessageList] a parsed object
        def self.createFromParsedJson(json)
          root = json["InboundSmsMessageList"]

          messages = SMSMessage.createFromParsedJson(root["InboundSmsMessage"])
          num_msgs = root["NumberOfMessagesInThisBatch"].to_i
          pending = root["TotalNumberOfPendingMessages"].to_i
          url = root["ResourceUrl"]

          new(messages, num_msgs, pending, url) 
        end
      end

      # Container for a single sms message
      class SMSMessage < ImmutableStruct.new(:id, :message, :sender)
        # @!attribute [r] id
        #   @return [String] the message's id
        # @!attribute [r] message 
        #   @return [String] the message's text
        # @!attribute [r] sender 
        #   @return [String] the address of the message sender

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Array<SMSMessage>] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Array<SMSMessage>] a parsed object
        def self.createFromParsedJson(json)
          list = Array.new
          Array(json).each do |msg|
            id = msg["MessageId"]
            message = msg["Message"]
            sender = msg["SenderAddress"]
            list << new(id, message, sender)
          end
          list
        end
      end

    end
  end
end
