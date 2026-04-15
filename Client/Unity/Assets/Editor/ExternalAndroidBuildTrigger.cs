#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{
    [InitializeOnLoad]
    public static class ExternalAndroidBuildTrigger
    {
        private const string RequestPath = @"F:\MUSF\Client\Builds\Android\external-build-request.txt";
        private const string ResultPath = @"F:\MUSF\Client\Builds\Android\external-build-result.txt";
        private static bool isBuilding;

        static ExternalAndroidBuildTrigger()
        {
            EditorApplication.update += PollBuildRequest;
        }

        private static void PollBuildRequest()
        {
            if (isBuilding || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            if (!File.Exists(RequestPath))
            {
                return;
            }

            string request = SafeReadAllText(RequestPath).Trim();
            if (!string.Equals(request, "android-runtime-fix", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            isBuilding = true;
            SafeDelete(RequestPath);
            WriteResult("queued");
            RunBuild();
        }

        private static void RunBuild()
        {
            try
            {
                WriteResult("running");
                BatchBuildAndroid.PerformBuild();
                WriteResult($"success apk={Path.Combine(@"F:\MUSF\Client\Builds\Android", "MUSF-test.apk")}");
            }
            catch (Exception e)
            {
                WriteResult($"error\n{e}");
                Debug.LogException(e);
            }
            finally
            {
                isBuilding = false;
                AssetDatabase.Refresh();
            }
        }

        private static string SafeReadAllText(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void SafeDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
            }
        }

        private static void WriteResult(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ResultPath) ?? ".");
                File.WriteAllText(ResultPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{message}");
            }
            catch
            {
            }
        }
    }
}
#endif
