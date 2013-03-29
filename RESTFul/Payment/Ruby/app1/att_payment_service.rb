# Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2013
# TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
# Copyright 2013 AT&T Intellectual Property. All rights reserved. http://developer.att.com
# For more information contact developer.support@att.com

require 'json'
require 'rest_client'
require 'att_oauth_token'
require 'att_oauth_service'

#@author Kyle Hill <kh455g@att.com>
module AttCloudServices

  class ServiceException < StandardError; end

  module Payment

    # Defining constants for payment
    module Endpoints
      TRANSACTIONS = "/rest/3/Commerce/Payment/Transactions"
      NOTARY_SIGNATURE = "/Security/Notary/Rest/1/SignedPayload"
      SUBSCRIPTIONS = "/rest/3/Commerce/Payment/Subscriptions"
    end

    module Categories
      IN_APP_GAMES = 1
      IN_APP_VIRTUAL_GOODS = 2
      IN_APP_OTHER = 3
      APPLICATION_GAMES = 4
      APPLICATION_OTHER = 5
    end

    module TransactionType
      TransactionId = "TransactionId"
      TransactionAuthCode = "TransactionAuthCode"
      MerchantTransactionId = "MerchantTransactionId"
    end

    module SubscriptionType
      SubscriptionId = "SubscriptionId"
      SubscriptionAuthCode = "SubscriptionAuthCode"
      MerchantTransactionId = "MerchantTransactionId"
    end

    module TransactionState
      Refunded = "Refunded"
      SubscriptionCancelled = "SubscriptionCancelled"
    end

    module RefundCodes
      CP_None = 1
      CP_Loss_of_Eligibility = 2
      CP_Product_Termination = 3
      CP_Abuse_of_Privileges = 4
      CP_Conversion_to_New_Product = 5
      CP_Nonrenewable_Product = 6
      CP_Duplicate_Subscription =7 
      CP_Other = 8
      Subscriber_None = 9
      Subscriber_Did_Not_Use = 10
      Subscriber_Too_Expensive = 11
      Subscriber_Did_Not_Like = 12
      Subscriber_Replaced_By_Same_Company = 13
      Subscriber_Replaced_By_Different_Company =14
      Subscriber_Duplicate_Subscription = 15
      Subscriber_Other = 16
    end

    class PaymentService < OAuthService

      # Creates a new Payment service
      #
      # (see #AttService.new)
      def initialize(*args)
        super(*args)
      end

      # Create a new transaction and return a url for authentication
      #
      # @param amount [#to_f] how much the item costs, rounds to 2 decimal places
      # @param category [#to_i] see module Categories for possible values
      # @param desc [String] short description of purchase, must be less than 128 chars
      # @param merch_trans_id [String] the transaction id in merchant's system, must be unique for every purchase
      # @param merch_prod_id [String] specifies the product id of the item purchased, must be less than 50 chars
      # @param [Hash] opts an option hash to define additional parameters
      # @option opts [String] :redirect_uri the location to redirect to after a new transaction
      # @option opts [String] :channel defines the merchant user interface, currently only one option so ignore
      # @return [String] a url that can be redirected to for completing authentication of a payment
      def newTransaction(amount, category, desc, merch_trans_id, merch_prod_id, opts={})
        redirect_uri = (opts[:redirect_uri] || @redirect_uri)
        channel = (opts[:channel] || "MOBILE_WEB")

        payload = {
          :Amount => amount,
          :Category => category.to_i,
          :Description => desc,
          :MerchantTransactionId => merch_trans_id,
          :MerchantProductId => merch_prod_id,
          :MerchantPaymentRedirectUrl => redirect_uri,
          :Channel => channel,
        }.to_json

        response = signPayload(payload)

        from_json = JSON.parse response

        parameters = "?Signature=#{from_json['Signature']}&SignedPaymentDetail=#{from_json['SignedDocument']}&clientid=#{@client_id}"

        @fqdn + Endpoints::TRANSACTIONS + parameters
      end

      # Create a new transaction and return a url for authentication
      #
      # @param amount [#to_f] how much the item costs, rounds to 2 decimal places
      # @param category [#to_i] see module Categories for possible values
      # @param desc [String] short description of purchase, must be less than 128 chars
      # @param merch_trans_id [String] the transaction id in merchant's system, must be unique for every purchase
      # @param merch_prod_id [String] specifies the product id of the item purchased, must be less than 50 chars
      # @param [Hash] opts an option hash to define additional parameters
      # @option opts [String] :redirect_uri the location to redirect to after a new transaction
      # @option opts [String] :channel defines the merchant user interface, currently only one option so ignore
      # @option opts [Integer] :sub_period_amount number of subscription periods between renewals, currently only one option so ignore
      # @option opts [String] :sub_period The interval of periods, currently only one option ('MONTHLY') so ignore
      # @option opts [Boolean] :iponas Current documentation is unclear, currently only one option so ignore
      # @return [String] a url that can be redirected to for completing authentication of a subscription
      def newSubscription(amount, category, desc, merch_trans_id, merch_prod_id, merch_sub_id_list, sub_recurrances, opts={})
        redirect_uri = (opts[:redirect_uri] || @redirect_uri)
        sub_period_amount = (opts[:sub_period_amount] || 1) 
        sub_period = (opts[:sub_period] || 'MONTHLY')
        is_purchase_on_no_active_sub = (opts[:iponas] || false)
        channel = (opts[:channel] || "MOBILE_WEB")

        payload = {
          :Amount => amount,
          :Category => category,
          :Description => desc,
          :MerchantTransactionId => merch_trans_id,
          :MerchantProductId => merch_prod_id,
          :MerchantSubscriptionIdList => merch_sub_id_list,
          :SubscriptionRecurrences => sub_recurrances,
          :MerchantPaymentRedirectUrl => redirect_uri,
          :SubscriptionPeriodAmount => sub_period_amount,
          :SubscriptionPeriod => sub_period,
          :IsPurchaseOnNoActiveSubscription => is_purchase_on_no_active_sub,
          :Channel => channel,
        }.to_json

        response = signPayload(payload)

        from_json = JSON.parse response

        parameters = "?Signature=#{from_json['Signature']}&SignedPaymentDetail=#{from_json['SignedDocument']}&clientid=#{@client_id}"

        @fqdn + Endpoints::SUBSCRIPTIONS + parameters
      end

      # Get the transaction status based on type and id
      #
      # @param transaction_type [String] the type of transaction being performed (see #TransactionType)
      # @param transaction_id [String] the id relative to the type
      # @return [RestClient::Response] a parsed response object 
      # @raise [ServiceException] raised when issue with transaction type
      def getTransaction(transaction_type, transaction_id)
        raise ServiceException, "Unknown Transaction type: " + transaction_type unless TransactionType.const_defined? transaction_type
        url = @fqdn + Endpoints::TRANSACTIONS + "/" + transaction_type + "/" + transaction_id

        @oauth_token = generateToken(:old_token => @oauth_token) if @oauth_token.expired?

        RestClient.get url, :Authorization => "Bearer #{@oauth_token.access_token}", :Content_Type => 'application/json', :Accept => 'application/json'
      end

      # Get the Subscription object specified
      #
      # @param subscription_type [String] method to obtain the Subscription (see #SubscriptionType)
      # @param subscription_id [String] the id relative to the type
      # @return [RestClient::Response] a parsed response object
      def getSubscription(subscription_type, subscription_id)
        raise ServiceException, "Unknown Subscription type: " + subscription_type unless SubscriptionType.const_defined? subscription_type
        url = @fqdn + Endpoints::SUBSCRIPTIONS + "/" + subscription_type + "/" + subscription_id

        @oauth_token = generateToken(:old_token => @oauth_token) if @oauth_token.expired?

        RestClient.get url, :Authorization => "Bearer #{@oauth_token.access_token}", :Content_Type => 'application/json', :Accept => 'application/json'
      end

      # Get the Subscription Details from a transaction
      #
      # @param consumer_id [String] the user id generated representing the subscriber
      # @param merchant_subscription_id [String] the subscription product id of the merchant
      # @return [RestClient::Response] a parsed response object
      def getSubscriptionDetails(consumer_id, merchant_subscription_id)
        url = @fqdn + Endpoints::SUBSCRIPTIONS + "/" + merchant_subscription_id + "/Detail/" + consumer_id

        @oauth_token = generateToken(:old_token => @oauth_token) if @oauth_token.expired?

        RestClient.get url, :Authorization => "Bearer #{@oauth_token.access_token}", :Content_Type => 'application/json', :Accept => 'application/json'
      end

      # Refund a previous transaction
      #
      # @param transaction_id [String] the id of the transaction to refund
      # @param transaction_state [String] the state of the transaction (see #TransactionState)
      # @param refund_reason [Integer] generalized code for refund (see #RefundCodes)
      # @param refund_reason_text [String] specific reason for refund
      # @param action [String] action being performed
      # @return [RestClient::Response] a parsed response object
      def refundTransaction(transaction_id, refund_reason, refund_reason_text, transaction_state=TransactionState::Refunded, action="refund")
        raise ServiceException, "Unknown Transaction state: " + transaction_state unless TransactionState.const_defined? transaction_state

        payload = {
          :TransactionOperationStatus => transaction_state,
          :RefundReasonCode => refund_reason,
          :RefundReasonText => refund_reason_text,
        }.to_json

        url = @fqdn + Endpoints::TRANSACTIONS + "/" + transaction_id + "?Action=#{action}"

        @oauth_token = generateToken(:old_token => @oauth_token) if @oauth_token.expired?

        RestClient.put url, payload, :Authorization => "Bearer #{@oauth_token.access_token}", :Content_Type => 'application/json', :Accept => 'application/json'
      end
      alias_method :refundSubscription, :refundTransaction

      # Cancel a transaction
      #
      # @see refundTransaction
      def cancelSubscription(transaction_id, refund_reason, refund_reason_text)
        refundTransaction(transaction_id, refund_reason, refund_reason_text, TransactionState::SubscriptionCancelled)
      end

      # Sign a payload with the ATT notary service
      #
      # @param payload [JSON] a json object that defines the payload to sign.
      def signPayload(payload)
        RestClient.post @fqdn + Endpoints::NOTARY_SIGNATURE, 
          payload, 
          :Accept => 'application/json', 
          :Content_Type => 'application/json', 
          'client_id' => @client_id, 
          'client_secret' => @client_secret
      end

    end
  end
end
