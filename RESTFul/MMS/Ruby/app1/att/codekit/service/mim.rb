# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Service

      #@author Kyle Hill <kh455g@att.com>
      class MIMService < CloudService
        SERVICE_URL = '/rest/1/MyMessages'

        # Cloud API call to obtain message headers
        #
        # @param header_count [#to_i] the number of headers to request
        # @param index [String] the type of headers to return
        def getMessageHeaders(header_count, index=nil)
          url = "#{@fqdn}#{SERVICE_URL}"

          params = "?HeaderCount=#{header_count.to_i}"

          params << "&IndexCursor=#{index}" unless index.nil?

          url << params

          self.get(url)
        end

        # Return the content of a message (only available to mms messages)
        #
        # @param message_id [String] the id of the message header
        # @param part_number [#to_i] the index of the content to retrieve 
        def getMessageContent(message_id, part_number)
          url = "#{@fqdn}#{SERVICE_URL}"
          url << "/#{message_id}/#{part_number.to_i}"

          self.get(url)
        end

      end
    end
  end
end
