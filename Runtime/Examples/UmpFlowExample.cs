using UnityEngine;
using System.Collections;

namespace EasyUmp
{
    /// <summary>
    /// Minimal end-to-end UMP flow example:
    /// Init -> Show -> read consent strings -> Reshow.
    /// </summary>
    public sealed class UmpFlowExample : MonoBehaviour
    {
        [Header("Init Options")]
        [SerializeField] private bool tagForUnderAgeOfConsent;
        [SerializeField] private UmpDebugGeography debugGeography = UmpDebugGeography.Disabled;
        [SerializeField] private string[] testDeviceHashedIds;
        [SerializeField] private string consentSyncId;

        [Header("Flow")]
        [SerializeField] private bool callReshowAfterShow = true;
        [SerializeField] private float reshowDelaySeconds = 1f;

        private void Start()
        {
            if (!UmpClient.IsSupported)
            {
                Debug.LogWarning("[easy-ump] UMP is not supported on this platform.");
                return;
            }

            var options = new UmpInitOptions
            {
                TagForUnderAgeOfConsent = tagForUnderAgeOfConsent,
                DebugGeography = debugGeography,
                // Beta feature. Hash or encrypt before assigning.
                ConsentSyncId = consentSyncId
            };

            if (testDeviceHashedIds != null)
            {
                foreach (var id in testDeviceHashedIds)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        options.TestDeviceHashedIds.Add(id.Trim());
                    }
                }
            }

            UmpClient.Init(
                options,
                onSuccess: OnInitSuccess,
                onFailure: OnInitFailure);
        }

        private void OnInitSuccess()
        {
            Debug.Log("[easy-ump] Init success.");

            UmpClient.Show(
                onDismissed: OnShowDismissed,
                onFailure: error => Debug.LogError($"[easy-ump] Show failed: {error.Code} - {error.Message}"));
        }

        private void OnInitFailure(UmpError error)
        {
            Debug.LogError($"[easy-ump] Init failed: {error.Code} - {error.Message}");
        }

        private void OnShowDismissed()
        {
            Debug.Log("[easy-ump] Show dismissed.");
            DumpConsentValues();

            if (!callReshowAfterShow)
            {
                return;
            }

            StartCoroutine(CallReshowWithDelay());
        }

        private IEnumerator CallReshowWithDelay()
        {
            if (reshowDelaySeconds > 0f)
            {
                yield return new WaitForSeconds(reshowDelaySeconds);
            }

            UmpClient.Reshow(
                onDismissed: () =>
                {
                    Debug.Log("[easy-ump] Reshow dismissed.");
                    DumpConsentValues();
                },
                onFailure: error => Debug.LogError($"[easy-ump] Reshow failed: {error.Code} - {error.Message}"));
        }

        private static void DumpConsentValues()
        {
            Debug.Log($"[easy-ump] ConsentStatus: {UmpClient.ConsentStatus}");
            Debug.Log($"[easy-ump] CanRequestAds: {UmpClient.CanRequestAds}");
            Debug.Log($"[easy-ump] IABTCF_TCString: {UmpClient.GetTcString()}");
            Debug.Log($"[easy-ump] IABTCF_AddtlConsent: {UmpClient.GetAdditionalConsentString()}");
            Debug.Log($"[easy-ump] IABTCF_PurposeConsents: {UmpClient.GetPurposeConsentsString()}");
            Debug.Log($"[easy-ump] IABTCF_gdprApplies: {UmpClient.GetGdprApplies()}");
        }
    }
}
