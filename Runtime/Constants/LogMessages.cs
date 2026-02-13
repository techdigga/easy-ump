namespace EasyUmp
{
    /// <summary>
    /// User-facing log message templates.
    /// </summary>
    public static class LogMessages
    {
        public const string AndroidAppIdMissing =
            "AdMob Application Id is not set. Set it in Project Settings > Easy UMP.";

        public const string ManifestUpdateFailed =
            "Failed to update AndroidManifest at {0}. {1}";

        public const string ManifestNotFound =
            "No AndroidManifest.xml found to update.";

        public const string IosAppIdMissing =
            "AdMob Application Id (iOS) is not set. Set it in Project Settings > Easy UMP.";

        public const string ConsentStringsUnavailable =
            "Consent strings may be unavailable before consent is collected. Call after Init/consent flow completes.";
    }
}
