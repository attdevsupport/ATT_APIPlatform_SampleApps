package com.att.api.sms.service;

import java.util.ArrayList;
import java.util.List;

import com.att.api.sms.model.SMSReceiveMessage;
import com.att.api.sms.model.SMSReceiveStatus;

public class SMSFileUtil {
    private static final Object statusLockObj = new Object();
    private static final Object msgLockObj = new Object();

    // For now store in memory
    // TODO: Store in a file
    private static List<SMSReceiveStatus> statuses = new ArrayList<SMSReceiveStatus>();
    private static List<SMSReceiveMessage> msgs = new ArrayList<SMSReceiveMessage>();

    public static SMSReceiveStatus[] getStatuses() {
        SMSReceiveStatus[] localStatuses = new SMSReceiveStatus[statuses.size()];
        synchronized(statusLockObj) {
            for (int i = 0; i < statuses.size(); ++i) {
                localStatuses[i] = statuses.get(i);
            }
        }
        return localStatuses;
    }

    public static void addStatus(SMSReceiveStatus status, int limit) {
        synchronized(statusLockObj) {
            while (statuses.size() > limit) {
                statuses.remove(0);
            }
            statuses.add(status);
        }
    }

    public static SMSReceiveMessage[] getMsgs() {
        SMSReceiveMessage[] localMsgs = new SMSReceiveMessage[msgs.size()];
        synchronized(msgLockObj) {
            for (int i = 0; i < msgs.size(); ++i) {
                localMsgs[i] = msgs.get(i);
            }
        }
        return localMsgs;
    }

    public static void addSMSMsg(SMSReceiveMessage msg, int limit) {
        synchronized(msgLockObj) {
            while (msgs.size() > limit) {
                msgs.remove(0);
            }

            msgs.add(msg);
        }
    }
}
