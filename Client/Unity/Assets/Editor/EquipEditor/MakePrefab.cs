using ETModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using static NPOI.HSSF.Util.HSSFColor;
using static UnityEditor.Progress;
using Object = UnityEngine.Object;

namespace ETEditor
{
    public class MakePrefab : EditorWindow
    {
        [MenuItem("Assets/制作预制体")]
        public static void CreatePrefabWindow()
        {
            EditorWindow window = EditorWindow.GetWindowWithRect(typeof(MakePrefab), new Rect(Screen.width / 3, Screen.height / 3, 800, 500), true, "MakePrefab");
            window.Show();
        }

        private static string toSavePrefabPath = "Assets/Bundles/Props/Others";
        private static string PrefabPrefixName= "Other";
        bool isAdd;
        bool isWing;
        bool isBack;

        private void OnGUI()
        {
           // EditorGUILayout.LabelField("资源路径：",EquipsEditor.GetSelectionDir(), GUILayout.Width(110));
            EditorGUILayout.LabelField("资源路径：",EquipsEditor.GetSelectionDir());
            EditorGUILayout.LabelField("预制体保存路径：",toSavePrefabPath, GUILayout.Width(110));
            toSavePrefabPath = EditorGUILayout.TextArea(toSavePrefabPath, GUILayout.Width(250));
            EditorGUILayout.LabelField("预制体前缀：", PrefabPrefixName, GUILayout.Width(110));
            PrefabPrefixName = EditorGUILayout.TextArea(PrefabPrefixName, GUILayout.Width(250));
            this.isAdd = EditorGUILayout.Toggle("是否添加动画控制器: ", this.isAdd);
            this.isWing = EditorGUILayout.Toggle("添加待机动画控制器: ", this.isWing);
            this.isBack = EditorGUILayout.Toggle("关闭Back: ", this.isBack);


            if (GUILayout.Button("转换预制体", GUILayout.Width(260)))
            {
                if(!string.IsNullOrEmpty(PrefabPrefixName))
                EquipsEditor.RenameEquips(PrefabPrefixName);//修改物品的名字

                ToPrafab();
            }
            if (GUILayout.Button("修改预制体", GUILayout.Width(260)))
            { 
            
            }
        }
        private void ToPrafab() 
        {
            //文件夹 不存在 就创建一个
            if (!Directory.Exists(toSavePrefabPath))
            {
                Directory.CreateDirectory(toSavePrefabPath);
            }

            List<string> paths = GetPrefabsAndScenes(EquipsEditor.GetSelectionDir());
            Log.DebugGreen($"paths:{paths.Count}");
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                Log.DebugGreen($"{path1}");
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                 Log.DebugBrown($"name:{go.name}");
                if (go.name.Contains("-"))
                {
                    string name = go.name.Split('-')[1];
                    go.name = name;
                }
                go.transform.position = Vector3.zero;
              
                go = PrefabUtility.InstantiatePrefab(go) as GameObject;
                PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                ///带动画
                if (go.transform.childCount > 0 && go.transform.Find("SMDImport") is Transform SMDImport_world)
                {
                    Transform UI_SMDImport = SMDImport_world;

                    GameObject gameObject = new GameObject("World");
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localScale = Vector3.one;
                    for (int i = go.transform.childCount-1; i>=0; i--)
                    {
                        go.transform.GetChild(i).SetParent(gameObject.transform,false);
                    }
                    gameObject.transform.SetParent(go.transform, false);
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.transform.localScale = Vector3.one;

                    if (isAdd)
                    {
                        Animator animator = gameObject.gameObject.AddComponent<Animator>();
                        //创建animationController文件，保存在animatorControllerpath路径下
                        string animatorControllerpath = path1.Replace($"FBX", $"controller");
                        Log.DebugGreen($"animatorControllerpath:{animatorControllerpath}");
                        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerpath);
                        //得到它的Layer， 默认layer为base 你可以去拓展
                        AnimatorControllerLayer layer = animatorController.layers[0];
                        //把动画文件保存在我们创建的AnimationController中
                        AnimationClip clip = AssetDatabase.LoadAssetAtPath(path1, typeof(AnimationClip)) as AnimationClip;
                        //添加 参数
                        animatorController.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

                        AnimatorStateMachine sm = layer.stateMachine;  //状态机

                        // 先添加一个默认的空状态
                        var emptyState = sm.AddState("empty", new Vector3(300, 0, 0));
                        sm.AddEntryTransition(emptyState);

                        // 取出动画名字，添加到state里面
                        AnimatorState state = sm.AddState("Attack", new Vector3(500, sm.states.Length * 60, 0)); //将动画添加到动画控制器
                        state.motion = clip;
                        sm.AddAnyStateTransition(state).AddCondition(AnimatorConditionMode.If, 0f, "Attack");//参数1-》 触发状态 （如果状态 参数是1 即触发转换） 参数2-》阈值 不知道干啥 参数3-》条件的名字
                        animator.runtimeAnimatorController = animatorController;
                    }
                    if (isWing)
                    {
                        Animator animator = gameObject.gameObject.AddComponent<Animator>();
                        //创建animationController文件，保存在animatorControllerpath路径下
                        string animatorControllerpath = path1.Replace($"FBX", $"controller");
                        Log.DebugGreen($"animatorControllerpath:{animatorControllerpath}");
                        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerpath);
                        //得到它的Layer， 默认layer为base 你可以去拓展
                        AnimatorControllerLayer layer = animatorController.layers[0];
                        //把动画文件保存在我们创建的AnimationController中
                        AnimationClip clip = AssetDatabase.LoadAssetAtPath(path1, typeof(AnimationClip)) as AnimationClip;
                        AnimationUtility.GetAnimationClipSettings(clip).loopTime = true;
                       
                        AnimatorStateMachine sm = layer.stateMachine;  //状态机

                        // 先添加一个默认的空状态
                        var emptyState = sm.AddState("empty", new Vector3(300, 0, 0));
                        emptyState.motion = clip;
                        sm.AddEntryTransition(emptyState);
                        animator.runtimeAnimatorController = animatorController;
                    }
                    //UI
                    Transform ui = GameObject.Instantiate<Transform>(UI_SMDImport, go.transform);
                    ui.localPosition = Vector3.zero;
                    ui.localScale = Vector3.one;
                    if (ui.GetComponent<SkinnedMeshRenderer>() != null)
                    {
                        SkinnedMeshRenderer skinnedMesh = ui.GetComponent<SkinnedMeshRenderer>();
                        ui.gameObject.AddComponent<MeshFilter>().mesh = skinnedMesh.sharedMesh;
                        ui.gameObject.AddComponent<MeshRenderer>().materials = skinnedMesh.sharedMaterials;
                        GameObject.DestroyImmediate(skinnedMesh);
                    }
                    ui.name = "UI";
                    ui.gameObject.SetLayer(LayerNames.UI);
                    //Back
                    if (isBack == false)
                    {
                        Transform back = GameObject.Instantiate<Transform>(ui.transform, go.transform);
                        back.localPosition = Vector3.zero;
                        back.localScale = Vector3.one;
                        back.name = "Back";
                        back.gameObject.SetLayer(LayerNames.ROLE);
                    }
                }
                else
                {
                    ///不带动画
                    if (go.transform.childCount > 0 && go.transform.GetChild(0) is Transform world)
                    {
                        if (world.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            SkinnedMeshRenderer skinnedMesh = world.GetComponent<SkinnedMeshRenderer>();
                            world.gameObject.AddComponent<MeshFilter>().mesh = skinnedMesh.sharedMesh;

                            world.gameObject.AddComponent<MeshRenderer>().materials = skinnedMesh.sharedMaterials;

                            GameObject.DestroyImmediate(skinnedMesh);
                        }
                        world.name = "World";
                        Transform ui = GameObject.Instantiate<Transform>(world, go.transform);
                        ui.name = "UI";
                        ui.gameObject.SetLayer(LayerNames.UI);
                        //Back
                        //Back
                        if (isBack == false)
                        {
                            Transform back = GameObject.Instantiate<Transform>(world.transform, go.transform);
                            back.name = "Back";
                            back.gameObject.SetLayer(LayerNames.ROLE);
                        }
                    }
                    else
                    {
                        GameObject gameObject = new GameObject("World");
                        gameObject.transform.SetParent(go.transform);
                        if (go.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            SkinnedMeshRenderer skinnedMesh = go.GetComponent<SkinnedMeshRenderer>();
                            gameObject.gameObject.AddComponent<MeshFilter>().mesh = skinnedMesh.sharedMesh;
                            gameObject.gameObject.AddComponent<MeshRenderer>().materials = skinnedMesh.sharedMaterials;
                            GameObject.DestroyImmediate(skinnedMesh);
                        }
                        else
                        {
                            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
                           
                            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                            gameObject.AddComponent<MeshFilter>().mesh = mesh;
                            gameObject.AddComponent<MeshRenderer>().materials = meshRenderer.sharedMaterials;

                            go.GetComponent<MeshFilter>().mesh = null;
                            go.GetComponent<MeshRenderer>().material = null;
                             
                        }

                        Transform ui = GameObject.Instantiate<Transform>(gameObject.transform, go.transform);
                        ui.name = "UI";
                        ui.gameObject.SetLayer(LayerNames.UI);

                        //Back
                        if (isBack == false)
                        {
                            Transform back = GameObject.Instantiate<Transform>(gameObject.transform, go.transform);
                            back.name = "Back";
                            back.gameObject.SetLayer(LayerNames.ROLE);
                        }
                    }
                }

                
                // PrefabUtility.SaveAsPrefabAsset(go, $"{toSavePrefabPath}/{go.name}.prefab");

