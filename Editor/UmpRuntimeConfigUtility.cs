using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyUmp.Editor
{
    /// <summary>
    /// Keeps a runtime config asset in sync with editor settings.
    /// </summary>
    internal static class UmpRuntimeConfigUtility
    {
        private const string RuntimeResourcesPath = "Assets/Resources";
        private const string AssetPath = RuntimeResourcesPath + "/EasyUmpRuntimeConfig.asset";

        public static void SyncFromSettings(UmpSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            var asset = AssetDatabase.LoadAssetAtPath<EasyUmpRuntimeConfig>(AssetPath);
            if (asset == null)
            {
                EnsureResourcesFolder();
                asset = ScriptableObject.CreateInstance<EasyUmpRuntimeConfig>();
                AssetDatabase.CreateAsset(asset, AssetPath);
            }

            asset.AutoShow = settings.AutoShow;
            asset.DebugLogging = settings.DebugLogging;
            asset.TestDeviceHashedIds = settings.TestDeviceHashedIds.ToArray();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureResourcesFolder()
        {
            if (AssetDatabase.IsValidFolder(RuntimeResourcesPath))
            {
                return;
            }

            var parts = RuntimeResourcesPath.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }
    }
}
