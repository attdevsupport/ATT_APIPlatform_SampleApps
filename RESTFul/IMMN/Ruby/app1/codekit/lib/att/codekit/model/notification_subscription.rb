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

      class NotificationSubscription < ImmutableStruct.new(:events,
                                                           :callback_data,
                                                           :expires_in,
                                                           :subscription_id,
                                                           :transaction_id,
                                                           :location,
                                                           :time_created)
        # @!attribute [r] events
        #   @return [Array] list of events to subscribe, a nil value will
        #     default to ALL events. Acceptable values are "TEXT" and/or "MMS" 
        # @!attribute [r] callback_data
        #   @return [String] This is an optional piece of data that the DHS can
        #     pass to the Subscription and expect to be returned along with any
        #     events specific to the Subscription. cannot exceed 50 characters.
        # @!attribute [r] expires_in
        #   @return [Integer] Specify the time-to-live (seconds) for the
        #     subscription
        # @!attribute [r] subscription_id
        #   @return [String] (default: nil) Subscription ID assigned by the API
        #     (not required to create a subscription)
        # @!attribute [r] transaction_id
        #   @return [String] (default: nil) x-systemTransactionId set by the
        #     API (not required to create a subscription)
        # @!attribute [r] location
        #   @return [String] location of the notification channel
        #     (not required to create a subscription)
        # @!attribute [r] time_created
        #   @return [Integer] epoch time the subscription was created 
        #     (not required to create a subscription)

        # Alias for transaction_id
        #
        # @return [String] x-systemTransactionId
        def x_systemTransactionId
          return self.transaction_id
        end

        # Returns if this subscription can expire
        #
        # @return [Boolean] true if the subscription can expire
        def can_expire?
          return !!self.expires_in && !!self.time_created
        end

        # Check if subscription is expired 
        #
        # @return [Boolean] true if subscription is expired, false if it's 
        #   valid.
        def expired?
          can_expire? && (self.expires_in + self.time_created) < Time.now.to_i
        end

        # Construct a NotificationSubscription from API response
        #
        # @param response [RestClient::Response] restclient response object
        #
        # @return [NotificationSubscription]
        def self.from_response(response)
          time_created = Time.now.to_i

          headers = response.headers 
          location = headers[:location] 
          trans_id = headers[:x_systemTransactionId] 

          sub = JSON.parse(response)['subscription']

          subid = sub['subscriptionId']
          expires_in = sub['expiresIn'].to_i
          events = sub['events'] || sub['eventFilters']
          callback = sub['callbackData']

          new(events, callback, expires_in, subid, trans_id,
              location, time_created)
        end

        # Convert NotificationSubscription to json accepted by AT&T API
        def to_json
          obj = {}
          obj[:events] = Array(self.events) unless self.events.nil?
          obj[:callbackData] = self.callback_data unless self.callback_data.nil?
          obj[:expiresIn] = self.expires_in.to_i unless self.expires_in.nil?
          obj[:subscriptionId] = self.subscription_id unless self.subscription_id.nil?
          { :subscription => obj }.to_json
        end
      end

      class DeletedNotificationSubscription < ImmutableStruct.new(:transaction_id)
        # @!attribute [r] transaction_id
        #   @return [String] (default: nil) x-systemTransactionId set by the
        #     API

        # Alias for transaction_id
        #
        # @return [String] x-systemTransactionId
        def x_systemTransactionId
          return self.transaction_id
        end

        # Construct a NotificationSubscription from API response
        #
        # @param response [RestClient::Response] restclient response object
        #
        # @return [NotificationSubscription]
        def self.from_response(response)
          trans_id = response.headers[:x_systemTransactionId] 
          new(trans_id)
        end
      end

    end
  end
end
