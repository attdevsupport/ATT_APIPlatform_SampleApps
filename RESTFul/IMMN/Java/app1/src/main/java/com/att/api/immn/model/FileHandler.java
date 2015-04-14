package com.att.api.immn.model;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.channels.FileLock;
import java.text.ParseException;
import java.util.LinkedList;
import java.util.List;
import java.util.Properties;

import org.json.JSONObject;

public class FileHandler {
    private File path = null;
    private int limit;

    public FileHandler(String pre, String post, int limit)
        throws IOException {
        this.path = File.createTempFile(pre, post);
        this.limit = limit;
    }

    public FileHandler(String pre, String post) throws IOException {
        this(pre, post, 100);
    }

    public FileHandler(String path, int limit) {
        this.path = new File(path);
        this.limit = limit;
    }

    public void addObj(JSONObject jobj) throws IOException, ParseException {
        addObj(jobj, "Storing information");
    }

    public void addObj(JSONObject jobj, String comment) 
            throws IOException, ParseException {
        List<JSONObject> entries = loadObjs();
        if (entries.size() >= this.limit) {
            entries.remove(0);
        }
        entries.add(jobj);

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
            props.store(fOutputStream, comment);
        } finally {
            if (fLock != null) fLock.release();
            if (fOutputStream != null) fOutputStream.close();
        }
    }

    public List<JSONObject> loadObjs() throws IOException {
        FileInputStream fInputStream = null;
        FileLock fLock = null;

        try {
            fInputStream = new FileInputStream(this.path);
            fLock = fInputStream.getChannel().lock(0L, Long.MAX_VALUE, true);
            Properties props = new Properties();
            props.load(fInputStream);

            List<JSONObject> entries = new LinkedList<JSONObject>();
            for (int i = 0; i < this.limit; ++i) {
                String json = props.getProperty("json"+i);
                if (json == null) {
                    break;
                }
                entries.add(new JSONObject(json));
            }
            return entries;
        } finally {
            if (fLock != null) fLock.release();
            if (fInputStream != null) fInputStream.close();
        }
    }
}
// vim: set ts=8 sw=4 sts=4 tw=79 ft=java et :
