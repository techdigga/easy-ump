namespace EasyUmp
{
    /// <summary>
    /// Public facade that routes calls to the active platform implementation.
    /// </summary>
    public static class EasyUmp
    {
        private static IEasyUmp implementation;
        private static bool consentStringsWarned;

        /// <summary>
        /// Lazily resolves the platform implementation.
        /// </summary>
        private static IEasyUmp Impl
        {
            get
            {
                if (implementation != null)
                {
                    return implementation;
                }

#if UNITY_ANDROID && !UNITY_EDITOR
                implementation = new EasyUmpAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
                implementation = new EasyUmpIos();
#else
                implementation = new EasyUmpEditor();
#endif
                return implementation;
            }
        }

        /// <summary>
        /// Whether UMP is supported on the current platform.
        /// </summary>
        /// <returns>True when running on a supported platform.</returns>
        public static bool IsSupported => Impl.IsSupported;
        /// <summary>
        /// Whether ads can be requested per UMP consent state.
        /// </summary>
        /// <returns>True if ads can be requested.</returns>
        public static bool CanRequestAds => Impl.CanRequestAds;
        /// <summary>
        /// Current consent status from UMP.
        /// </summary>
        /// <returns>Consent status enum value.</returns>
        public static UmpConsentStatus ConsentStatus => Impl.ConsentStatus;
        /// <summary>
        /// IAB TCF TC String.
        /// </summary>
        /// <returns>TC string or empty if unavailable.</returns>
        public static string GetTcString()
        {
            WarnIfConsentStringsUnavailable();
            return Impl.GetTcString();
        }
        /// <summary>
        /// IAB TCF Additional Consent String.
        /// </summary>
        /// <returns>Additional consent string or empty if unavailable.</returns>
        public static string GetAdditionalConsentString()
        {
            WarnIfConsentStringsUnavailable();
            return Impl.GetAdditionalConsentString();
        }
        /// <summary>
        /// IAB TCF Purpose Consents String.
        /// </summary>
        /// <returns>Purpose consents string or empty if unavailable.</returns>
        public static string GetPurposeConsentsString()
        {
            WarnIfConsentStringsUnavailable();
            return Impl.GetPurposeConsentsString();
        }
        /// <summary>
        /// IAB TCF GDPR Applies value (-1 if unknown).
        /// </summary>
        /// <returns>0/1 for applies, or -1 if unknown.</returns>
        public static int GetGdprApplies()
        {
            WarnIfConsentStringsUnavailable();
            return Impl.GetGdprApplies();
        }
        /// <summary>
        /// Initializes UMP and fetches consent info.
        /// </summary>
        /// <param name="options">Initialization options (may be null).</param>
        /// <param name="onSuccess">Invoked when consent info update succeeds.</param>
        /// <param name="onFailure">Invoked with error when init fails.</param>
        public static void Init(UmpInitOptions options, System.Action onSuccess, System.Action<UmpError> onFailure) =>
            Impl.Init(options, onSuccess, onFailure);
        /// <summary>
        /// Shows the consent form if required.
        /// </summary>
        /// <param name="onDismissed">Invoked when the form is dismissed or not required.</param>
        /// <param name="onFailure">Invoked with error when show fails.</param>
        public static void Show(System.Action onDismissed, System.Action<UmpError> onFailure) =>
            Impl.Show(onDismissed, onFailure);
        /// <summary>
        /// Shows the privacy options form.
        /// </summary>
        /// <param name="onDismissed">Invoked when the form is dismissed.</param>
        /// <param name="onFailure">Invoked with error when show fails.</param>
        public static void Reshow(System.Action onDismissed, System.Action<UmpError> onFailure) =>
            Impl.Reshow(onDismissed, onFailure);
        /// <summary>
        /// Resets local consent information.
        /// </summary>
        public static void Reset() => Impl.Reset();

        private static void WarnIfConsentStringsUnavailable()
        {
            if (consentStringsWarned)
            {
                return;
            }

            if (ConsentStatus == UmpConsentStatus.Unknown)
            {
                consentStringsWarned = true;
                Logger.Warning(LogMessages.ConsentStringsUnavailable);
            }
        }
    }
}
