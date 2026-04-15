using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.IO;
using System;
using UnityEditor;

namespace ETEditor
{
    public static class UpLoadFilesEditor
    {
        public static string sourceFilePath;
        public static string destinationFolderPath;// = "../../Release/LocalTest/Android/UpdateResources/";
        public static VersionConfig remoteVersionConfig;

        public static void InitPath() 
        {
            sourceFilePath = string.Format(BuildHelper.BuildFolder, GetPlatformName(EditorUserBuildSettings.activeBuildTarget));
            destinationFolderPath = sourceFilePath.Replace("StreamingAssets", "UpdateResources");
            Log.DebugGreen("sourceFilePath:"+ sourceFilePath);
            Log.DebugGreen("destinationFolderPath:" + destinationFolderPath);
        }

        //开始打包资源

        public static void CopyVersionTxt()
        {
           
           
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }
            FileHelper.CleanDirectory(destinationFolderPath);
            string versionPath = Path.Combine(sourceFilePath, "Version.txt");
           
            CopyFileToFolder(versionPath, destinationFolderPath);

            string fileContent = File.ReadAllText(versionPath);
            remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(fileContent);
            Log.DebugBrown($"旧资源版本号：{remoteVersionConfig.Version}");
            remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(fileContent);
        }

        //生成对比文件
        public static void CreatDuiBiFiles() 
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                sourceFilePath = string.Format(BuildHelper.BuildFolder, GetPlatformName(EditorUserBuildSettings.activeBuildTarget));
            }
            string versionPath = Path.Combine(sourceFilePath, "Version.txt");
            string fileContent = File.ReadAllText(versionPath);
            VersionConfig streamingVersionConfig = JsonHelper.FromJson<VersionConfig>(fileContent);
            Log.DebugBrown($"新资源版本号：{streamingVersionConfig.Version}");

            if (string.IsNullOrEmpty(destinationFolderPath))
            {
                destinationFolderPath = sourceFilePath.Replace("StreamingAssets", "UpdateResources");
            }
            string remoteversionPath = Path.Combine(destinationFolderPath, "Version.txt");
            string remotefileContent = File.ReadAllText(remoteversionPath);
            remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(remotefileContent);
            Log.DebugBrown($"旧资源版本号：{remoteVersionConfig.Version}");
            long size = 0;

            if (streamingVersionConfig.Version == remoteVersionConfig.Version)
            {
                Debug.LogError("Version相同");
                return;
            }

            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }
            FileHelper.CleanDirectory(destinationFolderPath);

            // 对比MD5
            foreach (FileVersionInfo fileVersionInfo in remoteVersionConfig.FileInfoDict.Values)
            {
                // 对比md5 跟我们本地的文件进行MD5对比
                string localFileMD5 = GetBundleMD5(streamingVersionConfig, fileVersionInfo.File);
                //如果相等 就忽略 表示两个版本中 这个文件并未做任何改动
                if (fileVersionInfo.MD5 == localFileMD5)
                {

                    continue;
                }
                else
                {
                    Log.DebugGreen($"文件名：{fileVersionInfo.File}");
                    size += fileVersionInfo.Size;
                    CopyFileToFolder(Path.Combine(sourceFilePath, fileVersionInfo.File), destinationFolderPath);
                }

            }
            Log.DebugGreen($"变动资源：{TextFromBytesSize(size)}");
        }

        public static void StartPack(string path) 
        {
            Log.DebugPurple("开始打包");
            sourceFilePath=path;
            string versionPath = Path.Combine(sourceFilePath, "Version.txt");
            string fileContent = File.ReadAllText(versionPath);
            remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(fileContent);
            Log.DebugBrown($"旧资源版本号：{remoteVersionConfig.Version}");

            destinationFolderPath = sourceFilePath.Replace("StreamingAssets", "UpdateResources");
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }
            FileHelper.CleanDirectory(destinationFolderPath);
        }
        public static void EndPack()
        {
            Log.DebugPurple("结束打包");
            // 获取streaming目录的Version.txt
            string versionPath = Path.Combine(sourceFilePath, "Version.txt");
            string fileContent = File.ReadAllText(versionPath);
            VersionConfig streamingVersionConfig = JsonHelper.FromJson<VersionConfig>(fileContent);
            Log.DebugBrown($"旧资源版本号:{remoteVersionConfig.Version}");
            Log.DebugBrown($"本地资源版本号:{streamingVersionConfig.Version}");
            FileHelper.CleanDirectory(destinationFolderPath);
            long size = 0;
            // 对比MD5
            foreach (FileVersionInfo fileVersionInfo in remoteVersionConfig.FileInfoDict.Values)
            {
                // 对比md5 跟我们本地的文件进行MD5对比
                string localFileMD5 = GetBundleMD5(streamingVersionConfig, fileVersionInfo.File);
                //如果相等 就忽略 表示两个版本中 这个文件并未做任何改动
                if (fileVersionInfo.MD5 == localFileMD5)
                {

                    continue;
                }
                else
                {
                    Log.DebugGreen($"文件名：{fileVersionInfo.File}");
                    size += fileVersionInfo.Size;
                    CopyFileToFolder(Path.Combine(sourceFilePath, fileVersionInfo.File), destinationFolderPath);
                }

            }
            Log.DebugGreen($"变动资源：{TextFromBytesSize(size)}");
        }
        public static string GetBundleMD5(VersionConfig streamingVersionConfig, string bundleName)
        {
            string path = Path.Combine(sourceFilePath, bundleName);
            if (File.Exists(path))
            {
                return MD5Helper.FileMD5(path);
            }
            if (streamingVersionConfig != null && streamingVersionConfig.FileInfoDict.ContainsKey(bundleName))
            {
                return streamingVersionConfig.FileInfoDict[bundleName].MD5;
            }

            return "";
        }

        public static void CopyFileToFolder(string sourceFilePath, string destinationFolderPath)
        {
            try
            {
                // 获取源文件名
                string fileName = Path.GetFileName(sourceFilePath);

                // 构建目标文件路径
                string destinationFilePath = Path.Combine(destinationFolderPath, fileName);

                // 复制文件
                File.Copy(sourceFilePath, destinationFilePath, true); // 如果目标文件已存在，覆盖原文件
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}");
            }
        }
        public static string TextFromBytesSize(long size_num)
        {
            long data_size = size_num;
            if (data_size < 1024)
            {
                // B
                return data_size + "B";
            }
            if (data_size / 1024 < 1024)
            {
                // KB
                return String.Format("{0:F2}KB", data_size / 1024f);
            }
            data_size /= 1024;
            if (data_size / 1024 < 1024)
            {
                // MB
                return String.Format("{0:F2}MB", data_size / 1024f);
            }
            data_size /= 1024;
            // G
            return String.Format("{0:F2}G", data_size / 1024f);
        }

        public static string GetPlatformName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                default:
                    return "PC";
            }
        }
    }
}