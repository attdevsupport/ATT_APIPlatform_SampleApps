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

require 'restclient'

module Att
  module Codekit

    # Static class for defining how to perform the transport layer
    class Transport
    #TODO: implement facade to allow user to specify what rest library to use

      class << self
        # Send a post request with standard headers
        #
        # @param url [String] The url to send the request 
        # @param payload [String] The body of the post request
        # @param headers [Hash] A hash of headers 
        #
        # @return [RestClient::Response] http response object
        def post(url, payload, headers={})
          RestClient.post url, payload, headers
        end

        # Send a get request with standard headers
        #
        # @param url [String] The url to send the request
        # @param headers [Hash] A hash of headers 
        #
        # @return [RestClient::Response] http response object
        def get(url, headers={})
          RestClient.get url, headers
        end

        # Send a Http put request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param payload [String] The data to send to the url
        # @param headers [Hash] A hash of headers
        #
        # @return [RestClient::Response] http response object
        def put(url, payload, headers={})
          RestClient.put url, payload, headers
        end

        # Send a Http patch request with standard headers
        #
        # @param url [String] The url to send the request to
        # @param payload [String] The data to send to the url
        # @param headers [Hash] A hash of headers
        #
        # @return [RestClient::Response] http response object
        def patch(url, payload, headers={})
          RestClient.patch url, payload, headers
        end

        # Send a Http delete request with standard headers
        #
        # @param url [String] The url to send the request 
        # @param headers [Hash] A hash of headers
        #
        # @return [RestClient::Response] http response object
        def delete(url, headers={})
          RestClient.delete url, headers
        end

        # Set a proxy to use when making requests
        #
        # @param prox [String] The proxy to use for routing
        def proxy(prox)
          RestClient.proxy = prox
        end

      end
    end
  end
end
