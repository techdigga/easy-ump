using System;

namespace EasyUmp
{
    /// <summary>
    /// Editor-only implementation that reports unsupported platform.
    /// </summary>
    internal sealed class EasyUmpEditor : IEasyUmp
    {
        /// <summary>
        /// UMP is not supported in the Editor.
        /// </summary>
        public bool IsSupported => false;
        /// <summary>
        /// Always false in the Editor.
        /// </summary>
        public bool CanRequestAds => false;
        /// <summary>
        /// Unknown consent status in the Editor.
        /// </summary>
        public UmpConsentStatus ConsentStatus => UmpConsentStatus.Unknown;

        public string GetTcString()
        {
            return string.Empty;
        }

        public string GetAdditionalConsentString()
        {
            return string.Empty;
        }

        public string GetPurposeConsentsString()
        {
            return string.Empty;
        }

        public int GetGdprApplies()
        {
            return -1;
        }

        /// <summary>
        /// Editor stub for Init.
        /// </summary>
        public void Init(UmpInitOptions options, Action onSuccess, Action<UmpError> onFailure)
        {
            if (TryShowPopup("Init", onSuccess, onFailure))
            {
                return;
            }

            onSuccess?.Invoke();
        }

        /// <summary>
        /// Editor stub for Show.
        /// </summary>
        public void Show(Action onDismissed, Action<UmpError> onFailure)
        {
            if (TryShowPopup("Show", onDismissed, onFailure))
            {
                return;
            }

            onDismissed?.Invoke();
        }

        /// <summary>
        /// Editor stub for Reshow.
        /// </summary>
        public void Reshow(Action onDismissed, Action<UmpError> onFailure)
        {
            if (TryShowPopup("Reshow", onDismissed, onFailure))
            {
                return;
            }

            onDismissed?.Invoke();
        }

        /// <summary>
        /// Editor stub for Reset.
        /// </summary>
        public void Reset()
        {
        }

        private static bool TryShowPopup(string operationName, Action onSuccess, Action<UmpError> onFailure)
        {
#if UNITY_EDITOR
            var type = Type.GetType("EasyUmp.Editor.EasyUmpEditorPopup, EasyUmp.Editor");
            if (type == null)
            {
                return false;
            }

            var method = type.GetMethod("Show");
            if (method == null)
            {
                return false;
            }

            var shown = method.Invoke(null, new object[] { operationName, onSuccess, onFailure });
            return shown is bool value && value;
#else
            return false;
#endif
        }
    }
}
