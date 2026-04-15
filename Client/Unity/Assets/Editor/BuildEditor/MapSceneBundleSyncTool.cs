using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class MapSceneBundleSyncTool
    {
        [MenuItem("Tools/构建/校验重建并同步地图资源/Android")]
        public static void RebuildAndroidGameplaySceneBundlesAndSync()
        {
            RebuildAndSync(PlatformType.Android);
        }

        [MenuItem("Tools/构建/校验重建并同步地图资源/PC")]
        public static void RebuildPcGameplaySceneBundlesAndSync()
        {
            RebuildAndSync(PlatformType.PC);
        }

        public static void PerformAndroidBuildAndSync()
        {
            RebuildAndSync(PlatformType.Android);
        }

        public static void PerformPcBuildAndSync()
        {
            RebuildAndSync(PlatformType.PC);
        }

        private static void RebuildAndSync(PlatformType platformType)
        {
            SwitchActiveBuildTarget(platformType);
            UpLoadFilesEditor.InitPath();
            BuildHelper.Build(platformType, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildOptions.None, false, true);

            Debug.Log(
                $"MapSceneBundleSyncTool finished platform={platformType} " +
                $"local={BuildHelper.GetLocalStreamingAssetsPath()} " +
                $"server={BuildHelper.GetServerStreamingAssetsPath(platformType)}");
        }

        private static void SwitchActiveBuildTarget(PlatformType platformType)
        {
            switch (platformType)
            {
                case PlatformType.Android:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    break;
                case PlatformType.IOS:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                    break;
                case PlatformType.MacOS:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
                    break;
                default:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                    break;
            }
        }
    }
}
