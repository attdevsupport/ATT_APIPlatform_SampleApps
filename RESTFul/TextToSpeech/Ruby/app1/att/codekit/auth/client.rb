require 'immutable_struct'

module Att
  module Codekit
    module Auth

      class Client < ImmutableStruct.new(:id, :secret)
        # @!attribute [r] id
        #   @return [String] the client id
        # @!attribute [r] secret
        #   @return [String] the client secret
      end

    end
  end
end
