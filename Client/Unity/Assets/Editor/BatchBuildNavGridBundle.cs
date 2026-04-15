using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildNavGridBundle
    {
        public static void PerformBuild()
        {
            const string outputDir = "Assets/StreamingAssets";
            const string tempDir = "Temp/NavGridBundleBuild";
            const string prefabPath = "Assets/Bundles/Independent/NavGridData.prefab";

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            Directory.CreateDirectory(tempDir);

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            AssetDatabase.Refresh();

            AssetBundleBuild[] builds =
            {
                new AssetBundleBuild
                {
                    assetBundleName = "navgriddata.unity3d",
                    assetNames = new[] { prefabPath }
                }
            };

            BuildPipeline.BuildAssetBundles(tempDir, builds, BuildAssetBundleOptions.None, BuildTarget.Android);

            File.Copy(Path.Combine(tempDir, "navgriddata.unity3d"), Path.Combine(outputDir, "navgriddata.unity3d"), true);
            File.Copy(Path.Combine(tempDir, "navgriddata.unity3d.manifest"), Path.Combine(outputDir, "navgriddata.unity3d.manifest"), true);

            AssetDatabase.Refresh();
            Debug.Log("NavGrid bundle build succeeded.");
        }
    }
}
