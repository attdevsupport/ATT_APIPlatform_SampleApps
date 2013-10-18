# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Service

      #@author Kyle Hill <kh455g@att.com>
      class MMSService < CloudService
        SERVICE_URL_SEND = "/mms/v3/messaging/outbox"

        # Send an MMS message
        #
        # @param address_string [String] a comma separated list of phone numbers
        # @param subject [String] subject of the message to send
        # @param attachments [Array<String>] array of paths to files to be attached
        # @param notify [Boolean] if set to true then a notification will be sent to the appropriate url, setup in the app sandbox.
        def sendMms(address_string, subject, attachments, notify=false)
          addresses = address_string.gsub("-","").split(",")
          parsed_addresses = Array.new

          addresses.each do |a|
            parsed_addresses << 'tel:'  + a.strip unless(a.include? "tel:" or a.include? "@")
            parsed_addresses << a.strip if a.include? "tel:"
          end

          # send in array if more than one otherwise string
          parsed_addresses = parsed_addresses.to_s unless parsed_addresses.size > 1

          boundary = "#{((rand*10000000) + 10000000).to_i}.#{Time.new.to_i}"

          multipart_data = []

          payload = {
            :outboundMessageRequest => { :address => parsed_addresses, :subject => subject, :notifyDeliveryStatus => notify }
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
            attachments.each do |attach|
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
          self.post(url, body, :Content_Type => content_type)
        end

        # Query the current status of a sent message
        #
        # @param mms_id [String] the id of the message being queried
        def mmsStatus(mms_id)
          url = "#{@fqdn}#{SERVICE_URL_SEND}/#{mms_id}"

          self.get(url)
        end
        alias_method :getDeliveryStatus, :mmsStatus 

        # Decodes an inbound mms message
        #
        # @param input [String] the full html request being decoded
        # @param boundary [String] the boundary to split data on
        #
        # @yieldparam sender [String] the sender of the message
        # @yieldparam date [String] the date sent
        # @yieldparam text [String] message body contained inside the message
        # @yieldparam type [String] content type of the message
        # @yieldparam part [Base64, String] the base64 decoded body 
        def parseReceivedMms(input, boundary="--Nokia-mm-messageHandler-BoUnDaRy")
          sender = /\<SenderAddress\>tel:([0-9\+]+)<\/SenderAddress>/.match(input)[1]
          parts   = input.split(boundary)
          body    = parts[2].split "BASE64"
          type    = /Content\-Type: image\/([^;]+)/.match(body[0])[1];
          date    = Time.now.utc

          random  = rand(10_000_000).to_s

          text = parts.length > 4 ? Base64.decode64(parts[3].split("BASE64")[1]).strip : ""

          yield sender
          yield date
          yield text
          yield type

          yield Base64.decode64 body[1]
        end

      end
    end
  end
end
