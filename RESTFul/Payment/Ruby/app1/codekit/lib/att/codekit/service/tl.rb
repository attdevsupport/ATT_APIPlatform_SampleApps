# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'json'
require_relative '../model/tl'

module Att
  module Codekit
    module Service

      #@author kh455g
      class TLService < CloudService
        SERVICE_URL = "/2/devices/location"

        module Tolerance
          LOW = "LowDelay"
          NONE = "NoDelay"
          TOLERANT = "DelayTolerant"
        end

        # Return an authenticated devices location
        #
        # @param code [String] Authentication code returned by consent flow
        # @option opts [Integer] :req_accuracy requested accuracy in meters (Default: 100)
        # @option opts [Integer] :accept_accuracy acceptable accuracy in meters (Default: 10_000)
        # @option opts [String] :tolerance priority of response time vs accuracy (Default: Tolerance::LOW)
        # 
        # @return [Model::TLResponse] location object of the authenticated device
        def getDeviceLocation(opts={}) 
          req_accuracy = (opts[:req_accuracy] || 100)
          accept_accuracy = (opts[:accept_accuracy] || 10_000)
          tolerance = (opts[:tolerance] || Tolerance::LOW)
          url = "#{@fqdn}#{SERVICE_URL}"

          url << "?requestedAccuracy=#{req_accuracy}"
          url << "&acceptableAccuracy=#{accept_accuracy}"
          url << "&tolerance=#{tolerance}"

          begin
            response = self.get(url)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::TLResponse.createFromJson(response)
        end
        alias_method :getLocation, :getDeviceLocation

      end
    end
  end
end
