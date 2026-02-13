using UnityEditor;
using UnityEngine;

namespace EasyUmp.Editor
{
    /// <summary>
    /// Project settings asset for Easy UMP.
    /// </summary>
    [FilePath(EasyUmpConstants.SettingsFilePath, FilePathAttribute.Location.ProjectFolder)]
    public sealed class UmpSettings : ScriptableSingleton<UmpSettings>
    {
        [SerializeField] private string androidAppId;
        [SerializeField] private string iosAppId;
        [SerializeField] private bool debugLogging = true;
        [SerializeField] private EditorPopupMode editorPopupMode = EditorPopupMode.Always;
        [SerializeField] private string testDeviceHashedIds = "";

        public string AndroidAppId
        {
            get => androidAppId;
            set => androidAppId = value;
        }

        /// <summary>
        /// iOS AdMob Application Id for Info.plist.
        /// </summary>
        public string IosAppId
        {
            get => iosAppId;
            set => iosAppId = value;
        }

        /// <summary>
        /// Enables or disables EasyUmp logging.
        /// </summary>
        public bool DebugLogging
        {
            get => debugLogging;
            set => debugLogging = value;
        }

        /// <summary>
        /// Controls when the editor simulation popup appears.
        /// </summary>
        public EditorPopupMode EditorPopupMode
        {
            get => editorPopupMode;
            set => editorPopupMode = value;
        }

        /// <summary>
        /// Comma or newline separated list of test device hashed ids.
        /// </summary>
        public System.Collections.Generic.List<string> TestDeviceHashedIds
        {
            get
            {
                var list = new System.Collections.Generic.List<string>();
                if (string.IsNullOrWhiteSpace(testDeviceHashedIds))
                {
                    return list;
                }

                var split = testDeviceHashedIds.Split(new[] { ',', '\n', '\r', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in split)
                {
                    var trimmed = item.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        list.Add(trimmed);
                    }
                }

                return list;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    testDeviceHashedIds = string.Empty;
                    return;
                }

                testDeviceHashedIds = string.Join("\n", value);
            }
        }

        internal string TestDeviceHashedIdsRaw
        {
            get => testDeviceHashedIds;
            set => testDeviceHashedIds = value ?? string.Empty;
        }

        [SerializeField] private bool autoShow;

        /// <summary>
        /// Enables auto-show after successful init.
        /// </summary>
        public bool AutoShow
        {
            get => autoShow;
            set => autoShow = value;
        }

        public void SaveSettings()
        {
            Save(true);
            UmpRuntimeConfigUtility.SyncFromSettings(this);
        }
    }

    /// <summary>
    /// Project settings UI for Easy UMP.
    /// </summary>
    public static class UmpSettingsProvider
    {
        private const string SettingsPath = EasyUmpConstants.SettingsPath;

        /// <summary>
        /// Registers the settings provider.
        /// </summary>
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new SettingsProvider(SettingsPath, SettingsScope.Project)
            {
                label = EasyUmpConstants.SettingsLabel,
                guiHandler = _ =>
                {
                    var settings = UmpSettings.instance;
                    EditorGUILayout.LabelField(EasyUmpConstants.SettingsAndroidSection, EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    settings.AndroidAppId = EditorGUILayout.TextField(
                        new GUIContent(
                            EasyUmpConstants.AdMobAppIdLabel,
                            EasyUmpConstants.AdMobAppIdTooltip),
                        settings.AndroidAppId);
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField(EasyUmpConstants.SettingsIosSection, EditorStyles.boldLabel);
                    settings.IosAppId = EditorGUILayout.TextField(
                        new GUIContent(
                            EasyUmpConstants.AdMobAppIdIosLabel,
                            EasyUmpConstants.AdMobAppIdIosTooltip),
                        settings.IosAppId);
                    EditorGUILayout.Space(4);
                    settings.DebugLogging = EditorGUILayout.Toggle(
                        new GUIContent(EasyUmpConstants.DebugLogsLabel, EasyUmpConstants.DebugLogsTooltip),
                        settings.DebugLogging);
                    settings.AutoShow = EditorGUILayout.Toggle(
                        new GUIContent(EasyUmpConstants.AutoShowLabel, EasyUmpConstants.AutoShowTooltip),
                        settings.AutoShow);
                    settings.EditorPopupMode = (EditorPopupMode)EditorGUILayout.EnumPopup(
                        new GUIContent("Editor Popup", "Simulate callbacks in the Editor."),
                        settings.EditorPopupMode);
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField("Testing", EditorStyles.boldLabel);
                    settings.TestDeviceHashedIdsRaw = EditorGUILayout.TextArea(
                        settings.TestDeviceHashedIdsRaw,
                        GUILayout.MinHeight(60));
                    EditorGUILayout.HelpBox("Enter test device hashed IDs (one per line or comma-separated).", MessageType.Info);
                    if (EditorGUI.EndChangeCheck())
                    {
                        settings.SaveSettings();
                        Logger.Enabled = settings.DebugLogging;
                    }
                }
            };

            return provider;
        }
    }

    /// <summary>
    /// Applies settings defaults on editor load.
    /// </summary>
    [InitializeOnLoad]
    internal static class UmpSettingsBootstrap
    {
        static UmpSettingsBootstrap()
        {
            Logger.Enabled = UmpSettings.instance.DebugLogging;
            UmpRuntimeConfigUtility.SyncFromSettings(UmpSettings.instance);
        }
    }
}
