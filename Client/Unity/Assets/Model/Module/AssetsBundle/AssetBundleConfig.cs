using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

/// <summary>
/// 1、所有ab路径中目录、文件名不能以下划线打头，否则出包时StreamingAssets中的资源不能打到真机上，很坑爹
/// </summary>

namespace ETModel
{
    public class AssetBundleConfig
    {
        public const string localSvrAppPath = "Editor/AssetBundle/LocalServer/AssetBundleServer.exe";
        public const string AssetBundlesFolderName = "";
        public const string TempFolderName = "Temp";
        public const string AssetBundleSuffix = ".unity3d";
        public const string AssetsFolderName = "Bundles";
        public const string AssetBundleServerUrlFileName = "AssetBundleServerUrl.txt";
        public const string CommonMapPattren = ",";
        public static string BuildFolder = "../Release/{0}/StreamingAssets/";

        public static string GetLocalServerIP()
        {
            string ip = "localhost";
            return ip;
        }

        public static string GetAssetBundleServerURL()
        {
            string ip = GetLocalServerIP();
            string downloadURL = "http://" + ip + ":7888/";//:80
            downloadURL = downloadURL + "PC" + "/";
            return downloadURL;
        }      

#if UNITY_EDITOR
        public static string AssetBundlesBuildOutputPath
        {
            get
            {
                // string outputPath = Path.Combine(System.Environment.CurrentDirectory, AssetBundlesFolderName);
                string outputPath = "../Release/";//string.Format(BuildFolder, GetPlatformName(EditorUserBuildSettings.activeBuildTarget));
                GameUtility.CheckDirAndCreateWhenNeeded(outputPath);
                return outputPath;
            }
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

        public static string LocalSvrAppPath
        {
            get
            {
                return Path.Combine(Application.dataPath, localSvrAppPath);
            }
        }

        public static string LocalSvrAppWorkPath
        {
            get
            {
                return AssetBundlesBuildOutputPath;
            }
        }

        private static int mIsEditorMode = -1;
        private const string kIsEditorMode = "IsEditorMode";
        private static int mIsSimulateMode = -1;
        private const string kIsSimulateMode = "IsSimulateMode";

        public static bool IsEditorMode
        {
            get
            {
                if (mIsEditorMode == -1)
                {
                    if (!EditorPrefs.HasKey(kIsEditorMode))
                    {
                        EditorPrefs.SetBool(kIsEditorMode, false);
                    }
                    mIsEditorMode = EditorPrefs.GetBool(kIsEditorMode, true) ? 1 : 0;
                }

                return mIsEditorMode != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != mIsEditorMode)
                {
                    mIsEditorMode = newValue;
                    EditorPrefs.SetBool(kIsEditorMode, value);
                    if (value)
                    {
                        IsSimulateMode = false;
                    }
                }
            }
        }

        public static bool IsSimulateMode
        {
            get
            {
                if (mIsSimulateMode == -1)
                {
                    if (!EditorPrefs.HasKey(kIsSimulateMode))
                    {
                        EditorPrefs.SetBool(kIsSimulateMode, true);
                    }
                    mIsSimulateMode = EditorPrefs.GetBool(kIsSimulateMode, true) ? 1 : 0;
                }

                return mIsSimulateMode != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != mIsSimulateMode)
                {
                    mIsSimulateMode = newValue;
                    EditorPrefs.SetBool(kIsSimulateMode, value);

                    if (value)
                    {
                        IsEditorMode = false;
                    }
                }
            }
        }
#endif
    }
}
