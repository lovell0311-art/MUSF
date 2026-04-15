using UnityEditor;

namespace ETEditor
{
    public static class BatchBuildAndroidAssets
    {
        public static void PerformBuild()
        {
            MapSceneBundleSyncTool.RebuildAndroidGameplaySceneBundlesAndSync();
        }
    }
}
