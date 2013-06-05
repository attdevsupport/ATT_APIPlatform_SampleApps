# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'rest_client'
require 'att_api_service'

#@author Kyle Hill <kh455g@att.com>
module AttCloudServices

  module DC

    SCOPE = "DC"

    module ENDPOINTS 
      DC = "/rest/2/Devices/Info/"
    end

    class DCService < AttApiService

      def getDeviceCapabilties(code, endpoint=ENDPOINTS::DC)
        url = "#{@oauth.fqdn}#{endpoint}"
        headers =  {
          :Authorization => "BEARER #{@oauth.access_token(:code => code)}", 
          :Content_Type => 'application/json', 
          :Accept => 'application/json'
        }

        self.getRequest(url, headers)
      end

      def consentFlow
        @oauth.generateConsentFlowUrl
      end

      def authenticated?
        return !@oauth.nil_oauth_token?
      end

    end
  end
end
