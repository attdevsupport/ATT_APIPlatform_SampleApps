/*
 * Copyright 2015 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.att.api.webhooks.service;

import org.json.JSONArray;
import org.json.JSONObject;
import org.json.JSONException;

import com.att.api.oauth.OAuthToken;
import com.att.api.rest.APIResponse;
import com.att.api.rest.RESTClient;
import com.att.api.rest.RESTException;
import com.att.api.service.APIService;

import org.apache.commons.codec.EncoderException;
import org.apache.commons.codec.net.URLCodec;

/**
 * Used to interact with version 1 of the Webhooks API.
 *
 */
public class WebhooksService extends APIService {
    protected final URLCodec codec;

    /**
     * Creates a WebhooksService object.
     *
     * @param fqdn fully qualified domain name to use for sending requests
     * @param token OAuth token to use for authorization
     */
    public WebhooksService(String fqdn, OAuthToken token) {
        super(fqdn, token);
        codec = new URLCodec();
    }

    public CreateChannelResponse createNotificationChannel(
            final CreateChannelArgs args) throws RESTException {
        final String endpoint = getFQDN() + "/notification/v1/channels";

        final JSONObject jchannel = new JSONObject();
        jchannel.put("serviceName", args.getServiceName());
        jchannel.put("notificationContentType", args.getContentType());
        if (args.getVersion() != null)
            jchannel.put("notificationVersion", args.getVersion());
        final JSONObject jobj = new JSONObject();
        jobj.put("channel", jchannel);

        final APIResponse response = new RESTClient(endpoint)
                .addAuthorizationHeader(getToken())
                .setHeader("Accept", "application/json")
                .setHeader("Content-Type", "application/json")
                .setSuccessCode(201).httpPost(jobj.toString());

        final String location = response.getHeader("location");
        final String systemTransId = response
                .getHeader("x-systemTransactionId");

        try {
            JSONObject jobjresponse = new JSONObject(response.getResponseBody());
            JSONObject jchannelresponse = jobjresponse.getJSONObject("channel");
            final String channelId = jchannelresponse.getString("channelId");
            Integer maxEvts = null;
            if (jchannelresponse.has("maxEventsPerNotification")) {
                maxEvts = jchannelresponse
                        .getInt("maxEventsPerNotification");
            }

            final CreateChannelResponse.Channel channel = new CreateChannelResponse.Channel(
                    channelId, maxEvts);
            return new CreateChannelResponse(location, systemTransId, channel);
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }

    public GetChannelResponse getNotificationChannelDetails(String channelId)
            throws RESTException {
        try {
            channelId = this.codec.encode(channelId);
        } catch (EncoderException e) {
            throw new RESTException(e);
        }
        final String endpoint
            = getFQDN() + "/notification/v1/channels/" + channelId;

        final APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .setHeader("Accept", "application/json")
            .setHeader("Content-Type", "application/json")
            .setSuccessCode(200)
            .httpGet();

        final String systemTransId = response.getHeader("x-systemTransactionId");
        try {
            JSONObject jobjresponse = new JSONObject(response.getResponseBody());
            JSONObject jchannelresponse = jobjresponse.getJSONObject("channel");
            final String channelIdResponse
                = jchannelresponse.getString("channelId");
            Integer maxEvts = null;
            if (jchannelresponse.has("maxEventsPerNotification")) {
                maxEvts = jchannelresponse.getInt("maxEventsPerNotification");
            }
            final String serviceName = jchannelresponse.getString("serviceName");
            String channelType = null;
            if (jchannelresponse.has("channelType")) {
                channelType = jchannelresponse.getString("channelType");
            }
            final String notificationContentType
                = jchannelresponse.getString("notificationContentType");
            final double notificationVersion
                = jchannelresponse.getDouble("notificationVersion");
            final GetChannelResponse.Channel channel
                = new GetChannelResponse.Channel(
                    channelIdResponse, maxEvts, serviceName, channelType,
                    notificationContentType, notificationVersion
                );
            return new GetChannelResponse(systemTransId, channel);
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }

    public String deleteNotificationChannel(String channelId)
            throws RESTException {
        try {
            channelId = this.codec.encode(channelId);
        } catch (EncoderException e) {
            throw new RESTException(e);
        }
        final String endpoint
            = getFQDN() + "/notification/v1/channels/" + channelId;

        final APIResponse response = new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .setSuccessCode(204)
            .httpDelete();

        return response.getHeader("x-systemTransactionId");
    }

    public CreateSubscriptionResponse createNotificationSubscription(
        final CreateSubscriptionArgs args
    ) throws RESTException {
        String channelId = args.getChannelId();
        try {
            channelId = this.codec.encode(channelId);
        } catch (EncoderException e) {
            throw new RESTException(e);
        }
        final String endpoint = getFQDN() + "/notification/v1/channels/"
            + channelId + "/subscriptions";

        final JSONObject jsubscription = new JSONObject()
            .put("events", args.getEvents());
        if (args.getCallbackData() != null) {
            jsubscription.put("callbackData", args.getCallbackData());
        }
        if (args.getExpiresIn() != null) {
            jsubscription.put("expiresIn", args.getExpiresIn());
        }

        final JSONObject jobj = new JSONObject()
            .put("subscription", jsubscription);

        final APIResponse response = new RESTClient(endpoint)
                .addAuthorizationHeader(getToken())
                .setHeader("Accept", "application/json")
                .setHeader("Content-Type", "application/json")
                .setSuccessCode(201)
                .httpPost(jobj.toString());

        return CreateSubscriptionResponse.valueOf(response);
    }

    public GetSubscriptionResponse getNotificationSubscriptionDetails(
        String channelId, String subscriptionId
    ) throws RESTException {
        try {
            channelId = this.codec.encode(channelId);
            subscriptionId = this.codec.encode(subscriptionId);
        } catch (EncoderException e) {
            throw new RESTException(e);
        }
        final String endpoint = getFQDN() + "/notification/v1/channels/"
            + channelId + "/subscriptions/" + subscriptionId;

        final APIResponse response = new RESTClient(endpoint)
                .addAuthorizationHeader(getToken())
                .setHeader("Accept", "application/json")
                .setSuccessCode(200)
                .httpGet();

        final String systemTransId = response
                .getHeader("x-systemTransactionId");
        final String contentType = response.getHeader("content-type");
    
        try {
            final JSONObject jobjresponse 
                = new JSONObject(response.getResponseBody());
            final JSONObject jsubscriptionresponse 
                = jobjresponse.getJSONObject("subscription");
            final String subscriptionIdResponse 
                = jsubscriptionresponse.getString("subscriptionId");
            final Integer expiresInResponse
                = jsubscriptionresponse.getInt("expiresIn");
            final JSONArray jevents =
                jsubscriptionresponse.getJSONArray("eventFilters");

            final String[] subscriptionEventsResponse 
                = new String[jevents.length()];
            for (int i = 0; i < jevents.length(); i++) {
                subscriptionEventsResponse[i] = jevents.getString(i);
            }
            final String callbackDataResponse
                = jsubscriptionresponse.getString("callbackData");

            final GetSubscriptionResponse.Subscription subscription
                = new GetSubscriptionResponse.Subscription(
                    subscriptionIdResponse, expiresInResponse,
                    subscriptionEventsResponse, callbackDataResponse
                );
                    
            return new GetSubscriptionResponse(
                contentType, systemTransId, subscription
            );
        } catch (JSONException pe) {
            throw new RESTException(pe);
        }
    }

    public UpdateSubscriptionResponse updateNotificationSubscription(
        final UpdateSubscriptionArgs args
    ) throws RESTException {
        String channelId = args.getChannelId();
        String subscriptionId = args.getSubscriptionId();
        try {
            channelId = this.codec.encode(channelId);
            subscriptionId = this.codec.encode(subscriptionId);
        } catch (EncoderException e) {
            throw new RESTException(e);
        }
        final String endpoint = getFQDN() + "/notification/v1/channels/"
            + channelId + "/subscriptions/" + subscriptionId;

        final JSONObject jsubscription = new JSONObject()
            .put("events", args.getEvents());
        if (args.getCallbackData() != null) {
            jsubscription.put("callbackData", args.getCallbackData());
        }
        if (args.getExpiresIn() != null) {
            jsubscription.put("expiresIn", args.getExpiresIn());
        }

        final JSONObject jobj = new JSONObject()
            .put("subscription", jsubscription);

        final APIResponse response = new RESTClient(endpoint)
                .addAuthorizationHeader(getToken())
                .setHeader("Accept", "application/json")
                .setHeader("Content-Type", "application/json")
                .setSuccessCode(200)
                .httpPut(jobj.toString());

        return UpdateSubscriptionResponse.valueOf(response);
    }

    /**
     * Returns x-systemTransactionId or null if successful
     *
     * @param channelId Channel id in use
     * @param subscriptionId Subscription id in use
     * @return String x-systemTransactionId or null
     *
     * @throws RESTException error happened making the request
     */
    public String deleteNotificationSubscription(
        String channelId, String subscriptionId
    ) throws RESTException {
        try {
            channelId = this.codec.encode(channelId);
            subscriptionId = this.codec.encode(subscriptionId);
        } catch (EncoderException e) {
            throw new RESTException(e);
        }
        final String endpoint = getFQDN() + "/notification/v1/channels/"
            + channelId + "/subscriptions/" + subscriptionId;
        return new RESTClient(endpoint)
            .addAuthorizationHeader(getToken())
            .setSuccessCode(204)
            .httpDelete()
            .getHeader("x-systemTransactionId");
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
