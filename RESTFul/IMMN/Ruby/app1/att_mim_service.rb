require 'att_api_service'

module AttCloudServices

  module MIM
    SCOPE = "MIM"

    module ENDPOINTS
      MIM = '/rest/1/MyMessages'
    end

    class MimService < AttApiService

      def getMessageHeaders(header_count, index=nil)
        url = "#{@oauth.fqdn}#{ENDPOINTS::MIM}"
        
        params = "?HeaderCount=#{header_count}"

        url << params

        self.getRequest(url)
      end

      def getMessageContent(message_id, part_number)
        url = "#{@oauth.fqdn}#{ENDPOINTS::MIM}"
        url << "/#{message_id}/#{part_number}"

        self.getRequest(url)
      end

    end
  end
end
