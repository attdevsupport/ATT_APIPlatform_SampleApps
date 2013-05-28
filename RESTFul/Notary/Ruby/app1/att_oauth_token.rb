# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

#@author Kyle Hill <kh455g@att.com>
module AttCloudServices

  # A container for tokens returned by the AT&T oauth api
  class OAuthToken
    include Enumerable

    # Construct an OAuthToken object
    #
    # @param access_token [String] token used for authentication
    # @param expiry [String, Long, Time] a representation of time in ms since epoch. responds_to :to_i
    # @param refresh_token [String] token used for re-obtaining an access_token after expiry.
    def initialize(access_token, expiry, refresh_token)
      @access_token = access_token
      @refresh_token = refresh_token
      expiry = expiry.to_i 

      if expiry == HUNDRED_YEARS
        if RUBY_VERSION < "1.9"
          #treat 100 years as never expires since ruby 1.8 can only goto the year 2038
          @expiry = NEVER_EXPIRES
        else 
          @expiry = Time.now + (60*60*24*365*100)
        end
      elsif expiry == NEVER_EXPIRES
        @expiry = NEVER_EXPIRES
      else
        @expiry = Time.at(expiry.to_i)
      end
    end

    attr_reader :access_token, :refresh_token, :expiry

    # # # # # # #
    # CONSTANTS #
    # # # # # # #
    HUNDRED_YEARS = 0
    NEVER_EXPIRES = -1

    # Check if access token is expired or not
    #
    # @return [true,false] true if access token is expired, false if it's valid.
    def expired?
      return false if @expiry.to_i == NEVER_EXPIRES 
      @expiry < Time.now
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
            f.puts token.expiry.to_i
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
