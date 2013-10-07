
module Att
  module Codekit
    module Model

      #Contains the location response
      class TLResponse < Struct.new(:timestamp, :accuracy, :latitude, :longitude); end

    end
  end
end
