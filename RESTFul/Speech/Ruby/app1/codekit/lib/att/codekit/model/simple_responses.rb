# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'immutable_struct'


module Att
  module Codekit
    module Model

      class SuccessCreated < ImmutableStruct.new(:location)
        def self.from_response(response)
          headers = response.headers unless response.nil?

          new(headers[:location]) if headers
        end
      end

      class SuccessNoContent < ImmutableStruct.new(:last_modified)
        def self.from_response(response)
          headers = response.headers unless response.nil?

          new(headers[:last_modified]) if headers
        end
      end

    end
  end
end
