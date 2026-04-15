using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace ETEditor
{
    [InitializeOnLoad]
    public static class CodexBuildBridge
    {
        private const string RequestDir = "Temp/CodexRequests";
        private const string BuildCoreRequest = "build-core-bundles.txt";
        private const string BuildCoreResult = "build-core-bundles.result.txt";
        private const string BuildCodeRequest = "build-codebundle.txt";
        private const string BuildCodeResult = "build-codebundle.result.txt";
        private const string BuildMapSyncRequest = "build-map-sync.txt";
        private const string BuildMapSyncResult = "build-map-sync.result.txt";
        private const string BuildAndroidRequest = "build-android-player.txt";
        private const string BuildAndroidResult = "build-android-player.result.txt";
        private const string RefreshScriptsRequest = "refresh-scripts.txt";
        private const string RefreshScriptsResult = "refresh-scripts.result.txt";
        private const string ForceCompileRequest = "force-compile-hotfix.txt";
        private const string ForceCompileResult = "force-compile-hotfix.result.txt";
        private static double nextPollTime;

        static CodexBuildBridge()
        {
            EditorApplication.update -= ProcessRequests;
            EditorApplication.update += ProcessRequests;
        }

        private static void ProcessRequests()
        {
            if (EditorApplication.timeSinceStartup < nextPollTime)
            {
                return;
            }

            nextPollTime = EditorApplication.timeSinceStartup + 1d;

            try
            {
                string projectRoot = Path.GetDirectoryName(Application.dataPath);
                string requestDir = Path.Combine(projectRoot, RequestDir);
                Directory.CreateDirectory(requestDir);
                ProcessBuildCoreBundlesRequest(requestDir);
                ProcessBuildCodeBundleRequest(requestDir);
                ProcessMapSyncRequest(requestDir);
                ProcessAndroidBuildRequest(requestDir);
                ProcessRefreshScriptsRequest(requestDir);
                ProcessForceCompileRequest(requestDir);
                FinalizeRefreshScripts(requestDir);
                FinalizeForceCompile(requestDir);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static void ProcessBuildCodeBundleRequest(string requestDir)
        {
            string requestPath = Path.Combine(requestDir, BuildCodeRequest);
            if (!File.Exists(requestPath))
            {
                return;
            }

            File.Delete(requestPath);
            WriteResult(BuildCodeResult, "START");

            try
            {
                BatchBuildCodeBundle.PerformBuild();
                WriteResult(BuildCodeResult, "OK");
                Debug.Log("CodexBuildBridge: code bundle build request completed and published to StreamingAssets.");
            }
            catch (Exception ex)
            {
                WriteResult(BuildCodeResult, "ERROR\n" + ex);
                Debug.LogException(ex);
            }
        }

        private static void ProcessBuildCoreBundlesRequest(string requestDir)
        {
            string requestPath = Path.Combine(requestDir, BuildCoreRequest);
            if (!File.Exists(requestPath))
            {
                return;
            }

            File.Delete(requestPath);
            WriteResult(BuildCoreResult, "START");

            try
            {
                BatchBuildCoreBundles.PerformBuild();
                WriteResult(BuildCoreResult, "OK");
                Debug.Log("CodexBuildBridge: core bundle build request completed and published to StreamingAssets.");
            }
            catch (Exception ex)
            {
                WriteResult(BuildCoreResult, "ERROR\n" + ex);
                Debug.LogException(ex);
            }
        }

        private static void ProcessMapSyncRequest(string requestDir)
        {
            string requestPath = Path.Combine(requestDir, BuildMapSyncRequest);
            if (!File.Exists(requestPath))
            {
                return;
            }

            string requestText = File.ReadAllText(requestPath).Trim();
            File.Delete(requestPath);
            WriteResult(BuildMapSyncResult, "START");

            try
            {
                if (string.Equals(requestText, "PC", StringComparison.OrdinalIgnoreCase))
                {
                    MapSceneBundleSyncTool.PerformPcBuildAndSync();
                }
                else
                {
                    MapSceneBundleSyncTool.PerformAndroidBuildAndSync();
                }

                WriteResult(BuildMapSyncResult, "OK");
                Debug.Log($"CodexBuildBridge: map scene bundle sync completed. platform={requestText}");
            }
            catch (Exception ex)
            {
                WriteResult(BuildMapSyncResult, "ERROR\n" + ex);
                Debug.LogException(ex);
            }
        }

        private static void ProcessAndroidBuildRequest(string requestDir)
        {
            string requestPath = Path.Combine(requestDir, BuildAndroidRequest);
            if (!File.Exists(requestPath))
            {
                return;
            }

            File.Delete(requestPath);
            WriteResult(BuildAndroidResult, "START");

            try
            {
                BatchBuildAndroid.PerformBuild();
                WriteResult(BuildAndroidResult, "OK");
                Debug.Log("CodexBuildBridge: Android player build completed.");
            }
            catch (Exception ex)
            {
                WriteResult(BuildAndroidResult, "ERROR\n" + ex);
                Debug.LogException(ex);
            }
        }

        private static void ProcessRefreshScriptsRequest(string requestDir)
        {
            string requestPath = Path.Combine(requestDir, RefreshScriptsRequest);
            if (!File.Exists(requestPath))
            {
                return;
            }

            File.Delete(requestPath);
            WriteResult(RefreshScriptsResult, "START");

            try
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                CompilationPipeline.RequestScriptCompilation();
                Debug.Log("CodexBuildBridge: script refresh request queued.");
            }
            catch (Exception ex)
            {
                WriteResult(RefreshScriptsResult, "ERROR\n" + ex);
                Debug.LogException(ex);
            }
        }

        private static void ProcessForceCompileRequest(string requestDir)
        {
            string requestPath = Path.Combine(requestDir, ForceCompileRequest);
            if (!File.Exists(requestPath))
            {
                return;
            }

            File.Delete(requestPath);
            WriteResult(ForceCompileResult, "START");

            try
            {
                AssetDatabase.ImportAsset("Assets/Hotfix/Miracle_MU/UI/UIChooseRole/UIChooseRoleComponent.cs", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.ImportAsset("Assets/Hotfix/Miracle_MU/Entity/RoleEntity/Data/RoleArchiveInfoManager.cs", ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                CompilationPipeline.RequestScriptCompilation();
                Debug.Log("CodexBuildBridge: force compile request queued.");
            }
            catch (Exception ex)
            {
                WriteResult(ForceCompileResult, "ERROR\n" + ex);
                Debug.LogException(ex);
            }
        }

        private static void FinalizeRefreshScripts(string requestDir)
        {
            string resultPath = Path.Combine(requestDir, RefreshScriptsResult);
            if (!File.Exists(resultPath))
            {
                return;
            }

            if (!string.Equals(File.ReadAllText(resultPath), "START", StringComparison.Ordinal))
            {
                return;
            }

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            WriteResult(RefreshScriptsResult, "OK");
            Debug.Log("CodexBuildBridge: script refresh request completed.");
        }

        private static void FinalizeForceCompile(string requestDir)
        {
            string resultPath = Path.Combine(requestDir, ForceCompileResult);
            if (!File.Exists(resultPath))
            {
                return;
            }

            if (!string.Equals(File.ReadAllText(resultPath), "START", StringComparison.Ordinal))
            {
                return;
            }

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            WriteResult(ForceCompileResult, "OK");
            Debug.Log("CodexBuildBridge: force compile request completed.");
        }

        private static void WriteResult(string fileName, string content)
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string resultPath = Path.Combine(projectRoot, RequestDir, fileName);
            File.WriteAllText(resultPath, content);
        }
    }
}
