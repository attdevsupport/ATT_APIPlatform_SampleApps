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

        attr_reader :access_token, :refresh_token, :expiry
        # @!attribute [r] access_token
        #   @return [String] token used for authentication
        # @!attribute [r] refresh_token
        #   @return [String] token used to refresh access_token if it expires
        alias_method :access, :access_token
        alias_method :refresh, :refresh_token

        # Construct an OAuthToken object
        #
        # @param access_token [String] token used for authentication
        # @param expiry [#to_i] a representation of time in seconds since 
        #   epoch of when the token should expire. set nil if never expires
        # @param refresh_token [String] token used for re-obtaining an 
        #   access_token after expiry.
        def initialize(access_token, expiry, refresh_token)
          @access_token = access_token
          @refresh_token = refresh_token
          @expiry = expiry.to_i if expiry
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

        # 'Each' definition for OAuthToken object
        #
        # @yieldparam access_token [String] The access token used for 
        #   authentication
        # @yieldparam expiry [Time] the time that the access token expires
        # @yieldparam refresh_token [String] the token used to obtain an 
        #   access token after expiry
        def each
          yield @access_token
          yield @expiry
          yield @refresh_token
        end

        def eql?(other)
          self.class.equal?(other.class) &&
            @access_token == other.access_token &&
            @expiry == other.expiry &&
            @refresh_token == other.refresh_token
        end
        alias == eql?

        # Serialize token into a json object
        def to_json(*a)
          {
            "json_class" => self.class.name,
            "data" => [@access_token, @expiry, @refresh_token]
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

            if expires_in
              #if expires_in is 0 then set expiration to 100 years
              if expires_in.to_i == 0
                expires_in = Time.now.to_i + (60*60*24*365*100) - 30 
              else
                expires_in = Time.now.to_i + expires_in.to_i - 30 
              end
            end

            new(access_token, expires_in, refresh_token)
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
