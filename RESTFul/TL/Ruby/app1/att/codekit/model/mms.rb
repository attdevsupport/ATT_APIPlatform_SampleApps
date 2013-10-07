require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      # Holds the response for sending a mms
      class MMSResponse < ImmutableStruct.new(:id, :resource_url)
        # @!attribute [r] id
        #   @return [String] the message id
        # @!attribute [r] resource_url 
        #   @return [String] the url which can be used for status

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [MMSResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a json encoded string
        #
        # @return [MMSResponse] a parsed object
        def self.createFromParsedJson(json)
          root = json["outboundMessageResponse"]

          id = root["messageId"]
          ref = root["resourceReference"]
          url = ref["resourceURL"]

          new(id, url)
        end
      end

      # Contains an array of info objects and resource url
      class MMSStatus < ImmutableStruct.new(:delivery_info, :resource_url)
        # @!attribute [r] delivery_info
        #   @return [Array<MMSDeliveryInfo>] a list of info objects that the 
        #     message was sent to
        # @!attribute [r] resource_url 
        #   @return [String] the url which can be used for status
        
        # Loop over each delivery info object
        #
        # @yield [MMSDeliveryInfo] the information relating to each delivery
        def each
          self.delivery_info.each {|x| yield x}
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [MMSStatus] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [MMSStatus] a parsed object
        def self.createFromParsedJson(json)
          root = json["DeliveryInfoList"]

          info_list = MMSDeliveryInfo.createFromParsedJson(root["DeliveryInfo"])
          url = root["ResourceUrl"]

          new(info_list, url)
        end
      end

      # Individual delivery info object
      class MMSDeliveryInfo < ImmutableStruct.new(:id, :address, :status)
        # @!attribute [r] id
        #   @return [String] the mms message id
        # @!attribute [r] address
        #   @return [String] address the message was sent to
        # @!attribute [r] status 
        #   @return [String] String representation of the message status

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Array<MMSDeliveryInfo>] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Array<MMSDeliveryInfo>] a parsed object
        def self.createFromParsedJson(json)
          list = Array.new
          json.each do |info|
            id = info["Id"]
            addr = info["Address"]
            status = info["DeliveryStatus"]

            list << new(id, addr, status)
          end
          list
        end
      end

      # Container for a single mms message
      class MMSMessage < ImmutableStruct.new(:sender, :date, :text, :type, :attachment) 
        # @!attribute [r] sender
        #   @return [String] the address of the message sender
        # @!attribute [r] date 
        #   @return [String] the date the message was sent
        # @!attribute [r] text 
        #   @return [String] the message's text
        # @!attribute [r] type 
        #   @return [String] the attachments content type
        # @!attribute [r] attachment 
        #   @return [String] the attachments of the message 
      end

    end
  end
end
