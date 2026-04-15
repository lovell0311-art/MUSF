using ETModel;
using NPOI.POIFS.Properties;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ETEditor
{

    /// <summary>
    /// 武器 预制体编辑工具
    /// </summary>

    public class WeaponEditor : EditorWindow
    {
        [MenuItem("Assets/设置武器 预制体")]
        public static void CreatePrefabWindow()
        {
            WeaponEditor window = (WeaponEditor)EditorWindow.GetWindow(typeof(WeaponEditor), true, "武器编辑");
            window.Show();
        }


        public string selectPath;

        Vector3 World_pos = new Vector3(0.08f,-0.1f,0);
        Vector3 World_rot =new Vector3(-260,45,-120);
        Vector3 World_scale = Vector3.one;

        Vector3 UI_pos =new Vector3(-0.13f,-0.7f,0);
        Vector3 UI_rot = new Vector3(-170,90,-10);
        Vector3 UI_scale = Vector3.one*.4f;


        Vector3 Back_pos = Vector3.zero;
        Vector3 Back_rot = Vector3.zero;
        Vector3 Back_scale = Vector3.one;

        private void OnGUI()
        {

            EditorGUILayout.LabelField("当前所选文件夹：", GetSelectionDir());
            selectPath = EditorGUILayout.TextArea(selectPath);
            EditorGUILayout.LabelField("World 信息");
            World_pos = EditorGUILayout.Vector3Field("World-Pos:", World_pos);
            World_rot = EditorGUILayout.Vector3Field("World_rot:", World_rot);
            World_scale = EditorGUILayout.Vector3Field("World_scale:", World_scale);
            if (GUILayout.Button("设置预制体_World", GUILayout.Width(260)))
            {
                List<string> paths = EquipsEditor.GetPrefabsAndScenes(selectPath);
               
                foreach (string path in paths)
                {
                    string path1 = path.Replace('\\', '/');
                    //当前预制体
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                    Log.DebugGreen($"{go.name}  ->{path1}");
                    if (go != null)
                    {
                      
                       if (go.transform.Find("World") is Transform world)
                        {
                            world.position = World_pos;
                            world.rotation = Quaternion.Euler(World_rot);
                            world.localScale = World_scale;


                        }
                        if (go.transform.Find("Stage_1") is Transform Stage_1)
                        {

                            Stage_1.position = World_pos;
                            Stage_1.rotation = Quaternion.Euler(World_rot);
                            Stage_1.localScale = World_scale;
                        } 
                        if (go.transform.Find("Stage_2") is Transform Stage_2)
                        {
                            Stage_2.position = World_pos;
                            Stage_2.rotation = Quaternion.Euler(World_rot);
                            Stage_2.localScale = World_scale;
                        }
                        //PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);
                        PrefabUtility.SavePrefabAsset(go);

                        //GameObject.DestroyImmediate(go);
                    }
                }
                //保存修改
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("UI 信息");
            UI_pos = EditorGUILayout.Vector3Field("UI_pos:", UI_pos);
            UI_rot = EditorGUILayout.Vector3Field("UI_rot:", UI_rot);
            UI_scale = EditorGUILayout.Vector3Field("UI_scale:", UI_scale);
            if (GUILayout.Button("修改预制体_UI", GUILayout.Width(260)))
            {
                List<string> paths = EquipsEditor.GetPrefabsAndScenes(selectPath);
                foreach (string path in paths)
                {
                    string path1 = path.Replace('\\', '/');
                    //当前预制体
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                    if (go != null)
                    {
                        if (go.transform.Find("UI") is Transform ui)
                        {
                            ui.position = UI_pos;
                            ui.rotation = Quaternion.Euler(UI_rot) ;
                            ui.localScale = UI_scale;
                         
                        }
                       
                        //PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);
                        PrefabUtility.SavePrefabAsset(go);
                       // GameObject.DestroyImmediate(go);
                    }
                }
                //保存修改
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Back 信息");
            Back_pos = EditorGUILayout.Vector3Field("Back_pos:", Back_pos);
            Back_rot = EditorGUILayout.Vector3Field("Back_rot:", Back_rot);
            Back_scale = EditorGUILayout.Vector3Field("Back_scale:", Back_scale);
            if (GUILayout.Button("修改预制体_Back", GUILayout.Width(260)))
            {
                List<string> paths = EquipsEditor.GetPrefabsAndScenes(selectPath);
                foreach (string path in paths)
                {
                    string path1 = path.Replace('\\', '/');
                    //当前预制体
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                    if (go != null)
                    {
                        if (go.transform.Find("Back") is Transform back)
                        {
                            back.position = Back_pos;
                            back.rotation = Quaternion.Euler(Back_rot);
                            back.localScale = Back_scale;
                        }
                        // PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);
                        // GameObject.DestroyImmediate(go);
                        PrefabUtility.SavePrefabAsset(go);
                    }
                }

                //保存修改
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.Space(20);

            if (GUILayout.Button("设置预制体", GUILayout.Width(260)))
            {
                EditorWeaponPrefab();
            }


        }

        private void SetWorld()
        {
            //List<string> paths = EquipsEditor.GetPrefabsAndScenes(selectPath);
            List<string> paths = EquipsEditor.GetPrefabsAndScenes(GetSelectionDir());
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                Log.DebugGreen($"{go.name}  ->{path1}");
                if (go != null)
                {
                    if (go.transform.Find("World") is Transform world)
                    {
                        world.position = World_pos;
                        world.rotation = Quaternion.Euler(World_rot.x, World_rot.y, World_rot.z);
                        world.position = World_scale;
                    }
                    //PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);
                    PrefabUtility.SavePrefabAsset(go);

                    //GameObject.DestroyImmediate(go);
                }
            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void EditorWeaponPrefab()
        {
            List<string> paths = EquipsEditor.GetPrefabsAndScenes(selectPath);
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                Log.DebugBrown($"go.name:{go.name}");
                if (go != null)
                {
                    go = PrefabUtility.InstantiatePrefab(go) as GameObject;
                    Transform ui = null;//UI
                    Transform Back = null;//Back
                    ReferenceCollector collector = null;
                    ReferenceCollector UIcollector = null;
                    ReferenceCollector Backcollector = null;
                    if(go.activeSelf==false) go.SetActive(true);
                    Log.DebugBrown($"go.name:{go.name}  {go.transform.Find("go.name")==null}  {go.transform.GetChild(0).name}");
                  //  if (go.transform.Find("go.name") is Transform world)
                    if (go.transform.GetChild(0) is Transform world)
                    {
                        world.name = "World";
                        world.gameObject.SetLayer(LayerNames.ROLE);
                        collector = world.gameObject.GetEquipReferenceCollector();
                        collector.Clear();
                        //设置UI 
                        ui = go.transform.Find("UI");
                        if (ui == null)
                        {
                            ui = AddUI(go.transform, world);
                            Log.DebugGreen($"ui==null:{ui==null}");
                            UIcollector = ui.gameObject.GetEquipReferenceCollector();
                            Log.DebugGreen($"UIcollector==null:{UIcollector == null}");
                        }
                        Back = go.transform.Find("Back");
                        if (Back == null)
                        {
                            Back = GameObject.Instantiate<Transform>(world, go.transform);
                            Back.name = "Back";
                            Backcollector = Back.gameObject.GetEquipReferenceCollector();
                            Backcollector.Clear();

                        }
                    }
                   
                    if (go.transform.Find($"Stage_1") is Transform stage_1)
                    {
                        stage_1.gameObject.SetLayer(LayerNames.ROLE);
                        AddUIChild(ui, stage_1, UIcollector);
                        AddBackChild(Back, stage_1, Backcollector);
                        collector.Add(stage_1.name, stage_1.gameObject);
                    }
                    if (go.transform.Find($"Stage_2") is Transform stage_2)
                    {
                        stage_2.gameObject.SetLayer(LayerNames.DEFAULT);
                        AddUIChild(ui, stage_2,UIcollector);
                        AddBackChild(Back, stage_2, Backcollector);

                        collector.Add(stage_2.name, stage_2.gameObject);
                    }

                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);
                    GameObject.DestroyImmediate(go);
                }
            }
            EquipsEditor.RenameEquips("Weapon");
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //标记资源
            SetAbBundleTagEditor_RightButton.SetRootBundleOnly(AssetDatabase.GetAssetPath(Selection.objects[0]));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        }
        public Transform AddUI(Transform parent, Transform child)
        {

            Transform transform = GameObject.Instantiate<Transform>(child, parent);
            transform.name = "UI";
            transform.gameObject.SetLayer(LayerNames.UI);
            return transform;

        }
        public void AddUIChild(Transform parent, Transform child, ReferenceCollector reference)
        {

            Transform tr = GameObject.Instantiate<Transform>(child, parent);
            tr.name = tr.name.Replace("(Clone)", string.Empty);
            tr.gameObject.SetLayer(LayerNames.UI);
            tr.transform.localScale = Vector3.one;
            tr.transform.localRotation = Quaternion.identity;
            tr.transform.localPosition = Vector3.zero;
            Log.DebugGreen($"reference==null:{reference==null}  tr:{tr==null}");
            reference.Add(tr.name, tr.gameObject);
        }
        public void AddBackChild(Transform parent, Transform child, ReferenceCollector reference)
        {

            Transform tr = GameObject.Instantiate<Transform>(child, parent);
            tr.name = tr.name.Replace("(Clone)", string.Empty);
            tr.transform.localScale = Vector3.one;
            tr.transform.localRotation = Quaternion.identity;
            tr.transform.localPosition = Vector3.zero;
            reference.Add(tr.name, tr.gameObject);
        }

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
                selectPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                return AssetDatabase.GetAssetPath(Selection.objects[0]);
            }
        }
    }
}