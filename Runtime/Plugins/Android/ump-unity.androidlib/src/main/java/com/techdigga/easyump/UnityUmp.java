package com.techdigga.easyump;

import android.app.Activity;
import android.os.Handler;
import android.os.Looper;

import com.google.android.ump.ConsentDebugSettings;
import com.google.android.ump.ConsentForm;
import com.google.android.ump.ConsentInformation;
import com.google.android.ump.ConsentRequestParameters;
import com.google.android.ump.FormError;
import com.google.android.ump.UserMessagingPlatform;
import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;

public final class UnityUmp {
    private static String unityCallbackObject;
    private static ConsentInformation consentInformation;
    private static final Handler mainHandler = new Handler(Looper.getMainLooper());

    private UnityUmp() {}

    /**
     * Sets the Unity GameObject name that receives UnitySendMessage callbacks.
     */
    public static void setUnityCallbackObject(String name) {
        unityCallbackObject = name;
    }

    /**
     * Initializes UMP and requests consent info update.
     */
    public static void init(final String optionsJson) {
        final Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            sendError(UnityUmpConstants.UNITY_METHOD_INIT_FAILURE, -1, UnityUmpConstants.ERROR_ACTIVITY_NULL);
            return;
        }

        mainHandler.post(new Runnable() {
            @Override
            public void run() {
                ConsentRequestParameters params = buildRequestParameters(activity, optionsJson);
                consentInformation = UserMessagingPlatform.getConsentInformation(activity);
                consentInformation.requestConsentInfoUpdate(
                        activity,
                        params,
                        new ConsentInformation.OnConsentInfoUpdateSuccessListener() {
                            @Override
                            public void onConsentInfoUpdateSuccess() {
                                sendMessage(UnityUmpConstants.UNITY_METHOD_INIT_SUCCESS, "{}");
                            }
                        },
                        new ConsentInformation.OnConsentInfoUpdateFailureListener() {
                            @Override
                            public void onConsentInfoUpdateFailure(FormError formError) {
                                sendFormError(UnityUmpConstants.UNITY_METHOD_INIT_FAILURE, formError);
                            }
                        });
            }
        });
    }

    /**
     * Shows the consent form if required.
     */
    public static void show() {
        final Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            sendError(UnityUmpConstants.UNITY_METHOD_SHOW_FAILED, -1, UnityUmpConstants.ERROR_ACTIVITY_NULL);
            return;
        }

        mainHandler.post(new Runnable() {
            @Override
            public void run() {
                UserMessagingPlatform.loadAndShowConsentFormIfRequired(
                        activity,
                        new ConsentForm.OnConsentFormDismissedListener() {
                            @Override
                            public void onConsentFormDismissed(FormError formError) {
                                if (formError != null) {
                                    sendFormError(UnityUmpConstants.UNITY_METHOD_SHOW_FAILED, formError);
                                } else {
                                    sendMessage(UnityUmpConstants.UNITY_METHOD_SHOW_DISMISSED, "{}");
                                }
                            }
                        }
                );
            }
        });
    }

    /**
     * Shows the privacy options form.
     */
    public static void reshow() {
        final Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            sendError(UnityUmpConstants.UNITY_METHOD_RESHOW_FAILED, -1, UnityUmpConstants.ERROR_ACTIVITY_NULL);
            return;
        }

        mainHandler.post(new Runnable() {
            @Override
            public void run() {
                UserMessagingPlatform.showPrivacyOptionsForm(
                        activity,
                        new ConsentForm.OnConsentFormDismissedListener() {
                            @Override
                            public void onConsentFormDismissed(FormError formError) {
                                if (formError != null) {
                                    sendFormError(UnityUmpConstants.UNITY_METHOD_RESHOW_FAILED, formError);
                                } else {
                                    sendMessage(UnityUmpConstants.UNITY_METHOD_RESHOW_DISMISSED, "{}");
                                }
                            }
                        }
                );
            }
        });
    }

    /**
     * Resets local consent information.
     */
    public static void reset() {
        final Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            return;
        }

        mainHandler.post(new Runnable() {
            @Override
            public void run() {
                ConsentInformation info = UserMessagingPlatform.getConsentInformation(activity);
                info.reset();
            }
        });
    }

    /**
     * Returns the current consent status.
     */
    public static int getConsentStatus() {
        Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            return 0;
        }
        ConsentInformation info = UserMessagingPlatform.getConsentInformation(activity);
        return info.getConsentStatus();
    }

    /**
     * Returns whether ads can be requested.
     */
    public static boolean canRequestAds() {
        Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            return false;
        }
        ConsentInformation info = UserMessagingPlatform.getConsentInformation(activity);
        return info.canRequestAds();
    }

    public static String getIabTcfTcString() {
        return getPrefsString(UnityUmpConstants.IAB_TCF_TC_STRING);
    }

    public static String getIabTcfAddtlConsent() {
        return getPrefsString(UnityUmpConstants.IAB_TCF_ADDL_CONSENT);
    }

    public static String getIabTcfPurposeConsents() {
        return getPrefsString(UnityUmpConstants.IAB_TCF_PURPOSE_CONSENTS);
    }

    public static int getIabTcfGdprApplies() {
        Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            return -1;
        }
        SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(activity);
        if (!prefs.contains(UnityUmpConstants.IAB_TCF_GDPR_APPLIES)) {
            return -1;
        }
        return prefs.getInt(UnityUmpConstants.IAB_TCF_GDPR_APPLIES, -1);
    }

    /**
     * Builds consent request parameters from Unity JSON options.
     */
    private static ConsentRequestParameters buildRequestParameters(Activity activity, String optionsJson) {
        ConsentRequestParameters.Builder builder = new ConsentRequestParameters.Builder();

        if (optionsJson == null || optionsJson.isEmpty()) {
            return builder.build();
        }

        try {
            JSONObject obj = new JSONObject(optionsJson);
            if (obj.optBoolean(UnityUmpConstants.JSON_KEY_TAG_UNDER_AGE, false)) {
                builder.setTagForUnderAgeOfConsent(true);
            }

            int debugGeography = obj.optInt(UnityUmpConstants.JSON_KEY_DEBUG_GEOGRAPHY, 0);
            JSONArray testIds = obj.optJSONArray(UnityUmpConstants.JSON_KEY_TEST_DEVICE_IDS);
            if (debugGeography != 0 || (testIds != null && testIds.length() > 0)) {
                ConsentDebugSettings.Builder debugBuilder = new ConsentDebugSettings.Builder(activity);
                if (debugGeography != 0) {
                    debugBuilder.setDebugGeography(debugGeography);
                }
                if (testIds != null) {
                    for (int i = 0; i < testIds.length(); i++) {
                        String id = testIds.optString(i, null);
                        if (id != null && !id.isEmpty()) {
                            debugBuilder.addTestDeviceHashedId(id);
                        }
                    }
                }
                builder.setConsentDebugSettings(debugBuilder.build());
            }
        } catch (JSONException ignored) {
        }

        return builder.build();
    }

    /**
     * Sends a form error back to Unity.
     */
    private static void sendFormError(String method, FormError formError) {
        if (formError == null) {
            sendError(method, -2, UnityUmpConstants.ERROR_UNKNOWN);
            return;
        }
        sendError(method, formError.getErrorCode(), formError.getMessage());
    }

    private static String getPrefsString(String key) {
        Activity activity = UnityPlayer.currentActivity;
        if (activity == null) {
            return "";
        }
        SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(activity);
        return prefs.getString(key, "");
    }

    /**
     * Sends an error JSON payload back to Unity.
     */
    private static void sendError(String method, int code, String message) {
        try {
            JSONObject obj = new JSONObject();
            obj.put(UnityUmpConstants.JSON_KEY_CODE, code);
            obj.put(UnityUmpConstants.JSON_KEY_MESSAGE, message == null ? "" : message);
            sendMessage(method, obj.toString());
        } catch (JSONException ignored) {
            sendMessage(method, "{\"" + UnityUmpConstants.JSON_KEY_CODE + "\":" + code +
                    ",\"" + UnityUmpConstants.JSON_KEY_MESSAGE + "\":\"" + escape(message) + "\"}");
        }
    }

    /**
     * Sends a UnitySendMessage to the configured callback object.
     */
    private static void sendMessage(String method, String payload) {
        if (unityCallbackObject == null || unityCallbackObject.isEmpty()) {
            return;
        }
        UnityPlayer.UnitySendMessage(unityCallbackObject, method, payload == null ? "" : payload);
    }

    /**
     * Escapes quotes and backslashes for JSON string values.
     */
    private static String escape(String input) {
        if (input == null) {
            return "";
        }
        return input.replace("\\", "\\\\").replace("\"", "\\\"");
    }
}
