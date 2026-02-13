namespace EasyUmp
{
    /// <summary>
    /// Constants used by the Unity/Android bridge.
    /// </summary>
    public static class UnityNativeBridgeConstants
    {
        public const string AndroidJavaClass = "com.techdigga.easyump.UnityUmp";
        public const string CallbackObjectName = "__EasyUmpCallbacks";
        public const string MainThreadDispatcherObjectName = "__EasyUmpMainThreadDispatcher";

        public const string SetUnityCallbackObjectMethod = "setUnityCallbackObject";
        public const string InitMethod = "init";
        public const string ShowMethod = "show";
        public const string ReshowMethod = "reshow";
        public const string ResetMethod = "reset";
        public const string CanRequestAdsMethod = "canRequestAds";
        public const string GetConsentStatusMethod = "getConsentStatus";

        public const string OnInitSuccess = "OnInitSuccess";
        public const string OnInitFailure = "OnInitFailure";
        public const string OnShowDismissed = "OnShowDismissed";
        public const string OnShowFailed = "OnShowFailed";
        public const string OnReshowDismissed = "OnReshowDismissed";
        public const string OnReshowFailed = "OnReshowFailed";

        public const string ErrorCodeKey = "Code";
        public const string ErrorMessageKey = "Message";
        public const string ErrorDomainKey = "Domain";

        public const string IabTcStringKey = "IABTCF_TCString";
        public const string IabAddtlConsentKey = "IABTCF_AddtlConsent";
        public const string IabPurposeConsentsKey = "IABTCF_PurposeConsents";
        public const string IabGdprAppliesKey = "IABTCF_gdprApplies";

        public const string GetIabTcStringMethod = "getIabTcfTcString";
        public const string GetIabAddtlConsentMethod = "getIabTcfAddtlConsent";
        public const string GetIabPurposeConsentsMethod = "getIabTcfPurposeConsents";
        public const string GetIabGdprAppliesMethod = "getIabTcfGdprApplies";
    }
}
