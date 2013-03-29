package com.att.api.mim.model;

import org.json.*;
import java.util.ArrayList;
import java.text.ParseException;
import com.att.api.rest.APIResponse;
import org.apache.commons.codec.binary.Base64;
import org.apache.commons.collections.map.MultiValueMap;

public class MIMContentResponse {

  private APIResponse response;
  private String type;
  private String error = null;
  private final String TYPE_KEY = "Content-Type";

    private MIMContentResponse(){
    }

  /**
   * Create a response object based on string value
   *
   * @param message The response message
   */
  public MIMContentResponse(APIResponse response) throws ParseException{
    this.response = response;
    ArrayList<String> list = new ArrayList<String>(response.getAllHeaders().getCollection(TYPE_KEY));
    this.type = list.get(0);
  }

  public String toString() {
    return response.getResponseBody();
  }

  public static MIMContentResponse createErrorResponse(String error){
    MIMContentResponse temp = new MIMContentResponse();
    temp.setError(error);
    return temp;
  }

  public String getError() {
    return this.error;
  }

  public boolean hasError() {
    return this.getError() != null;
  }

  private MIMContentResponse setError(String error) {
    this.error = error;
    return this;
  }

  public String getImage(){
    Base64 codec = new Base64();
    return codec.encodeBase64String(this.response.getRawBody());
  }

  public String getType(){
    return this.type;
  }

  public String getContent(){
    return this.toString();
  }

  public void setResponse(APIResponse message) throws ParseException{
    this.response = message;
  }
}
