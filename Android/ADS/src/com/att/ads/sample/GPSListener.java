package com.att.ads.sample;

import android.content.Context;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;

/**
 * This class for the GPS enabling and disabling . getting the location of the
 * device. there two ways for getting the location.one is the NETWORK PROVIDER
 * other is GPS PROVIDER. Both the permission should be declare in manifest.
 * Allows an application to access coarse (e.g., Cell-ID, WiFi) location
 * <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
 * Allows an application to access (e.g., GPS) location <uses-permission
 * android:name="android.permission.ACCESS_FINE_LOCATION" />
 */
public class GPSListener {

	private static String TAG = "GPS Listener";

	protected static final int DO_EXPIRY = 0;

	private Context context;

	private static GPSListener instance = null;

	private LocationManager locationMgr = null;

	private Location currentLocation = null;

	private OnLocationInitialized onLocationInitialized = null;

	/**
	 * GPS Location Listener
	 */
	private LocationListener gpsLocListener = null;

	/**
	 * Network Location listener
	 */
	private LocationListener nwLocListener = null;

	private GPSListener() {

	}

	public static GPSListener getInstance() {
		if (instance == null) {
			instance = new GPSListener();
		}
		return instance;
	}

	/**
	 * Initiates location listeners. This method initiates both GPS and Network
	 * Location Listeners
	 */
	public void startLocationListening() {

		this.context = AdsApplication.getInstance().getApplicationContext();

		LocationListener gpsLocListenerTemp = gpsLocListener;
		LocationListener nwLocListenerTemp = nwLocListener;

		gpsLocListener = new GPSLocListener();
		nwLocListener = new NWLocationListener();

		currentLocation = null;

		locationMgr = (LocationManager) context.getSystemService(Context.LOCATION_SERVICE);
		locationMgr.requestLocationUpdates(LocationManager.GPS_PROVIDER, 1000L, 10, gpsLocListener);
		locationMgr.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, 1000L, 10,
				nwLocListener);

		startTimeTracker();
		if (gpsLocListenerTemp != null) {
			locationMgr.removeUpdates(gpsLocListenerTemp);
		}
		if (nwLocListenerTemp != null) {
			locationMgr.removeUpdates(nwLocListenerTemp);
		}

	}

	/**
	 * Stops the location listening.
	 */
	public void stopLocationListening() {
		Log.i(TAG, "stopLocationListening");
		if (locationMgr != null && gpsLocListener != null) {
			locationMgr.removeUpdates(gpsLocListener);
			gpsLocListener = null;
		}
		if (locationMgr != null && nwLocListener != null) {
			locationMgr.removeUpdates(nwLocListener);
			nwLocListener = null;
		}

		currentLocation = null;

		onLocationInitialized = null;

		/**
		 * Removing Expiry message if it is present
		 */
		removeHandlerMessage();
	}

	/**
	 * Gives the current location
	 * 
	 * @return
	 */
	public Location getCurrLocation() {
		return currentLocation;
	}

	/**
	 * Checks for the last known location from Network Provider
	 */
	public Location getLastKnownLocation() {

		Location lastKnownLocation = null;
		LocationManager locationMgr = (LocationManager) context
				.getSystemService(Context.LOCATION_SERVICE);

		lastKnownLocation = locationMgr.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);

		return lastKnownLocation;
	}

	/**
	 * 
	 * One of the usages of Handler is to schedule messages at some point in the
	 * future; Here we are using Handler to process the EXPIRY message which is
	 * sent after 90 seconds if the device does not fetch any Location details.
	 */
	private Handler handler = new Handler() {
		public void handleMessage(android.os.Message msg) {
			if (msg.what == DO_EXPIRY) {
				if (onLocationInitialized != null) {
					onLocationInitialized.onTimeExpired(getLastKnownLocation());
				}
			}

		};
	};

	/**
	 * Starts the time tracker for monitoring to get Location details. If
	 * location details are not available then after mentioned time it calls
	 * timeExpired message.
	 */
	private void startTimeTracker() {
		long delayInStarting = 30 * 1000L; // 1 minute
		Log.d(TAG, " send DO_EXPIRY");
		handler.sendEmptyMessageDelayed(DO_EXPIRY, delayInStarting);
	}

	/**
	 * Remove handler message.
	 */
	private void removeHandlerMessage() {
		if (handler != null && handler.hasMessages(DO_EXPIRY)) {
			handler.removeMessages(DO_EXPIRY);
		}
	}

	/**
	 * 
	 * @param onLocationInitialized
	 */
	public void setOnLocationInitialized(OnLocationInitialized onLocationInitialized) {
		this.onLocationInitialized = onLocationInitialized;
	}

	/**
	 * Location listener which gives updates based on the network connection.
	 */
	private class NWLocationListener implements LocationListener {

		public void onStatusChanged(String provider, int status, Bundle extras) {
			Log.d(TAG, " NW_PROVIDER: onStatusChanged");
		}

		public void onProviderEnabled(String provider) {
			Log.d(TAG, " NW_PROVIDER: onProviderEnabled");

		}

		public void onProviderDisabled(String provider) {
			Log.d(TAG, " NW_PROVIDER: onProviderDisabled");
		}

		public void onLocationChanged(Location location) {
			Log.i(TAG, "NW_PROVIDER: onLocationChanged");
			currentLocation = location;
			if (currentLocation != null && onLocationInitialized != null) {
				onLocationInitialized.onLocationChanged(location);
				/**
				 * Removing Expiry message if it is present
				 */
				removeHandlerMessage();
			}

		}
	}

	/**
	 * Location listener which gives updates based on GPS Location
	 */
	private class GPSLocListener implements LocationListener {
		public void onStatusChanged(String provider, int status, Bundle extras) {
			Log.i(TAG, "GPS_PROVIDER: onStatusChanged");
		}

		public void onProviderEnabled(String provider) {
			Log.i(TAG, "GPS_PROVIDER: onProviderEnabled");

		}

		public void onProviderDisabled(String provider) {
			Log.i(TAG, "GPS_PROVIDER: onProviderDisabled");
		}

		public void onLocationChanged(Location location) {
			Log.i(TAG, "GPS_PROVIDER: onLocationChanged");
			currentLocation = location;
			if (currentLocation != null && onLocationInitialized != null) {
				onLocationInitialized.onLocationChanged(location);
				/**
				 * Removing Expiry message if it is present
				 */
				removeHandlerMessage();
			}
		}

	}

}
