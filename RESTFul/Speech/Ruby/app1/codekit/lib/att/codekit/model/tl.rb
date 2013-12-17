require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      # Location response object
      class TLResponse < ImmutableStruct.new(:timestamp, :accuracy, :latitude, :longitude)
        # @!attribute [r] timestamp
        #   @return [String] time of the location request
        # @!attribute [r] accuracy
        #   @return [String] accuracy of the request
        # @!attribute [r] latitude
        #   @return [String] the latitude of the location
        # @!attribute [r] longitude
        #   @return [String] the longitude of the location

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [TLResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [TLResponse] a parsed object
        def self.createFromParsedJson(json)
          root = json
          timestamp = root["timestamp"]
          accuracy = root["accuracy"]
          latitude = root["latitude"]
          longitude = root["longitude"]

          new(timestamp, accuracy, latitude, longitude)
        end
      end

    end
  end
end
