package com.att.ads.sample;

import android.app.TabActivity;
import android.content.Intent;
import android.os.Bundle;
import android.widget.TabHost;

/**
 * 
 * Main activity of the Sample SDK. It is designed as a TabActivity View. It
 * contains 3 tabs. First Tab contains an Ad Activity which displays the Ads
 * based on the categories. Second tab is used for settings configurations.
 * Third Tab is for displaying the Ad Responses in JSON format.
 * 
 * @author ATT
 * 
 */
public class AdsMainActivity extends TabActivity {

	/**
	 * Called when the activity is first created.
	 */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.tab_layout);

		final TabHost tabHost = getTabHost();

		// Create the tabs
		tabHost.addTab(tabHost.newTabSpec("tab1").setIndicator("AdView")
				.setContent(new Intent(this, AdsViewActivity.class)));

		tabHost.addTab(tabHost.newTabSpec("tab2")
				.setIndicator("Settings", getResources().getDrawable(R.drawable.ads_tab_settings))
				.setContent(new Intent(this, SettingsViewActivity.class)));

		// This tab sets the intent flag-FLAG_ACTIVITY_CLEAR_TOP so that it is
		// recreated each time the tab is clicked.
		tabHost.addTab(tabHost
				.newTabSpec("tab3")
				.setIndicator("Response", getResources().getDrawable(R.drawable.ads_tab_results))
				.setContent(
						new Intent(this, ResponseActivity.class)
								.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP)));

	}

}