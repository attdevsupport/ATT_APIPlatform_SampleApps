# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'rest_client'
require 'att_oauth_token'

#@author Kyle Hill <kh455g@att.com>
module AttCloudServices

  # Enum for the possible grant types available
  module GrantType
    CLIENT_CREDENTIALS = :client_credentials
    AUTHORIZATION = :authorization_code
    REFRESH = :refresh_token
  end

  class OAuthService 

    attr_reader :fqdn, :client_id, :client_secret, :scope, :redirect_uri, :grant_type, :endpoint

    # Constructor for OAuthService
    #
    # @param fqdn [String] the url to make the request to
    # @param client_id [String] the client id assigned for application
    # @param client_secret [String] the client secret assigned for the application
    # @param scope [String] the scope to request a token for
    # @param redirect_uri [String] the redirect uri to return to if doing authorization flow
    # @param grant_type [Symbol] the type of request being made (see #GrantType)
    # @param endpoint [String] the endpoint to the fqdn, should never change
    def initialize(fqdn, client_id, client_secret, scope, opts={})
      @fqdn = fqdn
      @client_id = client_id
      @client_secret = client_secret
      @scope = scope

      @redirect_uri = (opts[:redirect_uri] || "/")
      @grant_type = (opts[:grant_type] || GrantType::CLIENT_CREDENTIALS)
      @endpoint = (opts[:endpoint] || "/oauth/token")
      @token_file = opts[:tokens_file]
      @headers = (opts[:headers] || {})
    end

    def access_token(headers={})
      self.oauth_token(headers).access_token
    end

    def nil_oauth_token?
      return @oauth_token.nil?
    end

    def oauth_token(headers={})
      headers ||= {}
      headers = @headers.merge(headers)
      if @oauth_token.nil? then
        if @token_file
          if File.file? @token_file
            @oauth_token = OAuthToken.load(@token_file)
          else
            @oauth_token = createToken headers 
            OAuthToken.save(@token_file, @oauth_token) 
          end
        else
          @oauth_token = createToken headers
        end
      end
      @oauth_token = generateToken(:old_token => @oauth_token) if @oauth_token.expired?
      @oauth_token
    end

    # Create and return an OAuthToken
    #
    # @param [Hash] opts the options used to create a token.
    # @option opts [String] :code The code returned from authentication flow
    # @option opts [OAuthToken] :old_token Pass in a token when you want to do a refresh
    # @raise [Exception] when a problem occurs while making the request
    # @return [OAuthToken] a token which contains the access, expiration and refresh tokens
    def createToken(opts = {})
      headers = opts
      if headers[:old_token]
        headers[:refresh_token] = headers[:old_token].refresh_token
        headers[:grant_type] = "refresh_token"
        headers.delete(:old_token)
      end

      default_headers = {
        :grant_type => @grant_type,
        :client_id => @client_id,
        :client_secret => @client_secret,
        :scope => @scope
      }

      headers = default_headers.merge(headers)

      response = RestClient.post "#{@fqdn}#{@endpoint}", headers
      json = JSON.parse(response)
      expiry = Time.now.to_i + json['expires_in'].to_i
      return OAuthToken.new(json['access_token'], expiry, json['refresh_token'])
    end
    alias_method :generateToken, :createToken

    # Generate a url to redirect to in order to perform authentication
    # and obtain a code to receive a token, if required.
    #
    # @param endpoint [String] the location relative to the fqdn
    # @return [String] a url based on the OAuth object to perform authentication
    def generateConsentFlowUrl(endpoint="/oauth/authorize")
      "#{@fqdn}#{endpoint}?client_id=#{@client_id}&scope=#{@scope}&redirect_uri=#{@redirect_uri}"
    end
  end
end
