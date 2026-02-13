using UnityEngine;

namespace EasyUmp
{
    /// <summary>
    /// Logging helper with a consistent tag and enable toggle.
    /// </summary>
    public static class Logger
    {
        private static bool enabled = true;

        /// <summary>
        /// Enables or disables EasyUmp logging.
        /// </summary>
        public static bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        /// <summary>
        /// Writes a debug-level log entry.
        /// </summary>
        public static void Debug(string message)
        {
            if (!enabled)
            {
                return;
            }
            UnityEngine.Debug.Log($"[{EasyUmpConstants.LogTag}] {message}");
        }

        /// <summary>
        /// Writes a warning-level log entry.
        /// </summary>
        public static void Warning(string message)
        {
            if (!enabled)
            {
                return;
            }
            UnityEngine.Debug.LogWarning($"[{EasyUmpConstants.LogTag}] {message}");
        }

        /// <summary>
        /// Writes an error-level log entry.
        /// </summary>
        public static void Error(string message)
        {
            if (!enabled)
            {
                return;
            }
            UnityEngine.Debug.LogError($"[{EasyUmpConstants.LogTag}] {message}");
        }
    }
}
