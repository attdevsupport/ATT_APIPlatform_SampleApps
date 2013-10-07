
module Att
  module Codekit
    module Model

      #Contains an array of info objects and resource url
      class MMSStatus < Struct.new(:delivery_info, :resource_url)
        def url
          self.resource_url
        end
        def each
          self.delivery_info.each {|x| yield x}
        end
      end

      #Contains an individual delivery info status
      class MMSDeliveryInfo < Struct.new(:id, :address, :status); end

      #Container for holding a parsed for mms send message api call
      class MMSResponse < Struct.new(:id, :resource_url)
        def url
          self.resource_url
        end
      end

      #Container for holding a parsed mms message
      class MMSMessage < Struct.new(:sender, :date, :text, :type, :attachment); end

    end
  end
end
