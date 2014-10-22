/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

/*
 * Copyright 2014 AT&T
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

package com.att.api.payment.model;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.xml.sax.SAXException;

import com.att.api.rest.RESTException;

/**
 * Used to hold a Notification sent to a Payment listener
 *
 * <p>
 * This class uses a factory method to construct a notification from XML
 * </p>
 *
 *
 * @author kh455g
 * @version 1.0
 * @since 1.0
 * @see <a href="https://developer.att.com/apis/payment/docs">Payment Documentation</a>
 */
final public class Notification {
    private String type;
    private String timestamp;
    private String effective;
    private String networkOperatorId;
    private String ownerId;
    private String purchaseDate;
    private String productId;
    private String instanceId;
    private String minId;
    private String oldMinId;
    private String sequenceNumber;
    private String purchaseActivityId;
    private String vendorPurchaseId;
    private String reasonCode;
    private String reasonMessage;

    /**
     * @return the type
     */
    public String getType() {
        return type;
    }

    /**
     * @return the timestamp
     */
    public String getTimestamp() {
        return timestamp;
    }

    /**
     * @return the effective
     */
    public String getEffective() {
        return effective;
    }

    /**
     * @return the networkOperatorId
     */
    public String getNetworkOperatorId() {
        return networkOperatorId;
    }

    /**
     * @return the ownerId
     */
    public String getOwnerId() {
        return ownerId;
    }

    /**
     * @return the purchaseDate
     */
    public String getPurchaseDate() {
        return purchaseDate;
    }

    /**
     * @return the productId
     */
    public String getProductId() {
        return productId;
    }

    /**
     * @return the instanceId
     */
    public String getInstanceId() {
        return instanceId;
    }

    /**
     * @return the minId
     */
    public String getMinId() {
        return minId;
    }

    /**
     * @return the oldMinId
     */
    public String getOldMinId() {
        return oldMinId;
    }

    /**
     * @return the sequenceNumber
     */
    public String getSequenceNumber() {
        return sequenceNumber;
    }

    /**
     * @return the purchaseActivityId
     */
    public String getPurchaseActivityId() {
        return purchaseActivityId;
    }

    /**
     * @return the vendorPurchaseId
     */
    public String getVendorPurchaseId() {
        return vendorPurchaseId;
    }

    /**
     * @return the reasonCode
     */
    public String getReasonCode() {
        return reasonCode;
    }

    /**
     * @return the reasonMessage
     */
    public String getReasonMessage() {
        return reasonMessage;
    }

    public Notification(String type, String timestamp, String effective,
            String networkOperatorId, String ownerId, String purchaseDate,
            String productId, String instanceId, String minId, String oldMinId,
            String sequenceNumber, String purchaseActivityId,
            String vendorPurchaseId, String reasonCode, String reasonMessage) {

        this.type = type;
        this.timestamp = timestamp;
        this.effective = effective;
        this.networkOperatorId = networkOperatorId;
        this.ownerId = ownerId;
        this.purchaseDate = purchaseDate;
        this.productId = productId;
        this.instanceId = instanceId;
        this.minId = minId;
        this.oldMinId = oldMinId;
        this.sequenceNumber = sequenceNumber;
        this.purchaseActivityId = purchaseActivityId;
        this.vendorPurchaseId = vendorPurchaseId;
        this.reasonCode = reasonCode;
        this.reasonMessage = reasonMessage;
    }

    public static Notification fromXml(InputStream is) throws RESTException {
        try {
            DocumentBuilder docBuilder = DocumentBuilderFactory.newInstance()
                .newDocumentBuilder();
            Document doc = docBuilder.parse(is);

            Element root = doc.getDocumentElement();

            NamedNodeMap attrib = root.getAttributes();

            String type = attrib.getNamedItem("type").getNodeValue();
            String timestamp = attrib.getNamedItem("timestamp").getNodeValue();

            String effective = null;
            Node eff = attrib.getNamedItem("effective");
            if (eff != null)
                effective = eff.getNodeValue();

            String networkOperatorId =
                root.getElementsByTagName("networkOperatorId")
                .item(0).getTextContent();

            String ownerId = 
                root.getElementsByTagName("ownerIdentifier")
                .item(0).getTextContent();

            String purchaseDate = 
                root.getElementsByTagName("purchaseDate")
                .item(0).getTextContent();

            String productId = 
                root.getElementsByTagName("productIdentifier")
                .item(0).getTextContent();
            
            String instanceId = 
                root.getElementsByTagName("instanceIdentifier")
                .item(0).getTextContent();

            String minId = 
                root.getElementsByTagName("minIdentifier")
                .item(0).getTextContent();

            String oldMinId = null;
            Node omi = root.getElementsByTagName("oldMinIdentifier").item(0);
            if (omi != null)
                oldMinId = omi.getTextContent();

            String sequenceNumber = 
                root.getElementsByTagName("sequenceNumber")
                .item(0).getTextContent();

            String purchaseActivityId = 
                root.getElementsByTagName("purchaseActivityIdentifier")
                .item(0).getTextContent();

            String vendorPurchaseId = 
                root.getElementsByTagName("vendorPurchaseIdentifier")
                .item(0).getTextContent();

            String reasonCode = 
                root.getElementsByTagName("reasonCode")
                .item(0).getTextContent();

            String reasonMessage = 
                root.getElementsByTagName("reasonMessage")
                .item(0).getTextContent();

            return new Notification(type, timestamp, effective,
                    networkOperatorId, ownerId, purchaseDate, productId,
                    instanceId, minId, oldMinId, sequenceNumber,
                    purchaseActivityId, vendorPurchaseId, reasonCode,
                    reasonMessage);

        } catch (IOException ex) {
            throw new RESTException(ex);
        } catch (SAXException ex) {
            throw new RESTException(ex);
        } catch (ParserConfigurationException ex) {
            throw new RESTException(ex);
        }
    }

    /**
     * Construct a notification from an xml byte array
     *
     * @param xml an xml byte array representation of a notification
     * @return Notification
     * @throws RESTException
     */
    public static Notification fromXml(byte[] xml) throws RESTException {
        InputStream is = new ByteArrayInputStream(xml);
        return (fromXml(is));
    }
    /**
     * Construct a notification from an xml string
     *
     * @param xml an xml string representation of a notification
     * @return Notification
     * @throws RESTException
     */
    public static Notification fromXml(String xml) throws RESTException {
        try {
            InputStream is = new ByteArrayInputStream(xml.getBytes("ISO-8859-1"));
            return(fromXml(is));
        } catch (UnsupportedEncodingException ex) {
            throw new RESTException(ex);
        }
    }
}
