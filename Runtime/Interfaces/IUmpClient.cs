using System;

namespace EasyUmp
{
    /// <summary>
    /// Platform abstraction for UMP functionality.
    /// </summary>
    public interface IUmpClient
    {
        /// <summary>
        /// Whether the current runtime supports UMP.
        /// </summary>
        /// <returns>True when running on a supported platform.</returns>
        bool IsSupported { get; }
        /// <summary>
        /// Whether ads can be requested per consent state.
        /// </summary>
        /// <returns>True if ads can be requested.</returns>
        bool CanRequestAds { get; }
        /// <summary>
        /// Current UMP consent status.
        /// </summary>
        /// <returns>Consent status enum value.</returns>
        UmpConsentStatus ConsentStatus { get; }
        /// <summary>
        /// IAB TCF TC String.
        /// </summary>
        /// <returns>TC string or empty if unavailable.</returns>
        string GetTcString();
        /// <summary>
        /// IAB TCF Additional Consent String.
        /// </summary>
        /// <returns>Additional consent string or empty if unavailable.</returns>
        string GetAdditionalConsentString();
        /// <summary>
        /// IAB TCF Purpose Consents String.
        /// </summary>
        /// <returns>Purpose consents string or empty if unavailable.</returns>
        string GetPurposeConsentsString();
        /// <summary>
        /// IAB TCF GDPR Applies value (-1 if unknown).
        /// </summary>
        /// <returns>0/1 for applies, or -1 if unknown.</returns>
        int GetGdprApplies();
        /// <summary>
        /// Initializes UMP and fetches consent info.
        /// </summary>
        /// <param name="options">Initialization options (may be null).</param>
        /// <param name="onSuccess">Invoked when consent info update succeeds.</param>
        /// <param name="onFailure">Invoked with error when init fails.</param>
        void Init(UmpInitOptions options, Action onSuccess, Action<UmpError> onFailure);
        /// <summary>
        /// Shows the consent form if required.
        /// </summary>
        /// <param name="onDismissed">Invoked when the form is dismissed or not required.</param>
        /// <param name="onFailure">Invoked with error when show fails.</param>
        void Show(Action onDismissed, Action<UmpError> onFailure);
        /// <summary>
        /// Shows the privacy options form.
        /// </summary>
        /// <param name="onDismissed">Invoked when the form is dismissed.</param>
        /// <param name="onFailure">Invoked with error when show fails.</param>
        void Reshow(Action onDismissed, Action<UmpError> onFailure);
        /// <summary>
        /// Resets locally stored consent info.
        /// </summary>
        void Reset();
    }
}
