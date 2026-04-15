using Aliyun.OSS.Common;
using Aliyun.OSS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ETModel;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;
using System;

namespace ETEditor
{
    public  class OSSFolderUploaderEditor: EditorWindow
    {
        // 在Inspector窗口中设置你的OSS信息
        public  string accessKeyId = "LTAI4GCLutFR8UtNjbZ8dTcD";
        public  string accessKeySecret = "nsmzCqdXgOFnl5mIAMtrzQO4o5QpHn";
        public  string endpoint = "https://oss-cn-shanghai.aliyuncs.com";
        public  string bucketName = "yongzhedalu";
        public  string ossDirectory = "ZhuZaiWuShuangRes/FuGu_DouYing/{0}/StreamingAssets"; // 目标目录
        // 设置本地文件夹路径
        string localFolderPath = "E:\\Miracle_Mu\\Release\\LocalTest\\Android\\UpdateResources";

        private void OnGUI()
        {
            localFolderPath = BuildHelper.BuildFolder.Replace("StreamingAssets", "UpdateResources");
            localFolderPath = EditorGUILayout.TextField("本地文件夹路径:", string.Format(localFolderPath, GetPlatformName(EditorUserBuildSettings.activeBuildTarget)));
            ossDirectory= EditorGUILayout.TextField("热更资源路径:", string.Format(ossDirectory, GetPlatformName(EditorUserBuildSettings.activeBuildTarget)));
            if (GUILayout.Button("上传资源"))
            {
                Start();
            }
        }

        public  void Start()
        {
            Log.DebugBrown("开始上传");
            // 初始化OSS客户端
            var config = new ClientConfiguration();
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret, config);

            long size = 0;
            // 获取文件夹中的所有文件
            string[] filePaths = Directory.GetFiles(localFolderPath);

            foreach (var filePath in filePaths)
            {
                // 获取文件名
                string objectKey = ossDirectory + "/" + Path.GetFileName(filePath);

                // 打开文件并将其内容读取到Stream
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    size += fs.Length;
                    // 创建上传请求
                    var putObjectRequest = new PutObjectRequest(bucketName, objectKey, fs);

                    try
                    {
                        // 执行上传
                        client.PutObject(putObjectRequest);
                        Debug.Log("File uploaded successfully: " + objectKey);
                    }
                    catch (OssException ex)
                    {
                        Debug.LogError("Failed to upload file. Error: " + ex.ErrorCode + " - " + ex.Message);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("Failed to upload file. Error: " + ex.Message);
                    }
                }
               
            }
            Log.DebugGreen($"资源上传完成:{TextFromBytesSize(size)}");
        }

        public string GetPlatformName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "IOS";
                default:
                    return "PC";
            }
        }
        public string TextFromBytesSize(long size_num)
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
    }
}