using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildCoreBundles
    {
        public static void PerformBuild()
        {
            string outputDir = "Assets/StreamingAssets";
            string tempDir = "Temp/CoreBundleBuild";

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
                },
                new AssetBundleBuild
                {
                    assetBundleName = "config.unity3d",
                    assetNames = new[] { "Assets/Bundles/Independent/Config.prefab" }
                }
            };

            BuildPipeline.BuildAssetBundles(tempDir, builds, BuildAssetBundleOptions.None, BuildTarget.Android);

            CopyBundleFile(Path.Combine(tempDir, "code.unity3d"), Path.Combine(outputDir, "code.unity3d"));
            CopyBundleFile(Path.Combine(tempDir, "code.unity3d.manifest"), Path.Combine(outputDir, "code.unity3d.manifest"));
            CopyBundleFile(Path.Combine(tempDir, "config.unity3d"), Path.Combine(outputDir, "config.unity3d"));
            CopyBundleFile(Path.Combine(tempDir, "config.unity3d.manifest"), Path.Combine(outputDir, "config.unity3d.manifest"));

            AssetDatabase.Refresh();
            Debug.Log("Core bundles build succeeded. Preserved global StreamingAssets.manifest.");
        }

        private static void CopyBundleFile(string sourcePath, string targetPath)
        {
            if (File.Exists(targetPath))
            {
                FileAttributes attributes = File.GetAttributes(targetPath);
                if ((attributes & FileAttributes.ReadOnly) != 0)
                {
                    File.SetAttributes(targetPath, attributes & ~FileAttributes.ReadOnly);
                }
            }

            File.Copy(sourcePath, targetPath, true);
        }
    }
}
