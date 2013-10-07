# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013 TERMS
# AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:
# http://developer.att.com/sdk_agreement/ Copyright 2013 AT&T Intellectual
# Property. All rights reserved. http://developer.att.com For more information
# contact developer.support@att.com

require 'att/codekit/model/cms'

module Att
  module Codekit
    module Service

      #@author kh455g
      class CMSService < CloudService
        SERVICE_URL = "/rest/1/Sessions"

        # Creates a new session with the specified args
        #
        # @param args [Hash] passes "variable names" => "values" to the session being created
        #
        # @return [Model::CMSResponse] parsed api response
        def createSession(args={})
          url = "#{@fqdn}#{SERVICE_URL}"

          begin
            response = self.post(url, args.to_json)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::CMSResponse.createFromJson(response)
        end
        alias_method :create_session, :createSession

        # Function which sends signal to current CMS session 
        #
        # @param id [String] the CMS session id
        # @param signal [String] sends the designated signal to the specified id
        #
        # @return [SignalResponse] parsed api response
        def sendSignal(id, signal)
          url = "#{@fqdn}#{SERVICE_URL}/#{id}/Signals"

          body = {:signal => signal.to_s}

          begin
            response = self.post(url, body.to_json)
          rescue RestClient::Exception => e
            raise(ServiceException, e.response || e.message, e.backtrace)
          end
          Model::SignalResponse.createFromJson(response)
        end
        alias_method :send_signal, :sendSignal
      end

    end
  end
end
