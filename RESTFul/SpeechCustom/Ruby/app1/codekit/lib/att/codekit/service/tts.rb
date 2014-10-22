# Copyright 2014 AT&T
# 
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# 
# http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

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
            :Content_Type => type.to_s,
          }

          headers[:X_arg] = x_arg_val.to_s unless x_arg_val.to_s.empty?
          headers[:Content_Language] = lang.to_s unless lang.to_s.empty?

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
