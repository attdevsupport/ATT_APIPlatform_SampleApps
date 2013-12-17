# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      class IMMNResponse < ImmutableStruct.new(:id)
        # IMMN request response
        #
        # @!attribute [r] id
        #   @return [String] the id of the sent message

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [IMMNResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a json encoded string
        #
        # @return [IMMNResponse] a parsed object
        def self.createFromParsedJson(json)
          new(json["id"])
        end
      end

    end
  end
end
