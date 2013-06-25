package com.att.api.payment.model;

public class Notary {
    private String signature;
    private String signedDocument;
    private String payload;

    public Notary(String payload, String signedDoc, String signature) {
        this.signedDocument = signedDoc;
        this.signature = signature;
        this.payload = payload;
    }

    public String getSignedDocument() {
        return this.signedDocument;
    }

    public String getSignature() {
        return this.signature;
    }

    public String getPayload(){
        return this.payload;
    }
}
