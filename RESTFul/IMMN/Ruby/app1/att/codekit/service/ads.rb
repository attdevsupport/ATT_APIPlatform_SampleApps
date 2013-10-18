# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'cgi'

module Att
  module Codekit
    module Service

      #@author Kyle Hill <kh455g@att.com>
      class ADService < CloudService
        SERVICE_URL = "/rest/1/ads"

        # Return ads based on parameters
        #
        # @param params [Hash] specifies the arguments sent to the api
        # @param headers [Hash] additional headers to forward to the api
        def get_ads(params, headers={})
          url = "#{@fqdn}#{SERVICE_URL}"

          if params
            args = "?"
            params.each do |key, value|
              if value && !value.empty?
                args << "&" unless args.end_with? "?"
                args << "#{CGI.escape(key.to_s)}=#{CGI.escape(value.to_s)}"
              end
            end
            url << args
          end

          self.get(url, headers)
        end

      end
    end
  end
end
