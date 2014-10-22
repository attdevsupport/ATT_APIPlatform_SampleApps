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

module Att
  module Codekit
    module Auth

      # A container for tokens returned by the AT&T oauth api
      # @author kh455g
      class OAuthToken
        include Enumerable

        # Set default refresh token expiry to 24 hours (in seconds)
        DEFAULT_REFRESH_EXPIRES_IN = 60*60*24

        attr_reader :access_token, :refresh_token, :expiry, :expires_in, :refresh_expiry
        # @!attribute [r] access_token
        #   @return [String] token used for authentication
        # @!attribute [r] refresh_token
        #   @return [String] token used to refresh access_token if it expires
        # @!attribute [r] expiry
        #   @return [String] the date (seconds from epoch) the token expires
        # @!attribute [r] expires_in
        #   @return [String] the original seconds from request the token
        #     expires
        # @!attribute [r] refresh_expiry
        #   @return [String] the date (seconds from epoch) the refresh token
        #     expires
        alias_method :access, :access_token
        alias_method :refresh, :refresh_token

        # Construct an OAuthToken object
        #
        # @param access_token [String] token used for authentication
        # @param expiry [#to_i] a representation of time in seconds since 
        #   epoch of when the token should expire. set nil if never expires
        # @param refresh_token [String] token used for re-obtaining an 
        #   access_token after expiry.
        # @param expires_in [#to_i]  the original api response 'unparsed' 
        #   expires_in (default: nil)
        def initialize(access_token, expiry, refresh_token, expires_in=nil, refresh_expiry=nil)
          @access_token = access_token
          @refresh_token = refresh_token
          @expiry = expiry.to_i if expiry
          @expires_in = expires_in
          @refresh_expiry = refresh_expiry.to_i
        end

        # Returns if this token can expire
        #
        # @return [Boolean] true if the token is allowed to expire
        def can_expire?
          return !!@expiry
        end

        # Check if access token is expired 
        #
        # @return [Boolean] true if access token is expired, false if it's 
        #   valid.
        def expired?
          can_expire? && @expiry < Time.now.to_i
        end

        # Returns if this refresh token can expire
        #
        # @return [Boolean] true if the token is allowed to expire
        def can_refresh_expire?
          return !!@refresh_expiry
        end

        # Check if refresh token is expired 
        #
        # @return [Boolean] true if refresh token is expired, false if it's 
        #   valid.
        def refresh_expired?
          can_refresh_expire? && @refresh_expiry < Time.now.to_i
        end

        # 'Each' definition for OAuthToken object
        #
        # @yieldparam access_token [String] The access token used for 
        #   authentication
        # @yieldparam expiry [Time] the time that the access token expires
        # @yieldparam refresh_token [String] the token used to obtain an 
        #   access token after expiry
        # @yieldparam expires_in [Integer] the original api response 'unparsed' 
        #   expires_in if not nil (for backwards compatibility)
        def each
          yield @access_token
          yield @expiry
          yield @refresh_token
          yield @expires_in unless @expires_in.nil?
          yield @refresh_expiry unless @refresh_expiry.nil?
        end

        def eql?(other)
          self.class.equal?(other.class) &&
            @access_token == other.access_token &&
            @expiry == other.expiry &&
            @refresh_token == other.refresh_token
            @refresh_expiry == other.refresh_expiry
        end
        alias == eql?

        # Serialize token into a json object
        def to_json(*a)
          {
            "json_class" => self.class.name,
            "data" => [@access_token, @expiry, @refresh_token, @expires_in, @refresh_expiry]
          }.to_json(*a)
        end

        class << self # begin static methods

          # Factory method to create an object from a json string
          #
          # @param json [String] a json encoded string
          #
          # @return [OAuthToken] a parsed object
          def createFromJson(json)
            self.createFromParsedJson(JSON.parse(json))
          end

          # Factory method to create an object from a json string
          #
          # @param json [Object] a decoded json string
          #
          # @return [OAuthToken] a parsed object
          def createFromParsedJson(json)
            expires_in = json['expires_in']
            access_token = json['access_token']
            refresh_token = json['refresh_token']

            now = Time.now.to_i

            if expires_in
              #if expires_in is 0 then set expiration to 100 years
              if expires_in.to_i == 0
                expiry = now + (60*60*24*365*100) - 30 
              else
                expiry = now + expires_in.to_i - 30 
              end
            end

            refresh_expiry = now + DEFAULT_REFRESH_EXPIRES_IN

            new(access_token, expiry, refresh_token, expires_in, refresh_expiry)
          end

          # Deserialize a token object
          #
          # @return [OAuthToken] parsed oauth token object
          def json_create(o)
            new(*o["data"])
          end


          # Open file and save specified token
          #
          # @note Overwrites file if already exists.
          #
          # @param path [String] path of file to save token
          # @param token [OAuthToken] the token to save to file.
          def save_token(path, token)
            File.open(path, 'w') do |f|
              begin
                f.flock(File::LOCK_EX)
                f.puts token.to_json
              ensure
                f.flock(File::LOCK_UN)
              end
            end
          end
          alias_method :save, :save_token

          # Open file and return new OAuthToken
          # 
          # @param path [String] path of file to load token from
          #
          # @return [OAuthToken] returns an OAuthToken object
          def open_token(path)
            File.open(path,'r') do |f|
              begin
                f.flock(File::LOCK_EX)
                JSON.load(f.read)
              ensure
                f.flock(File::LOCK_UN)
              end
            end
          end
          alias_method :open, :open_token
          alias_method :load, :open_token
          alias_method :load_token, :open_token

        end # end static methods

      end # END OF CLASS
    end
  end
end
#  vim: set ts=8 sw=2 sts=2 tw=79 et :
