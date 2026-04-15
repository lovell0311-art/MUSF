using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildCodeBundleToTemp
    {
        public static void PerformBuild()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDir = Path.Combine(projectRoot, "Temp", "CodeBundleBuild");
            string exportDir = Path.Combine(projectRoot, "BuildOutput", "CodeBundleBuild");
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }

            Directory.CreateDirectory(outputDir);
            Directory.CreateDirectory(exportDir);

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            AssetDatabase.Refresh();

            AssetBundleBuild[] builds =
            {
                new AssetBundleBuild
                {
                    assetBundleName = "code.unity3d",
                    assetNames = new[] { "Assets/Bundles/Independent/Code.prefab" }
                }
            };

            BuildPipeline.BuildAssetBundles(
                outputDir,
                builds,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);

            File.Copy(Path.Combine(outputDir, "code.unity3d"), Path.Combine(exportDir, "code.unity3d"), true);
            File.Copy(Path.Combine(outputDir, "code.unity3d.manifest"), Path.Combine(exportDir, "code.unity3d.manifest"), true);

            AssetDatabase.Refresh();
            Debug.Log($"Temp code bundle build succeeded: {outputDir} exists={File.Exists(Path.Combine(outputDir, "code.unity3d"))} export={exportDir}");
        }
    }
}
