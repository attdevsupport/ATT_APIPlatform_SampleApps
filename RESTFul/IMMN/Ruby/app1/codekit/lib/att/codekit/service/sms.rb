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

require 'cgi'
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

          if parsed_addresses.empty?
            raise(ServiceException, "No valid address was specified!")
          end

          # send in array if more than one otherwise string
          parsed_addresses = parsed_addresses[0] unless parsed_addresses.length > 1

          #make sure that notify is a boolean
          notify = notify.to_s.downcase == "true"

          sms_request = { 
            :address => parsed_addresses, 
            :message => CGI.escape(message), 
            :notifyDeliveryStatus => notify
          }

          payload = { :outboundSMSRequest => sms_request }.to_json

          url = "#{@fqdn}#{SERVICE_URL_SEND}"

          headers = {
            :Accept => "application/json",
            :Content_Type => "application/json",
          }

          begin
            response = self.post(url, payload, headers)
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
          url = self.getResourceUrl(CGI.escape(sms_id.to_s))

          headers = {
            :Accept => "application/json",
          }
          
          begin
            response = self.get(url, headers)
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
        # @return [Model::SMSMessageList] the message list that was retrieved
        #   at the short code
        def getReceivedMessages(short_code)
          url = "#{@fqdn}#{SERVICE_URL_RECEIVE}/#{CGI.escape(short_code.to_s)}"

          headers = {
            :Accept => "application/json",
          }

          begin
            response = self.get(url, headers)
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
          "#{@fqdn}#{SERVICE_URL_SEND}/#{CGI.escape(sms_id.to_s)}"
        end

        def self.handleSmsMessage(input)
        end

      end
    end
  end
end

#  vim: set ts=8 sw=2 tw=0 et :
