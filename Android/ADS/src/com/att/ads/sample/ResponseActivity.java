package com.att.ads.sample;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

/**
 * 
 * This UI Activity which displays the Ad response in JSON format.
 * 
 * @author ATT
 * 
 */
public class ResponseActivity extends Activity {

	/**
	 * Called when the activity is first created.
	 */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.response_tab_layout);

		TextView responsetextView = (TextView) findViewById(R.id.responseText);
		//responsetextView.setText(AdsApplication.getInstance().getAdView().getAdResponse());
		responsetextView.setText(AdsApplication.getInstance().getResponse());
	}

}
