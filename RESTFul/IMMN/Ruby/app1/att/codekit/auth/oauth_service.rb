# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'json'
require 'att/codekit/transport'
require 'att/codekit/auth/oauth_token'

module Att
  module Codekit
    module Auth

      class OAuthException < Exception; end

      #@author Kyle Hill <kh455g@att.com>
      class OAuthService 

        attr_reader :client_id, :client_secret, :fqdn, :scope, :suburl

        # Constructor for OAuthService
        # @note do not use this directly, use a provided wrapper AuthCode or ClientCred
        #
        # @param fqdn [String] the url to make the request to
        # @param client_id [String] the client id assigned for application
        # @param client_secret [String] the client secret assigned for the application
        # @param scope [String, Array<String>] the scope(s) in which to request a token
        # @param suburl [String] the suburl to the fqdn for requesting tokens, do not change unless you absolutely know what you are doing 
        def initialize(fqdn, client_id, client_secret, scope, suburl=nil)
          @fqdn = fqdn
          @client_id = client_id
          @client_secret = client_secret
          @scope = Array(scope).join(",")
          @suburl = (suburl || '/oauth/token')
        end

        def access_token
          self.oauth_token.access_token
        end

        # Obtain the oauth token associated with current service
        def oauth_token
          @oauth_token
        end

        # Check if our oauth token has been created
        #
        # @return [Boolean] True if oauth token exists false otherwise
        def authenticated?
          return !!@oauth_token
        end


        # Refreshes the current oauth token
        def refreshToken
          parameters = {
            :grant_type => :refresh_token,
            :refresh_token => @oauth_token.refresh_token
          }

          getNewToken(parameters)
        end

        # Create and return an OAuthToken
        #
        # @param [Hash] parameters the params used to create a token.
        # @raise [OAuthException] when a problem occurs while making the request
        # @return [OAuthToken] a token which contains the access, expiration and refresh tokens
        def getNewToken(parameters = {})
          parameters.merge!({
            :client_id => @client_id,
            :client_secret => @client_secret
          })
          headers = {
            :content_type => "application/x-www-form-urlencoded",
            :accept => "application/json"
          }
          begin
            response = Transport.post("#{@fqdn}#{@suburl}", parameters, headers)
          rescue => e
            raise(OAuthException, "Problem obtaining a token: #{e}", e.backtrace)
          end
          curr_time_seconds = Time.now.to_i 

          begin
            json = JSON.parse(response)

            expiry = json['expires_in']
            access_token = json['access_token']
            refresh_token = json['refresh_token']
          rescue => e
            raise(OAuthException, "Error parsing the token response data: #{e}", e.backtrace)
          end

          #create the relative expire time and remove 30 seconds for better results
          expiry = curr_time_seconds + expiry.to_i - 30 unless expiry.nil? or expiry.to_i <= 0 
          expiry = Time.now.to_i + (60*60*24*365*100) - 30 if expiry.to_i == 0

          OAuthToken.new(access_token, expiry, refresh_token)
        end
        protected :getNewToken

      end
    end
  end
end
