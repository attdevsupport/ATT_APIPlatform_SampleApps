require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      # Contains the data returned for an ad
      class ADSResponse < ImmutableStruct.new(:type, :clickurl, :imageurl, :trackurl, :content)
        # @!attribute [r] type
        #   @return [String] the type of ad returned
        # @!attribute [r] clickurl
        #   @return [String] the url the ad should redirect to
        # @!attribute [r] imageurl
        #   @return [String] may be nil, can be used to display the ad
        # @!attribute [r] trackurl
        #   @return [String] 
        # @!attribute [r] content
        #   @return [String] may be nil, a content body that can display the ad

        # Were ads returned?
        #
        # @return [Boolean] true if ads are present in the response
        def hasAds?
          true
        end
        alias_method :has_ads?, :hasAds?

        # Does the ads response contain an imageurl?
        #
        # @return [Boolean] true if imageurl is present
        def imageurl?
          !!self.imageurl
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [ADSResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [ADSResponse] a parsed object
        def self.createFromParsedJson(json)
          root = json['AdsResponse']['Ads']

          type = root['Type']
          clickurl = root['ClickUrl']
          imageurl = root['ImageUrl']
          trackurl = root['TrackUrl']
          content = root['Content']

          new(type, clickurl, imageurl, trackurl, content)
        end
      end

      # Dummy wrapper class that indicates no ads were returned
      class NoAds  < ADSResponse
        # Were ads returned?
        #
        # @return [Boolean] true if ads are present in the response
        def hasAds?
          false
        end
        alias_method :has_ads?, :hasAds?
      end

    end
  end
end
