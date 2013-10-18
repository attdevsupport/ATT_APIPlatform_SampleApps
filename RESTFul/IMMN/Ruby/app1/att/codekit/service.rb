# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

#@author Kyle Hill <kh455g@att.com>
module Att
  module Codekit
    module Service
      require 'att/codekit/auth/oauth_service'

      # Helper class which contains common code for use with the AT&T Cloud API.
      class CloudService
        include Att::Codekit::Auth

        attr_reader :fqdn

        # Creates a base service 
        #
        # @param oauth [OAuthService] the oauth object used to authenticate
        # @param fqdn [String] url that points to where the cloud api exists
        # @see OAuthService#initialize
        def initialize(oauth, fqdn=nil) 
          @oauth = oauth
          @fqdn = (fqdn || oauth.fqdn)
        end

        # Updates and returns an OAuthToken based on auth code
        #
        # @param code [String] The auth code used to update the token
        def updateAccessToken(code)
          @oauth.authenticate_token(code)
        end

        # Check if our token has been authenticated for the user.
        def authenticated?
          @oauth.authenticated?
        end

        # Returns the url that can be used for consent flow
        def consentFlow
          @oauth.generateConsentFlowUrl
        end

        # Send a post request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param payload [String] The body of the post request
        # @param custom_headers [Hash] A hash of headers that override/extend the 
        # defaults
        def post(url, payload, custom_headers={})
          headers = {
            :Accept => "application/json",
            :Content_Type => "application/json",
            :Authorization => "Bearer #{@oauth.access_token}"
          }

          headers.merge!(custom_headers)

          Transport.post url, payload, headers
        end

        # Send a get request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param custom_headers [Hash] A hash of headers that override/extend the defaults
        def get(url, custom_headers={})
          headers = {
            :Accept => "application/json",
            :Authorization => "Bearer #{@oauth.access_token}", 
          }

          headers.merge!(custom_headers)

          Transport.get url, headers
        end

        # Send a Http put request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param payload [String] The data to send to the url
        # @param custom_headers [Hash] A hash of headers that override/extend the 
        # defaults
        def put(url, payload, custom_headers={})
          headers = {
            :Accept => "application/json",
            :Content_Type => 'application/json',
            :Authorization => "Bearer #{@oauth.access_token}", 
          }

          headers.merge!(custom_headers)

          Transport.put url, payload, headers
        end

        # Internal function used to generate random boundary
        def self.generateBoundary
          "----=_Part_#{((rand*10000000) + 10000000).to_i}.#{Time.now.to_i}"
        end

        # Construct a multipart body that's compatible with the AT&T Cloud API
        #
        # @param boundary [String] a random string to split chunks on
        # @param data [Array,Hash] An array of hashes or a single hash, each hash
        # represents a seperate chunk of data. Must contain the following two keys.
        # @option data [Hash] :headers key value pairs, where key is the header 
        # and value is the headers value
        # @option data [String] :data the actual object to send
        # @return [String] A payload that can be sent to the AT&T Cloud API
        def self.generateMultiPart(boundary, data)
          body = ""
          data = [data] unless data.instance_of? Array
          data.each do |part|
            body += "--#{boundary}\r\n"
            part[:headers].each do |key, value|
              body += key.to_s + ": " + value.to_s + "\r\n"
            end
            body += "\r\n" + part[:data].to_s + "\r\n\r\n"
          end
          body += "--#{boundary}--\r\n"
        end

        # Simple file extension checking to obtain filetype
        #
        # This method should not be used outside of the services as it
        # will eventually be removed and updated with libmagic.
        # The current speech api does not accept all the current values
        # returned from libmagic so a custom function is present here.
        #
        # @param file [String] the path or name of the file to check type
        def self.getMimeType(file)
          data = file.split(".").last
          case data
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

      require "att/codekit/service/tts" 
      require "att/codekit/service/ads" 
      require "att/codekit/service/speech" 
      require "att/codekit/service/dc" 
      require "att/codekit/service/tl" 
      require "att/codekit/service/mim" 
      require "att/codekit/service/immn" 
      require "att/codekit/service/sms" 
      require "att/codekit/service/mms" 
      require "att/codekit/service/cms" 
      require "att/codekit/service/payment" 

    end
  end
end
