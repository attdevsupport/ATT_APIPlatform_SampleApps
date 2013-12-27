package com.att.ads.sample;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.res.Configuration;
import android.graphics.Color;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.att.ads.ATTAdView;
import com.att.ads.ATTAdViewError;
import com.att.ads.listeners.ATTAdViewListener;

/**
 *
 * Displays Ad View based on selected category and settings. Create ATTAdView
 * object by passing the app key , secret key, UDID and Category. Finally add your view
 * object to adFrameLayout.
 *
 * @author ATT
 *
 */
public class AdsViewActivity extends Activity implements ATTAdViewListener {

	private static final String TAG = "AdsViewActivity";
	private TextView selectCategoryView = null;
	private Button btnGetAd = null;
	private String[] categoryItems = null;
	private ATTAdView attAdView;
	private LinearLayout adFrameLayout;
	private String appKey = null;
	private String secret = null;

	/**
	 * stores the selected category item value
	 */
	private String selectedCategory = null;

	/**
	 * Called when the activity is first created.
	 */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.adsview_tab_layout);

		categoryItems = getResources().getStringArray(R.array.categoryTypes);
		selectedCategory = categoryItems[0];
		adFrameLayout = (LinearLayout) findViewById(R.id.frameAdContent);

		btnGetAd = (Button) findViewById(R.id.btnGetAd);
		btnGetAd.setOnClickListener(onClickListener);

		selectCategoryView = (TextView) findViewById(R.id.selectCategory);
		selectCategoryView.setText(selectedCategory);
		selectCategoryView.setOnClickListener(onClickListener);

		/*
		 * NOTE:
		 * We are using the obfuscated App Key and App Secret in the below declaration.
		 * These will be unobfuscated while calling the ATTAdView constructor.
		 * you can use your own encryption/decreption algorithm to manage data security.
		 *
		 * Below commented getEncryptedAppAndSecret method is very useful to
		 * generate encrypted app and secret key values using AES algorithm.
		 * This is one of the powerful and secure store data on device
		 * 1.First you should uncomment the method.
		 * 2.Go to below of the class for this  method implementation.
		 * 3.provide your original app key and secret key.
		 * 4.Run this sample app on device/Emulator and find debug log with this keys (en_appKey,en_secret).
		 * 5. copy those values into string.xml like below
		 * 		<string name="appKey">4568b859645847b01b6d0937d63c5083e697bd454848e104fe6c89649cae89d32a8a60b697a03b75312d7e593ba8f124329</string>
		 * 		<string name="secret">zxb3fd2555259b664e35f445243cb1fd24a228ace99b8cd6d3e264618e549c6000</string>
		 * 6.Decrypt an app and secret key values like try/catch block.
		 * 7.Remove getEncryptedAppAndSecret method declaration and implementation in  this class.
		 */

		// getEncryptedAppAndSecret();


		/*try {
			appKey = AdsEncryptDecrypt.getDecryptedValue(getResources().getString(R.string.appKey),
					AdsEncryptDecrypt.getSecretKeySpec("app_key"));
			secret = AdsEncryptDecrypt.getDecryptedValue(getResources().getString(R.string.secret),
					AdsEncryptDecrypt.getSecretKeySpec("secret_key"));
		} catch (Exception e) {
			Log.e(TAG, "Eror :" + e.fillInStackTrace());
		}*/
		appKey = "UPDATE_APP_KEY_HERE";
		secret = "UPDATE_SECRET_KEY_HERE";
		
		// Category always in lower case
		if(null == appKey || null == secret){
			Log.e(TAG, "Eror :Either App Key or secret or both are null.");
			return;
		}
		attAdView = new ATTAdView(this, appKey, secret, getResources().getString(R.string.udid),
				selectedCategory);
		//Ad reload time set to 60 seconds.
		attAdView.setAdReloadPeriod(60);
		//This method will dynamically read the device width & height and set the accordingly.
		setAdLayoutParams();
		//Simply AppDeveloper can also set the layout parameters as below instead the above.
		//attAdView.setLayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FILL_PARENT, 100));
		adFrameLayout.addView(attAdView);
		attAdView.setAdViewListener(this);
		//Setting the Ad View background to transparent.
		attAdView.setBackgroundColor(Color.TRANSPARENT);
		attAdView.initOrRefresh();
	}


	/**
	 * on click event listener for views
	 */
	private View.OnClickListener onClickListener = new OnClickListener() {

		@Override
		public void onClick(View view) {
			if (view.getId() == R.id.selectCategory) {
				getCategory().show();
			} else if (view.getId() == R.id.btnGetAd) {
				onSettingsChange();
			}
		}
	};

	private void onSettingsChange() {
		try {
			setToNullifyParams();
			// Category always in lower case
			attAdView.setCategory(selectedCategory);
			// setting tab parameters
			if (AdsApplication.getInstance().getKeywords() != null
					&& AdsApplication.getInstance().getKeywords().length() > 0) {
				attAdView.setKeywords(AdsApplication.getInstance().getKeywords());
			}
			if (AdsApplication.getInstance().getZip() > 0) {
				attAdView.setZipCode(AdsApplication.getInstance().getZip());
			}
			attAdView.setPremium(AdsApplication.getInstance().getPremium());
			if (AdsApplication.getInstance().getAgeGroup() != null
					&& AdsApplication.getInstance().getAgeGroup().length() > 0) {
				attAdView.setAgeGroup(AdsApplication.getInstance().getAgeGroup());
			}
			if (AdsApplication.getInstance().getMaxHeight() > 0) {
				attAdView.setMaxHeight(AdsApplication.getInstance().getMaxHeight());
			}
			if (AdsApplication.getInstance().getMaxWidth() > 0) {
				attAdView.setMaxWidth(AdsApplication.getInstance().getMaxWidth());
			}
			if (AdsApplication.getInstance().getLatitude() != 0.0) {
				attAdView.setLatitude(AdsApplication.getInstance().getLatitude());
			}
			if (AdsApplication.getInstance().getLongitude() != 0.0) {
				attAdView.setLongitude(AdsApplication.getInstance().getLongitude());
			}
			attAdView.initOrRefresh();

			if(AdsApplication.getInstance().getMaxWidth() >= 300 &&
					AdsApplication.getInstance().getMaxHeight() >= 250){
				openFullScreenAd();
			}else {
				if (attAdView.getParent() != null) {
					((ViewGroup)attAdView.getParent()).removeAllViews();
				}
				setAdLayoutParams();
				//attAdView.setLayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FILL_PARENT, 100));
				adFrameLayout.addView(attAdView);
			}

		} catch (Exception e) {
			Log.e(TAG, "Eror :" + e.fillInStackTrace());
		}
	}
	/**
	 * Returns category.
	 *
	 * @return
	 */
	private Dialog getCategory() {

		AlertDialog.Builder repeatBuilder = new AlertDialog.Builder(this);
		repeatBuilder.setTitle("Choose Category :");
		repeatBuilder.setItems(categoryItems, new DialogInterface.OnClickListener() {

			public void onClick(DialogInterface dialog, int item) {
				selectedCategory = categoryItems[item];
				selectCategoryView.setText(selectedCategory);
				dialog.cancel();
			}

		});
		return repeatBuilder.create();
	}


	/**
	 * Nullifying the parameters
	 */
	protected void setToNullifyParams() {
		attAdView.setCategory(null);
		attAdView.setKeywords(null);
		attAdView.setZipCode(null);
		attAdView.setAgeGroup(null);
		attAdView.setMaxHeight(null);
		attAdView.setMaxWidth(null);
		attAdView.setLatitude(null);
		attAdView.setLongitude(null);
	}

	/**
	 * Refresh the ad-view based on device/tablet.
	 */
	@Override
	public void onConfigurationChanged(Configuration newConfig) {
		super.onConfigurationChanged(newConfig);
		setAdLayoutParams();
		attAdView.initOrRefresh();
	}

	/**
	 * Set the layout parameters associated with ad view. These supply
	 * parameters to the parent of this view specifying how it should be
	 * arranged.
	 */
	private void setAdLayoutParams() {
		Log.d(TAG, "set Ad Layout params");
		WindowManager windowManager = (WindowManager) getSystemService(Context.WINDOW_SERVICE);
		DisplayMetrics metrics = new DisplayMetrics();
		windowManager.getDefaultDisplay().getMetrics(metrics);
		int height = 50;

		Log.d(TAG, "Height: "+metrics.heightPixels);
		Log.d(TAG, "Width: "+metrics.widthPixels);

		int maxSize = metrics.heightPixels;
		if (maxSize < metrics.widthPixels) {
			maxSize = metrics.widthPixels;
		}

		if (maxSize <= 480) {
			height = 50;
		} else if ((maxSize > 480) && (maxSize <= 800)) {
			height = 100;
		} else if (maxSize > 800) {
			height = 120;
		}


		//Ad size - Max height and Max width that the component can allow the ad image
		//Note: The minSize and maxSize parameters can be used to tune the ads
		//received from the server based on the requirements of the device,
		//but it is not guaranteed that the received ads will be of that size.
		attAdView.setLayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FILL_PARENT, height+(height/2)));

		/*ViewGroup.LayoutParams lp = attAdView.getLayoutParams();
		if (lp == null) {
			lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FILL_PARENT, height);
			attAdView.setLayoutParams(lp);
		}*/

		// Max size can be useful, but if you don't have ads large enough for
		// all devices,
		// it can result in no ad being shown, so use it sparingly.
		//attAdView.setMaxWidth(metrics.widthPixels);
		//attAdView.setMaxHeight(height);

		attAdView.requestLayout();
	}
	/**
	 * Success call back method. Method triggers when the ad is received
	 * successfully and rendered. Generally this is the place holder for
	 * application developer to do the logging or debugging purposes.
	 */
	@Override
	public void onSuccess(String adViewResponse) {
		Log.d(TAG, "onSuccess()");
		AdsApplication.getInstance().setResponse(adViewResponse);
	}
	/**
	 * Error call back method. Method triggers when the ad is failed to receive
	 * valid response. Generally this is the place holder for application
	 * developer to do the error handling is own way.
	 */
	@Override
	public void onError(ATTAdViewError error) {
		Log.d(TAG, "onError()");
		StringBuffer res = new StringBuffer();
		res.append(error.getType());
		res.append(": ");
		res.append(error.getMessage());
		Exception e = error.getException();
		if (null != e) {
			res.append("\n Exception :\n ");
			res.append(e);
		}
		AdsApplication.getInstance().setResponse(res.toString());
	}

	@Override
	protected void onPause() {
		super.onPause();
		Log.d(TAG, "onPause()");
		if (null != attAdView) {
			// Stops the Reload timer and all listeners
			attAdView.stopRefresh();
		}
	}

	@Override
	protected void onResume() {
		super.onResume();
		Log.d(TAG, "onResume()");
		if (null != attAdView) {
			// Starts the Reload timer and all listeners
			attAdView.startRefresh();
			//Following is only for sample application.
			//Automatically calls the ATTAdView with out clicking of GetAd button.
			if(AdsApplication.getInstance().isChangeInSettings()){
				AdsApplication.getInstance().setChangeInSettings(false);
				onSettingsChange();
			}
		}
	}

	/**
	 * This method will generate encrypted app and secret key values using AES
	 * algorithm.
	 */
	@SuppressWarnings("unused")
	private void getEncryptedAppAndSecret() {
		/*
		 *   String appKey = "provide your appKey ";
		 *   String secret = "provide your secret key";
		 *   For example like below and those are dummy values.
		 */

		try {
			String appKey = "4534545e9a5b7381d9090e70cdd118046c462b2234234";
			String secret = "234534780fbad343dd30bcdfgf";
			String en_appKey = AdsEncryptDecrypt.getEncryptedValue(appKey,
					AdsEncryptDecrypt.getSecretKeySpec("app_key"));
			String en_secret = AdsEncryptDecrypt.getEncryptedValue(secret,
					AdsEncryptDecrypt.getSecretKeySpec("secret_key"));
			Log.d(TAG, "en_appKey:" + en_appKey + ",en_secret:" + en_secret);
		} catch (Exception e) {
			Log.e(TAG, "Error:" + e.fillInStackTrace());
		}
	}

	private void openFullScreenAd() {

		//ImageButton closeButton = null;
		Button closeButton = null;

		//show dialog
		final Dialog dialog;

		dialog = new Dialog(this, android.R.style.Theme_NoTitleBar);

		if (attAdView.getParent() != null) {
			((ViewGroup)attAdView.getParent()).removeAllViews();
		}

		final RelativeLayout mainLayout = new RelativeLayout(this);
		//mainLayout.setBackgroundColor(Color.TRANSPARENT);
		mainLayout.setLayoutParams(new ViewGroup.LayoutParams(
				ViewGroup.LayoutParams.FILL_PARENT, ViewGroup.LayoutParams.FILL_PARENT));
		RelativeLayout.LayoutParams adLayoutParams = new RelativeLayout.LayoutParams(
				ViewGroup.LayoutParams.FILL_PARENT, ViewGroup.LayoutParams.FILL_PARENT);
		adLayoutParams.addRule(RelativeLayout.CENTER_HORIZONTAL, RelativeLayout.TRUE);
		adLayoutParams.addRule(RelativeLayout.CENTER_VERTICAL, RelativeLayout.TRUE);
		attAdView.setLayoutParams(adLayoutParams);
		mainLayout.addView(attAdView);

		if(closeButton == null) {
			//Below code base is to add the close image button at top right corner.
			/*closeButton = new ImageButton(this);
			closeButton.setImageResource(R.drawable.popup_close_btn);
			RelativeLayout.LayoutParams closeLayoutParams = new RelativeLayout.LayoutParams(
					RelativeLayout.LayoutParams.WRAP_CONTENT, RelativeLayout.LayoutParams.WRAP_CONTENT);
			closeLayoutParams.addRule(RelativeLayout.ALIGN_PARENT_TOP, RelativeLayout.TRUE);
			closeLayoutParams.addRule(RelativeLayout.ALIGN_PARENT_RIGHT, RelativeLayout.TRUE);
			closeButton.setLayoutParams(closeLayoutParams);*/

			//Below code base is to add the close button at bottom.
			closeButton = new Button(this);
			closeButton.setText("Close");
			RelativeLayout.LayoutParams closeLayoutParams = new RelativeLayout.LayoutParams(
					RelativeLayout.LayoutParams.WRAP_CONTENT, RelativeLayout.LayoutParams.WRAP_CONTENT);
			closeLayoutParams.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM, RelativeLayout.TRUE);
			closeLayoutParams.addRule(RelativeLayout.CENTER_HORIZONTAL, RelativeLayout.TRUE);
			closeButton.setLayoutParams(closeLayoutParams);
		}
		closeButton.setOnClickListener(new View.OnClickListener(){
			@Override
			public void onClick(View view) {
				Log.d(TAG, "Close Button onClick");
				dialog.dismiss();
				if (null != attAdView) {
					// Stops the Reload timer and all listeners
					attAdView.stopRefresh();
				}
			}
		});
		mainLayout.addView(closeButton);
		closeButton.setVisibility(View.VISIBLE);

		dialog.setContentView(mainLayout);
		dialog.setOnDismissListener(new Dialog.OnDismissListener() {

			@Override
			public void onDismiss(DialogInterface dialog) {
				mainLayout.removeAllViews();
			}
		});

		dialog.show();
	}

}
