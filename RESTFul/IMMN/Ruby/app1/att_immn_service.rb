require 'json'
require 'att_api_service'

module AttCloudServices

  module IMMN
    SCOPE = "IMMN"

    module ENDPOINTS
      IMMN = "/rest/1/MyMessages"
    end

    class ImmnService < AttApiService


      # Send a message using IMMN services. Determins automatically if it 
      # should be a SMS or MMS message based on attachments and group.
      #
      # @param addresses [String] A comma seperated list of phone numbers.
      # @param subject [String] The subject of the message.
      # @param message [String] The body of the message.
      # @param attachments [Array<String>] An array of paths to files, if
      # present a MMS is sent.
      # @param group [Boolean] Send the message as a group message to provided
      # addresses. If true then this methods sends a MMS (default: false)
      def sendMessage(addresses, subject, message, attachments=nil, group=nil)
        subject ||= ""
        message ||= ""

        addresses = addresses.gsub("-","").split(",")
        parsed_addresses = Array.new

        addresses.each do |a|
          parsed_addresses << 'tel:'  + a.strip unless(a.include? "tel:" or a.include? "@")
          parsed_addresses << a.strip if(a.include? "tel:" or a.include? "@")
        end

        payload = {
          :Addresses => parsed_addresses,
          :Text => message,
          :Subject => subject
        }

        url = "#{@oauth.fqdn}#{ENDPOINTS::IMMN}"

        if attachments or group then
          self.sendMMS(url, payload, attachments, group)
        else
          self.sendSMS(url, payload)
        end
      end

      # Send an SMS using IMMN services.
      #
      # @param url [String] The url which accepts payload and sends SMS. 
      # @param payload [#to_json] A json-able object to set as the payload.
      def sendSMS(url, payload)
        content_type = 'application/json' 
        self.postRequest(url, payload.to_json, :Content_Type => content_type)
      end

      # Send an MMS using IMMN services.
      #
      # @param url [String] url which accepts payload and fowards MMS
      # @param payload [#to_json] A payload which responds to #to_json
      # @param attachments [Array<String>, String] An array of paths or a path to a file
      # @param group [Boolean] Send the MMS as a group message (default: false)
      def sendMMS(url, payload, attachments, group=nil)
        attachments = [attachments] if attachments.instance_of? String
        boundary = self.generateBoundary

        if group.nil? or group == false then
          payload[:Group] = false
        else
          payload[:Group] = true 
        end

        boundary = "#{(rand*10000000) + 10000000}.#{Time.now.to_i}"

        headers = {
          "Content-Type" => "application/json; charset=UTF-8",
          "Content-Disposition" => 'form-data; name="root-fields"',
          "Content-ID" => "<startpart>"
        }

        json_payload = {
          :headers => headers,
          :data => payload.to_json
        }

        parts = [json_payload]

        count = 0
        if attachments then
          attachments.each do |attach|
            if attach and not attach.empty? then
              data = File.open(attach, 'rb') { |io| io.read }
              filename = attach.split("/")[-1]

              headers = {
                "Content-Disposition" => "attachment; name=\"file#{count}\"; filename=\"#{filename}\"",
                "Content-Type" => "file; charset=binary",
                "Content-ID" => "#{count}",
                "Content-Transfer-Encoding" => "binary",
              }

              file_payload = {
                :headers => headers,
                :data => data
              }

              count += 1
              parts << file_payload
            end
          end
        end

        multipart = self.generateMultiPart(boundary, parts)

        content_type = 'multipart/related; '
        content_type << 'type="application/json"; ' 
        content_type << 'start="<startpart>"; ' 
        content_type << "boundary=\"#{boundary}\"" 

        self.postRequest(url, multipart, :Content_Type => content_type)
      end

      def generateBoundary
        "----=_Part_#{((rand*10000000) + 10000000).to_i}.#{Time.now.to_i}"
      end

    end
  end
end
