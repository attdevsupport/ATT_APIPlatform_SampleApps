package com.att.api.mms.service;

import java.util.List;
import java.util.ArrayList;

import com.att.api.mms.model.MMSStatus;

public class MMSFileUtil {
    private static final Object lockObj = new Object();

    // For now store in memory
    // TODO: Store in a file
    private static List<MMSStatus> statuses = new ArrayList<MMSStatus>();

    public static MMSStatus[] getStatuses() {
        MMSStatus[] localStatuses = new MMSStatus[statuses.size()];
        synchronized(lockObj) {
            for (int i = 0; i < statuses.size(); ++i) {
                localStatuses[i] = statuses.get(i);
            }
        }
        return localStatuses;
    }

    public static void addStatus(MMSStatus status) {
        synchronized(lockObj) {
            // TODO: Move limit to config
            while (statuses.size() >= 5) {
                statuses.remove(0);
            }
            statuses.add(status);
        }
    }
}
