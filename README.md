# Easy UMP (Android)

Lightweight Unity wrapper around Google’s User Messaging Platform (UMP) SDK for **Android only**.  
Designed for Unity **2022 LTS** and **Unity 6 (v6000)**.

## Status

- Android: ✅ Supported
- iOS: ✅ Supported
- UMP SDK (Android): `com.google.android.ump:user-messaging-platform:4.0.0`
- UMP SDK (iOS): `pod 'GoogleUserMessagingPlatform'`

## Install (UPM)

Package name: `techdigga.easy-ump`

Add to `Packages/manifest.json` or a Git URL.

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

Ensure the App Id matches your AdMob App Id in the Google console. This key is required for UMP initialization. citeturn2view0

## Android Minify (ProGuard/R8)

If you enable minification and see consent SDK classes stripped, add keep rules for the consent SDK.  
Community reports suggest keeping `com.google.android.gms.internal.consent_sdk.**`. citeturn1search7

## API (EasyUmp)

- `EasyUmp.Init(options, onSuccess, onFailure)`
- `EasyUmp.Show(onDismissed, onFailure)`
- `EasyUmp.Reshow(onDismissed, onFailure)`
- `EasyUmp.Reset()`
- `EasyUmp.CanRequestAds`
- `EasyUmp.ConsentStatus`
- `EasyUmp.IsSupported`
- `EasyUmp.GetTcString()`
- `EasyUmp.GetAdditionalConsentString()`
- `EasyUmp.GetPurposeConsentsString()`
- `EasyUmp.GetGdprApplies()` (returns `-1` if unknown)

## API Reference (Summary)

| Method / Property | Description | Returns |
| --- | --- | --- |
| `EasyUmp.IsSupported` | Whether UMP is supported on this platform. | `bool` |
| `EasyUmp.CanRequestAds` | Whether ads can be requested per consent state. | `bool` |
| `EasyUmp.ConsentStatus` | Current consent status. | `UmpConsentStatus` |
| `EasyUmp.Init(options, onSuccess, onFailure)` | Initializes UMP and fetches consent info. | `void` |
| `EasyUmp.Show(onDismissed, onFailure)` | Shows consent form if required. | `void` |
| `EasyUmp.Reshow(onDismissed, onFailure)` | Shows privacy options form. | `void` |
| `EasyUmp.Reset()` | Resets locally stored consent info. | `void` |
| `EasyUmp.GetTcString()` | IAB TCF TC String. | `string` |
| `EasyUmp.GetAdditionalConsentString()` | IAB TCF Additional Consent String. | `string` |
| `EasyUmp.GetPurposeConsentsString()` | IAB TCF Purpose Consents String. | `string` |
| `EasyUmp.GetGdprApplies()` | IAB TCF GDPR Applies (`-1` if unknown). | `int` |

## Usage

```csharp
using EasyUmp;

var options = new UmpInitOptions
{
    TagForUnderAgeOfConsent = false,
    DebugGeography = UmpDebugGeography.Disabled,
    TestDeviceHashedIds = { "TEST_DEVICE_HASH" }
};

EasyUmp.Init(
    options,
    onSuccess: () =>
    {
        // If Auto-show is enabled, this is called after the form is shown/dismissed.
        if (EasyUmp.CanRequestAds)
        {
            // Ready to request ads.
        }
    },
    onFailure: error =>
    {
        UnityEngine.Debug.LogError($"UMP init failed: {error.Code} - {error.Message}");
    });

// Manually show (if Auto-show is disabled)
EasyUmp.Show(
    onDismissed: () => { },
    onFailure: error => { UnityEngine.Debug.LogError(error.Message); });

// Show privacy options form
EasyUmp.Reshow(
    onDismissed: () => { },
    onFailure: error => { UnityEngine.Debug.LogError(error.Message); });

// IAB TCF consent strings
var tcString = EasyUmp.GetTcString();
var addtlConsent = EasyUmp.GetAdditionalConsentString();
var purposeConsents = EasyUmp.GetPurposeConsentsString();
var gdprApplies = EasyUmp.GetGdprApplies();
```

## IAB TCF Consent Strings

The UMP SDK writes IAB TCF values into local storage. You can read:

- `IABTCF_TCString`
- `IABTCF_AddtlConsent`
- `IABTCF_PurposeConsents`
- `IABTCF_gdprApplies`

## Notes

- Calls are serialized; overlapping operations return an error with code `-3`.