                if (go.GetComponent<MeshFilter>() is MeshFilter meshFilter)
                { 
                 GameObject.DestroyImmediate(meshFilter);
                    
                }
                if (go.GetComponent<MeshRenderer>() is MeshRenderer renderer)
                {
                    GameObject.DestroyImmediate(renderer);
                }
                go.transform.rotation = Quaternion.identity;


                ///修改预制体的名字
                if (!string.IsNullOrEmpty(PrefabPrefixName))
                    go.name = PrefabPrefixName+"_"+go.name;

                  Log.DebugRed($"{toSavePrefabPath}/{go.name}.prefab");
                PrefabUtility.SaveAsPrefabAssetAndConnect(go, $"{toSavePrefabPath}/{go.name}.prefab", InteractionMode.AutomatedAction);
                DestroyImmediate(go);
              
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

       
        public static List<string> GetPrefabsAndScenes(string srcPath)
        {
            List<string> paths = new List<string>();
            FileHelper.GetAllFiles(paths, srcPath);

            List<string> files = new List<string>();
            foreach (string str in paths)
            {
                if (str.EndsWith(".FBX"))
                {
                    files.Add(str);
                }
            }
            return files;
        }


        [MenuItem("Assets/材质球/一键生成材质球")]

        static void CreateMaterialsFromFBX()
        {
            //string[] subDirs = Directory.GetDirectories(dir);
            UnityEngine.Object[] gameObjects = Selection.objects;
            string[] strs = Selection.assetGUIDs;

            if (gameObjects.Length > 0)
            {
                if (gameObjects.Length > 1)
                {
                    Debug.LogError("不支持多选，请选择一个fbx!");
                }
                else
                {
                    Log.DebugGreen($"{strs[0]}");
                    string assetPath = AssetDatabase.GUIDToAssetPath(strs[0]);
                    Log.DebugGreen($"{Path.GetDirectoryName(assetPath)}");
                    string materialFolder = Path.GetDirectoryName(assetPath) + "/Materials";
                    // 如果不存在该文件夹则创建一个新的
                    if (!AssetDatabase.IsValidFolder(materialFolder))
                    {
                        AssetDatabase.CreateFolder(Path.GetDirectoryName(assetPath), "Materials");
                    }
                    // 获取 assetPath 下所有资源。
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                    bool isCreate = false;
                    foreach (Object item in assets)
                    {
                        if (typeof(Material) == item?.GetType())
                        {
                            //Material #36
                            
                            Debug.Log("xxxxxxxxx=====" + item +"Name: "+ (item as Material).mainTexture.name);
                            //string maName = item.name.Replace(" ", "").Replace("#","_");
                            string maName = (item as Material).mainTexture.name;
                            //  string path = System.IO.Path.Combine(materialFolder, item.name) + ".mat";
                            string path = System.IO.Path.Combine(materialFolder, maName) + ".mat";
                            // 为资源创建一个新的唯一路径。
                            path = AssetDatabase.GenerateUniqueAssetPath(path);
                            // 通过在导入资源(例如，FBX 文件)中提取外部资源，在对象(例如，材质)中创建此资源。
                            if (item is Material material)
                            {
                                material.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");//Mobile/Unlit (Supports Lightmap)
                            }
                            string value = AssetDatabase.ExtractAsset(item, path);
                            // 成功提取( 如果 Unity 已成功提取资源，则返回一个空字符串)
                            if (string.IsNullOrEmpty(value))
                            {
                                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
                                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                                isCreate = true;
                            }
                        }
                    }

                    AssetDatabase.Refresh();
                    if (isCreate)
                        Debug.Log("自动创建材质球成功：" + materialFolder);
                    else
                        Debug.Log("自动创建材质球失败！");

                }
            }
            else
            {
                Debug.LogError("请选中需要一键生成材质球的模型");
            }
        }

