using UnityEditor;
using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ETModel;

/// <summary>
/// 功能： 打包相关配置和通用函数
/// </summary>

namespace ETEditor
{
    public class PackageUtils
    {
        

        public static string GetCurrentMachineLocalIP()
        {
            try
            {
                // 注意：这里获取所有内网地址后选择一个最小的，因为可能存在虚拟机网卡
                var ips = new List<string>();
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ips.Add(ip.ToString());
                    }
                }
                ips.Sort();
                if (ips.Count <= 0)
                {
                    Debug.LogError("Get inter network ip failed!");
                }
                else
                {
                    return ips[0];
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Get inter network ip failed with err : " + ex.Message);
                Debug.LogError("Go Tools/Package to specify any machine as local server!!!");
            }
            return string.Empty;
        }

        

        public static string GetCurPlatformName()
        {
            return GetPlatformName(EditorUserBuildSettings.activeBuildTarget);
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

        public static BuildTargetGroup GetTargetGroup(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                default:
                    return BuildTargetGroup.Standalone;
            }
        }

        public static string[] GetTargetGroup(BuildTarget buildTarget , bool IsSimulate)
        {
            List<string> define = new List<string>();
            switch (buildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                    define.Add("NET452");
                    define.Add("ILRuntime");
                    break;
                default:
                    break;
            }
            if (IsSimulate)
            {
                define.Add("ASYNC");
            }
            return define.ToArray();
        }

        public static string GetAssetBundleOutputPath(BuildTarget target)
        {
            string outputPath = string.Format(BuildHelper.BuildFolder, GetPlatformName(target));
            GameUtility.CheckDirAndCreateWhenNeeded(outputPath);
            return outputPath;
        }
        
        public static string GetAssetbundleManifestPath(BuildTarget target)
        {
            string outputPath = GetAssetBundleOutputPath(target);
            return Path.Combine(outputPath, BuildUtils.ManifestBundleName);
        }

        

