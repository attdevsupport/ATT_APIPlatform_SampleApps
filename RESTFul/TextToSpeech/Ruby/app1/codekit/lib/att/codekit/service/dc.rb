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

require_relative '../model/dc'

module Att
  module Codekit
    module Service

      #@author kh455g
      class DCService < CloudService
        SERVICE_URL = "/rest/2/Devices/Info"

        # Obtain the device capabilities of device authenticated by code
        #
        # @param code [String] the authentication code (not required if token is already authenticated)
        #
        # @return [DCResponse] parsed api response 
        def getDeviceCapabilities
          url = "#{@fqdn}#{SERVICE_URL}"

          headers = {
            :Accept => "application/json",
          }

          begin
            response = self.get(url, headers)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::DCResponse.createFromJson(response)
        end
        alias_method :capabilities, :getDeviceCapabilities
      end

    end
  end
end
