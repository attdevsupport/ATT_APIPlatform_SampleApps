package com.att.api.mim.model;

import org.json.*;
import java.text.ParseException;

public class MIMResponse {

    private JSONObject json;
    private String error = null;
    private final String HEADERS_KEY = "Headers";
    private final String INDEXCURSOR_KEY = "IndexCursor";
    private final String MSG_HEADERS_KEY = "MessageHeadersList";


    private MIMResponse(){
    }

    /**
     * Create a response object based on string value
     *
     * @param message The response message
     */
    public MIMResponse(String message) throws ParseException{
        this(new JSONObject(message));
    }

    /**
     * Creates a response object based on json response.
     * @param json a response in JSON form
     */
    public MIMResponse(JSONObject json) throws IllegalArgumentException{
        if (json == null)
            throw new IllegalArgumentException("JSON Object must not be null!");
        this.json = json;
    }

    public String toString() {
        return json.toString();
    }

    public static MIMResponse createErrorResponse(String error){
        MIMResponse temp = new MIMResponse();
        temp.setError(error);
        return temp;
    }

    public String getError() {
        return this.error;
    }

    public boolean hasError() {
        return this.getError() != null;
    }

    private MIMResponse setError(String error) {
        this.error = error;
        return this;
    }

    public JSONArray getHeaders(){
        return this.getMessageHeaderList().getJSONArray(HEADERS_KEY);
    }

    public String getIndexCursor(){
        return this.getMessageHeaderList().getString(INDEXCURSOR_KEY);
    }

    public JSONObject getMessageHeaderList(){
        return this.json.getJSONObject(MSG_HEADERS_KEY);
    }

    public void setResponse(String message) throws ParseException{
        this.json = new JSONObject(message);
    }

    public void setResponse(JSONObject json){
        this.json = json;
    }
}
