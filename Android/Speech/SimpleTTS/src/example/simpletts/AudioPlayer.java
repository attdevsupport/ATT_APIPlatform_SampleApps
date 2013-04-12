package example.simpletts;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;

import android.app.Activity;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.util.Log;

/**
 * A minimal, UI-less audio player.
 * Works around shortcomings of MediaPlayer, such as requiring audio data
 * to be in a file.
**/
public class AudioPlayer
{
    /** Activity this player is associated to. **/
    private final Activity activity;
    
    /**
     * Create a player for an activity.
    **/
    public AudioPlayer(Activity activity)
    {
        this.activity = activity;
    }
    
    /** Current media player.  Only exists while we're activity playing. **/
    private MediaPlayer mediaPlayer;
    /** The file holding the bytes we're playing. **/
    private File audioFile;
    
    /**
     * Play the audio data.
     * Only call this on the main thread.
    **/
    public void play(final byte[] audioData)
    {
        if (mediaPlayer != null || audioFile != null)
            stop();
        final MediaPlayer mediaPlayer = new MediaPlayer();
        this.mediaPlayer = mediaPlayer;
        // Call on background thread to avoid blocking UI.
        new Thread("Play audio "+audioData) {
            @Override public void run() {
                try {
                    final File audioFile = createTempFile(audioData);
                    mediaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);
                    final FileInputStream audioStream = new FileInputStream(audioFile);
                    mediaPlayer.setDataSource(audioStream.getFD());
                    mediaPlayer.setOnCompletionListener(new MediaPlayer.OnCompletionListener() {
                        @Override public void onCompletion(MediaPlayer mp) {
                            try {audioStream.close();} catch (IOException ex) {}
                            deleteTempFile(audioFile);
                        }
                    });
                    mediaPlayer.prepare();
                    // Start playing on main thread, in case we were already stopped.
                    activity.runOnUiThread(new Runnable() {
                        @Override public void run() {
                            if (AudioPlayer.this.mediaPlayer == mediaPlayer) {
                                mediaPlayer.start();
                                AudioPlayer.this.audioFile = audioFile;
                                Log.d("SimpleTTS", "Started playing audio from "+audioFile);
                            }
                            else {
                                mediaPlayer.release();
                                deleteTempFile(audioFile);
                            }
                        }
                    });
                }
                catch (IOException ex) {
                    Log.e("SimpleTTS", "Error spooling and playing audio data", ex);
                }
            }
        }.start();
    }
    
    /**
     * Stop playing audio.  It's OK to call this if nothing is being played.
     * Only call this on the main thread.
    **/
    public void stop()
    {
        if (mediaPlayer != null)
            mediaPlayer.release();
        mediaPlayer = null;
        if (audioFile != null)
            deleteTempFile(audioFile);
        audioFile = null;
    }
    
    /**
     * Spool the bytes to a temporary file.
    **/
    private File createTempFile(byte[] audioData) throws IOException
    {
        File file = File.createTempFile("SimpleTTS", "audio", activity.getCacheDir());
        file.deleteOnExit();
        FileOutputStream stream = new FileOutputStream(file);
        stream.write(audioData);
        stream.close();
        return file;
    }
    
    /**
     * Try to delete the temporary file.
    **/
    private static void deleteTempFile(final File tempFile)
    {
        // Do it on background thread to avoid blocking the UI.
        new Thread("Deleting "+tempFile.getName()) {
            @Override public void run() {
                tempFile.delete();
            }
        }.start();
    }
}
