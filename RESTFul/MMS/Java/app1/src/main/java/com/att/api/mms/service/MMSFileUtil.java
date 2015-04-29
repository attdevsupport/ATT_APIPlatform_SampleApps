package com.att.api.mms.service;

import java.util.List;
import java.util.ArrayList;

import com.att.api.mms.model.MMSDeliveryInfo;

public class MMSFileUtil {
    private static final Object lockObj = new Object();

    // For now store in memory
    // TODO: Store in a file
    private static List<MMSDeliveryInfo> statuses = new ArrayList<MMSDeliveryInfo>();

    public static MMSDeliveryInfo[] getStatuses() {
        MMSDeliveryInfo[] localStatuses = new MMSDeliveryInfo[statuses.size()];
        synchronized(lockObj) {
            for (int i = 0; i < statuses.size(); ++i) {
                localStatuses[i] = statuses.get(i);
            }
        }
        return localStatuses;
    }

    public static void addStatus(MMSDeliveryInfo status) {
        synchronized(lockObj) {
            // TODO: Move limit to config
            while (statuses.size() >= 5) {
                statuses.remove(0);
            }
            statuses.add(status);
        }
    }
}
