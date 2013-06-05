require 'uri'
require 'att_api_service'

module AttCloudServices
  module SpeechCustom
    SCOPE = 'STTC'

    module ENDPOINTS
      CUSTOM = '/speech/v3/speechToTextCustom'
    end

    class SpeechCustomService < AttApiService

      def speechToText(xArgs, multipart, speech_context)
        xArgs ||= "" #set to empty string if nil
        x_arg_val = URI.escape(xArgs)

        url = "#{@oauth.fqdn}#{ENDPOINTS::CUSTOM}"

        boundary = rand(1_000_000).to_s
        payload = self.generateMultiPart(boundary, multipart)

        headers = {
          :X_arg => "#{x_arg_val}", 
          :X_SpeechContext => speech_context, 
          :Content_Type => "multipart/x-srgs-audio; boundary=\"#{boundary}\"",
        }

        self.postRequest(url, payload, headers)
      end

    end
  end
end
