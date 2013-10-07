# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Auth

      class ClientCred < OAuthService

        # Creates a new oauth token
        #
        # @param scope [String, Array<String>] The scope to create a token for, can be omitted if set in constructor
        #
        # @return [OAuthToken] a new oauth token
        def createToken(scope=nil)
          scope = Array(scope || @scope).join(",")
          makeToken(:client_credentials, :scope => scope)
        end

      end

    end
  end
end
