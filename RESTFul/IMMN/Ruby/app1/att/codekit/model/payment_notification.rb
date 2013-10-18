require 'json'

module Att
  module Codekit
    module Model
      #@author Kyle Hill <kh455g@att.com>
      class PaymentNotification
        def initialize(json, id)
          @json = json
          @json = JSON.parse @json if json.is_a? String
          @id = id
        end

        # Accessor for obtaining the notification id
        def notification_id
          @id
        end

        # Each loop for a payment notification
        def each
          yield "NotificationId", @id
          @json.each do |key, value|
            if key == "GetNotificationResponse" 
              value.each do |skey, svalue|
                yield skey, svalue
              end
            else
              yield key, value
            end
          end
        end

        def generateHtmlTable(table_tag='<table>')
          table = table_tag
          headers = Array.new

          tbody = '<tbody>'
          tbody << '<tr>'
          self.each do |key, value|
            headers << key
            if value.to_s.empty?
              tbody << %(td data-value="#{key}">-</td>)
            else
              tbody << %(td data-value="#{key}">#{value}</td>)
            end
          end
          tbody << '</tr>'
          tbody << '</tbody>'

          theaders = '<thead><tr>'
          headers.each do |key|
            theaders << "<th>#{key}</th>"
          end 
          theaders << '</tr></thead>'

          table << theaders
          table << tbody
          table << '</table>'
        end

        def notification_type
          response = @json["GetNotificationResponse"]
          response["NotificationType"]
        end

        def transaction_id
          response = @json["GetNotificationResponse"]
          response["OriginalTransactionId"]
        end

        # Serializes instance to a json string
        def to_json(*a)
          {
            "json_class" => self.class.name,
            "data" => {"raw_json" => @json, "notification_id" => @id}
          }.to_json(*a)
        end

        # Decodes a json serialized object into a payment notification
        def self.json_create(o)
          new(o["data"]["raw_json"],o["data"]["notification_id"])
        end
      end

    end
  end
end
