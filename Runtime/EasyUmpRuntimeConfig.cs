using UnityEngine;

namespace EasyUmp
{
    /// <summary>
    /// Runtime configuration loaded from Resources.
    /// </summary>
    public sealed class EasyUmpRuntimeConfig : ScriptableObject
    {
        private const string ResourcePath = "EasyUmpRuntimeConfig";

        public bool AutoShow;
        public bool DebugLogging = true;
        public string[] TestDeviceHashedIds = new string[0];

        /// <summary>
        /// Loads the runtime config from Resources if present.
        /// </summary>
        public static EasyUmpRuntimeConfig Load()
        {
            return Resources.Load<EasyUmpRuntimeConfig>(ResourcePath);
        }
    }
}
