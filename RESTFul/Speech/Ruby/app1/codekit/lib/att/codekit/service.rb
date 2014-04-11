# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

#@author kh455g
module Att
  module Codekit
    module Service

      class ServiceException < Exception; end

      # Helper class which contains common code for use with the AT&T Cloud API.
      class CloudService
        include Att::Codekit::Auth

        attr_reader :fqdn, :token

        # Creates a base service 
        #
        # @param token [OAuthToken] the oauth token used to authenticate
        # @param fqdn [String] url that points to where the cloud api exists
        def initialize(fqdn, token, client=nil) 
          @token = token
          @fqdn = fqdn 
          @client = client
        end

        # Send a post request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param payload [String] The body of the post request
        # @param custom_headers [Hash] A hash of headers that override/extend
        #  the defaults
        #
        # @return [RestClient::Response] http response object
        def post(url, payload, custom_headers={})
          headers = {
            :Accept => "application/json",
            :Content_Type => "application/json",
            :Authorization => "Bearer #{@token.access_token}"
          }

          headers.merge!(custom_headers)

          Transport.post url, payload, headers
        end

        # Send a get request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param custom_headers [Hash] A hash of headers that override/extend 
        #   the defaults
        #
        # @return [RestClient::Response] http response object
        def get(url, custom_headers={})
          headers = {
            :Accept => "application/json",
            :Authorization => "Bearer #{@token.access_token}", 
          }

          headers.merge!(custom_headers)

          Transport.get url, headers
        end

        # Send a Http put request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param payload [String] The data to send to the url
        # @param custom_headers [Hash] A hash of headers that override/extend 
        #   the defaults
        #
        # @return [RestClient::Response] http response object
        def put(url, payload, custom_headers={})
          headers = {
            :Accept => "application/json",
            :Content_Type => 'application/json',
            :Authorization => "Bearer #{@token.access_token}", 
          }

          headers.merge!(custom_headers)

          Transport.put url, payload, headers
        end

        # Send a Http delete request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param custom_headers [Hash] A hash of headers that override/extend 
        #   the defaults
        #
        # @return [RestClient::Response] http response object
        def delete(url, custom_headers={})
          headers = {
            :Accept => "application/json",
            :Authorization => "Bearer #{@token.access_token}", 
          }

          headers.merge!(custom_headers)

          Transport.delete url, headers
        end

        # Transform a list of addresses into a format acceptable by the api
        # 
        # @note If the prefix tel: or short: is not present, then a best guess
        #   based on the length is made.
        #
        # @param addresses [String, Array<String>] Comma seperated string or 
        #   array of phone numbers
        #
        # @return [Array<String>] an array of acceptable addresses
        def self.format_addresses(addresses)
          # convert any input to an array of strings
          addresses = Array(addresses).join(",").split(",")

          parsed_addresses = Array.new

          addresses.each do |a|
            a = a.gsub("-","").strip
            if a.include?("@") || a.include?("tel:") || a.include?("short:")
              parsed_addresses << a 
            elsif a.length <= 8
              parsed_addresses << "short:#{a}" 
            else
              parsed_addresses << "tel:#{a}" 
            end
          end
          parsed_addresses
        end

        # Internal function used to generate random boundary
        def self.generateBoundary
          "----#{(rand*10_000_000) + 10_000_000}.#{Time.now.to_i}"
        end

        # Construct a multipart body that's compatible with the AT&T Cloud API
        #
        # @param boundary [String] a random string to split chunks on
        # @param data [Array<Hash>,Hash] An array of hashes or a single hash, each 
        #   hash represents a seperate chunk of data. 
        #
        # @option data [Hash] :headers key value pairs, where key is the header
        #   and value is the headers value
        # @option data [String] :data the actual object (payload) to send
        #
        # @return [String] A payload that can be sent to the AT&T Cloud API
        def self.generateMultiPart(boundary, data)
          body = ""
          data = Array(data)
          data.each do |part|
            body += "--#{boundary}\r\n"
            part[:headers].each do |key, value|
              body += "#{key}: #{value}\r\n"
            end
            body += "\r\n#{part[:data]}\r\n\r\n"
          end
          body += "--#{boundary}--\r\n"
        end

        # Simple file extension checking to obtain filetype
        #
        # @note This method should not be used outside of the services as it
        #   will eventually be removed and updated with libmagic.
        #   The current speech api does not accept all the current values
        #   returned from libmagic so a custom function is present here.
        #
        # @param file [String] the path or name of the file to check type
        #
        # @return [String] the mime type of the file
        def self.getMimeType(file)
          extension = file.split(".").last
          case extension
          when "txt" then "text/plain"
          when "html" then "text/html"
          when "xml" then "application/xml"
          when "ssml" then "application/ssml+xml"
          when "jpg", "jpeg", "jpe" then "image/jpeg"
          when "png" then "image/png"
          when "gif" then "image/gif"
          when "bmp" then "image/bmp"
          when "tif", "tiff" then "image/tiff"
          when "mov", "qt" then "video/quicktime"
          when "mpa", "mpe", "mpeg", "mpg", "mpv2" then "video/mpeg"
          when "mp4" then "video/mp4"
          when "avi" then "video/x-msvideo"
          when "asf", "asr", "asx" then "video/x-ms-asf"
          when "amr" then "audio/amr"
          when "awb" then "audio/amr-wb"
          when "aif", "aifc", "aiff" then "audio/x-aiff"
          when "au", "snd" then "audio/basic"
          when "m3u" then "audio/x-mpegurl"
          when "mid","midi", "rmi" then "audio/midi"
          when "mp3" then "audio/mpeg"
          when "wav" then "audio/x-wav"
          when "ogg", "oga" then "audio/ogg"
          when "spx" then "audio/x-speex"
          when "raw" then "audio/raw"
          when "3gp" then "audio/3gpp"
          else  raise "Unsupported media type extension, file: #{file}"
          end
        end
      end

      require_relative "service/ads" 
      require_relative "service/speech" 
      require_relative "service/tts" 
      require_relative "service/dc" 
      require_relative "service/tl" 
      require_relative "service/mim" 
      require_relative "service/immn" 
      require_relative "service/sms" 
      require_relative "service/mms" 
      require_relative "service/cms" 
      require_relative "service/payment" 

    end
  end
end
