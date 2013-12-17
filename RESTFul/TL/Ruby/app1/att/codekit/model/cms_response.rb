module Att
  module Codekit
    module Model

      #Model for containing a cms request
      class CMSResponse < Struct.new(:success, :id)
        def success?
          self.success
        end
      end

      #Model for containing a signal request
      class SignalResponse < Struct.new(:status); end

    end
  end
end
