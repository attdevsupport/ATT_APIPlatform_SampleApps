require 'att_oauth_service'
require 'rest_client'

module AttCloudServices
  class ServiceException < StandardError; end

  # Helper class which contains common code for use with the api
  #
  # see #OAuthService
  class AttApiService
    def initialize(*args)
      if args.first.instance_of? OAuthService
        @oauth = args.first
      else
        @oauth = OAuthService.new(*args)
      end
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
    def generateMultiPart(boundary, data)
      body = ""
      data = [data] unless data.instance_of? Array
      data.each do |part|
        body += "--#{boundary}\r\n"
        part[:headers].each do |key, value|
          body += key.to_s + ": " + value.to_s + "\r\n"
        end
        body += "\r\n" + part[:data].to_s + "\r\n\r\n"
      end
      body += "#{boundary}--\r\n"
    end

    # Send a post request with standard headers
    #
    # @param url [String] The url to send the request to
    # @param payload [String] The body of the post request
    # @param extra_headers [Hash] A hash of headers that override/extend the 
    # defaults
    def postRequest(url, payload, extra_headers={})
      headers = {
        :Content_Type => "application/json",
        :Accept => "application/json",
        :Authorization => "Bearer #{@oauth.access_token}"
      }

      headers.merge!(extra_headers)

      RestClient.post url, payload, headers
    end

    # Send a get request with standard headers
    #
    # @param url [String] The url to send the request to
    # @param extra_headers [Hash] A hash of headers that override/extend the 
    # defaults
    def getRequest(url, extra_headers={})
      headers = {
        :Authorization => "Bearer #{@oauth.access_token}", 
        :Accept => "application/json"
      }

      headers.merge!(extra_headers)

      RestClient.get url, headers
    end

  end
end
