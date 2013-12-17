# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'json'
require_relative '../model/sms'

module Att
  module Codekit
    module Service

      #@author kh455g
      class SMSService < CloudService
        SERVICE_URL_SEND = '/sms/v3/messaging/outbox'
        SERVICE_URL_RECEIVE = '/sms/v3/messaging/inbox'

        # Send a sms message
        #
        # @param addresses [String] a comma separated list of addresses to send the sms
        # @param message [String] the message to send over sms
        # @param notify [Boolean] if set to true then a notification will be sent to the appropriate url, setup in the app sandbox.
        #
        # @return [Model::SMSResponse] container for sms send response
        def sendSms(addresses, message, notify=false)
          parsed_addresses = CloudService.format_addresses(addresses)

          # send in array if more than one otherwise string
          parsed_addresses = parsed_addresses.to_s unless parsed_addresses.length > 1

          #make sure that notify is a boolean
          notify = notify.to_s.downcase == "true"

          sms_request = { 
            :address => parsed_addresses, 
            :message => message, 
            :notifyDeliveryStatus => notify
          }

          payload = { :outboundSMSRequest => sms_request }.to_json

          url = "#{@fqdn}#{SERVICE_URL_SEND}"

          begin
            response = self.post(url, payload)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response, e.backtrace)
          end
          Model::SMSResponse.createFromJson(response)
        end

        # Obtain the status of a sent sms message
        #
        # @param sms_id [String] the id of the sms to obtain status
        #
        # @return [Model::SMSStatus] the status of the sms with specified id
        def smsStatus(sms_id)
          begin
            response = self.get(self.getResourceUrl(sms_id))
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::SMSStatus.createFromJson(response)
        end
        alias_method :getDeliveryStatus, :smsStatus 

        # Get messages sent to a short code.
        #
        # @param short_code [String] the short code to check for messages
        # 
        # @return [Model::SMSMessageList] the message list that was retrieved at the 
        #   short code
        def getReceivedMessages(short_code)
          url = "#{@fqdn}#{SERVICE_URL_RECEIVE}/#{short_code}"
          begin
            response = self.get(url)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::SMSMessageList.createFromJson(response)
        end

        # The url that contains the sms status
        # 
        # @param sms_id (see #getDeliveryStatus)
        #
        # @return [String] the url that the resource is contained
        def getResourceUrl(sms_id)
          "#{@fqdn}#{SERVICE_URL_SEND}/#{sms_id}"
        end

      end
    end
  end
end
