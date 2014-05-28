# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'cgi'
require_relative '../model/ads'

module Att
  module Codekit
    module Service

      #@author kh455g
      class ADSService < CloudService
        SERVICE_URL = "/rest/1/ads"

        # Return ads based on parameters
        #
        # @param category [#to_s] the category which to request ads 
        # @param user_agent [#to_s] the user_agent being used for request
        # @param udid [#to_s] a unique identifier for the user
        # @param optional [Hash] additional arguments to forward to the api
        #
        # @return [Model::ADSResponse, Model::NoAds] An ads container object 
        def getAds(category, user_agent, udid, optional={})
          url = "#{@fqdn}#{SERVICE_URL}"

          headers = {
            :user_agent => user_agent.to_s,
            :udid => udid.to_s,
          }

          url << "?Category=#{CGI.escape(category.to_s)}"

          if optional
            optional.each do |key, value|
              if value && !value.empty?
                url << "&#{CGI.escape(key.to_s)}=#{CGI.escape(value.to_s)}"
              end
            end
          end

          begin
            response = self.get(url, headers)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end

          if response.code == 204
            Model::NoAds.new
          else
            Model::ADSResponse.createFromJson(response)
          end
        end
        alias_method :get_ads, :getAds

      end
    end
  end
end
