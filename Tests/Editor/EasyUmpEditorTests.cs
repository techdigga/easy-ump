using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace EasyUmp.Editor.Tests
{
    public sealed class EasyUmpEditorTests
    {
        private const string RuntimeConfigPath = "Assets/Resources/EasyUmpRuntimeConfig.asset";

        [SetUp]
        public void SetUp()
        {
            AssetDatabase.DeleteAsset(RuntimeConfigPath);
            AssetDatabase.Refresh();
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(RuntimeConfigPath);
            AssetDatabase.Refresh();
        }

        [Test]
        public void SaveSettings_CreatesRuntimeConfigAsset()
        {
            var settings = UmpSettings.instance;
            settings.AutoShow = true;
            settings.DebugLogging = false;
            settings.SaveSettings();

            AssetDatabase.Refresh();

            var asset = AssetDatabase.LoadAssetAtPath<EasyUmpRuntimeConfig>(RuntimeConfigPath);
            Assert.IsNotNull(asset, "Runtime config asset should be created under Assets/Resources.");
            Assert.IsTrue(asset.AutoShow);
            Assert.IsFalse(asset.DebugLogging);
        }

        [Test]
        public void RuntimeConfig_LoadsFromResources()
        {
            var settings = UmpSettings.instance;
            settings.AutoShow = false;
            settings.DebugLogging = true;
            settings.SaveSettings();

            AssetDatabase.Refresh();

            var loaded = EasyUmpRuntimeConfig.Load();
            Assert.IsNotNull(loaded, "Resources.Load should return the runtime config asset.");
            Assert.IsFalse(loaded.AutoShow);
            Assert.IsTrue(loaded.DebugLogging);
        }

        [Test]
        public void EditorImplementation_ReturnsDefaults()
        {
            var editorImpl = new EasyUmpEditor();
            Assert.IsFalse(editorImpl.IsSupported);
            Assert.IsFalse(editorImpl.CanRequestAds);
            Assert.AreEqual(UmpConsentStatus.Unknown, editorImpl.ConsentStatus);
            Assert.AreEqual(string.Empty, editorImpl.GetTcString());
            Assert.AreEqual(string.Empty, editorImpl.GetAdditionalConsentString());
            Assert.AreEqual(string.Empty, editorImpl.GetPurposeConsentsString());
            Assert.AreEqual(-1, editorImpl.GetGdprApplies());
        }

        [Test]
        public void MergeWithRuntimeDefaults_TrimsConsentSyncId()
        {
            var options = new UmpInitOptions
            {
                ConsentSyncId = "  123e4567-e89b-12d3-a456-426614174000  "
            };

            var merged = UmpInitOptionsSanitizer.MergeWithRuntimeDefaults(options);

            Assert.AreEqual("123e4567-e89b-12d3-a456-426614174000", merged.ConsentSyncId);
        }

        [Test]
        public void MergeWithRuntimeDefaults_ClearsWhitespaceOnlyConsentSyncId()
        {
            var options = new UmpInitOptions
            {
                ConsentSyncId = "   \n\t   "
            };

            var merged = UmpInitOptionsSanitizer.MergeWithRuntimeDefaults(options);

            Assert.IsNull(merged.ConsentSyncId);
        }

        [Test]
        public void InitOptionsSerialization_IncludesConsentSyncId()
        {
            var options = new UmpInitOptions
            {
                ConsentSyncId = "123e4567-e89b-12d3-a456-426614174000",
                TestDeviceHashedIds = new List<string>()
            };

            var json = JsonUtility.ToJson(options);

            StringAssert.Contains("\"ConsentSyncId\":\"123e4567-e89b-12d3-a456-426614174000\"", json);
        }
    }
}
