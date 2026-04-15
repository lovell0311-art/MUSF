using UnityEngine;
using System.IO;

/// <summary>
/// 功能： Assetbundle相关的通用静态函数，提供运行时，或者Editor中使用到的有关Assetbundle操作和路径处理的函数
/// </summary>

namespace ETModel
{
    public class AssetBundleUtility
    {
        /// <summary>
        /// 是否是编辑器模式
        /// </summary>
        /// <returns></returns>
        public static bool IsEditorMode()
        {
#if UNITY_EDITOR
            return AssetBundleConfig.IsEditorMode;
#else
            return false;
#endif
        }
        
        /// <summary>
        /// 获取当前平台名字
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        private static string GetPlatformName(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                default:
                    return "PC";
                    
            }
        }


        /// <summary>
        /// 相对StreamingAssets路径
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetStreamingAssetsFilePath(string assetPath = null)
        {
//#if UNITY_EDITOR

//#else
#if UNITY_IPHONE || UNITY_IOS
            string outputPath = Path.Combine("file://" + Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName);
#elif UNITY_ANDROID
            string outputPath = Path.Combine(Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName);
#else
            string outputPath = Path.Combine("file://" + Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName);
#endif
//#endif
            if (!string.IsNullOrEmpty(assetPath))
            {
                outputPath = Path.Combine(outputPath, assetPath);
            }
            return outputPath;
        }

        /// <summary>
        /// 相对StreamingAssets路径
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetStreamingAssetsDataPath(string assetPath = null)
        {
            string outputPath = Path.Combine(Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName);
            if (!string.IsNullOrEmpty(assetPath))
            {
                outputPath = Path.Combine(outputPath, assetPath);
            }
            return outputPath;
        }

        /// <summary>
        /// 相对Persistent路径
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetPersistentFilePath(string assetPath = null)
        {
            return "file://" + GetPersistentDataPath(assetPath);
        }

        /// <summary>
        /// 相对Persistent路径
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetPersistentDataPath(string assetPath = null)
        {
            string outputPath = Path.Combine(Application.persistentDataPath, AssetBundleConfig.AssetBundlesFolderName);
            if (!string.IsNullOrEmpty(assetPath))
            {
                outputPath = Path.Combine(outputPath, assetPath);
            }
#if UNITY_STANDALONE_WIN
            return GameUtility.FormatToSysFilePath(outputPath);
#else
            return outputPath;
#endif
        }

        public static string GetPersistentTempPath(string assetPath = null)
        {
            string outputPath = Path.Combine(Application.persistentDataPath, AssetBundleConfig.TempFolderName);
            if (!string.IsNullOrEmpty(assetPath))
            {
                outputPath = Path.Combine(outputPath, assetPath);
            }
#if UNITY_STANDALONE_WIN
            return GameUtility.FormatToSysFilePath(outputPath);
#else
            return outputPath;
#endif
        }
        
        public static bool CheckPersistentFileExsits(string filePath)
        {
            var path = GetPersistentDataPath(filePath);
            return File.Exists(path);
        }

        // 注意：这个路径是给WWW读文件使用的url，如果要直接磁盘写persistentDataPath，使用GetPlatformPersistentDataPath
        public static string GetAssetBundleFileUrl(string filePath)
        {
            if (CheckPersistentFileExsits(filePath))
            {
                return GetPersistentFilePath(filePath);
            }
            else
            {
                return GetStreamingAssetsFilePath(filePath);
            }
        }
        
        public static string GetAssetBundleDataPath(string filePath)
        {
            if (CheckPersistentFileExsits(filePath))
            {
                return GetPersistentDataPath(filePath);
            }
            else
            {
                return GetStreamingAssetsDataPath(filePath);
            }
        }
        
        public static string AssetBundlePathToAssetBundleName(string assetPath)
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                if (assetPath.StartsWith("Assets/"))
                {
                    assetPath = AssetsPathToPackagePath(assetPath);
                }
                //去掉" "
                assetPath = assetPath.Replace(" ", "");
                //there should not be any '.' in the assetbundle name
                //otherwise the variant handling in client may go wrong
                assetPath = assetPath.Replace(".", "_");
                //只要文件夹名字
                //string[] split = assetPath.Split('/');
                //add after suffix ".assetbundle" to the end
                //assetPath = split[split.Length - 1] + AssetBundleConfig.AssetBundleSuffix;
                assetPath = assetPath + AssetBundleConfig.AssetBundleSuffix;
                return assetPath.ToLower();
            }
            return null;
        }
        
        public static string PackagePathToAssetsPath(string assetPath)
        {
            return "Assets/" + AssetBundleConfig.AssetsFolderName + "/" + assetPath;
        }

        public static bool IsPackagePath(string assetPath)
        {
            string path = "Assets/" + AssetBundleConfig.AssetsFolderName + "/";
            return assetPath.StartsWith(path);
        }
        
        public static string AssetsPathToPackagePath(string assetPath)
        {
            string path = "Assets/" + AssetBundleConfig.AssetsFolderName + "/";
            if (assetPath.StartsWith(path))
            {
                return assetPath.Substring(path.Length);
            }
            else
            {
                Debug.LogError($"Asset path is not a package path! \n{assetPath}");
                return assetPath;
            }
        }
    }
}