# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Auth

      # A container for tokens returned by the AT&T oauth api
      # @author Kyle Hill <kh455g@att.com>
      class OAuthToken
        include Enumerable

        NEVER_EXPIRES = -1

        # Construct an OAuthToken object
        # @note Due to overflow of the time object in ruby <= 1.8, if the expiry exceeds year 2038 it is set to never expire.
        # @param access_token [String] token used for authentication
        # @param expiry [#to_i] a representation of time in ms since epoch. 
        # @param refresh_token [String] token used for re-obtaining an access_token after expiry.
        def initialize(access_token, expiry, refresh_token)
          @access_token = access_token
          @expiry = (expiry.to_i || NEVER_EXPIRES)
          @refresh_token = refresh_token
        end

        attr_reader :access_token, :refresh_token, :expiry

        # Check if access token is expired 
        #
        # @return [Boolean] true if access token is expired, false if it's valid.
        def expired?
          return false if @expiry == NEVER_EXPIRES 
          @expiry < Time.now.to_i
        end

        # 'Each' definition for OAuthToken object
        #
        # @yieldparam access_token [String] The access token used for authentication
        # @yieldparam expiry [Time] the time that the access token expires
        # @yieldparam refresh_token [String] the token used to obtain an access token after expiry
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

        class << self # begin static methods

          # Open file and save specified token
          # Overwrites file.
          #
          # @param path [String] path of file to save token
          # @param token [OAuthToken] the token to save to file.
          def save_token(path, token)
            File.open(path, 'w') do |f|
              begin
                f.flock(File::LOCK_EX)
                f.puts token.access_token
                f.puts token.expiry
                f.puts token.refresh_token
              ensure
                f.flock(File::LOCK_UN)
              end
            end
          end
          alias_method :save, :save_token

          # Open file and return new OAuthToken
          # 
          # @param path [String] path of file to load token from
          # @return [OAuthToken] returns an OAuthToken object
          def open_token(path)
            File.open(path,'r') do |f|
              begin
                f.flock(File::LOCK_EX)
                return self.new(*f.map.collect(&:chomp))
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
