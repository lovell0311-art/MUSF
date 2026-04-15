using ETModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ETEditor
{

    /// <summary>
    /// 标记资源
    /// </summary>
    public class SetAbBundleEditor : EditorWindow
    {

        private readonly Dictionary<string, BundleInfo> dictionary = new Dictionary<string, BundleInfo>();


        [MenuItem("Tools/标记工具")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SetAbBundleEditor));

        }

        private void OnGUI()
        {
            if (Selection.objects.Length <= 0)
            {
                GUILayout.Label("请选择一个文件或文件夹！！！！");
            }
            else
            {
                GUILayout.Label("当前选中的文件夹：" + AssetDatabase.GetAssetPath(Selection.objects[0]));
            }

            GUILayout.Label("清理资源的ab标记");
            GUILayout.Label("清理所选文件夹的下的Ab标记");
            if (GUILayout.Button("清理ab标记"))
            {

                switch (EditorUtility.DisplayDialogComplex("警告!", $"确定要清理{GetSelectionDir()} AB标记？", "确定", "取消", ""))
                {
                    case 0:
                        break;
                    case 1:
                        return;
                    case 2:
                        return;
                }
                ClearPackingTagAndAssetBundle(GetSelectionDir());
            }
            GUILayout.Space(2);
            GUILayout.Label("清理所选文件的Ab标记");
            if (GUILayout.Button("清理单个资源的ab标记"))
            {
                switch (EditorUtility.DisplayDialogComplex("警告!", $"确定要清理{GetSelectionDir()} AB标记？", "确定", "取消", ""))
                {
                    case 0:
                        break;
                    case 1:
                        return;
                    case 2:
                        return;
                }
                ClearSinglePackingTagAndAssetBundle(GetSelectionDir());
            }
          
            GUILayout.Space(25);


            GUILayout.Label("整理资源");
            GUILayout.Label("获取依赖资源");
            if (GUILayout.Button("获取依赖"))
            {
                List<string> dependens = CollectDependencies(GetSelectionDir());

                foreach (var item in dependens)
                {

                    Debug.LogError("已存在：" + item);

                }
            }

           
            GUILayout.Space(30);
            GUILayout.Label("资源的ab标记  不标记对应的依赖资源");
            GUILayout.Label("标记当前所选文件夹下的资源 Ab名为->文件夹名");
            if (GUILayout.Button("根据文件夹 标记所选文件夹下的ab资源    (文件夹下的所有资源的AB名为->文件夹名)"))
            {
                SetRootBundleOnlyDir(GetSelectionDir());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            }
          
            GUILayout.Space(5);
            GUILayout.Label("标记单个资源，不分析共享资源 AB名为->prefab的名字");
            if (GUILayout.Button("标记单个ab资源"))
            {
                Debug.Log("开始标记资源");
                SetSingleBundleAndAtlas(GetSelectionDir());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                Debug.Log("标记完成");
            }

            GUILayout.Space(5);
            GUILayout.Label("会将目录下的每个prefab引用的资源打成一个包,只给顶层prefab打包 AB名为->prefab的名字");
            if (GUILayout.Button("标记文件下的各个资源（不标记对应的依赖资源） (AB名为->prefab的名字)"))
            {
                Debug.Log("开始标记资源");
                SetRootBundleOnly(GetSelectionDir());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                Debug.Log("标记完成");
            }

           
            


            GUILayout.Space(30);
            GUILayout.Label("资源的ab标记  标记对应的依赖资源");
            GUILayout.Space(10);
            GUILayout.Label("标记单个资源，不分析共享资源 （包括标记依赖资源） AB名为->prefab的名字");
            if (GUILayout.Button("标记单个ab资源 (（包括标记依赖资源）)"))
            {
                Debug.Log("开始标记资源");
                SetSingleIndependentBundleAndAtlas(GetSelectionDir());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                Debug.Log("标记完成");
            }

            GUILayout.Space(10);
            GUILayout.Label("分析共享资源 将目录下文件的共享资源打成一个共享资源包({dirName}-share) AB名为->prefab的名字");
            if (GUILayout.Button("标记资源(分析共享资源)"))
            {
                Debug.Log("开始分析标记资源");
                SetShareBundleAndAtlas(GetSelectionDir());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                Debug.Log("分析标记完成");
            }

            GUILayout.Space(10);
            GUILayout.Label("目录下的每个prefab引用的资源强制打成一个包（包含依赖资源），不分析共享资源 AB名为->prefab的名字");
            if (GUILayout.Button("标记目录下的每个资源(不分析共享资源) 会标记对应的依赖资源  AB名为->prefab的名字"))
            {
                Debug.Log("开始标记资源");
                SetIndependentBundleAndAtlas(GetSelectionDir());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                Debug.Log("标记完成");
            }


        }

        /// <summary>
        /// 得到当前所选文件的路径
        /// </summary>
        /// <returns></returns>
        public string GetSelectionDir()
        {
            if (Selection.objects.Length <= 0)
            {
                // EditorUtility.DisplayDialogComplex("警告!", $"请先选择一个文件夹！！", "切换", "取消", "不切换");
                Log.DebugRed("请先选择一个文件夹");
                return null;
            }
            else
            {
                GUILayout.Label("当前选中的文件夹：" + AssetDatabase.GetAssetPath(Selection.objects[0]));
                return AssetDatabase.GetAssetPath(Selection.objects[0]);
            }
        }
      

        /// <summary>
        /// 根据文件名 标记ab资源
        /// ab包的名字就是 文件夹的名字
        /// </summary>
        /// <param name="dir"></param>
        private static void SetRootBundleOnlyDir(string dir)
        {
            //获取该名字是为了等下给文件打AB包时，设置AB包名用的
            int index = dir.LastIndexOf("/");
            string lastName = dir.Substring(index + 1);//通过这个可以获取当前我们搜到的文件系统的名称Sence
            lastName += "s";
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                SetBundleAndAtlas(path1, lastName, true);
            }
        }
        /// <summary>
        /// 会将目录下的每个prefab引用的资源强制打成一个包，不分析共享资源
        /// </summary>
        /// <param name="dir"></param>
        private static void SetIndependentBundleAndAtlas(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                UnityEngine.Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

                AssetImporter importer = AssetImporter.GetAtPath(path1);
                if (importer == null || go == null)
                {
                    Debug.LogError("error: " + path1);
                    continue;
                }
                importer.assetBundleName = $"{go.name}.unity3d";

                //标记依赖资源
                List<string> pathes = CollectDependencies(path1);

                foreach (string pt in pathes)
                {
                    if (pt == path1)
                    {
                        continue;
                    }

                    SetBundleAndAtlas(pt, go.name, true);
                }
            }
        }
        /// <summary>
        /// 标记单个资源 包括标记依赖资源
        /// </summary>
        /// <param name="dir"></param>
        private static void SetSingleIndependentBundleAndAtlas(string dir)
        {

            string path1 = dir.Replace('\\', '/');
            Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);
            AssetImporter importer = AssetImporter.GetAtPath(path1);
            importer.assetBundleName = $"{go.name}.unity3d";
            //标记依赖资源
            List<string> pathes = CollectDependencies(path1);

            foreach (string pt in pathes)
            {
                if (pt == path1)
                {
                    continue;
                }

                SetBundleAndAtlas(pt, go.name, true);
            }
        }
        /// <summary>
        /// 标记单个资源 不包含依赖资源
        /// </summary>
        /// <param name="dir"></param>
        private static void SetSingleBundleAndAtlas(string dir)
        {
            string path1 = dir.Replace('\\', '/');
            Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);
            AssetImporter importer = AssetImporter.GetAtPath(path1);
            importer.assetBundleName = $"{go.name}.unity3d";
            
        }
        /// <summary>
        /// 获取依赖文件
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static List<string> CollectDependencies(string o)
        {
            string[] paths = AssetDatabase.GetDependencies(o);

            return paths.ToList();
        }

        /// <summary>
        /// 分析共享资源
        /// </summary>
        /// <param name="dir"></param>
        private  void SetShareBundleAndAtlas(string dir)
        {
            this.dictionary.Clear();
            //获取该文件夹下的所有预制体和场景
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);

            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);
                //设置ab包名
                SetBundle(path1, go.name);
                //获取改包的依赖文件
                List<string> pathes = CollectDependencies(path1);
                foreach (string pt in pathes)
                {
                    if (pt == path1)
                    {
                        //依赖文件在同意文件夹下 继续遍历下一个
                        continue;
                    }

                    // 不存在则记录下来
                    if (!this.dictionary.ContainsKey(pt))
                    {
                        // 如果已经设置了包
                        if (GetBundleName(pt) != "")
                        {
                            continue;
                        }
                        BundleInfo bundleInfo = new BundleInfo();
                        bundleInfo.ParentPaths.Add(path1);
                        this.dictionary.Add(pt, bundleInfo);

                        SetAtlas(pt, go.name);

                        continue;
                    }

                    // 依赖的父亲不一样
                    BundleInfo info = this.dictionary[pt];
                    if (info.ParentPaths.Contains(path1))
                    {
                        continue;
                    }
                    info.ParentPaths.Add(path1);

                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    string dirName = dirInfo.Name;

                    SetBundleAndAtlas(pt, $"{dirName}-share", true);
                }
            }
        }
        /// <summary>
        /// 获取ab包名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetBundleName(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".cs" || extension == ".dll" || extension == ".js")
            {
                return "";
            }
            if (path.Contains("Resources"))
            {
                return "";
            }

            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer == null)
            {
                return "";
            }

            return importer.assetBundleName;
        }
        /// <summary>
        /// 清理图集ab标识、PackingTag
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="issub">是否清理子文件夹</param>
        private static void ClearPackingTagAndAssetBundle(string dirPath, bool issub = true)
        {
            List<string> paths = EditorResHelper.GetAllResourcePath(dirPath, issub);
            foreach (string pt in paths)
            {
                SetBundleAndAtlas(pt, "", true);
            }

        }
        /// <summary>
        /// 清理单个资源的ab标记以及PackingTag
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="issub"></param>
        private static void ClearSinglePackingTagAndAssetBundle(string resPath, bool issub = true)
        {

            SetBundleAndAtlas(resPath, "", true);

        }
        /// <summary>
        /// 清理ab包的标记
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="issub"></param>
        private static void ClearAssetBundle(string dirPath, bool issub = true)
        {
            List<string> bundlePaths = EditorResHelper.GetAllResourcePath("Assets/Bundles/", true);
            foreach (string bundlePath in bundlePaths)
            {
                SetBundle(bundlePath, "", true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        private static void SetNoAtlas(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);

            foreach (string path in paths)
            {
                List<string> pathes = CollectDependencies(path);

                foreach (string pt in pathes)
                {
                    if (pt == path)
                    {
                        continue;
                    }
                    SetAtlas(pt, "", true);
                }
            }
        }
        /// <summary>
        /// 设置AB包名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="overwrite"></param>
        private static void SetBundle(string path, string name, bool overwrite = false)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".cs" || extension == ".dll" || extension == ".js")
            {
                return;
            }
            if (path.Contains("Resources"))
            {
                return;
            }

            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer == null)
            {
                return;
            }

            if (name != "")
                if (importer.assetBundleName != "" && overwrite == false && importer.assetBundleName.Contains(name))
                {
                    return;
                }

            string bundleName = "";
            if (name != "")
            {
                bundleName = $"{name}.unity3d";
            }

            importer.assetBundleName = bundleName;
        }


        // 会将目录下的每个prefab引用的资源强制打成一个包，不分析共享资源
        private static void SetBundles(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

                SetBundle(path1, go.name);
            }
        }


        /// <summary>
        /// 会将目录下的每个prefab引用的资源打成一个包,只给顶层prefab打包
        /// </summary>
        /// <param name="dir"></param>
        private static void SetRootBundleOnly(string dir)
        {
            List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

                SetBundle(path1, go.name);
            }
        }

        private static void SetBundleAndAtlas(string path, string name, bool overwrite = false)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".cs" || extension == ".dll" || extension == ".js")// || extension == ".mat"
            {
                return;
            }
            if (path.Contains("Resources"))
            {
                return;
            }

            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer == null)
            {
                
                return;
            }

            if (importer.assetBundleName == "" || overwrite)
            {
                string bundleName = "";
                if (name != "")
                {
                    bundleName = $"{name}.unity3d";
                }

                importer.assetBundleName = bundleName;
            }

            TextureImporter textureImporter = importer as TextureImporter;
            if (textureImporter == null)
            {
                return;
            }

            if (textureImporter.spritePackingTag == "" || overwrite)
            {
                textureImporter.spritePackingTag = name;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            }
        }

        /// <summary>
        /// 设置图集
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="overwrite"></param>

        private static void SetAtlas(string path, string name, bool overwrite = false)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".cs" || extension == ".dll" || extension == ".js")
            {
                return;
            }
            if (path.Contains("Resources"))
            {
                return;
            }

            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter == null)
            {
                return;
            }

            if (textureImporter.spritePackingTag != "" && overwrite == false)
            {
                return;
            }

            textureImporter.spritePackingTag = name;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        }
        /// <summary>
		/// 分析该目录下 子文件夹的共享资源
		/// </summary>
		/// <param name="srcPath"></param>
		private void SetSubDirShareBundleAndAtlas(string srcPath)
        {

            foreach (string subPath in Directory.GetDirectories(srcPath))
            {
                SetShareBundleAndAtlas(subPath);
            }
        }


    }
}
