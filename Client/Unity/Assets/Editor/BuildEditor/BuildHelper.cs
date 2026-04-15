using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ETModel;
using UnityEditor;
using UnityEngine;

namespace ETEditor
{

	public enum BuildResPath 
	{
	 LocalTest,
	 Test,
	 WaiWang,
	 DouYin,
	 ShouQ,
	 KaiYing,
	 LiZi,
	 TapTap,
	 Three,
     RongYaoChuanShuo,
	 QuanZhi,
     LingWu
	}
	
	public static class BuildHelper
	{
        private const string relativeDirPrefix = "../Release";

		public static string BuildFolder = "../../Release_Test/{0}/StreamingAssets/";
		//public static string BuildFolder = "../../Release_LocalTest/{0}/StreamingAssets/";
		//public static string BuildFolder = "../../Release_LiZi/{0}/StreamingAssets/";

        public static string GetLocalStreamingAssetsPath()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "StreamingAssets"));
        }

        public static string GetServerStreamingAssetsPath(PlatformType type)
        {
            return Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "..",
                "Server",
                "Release",
                GetPlatformFolderName(type),
                "StreamingAssets"));
        }

        public static string GetServerUpdateStreamingAssetsPath(PlatformType type)
        {
            return Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "..",
                "Server",
                "Release",
                "update",
                "2.0TestGame",
                GetPlatformFolderName(type),
                "StreamingAssets"));
        }


        [MenuItem("Tools/web资源服务器")]
		public static void OpenFileServer()
		{
            //进程帮助类 dotnet
            ProcessHelper.Run("dotnet", "FileServer.dll", "../FileServer/");
		}

		public static string GetBuildRes(BuildResPath buildRes)
		{
            BuildFolder= "../../Release/" + buildRes.ToString() + "/{0}/StreamingAssets/";
            return BuildFolder;
           
		}

		public static void Build(PlatformType type, BuildAssetBundleOptions buildAssetBundleOptions, BuildOptions buildOptions, bool isBuildExe, bool isContainAB)
		{
			BuildTarget buildTarget = BuildTarget.StandaloneWindows;
			string exeName = "ET";
			switch (type)
			{
				case PlatformType.PC:
					buildTarget = BuildTarget.StandaloneWindows64;
					exeName += ".exe";
					break;
				case PlatformType.Android:
					buildTarget = BuildTarget.Android;
					exeName += ".apk";
					break;
				case PlatformType.IOS:
					buildTarget = BuildTarget.iOS;
					break;
				case PlatformType.MacOS:
					buildTarget = BuildTarget.StandaloneOSX;
					break;
			}

			string fold = string.Format(BuildFolder, type);
            
            if (!Directory.Exists(fold))
			{
				Directory.CreateDirectory(fold);
			}
			
			Debug.Log("开始资源打包"+fold);
			//UpLoadFilesEditor.StartPack(fold);
			UpLoadFilesEditor.CopyVersionTxt();
            SceneBundleBuildGuard.RepairAndAuditGameplaySceneBundles();
            MainUiBundleBuildGuard.RepairAndAuditLockedMainUiBundles();
			BuildPipeline.BuildAssetBundles(fold, buildAssetBundleOptions, buildTarget);
			
            //生成Version.txt
			GenerateVersionInfo(fold);
            Debug.Log("完成资源打包");
         //   UpLoadFilesEditor.EndPack();
            //将资源 复制到StremingAsset文件夹下
            if (isContainAB)
			{
                List<Exception> syncExceptions = new List<Exception>();
                TrySyncBuildOutput(
                    fold,
                    GetServerStreamingAssetsPath(type),
                    $"Server/Release/{GetPlatformFolderName(type)}/StreamingAssets",
                    syncExceptions);
                TrySyncBuildOutput(
                    fold,
                    GetServerUpdateStreamingAssetsPath(type),
                    $"Server/Release/update/2.0TestGame/{GetPlatformFolderName(type)}/StreamingAssets",
                    syncExceptions);
                TrySyncBuildOutput(
                    fold,
                    GetLocalStreamingAssetsPath(),
                    "Assets/StreamingAssets",
                    syncExceptions);
                AssetDatabase.Refresh();
                if (syncExceptions.Count > 0)
                {
                    throw new AggregateException("One or more build output sync operations failed.", syncExceptions);
                }
			}

			if (isBuildExe)
			{
				AssetDatabase.Refresh();
				string[] levels = {
					"Assets/Scenes/Init.unity",
				};
				Log.Info("开始EXE打包");
				BuildPipeline.BuildPlayer(levels, $"{relativeDirPrefix}/{exeName}", buildTarget, buildOptions);
				Log.Info("完成exe打包");
			}
		}

        private static void TrySyncBuildOutput(string sourceDir, string targetDir, string label, List<Exception> syncExceptions)
        {
            try
            {
                SyncBuildOutput(sourceDir, targetDir, label);
            }
            catch (Exception e)
            {
                syncExceptions.Add(new Exception($"Sync failed for {label}: {e.Message}", e));
            }
        }

        private static void SyncBuildOutput(string sourceDir, string targetDir, string label)
        {
            string normalizedSource = Path.GetFullPath(sourceDir);
            string normalizedTarget = Path.GetFullPath(targetDir);
            if (string.Equals(normalizedSource, normalizedTarget, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Skip sync because source and target are identical: {label} => {normalizedTarget}");
                return;
            }

            if (!Directory.Exists(normalizedTarget))
            {
                Directory.CreateDirectory(normalizedTarget);
            }

            CleanRuntimeDirectory(normalizedTarget);
            FileHelper.CopyDirectory(normalizedSource, normalizedTarget);
            if (!string.Equals(label, "Assets/StreamingAssets", StringComparison.OrdinalIgnoreCase))
            {
                ValidateVersionDirectory(normalizedTarget, label);
            }
            Debug.Log($"Synced build output to {label}: {normalizedTarget}");
        }

        private static void CleanRuntimeDirectory(string targetDir)
        {
            foreach (string subdir in Directory.GetDirectories(targetDir))
            {
                DeleteRuntimePath(subdir);
            }

            foreach (string subFile in Directory.GetFiles(targetDir))
            {
                DeleteRuntimePath(subFile);
            }
        }

        private static void DeleteRuntimePath(string absolutePath)
        {
            string normalizedPath = Path.GetFullPath(absolutePath);
            if (TryGetProjectRelativePath(normalizedPath, out string relativePath))
            {
                FileUtil.DeleteFileOrDirectory(relativePath);
                FileUtil.DeleteFileOrDirectory($"{relativePath}.meta");
                return;
            }

            if (Directory.Exists(normalizedPath))
            {
                Directory.Delete(normalizedPath, true);
            }
            else if (File.Exists(normalizedPath))
            {
                File.SetAttributes(normalizedPath, FileAttributes.Normal);
                File.Delete(normalizedPath);
            }
        }

        private static bool TryGetProjectRelativePath(string absolutePath, out string relativePath)
        {
            string assetsPath = Path.GetFullPath(Application.dataPath);
            if (absolutePath.StartsWith(assetsPath, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = "Assets" + absolutePath.Substring(assetsPath.Length).Replace('\\', '/');
                return true;
            }

            relativePath = null;
            return false;
        }

        private static void ValidateVersionDirectory(string targetDir, string label)
        {
            string versionPath = Path.Combine(targetDir, "Version.txt");
            if (!File.Exists(versionPath))
            {
                return;
            }

            VersionConfig versionProto = JsonHelper.FromJson<VersionConfig>(File.ReadAllText(versionPath));
            if (versionProto == null)
            {
                throw new InvalidDataException($"ValidateVersionDirectory failed: Version.txt parse returned null. label={label} path={versionPath}");
            }

            List<string> mismatches = new List<string>();
            foreach (KeyValuePair<string, FileVersionInfo> entry in versionProto.FileInfoDict.OrderBy(p => p.Key, StringComparer.Ordinal))
            {
                FileVersionInfo fileInfo = entry.Value;
                if (fileInfo == null)
                {
                    mismatches.Add($"{entry.Key}: missing file info");
                }
                else if (ShouldSkipVersionEntry(entry.Key))
                {
                    mismatches.Add($"{entry.Key}: unexpected skipped entry");
                }
                else
                {
                    string relativePath = fileInfo.File.Replace('/', Path.DirectorySeparatorChar);
                    string filePath = Path.Combine(targetDir, relativePath);
                    if (!File.Exists(filePath))
                    {
                        mismatches.Add($"{entry.Key}: missing file");
                    }
                    else
                    {
                        FileInfo localFileInfo = new FileInfo(filePath);
                        if (localFileInfo.Length != fileInfo.Size)
                        {
                            mismatches.Add($"{entry.Key}: size {localFileInfo.Length} != {fileInfo.Size}");
                        }
                        else
                        {
                            string md5 = MD5Helper.FileMD5(filePath);
                            if (!string.Equals(md5, fileInfo.MD5, StringComparison.OrdinalIgnoreCase))
                            {
                                mismatches.Add($"{entry.Key}: md5 {md5} != {fileInfo.MD5}");
                            }
                        }
                    }
                }

                if (mismatches.Count >= 8)
                {
                    break;
                }
            }

            if (mismatches.Count > 0)
            {
                throw new InvalidDataException(
                    $"ValidateVersionDirectory failed for {label}. sample={string.Join(" | ", mismatches)}");
            }

            Debug.Log($"Validated Version.txt for {label}: entries={versionProto.FileInfoDict.Count}");
        }

        private static string GetPlatformFolderName(PlatformType type)
        {
            switch (type)
            {
                case PlatformType.Android:
                    return "Android";
                case PlatformType.IOS:
                    return "IOS";
                case PlatformType.MacOS:
                    return "MacOS";
                default:
                    return "PC";
            }
        }

		private static void GenerateVersionInfo(string dir)
		{
            //将所有的AB文件写入到 FileInfoDict 这个字典中
            VersionConfig versionProto = new VersionConfig();
			GenerateVersionProto(dir, versionProto, "");
            int preservedVersion = TryGetPreservedVersion(dir, versionProto);
			versionProto.Version = preservedVersion > 0 ? preservedVersion : GetTimestamp();

            //创建一个文件流 然后往这个流里写入数据 
			using (FileStream fileStream = new FileStream($"{dir}/Version.txt", FileMode.Create))
			{
                //序列化成byte[]
				byte[] bytes = JsonHelper.ToJson(versionProto).ToByteArray();
                //通过字节数组写入到文本文件中
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}
        public static int GetTimestamp()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);//ToUniversalTime()转换为标准时区的时间,去掉的话直接就用北京时间
           // return (long)ts.TotalMilliseconds; //精确到毫秒
            return (int)ts.TotalSeconds;//获取10位
        }

        private static int TryGetPreservedVersion(string dir, VersionConfig currentVersionProto)
        {
            string versionPath = Path.Combine(dir, "Version.txt");
            if (!File.Exists(versionPath))
            {
                return 0;
            }

            try
            {
                VersionConfig previousVersionProto = JsonHelper.FromJson<VersionConfig>(File.ReadAllText(versionPath));
                if (previousVersionProto == null)
                {
                    return 0;
                }

                return HasSameVersionEntries(previousVersionProto, currentVersionProto)
                    ? previousVersionProto.Version
                    : 0;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"BuildHelper.TryGetPreservedVersion failed: {e.Message}");
                return 0;
            }
        }

        private static bool HasSameVersionEntries(VersionConfig previousVersionProto, VersionConfig currentVersionProto)
        {
            if (previousVersionProto.FileInfoDict.Count != currentVersionProto.FileInfoDict.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, FileVersionInfo> entry in currentVersionProto.FileInfoDict)
            {
                if (!previousVersionProto.FileInfoDict.TryGetValue(entry.Key, out FileVersionInfo previousFileInfo))
                {
                    return false;
                }

                FileVersionInfo currentFileInfo = entry.Value;
                if (!string.Equals(previousFileInfo.File, currentFileInfo.File, StringComparison.Ordinal) ||
                    !string.Equals(previousFileInfo.MD5, currentFileInfo.MD5, StringComparison.OrdinalIgnoreCase) ||
                    previousFileInfo.Size != currentFileInfo.Size)
                {
                    return false;
                }
            }

            return true;
        }


		private static void GenerateVersionProto(string dir, VersionConfig versionProto, string relativePath)
		{
            //遍历输出AB包的路径 找到他下面的所有资源(文件)
			foreach (string file in Directory.GetFiles(dir).OrderBy(p => p, StringComparer.Ordinal))
			{
                FileInfo fi = new FileInfo(file);
                string filePath = relativePath == "" ? fi.Name : $"{relativePath}/{fi.Name}";
                if (ShouldSkipVersionEntry(filePath))
                {
                    continue;
                }

				//每个文件信息:Md5 获取它的大小 路径
				string md5 = MD5Helper.FileMD5(file);
				long size = fi.Length;

                //key是文件的路径 Value文件信息
				versionProto.FileInfoDict.Add(filePath, new FileVersionInfo
				{
					File = filePath,
					MD5 = md5,
					Size = size,
				});
			}

            //对资源AB包输出路径下的文件夹进行操作
			foreach (string directory in Directory.GetDirectories(dir).OrderBy(p => p, StringComparer.Ordinal))
			{
                //找到子文件夹 然后回调GenerateVersionProto 将文件夹路径传递进来
                DirectoryInfo dinfo = new DirectoryInfo(directory);
				string rel = relativePath == "" ? dinfo.Name : $"{relativePath}/{dinfo.Name}";
                if (ShouldSkipVersionEntry(rel))
                {
                    continue;
                }
				GenerateVersionProto($"{dir}/{dinfo.Name}", versionProto, rel);
			}
		}

        private static bool ShouldSkipVersionEntry(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return true;
            }

            string normalizedPath = relativePath.Replace('\\', '/').Trim('/');
            if (string.IsNullOrWhiteSpace(normalizedPath))
            {
                return true;
            }

            string[] segments = normalizedPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
            {
                return true;
            }

            string leafName = segments[segments.Length - 1];
            if (string.Equals(leafName, "Version.txt", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (leafName.EndsWith("-trace.txt", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            foreach (string segment in segments)
            {
                if (segment.StartsWith("backup-", StringComparison.OrdinalIgnoreCase) ||
                    segment.StartsWith("backup_", StringComparison.OrdinalIgnoreCase) ||
                    segment.IndexOf(".bak", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
	}
}
