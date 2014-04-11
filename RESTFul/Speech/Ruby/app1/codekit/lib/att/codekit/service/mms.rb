# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2014 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2014 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'json'
require_relative '../model/mms'

module Att
  module Codekit
    module Service

      #@author kh455g
      class MMSService < CloudService
        SERVICE_URL_SEND = "/mms/v3/messaging/outbox"

        # Send an MMS message
        #
        # @param addresses [String] a comma separated list of phone numbers
        # @param subject [String] subject of the message to send
        # @param attachments [Array<String>] array of paths to files to be attached
        # @param notify [Boolean] if set to true then a notification will be sent to the appropriate url, setup in the app sandbox.
        #
        # @return [Model::MMSResponse] parsed api response
        def sendMms(addresses, subject, attachments, notify=false)
          parsed_addresses = CloudService.format_addresses(addresses)

          # send in array if more than one, otherwise string
          parsed_addresses = parsed_addresses.to_s unless parsed_addresses.size > 1

          boundary = "#{((rand*10000000) + 10000000).to_i}.#{Time.new.to_i}"

          multipart_data = []

          #make absolutely sure notify is a boolean
          notify = notify.to_s.downcase == "true"

          payload = {
            :outboundMessageRequest => { :address => parsed_addresses,
                                         :subject => subject, 
                                         :notifyDeliveryStatus => notify }
          }.to_json

          headers = {
            'Content-Type' => 'application/json',
            'Content-ID' => '<startpart>',
          }

          data = {
            :headers => headers,
            :data => payload
          }

          multipart_data << data

          if attachments && !attachments.empty? 
            attachment_count = 0
            Array(attachments).each do |attach|
              File.open(attach, "rb") do |file|
                path = file.path
                filename = path.split("/").last
                mime = CloudService.getMimeType(path)

                headers = {
                  'Content-Type' => mime.to_s,
                  'Content-ID' => "attachment#{attachment_count}",
                  'Content-Transfer-Encoding' => 'binary',
                  'Content-Disposition' => %(attachment; name="file#{attachment_count}"; filename="#{filename}")
                }

                data = { 
                  :headers => headers,
                  :data => "#{file.read}"
                }

                multipart_data << data

              end
              attachment_count += 1
            end
          end
          url = "#{@fqdn}#{SERVICE_URL_SEND}"
          content_type = 'multipart/related; type="application/json"; start="<startpart>"; boundary="' + boundary + '"'
          body = CloudService.generateMultiPart(boundary, multipart_data)

          begin
            response = self.post(url, body, :Content_Type => content_type)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::MMSResponse.createFromJson(response)
        end

        # Query the current status of a sent message
        #
        # @param mms_id [String] the id of the message being queried
        #
        # @return [Model::MMSStatus] parsed api response
        def mmsStatus(mms_id)
          url = "#{@fqdn}#{SERVICE_URL_SEND}/#{mms_id}"

          begin
            response = self.get(url)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::MMSStatus.createFromJson(response)
        end
        alias_method :getDeliveryStatus, :mmsStatus 

        # Decodes an inbound mms message
        #
        # @param input [String] the full html request being decoded
        # @param boundary [String] the boundary to split data on
        #
        # @return [Model::MMSMessage] parsed api response
        def parseReceivedMms(input, boundary="--Nokia-mm-messageHandler-BoUnDaRy")
          sender = /\<SenderAddress\>tel:([0-9\+]+)<\/SenderAddress>/.match(input)[1]
          parts   = input.split(boundary)
          body    = parts[2].split "BASE64"
          type    = /Content\-Type: image\/([^;]+)/.match(body[0])[1];
          date    = Time.now.utc

          random  = rand(10_000_000).to_s

          text = parts.length > 4 ? Base64.decode64(parts[3].split("BASE64")[1]).strip : ""

          attachment = Base64.decode64 body[1]

          Model::MMSMessage.new(sender, date, text, type, attachment)
        end

      end
    end
  end
end
