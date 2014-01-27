package com.att.ads.sample;

import android.util.Log;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import javax.crypto.BadPaddingException;
import javax.crypto.Cipher;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.KeyGenerator;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

/**
 * AES algorithm is the “Advanced Encryption Standard.” to encrypt, decrypt,
 * generate a message authentication code
 * 
 * @author ATT
 * 
 */
public class AdsEncryptDecrypt {

	private static final String TAG = "API_ED";

	private static final char[] HEX_CHARS = "0123456789abcdef".toCharArray();

	private final static String CIPHERMODEPADDING = "AES/CBC/PKCS7Padding";

	private static byte[] iv = { 0xA, 1, 0xB, 5, 4, 0xF, 7, 9, 0x17, 3, 1, 6, 8, 0xC, 0xD, 91 };

	private static IvParameterSpec IV = new IvParameterSpec(iv);

	/**
	 * Convert Byte Array to Hex String.
	 * 
	 * @param aByteBuffer
	 * @return
	 */
	public static String convertToHex(byte[] aByteBuffer) {
		String strConverted = null;
		if ((aByteBuffer != null) && (aByteBuffer.length > 0)) {
			char[] aChars = new char[2 * aByteBuffer.length];
			for (int iBufIdx = 0; iBufIdx < aByteBuffer.length; ++iBufIdx) {
				int iCharIdx = iBufIdx * 2;
				aChars[iCharIdx] = HEX_CHARS[(aByteBuffer[iBufIdx] & 0xF0) >>> 4];
				aChars[iCharIdx + 1] = HEX_CHARS[aByteBuffer[iBufIdx] & 0x0F];
			}
			strConverted = new String(aChars);
		}
		return strConverted;
	}

	/**
	 * Converting from hex String to a byte array.
	 * 
	 * @param strHexString
	 * @return
	 */
	public static byte[] convertHexStringToByteArray(String strHexString) {
		byte[] hexData = null;
		if ((strHexString != null) && (strHexString.length() > 0)) {
			int iHexLen = strHexString.length();
			hexData = new byte[iHexLen / 2];
			int iHexPos = 0;
			int iCurrStringPos = 0;
			for (int idx = 0; idx < iHexLen; idx += 2) {
				iHexPos = idx / 2;
				iCurrStringPos = idx;
				int iDigit = Character.digit(strHexString.charAt(iCurrStringPos), 16);
				int iDigit2 = Character.digit(strHexString.charAt(iCurrStringPos + 1), 16);
				hexData[iHexPos] = (byte) ((iDigit << 4) + iDigit2);
			}
		}
		return hexData;
	}

	/**
	 * Creates a new SecretKeySpec for the specified key data and algorithm
	 * name(AES) in a provider-independent fashion.
	 * 
	 * @param strSeedKey
	 * @return
	 * @throws Exception
	 */
	public static SecretKeySpec getSecretKeySpec(String strSeedKey) throws Exception {
		SecretKeySpec secretKeySpec = null;
		if ((strSeedKey != null) && (strSeedKey.length() > 0)) {
			byte[] byteBSSIDValue = strSeedKey.getBytes();
			try {
				SecureRandom rng;
				if(android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.JELLY_BEAN_MR1)
					rng = SecureRandom.getInstance("SHA1PRNG", "Crypto");
				else
					rng = SecureRandom.getInstance("SHA1PRNG");
				rng.setSeed(byteBSSIDValue);
				KeyGenerator kg = KeyGenerator.getInstance("AES");
				kg.init(128, rng);
				byte[] byteEncodedKeyValue = kg.generateKey().getEncoded();
				secretKeySpec = new SecretKeySpec(byteEncodedKeyValue, "AES");
			} catch (NoSuchAlgorithmException e) {
				throw e;
			}
		}
		return secretKeySpec;
	}

	/**
	 * get Encrypted value by given Original value and A key specification for a
	 * SecretKey.
	 * 
	 * @param strOrigValue
	 * @param secretKeySpec
	 * @return
	 * @throws Exception
	 */
	public static String getEncryptedValue(String strOrigValue, SecretKeySpec secretKeySpec)
			throws Exception {
		String strValue = null;
		try {
			if ((secretKeySpec != null) && (strOrigValue != null) && (strOrigValue.length() > 0)) {
				Cipher cipher = Cipher.getInstance(CIPHERMODEPADDING);
				cipher.init(Cipher.ENCRYPT_MODE, secretKeySpec, IV);
				byte[] encryptedOrigValue = cipher.doFinal(strOrigValue.getBytes());
				strValue = convertToHex(encryptedOrigValue);
			} else {
				Log.i(TAG, "ECY: warn: gev: sks is null");
			}
		} catch (NoSuchAlgorithmException e) {
			throw e;
		} catch (NoSuchPaddingException e) {
			throw e;
		} catch (InvalidKeyException e) {
			throw e;
		} catch (BadPaddingException e) {
			throw e;
		} catch (IllegalBlockSizeException e) {
			throw e;
		}
		return strValue;
	}

	/**
	 * get Decrypted value by given Encrypted value and A key specification for
	 * a SecretKey.
	 * 
	 * @param strEncryptedValue
	 * @param secretKeySpec
	 * @return
	 * @throws Exception
	 */
	public static String getDecryptedValue(String strEncryptedValue, SecretKeySpec secretKeySpec)
			throws Exception {
		String strDecryptedValue = null;
		if ((strEncryptedValue != null) && (strEncryptedValue.length() > 0)) {
			byte[] encryptedValue = convertHexStringToByteArray(strEncryptedValue);
			try {
				if (secretKeySpec != null) {
					Cipher cipher = Cipher.getInstance(CIPHERMODEPADDING);
					cipher.init(Cipher.DECRYPT_MODE, secretKeySpec, IV);
					strDecryptedValue = new String(cipher.doFinal(encryptedValue));
				} else {
					Log.i(TAG, "ECY: warn: gdv: sks is null");
				}
			} catch (NoSuchAlgorithmException e) {
				throw e;
			} catch (NoSuchPaddingException e) {
				throw e;
			} catch (InvalidKeyException e) {
				throw e;
			} catch (BadPaddingException e) {
				throw e;
			} catch (IllegalBlockSizeException e) {
				throw e;
			}
		}
		return strDecryptedValue;
	}
}
