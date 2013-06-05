/**
 * 
 */

package com.att.ads.sample;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.widget.TextView;

/**
 * Initial class for showing the splash screen.
 * 
 */
public class SplashActivity extends Activity {

    private static final String TAG = "SplashActivity";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.splash);
        
        //Updating the textView by reading the version number from the Android Manifest file.
        TextView splashMessage = (TextView) findViewById(R.id.message);
        
        PackageManager pkgManager = this.getPackageManager();
        try {
            PackageInfo pkgInfo = pkgManager.getPackageInfo(this.getPackageName(), 0);
            splashMessage.setText(getString(R.string.splash_version_message) + pkgInfo.versionName);
        } catch (NameNotFoundException e) {
            Log.e(TAG, "Name Not Found Exception caught");
        }
        
        new Handler().postDelayed(new Runnable() {
            public void run() {
                startMainActivity();
            }
        }, 3000);
    }

    protected void startMainActivity() {
        startActivity(new Intent(this, AdsMainActivity.class));
        finish();
    }
}
