# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'cgi'

module Att
  module Codekit
    module Auth

      class AuthCode < OAuthService
        attr_reader :grant_type, :redirect_uri

        # Constructor for OAuthService
        #
        # @param fqdn [String] the url to make the request to
        # @param client_id [String] the client id assigned for application
        # @param client_secret [String] the client secret assigned for the application
        # @param scope [String, Array<String>] the scope(s) in which to request a token
        # @param redirect_uri [String] location to redirect to after consent flow
        def initialize(fqdn, client_id, client_secret, scope, redirect_uri, suburl=nil) 
          super(fqdn, client_id, client_secret, scope, suburl)
          @redirect_uri = redirect_uri
          @grant_type = :authorization_code 
        end

        # Authenticate a token with authentication code
        #
        # @param [String] code The code to use for authentication
        def authenticate_token(code)
          parameters = {
            :grant_type => @grant_type,
            :code => code.to_s
          }

          @oauth_token = getNewToken(parameters)
        end

        # Obtain the oauth token associated with current service
        def oauth_token
          fail(OAuthException, "Token has not been authenticated") unless self.authenticated?

          @oauth_token = self.refreshToken if @oauth_token.expired?

          @oauth_token
        end

        # Generate a url to redirect to in order to perform authentication
        # and obtain a code to receive a token, if required.
        #
        # @param suburl [String] the location relative to the fqdn
        # @return [String] a url based on the OAuth object to perform authentication
        def generateConsentFlowUrl(suburl="/oauth/authorize")
          "#{@fqdn}#{suburl}?client_id=#{CGI.escape(@client_id)}&scope=#{CGI.escape(@scope)}&redirect_uri=#{CGI.escape(@redirect_uri)}"
        end

      end # AuthCode

    end
  end
end
