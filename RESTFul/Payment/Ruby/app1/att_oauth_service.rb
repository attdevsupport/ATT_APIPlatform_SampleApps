# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

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
    def initialize(fqdn, client_id, client_secret, scope, redirect_uri="/",opts={})
      @fqdn = fqdn
      @client_id = client_id
      @client_secret = client_secret
      @scope = scope
      @redirect_uri = redirect_uri

      @grant_type = (opts[:grant_type] || GrantType::CLIENT_CREDENTIALS)
      @endpoint = (opts[:endpoint] || "/oauth/token")

      if opts[:token_file]
        @token_file = opts[:token_file]
        if File.file? @token_file
          @oauth_token = OAuthToken.load(@token_file)
        else
          @oauth_token = createToken
          OAuthToken.save(@token_file, @oauth_token)
        end
      else
        @oauth_token = createToken
      end
    end

    # Create and return an OAuthToken
    #
    # @param [Hash] opts the options used to create a token.
    # @option opts [String] :code The code returned from authentication flow
    # @option opts [OAuthToken] :old_token Pass in a token when you want to do a refresh
    # @raise [Exception] when a problem occurs while making the request
    # @return [OAuthToken] a token which contains the access, expiration and refresh tokens
    def createToken(opts = {})
      if opts then
        code = opts[:code] if opts[:code]
        old_token = opts[:old_token] if opts[:old_token]
      end

      if code then
        response = RestClient.post "#{@fqdn}#{@endpoint}",
          :grant_type => @grant_type,
          :client_id => @client_id,
          :client_secret => @client_secret,
          :code => code
      elsif old_token then
        response = RestClient.post "#{@fqdn}#{@endpoint}",
          :grant_type => 'refresh_token',
          :client_id => @client_id,
          :client_secret => @client_secret,
          :refresh_token => old_token.refresh_token
      else
        response = RestClient.post "#{@fqdn}#{@endpoint}",
          :grant_type => @grant_type,
          :client_id => @client_id,
          :client_secret => @client_secret,
          :scope => @scope
      end

      json = JSON.parse(response)
      return OAuthToken.new(json['access_token'], json['expires_in'], json['refresh_token'])
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
