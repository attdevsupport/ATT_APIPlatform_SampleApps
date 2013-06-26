require 'att_api_service'

module AttCloudServices
  module TTS

    SCOPE = 'TTS'

    module ENDPOINTS
      TTS = "/speech/v3/textToSpeech"
    end

    class TtsService < AttApiService

      def textToSpeech(xArgs, content, content_type)
        xArgs ||= ""
        x_arg_val = URI.escape(xArgs)

        url = "#{@oauth.fqdn}#{ENDPOINTS::TTS}"

        headers = {
          :Accept => "audio/x-wav",
          :X_arg => "#{x_arg_val}", 
          :Content_Type => "#{content_type}"
        }

        self.postRequest(url, content.to_s, headers)
      end


    end
  end
end
