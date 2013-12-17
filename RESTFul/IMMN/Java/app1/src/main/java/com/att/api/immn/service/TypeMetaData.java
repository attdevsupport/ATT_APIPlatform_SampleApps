package com.att.api.immn.service;

import org.json.JSONObject;

public final class TypeMetaData {

    private final Boolean isSegmented;
    private final SegmentationDetails details;
    private final String subject;

    public TypeMetaData(Boolean isSegmented, SegmentationDetails details,
            String subject) {
        
        this.isSegmented = isSegmented;
        this.details = details;
        this.subject = subject;
    }

    public Boolean getIsSegmented() {
        return isSegmented;
    }

    public SegmentationDetails getSegmentationDetails() {
        return details;
    }

    public String getSubject() {
        return subject;
    }

    public static TypeMetaData valueOf(JSONObject jobj) {
        Boolean isSegmented = null;
        SegmentationDetails details = null;
        String subject = null;

        if (jobj.has("isSegmented"))
            isSegmented = jobj.getBoolean("isSegmented");

        if (jobj.has("segmentationDetails")) {
            JSONObject jdetails = jobj.getJSONObject("segmentationDetails");
            details = SegmentationDetails.valueOf(jdetails);
        }

        if (jobj.has("subject"))
            subject = jobj.getString("subject");
            
        return new TypeMetaData(isSegmented, details, subject);
    }

}
