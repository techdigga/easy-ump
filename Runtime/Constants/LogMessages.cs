namespace EasyUmp
{
    /// <summary>
    /// User-facing log message templates.
    /// </summary>
    public static class LogMessages
    {
        public const string ResolvePlatformImplementation = "Resolving UMP platform implementation.";
        public const string UsingAndroidImplementation = "Using Android UMP implementation.";
        public const string UsingIosImplementation = "Using iOS UMP implementation.";
        public const string UsingEditorImplementation = "Using Editor UMP implementation.";

        public const string InitRequested = "Init requested.";
        public const string ShowRequested = "Show requested.";
        public const string ReshowRequested = "Reshow requested.";
        public const string ResetRequested = "Reset requested.";

        public const string AndroidBridgeInitializing = "Initializing Android bridge.";
        public const string AndroidBridgeReady = "Android bridge initialized.";
        public const string IosBridgeReady = "iOS bridge initialized.";

        public const string OperationStarted = "{0} started.";
        public const string OperationCompleted = "{0} completed.";
        public const string OperationFailed = "{0} failed. Code={1}, Message={2}";
        public const string OperationRejectedInProgress = "{0} rejected because another operation is in progress.";
        public const string AutoShowTriggered = "Auto-show is enabled. Calling Show after Init success.";

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
