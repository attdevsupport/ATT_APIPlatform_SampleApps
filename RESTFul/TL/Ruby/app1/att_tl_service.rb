require 'att_api_service'

module AttCloudServices
  module TL
    SCOPE = 'TL'

    module ENDPOINTS
      TL = "/2/devices/location"
    end

    class TlService < AttApiService

      def getDeviceLocation(auth_code, req_accuracy, accept_accuracy, tolerance)
        auth_code ||= ""
        url = "#{@oauth.fqdn}#{ENDPOINTS::TL}"

        url << "?requestedAccuracy=#{req_accuracy}"
        url << "&acceptableAccuracy=#{accept_accuracy}&"
        url << "tolerance=#{tolerance}"

        headers =  {
          :Authorization => "BEARER #{@oauth.access_token(:code => auth_code)}", 
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
