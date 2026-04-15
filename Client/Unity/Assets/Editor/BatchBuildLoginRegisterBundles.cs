using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildLoginRegisterBundles
    {
        public static void PerformBuild()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDir = Path.Combine(Application.dataPath, "StreamingAssets");
            string tempDir = Path.Combine(projectRoot, "Temp", "LoginRegisterBundleBuild");

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(outputDir);

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            AssetDatabase.Refresh();

            AssetBundleBuild[] builds =
            {
                new AssetBundleBuild
                {
                    assetBundleName = "code.unity3d",
                    assetNames = new[] { "Assets/Bundles/Independent/Code.prefab" }
                },
                new AssetBundleBuild
                {
                    assetBundleName = "uilogin.unity3d",
                    assetNames = new[] { "Assets/Bundles/UI/UILogin.prefab" }
                }
            };

            BuildPipeline.BuildAssetBundles(
                tempDir,
                builds,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);

            foreach (string name in new[]
            {
                "code.unity3d",
                "uilogin.unity3d"
            })
            {
                CopyBundleOutput(Path.Combine(tempDir, name), Path.Combine(outputDir, name));
                CopyBundleOutput(Path.Combine(tempDir, $"{name}.manifest"), Path.Combine(outputDir, $"{name}.manifest"));
            }

            AssetDatabase.Refresh();
            Debug.Log("Login/register bundles build succeeded.");
        }

        private static void CopyBundleOutput(string sourcePath, string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                File.SetAttributes(destinationPath, FileAttributes.Normal);
                File.Delete(destinationPath);
            }

            File.Copy(sourcePath, destinationPath, true);
            File.SetAttributes(destinationPath, FileAttributes.Normal);
        }
    }
}
