# Changelog

## 1.0.4

- Add `Runtime/Examples/UmpFlowExample.cs` with a complete runtime flow:
  - `Init`
  - `Show`
  - consent value reads
  - delayed `Reshow`
- Document the example script in `README.md`.

## 1.0.3

- Fix Android editor popup reflection ambiguity (`AmbiguousMatchException`).
- Add AGP 8 namespace in Android library Gradle config.
- Add Android network permissions in plugin manifest (`INTERNET`, `ACCESS_NETWORK_STATE`).
- Add explicit Android dependencies for stable runtime/build compatibility:
  - `com.google.android.ump:user-messaging-platform:4.0.0`
  - `androidx.core:core:1.12.0`
  - `compileOnly` Unity classes jar reference for `UnityPlayer`.
- Add more runtime debug logs across init/show/reshow/reset and callback lifecycle.

## 1.0.2

- Fix iOS postprocessor missing System.IO.
- Fix Android manifest namespace reference.
- Fix test device IDs access in settings UI.
- Add InternalsVisibleTo for tests.
- Rename API class to UmpClient (avoid EasyUmp.EasyUmp usage).
- Fix EditorPopupMode accessibility.
- Rename Test Devices section label.
- Add Unity .meta files for immutable installs.

## 1.0.1

- Add OpenUPM badge and install instructions.

## 1.0.0

- Initial public release.
