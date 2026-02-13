using System;
using UnityEngine;

namespace EasyUmp
{
#if UNITY_ANDROID && !UNITY_EDITOR
    /// <summary>
    /// Android implementation that bridges to the native UMP SDK.
    /// </summary>
    internal sealed class EasyUmpAndroid : IUmpClient
    {
        private AndroidJavaClass java;
        private CallbackReceiver receiver;

        private Action onInitSuccess;
        private Action<UmpError> onInitFailure;

        private Action onShowDismissed;
        private Action<UmpError> onShowFailed;

        private Action onReshowDismissed;
        private Action<UmpError> onReshowFailed;

        private bool operationInProgress;

        /// <summary>
        /// Android implementation is supported at runtime.
        /// </summary>
        public bool IsSupported => true;

        /// <summary>
        /// Whether ads can be requested per consent state.
        /// </summary>
        public bool CanRequestAds
        {
            get
            {
                EnsureInitialized();
                return java.CallStatic<bool>(UnityNativeBridgeConstants.CanRequestAdsMethod);
            }
        }

        /// <summary>
        /// Current consent status from native UMP.
        /// </summary>
        public UmpConsentStatus ConsentStatus
        {
            get
            {
                EnsureInitialized();
                return (UmpConsentStatus)java.CallStatic<int>(UnityNativeBridgeConstants.GetConsentStatusMethod);
            }
        }

        /// <summary>
        /// IAB TCF TC String.
        /// </summary>
        public string GetTcString()
        {
            EnsureInitialized();
            return java.CallStatic<string>(UnityNativeBridgeConstants.GetIabTcStringMethod);
        }

        /// <summary>
        /// IAB TCF Additional Consent String.
        /// </summary>
        public string GetAdditionalConsentString()
        {
            EnsureInitialized();
            return java.CallStatic<string>(UnityNativeBridgeConstants.GetIabAddtlConsentMethod);
        }

        /// <summary>
        /// IAB TCF Purpose Consents String.
        /// </summary>
        public string GetPurposeConsentsString()
        {
            EnsureInitialized();
            return java.CallStatic<string>(UnityNativeBridgeConstants.GetIabPurposeConsentsMethod);
        }

        /// <summary>
        /// IAB TCF GDPR Applies value (-1 if unknown).
        /// </summary>
        public int GetGdprApplies()
        {
            EnsureInitialized();
            return java.CallStatic<int>(UnityNativeBridgeConstants.GetIabGdprAppliesMethod);
        }

        /// <summary>
        /// Initializes UMP and requests consent info.
        /// </summary>
        public void Init(UmpInitOptions options, Action onSuccess, Action<UmpError> onFailure)
        {
            if (!TryBeginOperation(onFailure))
            {
                return;
            }

            EnsureInitialized();
            onInitSuccess = onSuccess;
            onInitFailure = onFailure;

            var merged = MergeOptions(options);
            var payload = JsonUtility.ToJson(merged);
            java.CallStatic(UnityNativeBridgeConstants.InitMethod, payload);
        }

        /// <summary>
        /// Shows the consent form if required.
        /// </summary>
        public void Show(Action onDismissed, Action<UmpError> onFailure)
        {
            if (!TryBeginOperation(onFailure))
            {
                return;
            }

            EnsureInitialized();
            onShowDismissed = onDismissed;
            onShowFailed = onFailure;
            java.CallStatic(UnityNativeBridgeConstants.ShowMethod);
        }

        /// <summary>
        /// Shows the privacy options form.
        /// </summary>
        public void Reshow(Action onDismissed, Action<UmpError> onFailure)
        {
            if (!TryBeginOperation(onFailure))
            {
                return;
            }

            EnsureInitialized();
            onReshowDismissed = onDismissed;
            onReshowFailed = onFailure;
            java.CallStatic(UnityNativeBridgeConstants.ReshowMethod);
        }

        /// <summary>
        /// Resets local consent information.
        /// </summary>
        public void Reset()
        {
            EnsureInitialized();
            java.CallStatic(UnityNativeBridgeConstants.ResetMethod);
        }

        /// <summary>
        /// Ensures the Java bridge and callback receiver are initialized.
        /// </summary>
        private void EnsureInitialized()
        {
            if (java != null)
            {
                return;
            }

            java = new AndroidJavaClass(UnityNativeBridgeConstants.AndroidJavaClass);
            var go = new GameObject(UnityNativeBridgeConstants.CallbackObjectName);
            UnityEngine.Object.DontDestroyOnLoad(go);
            receiver = go.AddComponent<CallbackReceiver>();
            receiver.Bind(this);
            java.CallStatic(UnityNativeBridgeConstants.SetUnityCallbackObjectMethod, UnityNativeBridgeConstants.CallbackObjectName);

            var config = EasyUmpRuntimeConfig.Load();
            if (config != null)
            {
                Logger.Enabled = config.DebugLogging;
            }
        }

