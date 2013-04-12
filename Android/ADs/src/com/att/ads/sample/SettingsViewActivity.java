package com.att.ads.sample;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.app.ProgressDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.location.Location;
import android.location.LocationManager;
import android.os.Bundle;
import android.provider.Settings;
import android.text.Editable;
import android.text.InputFilter;
import android.text.Spanned;
import android.text.TextWatcher;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.EditText;
import android.widget.TextView;

/**
 * The UI activity which is used to configure Settings.
 * 
 * @author ATT
 * 
 */
public class SettingsViewActivity extends Activity {

	private static final String TAG = "SettingsViewActivity";

	private static final int DIALOG_PREMIUM_TYPE = 1;

	private static final int DIALOG_AGE_GROUP_TYPE = 2;

	private static final int DIALOG_AD_SIZE_TYPE = 3;

	private static final int DIALOG_AD_LOCATION = 4;
	
	private static final int LOCATION_SOURCE_SETTINGS = 5;	     
	
	private TextView premiumType = null;
	private TextView ageGroup = null;
	private TextView adSize = null;

	private EditText latitudeEditTextView = null;
	private EditText longitudeEditTextView = null;
	private CheckBox showLatLongCheckBox = null;

	private EditText keyWordsEditText = null;
	private EditText zipEditText = null;
	private String[] premiumTypes = null;
	private String[] ageGroupTypes = null;
	private String[] adSizeTypes = null;

	private ProgressDialog progressDialog = null;

