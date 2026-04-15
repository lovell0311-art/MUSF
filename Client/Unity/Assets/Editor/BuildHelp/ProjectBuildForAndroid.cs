using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using LitJson;
using ETModel;

namespace ETEditor
{
    public class ProjectBuildForAndroid : EditorWindow
    {
        string JsonPath = "../SDK配置/Json";
        string FilePath = "../SDK配置/File";

        Dictionary<string, JsonData> Dic = new Dictionary<string, JsonData>();
        // 公用信息
        string ConfigureJsonName = "";
        string ProductName = "";
        string PacketageName = "";
        string Version = "";
        string BundleVersionCode = "";

        string PluginsPath = "";
        string ApkPath = "";

        static JsonData lookJson;

        public E_SDK e_SDK = E_SDK.NONE;

        ProjectBuildForAndroid()
        {
            this.titleContent = new GUIContent("ProjectBuildForAndroid");
            titleContent.text = "安卓设置";
            getConfigureJson();
        }

        [MenuItem("Tools/自定义打包")]
        public static void showWindow()
        {
            EditorWindow.GetWindow(typeof(ProjectBuildForAndroid));
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("自定义打包");
            GUILayout.Label(System.DateTime.Now + "");
            this.e_SDK = (E_SDK)EditorGUILayout.EnumPopup("E_SDK: ", this.e_SDK);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            if (Dic.Count >= 0)
            {
                foreach (KeyValuePair<string, JsonData> item in Dic)
                {
                    if (GUILayout.Button(item.Key.ToString().Split('.')[0], GUILayout.Width(200), GUILayout.ExpandWidth(true)))
                    {
                        lookJson = item.Value;
                        ShowSetting();
                    }
                }
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("查看信息", GUILayout.Height(60), GUILayout.Width(200)))
            {
                if (lookJson == null)
                {
                    this.ShowNotification(new GUIContent("暂无提示信息，请选择要打包的配置"));
                    return;
                }
                string txt = "";
                for (int i = 0; i < lookJson.Count; i++)
                {
                    txt += lookJson[i].ToString() + "\n";
                }
                this.ShowNotification(new GUIContent(txt));
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label("配置信息");
            ConfigureJsonName = EditorGUILayout.TextField("配置信息名称：", ConfigureJsonName);
            ProductName = EditorGUILayout.TextField("产品名字：", ProductName);
            PacketageName = EditorGUILayout.TextField("包名：", PacketageName);
            Version = EditorGUILayout.TextField("版本", Version);
            BundleVersionCode = EditorGUILayout.TextField("代码版本：", BundleVersionCode);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            PluginsPath = EditorGUILayout.TextField("Plugins路径：", PluginsPath);
            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                PluginsPath = EditorUtility.OpenFolderPanel("Plugins路径", Application.dataPath, ConfigureJsonName);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ApkPath = EditorGUILayout.TextField("Apk保存路径：", ApkPath);
            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                ApkPath = EditorUtility.OpenFolderPanel("APK保存路径", Application.dataPath, ConfigureJsonName);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("保存配置信息", GUILayout.Height(60), GUILayout.Width(200)))
            {
                SaveConfigureJson();
            }
            if (GUILayout.Button("修改配置信息", GUILayout.Height(60), GUILayout.Width(200)))
            {
                ChangeConfigureJson();
            }
            if (GUILayout.Button("替换Plugins文件", GUILayout.Height(60), GUILayout.Width(200)))
            {
                // CopyFile();
            }
            if (GUILayout.Button("开始打包", GUILayout.Height(60), GUILayout.Width(200)))
            {
                Build();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        void getConfigureJson()
        {
            DirectoryInfo dir = new DirectoryInfo(JsonPath);
            if (!dir.Exists || dir.GetFiles().Length == 0)
            {
                this.ShowNotification(new GUIContent("暂无配置信息"));
                return;
            }

            Dic.Clear();
            FileInfo[] Jsonfiles = dir.GetFiles();
            for (int i = 0; i < Jsonfiles.Length; i++)
            {
                string name = Jsonfiles[i].Name;
                string txt = File.ReadAllText(Jsonfiles[i].FullName, Encoding.UTF8);
                JsonData obj = JsonMapper.ToObject(txt);
                if (!Dic.ContainsKey(name))
                {
                    Dic.Add(name, obj);
                }
            }
            if (lookJson != null)
            {
                ShowSetting();
            }
        }

        void SaveConfigureJson()
        {
            if (Dic.ContainsKey(ConfigureJsonName + ".json"))
            {
                this.ShowNotification(new GUIContent("该配置已存在"));
                return;
            }
            if (!Directory.Exists(JsonPath))
            {
                Directory.CreateDirectory(JsonPath);
            }

            JsonData saveJson = new JsonData();
            saveJson["ConfigureJsonName"] = ConfigureJsonName;
            saveJson["ProductName"] = ProductName;
            saveJson["PacketageName"] = PacketageName;
            saveJson["Version"] = Version;
            saveJson["BundleVersionCode"] = BundleVersionCode;
            saveJson["PluginsPath"] = FilePath + "/" + ConfigureJsonName + "/Plugins/Android";
            saveJson["ApkPath"] = ApkPath;

            string str = saveJson.ToJson();
            Debug.Log(str);
            StreamWriter sw = new StreamWriter(JsonPath + "/" + ConfigureJsonName + ".json");
            sw.WriteLine(str);
            sw.Close();

            if (!Directory.Exists(FilePath + "/" + ConfigureJsonName))
            {
                Directory.CreateDirectory(FilePath + "/" + ConfigureJsonName + "/Plugins/Android");
            }

            getConfigureJson();
        }

        void ChangeConfigureJson()
        {
            if (!Dic.ContainsKey(ConfigureJsonName + ".json"))
            {
                this.ShowNotification(new GUIContent("该配置不存在，请直接保存配置信息"));
                return;
            }

            Dic.Remove(ConfigureJsonName + ".json");
            File.Delete(JsonPath + "/" + ConfigureJsonName + ".json");
            lookJson["ConfigureJsonName"] = ConfigureJsonName;
            lookJson["ProductName"] = ProductName;
            lookJson["PacketageName"] = PacketageName;
            lookJson["Version"] = Version;
            lookJson["BundleVersionCode"] = BundleVersionCode;
            lookJson["PluginsPath"] = PluginsPath;
            lookJson["ApkPath"] = ApkPath;

            string str = lookJson.ToJson();
            StreamWriter sw = new StreamWriter(JsonPath + "/" + ConfigureJsonName + ".json");
            sw.WriteLine(str);
            sw.Close();

            getConfigureJson();
        }

        void CopyFile()
        {
            string PluginsFilePath = FilePath + "/" + ConfigureJsonName + "/Plugins/Android";
            if (!Directory.Exists(PluginsFilePath))
            {
                this.ShowNotification(new GUIContent("要复制的Plugins文件不存在，请注意"));
                return;
            }

            string Pluginspath = Application.dataPath + "/Plugins/Android";
            if (Directory.Exists(Pluginspath))
            {
                FileAttributes attr = File.GetAttributes(Pluginspath);
                if (attr == FileAttributes.Directory)
                {
                    Directory.Delete(Pluginspath, true);
                }
                else
                {
                    File.Delete(Pluginspath);
                }
            }

            CopyOldLabFilesToNewLab(PluginsFilePath, Pluginspath);
            Debug.Log("Plugins替换完成");
        }

        void Build()
        {
            PlayerSettings.companyName = ConfigureJsonName;
            PlayerSettings.productName = ProductName;

            string keystorePath = @"E:\Miracle_Mu\Client\user.keystore";
            if (File.Exists(keystorePath))
            {
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = keystorePath;
                PlayerSettings.Android.keyaliasName = "user.keystore";
                PlayerSettings.Android.keystorePass = "888888";
                PlayerSettings.Android.keyaliasPass = "888888";
            }
            else
            {
                // Fall back to the default debug keystore for local device testing.
                PlayerSettings.Android.useCustomKeystore = false;
                Debug.LogWarning($"Keystore not found: {keystorePath}, using Unity debug keystore.");
            }

            PlayerSettings.applicationIdentifier = PacketageName;
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.Android.bundleVersionCode = int.Parse(BundleVersionCode);

            if (string.IsNullOrWhiteSpace(ApkPath))
            {
                ApkPath = Path.GetFullPath("../Release");
            }
            if (!Directory.Exists(ApkPath))
            {
                Directory.CreateDirectory(ApkPath);
            }

            string apkName = Path.Combine(ApkPath, $"{ProductName}.apk");

            BuildTarget buildTarget = BuildTarget.Android;
            // 切换到 Android 平台
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, buildTarget);

            AssetDatabase.Refresh();
            string[] levels = {
                "Assets/Scenes/Init.unity",
            };
            Log.Info("开始EXE打包");
            BuildPipeline.BuildPlayer(levels, apkName, buildTarget, BuildOptions.None);
            AssetDatabase.Refresh();
        }

        void ShowSetting()
        {
            ConfigureJsonName = lookJson["ConfigureJsonName"].ToString();
            ProductName = lookJson["ProductName"].ToString();
            PacketageName = lookJson["PacketageName"].ToString();
            Version = lookJson["Version"].ToString();
            BundleVersionCode = lookJson["BundleVersionCode"].ToString();
            PluginsPath = lookJson["PluginsPath"].ToString();
            ApkPath = lookJson["ApkPath"].ToString();
        }

        public void CopyOldLabFilesToNewLab(string sourcePath, string savePath)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            try
            {
                string[] labDirs = Directory.GetDirectories(sourcePath);
                string[] labFiles = Directory.GetFiles(sourcePath);
                if (labFiles.Length > 0)
                {
                    for (int i = 0; i < labFiles.Length; i++)
                    {
                        var str = labFiles[i];
                        if (str.Contains("sdk") || str.Contains("AndroidManifest") || str.Contains("mainTemplate"))
                        {
                            if (Path.GetExtension(labFiles[i]) != ".meta")
                            {
                                File.Copy(sourcePath + "\\" + Path.GetFileName(labFiles[i]), savePath + "\\" + Path.GetFileName(labFiles[i]), true);
                            }
                        }
                    }
                }
                if (labDirs.Length > 0)
                {
                    for (int j = 0; j < labDirs.Length; j++)
                    {
                        var str = labDirs[j];
                        if (str.Contains("sdk") || str.Contains("AndroidManifest") || str.Contains("mainTemplate"))
                        {
                            Directory.GetDirectories(sourcePath + "\\" + Path.GetFileName(labDirs[j]));
                            CopyOldLabFilesToNewLab(sourcePath + "\\" + Path.GetFileName(labDirs[j]), savePath + "\\" + Path.GetFileName(labDirs[j]));
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}