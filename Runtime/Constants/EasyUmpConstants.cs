namespace EasyUmp
{
    /// <summary>
    /// General constants for settings, manifest injection, and logging.
    /// </summary>
    public static class EasyUmpConstants
    {
        public const string SettingsPath = "Project/Easy UMP";
        public const string SettingsFilePath = "ProjectSettings/EasyUmpSettings.asset";
        public const string SettingsLabel = "Easy UMP";
        public const string SettingsAndroidSection = "Android";
        public const string SettingsIosSection = "iOS";
        public const string AdMobAppIdLabel = "AdMob Application Id";
        public const string AdMobAppIdTooltip = "Example: ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY";
        public const string AdMobAppIdIosLabel = "AdMob Application Id (iOS)";
        public const string AdMobAppIdIosTooltip = "Example: ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY";
        public const string DebugLogsLabel = "Enable Debug Logs";
        public const string DebugLogsTooltip = "Enable easy-ump logging.";
        public const string AutoShowLabel = "Auto-show Consent Form";
        public const string AutoShowTooltip = "Automatically show consent form after Init succeeds.";

        public const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
        public const string AdMobAppIdMetaDataKey = "com.google.android.gms.ads.APPLICATION_ID";
        public const string LauncherManifestRelativePath = "launcher/src/main/AndroidManifest.xml";
        public const string DefaultManifestRelativePath = "src/main/AndroidManifest.xml";
        public const string MetaDataXPath = "meta-data[@android:name='" + AdMobAppIdMetaDataKey + "']";

        public const string LogTag = "easy-ump";
        public const string IosInfoPlistKey = "GADApplicationIdentifier";
    }
}
