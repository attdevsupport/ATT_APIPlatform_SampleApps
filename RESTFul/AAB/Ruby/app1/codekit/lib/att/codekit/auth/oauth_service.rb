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
require_relative '../transport'
require_relative 'oauth_token'

module Att
  module Codekit
    module Auth

      class OAuthException < Exception; end

      #@author kh455g
      class OAuthService 

        attr_reader :fqdn, :scope, :suburl
        # @!attribute [r] fqdn
        #   @return [String] the fqdn being used for making requests
        # @!attribute [r] scope
        #   @return [String] the scope to use for authentication (default: nil)
        # @!attribute [r] suburl
        #   @return [String] the suburl used with fqdn for authentication 

        # Constructor for OAuthService
        # @note do not use this directly, use a provided wrapper AuthCode or ClientCred
        #
        # @param fqdn [String] the url to make the request to
        # @param client_id [String] the client id assigned for application
        # @param client_secret [String] the client secret assigned for the application
        # @param opts [Hash] optional parameters 
        # @option opts [String, Array<String>] :scope the scope(s) in which to request a token
        # @option opts [String] :suburl the suburl to the fqdn for requesting tokens, do not change unless you absolutely know what you are doing 
        def initialize(fqdn, client_id, client_secret, opts={}) 
          @fqdn = fqdn
          @client_id = client_id
          @client_secret = client_secret

          @scope = opts[:scope]
          @suburl = (opts[:suburl] || '/oauth/token')
        end

        # Creates a new oauth token
        #
        # @param grant [String] grant type used to create the token
        # @param params [Hash] extra parameters needed for creating a token
        #
        # @return [OAuthToken] a new oauth token
        def makeToken(grant, params)
          parameters = {
            :grant_type => grant
          }.merge(params)

          getNewToken(parameters)
        end
        private :makeToken

        # Refreshes the current oauth token
        #
        # @param token [OAuthToken] the old token to refresh
        #
        # @return [OAuthToken] a refreshed oauth token
        def refreshToken(token)
          makeToken(:refresh_token, :refresh_token => token.refresh_token)
        end

        # Create and return an OAuthToken
        #
        # @param parameters [Hash] the params used to create a token.
        #
        # @raise [OAuthException] when a problem occurs while making the request
        # @return [OAuthToken] an oauth token which contains the access, expiration and refresh tokens
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
          rescue RestClient::Exception => e
            raise(OAuthException, "Problem obtaining a token: #{e.response || e.message}", e.backtrace)
          end

          begin
            OAuthToken.createFromJson(response)
          rescue Exception => e
            raise(OAuthException, "Error parsing the token response data: #{e}", e.backtrace)
          end
        end
        protected :getNewToken

      end
    end
  end
end
