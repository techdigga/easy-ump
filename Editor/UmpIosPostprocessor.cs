using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace EasyUmp.Editor
{
    /// <summary>
    /// Injects iOS Info.plist keys and CocoaPods dependencies.
    /// </summary>
    internal static class UmpIosPostprocessor
    {
#if UNITY_IOS
        private const string PodLine = "  pod 'GoogleUserMessagingPlatform'";

        [PostProcessBuild(1000)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            UpdateInfoPlist(pathToBuiltProject);
        }

        private static void UpdateInfoPlist(string pathToBuiltProject)
        {
            var appId = UmpSettings.instance.IosAppId;
            if (string.IsNullOrWhiteSpace(appId))
            {
                Logger.Error(LogMessages.IosAppIdMissing);
                throw new BuildFailedException(LogMessages.IosAppIdMissing);
            }

            var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            plist.root.SetString(EasyUmpConstants.IosInfoPlistKey, appId);
            File.WriteAllText(plistPath, plist.WriteToString());
        }

        // CocoaPods dependency is handled via EDM4U (External Dependency Manager for Unity).
#endif
    }
}
