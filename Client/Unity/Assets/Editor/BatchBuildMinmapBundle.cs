using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildMinmapBundle
    {
        public static void PerformBuild()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string tempDir = Path.Combine(projectRoot, "Temp", "MinmapBundleBuild");
            string outputDir = Path.Combine(projectRoot, "Temp", "MinmapBundleOutput");

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }

            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(outputDir);

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            AssetDatabase.Refresh();

            AssetBundleBuild[] builds =
            {
                new AssetBundleBuild
                {
                    assetBundleName = "minmap.unity3d",
                    assetNames = new[] { "Assets/Bundles/Independent/Minmap.prefab" }
                }
            };

            BuildPipeline.BuildAssetBundles(
                tempDir,
                builds,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);

            File.Copy(Path.Combine(tempDir, "minmap.unity3d"), Path.Combine(outputDir, "minmap.unity3d"), true);
            File.Copy(Path.Combine(tempDir, "minmap.unity3d.manifest"), Path.Combine(outputDir, "minmap.unity3d.manifest"), true);

            AssetDatabase.Refresh();
            Debug.Log($"Minmap bundle build succeeded. temp={tempDir} output={outputDir}");
        }
    }
}
