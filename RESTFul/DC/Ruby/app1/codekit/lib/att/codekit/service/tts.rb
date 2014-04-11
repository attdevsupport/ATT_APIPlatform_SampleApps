# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require_relative '../model/speech'

module Att
  module Codekit
    module Service

      #@author kh455g
      class TTSService < CloudService
        SERVICE_URL = "/speech/v3/textToSpeech"

        # Make a call to convert text into audio
        #
        # @param content [#to_s] the String to be converted to audio
        # @param opts [Hash] options hash
        # @option opts [#to_s] :xargs custom arguments to alter the conversion (default: nil)
        # @option opts [#to_s] :type the content type of the content (default: text/plain)
        # @option opts [#to_s] :lang the language iso of the content 
        #   (default: en-US)
        # @option opts [#to_s] :accept format to request for the service (default: audio/x-wav)
        #
        # @return [Model::TTSResponse] container holding the tts response
        def textToSpeech(content, opts={})
          accept = (opts[:accept] || "audio/x-wav")
          type = (opts[:type] || "text/plain")
          lang = (opts[:lang] || opts[:content_language])
          xArgs = (opts[:xargs] || opts[:xarg] || "")
          x_arg_val = URI.escape(xArgs.to_s)

          url = "#{@fqdn}#{SERVICE_URL}"

          headers = {
            :Accept => accept.to_s,
            :X_arg => x_arg_val.to_s, 
            :Content_Type => type.to_s,
            :Content_Language => lang.to_s,
          }

          begin
            response = self.post(url, content.to_s, headers)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::TTSResponse.createFromResponse(response)
        end
        alias_method :toSpeech, :textToSpeech

      end
    end
  end
end
