# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Service

      #@author Kyle Hill <kh455g@att.com>
      class DCService < CloudService
        SERVICE_URL = "/rest/2/Devices/Info"

        # Obtain the device capabilities of device authenticated by code
        #
        # @param code [String] the authentication code 
        # @return [RestClient::Response] 
        def getDeviceCapabilties(code=nil)
          url = "#{@fqdn}#{SERVICE_URL}"

          updateAccessToken(code) unless (code.nil? || authenticated?)

          self.get(url)
        end

      end
    end
  end
end
