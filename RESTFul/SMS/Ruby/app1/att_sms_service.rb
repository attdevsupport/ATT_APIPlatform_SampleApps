require 'att_api_service'

module AttCloudServices

  module SMS

    SCOPE = "SMS"

    module ENDPOINTS 
      SEND = '/sms/v3/messaging/outbox'
      RECEIVED = '/sms/v3/messaging/inbox'
    end

    class SmsService < AttApiService

      def sendSms(addresses, message, notify)
        addresses = addresses.gsub("-","").split(",")
        parsed_addresses = Array.new

        addresses.each do |a|
          parsed_addresses << 'tel:'  + a.strip unless(a.include? "tel:" or a.include? "@")
          parsed_addresses << a.strip if(a.include? "tel:" or a.include? "@")
        end

        # send in array if more than one otherwise string
        parsed_addresses = parsed_addresses.to_s unless parsed_addresses.length > 1

        sms_request = { 
          :address => parsed_addresses, 
          :message => message, 
          :notifyDeliveryStatus => notify.to_s 
        }

        payload = { :outboundSMSRequest => sms_request }.to_json

        url = "#{@oauth.fqdn}#{ENDPOINTS::SEND}"

        self.postRequest(url, payload)
      end

      def getDeliveryStatus(sms_id)
        self.getRequest(self.getResourceUrl(sms_id))
      end

      def getReceivedMessages(short_code)
        url = "#{@oauth.fqdn}#{ENDPOINTS::RECEIVED}/#{short_code}"
        self.getRequest(url)
      end

      def getResourceUrl(sms_id)
        "#{@oauth.fqdn}#{ENDPOINTS::SEND}/#{sms_id}"
      end

    end
  end
end
