using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildLockedMainUiBundles
    {
        [MenuItem("Tools/构建/构建锁定主界面 Bundles")]
        public static void PerformBuild()
        {
            LockedMainUiBundleTargets targets = MainUiBundleBuildGuard.RepairAndAuditLockedMainUiBundles();

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string outputDir = Path.Combine(Application.dataPath, "StreamingAssets");
            string tempDir = Path.Combine(projectRoot, "Temp", "LockedMainUiBundleBuild");
            string snapshotDir = MainUiBundleBuildGuard.GetSnapshotDirectoryAbsolutePath();

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(outputDir);
            Directory.CreateDirectory(snapshotDir);

            AssetBundleBuild[] builds =
            {
                new AssetBundleBuild
                {
                    assetBundleName = MainUiBundleBuildGuard.UIMainCanvasBundleName,
                    assetNames = new[] { targets.UIMainCanvasSource }
                },
                new AssetBundleBuild
                {
                    assetBundleName = MainUiBundleBuildGuard.UIHudBundleName,
                    assetNames = new[] { targets.UIHudSource }
                }
            };

            BuildPipeline.BuildAssetBundles(
                tempDir,
                builds,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                BuildTarget.Android);

            CopyBundlePair(tempDir, outputDir, MainUiBundleBuildGuard.UIMainCanvasBundleName);
            CopyBundlePair(tempDir, outputDir, MainUiBundleBuildGuard.UIHudBundleName);
            CopyBundlePair(tempDir, snapshotDir, MainUiBundleBuildGuard.UIMainCanvasBundleName);
            CopyBundlePair(tempDir, snapshotDir, MainUiBundleBuildGuard.UIHudBundleName);

            AssetDatabase.Refresh();
            Debug.Log(
                $"Locked main UI bundle build succeeded. main={targets.UIMainCanvasSource} hud={targets.UIHudSource} " +
                $"snapshot={snapshotDir}");
        }

        private static void CopyBundlePair(string sourceDir, string destinationDir, string bundleName)
        {
            File.Copy(Path.Combine(sourceDir, bundleName), Path.Combine(destinationDir, bundleName), true);
            File.Copy(
                Path.Combine(sourceDir, $"{bundleName}.manifest"),
                Path.Combine(destinationDir, $"{bundleName}.manifest"),
                true);
        }
    }
}
