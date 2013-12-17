package com.att.api.immn.service;

import org.json.JSONObject;

public final class SegmentationDetails {
    private final int segmentationMsgRefNumber;
    private final int totalNumberOfParts;
    private final int thisPartNumber;

    public SegmentationDetails(int refNumb, int numbParts, int partNumb) {
        this.segmentationMsgRefNumber = refNumb;
        this.totalNumberOfParts = numbParts;
        this.thisPartNumber = partNumb;
    }

    public int getSegmentationMsgRefNumber() {
        return segmentationMsgRefNumber;
    }

    public int getTotalNumberOfParts() {
        return totalNumberOfParts;
    }

    public int getThisPartNumber() {
        return thisPartNumber;
    }

    public static SegmentationDetails valueOf(JSONObject jobj) {
        int refNumb = jobj.getInt("segmentationMsgRefNumber");
        int numbParts = jobj.getInt("totalNumberOfParts");
        int partNumb = jobj.getInt("thisPartNumber");

        return new SegmentationDetails(refNumb, numbParts, partNumb);
    }
    
}
