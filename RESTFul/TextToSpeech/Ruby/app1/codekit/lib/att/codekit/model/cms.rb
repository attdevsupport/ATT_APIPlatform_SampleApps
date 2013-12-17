require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      #Model for containing a cms request
      class CMSResponse < ImmutableStruct.new(:success, :id)
        #  Contains the response of a cms request
        #
        # @!attribute [r] success
        #   @return [Boolean] if the request was successful
        # @!attribute [r] id
        #   @return [String] id of the request
        def success?
          self.success
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [CMSResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [CMSResponse] a parsed object
        def self.createFromParsedJson(json)
          success = json['success']
          id = json['id']

          new(success, id)
        end
      end

      #Model for containing a signal request
      class SignalResponse < ImmutableStruct.new(:status)
        # Signal response
        #
        # @!attribute [r] status
        #   @return [String] the status of the requested signal

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [SignalResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Create an CMS signal response from json
        # 
        # @param json [Object] The decoded json string
        #
        # @return [SignalResponse] A parsed SignalResponse object
        def self.createFromParsedJson(json)
          status = json['status']

          new(status)
        end
      end

    end
  end
end
