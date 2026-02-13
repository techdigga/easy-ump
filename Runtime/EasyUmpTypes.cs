using System;
using System.Collections.Generic;

namespace EasyUmp
{
    /// <summary>
    /// Maps to UMP consent status values.
    /// </summary>
    public enum UmpConsentStatus
    {
        Unknown = 0,
        Required = 1,
        NotRequired = 2,
        Obtained = 3
    }

    /// <summary>
    /// Debug geography overrides for testing.
    /// </summary>
    public enum UmpDebugGeography
    {
        Disabled = 0,
        Eea = 1,
        NotEea = 2
    }

    /// <summary>
    /// Initialization options for UMP.
    /// </summary>
    [Serializable]
    public sealed class UmpInitOptions
    {
        public bool TagForUnderAgeOfConsent;
        public UmpDebugGeography DebugGeography = UmpDebugGeography.Disabled;
        public List<string> TestDeviceHashedIds = new List<string>();
    }

    /// <summary>
    /// Error payload returned from native UMP.
    /// </summary>
    [Serializable]
    public sealed class UmpError
    {
        public int Code;
        public string Message;
        public string Domain;
    }
}
