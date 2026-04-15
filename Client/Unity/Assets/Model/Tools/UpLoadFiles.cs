using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;
using System.Security.Policy;

namespace ETModel
{

    public class UpLoadFilePath
    {
        public string sourceFilePath;
        //获取资源路径
       /* public string GetUrl()
        {

            string url = string.Format(sourceFilePath, GetPlatformName());
            return url;
        }

        public string GetPlatformName()
        {

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                default:
                    return "PC";
            }
        }*/
    }


    public class UpLoadFiles:MonoBehaviour
    {

        private static VersionConfig remoteVersionConfig;
        public static Queue<string> bundles;

        public string sourceFilePath ="../../Release/LocalTest/Android/StreamingAssets/";
        public string destinationFolderPath = "../../Release/LocalTest/Android/UpdateResources/";

        private void Start()
        {
           /* Log.DebugGreen($"UpLoadFilePath.sourceFilePath:{UpLoadFilePath.sourceFilePath}");
            destinationFolderPath = UpLoadFilePath.sourceFilePath.Replace("StreamingAssets", "UpdateResources");*/
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }
        }

        public void StartUp() 
        {
            StartCoroutine("StartAsync");
        }

        public  IEnumerator StartAsync()
        {
            Log.DebugPurple("生成对比文件");
            // 获取远程的Version.txt
           
                string versionUrl = GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + "Version.txt";
                using (UnityWebRequest webRequest = UnityWebRequest.Get(versionUrl))
                {
                    yield return webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                    remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(webRequest.downloadHandler.text);
                    // 成功接收响应
                   Log.DebugBrown("热更资源版本号:" + remoteVersionConfig.Version);
                    }
                    else
                    {
                        // 处理错误
                        Debug.LogError("Error: " + webRequest.error);
                    }
                }  
            
           

            // 获取streaming目录的Version.txt
            string versionPath = Path.Combine(sourceFilePath, "Version.txt");
            string fileContent = File.ReadAllText(versionPath);
            VersionConfig streamingVersionConfig = JsonHelper.FromJson<VersionConfig>(fileContent);
            Log.DebugBrown($"本地资源版本号：{streamingVersionConfig.Version}");
           
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
                    CopyFileToFolder(Path.Combine(sourceFilePath, fileVersionInfo.File), destinationFolderPath);
                }
                
                //如果两个文件的MD5不一致 把要下载的文件 压入到队列
                //bundles.Enqueue(fileVersionInfo.File);
            }
        }
        public string GetBundleMD5(VersionConfig streamingVersionConfig, string bundleName)
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
                Console.WriteLine("文件复制成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}");
            }
        }

    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(UpLoadFiles))]
    public class MapJsonCreateHelper : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("对比文件"))
            {
                var component = target as UpLoadFiles;
                component.StartUp();
            }
        }
    }
    #endif
}
