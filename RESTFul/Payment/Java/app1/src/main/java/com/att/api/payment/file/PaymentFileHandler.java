package com.att.api.payment.file;

import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.File;
import java.io.IOException;
import java.nio.channels.FileLock;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;
import org.json.JSONObject;

public class PaymentFileHandler {
    private File path = null;
    private int limit;

    public PaymentFileHandler(String pre, String post, int limit)
        throws IOException {
        this.path = File.createTempFile(pre, post);
        this.limit = limit;
    }

    public PaymentFileHandler(String pre, String post) throws IOException {
        this(pre, post, 5);
    }

    public void addTransactionEntry(JSONObject entry, String authCode)
        throws IOException{

        entry.put("authCode", authCode);
        List<JSONObject> entries = this.getTransactionEntrys();
        if (entries.size() >= this.limit) {
            entries.remove(0);
        }
        //check to make sure each entry is unique
        for (JSONObject json : entries){
            if (json.getString("authCode").equals(authCode))
                return;
        }
        entries.add(entry);

        FileOutputStream fOutputStream = null;
        FileLock fLock = null;

        try {
            fOutputStream = new FileOutputStream(this.path);
            fLock = fOutputStream.getChannel().lock();
            Properties props = new Properties();
            for (int i = 0; i < entries.size(); ++i) {
                JSONObject json = entries.get(i);
                props.setProperty("json" + i, json.toString());
            }
            props.store(fOutputStream, "Transaction Information");
        } catch (IOException ioe) {
            throw ioe;
        } finally {
            if (fLock != null) fLock.release();
            if (fOutputStream != null) fOutputStream.close();
        }
    }

    public List<JSONObject> getTransactionEntrys() 
        throws IOException {
        FileInputStream fInputStream = null;
        FileLock fLock = null;

        try {
            fInputStream = new FileInputStream(this.path);
            fLock = fInputStream.getChannel().lock(0L, Long.MAX_VALUE, true);
            Properties props = new Properties();
            props.load(fInputStream);

            List<JSONObject> entries = new ArrayList<JSONObject>();
            for (int i = 0; i < this.limit; ++i) {
                String json = props.getProperty("json"+i);
                if (json == null) {
                    break;
                }
                entries.add(new JSONObject(json));
            }
            return entries;
        } catch (IOException e) {
            throw e;
        }finally {
            if (fLock != null) fLock.release();
            if (fInputStream != null) fInputStream.close();
        }
    }
}
