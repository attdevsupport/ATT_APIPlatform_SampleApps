# Copyright 2015 AT&T
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

require 'json'
require_relative '../model/immn'

module Att
  module Codekit
    module Service

      #@author kh455g
      class IMMNService < CloudService
        SERVICE_URL = '/myMessages/v2/messages'

        # Send a message using IMMN services. Determines automatically if it 
        # should be a SMS or MMS message based on attachments, group and 
        # subject.
        #
        # @param addresses [String] A comma seperated list of phone numbers.
        # @param opts [Hash] Optional arguments hash
        #
        # @option opts [String] :message The body of the message.
        # @option opts [String] :subject The subject of the message, if 
        #   present a MMS is sent.
        # @option opts [Array<String>] :attachments An array of paths to files, if 
        #   present a MMS is sent.
        # @option opts [Boolean] :group Send the message as a group message to 
        #   provided addresses. If true then this methods sends a MMS
        #
        # @raise [ServiceException] A problem went wrong with processing the 
        #   request
        # @return [Model::IMMNResponse] parsed api response 
        def sendMessage(addresses, opts={})
          message = opts[:message]
          subject = opts[:subject]
          group = opts[:group]
          attachments = (opts[:attachments] || opts[:attachment])

          if (attachments || group || !subject.to_s.empty?)
            immnSendMMS(addresses, 
                        :attachments => attachments, 
                        :message => message,
                        :subject => subject, 
                        :group => group)
          else
            immnSendSMS(addresses, message)
          end
        end

        # Send an SMS using IMMN services.
        #
        # @param addresses [Array<String>] A list of phone numbers to send the 
        #   message 
        # @param url [String] The url which accepts payload and sends SMS. 
        #
        # @raise [ServiceException] A problem went wrong with processing the 
        #   request
        # @return [Model::IMMNResponse] parsed api response 
        def immnSendSMS(addresses, text)
          url = "#{@fqdn}#{SERVICE_URL}"


          addresses = CloudService.format_addresses(addresses)

          payload = {
            :messageRequest => {
              :addresses => addresses,
              :text => text.to_s,
            }
          }

          headers = {
            :Content_Type => 'application/json',
            :Accept => 'application/json',
          }

          begin
            response = self.post(url, payload.to_json, headers)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::IMMNResponse.createFromJson(response)
        end

        # Send an MMS using IMMN services.
        #
        # @param addresses [String] A comma seperated list of phone numbers.
        # @param opts [Hash] Optional arguments hash
        #
        # @option opts [String] :message The body of the message.
        # @option opts [String] :subject The subject of the message, if 
        #   present a MMS is sent.
        # @option opts [Array<String>] :attachments An array of paths to files, if 
        #   present a MMS is sent.
        # @option opts [String] :attachment singular form of :attachments
        # @option opts [Boolean] :group Send the message as a group message to 
        #   provided addresses. If true then this methods sends a MMS
        #
        # @raise [ServiceException] A problem went wrong with processing the 
        #   request
        # @return [Model::IMMNResponse] parsed api response 
        def immnSendMMS(addresses, opts={})
          url = "#{@fqdn}#{SERVICE_URL}"

          addresses = CloudService.format_addresses(addresses)

          attachments = (opts[:attachments] || opts[:attachment])
          text = (opts[:text] || opts[:message])
          group = (opts[:group] || false)
          subject = opts[:subject]

          payload = { :addresses => addresses }
          payload[:text] = text unless text.to_s.empty?
          payload[:subject] = subject unless subject.to_s.empty?

          boundary = CloudService.generateBoundary

          if (group && addresses.count > 1)
            payload[:isGroup] = true 
          else
            payload[:isGroup] = false
          end

          part_headers = {
            "Content-Type" => "application/json; charset=UTF-8",
            "Content-Disposition" => 'form-data; name="root-fields"',
            "Content-ID" => "<startpart>"
          }

          json_payload = {
            :headers => part_headers,
            :data => { :messageRequest => payload }.to_json
          }

          parts = [json_payload]

          count = 0
          if attachments 
            Array(attachments).each do |attach|
              if attach && !attach.empty? 
                data = File.open(attach, 'rb') { |io| io.read }
                mime = CloudService.getMimeType(attach);
                filename = attach.split("/")[-1]

                part_headers = {
                  "Content-Disposition" => %(attachment; name="file#{count}"; filename="#{filename}"),
                  "Content-Type" => %(#{mime}; charset="binary"),
                  "Content-ID" => "#{count}",
                  "Content-Transfer-Encoding" => "binary",
                }

                file_payload = {
                  :headers => part_headers,
                  :data => data.to_s
                }

                count += 1
                parts << file_payload
              end
            end
          end

          multipart = CloudService.generateMultiPart(boundary, parts)

          content_type = %{multipart/related; type="application/json"; start="<startpart>"; boundary="#{boundary}"}

          headers = {
            :Content_Type => content_type,
            :Accept => 'application/json',
          }

          begin
            response = self.post(url, multipart, headers)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::IMMNResponse.createFromJson(response)
        end

      end
    end
  end
end
