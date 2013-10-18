# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Service

      #@author Kyle Hill <kh455g@att.com>
      class SMSService < CloudService
          SERVICE_URL_SEND = '/sms/v3/messaging/outbox'
          SERVICE_URL_RECEIVE = '/sms/v3/messaging/inbox'

        # Send a sms message
        #
        # @param addresses [String] a comma separated list of addresses to send the sms
        # @param message [String] the message to send over sms
        # @param notify [Boolean] if set to true then a notification will be sent to the appropriate url, setup in the app sandbox.
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

          url = "#{@fqdn}#{SERVICE_URL_SEND}"

          self.post(url, payload)
        end

        # Obtain the status of a sent sms message
        #
        # @param sms_id [String] the id of the sms to obtain status
        def smsStatus(sms_id)
          self.get(self.getResourceUrl(sms_id))
        end
        alias_method :getDeliveryStatus, :smsStatus 

        # Get messages sent to a short code.
        #
        # @param short_code [String] the short code to check for messages
        def getReceivedMessages(short_code)
          url = "#{@fqdn}#{SERVICE_URL_RECEIVE}/#{short_code}"
          self.get(url)
        end

        # The url that contains the sms status
        # 
        # @param sms_id (see #getDeliveryStatus)
        def getResourceUrl(sms_id)
          "#{@fqdn}#{SERVICE_URL_SEND}/#{sms_id}"
        end

      end
    end
  end
end
