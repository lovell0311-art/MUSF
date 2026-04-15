using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildChooseRoleSceneBundle
    {
        public static void PerformBuild()
        {
            const string assetPath = "Assets/Scenes/GameMap/ChooseRole.unity";
            const string bundleName = "chooserole.unity3d";

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDir = Path.Combine(Application.dataPath, "StreamingAssets");
            string tempDir = Path.Combine(projectRoot, "Temp", "ChooseRoleSceneBundleBuild");

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
                    assetBundleName = bundleName,
                    assetNames = new[] { assetPath }
                }
            };

            BuildPipeline.BuildAssetBundles(
                tempDir,
                builds,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);

            CopyBundleOutput(Path.Combine(tempDir, bundleName), Path.Combine(outputDir, bundleName));
            CopyBundleOutput(
                Path.Combine(tempDir, $"{bundleName}.manifest"),
                Path.Combine(outputDir, $"{bundleName}.manifest"));

            AssetDatabase.Refresh();
            Debug.Log("ChooseRole scene bundle build succeeded.");
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
