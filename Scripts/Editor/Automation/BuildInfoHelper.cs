using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class BuildInfoHelper
    {
        #region Const

        private const string BuildInfoAssetPath = "Assets/Resources/BuildInfo.asset";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Shovel/Build System/Create Default Build Info")]
        private static void CreateDefaultBuildInfo()
        {
            CreateBuildInfo(new AutomationParameters(new Dictionary<string, string>()
            {
                { "buildVersion", "v0.0.0.0-local" },
                { "buildCommit", "none" },
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        private static void CreateBuildInfo(AutomationParameters parameters)
        {
            var version = parameters.Get("buildVersion");
            var commit = parameters.Get("buildCommit");

            var buildInfo = ScriptableObject.CreateInstance<BuildInfo>();
            buildInfo.version = version;
            buildInfo.commit = commit;

            DeleteBuildInfo();
            UnityEditor.AssetDatabase.CreateAsset(buildInfo, BuildInfoAssetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log($"Created BuildInfo file for version '{version}'.");
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DeleteBuildInfo()
        {
            UnityEditor.AssetDatabase.DeleteAsset(BuildInfoAssetPath);

            Debug.Log($"Deleted BuildInfo file.");
        }

        #endregion
    }
}