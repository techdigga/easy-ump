using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace EasyUmp
{
#if UNITY_IOS && !UNITY_EDITOR
    /// <summary>
    /// iOS implementation that bridges to the native UMP SDK.
    /// </summary>
    internal sealed class EasyUmpIos : IUmpClient
    {
        private Action onInitSuccess;
        private Action<UmpError> onInitFailure;

        private Action onShowDismissed;
        private Action<UmpError> onShowFailed;

        private Action onReshowDismissed;
        private Action<UmpError> onReshowFailed;

        private bool operationInProgress;
        private readonly CallbackReceiver receiver;

        public EasyUmpIos()
        {
            var go = new GameObject(UnityNativeBridgeConstants.CallbackObjectName);
            UnityEngine.Object.DontDestroyOnLoad(go);
            receiver = go.AddComponent<CallbackReceiver>();
            receiver.Bind(this);

            var config = EasyUmpRuntimeConfig.Load();
            if (config != null)
            {
                Logger.Enabled = config.DebugLogging;
            }
        }

        /// <summary>
        /// iOS implementation is supported at runtime.
        /// </summary>
        public bool IsSupported => true;

        /// <summary>
        /// Whether ads can be requested per consent state.
        /// </summary>
        public bool CanRequestAds => EasyUmpIosBridgeCanRequestAds();

        /// <summary>
        /// Current consent status from native UMP.
        /// </summary>
        public UmpConsentStatus ConsentStatus => (UmpConsentStatus)EasyUmpIosBridgeGetConsentStatus();

        /// <summary>
        /// IAB TCF TC String.
        /// </summary>
        public string GetTcString() => PtrToStringAndFree(EasyUmpIosBridgeGetTcString());

        /// <summary>
        /// IAB TCF Additional Consent String.
        /// </summary>
        public string GetAdditionalConsentString() => PtrToStringAndFree(EasyUmpIosBridgeGetAdditionalConsentString());

        /// <summary>
        /// IAB TCF Purpose Consents String.
        /// </summary>
        public string GetPurposeConsentsString() => PtrToStringAndFree(EasyUmpIosBridgeGetPurposeConsentsString());

        /// <summary>
        /// IAB TCF GDPR Applies value (-1 if unknown).
        /// </summary>
        public int GetGdprApplies() => EasyUmpIosBridgeGetGdprApplies();

        /// <summary>
        /// Initializes UMP and requests consent info.
        /// </summary>
        public void Init(UmpInitOptions options, Action onSuccess, Action<UmpError> onFailure)
        {
            if (!TryBeginOperation(onFailure))
            {
                return;
            }

            onInitSuccess = onSuccess;
            onInitFailure = onFailure;

            var merged = MergeOptions(options);
            var payload = JsonUtility.ToJson(merged);
            EasyUmpIosBridgeInit(payload);
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

            onShowDismissed = onDismissed;
            onShowFailed = onFailure;
            EasyUmpIosBridgeShow();
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

            onReshowDismissed = onDismissed;
            onReshowFailed = onFailure;
            EasyUmpIosBridgeReshow();
        }

        /// <summary>
        /// Resets local consent information.
        /// </summary>
        public void Reset()
        {
            EasyUmpIosBridgeReset();
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

        private sealed class CallbackReceiver : MonoBehaviour
        {
            private EasyUmpIos owner;

            public void Bind(EasyUmpIos instance)
            {
                owner = instance;
            }

            public void OnInitSuccess(string _)
            {
                var cb = owner.onInitSuccess;
                owner.onInitSuccess = null;
                owner.onInitFailure = null;
                owner.EndOperation();

                var config = EasyUmpRuntimeConfig.Load();
                if (config != null)
                {
                    Logger.Enabled = config.DebugLogging;
                }

                if (config != null && config.AutoShow)
                {
                    cb?.Invoke();
                    owner.Show(
                        onDismissed: () => { },
                        onFailure: error => { Logger.Warning(error.Message); });
                    return;
                }

                cb?.Invoke();
            }

            public void OnInitFailure(string json)
            {
                var cb = owner.onInitFailure;
                owner.onInitSuccess = null;
                owner.onInitFailure = null;
                owner.EndOperation();
                cb?.Invoke(ParseError(json));
            }

            public void OnShowDismissed(string _)
            {
                var cb = owner.onShowDismissed;
                owner.onShowDismissed = null;
                owner.onShowFailed = null;
                owner.EndOperation();
                cb?.Invoke();
            }

            public void OnShowFailed(string json)
            {
                var cb = owner.onShowFailed;
                owner.onShowDismissed = null;
                owner.onShowFailed = null;
                owner.EndOperation();
                cb?.Invoke(ParseError(json));
            }

            public void OnReshowDismissed(string _)
            {
                var cb = owner.onReshowDismissed;
                owner.onReshowDismissed = null;
                owner.onReshowFailed = null;
                owner.EndOperation();
                cb?.Invoke();
            }

            public void OnReshowFailed(string json)
            {
                var cb = owner.onReshowFailed;
                owner.onReshowDismissed = null;
                owner.onReshowFailed = null;
                owner.EndOperation();
                cb?.Invoke(ParseError(json));
            }

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

        [DllImport("__Internal")]
        private static extern void EasyUmpIosBridgeInit(string optionsJson);

        [DllImport("__Internal")]
        private static extern void EasyUmpIosBridgeShow();

        [DllImport("__Internal")]
        private static extern void EasyUmpIosBridgeReshow();

        [DllImport("__Internal")]
        private static extern void EasyUmpIosBridgeReset();

        [DllImport("__Internal")]
        private static extern int EasyUmpIosBridgeGetConsentStatus();

        [DllImport("__Internal")]
        private static extern bool EasyUmpIosBridgeCanRequestAds();

        [DllImport("__Internal")]
        private static extern IntPtr EasyUmpIosBridgeGetTcString();

        [DllImport("__Internal")]
        private static extern IntPtr EasyUmpIosBridgeGetAdditionalConsentString();

        [DllImport("__Internal")]
        private static extern IntPtr EasyUmpIosBridgeGetPurposeConsentsString();

        [DllImport("__Internal")]
        private static extern int EasyUmpIosBridgeGetGdprApplies();

        [DllImport("__Internal")]
        private static extern void EasyUmpIosBridgeFree(IntPtr ptr);

        private static string PtrToStringAndFree(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return string.Empty;
            }

            var value = Marshal.PtrToStringUTF8(ptr) ?? string.Empty;
            EasyUmpIosBridgeFree(ptr);
            return value;
        }
    }
#endif
}
