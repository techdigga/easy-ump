using UnityEditor;

namespace EasyUmp.Editor
{
    /// <summary>
    /// Menu shortcuts for Easy UMP.
    /// </summary>
    internal static class EasyUmpMenu
    {
        [MenuItem("Easy UMP/Settings", priority = 1)]
        public static void OpenSettings()
        {
            SettingsService.OpenProjectSettings(EasyUmpConstants.SettingsPath);
        }
    }
}
