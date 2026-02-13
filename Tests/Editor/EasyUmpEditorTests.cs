using NUnit.Framework;
using UnityEditor;
using UnityEngine;

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
    }
}
