package com.techdigga.easyump;

/**
 * Constants shared by the Unity UMP bridge.
 */
final class UnityUmpConstants {
    static final String UNITY_METHOD_INIT_SUCCESS = "OnInitSuccess";
    static final String UNITY_METHOD_INIT_FAILURE = "OnInitFailure";
    static final String UNITY_METHOD_SHOW_DISMISSED = "OnShowDismissed";
    static final String UNITY_METHOD_SHOW_FAILED = "OnShowFailed";
    static final String UNITY_METHOD_RESHOW_DISMISSED = "OnReshowDismissed";
    static final String UNITY_METHOD_RESHOW_FAILED = "OnReshowFailed";

    static final String JSON_KEY_CODE = "Code";
    static final String JSON_KEY_MESSAGE = "Message";
    static final String JSON_KEY_TAG_UNDER_AGE = "TagForUnderAgeOfConsent";
    static final String JSON_KEY_DEBUG_GEOGRAPHY = "DebugGeography";
    static final String JSON_KEY_TEST_DEVICE_IDS = "TestDeviceHashedIds";

    static final String IAB_TCF_TC_STRING = "IABTCF_TCString";
    static final String IAB_TCF_ADDL_CONSENT = "IABTCF_AddtlConsent";
    static final String IAB_TCF_PURPOSE_CONSENTS = "IABTCF_PurposeConsents";
    static final String IAB_TCF_GDPR_APPLIES = "IABTCF_gdprApplies";

    static final String ERROR_ACTIVITY_NULL = "Unity activity is null.";
    static final String ERROR_UNKNOWN = "Unknown UMP error.";

    private UnityUmpConstants() {}
}