        private static UmpInitOptions MergeOptions(UmpInitOptions options)
        {
            var merged = options ?? new UmpInitOptions();
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

        private bool TryBeginOperation(Action<UmpError> onFailure)
        {
            if (operationInProgress)
            {
                if (onFailure != null)
                {
                    var error = new UmpError { Code = -3, Message = ErrorMessages.OperationInProgress };
                    MainThreadDispatcher.Post(() => onFailure(error));
                }
                return false;
            }

            operationInProgress = true;
            return true;
        }

        private void EndOperation()
        {
            operationInProgress = false;
        }

        /// <summary>
        /// Receives messages from native via UnitySendMessage.
        /// </summary>
        private sealed class CallbackReceiver : MonoBehaviour
        {
            private EasyUmpAndroid owner;

            /// <summary>
            /// Binds the owning implementation instance.
            /// </summary>
            public void Bind(EasyUmpAndroid instance)
            {
                owner = instance;
            }

            // Called by Android via UnitySendMessage
            /// <summary>
            /// Success callback for Init.
            /// </summary>
            public void OnInitSuccess(string _)
            {
                var cb = owner.onInitSuccess;
                owner.onInitSuccess = null;
                owner.onInitFailure = null;
                owner.EndOperation();

                var config = EasyUmpRuntimeConfig.Load();
                if (config != null && config.AutoShow)
                {
                    cb?.Invoke();
                    owner.Show(
                        onDismissed: () => { },
                        onFailure: error => { Logger.Warning(error.Message); });
                    return;
                }

                if (cb != null)
                {
                    MainThreadDispatcher.Post(cb);
                }
            }

            /// <summary>
            /// Failure callback for Init.
            /// </summary>
            public void OnInitFailure(string json)
            {
                var cb = owner.onInitFailure;
                owner.onInitSuccess = null;
                owner.onInitFailure = null;
                owner.EndOperation();
                if (cb != null)
                {
                    var error = ParseError(json);
                    MainThreadDispatcher.Post(() => cb(error));
                }
            }

            /// <summary>
            /// Dismiss callback for Show.
            /// </summary>
            public void OnShowDismissed(string _)
            {
                var cb = owner.onShowDismissed;
                owner.onShowDismissed = null;
                owner.onShowFailed = null;
                owner.EndOperation();
                if (cb != null)
                {
                    MainThreadDispatcher.Post(cb);
                }
            }

            /// <summary>
            /// Failure callback for Show.
            /// </summary>
            public void OnShowFailed(string json)
            {
                var cb = owner.onShowFailed;
                owner.onShowDismissed = null;
                owner.onShowFailed = null;
                owner.EndOperation();
                if (cb != null)
                {
                    var error = ParseError(json);
                    MainThreadDispatcher.Post(() => cb(error));
                }
            }

            /// <summary>
            /// Dismiss callback for Reshow.
            /// </summary>
            public void OnReshowDismissed(string _)
            {
                var cb = owner.onReshowDismissed;
                owner.onReshowDismissed = null;
                owner.onReshowFailed = null;
                owner.EndOperation();
                if (cb != null)
                {
                    MainThreadDispatcher.Post(cb);
                }
            }

            /// <summary>
            /// Failure callback for Reshow.
            /// </summary>
            public void OnReshowFailed(string json)
            {
                var cb = owner.onReshowFailed;
                owner.onReshowDismissed = null;
                owner.onReshowFailed = null;
                owner.EndOperation();
                if (cb != null)
                {
                    var error = ParseError(json);
                    MainThreadDispatcher.Post(() => cb(error));
                }
            }

            /// <summary>
            /// Parses a native error payload.
            /// </summary>
            private static UmpError ParseError(string json)
            {
                if (string.IsNullOrEmpty(json))
                {
                    return new UmpError { Code = -2, Message = ErrorMessages.UnknownUmpError };
                }

                try
                {
                    return JsonUtility.FromJson<UmpError>(json);
                }
                catch
                {
                    return new UmpError { Code = -2, Message = ErrorMessages.MalformedUmpError };
                }
            }
        }
    }
#endif
}
