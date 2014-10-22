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
require 'immutable_struct'

module Att
  module Codekit
    module Model

      class IMMNResponse < ImmutableStruct.new(:id)
        # IMMN request response
        #
        # @!attribute [r] id
        #   @return [String] the id of the sent message

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [IMMNResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a json encoded string
        #
        # @return [IMMNResponse] a parsed object
        def self.createFromParsedJson(json)
          new(json["id"])
        end
      end

    end
  end
end
