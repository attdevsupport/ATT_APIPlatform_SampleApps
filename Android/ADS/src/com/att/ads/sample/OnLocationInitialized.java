package com.att.ads.sample;

import android.location.Location;
/**
 * This interface define the GPS location change & time Expire for Lat/Long.
 * @author ATT
 *
 */
public interface OnLocationInitialized {
	
	public void onLocationChanged(Location currentLocation);

	public void onTimeExpired(Location lastKnownLocation);
}
