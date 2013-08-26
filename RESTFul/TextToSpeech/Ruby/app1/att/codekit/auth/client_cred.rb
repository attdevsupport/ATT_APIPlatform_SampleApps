# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Auth

      # Wrapper class that should be used if additional functionality is added
      class ClientCred < OAuthService
        attr_reader :token_file, :grant_type

        # Constructor for OAuthService
        #
        # @param fqdn [String] the url to make the request to
        # @param client_id [String] the client id assigned for application
        # @param client_secret [String] the client secret assigned for the application
        # @param scope [String, Array<String>] the scope(s) in which to request a token
        # @param opts [Hash] Optional fields hash
        # @option opts [String] :token_file a path to save token files to
        # @option opts [String] :tokens_file a path to save token files to
        # @option opts [String] :suburl the suburl to the fqdn for requesting tokens, do not change unless you absolutely know what you're doing 
        def initialize(fqdn, client_id, client_secret, scope, opts={})
          super(fqdn, client_id, client_secret, scope, opts[:suburl])
          @grant_type = :client_credentials
          @token_file = (opts[:token_file] || opts[:tokens_file])
        end

        # Creates a new oauth token
        def createToken
          parameters = {
            :scope => @scope,
            :grant_type => @grant_type
          }
          getNewToken(parameters)
        end

        # Obtain the oauth token associated with current service
        # @note this creates a new token when needed
        def oauth_token
          filesupport = @token_file && File.file?(@token_file)

          if @oauth_token.nil? && filesupport 
            @oauth_token = OAuthToken.load(@token_file) 
          end

          if @oauth_token.nil? 
            @oauth_token = self.createToken 
            if filesupport
              OAuthToken.save(@token_file, @oauth_token) 
            end
          end

          if @oauth_token.expired? 
            @oauth_token = self.refreshToken 
            if filesupport
              OAuthToken.save(@token_file, @oauth_token) 
            end
          end

          @oauth_token
        end

      end

    end
  end
end
