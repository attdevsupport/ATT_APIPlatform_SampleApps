# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Service

      #@author Kyle Hill <kh455g@att.com>
      class TTSService < CloudService
        SERVICE_URL = "/speech/v3/textToSpeech"

        # Make a call to convert text into audio
        #
        # @param content [#to_s] the String to be converted to audio
        # @param opts [Hash] options hash
        # @option opts [#to_s] :xargs custom arguments to alter the conversion (default: nil)
        # @option opts [#to_s] :type the content type of the content (default: text/plain)
        # @option opts [#to_s] :accept format to request for the service (default: audio/x-wav)
        def textToSpeech(content, opts={})
          accept = (opts[:accept] || "audio/x-wav")
          type = (opts[:type] || "text/plain")
          xArgs = (opts[:xargs] || opts[:xarg] || "")
          x_arg_val = URI.escape(xArgs.to_s)

          url = "#{@fqdn}#{SERVICE_URL}"

          headers = {
            :Accept => accept.to_s,
            :X_arg => x_arg_val.to_s, 
            :Content_Type => type.to_s,
          }

          self.post(url, content.to_s, headers)
        end
        alias_method :toSpeech, :textToSpeech

      end
    end
  end
end
