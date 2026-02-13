# Easy UMP

[![openupm](https://img.shields.io/npm/v/com.techdigga.easy-ump?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.techdigga.easy-ump/)

Lightweight Unity wrapper around Google’s User Messaging Platform (UMP) SDK for **Android and iOS**.  
Designed for Unity **2022 LTS** and **Unity 6 (v6000)**.

## Quick Start

1. Install the package.
2. Set App IDs in `Project Settings > Easy UMP`.
3. Call `UmpClient.Init(...)` and show the form as needed.

## Why This Package?

- The official UMP Unity plugin ships inside **GoogleMobileAds**, so you must import the full ads SDK even if you only want consent.
- Mediation SDKs (e.g., MAX) often bundle UMP but may lag updates and limit control over the consent flow lifecycle.
- This package is **UMP‑only**, gives you **full control** over when/if you show the form, and exposes **all IAB TCF consent strings**.

## Status

- Android: ✅ Supported
- iOS: ✅ Supported
- UMP SDK (Android): `com.google.android.ump:user-messaging-platform:4.0.0`
- UMP SDK (iOS): `pod 'GoogleUserMessagingPlatform'`

## Install (UPM)

Package name: `com.techdigga.easy-ump`

Add to `Packages/manifest.json` or a Git URL.

### Install via OpenUPM

1. Add the OpenUPM registry:
   - Name: OpenUPM
   - URL: https://package.openupm.com
2. Install package:
   - `com.techdigga.easy-ump`

Or via CLI:
```
openupm add com.techdigga.easy-ump
```

## Setup

1. **Set AdMob Application Ids**  
   Unity: `Project Settings > Easy UMP`  
   - **Android** injects:
   ```
   <meta-data android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="YOUR_APP_ID" />
   ```
   - **iOS** injects:
   ```
   <key>GADApplicationIdentifier</key>
   <string>ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy</string>
   ```

2. **Optional settings**  
   - **Enable Debug Logs**: toggles `easy-ump` tagged logs
   - **Auto-show Consent Form**: if enabled, `Init` will show the form automatically after success
   - **Test Device Hashed IDs**: add one per line or comma-separated (used if not provided in code)
   - **Editor Popup**: enables a simple editor mock to test callbacks (Success/Fail)

3. **Dependencies (EDM4U)**  
   This package ships an EDM4U dependency file:
   `Editor/EasyUmpDependencies.xml`  
   Make sure External Dependency Manager for Unity (EDM4U) is installed so the UMP
   Android and iOS dependencies are resolved automatically.  
   https://github.com/googlesamples/unity-jar-resolver

## Build Steps (Quick)

**Android**
1. Open `Project Settings > Easy UMP` and set the Android App Id.
2. Ensure EDM4U resolves dependencies.
3. Build as usual.

**iOS**
1. Open `Project Settings > Easy UMP` and set the iOS App Id.
2. Ensure EDM4U resolves dependencies (CocoaPods).
3. Build for iOS, open the Xcode project, and run.

## AdMob Dashboard

Follow Google’s UMP setup guide in the AdMob dashboard to configure GDPR/consent messaging:
https://developers.google.com/admob/ump/android/quick-start

## iOS Info.plist

Ensure the App Id matches your AdMob App Id in the Google console. This key is required for UMP initialization.

## Android Minify (ProGuard/R8)

If you enable minification and see consent SDK classes stripped, add keep rules for the consent SDK.  
Community reports suggest keeping `com.google.android.gms.internal.consent_sdk.**`.

## API (UmpClient)

- `UmpClient.Init(options, onSuccess, onFailure)`
- `UmpClient.Show(onDismissed, onFailure)`
- `UmpClient.Reshow(onDismissed, onFailure)`
- `UmpClient.Reset()`
- `UmpClient.CanRequestAds`
- `UmpClient.ConsentStatus`
- `UmpClient.IsSupported`
- `UmpClient.GetTcString()`
- `UmpClient.GetAdditionalConsentString()`
- `UmpClient.GetPurposeConsentsString()`
- `UmpClient.GetGdprApplies()` (returns `-1` if unknown)

## API Reference (Summary)

| Method / Property | Description | Returns |
| --- | --- | --- |
| `UmpClient.IsSupported` | Whether UMP is supported on this platform. | `bool` |
| `UmpClient.CanRequestAds` | Whether ads can be requested per consent state. | `bool` |
| `UmpClient.ConsentStatus` | Current consent status. | `UmpConsentStatus` |
| `UmpClient.Init(options, onSuccess, onFailure)` | Initializes UMP and fetches consent info. | `void` |
| `UmpClient.Show(onDismissed, onFailure)` | Shows consent form if required. | `void` |
| `UmpClient.Reshow(onDismissed, onFailure)` | Shows privacy options form. | `void` |
| `UmpClient.Reset()` | Resets locally stored consent info. | `void` |
| `UmpClient.GetTcString()` | IAB TCF TC String. | `string` |
| `UmpClient.GetAdditionalConsentString()` | IAB TCF Additional Consent String. | `string` |
| `UmpClient.GetPurposeConsentsString()` | IAB TCF Purpose Consents String. | `string` |
| `UmpClient.GetGdprApplies()` | IAB TCF GDPR Applies (`-1` if unknown). | `int` |

## Usage

```csharp
using EasyUmp;

var options = new UmpInitOptions
{
    TagForUnderAgeOfConsent = false,
    DebugGeography = UmpDebugGeography.Disabled,
    TestDeviceHashedIds = { "TEST_DEVICE_HASH" }
};

UmpClient.Init(
    options,
    onSuccess: () =>
    {
        // If Auto-show is enabled, this is called after the form is shown/dismissed.
        if (UmpClient.CanRequestAds)
        {
            // Ready to request ads.
        }
    },
    onFailure: error =>
    {
        UnityEngine.Debug.LogError($"UMP init failed: {error.Code} - {error.Message}");
    });

// Manually show (if Auto-show is disabled)
UmpClient.Show(
    onDismissed: () => { },
    onFailure: error => { UnityEngine.Debug.LogError(error.Message); });

// Show privacy options form
UmpClient.Reshow(
    onDismissed: () => { },
    onFailure: error => { UnityEngine.Debug.LogError(error.Message); });

// IAB TCF consent strings
var tcString = UmpClient.GetTcString();
var addtlConsent = UmpClient.GetAdditionalConsentString();
var purposeConsents = UmpClient.GetPurposeConsentsString();
var gdprApplies = UmpClient.GetGdprApplies();
```

## IAB TCF Consent Strings

The UMP SDK writes IAB TCF values into local storage. You can read:

- `IABTCF_TCString`
- `IABTCF_AddtlConsent`
- `IABTCF_PurposeConsents`
- `IABTCF_gdprApplies`

## Notes

- Calls are serialized; overlapping operations return an error with code `-3`.
