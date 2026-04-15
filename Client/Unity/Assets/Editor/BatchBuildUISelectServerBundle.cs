using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildUISelectServerBundle
    {
        public static void PerformBuild()
        {
            const string assetPath = "Assets/Bundles/UI/UISelectServer.prefab";
            string outputDir = Path.Combine(Application.dataPath, "StreamingAssets");
            string tempDir = Path.Combine(Directory.GetCurrentDirectory(), "Temp", "UISelectServerBundleBuild");

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            Directory.CreateDirectory(tempDir);

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            AssetBundleBuild[] builds =
            {
                new AssetBundleBuild
                {
                    assetBundleName = "uiselectserver.unity3d",
                    assetNames = new[] { assetPath }
                }
            };

            BuildPipeline.BuildAssetBundles(
                tempDir,
                builds,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);

            File.Copy(Path.Combine(tempDir, "uiselectserver.unity3d"), Path.Combine(outputDir, "uiselectserver.unity3d"), true);
            File.Copy(Path.Combine(tempDir, "uiselectserver.unity3d.manifest"), Path.Combine(outputDir, "uiselectserver.unity3d.manifest"), true);

            AssetDatabase.Refresh();
            Debug.Log("UISelectServer bundle build succeeded.");
        }
    }
}