        public static string GetCurBuildSettingAssetBundleOutputPath()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            return GetAssetBundleOutputPath(buildTarget);
        }

        public static string GetCurBuildSettingAssetBundleManifestPath()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            return GetAssetbundleManifestPath(buildTarget);
        }

        public static string GetCurBuildSettingStreamingManifestPath()
        {
            string path = AssetBundleUtility.GetStreamingAssetsDataPath();
            path = Path.Combine(path, BuildUtils.ManifestBundleName);
            return path;
        }

        public static AssetBundleManifest GetManifestFormLocal(string manifestPath)
        {
            FileInfo fileInfo = new FileInfo(manifestPath);
            if (!fileInfo.Exists)
            {
                Debug.LogError("You need to build assetbundles first to get assetbundle dependencis info!");
                return null;
            }
            byte[] bytes = GameUtility.SafeReadAllBytes(fileInfo.FullName);
            if (bytes == null)
            {
                return null;
            }
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(bytes);
            AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            assetBundle.Unload(false);
            return manifest;
        }

        public static void CopyAssetBundlesToStreamingAssets(BuildTarget buildTarget)
        {
            string source = GetAssetBundleOutputPath(buildTarget);
            string destination = AssetBundleUtility.GetStreamingAssetsDataPath();
            // 有毒，竟然在有的windows系统这个函数删除不了目录，不知道是不是Unity的Bug
            // GameUtility.SafeDeleteDir(destination);
            AssetDatabase.DeleteAsset(GameUtility.FullPathToAssetPath(destination));
            AssetDatabase.Refresh();

            try
            {
                FileUtil.CopyFileOrDirectoryFollowSymlinks(source, destination);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Something wrong, you need manual delete AssetBundles folder in StreamingAssets, err : " + ex);
                return;
            }

            var allManifest = GameUtility.GetSpecifyFilesInFolder(destination, new string[] { ".manifest" });
            if (allManifest != null && allManifest.Length > 0)
            {
                for (int i = 0; i < allManifest.Length; i++)
                {
                    GameUtility.SafeDeleteFile(allManifest[i]);
                }
            }

            AssetDatabase.Refresh();
        }

        public static void CopyCurSettingAssetBundlesToStreamingAssets()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            CopyAssetBundlesToStreamingAssets(buildTarget);
            Debug.Log("Copy channel assetbundles to streaming assets done!");
        }

        public static void CheckAndAddSymbolIfNeeded(BuildTarget buildTarget, string targetSymbol)
        {
            if (buildTarget != BuildTarget.Android && buildTarget != BuildTarget.iOS)
            {
                Debug.LogError("Only support Android and IOS !");
                return;
            }

            var buildTargetGroup = buildTarget == BuildTarget.Android ? BuildTargetGroup.Android : BuildTargetGroup.iOS;
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (!symbols.Contains("HOTFIX_ENABLE"))
            {
                symbols = string.Format("{0};{1};", symbols, "HOTFIX_ENABLE");
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
        }

     

        public static void CopyAndroidSDKResources(string channelName)
        {
            string targetPath = Path.Combine(Application.dataPath, "Plugins");
            targetPath = Path.Combine(targetPath, "Android");
            GameUtility.SafeClearDir(targetPath);

            string channelPath = Path.Combine(Environment.CurrentDirectory, "Channel");
            string resPath = Path.Combine(channelPath, "UnityCallAndroid_" + channelName);
            if (!Directory.Exists(resPath))
            {
                resPath = Path.Combine(channelPath, "UnityCallAndroid");
            }

            EditorUtility.DisplayProgressBar("提示", "正在拷贝SDK资源，请稍等", 0f);
            PackageUtils.CopyJavaFolder(resPath + "/assets", targetPath + "/assets");
            EditorUtility.DisplayProgressBar("提示", "正在拷贝SDK资源，请稍等", 0.3f);
            PackageUtils.CopyJavaFolder(resPath + "/libs", targetPath + "/libs");
            EditorUtility.DisplayProgressBar("提示", "正在拷贝SDK资源，请稍等", 0.6f);
            PackageUtils.CopyJavaFolder(resPath + "/res", targetPath + "/res");
            if (File.Exists(resPath + "/bin/UnityCallAndroid.jar"))
            {
                File.Copy(resPath + "/bin/UnityCallAndroid.jar", targetPath + "/libs/UnityCallAndroid.jar", true);
            }
            if (File.Exists(resPath + "/AndroidManifest.xml"))
            {
                File.Copy(resPath + "/AndroidManifest.xml", targetPath + "/AndroidManifest.xml", true);
            }

            EditorUtility.DisplayProgressBar("提示", "正在拷贝SDK资源，请稍等", 1f);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static void CopyJavaFolder(string source, string destination)
        {
            if (!Directory.Exists(source))
            {
                return;
            }
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
                AssetDatabase.Refresh();
            }

            string[] sourceDirs = Directory.GetDirectories(source);
            for (int i = 0; i < sourceDirs.Length; i++)
            {
                CopyJavaFolder(sourceDirs[i] + "/", destination + "/" + Path.GetFileName(sourceDirs[i]));
            }

            string[] sourceFiles = Directory.GetFiles(source);
            for (int j = 0; j < sourceFiles.Length; j++)
            {
                if (sourceFiles[j].Contains("classes.jar"))
                {
                    continue;
                }
                File.Copy(sourceFiles[j], destination + "/" + Path.GetFileName(sourceFiles[j]), true);
            }
        }

        public static string[] ReadCurAppAndResVersionFile()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;

            return ReadAppAndResVersionFile(buildTarget);
        }

        public static string IncreaseResSubVersion(string versionstr)
        {
            // 每一次构建资源，子版本号自增，注意：前两个字段这里不做托管，自行编辑设置
            string[] vers = versionstr.Split('.');
            if (vers.Length > 0)
            {
                int subVer = 0;
                int.TryParse(vers[vers.Length - 1], out subVer);
                //            vers[vers.Length - 1] = string.Format("{0:D3}", subVer + 1);
                vers[vers.Length - 1] = (subVer + 1).ToString();
            }
            versionstr = string.Join(".", vers);
            return versionstr;
        }

        public static string[] ReadAppAndResVersionFile(BuildTarget buildTarget)
        {
            // 从资源版本号文件（当前渠道AB输出目录中）加载资源版本号
            string rootPath = GetAssetBundleOutputPath(buildTarget);

            string app_path = rootPath + "/" + BuildUtils.AppVersionFileName;
            app_path = GameUtility.FormatToUnityPath(app_path);
        //    string resVersion = "0.0.0";

            string content = GameUtility.SafeReadAllText(app_path);
            if (content != null)
            {
                var arr = content.Split('|');
                if (arr.Length >= 3)
                {
                    return arr;
                }
            }
            Debug.LogError("找不到 appVersion.bytes 文件");
            return null;
        }
    }
}