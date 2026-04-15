using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildChooseRoleBundle
    {
        public static void PerformBuild()
        {
            const string assetPath = "Assets/Bundles/UI/UIChooseRole.prefab";
            const string bundleName = "uichooserole.unity3d";

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDir = Path.Combine(Application.dataPath, "StreamingAssets");
            string tempDir = Path.Combine(projectRoot, "Temp", "ChooseRoleBundleBuild");

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(outputDir);

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

            File.Copy(Path.Combine(tempDir, bundleName), Path.Combine(outputDir, bundleName), true);
            File.Copy(
                Path.Combine(tempDir, $"{bundleName}.manifest"),
                Path.Combine(outputDir, $"{bundleName}.manifest"),
                true);

            AssetDatabase.Refresh();
            Debug.Log("ChooseRole bundle build succeeded.");
        }
    }
}
