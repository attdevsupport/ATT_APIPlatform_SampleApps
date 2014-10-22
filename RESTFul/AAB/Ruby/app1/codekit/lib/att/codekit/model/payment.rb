# Copyright 2014 AT&T
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

      class PaymentStatus 
        include Enumerable
        @@attributes = [:id, :amount, :channel, :consumer_id,
                        :content_category, :currency, :description,
                        :merchant_application_id, :merchant_product_id,
                        :merchant_transaction_id, :success, :status, :type,
                        :version, :original_transaction_id]

        @@attributes.each {|attr| attr_reader attr}

        def initialize(opts={})
          @id = opts[:id]
          @amount = opts[:amount]
          @channel = opts[:channel]
          @consumer_id = opts[:consumer_id]
          @content_category = opts[:content_category]
          @currency = opts[:currency]
          @description = opts[:description]
          @merchant_application_id = opts[:merchant_application_id]
          @merchant_product_id = opts[:merchant_product_id]
          @merchant_transaction_id = opts[:merchant_transaction_id]
          @success = opts[:success]
          @status = opts[:status] 
          @type = opts[:type]
          @version = opts[:version]
          @original_transaction_id = opts[:original_transaction_id]
        end

        def success?
          !!@success
        end

        def each
          self.each_pair do |name, value|
            yield value
          end
        end

        def each_pair
          @@attributes.each do |attr|
            value = self.send(attr)
            yield attr, value
          end
        end
      end

      class TransactionStatus < PaymentStatus

        alias_method :transaction_id, :id
        alias_method :transaction_status, :status
        alias_method :transaction_type, :type

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          self.json_create(json)
        end

        def to_json(*a)
          {
            :TransactionId => self.transaction_id,
            :Amount => self.amount,
            :Channel => self.channel,
            :ConsumerId => self.consumer_id,
            :ContentCategory => self.content_category,
            :Currency => self.currency,
            :Description => self.description,
            :MerchantApplicationId => self.merchant_application_id,
            :MerchantProductId => self.merchant_product_id,
            :MerchantTransactionId => self.merchant_transaction_id,
            :IsSuccess => self.success,
            :TransactionStatus => self.transaction_status,
            :TransactionType => self.transaction_type,
            :Version => self.version,
            :OriginalTransactionId => self.original_transaction_id
          }.to_json(*a)
        end

        def self.json_create(o)
          new(:id                      => o["TransactionId"],
              :amount                  => o["Amount"],
              :channel                 => o["Channel"],
              :consumer_id             => o["ConsumerId"],
              :content_category        => o["ContentCategory"],
              :currency                => o["Currency"],
              :description             => o["Description"],
              :merchant_application_id => o["MerchantApplicationId"],
              :merchant_product_id  => o["MerchantProductId"],
              :merchant_transaction_id => o["MerchantTransactionId"],
              :success                 => o["IsSuccess"],
              :status                  => o["TransactionStatus"],
              :type                    => o["TransactionType"],
              :version                 => o["Version"],
              :original_transaction_id => o["OriginalTransactionId"])
        end
      end

      class SubscriptionStatus < PaymentStatus
        @@subscription_attributes = [:auto_committed, :merchant_subscription_id,
                                     :period, :period_amount, :recurrences]

        @@subscription_attributes.each {|attr| attr_reader attr}

        alias_method :subscription_id, :id
        alias_method :subscription_status, :status
        alias_method :subscription_type, :type

        def initialize(opts={})
          super(opts)
          @auto_committed = opts[:auto_committed]
          @merchant_subscription_id = opts[:merchant_subscription_id]
          @period = opts[:period]
          @period_amount = opts[:period_amount]
          @recurrences = opts[:recurrences]
        end

        def auto_committed?
          !!@auto_committed
        end

        def each_pair
          @@attributes.each do |attr|
            value = self.send(attr)
            yield attr, value
          end
          @@subscription_attributes.each do |attr|
            value = self.send(attr)
            yield attr, value
          end
        end

        def to_json(*a)
          {
            :SubscriptionId => self.subscription_id,
            :Amount => self.amount,
            :Channel => self.channel,
            :ConsumerId => self.consumer_id,
            :ContentCategory => self.content_category,
            :Currency => self.currency,
            :Description => self.description,
            :MerchantApplicationId => self.merchant_application_id,
            :MerchantProductId => self.merchant_product_id,
            :MerchantTransactionId => self.merchant_transaction_id,
            :MerchantSubscriptionId => self.merchant_subscription_id,
            :IsSuccess => self.success,
            :SubscriptionStatus => self.status,
            :SubscriptionType => self.type,
            :Version => self.version,
            :IsAutoCommitted => self.auto_committed,
            :SubscriptionPeriod => self.period,
            :SubscriptionPeriodAmount => self.period_amount,
            :SubscriptionRecurrences => self.recurrences,
            :OriginalTransactionId => self.original_transaction_id
          }.to_json(*a)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          self.json_create(json)
        end

        def self.json_create(o)
          new(:id                       => o["SubscriptionId"],
              :amount                   => o["Amount"],
              :channel                  => o["Channel"],
              :consumer_id              => o["ConsumerId"],
              :content_category         => o["ContentCategory"],
              :currency                 => o["Currency"],
              :description              => o["Description"],
              :merchant_application_id  => o["MerchantApplicationId"],
              :merchant_product_id   => o["MerchantProductId"],
              :merchant_transaction_id  => o["MerchantTransactionId"],
              :merchant_subscription_id => o["MerchantSubscriptionId"],
              :success                  => o["IsSuccess"],
              :status                   => o["SubscriptionStatus"],
              :type                     => o["SubscriptionType"],
              :version                  => o["Version"],
              :auto_committed            => o["IsAutoCommitted"],
              :period                   => o["SubscriptionPeriod"],
              :period_amount            => o["SubscriptionPeriodAmount"],
              :recurrences              => o["SubscriptionRecurrences"],
              :original_transaction_id  => o["OriginalTransactionId"])
        end
      end

      class TransactionRefund < ImmutableStruct.new(:transaction_id,
                                                    :transaction_status,
                                                    :success,
                                                    :version,
                                                    :original_purchase_amount) 
        def to_json(*a)
          {
            :TransactionId => self.transaction_id,
            :TransactionStatus => self.transaction_status,
            :IsSuccess => self.success,
            :Version => self.version,
            :OriginalPurchaseAmount => self.original_purchase_amount
          }.to_json(*a)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          self.json_create(json)
        end

        def self.json_create(o)
          new(o["TransactionId"],
              o["TransactionStatus"],
              o["IsSuccess"],
              o["Version"],
              o["OriginalPurchaseAmount"])
        end
      end

      class SubscriptionDetails < ImmutableStruct.new(:currency,
                                                      :status,
                                                      :creation_date,
                                                      :gross_amount,
                                                      :recurrences,
                                                      :active,
                                                      :current_start_date,
                                                      :current_end_date,
                                                      :recurrences_left,
                                                      :version,
                                                      :success)

        def success?
          !!self.success
        end

        def active?
          if self.active.nil?
            raise(Exception, "Unknown, active is nil") 
          end
          !!self.active
        end

        def to_json(*a)
          {
            "Currency"              => self.currency,
            "Status"                => self.status,
            "CreationDate"          => self.creation_date,
            "GrossAmount"           => self.gross_amount,
            "Recurrences"           => self.recurrences,
            "IsActiveSubscription"  => self.active,
            "CurrentStartDate"      => self.current_start_date,
            "CurrentEndDate"        => self.current_end_date,
            "RecurrencesLeft"       => self.recurrences_left,
            "Version"               => self.version,
            "IsSuccess"             => self.success
          }.to_json(*a)
        end

        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        def self.createFromParsedJson(json)
          self.json_create(json)
        end

        def self.json_create(o)
          new(o["Currency"],
              o["Status"],
              o["CreationDate"],
              o["GrossAmount"],
              o["Recurrences"],
              o["IsActiveSubscription"],
              o["CurrentStartDate"],
              o["CurrentEndDate"],
              o["RecurrencesLeft"],
              o["Version"],
              o["IsSuccess"])
        end
      end


    end
  end
end

#  vim: set ts=8 sw=2 tw=79 et :
