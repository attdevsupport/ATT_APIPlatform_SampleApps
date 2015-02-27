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
require 'immutable_struct'

module Att
  module Codekit
    module Model

      class NotificationChannel < ImmutableStruct.new(:location, 
                                                      :transaction_id,
                                                      :channel_id,
                                                      :max_events,
                                                      :channel_type,
                                                      :content_type,
                                                      :version)
        # @!attribute [r] location
        #   @return [String] location of the notification channel
        # @!attribute [r] transaction_id
        #   @return [String] debug information returned by AT&T APIs
        # @!attribute [r] channel_id
        #   @return [String] Specifies the unique Id of the Notification Channel
        # @!attribute [r] max_events
        #   @return [String] maximum number of notification messages that will
        #     be included in any single Notification sent to the subscribing
        #     application through the Notification Channel.
        
        # Alias for max_events
        #
        # @return [String] maximum number of notification messages that will
        #   be included in any single Notification sent to the subscribing
        #   application through the Notification Channel.
        def max_events_per_notification
          self.max_events
        end

        # Alias for transaction_id
        #
        # @return [String] debug information returned by AT&T APIs
        def system_transaction_id
          self.transaction_id
        end

        def to_json(*a)
            {
              :json_class => self.class.name,
              :location => self.location,
              :transaction_id => self.transaction_id,
              :channel => {
                :channelId => self.channel_id,
                :maxEventsPerNotification => self.max_events,
                :channelType => self.channel_type,
                :notificationContentType => self.content_type,
                :notificationVersion => self.version
              }
            }.to_json(*a)
        end

        # Create a CreatedNotificationChannel object
        #
        # @param response [RestClient::Response] restclient response object
        #
        # @return [CreatedNotificationChannel]
        def self.from_response(response)
          headers = response.headers 
          location = headers[:location] 
          trans_id = headers[:x_systemTransactionId] 

          channel = JSON.parse(response)['channel']

          channel_id = channel['channelId']
          max_events = channel['maxEventsPerNotification']

          channel_type  = channel['channelType']
          content_type  = channel['notificationContentType']
          version       = channel['notificationVersion']

          new(location, trans_id, channel_id, max_events, 
              channel_type, content_type, version)
        end

        def self.json_create(o)
            c = o['channel']
            new(o['location'], o['transaction_id'], c['channelId'],
                c['maxEventsPerNotification'], c['channelType'],
                c['notificationContentType'], c['notificationVersion'])
        end

      end

    end
  end
end
