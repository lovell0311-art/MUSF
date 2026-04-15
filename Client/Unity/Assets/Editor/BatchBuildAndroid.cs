using System;
using System.IO;
using System.Reflection;
using ETModel;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ETEditor
{
    public static class BatchBuildAndroid
    {
        public static void PerformBuild()
        {
            string unityJdkRoot = ResolveUnityJdkRoot();
            string cmdlineJdkRoot = ResolveCommandLineJdkRoot(unityJdkRoot);
            string sdkRoot = ResolveAndroidSdkRoot();
            string ndkRoot = string.Empty;
            string outputDir = @"F:\MUSF\Client\Builds\Android";
            string outputPath = Path.Combine(outputDir, "MUSF-test.apk");

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            TrySetAndroidExternalToolsPath("jdkRootPath", unityJdkRoot);
            TrySetAndroidExternalToolsPath("sdkRootPath", sdkRoot);

            Environment.SetEnvironmentVariable("JAVA_HOME", cmdlineJdkRoot);
            Environment.SetEnvironmentVariable("ANDROID_SDK_ROOT", sdkRoot);
            Environment.SetEnvironmentVariable("ANDROID_HOME", sdkRoot);

            ForceUnityToUseCommandLineTools();

            PlayerSettings.Android.useCustomKeystore = false;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            ndkRoot = RequiresAndroidNdk()
                ? ResolveAndroidNdkRoot(sdkRoot)
                : TryResolveAndroidNdkRoot(sdkRoot);
            if (!string.IsNullOrWhiteSpace(ndkRoot))
            {
                EditorPrefs.SetInt("NdkUseEmbedded", 0);
                EditorPrefs.SetString("AndroidNdkRootR19", ndkRoot);
                Environment.SetEnvironmentVariable("ANDROID_NDK_ROOT", ndkRoot);
            }

            if (string.IsNullOrWhiteSpace(PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)))
            {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.musf.test");
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            AssetDatabase.Refresh();
            AndroidShaderKeepAliveBuilder.EnsureAssets();
            if (NeedsAndroidAssetSync())
            {
                MapSceneBundleSyncTool.RebuildAndroidGameplaySceneBundlesAndSync();
            }
            else
            {
                Debug.Log("BatchBuildAndroid: skip Android asset sync because local StreamingAssets already matches Release_Test.");
            }

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Init.unity" },
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None,
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception($"Android build failed: {report.summary.result}");
            }

            if (!File.Exists(outputPath))
            {
                string[] outputFiles = Directory.Exists(outputDir)
                    ? Directory.GetFiles(outputDir)
                    : Array.Empty<string>();
                throw new FileNotFoundException(
                    $"Android build reported success but APK is missing: {outputPath}. OutputDirFiles={string.Join(", ", outputFiles)}");
            }

            Debug.Log($"Android build succeeded: {outputPath}");
        }

        private static bool NeedsAndroidAssetSync()
        {
            string localVersionPath = Path.Combine(BuildHelper.GetLocalStreamingAssetsPath(), "Version.txt");
            string releaseVersionPath = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "..",
                "Release_Test",
                "Android",
                "StreamingAssets",
                "Version.txt"));

            if (!File.Exists(localVersionPath) || !File.Exists(releaseVersionPath))
            {
                return true;
            }

            try
            {
                string localVersion = File.ReadAllText(localVersionPath);
                string releaseVersion = File.ReadAllText(releaseVersionPath);
                return !string.Equals(localVersion, releaseVersion, StringComparison.Ordinal);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"BatchBuildAndroid: failed to compare Version.txt files, fallback to full sync. {e.Message}");
                return true;
            }
        }

        private static string ResolveUnityJdkRoot()
        {
            string[] candidates =
            {
                GetAndroidExternalToolsPath("jdkRootPath"),
                @"C:\Program Files\Eclipse Adoptium\jdk-8.0.482.8-hotspot",
                @"C:\Program Files\Unity 2020.3.49f1c1\Editor\Data\PlaybackEngines\AndroidPlayer\OpenJDK",
            };

            return GetFirstExistingDirectory(candidates, "Unity JDK");
        }

        private static string ResolveCommandLineJdkRoot(string unityJdkRoot)
        {
            string[] candidates =
            {
                Environment.GetEnvironmentVariable("JAVA_HOME"),
                @"C:\Program Files\Microsoft\jdk-17.0.18.8-hotspot",
                unityJdkRoot,
            };

            return GetFirstExistingDirectory(candidates, "Command line JDK");
        }

        private static string ResolveAndroidSdkRoot()
        {
            string[] candidates =
            {
                GetAndroidExternalToolsPath("sdkRootPath"),
                Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"),
                Environment.GetEnvironmentVariable("ANDROID_HOME"),
                @"C:\Android\Sdk",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Android", "Sdk"),
            };

            foreach (string candidate in candidates)
            {
                if (IsValidAndroidSdk(candidate))
                {
                    return candidate;
                }
            }

            throw new DirectoryNotFoundException(
                "Android SDK not found. Checked: " + string.Join(", ", candidates));
        }

        private static string ResolveAndroidNdkRoot(string sdkRoot)
        {
            string ndkRoot = TryResolveAndroidNdkRoot(sdkRoot);
            if (!string.IsNullOrWhiteSpace(ndkRoot))
            {
                return ndkRoot;
            }

            throw new DirectoryNotFoundException(
                "Android NDK not found. Checked: " + string.Join(", ", GetAndroidNdkCandidates(sdkRoot)));
        }

        private static string TryResolveAndroidNdkRoot(string sdkRoot)
        {
            string[] candidates = GetAndroidNdkCandidates(sdkRoot);

            foreach (string candidate in candidates)
            {
                if (IsValidAndroidNdk(candidate))
                {
                    return candidate;
                }
            }

            string sdkNdkRoot = Path.Combine(sdkRoot, "ndk");
            if (Directory.Exists(sdkNdkRoot))
            {
                string[] subDirs = Directory.GetDirectories(sdkNdkRoot);
                foreach (string subDir in subDirs)
                {
                    if (IsValidAndroidNdk(subDir))
                    {
                        return subDir;
                    }
                }
            }

            return string.Empty;
        }

        private static string[] GetAndroidNdkCandidates(string sdkRoot)
        {
            return new[]
            {
                Environment.GetEnvironmentVariable("ANDROID_NDK_ROOT"),
                EditorPrefs.GetString("AndroidNdkRootR19", string.Empty),
                Path.Combine(sdkRoot, "ndk", "19.0.5232133"),
            };
        }

        private static string GetFirstExistingDirectory(string[] candidates, string label)
        {
            foreach (string candidate in candidates)
            {
                if (!string.IsNullOrWhiteSpace(candidate) && Directory.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new DirectoryNotFoundException(
                $"{label} not found. Checked: " + string.Join(", ", candidates));
        }

        private static bool IsValidAndroidSdk(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return false;
            }

            bool hasAdb = File.Exists(Path.Combine(path, "platform-tools", "adb.exe"));
            bool hasBuildTools = Directory.Exists(Path.Combine(path, "build-tools"));
            bool hasCmdlineTools = Directory.Exists(Path.Combine(path, "cmdline-tools"));
            return hasAdb && (hasBuildTools || hasCmdlineTools);
        }

        private static bool IsValidAndroidNdk(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return false;
            }

            return File.Exists(Path.Combine(path, "source.properties")) ||
                   Directory.Exists(Path.Combine(path, "toolchains")) ||
                   File.Exists(Path.Combine(path, "ndk-build.cmd"));
        }

        private static void ForceUnityToUseCommandLineTools()
        {
            Type externalToolsType = FindAndroidEditorType("UnityEditor.Android.AndroidExternalToolsSettings");
            Assembly androidAssembly = externalToolsType?.Assembly;
            if (androidAssembly == null)
            {
                Debug.LogWarning("AndroidExternalToolsSettings type not found.");
                return;
            }

            Type sdkToolsType = androidAssembly.GetType("UnityEditor.Android.AndroidSDKTools");
            if (sdkToolsType == null)
            {
                Debug.LogWarning("AndroidSDKTools type not found.");
                return;
            }

            object sdkTools = sdkToolsType
                .GetMethod("GetInstance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, Type.EmptyTypes, null)
                ?.Invoke(null, null);

            if (sdkTools == null)
            {
                Debug.LogWarning("AndroidSDKTools instance not found.");
                return;
            }

            PropertyInfo useCmdLineTools = sdkToolsType.GetProperty("UseCmdLineTools", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo updateToolsDirectories = sdkToolsType.GetMethod("UpdateToolsDirectories", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo adbProperty = sdkToolsType.GetProperty("ADB", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (useCmdLineTools != null && useCmdLineTools.CanWrite)
            {
                useCmdLineTools.SetValue(sdkTools, true);
            }

            updateToolsDirectories?.Invoke(sdkTools, null);

            Debug.Log($"AndroidSDKTools.UseCmdLineTools = {useCmdLineTools?.GetValue(sdkTools)}");
            Debug.Log($"AndroidSDKTools.ADB = {adbProperty?.GetValue(sdkTools)}");
        }

        private static bool RequiresAndroidNdk()
        {
            return PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) == ScriptingImplementation.IL2CPP;
        }

        private static string GetAndroidExternalToolsPath(string memberName)
        {
            Type externalToolsType = FindAndroidEditorType("UnityEditor.Android.AndroidExternalToolsSettings");
            if (externalToolsType == null)
            {
                return string.Empty;
            }

            PropertyInfo property = externalToolsType.GetProperty(memberName, BindingFlags.Public | BindingFlags.Static);
            if (property != null && property.PropertyType == typeof(string))
            {
                return property.GetValue(null) as string ?? string.Empty;
            }

            FieldInfo field = externalToolsType.GetField(memberName, BindingFlags.Public | BindingFlags.Static);
            if (field != null && field.FieldType == typeof(string))
            {
                return field.GetValue(null) as string ?? string.Empty;
            }

            return string.Empty;
        }

        private static void TrySetAndroidExternalToolsPath(string memberName, string value)
        {
            Type externalToolsType = FindAndroidEditorType("UnityEditor.Android.AndroidExternalToolsSettings");
            if (externalToolsType == null)
            {
                Debug.LogWarning($"AndroidExternalToolsSettings type not found. skip setting {memberName}.");
                return;
            }

            PropertyInfo property = externalToolsType.GetProperty(memberName, BindingFlags.Public | BindingFlags.Static);
            if (property != null && property.PropertyType == typeof(string) && property.CanWrite)
            {
                property.SetValue(null, value);
                return;
            }

            FieldInfo field = externalToolsType.GetField(memberName, BindingFlags.Public | BindingFlags.Static);
            if (field != null && field.FieldType == typeof(string))
            {
                field.SetValue(null, value);
                return;
            }

            Debug.LogWarning($"AndroidExternalToolsSettings member not found. skip setting {memberName}.");
        }

        private static Type FindAndroidEditorType(string fullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName, false);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
