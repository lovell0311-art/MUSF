using System;
using System.IO;
using UnityEngine;

namespace ETModel
{
    public static class PathHelper
    {
        private const string PreferredHotfixFolderName = "星辰战纪";
        private static string cachedAppHotfixResPath;

        /// <summary>
        /// 应用程序外部资源路径存放路径(热更新资源路径)
        /// </summary>
        public static string AppHotfixResPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(cachedAppHotfixResPath))
                {
                    return cachedAppHotfixResPath;
                }

                cachedAppHotfixResPath = ResolveAppHotfixResPath();
                return cachedAppHotfixResPath;
            }
        }

        /// <summary>
        /// 应用程序内部资源路径存放路径
        /// </summary>
        public static string AppResPath
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }

        /// <summary>
        /// 应用程序内部资源路径存放路径(www/webrequest专用)
        /// </summary>
        public static string AppResPath4Web
        {
            get
            {
#if UNITY_IOS || UNITY_STANDALONE_OSX
                return $"file://{Application.streamingAssetsPath}";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
                return "file://" + Application.streamingAssetsPath + "/";
#else
                return Application.streamingAssetsPath + "/";
#endif
            }
        }

        private static string ResolveAppHotfixResPath()
        {
            string root = GetHotfixRootPath();
            string stablePath = Path.Combine(root, GetStableHotfixFolderName());
            string migratedStablePath = TryPromoteLegacyHotfixPath(root, stablePath);
            if (!string.IsNullOrWhiteSpace(migratedStablePath))
            {
                return EnsureTrailingSeparator(migratedStablePath);
            }

            if (HasHotfixContent(stablePath))
            {
                return EnsureTrailingSeparator(stablePath);
            }

            try
            {
                if (Directory.Exists(root))
                {
                    string bestLegacyPath = null;
                    DateTime bestLegacyTime = DateTime.MinValue;
                    string normalizedStablePath = Path.GetFullPath(stablePath);
                    foreach (string candidate in Directory.GetDirectories(root))
                    {
                        string normalizedCandidate = Path.GetFullPath(candidate);
                        if (string.Equals(normalizedStablePath, normalizedCandidate, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (!HasHotfixContent(candidate))
                        {
                            continue;
                        }

                        DateTime candidateTime = GetVersionFileWriteTimeUtc(candidate);
                        if (bestLegacyPath == null || candidateTime > bestLegacyTime)
                        {
                            bestLegacyPath = candidate;
                            bestLegacyTime = candidateTime;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(bestLegacyPath))
                    {
                        Debug.LogWarning($"PathHelper reuse legacy hotfix path: {bestLegacyPath}");
                        return EnsureTrailingSeparator(bestLegacyPath);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"PathHelper resolve hotfix path fallback failed: {e.Message}");
            }

            return EnsureTrailingSeparator(stablePath);
        }

        private static string GetHotfixRootPath()
        {
#if UNITY_IOS
            return Path.Combine(Application.persistentDataPath, "IOS");
#elif UNITY_ANDROID
            return Path.Combine(Application.persistentDataPath, "Android");
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
            return Path.Combine(Application.persistentDataPath, "PC");
#else
            return Application.persistentDataPath;
#endif
        }

        private static string GetStableHotfixFolderName()
        {
            return PreferredHotfixFolderName;
        }

        private static string TryPromoteLegacyHotfixPath(string root, string stablePath)
        {
            if (string.IsNullOrWhiteSpace(root) || string.IsNullOrWhiteSpace(stablePath))
            {
                return null;
            }

            if (HasHotfixContent(stablePath))
            {
                return stablePath;
            }

            try
            {
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                    return null;
                }

                string bestLegacyPath = null;
                DateTime bestLegacyTime = DateTime.MinValue;
                string normalizedStablePath = Path.GetFullPath(stablePath);
                foreach (string candidate in Directory.GetDirectories(root))
                {
                    string normalizedCandidate = Path.GetFullPath(candidate);
                    if (string.Equals(normalizedStablePath, normalizedCandidate, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!HasHotfixContent(candidate))
                    {
                        continue;
                    }

                    DateTime candidateTime = GetVersionFileWriteTimeUtc(candidate);
                    if (bestLegacyPath == null || candidateTime > bestLegacyTime)
                    {
                        bestLegacyPath = candidate;
                        bestLegacyTime = candidateTime;
                    }
                }

                if (string.IsNullOrWhiteSpace(bestLegacyPath))
                {
                    return null;
                }

                if (!Directory.Exists(stablePath))
                {
                    Directory.Move(bestLegacyPath, stablePath);
                    Debug.LogWarning($"PathHelper promoted legacy hotfix path: {bestLegacyPath} => {stablePath}");
                    return stablePath;
                }

                if (!HasHotfixContent(stablePath))
                {
                    foreach (string file in Directory.GetFiles(bestLegacyPath))
                    {
                        string destination = Path.Combine(stablePath, Path.GetFileName(file));
                        File.Copy(file, destination, true);
                    }

                    foreach (string directory in Directory.GetDirectories(bestLegacyPath))
                    {
                        string destination = Path.Combine(stablePath, Path.GetFileName(directory));
                        CopyDirectory(directory, destination);
                    }

                    Debug.LogWarning($"PathHelper copied legacy hotfix path into stable path: {bestLegacyPath} => {stablePath}");
                    return stablePath;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"PathHelper promote hotfix path failed: {e.Message}");
            }

            return null;
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destination = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destination, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string destination = Path.Combine(destinationDir, Path.GetFileName(directory));
                CopyDirectory(directory, destination);
            }
        }

        private static bool HasHotfixContent(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                return false;
            }

            string versionPath = Path.Combine(dir, "Version.txt");
            if (File.Exists(versionPath))
            {
                return true;
            }

            try
            {
                return Directory.GetFiles(dir).Length > 0 || Directory.GetDirectories(dir).Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private static DateTime GetVersionFileWriteTimeUtc(string dir)
        {
            try
            {
                string versionPath = Path.Combine(dir, "Version.txt");
                if (File.Exists(versionPath))
                {
                    return File.GetLastWriteTime(versionPath).ToUniversalTime();
                }
            }
            catch
            {
            }

            return DateTime.MinValue;
        }

        private static string EnsureTrailingSeparator(string path)
        {
            string normalizedPath = Path.GetFullPath(path);
            string separator = Path.DirectorySeparatorChar.ToString();
            if (!normalizedPath.EndsWith(separator, StringComparison.Ordinal))
            {
                normalizedPath += separator;
            }

            return normalizedPath;
        }
    }
}
