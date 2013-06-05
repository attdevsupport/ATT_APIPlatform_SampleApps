package com.att.api.payment.model;

import java.util.ArrayList;
import java.util.List;

public class NotificationPool {
    private static NotificationPool instance = null;
    private static final int IGNORE_MAX = -1;

    private int max;
    private List<Notification> notifications;
    private final Object note_lock = new Object();

    private NotificationPool(){
        this.notifications = new ArrayList<Notification>();
        this.max = IGNORE_MAX;
    }

    public synchronized void setMaxNotifications(int max){
        this.max = max;
    }

    /**
     * Obtain the instance of notifications
     *
     * @return shared instance of NotificationPool
     */
    public static NotificationPool getInstance(){
        if (instance == null){
            synchronized(NotificationPool.class){
                if (instance == null)
                    instance = new NotificationPool();
            }
        }
        return instance;
    }

    /**
     * @return the notifications
     */
    public List<Notification> getNotifications() {
        synchronized(this.note_lock){
            return this.notifications;
        }
    }

    /**
     * @param notifications the notifications to set
     */
    public void setNotifications(List<Notification> notifications) {
        synchronized(this.note_lock){
            this.notifications = notifications;
        }
    }

    /**
     * Add a list of notifications to the current list.
     * The list will remove old notifications if greater than max.
     *
     * @param update the list of new notifications
     */
    public void updateNotifications(List<Notification> update) {
        synchronized (this.note_lock) {
            this.notifications.addAll(update);
            if (this.max > IGNORE_MAX){
                while (this.notifications.size() > 0 && this.notifications.size() > this.max)
                    this.notifications.remove(0);
            }
        }
    }
}