	/**
	 * Called when the activity is first created.
	 */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.settings_tab_layout);

		keyWordsEditText = (EditText) findViewById(R.id.keyWordsEditText);
		zipEditText = (EditText) findViewById(R.id.zipEditText);

		keyWordsEditText.addTextChangedListener(new TextWatcher() {

			@Override
			public void onTextChanged(CharSequence s, int start, int before, int count) {
			}

			@Override
			public void beforeTextChanged(CharSequence s, int start, int count, int after) {
			}

			@Override
			public void afterTextChanged(Editable s) {
				Log.d(TAG, " Valeu after textchnaged : " + s.toString());
				AdsApplication.getInstance().setKeywords(s.toString());
			}
		});
		keyWordsEditText.setFilters(new InputFilter[] { new InputFilter() {
			public CharSequence filter(CharSequence src, int start, int end, Spanned dst,
					int dstart, int dend) {
				if (src.equals("")) { // for backspace
					return src;
				}
				if (src.toString().matches("[a-zA-Z0-9]+")) {
					return src;
				}
				return "";
			}
		} });

		zipEditText.addTextChangedListener(new TextWatcher() {

			@Override
			public void onTextChanged(CharSequence s, int start, int before, int count) {
			}

			@Override
			public void beforeTextChanged(CharSequence s, int start, int count, int after) {
			}

			@Override
			public void afterTextChanged(Editable s) {
				int zip = (s != null && s.length() > 0) ? Integer.parseInt(s.toString()) : 0;
				AdsApplication.getInstance().setZip(zip);
			}
		});

		premiumTypes = getResources().getStringArray(R.array.premiumTypes);
		ageGroupTypes = getResources().getStringArray(R.array.ageGroupTypes);
		adSizeTypes = getResources().getStringArray(R.array.adSizeTypes);

		premiumType = (TextView) findViewById(R.id.premiumType);
		premiumType.setText(premiumTypes[0]);
		premiumType.setOnClickListener(onClickListener);

		ageGroup = (TextView) findViewById(R.id.ageGroupType);
		ageGroup.setText(ageGroupTypes[0]);
		ageGroup.setOnClickListener(onClickListener);

		adSize = (TextView) findViewById(R.id.adSizeType);
		adSize.setText(adSizeTypes[0]);
		adSize.setOnClickListener(onClickListener);

		latitudeEditTextView = (EditText) findViewById(R.id.latitudeEditText);
		latitudeEditTextView.addTextChangedListener(new TextWatcher() {

			@Override
			public void onTextChanged(CharSequence s, int start, int before, int count) {
			}

			@Override
			public void beforeTextChanged(CharSequence s, int start, int count, int after) {
			}

			@Override
			public void afterTextChanged(Editable s) {
				if(s != null && s.length() > 0) {
					double latitude = 0;
					try{
						latitude = Double.parseDouble(s.toString());						
					}catch (NumberFormatException e) {

					}
					AdsApplication.getInstance().setLatitude(latitude);
				}
			}
		});
		longitudeEditTextView = (EditText) findViewById(R.id.longitudeEditText);
		longitudeEditTextView.addTextChangedListener(new TextWatcher() {

			@Override
			public void onTextChanged(CharSequence s, int start, int before, int count) {
			}

			@Override
			public void beforeTextChanged(CharSequence s, int start, int count, int after) {
			}

			@Override
			public void afterTextChanged(Editable s) {
				if(s != null && s.length() > 0) {
					double longitude = 0;
					try{
						longitude = Double.parseDouble(s.toString());						
					}catch (NumberFormatException e) {

					}
					AdsApplication.getInstance().setLongitude(longitude);
				}
			}
		});

		showLatLongCheckBox = (CheckBox) findViewById(R.id.showLatLong);
		showLatLongCheckBox.setOnCheckedChangeListener(new OnCheckedChangeListener() {

			@Override
			public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
				if (isChecked) {
					if (!isLocationServiceEnabled()) {
						Log.d(TAG, "showDialog");
						showDialog(DIALOG_AD_LOCATION);
					} else {
						GPSListener.getInstance().setOnLocationInitialized(onLocationInitialized);
						GPSListener.getInstance().startLocationListening();

						displayProgressDialog();

						latitudeEditTextView.setEnabled(false);
						longitudeEditTextView.setEnabled(false);
					}
				} else {
					
					isChecked = false;
					latitudeEditTextView.setEnabled(true);
					longitudeEditTextView.setEnabled(true);

					latitudeEditTextView.setText("");
					longitudeEditTextView.setText("");
				}
			}
		});

	}

	/**
	 * Creation of Dialogs/Alerts
	 * 
	 * @param id
	 * @return
	 */
	@Override
	protected Dialog onCreateDialog(int id) {
		switch (id) {
		case DIALOG_PREMIUM_TYPE:
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.setTitle(R.string.select_premium);
			builder.setSingleChoiceItems(premiumTypes, 0, new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog, int item) {
					premiumType.setText(premiumTypes[item]);
					AdsApplication.getInstance().setPremium(item);

					dialog.cancel();
				}
			});
			return builder.create();

		case DIALOG_AGE_GROUP_TYPE:
			AlertDialog.Builder ageGroupBuilder = new AlertDialog.Builder(this);
			ageGroupBuilder.setTitle("Select Age Group :");
			ageGroupBuilder.setSingleChoiceItems(ageGroupTypes, 0,
					new DialogInterface.OnClickListener() {
						public void onClick(DialogInterface dialog, int item) {
							if(ageGroupTypes[item].contains("-")){
							ageGroup.setText(ageGroupTypes[item]);
							AdsApplication.getInstance().setAgeGroup(ageGroupTypes[item]);
							}else{
								AdsApplication.getInstance().setAgeGroup(null);
							}
							dialog.cancel();
						}
					});
			return ageGroupBuilder.create();
		case DIALOG_AD_SIZE_TYPE:
			AlertDialog.Builder adSizeBuilder = new AlertDialog.Builder(this);
			adSizeBuilder.setTitle("Select Ad Size :");
			adSizeBuilder.setSingleChoiceItems(adSizeTypes, 0,
					new DialogInterface.OnClickListener() {
						public void onClick(DialogInterface dialog, int item) {
							adSize.setText(adSizeTypes[item]);
							if(adSizeTypes[item].contains("x")){
								String items[] = adSizeTypes[item].split(" x ");
								AdsApplication.getInstance().setMaxWidth(Integer.parseInt(items[0]));
								AdsApplication.getInstance().setMaxHeight(Integer.parseInt(items[1]));
							}else{
								AdsApplication.getInstance().setMaxWidth(0);
								AdsApplication.getInstance().setMaxHeight(0);
							}
							dialog.cancel();							
						}
					});
			return adSizeBuilder.create();

		case DIALOG_AD_LOCATION:
			AlertDialog.Builder builder1 = new AlertDialog.Builder(this);
			builder1.setMessage(R.string.no_location_providers);
			builder1.setCancelable(false);
			builder1.setPositiveButton("Settings", new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog, int which) {
					dialog.dismiss();
					startLocationSettingActivity();
				}
			});
			builder1.setNegativeButton("Quit", new DialogInterface.OnClickListener() {
				public void onClick(DialogInterface dialog, int which) {
					showLatLongCheckBox.setChecked(false);
					dialog.cancel();
					
				}
			});

			return builder1.create();
		default:
			return null;
		}

	}

	/**
	 * Click Event Listener for Views(Buttons,TextViews etc.)
	 */
	private OnClickListener onClickListener = new OnClickListener() {

		@Override
		public void onClick(View view) {
			switch (view.getId()) {
			case R.id.premiumType:
				showDialog(DIALOG_PREMIUM_TYPE);
				break;
			case R.id.ageGroupType:
				showDialog(DIALOG_AGE_GROUP_TYPE);
				break;
			case R.id.adSizeType:
				showDialog(DIALOG_AD_SIZE_TYPE);
				break;
			default:
				break;
			}
		}
	};

	private OnLocationInitialized onLocationInitialized = new OnLocationInitialized() {

		@Override
		public void onTimeExpired(Location lastKnownLocation) {
			if (lastKnownLocation != null) {
				AdsApplication.getInstance().setLatitude(lastKnownLocation.getLatitude());
				AdsApplication.getInstance().setLongitude(lastKnownLocation.getLongitude());
			} else {
				// Null cases will be handled here
			}

			latitudeEditTextView.setText("" + AdsApplication.getInstance().getLatitude());
			longitudeEditTextView.setText("" + AdsApplication.getInstance().getLongitude());

			GPSListener.getInstance().stopLocationListening();

			dismissProgressDialog();
		}

		@Override
		public void onLocationChanged(Location currentLocation) {
			if (currentLocation != null) {
				AdsApplication.getInstance().setLatitude(currentLocation.getLatitude());
				AdsApplication.getInstance().setLongitude(currentLocation.getLongitude());
			} else {
				// Null cases will be handled here
			}

			latitudeEditTextView.setText("" + AdsApplication.getInstance().getLatitude());
			longitudeEditTextView.setText("" + AdsApplication.getInstance().getLongitude());

			GPSListener.getInstance().stopLocationListening();

			dismissProgressDialog();

		}
	};

	private void displayProgressDialog() {

		if (progressDialog == null) {
			progressDialog = new ProgressDialog(this);
			progressDialog.setMessage("Fetching Lat/Long Values");
		}
		progressDialog.show();
	}

	private void dismissProgressDialog() {
		if (progressDialog != null) {
			progressDialog.dismiss();
		}
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		// TODO Auto-generated method stub
		if (requestCode == LOCATION_SOURCE_SETTINGS) {

			if (!isLocationServiceEnabled()) {
				Log.d(TAG, "showDialog");
				showDialog(DIALOG_AD_LOCATION);
			} else {
				// start GPS
				GPSListener.getInstance().setOnLocationInitialized(onLocationInitialized);
				GPSListener.getInstance().startLocationListening();
				displayProgressDialog();
				latitudeEditTextView.setEnabled(false);
				longitudeEditTextView.setEnabled(false);

			}
		}

		super.onActivityResult(requestCode, resultCode, data);
	}

	/**
	 * 
	 * @return
	 */
	private boolean isLocationServiceEnabled() {
		boolean enabled = false;
		// first, make sure at least one provider actually exists
		LocationManager lm = (LocationManager) getSystemService(LOCATION_SERVICE);

		boolean gpsExists = (lm.getProvider(LocationManager.GPS_PROVIDER) != null);
		boolean networkExists = (lm.getProvider(LocationManager.NETWORK_PROVIDER) != null);

		if (gpsExists || networkExists) {
			enabled = ((!gpsExists || lm.isProviderEnabled(LocationManager.GPS_PROVIDER)) && (!networkExists || lm
					.isProviderEnabled(LocationManager.NETWORK_PROVIDER)));
		}
		Log.d(TAG, "enabled = " + enabled);
		return enabled;
	}

	void startLocationSettingActivity() {
		Intent intent = new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS);
		startActivityForResult(intent, LOCATION_SOURCE_SETTINGS);
	}
	
	@Override
	protected void onPause() {
		// TODO Auto-generated method stub
		super.onPause();
		AdsApplication.getInstance().setChangeInSettings(true);
	}
}
