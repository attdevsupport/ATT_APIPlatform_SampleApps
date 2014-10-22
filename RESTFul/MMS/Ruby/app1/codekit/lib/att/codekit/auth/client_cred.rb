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
