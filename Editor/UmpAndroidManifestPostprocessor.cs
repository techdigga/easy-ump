using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using EasyUmp;

namespace EasyUmp.Editor
{
    /// <summary>
    /// Injects the AdMob Application Id into the generated Android manifest.
    /// </summary>
    public sealed class UmpAndroidManifestPostprocessor : IPostGenerateGradleAndroidProject
    {
        /// <summary>
        /// Postprocess order (runs late).
        /// </summary>
        public int callbackOrder => 1000;

        /// <summary>
        /// Applies manifest updates after Gradle project generation.
        /// </summary>
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var appId = UmpSettings.instance.AndroidAppId;
            if (string.IsNullOrWhiteSpace(appId))
            {
                Logger.Error(LogMessages.AndroidAppIdMissing);
                throw new BuildFailedException(LogMessages.AndroidAppIdMissing);
            }

            var manifestPaths = new[]
            {
                Path.Combine(path, EasyUmpConstants.LauncherManifestRelativePath),
                Path.Combine(path, EasyUmpConstants.DefaultManifestRelativePath)
            };

            var updated = false;
            foreach (var manifestPath in manifestPaths)
            {
                if (!File.Exists(manifestPath))
                {
                    continue;
                }

                try
                {
                    UpdateManifest(manifestPath, appId);
                    updated = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format(LogMessages.ManifestUpdateFailed, manifestPath, ex));
                }
            }

            if (!updated)
            {
                Logger.Warning(LogMessages.ManifestNotFound);
            }
        }

        /// <summary>
        /// Adds or updates the UMP meta-data tag in the given manifest.
        /// </summary>
        private static void UpdateManifest(string manifestPath, string appId)
        {
            var doc = new XmlDocument();
            doc.Load(manifestPath);

            var manifest = doc.SelectSingleNode("/manifest") as XmlElement;
            if (manifest == null)
            {
                throw new InvalidOperationException("Missing <manifest> element.");
            }

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("android", EasyUmpConstants.AndroidXmlNamespace);

            var application = doc.SelectSingleNode("/manifest/application") as XmlElement;
            if (application == null)
            {
                application = doc.CreateElement("application");
                manifest.AppendChild(application);
            }

            var metaData = application.SelectSingleNode(
                EasyUmpConstants.MetaDataXPath,
                nsmgr) as XmlElement;

            if (metaData == null)
            {
                metaData = doc.CreateElement("meta-data");
                application.AppendChild(metaData);
            }

            metaData.SetAttribute("name", EasyUmpConstants.AndroidXmlNamespace, EasyUmpConstants.AdMobAppIdMetaDataKey);
            metaData.SetAttribute("value", EasyUmpConstants.AndroidXmlNamespace, appId);

            doc.Save(manifestPath);
        }
    }
}
