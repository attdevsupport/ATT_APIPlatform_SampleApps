
module Att
  module Codekit
    module Model

      #Holds the response for sending an sms
      class SMSResponse < Struct.new(:id, :resource_url)
        def url
          self.resource_url
        end
      end

      #Contains an array of info objects and resource url
      class SMSStatus < Struct.new(:delivery_info, :resource_url); end

      #individual delivery info 
      class SMSDeliveryInfo < Struct.new(:id, :address, :status); end

      #Container for receiving sms messages
      class SMSMessageList < Struct.new(:messages, :count, :pending, :url); end

      #Container for a single sms message
      class SMSMessage < Struct.new(:id, :message, :sender); end

    end
  end
end
