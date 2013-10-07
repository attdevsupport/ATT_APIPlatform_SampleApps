module Att
  module Codekit
    module Model

      class ADSResponse < Struct.new(:type, :clickurl, :imageurl, :trackurl, :content)
        def hasAds?
          true
        end
        alias_method :has_ads?, :hasAds?

        def imageurl?
          !!self.imageurl
        end
      end

      class NoAds 
        def hasAds?
          false
        end
        alias_method :has_ads?, :hasAds?
      end

    end
  end
end
