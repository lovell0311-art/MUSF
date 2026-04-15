using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildYongZheDaLuBundle
    {
        public static void PerformBuildToTemp()
        {
            Debug.LogWarning("BatchBuildYongZheDaLuBundle is redirected to the full gameplay scene rebuild and sync workflow.");
            MapSceneBundleSyncTool.RebuildAndroidGameplaySceneBundlesAndSync();
        }
    }
}
