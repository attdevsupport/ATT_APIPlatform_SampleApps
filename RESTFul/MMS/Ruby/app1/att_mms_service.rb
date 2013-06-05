require 'att_api_service'

module AttCloudServices
  class ServiceException < StandardError; end

  module MMS

    module Endpoints
      SEND = "/mms/v3/messaging/outbox"
    end

    class MmsService < AttApiService
      def initialize(*args)
        super(*args)
      end

      def getDeliveryStatus(mms_id)
        url = "#{@oauth.fqdn}#{Endpoints::MMS}/#{mms_id}"

        RestClient.get url, :Authorization => "Bearer #{@oauth.access_token}", :Accept => 'application/json'
      end

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
          'Content-Disposition' => 'form-data; name="root-fields"'
        }

        data = {
          :headers => headers,
          :data => payload
        }

        multipart_data << data

        if attachments && !attachments.empty? then
          attachment_count = 0
          attachments.each do |file|
            file_path = File.join(ATTACH_DIR,file)
            File.open(file_path, "rb") do |file|

              headers = {
                'Content-Type' => 'image/gif',
                'Content-ID' => "<attachment#{attachment_count}",
                'Content-Transfer-Encoding' => 'base64',
                'Content-Disposition' => 'attachment; name=""; filename=""'      
              }

              data = { 
                :headers => headers,
                :data => "#{Base64.encode64(file.read)}"
              }

              multipart_data << data

            end
            attachment_count += 1
          end
        end

        multipart_body = self.generateMultiPart(boundary, multipart_data)

        url = "#{@oauth.fqdn}#{Endpoints::MMS}"

        RestClient.post url, multipart_body, :Authorization => "Bearer #{@oauth.access_token}", :Accept => 'application/json', :Content_Type => 'multipart/form-data; type="application/json"; start=""; boundary="' + boundary + '"'
      end

      def handleInput(input, boundary="--Nokia-mm-messageHandler-BoUnDaRy")
        sender = /\<SenderAddress\>tel:([0-9\+]+)<\/SenderAddress>/.match(input)[1]
        parts   = input.split(boundary)
        body    = parts[2].split "BASE64"
        type    = /Content\-Type: image\/([^;]+)/.match(body[0])[1];
        date    = Time.now.utc

        random  = rand(10000000).to_s

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
