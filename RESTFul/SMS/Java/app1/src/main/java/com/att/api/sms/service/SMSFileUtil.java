package com.att.api.sms.service;

import java.util.List;
import java.util.ArrayList;

import com.att.api.sms.model.SMSReceiveMsg;
import com.att.api.sms.model.SMSStatus;

public class SMSFileUtil {
    private static final Object lockObj = new Object();

    // For now store in memory
    // TODO: Store in a file
    private static List<SMSStatus> statuses = new ArrayList<SMSStatus>();
    private static List<SMSReceiveMsg> msgs = new ArrayList<SMSReceiveMsg>();

    public static SMSStatus[] getStatuses() {
        SMSStatus[] localStatuses = new SMSStatus[statuses.size()];
        synchronized(lockObj) {
            for (int i = 0; i < statuses.size(); ++i) {
                localStatuses[i] = statuses.get(i);
            }
        }
        return localStatuses;
    }

    public static void addStatus(SMSStatus status) {
        synchronized(lockObj) {
            // TODO: Move limit to config
            while (statuses.size() > 5) {
                statuses.remove(0);
            }
            statuses.add(status);
        }
    }

    public static SMSReceiveMsg[] getReceiveMsgs() {
        SMSReceiveMsg[] localMsgs = new SMSReceiveMsg[msgs.size()];
        synchronized(lockObj) {
            for (int i = 0; i < msgs.size(); ++i) {
                localMsgs[i] = msgs.get(i);
            }
        }
        return localMsgs;
    }

    public static void addSMSReceiveMsg(SMSReceiveMsg msg) {
        synchronized(lockObj) {
            // TODO: Move limit to config
            while (msgs.size() > 5) {
                msgs.remove(0);
            }

            msgs.add(msg);
        }
    }
}
