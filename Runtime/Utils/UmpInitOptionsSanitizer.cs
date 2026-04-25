namespace EasyUmp
{
    internal static class UmpInitOptionsSanitizer
    {
        internal static UmpInitOptions MergeWithRuntimeDefaults(UmpInitOptions options)
        {
            var merged = options ?? new UmpInitOptions();
            merged.ConsentSyncId = SanitizeConsentSyncId(merged.ConsentSyncId);

            if (merged.TestDeviceHashedIds == null || merged.TestDeviceHashedIds.Count == 0)
            {
                var config = EasyUmpRuntimeConfig.Load();
                if (config != null && config.TestDeviceHashedIds != null && config.TestDeviceHashedIds.Length > 0)
                {
                    merged.TestDeviceHashedIds = new System.Collections.Generic.List<string>(config.TestDeviceHashedIds);
                }
            }

            return merged;
        }

        internal static string SanitizeConsentSyncId(string consentSyncId)
        {
            if (string.IsNullOrWhiteSpace(consentSyncId))
            {
                return null;
            }

            return consentSyncId.Trim();
        }
    }
}
