using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildCodeBundle
    {
        public static void PerformBuild()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDir = Path.Combine(Application.dataPath, "StreamingAssets");
            string tempDir = Path.Combine(projectRoot, "Temp", "CodeBundleBuild");

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
            HotfixBuildSync.SyncCompiledHotfixToResCode();

            AssetBundleBuild[] builds =
            {
                new AssetBundleBuild
                {
                    assetBundleName = "code.unity3d",
                    assetNames = new[] { "Assets/Bundles/Independent/Code.prefab" }
                }
            };

            BuildPipeline.BuildAssetBundles(
                tempDir,
                builds,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);

            File.Copy(Path.Combine(tempDir, "code.unity3d"), Path.Combine(outputDir, "code.unity3d"), true);
            File.Copy(Path.Combine(tempDir, "code.unity3d.manifest"), Path.Combine(outputDir, "code.unity3d.manifest"), true);

            AssetDatabase.Refresh();
            Debug.Log($"Code bundle build succeeded. temp={tempDir} output={outputDir} codeExists={File.Exists(Path.Combine(outputDir, "code.unity3d"))}");
        }
    }
}
