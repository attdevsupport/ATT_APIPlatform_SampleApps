package com.att.api.immn.model;

public class IMMNResponse {

	private String id;
    private String error = null;

	private IMMNResponse(){
	}

	public IMMNResponse(String id){
		this.id = id;
	}

	public String getId() {
		return this.id;
	}

    public void setId(String id) {
        this.id = id;
    }

    public String getError(){
        return this.error;
    }

    public void setError(String error){
        this.error = error;
    }

    public boolean hasError(){
        return this.error != null;
    }
    
    public static IMMNResponse createErrorResponse(String error){
        IMMNResponse temp = new IMMNResponse();
        temp.setError(error);
        return temp;
    }
}
