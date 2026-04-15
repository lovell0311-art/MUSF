using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildUIHintBundle
    {
        public static void PerformBuild()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDir = Path.Combine(Application.dataPath, "StreamingAssets");
            string tempDir = Path.Combine(projectRoot, "Temp", "UIHintBundleBuild");

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
                    assetBundleName = "uihint.unity3d",
                    assetNames = new[] { "Assets/Bundles/UI/UIHint.prefab" }
                }
            };

            BuildPipeline.BuildAssetBundles(
                tempDir,
                builds,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);

            File.Copy(Path.Combine(tempDir, "uihint.unity3d"), Path.Combine(outputDir, "uihint.unity3d"), true);
            File.Copy(Path.Combine(tempDir, "uihint.unity3d.manifest"), Path.Combine(outputDir, "uihint.unity3d.manifest"), true);

            AssetDatabase.Refresh();
            Debug.Log($"UIHint bundle build succeeded. temp={tempDir} output={outputDir}");
        }
    }
}
