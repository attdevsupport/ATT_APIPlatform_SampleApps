# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Service

      #@author Kyle Hill <kh455g@att.com>
      class SpeechService < CloudService
        STANDARD_SERVICE_URL = "/speech/v3/speechToText"
        CUSTOM_SERVICE_URL = "/speech/v3/speechToTextCustom"

        # Convert a speech audio file to text, convience method based on arg length
        #
        # @note Scopes differ between standard and custom calls. 
        # @see #stdSpeechToText
        # @see #customSpeechToText
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
        # @param file [String] path of file to convert
        # @param opts [Hash] Options hashmap for extra params
        # @option opts [String] :context meta info on context (default: Generic)
        # @option opts [String] :xargs custom extra parameters to send for decoding
        # @option opts [Boolean] :chunked set transfter encoding to chunked
        def stdSpeechToText(file, opts={})
          xArgs = (opts[:xargs] || opts[:xarg] || "") #set to empty string if nil
          chunked = opts[:chunked]
          context = (opts[:context] || "Generic")
          x_arg_val = URI.escape(xArgs)

          filecontents = File.read(file)

          filetype = CloudService.getMimeType file

          headers = {
            :X_arg => "#{x_arg_val}",
            :X_SpeechContext => "#{context}",
            :Content_Type => "#{filetype}"
          }

          headers[:Content_Transfer_Encoding] = 'chunked' if chunked 

          url = "#{@fqdn}#{STANDARD_SERVICE_URL}"

          self.post(url, filecontents, headers)
        end

        # Send in an audio file to convert into text
        #
        # @param audio_file [String] path to audio file to submit
        # @param dictionary [String] path to dictionary file
        # @param grammar [String] path to grammar file
        # @param opts [Hash] optional parameter hash
        # @option opts [String] :context The speech context (default: GenericHints)
        # @option opts [String] :grammar The type of grammar of the grammar file (default: x-grammar)
        # @option opts [String] :xargs Custom parameters to send along with the request (default: "")
        def customSpeechToText(audio_file, dictionary, grammar, opts={})
          context = (opts[:context] || "GenericHints")
          grammar_type = (opts[:grammar] || "x-grammar")
          xArgs = (opts[:xargs] || "")
          x_arg_val = URI.escape(xArgs)

          dictionary_name = File.basename(dictionary)
          grammar_name = File.basename(grammar)
          filename = File.basename(audio_file)

          dheaders = {
            "Content-Disposition" => %(form-data; name="x-dictionary"; filename="#{dictionary}"),
            "Content-Type" => "application/pls+xml"
          }
          dict_part = {
            :headers => dheaders,
            :data => File.read(dictionary)
          }

          gheaders = {
            "Content-Disposition" => %(form-data; name="#{grammar_type.to_s}" filename="#{grammar_name}"),
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
            :data => File.read(audio_file)
          }

          multipart = [dict_part, grammar_part, file_part]

          url = "#{@fqdn}#{CUSTOM_SERVICE_URL}"

          boundary = rand(1_000_000).to_s
          payload = CloudService.generateMultiPart(boundary, multipart)

          headers = {
            :X_arg => "#{x_arg_val}", 
            :X_SpeechContext => context, 
            :Content_Type => %(multipart/x-srgs-audio; boundary="#{boundary}"),
          }

          self.post(url, payload, headers)
        end

      end
    end
  end
end