        [MenuItem("Assets/材质球/一键生成多个材质球")]

        static void CreateMaterialsFromFBXS()
        {
            string[] subDirs = Directory.GetDirectories(AssetDatabase.GetAssetPath(Selection.objects[0]));
            foreach (string assetPath in subDirs)
            {
                Log.DebugBrown(assetPath);
                List<string> paths = GetPrefabsAndScenes(assetPath);
                foreach (string path in paths)
                {
                    string materialFolder = assetPath + "/Materials";
                    // 如果不存在该文件夹则创建一个新的
                    if (!AssetDatabase.IsValidFolder(materialFolder))
                    {
                        AssetDatabase.CreateFolder(assetPath, "Materials");
                    }
                        // 获取 assetPath 下所有资源。
                        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                        foreach (Object item in assets)
                        {
                            if (typeof(Material) == item?.GetType())
                            {
                                //Material #36
                                Debug.Log("xxxxxxxxx=====" + item);
                               // string maName = item.name.Replace(" ", "").Replace("#", "_");
                                string maName = (item as Material).mainTexture.name;
                            string path_ = System.IO.Path.Combine(materialFolder, maName) + ".mat";
                                // 为资源创建一个新的唯一路径。
                                path_ = AssetDatabase.GenerateUniqueAssetPath(path_);
                            // 通过在导入资源(例如，FBX 文件)中提取外部资源，在对象(例如，材质)中创建此资源。
                            if (item is Material material)
                            {
                                material.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
                            }
                                string value = AssetDatabase.ExtractAsset(item, path_);
                                // 成功提取( 如果 Unity 已成功提取资源，则返回一个空字符串)
                                if (string.IsNullOrEmpty(value))
                                {
                                    AssetDatabase.WriteImportSettingsIfDirty(path);
                                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                                }
                            }
                        }
                        AssetDatabase.Refresh();
                   
                }
            }

        }

    }
}