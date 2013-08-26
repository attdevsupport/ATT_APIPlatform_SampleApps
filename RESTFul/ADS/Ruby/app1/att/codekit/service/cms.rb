# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

module Att
  module Codekit
    module Service

      #@author Kyle Hill <kh455g@att.com>
      class CMSService < CloudService
        SERVICE_URL = "/rest/1/Sessions"

        def create_session(args={})
          # Resource URL for Create Session.
          url = "#{@fqdn}#{SERVICE_URL}"

          self.post(url, args.to_json)
        end

        # Function which sends signal to current CMS session 
        #
        # @param id [String] the CMS session id
        # @param signal [String] sends the designated signal to the specified id
        def send_signal(id, signal)
          url = "#{@fqdn}#{SERVICE_URL}/#{id}/Signals"

          body = {:signal => signal.to_s}

          self.post(url, body.to_json)
        end

      end

    end
  end
end
