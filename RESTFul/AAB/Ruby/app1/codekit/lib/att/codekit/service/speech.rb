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

require 'json'
require_relative '../model/speech'

module Att
  module Codekit
    module Service

      #@author kh455g
      class SpeechService < CloudService
        STANDARD_SERVICE_URL = "/speech/v3/speechToText"
        CUSTOM_SERVICE_URL = "/speech/v3/speechToTextCustom"

        # Convert a speech audio file to text, convience method based on arg length
        #
        # @note Scopes differ between standard and custom calls. 
        # @see #stdSpeechToText
        # @see #customSpeechToText
        #
        # @return [Model::SpeechResponse] the results of processing the speech 
        #   file
        def speechToText(*args)
          if args.length > 2 
            customSpeechToText(*args)
          else
            stdSpeechToText(*args)
          end
        end
        alias_method :toText, :speechToText

        # Send in an audio file to convert into text
        #
        # @param audio_file [String] path of file to convert
        # @param opts [Hash] Options hashmap for extra params
        # @option opts [String] :context meta info on context (default: Generic)
        # @option opts [String] :xargs custom extra parameters to send for decoding
        # @option opts [Boolean] :chunked set transfter encoding to chunked
        #
        # @return (see speechToText)
        def stdSpeechToText(audio_file, opts={})
          # set to empty string if nil
          xArgs = (opts[:xargs] || opts[:xarg] || "") 
          chunked = opts[:chunked]
          context = (opts[:context] || "Generic")
          subcontext = opts[:subcontext]

          x_arg_val = URI.escape(xArgs)

          filecontents = File.open(audio_file, 'rb') {|io| io.read}

          filetype = CloudService.getMimeType audio_file

          headers = {
            :X_arg => "#{x_arg_val}",
            :X_SpeechContext => "#{context}",
            :Content_Type => "#{filetype}"
          }

          headers[:X_SpeechSubContext] = subcontext if (subcontext && context == "Gaming")

          headers[:Content_Transfer_Encoding] = 'chunked' if chunked 

          url = "#{@fqdn}#{STANDARD_SERVICE_URL}"

          begin
            response = self.post(url, filecontents, headers)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::SpeechResponse.createFromJson(response)
        end

        # Send in an audio file to convert into text
        #
        # @param audio_file [String] path to audio file to submit
        # @param dictionary [String] path to dictionary file
        # @param grammar [String] path to grammar file
        # @param opts [Hash] optional parameter hash
        # @option opts [String] :context The speech context 
        #   (default: GenericHints)
        # @option opts [String] :grammar The type of grammar of the grammar file
        #   (default: x-grammar)
        # @option opts [String] :xargs Custom parameters to send along with the 
        #   request (default: "")
        #
        # @return (see speechToText)
        def customSpeechToText(audio_file, dictionary, grammar, opts={})
          context = (opts[:context] || "GenericHints")
          grammar_type = (opts[:grammar] || "x-grammar")
          xArgs = (opts[:xargs] || "")
          x_arg_val = URI.escape(xArgs)

          dictionary_name = File.basename(dictionary)
          grammar_name = File.basename(grammar)
          filename = File.basename(audio_file)

          filecontents = File.open(audio_file, "rb") {|io| io.read}

          dheaders = {
            "Content-Disposition" => %(form-data; name="x-dictionary"; filename="#{dictionary_name}"),
            "Content-Type" => "application/pls+xml"
          }
          dict_part = {
            :headers => dheaders,
            :data => File.read(dictionary)
          }

          gheaders = {
            "Content-Disposition" => %(form-data; name="#{grammar_type}"; filename="#{grammar_name}"),
            "Content-Type" => "application/srgs+xml"
          }
          grammar_part = {
            :headers => gheaders,
            :data => File.read(grammar)
          }

          mime = CloudService.getMimeType audio_file
          fheaders = {
            "Content-Disposition" =>  %(form-data; name="x-voice"; filename="#{filename}"),
            "Content-Type" => %(#{mime}; charset="binary")
          }
          file_part = {
            :headers => fheaders,
            :data => filecontents
          }

          multipart = [dict_part, grammar_part, file_part]

          url = "#{@fqdn}#{CUSTOM_SERVICE_URL}"

          boundary = rand(1_000_000).to_s
          payload = CloudService.generateMultiPart(boundary, multipart)

          headers = {
            :X_arg => "#{x_arg_val}", 
            :X_SpeechContext => "#{context}", 
            :Content_Type => %(multipart/x-srgs-audio; boundary="#{boundary}"),
          }

          begin
            response = self.post(url, payload, headers)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::SpeechResponse.createFromJson(response)
        end

      end
    end
  end
end
