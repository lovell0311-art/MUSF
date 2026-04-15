using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ETEditor
{

    /// <summary>
    /// 玩家装备 编辑器
    /// </summary>
    public static class EquipsEditor
    {
        /// <summary>
        /// 得到当前所选文件的路径
        /// </summary>
        /// <returns></returns>
        public static string GetSelectionDir()
        {
            if (Selection.objects.Length <= 0)
            {
               // EditorUtility.DisplayDialogComplex("警告!", $"请先选择一个文件夹！！", "切换", "取消", "不切换");
                Log.DebugRed("请先选择一个文件夹");
                return null;
            }
            else
            {

                return AssetDatabase.GetAssetPath(Selection.objects[0]);
            }
        }

        [MenuItem("Assets/新套装/修改套装预制体")]
        static void EitorEquipPrefab() 
        {
           
            List<string> paths = GetPrefabsAndScenes(GetSelectionDir());
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                if (go != null)
                {
                    go = PrefabUtility.InstantiatePrefab(go) as GameObject;


                    if (go.transform.Find(go.name) is Transform world)
                    {
                        world.name = "World";
                        world.gameObject.SetLayer(LayerNames.ROLE);
                        ReferenceCollector collector = world.gameObject.GetEquipReferenceCollector();
                        collector.Clear();
                        FindGuangQuna_New(go.name, go.transform, collector);
                        //设置UI 
                        if (go.transform.Find("UI") == null)
                        {
                            AddUI(go.transform, world);
                        }

                    }
                    else
                    {
                        for (int i = go.transform.childCount-1; i >=0; i--)
                        {
                            if (!go.transform.GetChild(i).name.Contains("Bip001"))
                            {
                                if (go.transform.GetChild(i) is Transform transform)
                                {
                                    transform.name = "World";
                                    transform.gameObject.SetLayer(LayerNames.ROLE);
                                    ReferenceCollector collector = transform.gameObject.GetEquipReferenceCollector();
                                    collector.Clear();
                                    FindGuangQuna_New(go.name, go.transform, collector);
                                    //设置UI 
                                    if (go.transform.Find("UI") == null)
                                    {
                                        AddUI(go.transform, transform);
                                    }
                                }
                            }
                        }
                       
                    }


                    go.transform.SetUIPos();
                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);
                    GameObject.DestroyImmediate(go);
                }
            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RenameEquips("Suit");
            //标记资源
            SetAbBundleTagEditor_RightButton.SetRootBundleOnly(AssetDatabase.GetAssetPath(Selection.objects[0]));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            Debug.Log("标记完成");
        }
        [MenuItem("Assets/设置翅膀预制体")]
        static void EitorWingPrefab()
        {

            List<string> paths = GetPrefabsAndScenes(GetSelectionDir());
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                if (go != null)
                {
                    go = PrefabUtility.InstantiatePrefab(go) as GameObject;


                    if (go.transform.Find(go.name) is Transform world)
                    {
                        world.name = "World";
                        world.gameObject.SetLayer(LayerNames.ROLE);
                        ReferenceCollector collector = world.gameObject.GetEquipReferenceCollector();
                        collector.Clear();
                        FindGuangQuna_New(go.name, go.transform, collector);
                        //设置UI 
                        if (go.transform.Find("UI") == null)
                        {
                            AddUI(go.transform, world);
                        }

                    }
                    else
                    {
                        for (int i = go.transform.childCount - 1; i >= 0; i--)
                        {
                            if (!go.transform.GetChild(i).name.Contains("Bip001"))
                            {
                                if (go.transform.GetChild(i) is Transform transform)
                                {
                                    transform.name = "World";
                                    transform.gameObject.SetLayer(LayerNames.ROLE);
                                    ReferenceCollector collector = transform.gameObject.GetEquipReferenceCollector();
                                    collector.Clear();
                                    FindGuangQuna_New(go.name, go.transform, collector);
                                    //设置UI 
                                    if (go.transform.Find("UI") == null)
                                    {
                                        AddUI(go.transform, transform);
                                    }
                                }
                            }
                        }

                    }


                    go.transform.SetUIPos();
                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);
                    GameObject.DestroyImmediate(go);
                }
            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RenameEquips("Suit");
            //标记资源
            SetAbBundleTagEditor_RightButton.SetRootBundleOnly(AssetDatabase.GetAssetPath(Selection.objects[0]));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            Debug.Log("标记完成");
        }

        [MenuItem("Assets/设置装备/套装")]
        static void SetSuit()
        {
            RenameEquips("Suit");
            EditorApplyPrefab();
        }
        [MenuItem("Assets/设置装备/弓箭手套装")]
        static void SetArcherSuit()
        {
            RenameEquips("Suit");
            EditorApplyPrefab("gongjianshou");
        }
        [MenuItem("Assets/设置装备/设置套装名字")]
        static void SetEquipName()
        {
            RenameEquips("Suit");
        }

        [MenuItem("Assets/修改/修改装备引用")]
        static void ChangeRefren()
        {
            Log.DebugGreen($"GetSelectionDir:{GetSelectionDir()}");
            List<string> paths = GetPrefabsAndScenes(GetSelectionDir());
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
                    ReferenceCollector collector = null;
                    ReferenceCollector UIcollector = null;
                    Log.DebugGreen($"名字：{go.name.Split('_')[1]} ");


                    if (go.transform.Find("World") is Transform world)
                    {
                        world.name = "World";
                        world.gameObject.SetLayer(LayerNames.ROLE);
                        collector = world.gameObject.GetEquipReferenceCollector();
                        collector.Clear();
                        FindGuangQuna(go.name, go.transform, collector);
                        //设置UI 
                        ui = go.transform.Find("UI");
                        if (ui == null)
                        {
                            ui = AddUI(go.transform, world);
                            UIcollector = ui.GetReferenceCollector();
                        }

                    }

                    #region 弃用
                    if (go.transform.Find($"Stage_1") is Transform stage_1)
                    {
                        stage_1.name = "Stage_1";
                        stage_1.gameObject.SetLayer(LayerNames.ROLE);
                        AddUIChild(ui, stage_1, "Stage_1", UIcollector);

                        //// stage_1.SetParent(parent);
                        collector.Add(stage_1.name, stage_1.gameObject);
                    }
                    if (go.transform.Find($"Stage_2") is Transform stage_2)
                    {
                        stage_2.name = "Stage_2";
                        stage_2.gameObject.SetLayer(LayerNames.ROLE);
                        AddUIChild(ui, stage_2, "Stage_2", UIcollector);

                        //// stage_2.SetParent(parent);
                        collector.Add(stage_2.name, stage_2.gameObject);
                    }

                    if (go.transform.Find($"Stage_3") is Transform stage_3)
                    {
                        stage_3.name = "Stage_3";
                        stage_3.gameObject.SetLayer(LayerNames.ROLE);
                        AddUIChild(ui, stage_3, "Stage_3", UIcollector);


                        collector.Add(stage_3.name, stage_3.gameObject);
                    }
                    #endregion

                    go.transform.SetUIPos();

                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);

                    GameObject.DestroyImmediate(go);
                }




            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// 重命名 
        /// </summary>
        /// <param name="name">前缀名</param>
       public  static void RenameEquips(string name)
        {
            List<string> paths = GetPrefabsAndScenes(GetSelectionDir());
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                if (!go.name.Contains($"{name}"))
                {
                    AssetDatabase.RenameAsset(path1, $"{name}_" + go.name.Replace("_10", string.Empty));//修改预制体的名字
                }
            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void EditorApplyPrefab(string suitType = null)
        {

            List<string> paths = GetPrefabsAndScenes(GetSelectionDir());
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
                    ReferenceCollector collector = null;
                    ReferenceCollector UIcollector = null;
                    Log.DebugGreen($"名字：{go.name.Split('_')[1]} ");

                    if (go.transform.Find(go.name.Split('_')[1]) is Transform world)
                    {
                        world.name = "World";
                        world.gameObject.SetLayer(LayerNames.ROLE);
                        collector = world.gameObject.GetEquipReferenceCollector();
                        collector.Clear();
                        FindGuangQuna(go.name, go.transform, collector);
                        //设置UI 
                        ui = go.transform.Find("UI");
                        if (ui == null)
                        {
                            ui = AddUI(go.transform, world);
                            UIcollector = ui.GetReferenceCollector();
                        }

                    }
                    #region 弃用
                    if (go.transform.Find($"{go.name.Split('_')[1]}001") is Transform stage_1)
                    {
                        stage_1.name = "Stage_1";
                        stage_1.gameObject.SetLayer(LayerNames.ROLE);
                        AddUIChild(ui, stage_1, "Stage_1", UIcollector);

                        //// stage_1.SetParent(parent);
                        collector.Add(stage_1.name, stage_1.gameObject);
                    }
                    if (go.transform.Find($"{go.name.Split('_')[1]}001 (1)") is Transform stage_2)
                    {
                        stage_2.name = "Stage_2";
                        stage_2.gameObject.SetLayer(LayerNames.ROLE);
                        AddUIChild(ui, stage_2, "Stage_2", UIcollector);

                        //// stage_2.SetParent(parent);
                        collector.Add(stage_2.name, stage_2.gameObject);
                    }
                    if (go.transform.Find($"{go.name.Split('_')[1]}001 (2)") is Transform stage_3)
                    {
                        stage_3.name = "Stage_3";
                        stage_3.gameObject.SetLayer(LayerNames.ROLE);
                        AddUIChild(ui, stage_3, "Stage_3", UIcollector);


                        collector.Add(stage_3.name, stage_3.gameObject);
                    }

                    #endregion
                    if (!string.IsNullOrEmpty(suitType) && suitType.Contains("gongjianshou"))
                        go.transform.SetArcherUIPos();
                    else
                        go.transform.SetUIPos();
                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);

                    GameObject.DestroyImmediate(go);
                }




            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //标记资源
            SetAbBundleTagEditor_RightButton.SetRootBundleOnly(AssetDatabase.GetAssetPath(Selection.objects[0]));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            Debug.Log("标记完成");
        }
        public static Transform AddUI(Transform parent, Transform child)
        {

            Transform transform = GameObject.Instantiate<Transform>(child, parent);
            transform.name = "UI";
            SkinnedMeshRenderer skinnedMesh = transform.gameObject.GetComponent<SkinnedMeshRenderer>();
            transform.gameObject.AddComponent<MeshFilter>().mesh = skinnedMesh.sharedMesh;
            transform.gameObject.AddComponent<MeshRenderer>().materials = skinnedMesh.sharedMaterials;
            GameObject.DestroyImmediate(skinnedMesh);
            transform.gameObject.SetLayer(LayerNames.UI);
            return transform;

        }

        public static void AddUIChild(Transform parent, Transform child, string name, ReferenceCollector reference)
        {
            if (parent.Find(name) != null) return;
            Transform tr = GameObject.Instantiate<Transform>(child, parent);
            tr.name = name;
            SkinnedMeshRenderer skinnedMesh = tr.gameObject.GetComponent<SkinnedMeshRenderer>();
            tr.gameObject.AddComponent<MeshFilter>().mesh = skinnedMesh.sharedMesh;
            tr.gameObject.AddComponent<MeshRenderer>().materials = skinnedMesh.sharedMaterials;
            GameObject.DestroyImmediate(skinnedMesh);
            tr.gameObject.SetLayer(LayerNames.UI);
            tr.transform.localScale = Vector3.one;
            reference.Add(tr.name, tr.gameObject);
        }


        public static void SetUIPos(this Transform self)
        {
            Transform ui = self.Find("UI");
            if (ui == null) return;
            if (self.name.Contains("shou"))
            {
                ui.transform.localPosition = new Vector3(0, -1.5f, 0);
                ui.transform.localScale = new Vector3(.2f, .2f, .2f);
                // ui.transform.localRotation =ui.transform.rotation * Quaternion.Euler(0, 0, 0);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 0);
            }
            else if (self.name.Contains("tui"))
            {
                ui.transform.localPosition = new Vector3(0, -1.6f, 0);
                ui.transform.localScale = new Vector3(.3f, .3f, .3f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 180, 0);
            }
            else if (self.name.Contains("kai"))
            {
                ui.transform.localPosition = new Vector3(0, -1.5f, 0);
                ui.transform.localScale = new Vector3(.2f, .2f, .2f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 180, 0);
            }
            else if (self.name.Contains("kui"))
            {
                ui.transform.localPosition = new Vector3(0, -4.5f, 0);
                ui.transform.localScale = new Vector3(.4f, .4f, .4f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 180, 0);
            }
            else if (self.name.Contains("xue"))
            {
                ui.transform.localPosition = new Vector3(0, -0.5f, 0);
                ui.transform.localScale = new Vector3(.2f, .2f, .2f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 180, 0);
            }

        }
        public static void SetArcherUIPos(this Transform self)
        {
            Transform ui = self.Find("UI");
            if (ui == null) return;
            if (self.name.Contains("shou"))
            {
                ui.transform.localPosition = new Vector3(0, -1.5f, 0);
                ui.transform.localScale = new Vector3(.2f, .2f, .2f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 0);
            }
            else if (self.name.Contains("tui"))
            {
                ui.transform.localPosition = new Vector3(0, -1.6f, 0);
                ui.transform.localScale = new Vector3(.3f, .3f, .3f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 180, 0);
            }
            else if (self.name.Contains("kai"))
            {
                ui.transform.localPosition = new Vector3(0, -1.8f, 0);
                ui.transform.localScale = new Vector3(.2f, .2f, .2f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 180, 0);
            }
            else if (self.name.Contains("kui"))
            {
                ui.transform.localPosition = new Vector3(0, -4.5f, 0);
                ui.transform.localScale = new Vector3(.4f, .4f, .4f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 180, 0);
            }
            else if (self.name.Contains("xue"))
            {
                ui.transform.localPosition = new Vector3(0, -0.5f, 0);
                ui.transform.localScale = new Vector3(.2f, .2f, .2f);
                ui.transform.localRotation = ui.transform.eulerAngles != Vector3.zero ? ui.transform.localRotation * Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 180, 0);
            }

        }

        public static ReferenceCollector GetEquipReferenceCollector(this GameObject self)
        {
            ReferenceCollector collector;
            if (self.GetComponent<ReferenceCollector>() == null)
                collector = self.AddComponent<ReferenceCollector>();
            else
                collector = self.GetComponent<ReferenceCollector>();

            collector.Clear();

            return collector;
        }

        public static List<string> GetPrefabsAndScenes(string srcPath)
        {
            List<string> paths = new List<string>();
            FileHelper.GetAllFiles(paths, srcPath);

            List<string> files = new List<string>();
            foreach (string str in paths)
            {
                if (str.EndsWith(".prefab"))
                {
                    files.Add(str);
                }
            }
            return files;
        }

        /// <summary>
        /// 将特效放入ReferenceCollector
        /// </summary>
        /// <param name="parentName"></param>
        /// <param name="parent"></param>
        /// <param name="reference"></param>
        public static void FindGuangQuna(string parentName, Transform parent, ReferenceCollector reference)
        {
            if (parentName.Contains("hushou"))//护手
            {
                Transform parent_ = parent.Find("Bip001");
                Transform hushouguangquan_L = FinChildInTransform(parent_, "guangquan_L_hand");
                if (hushouguangquan_L != null)
                {
                    reference.Add("Angle_L", hushouguangquan_L.gameObject);
                }
                Transform hushouguangquan_R = FinChildInTransform(parent_, "guangquan_R_hand");
                if (hushouguangquan_R != null)
                {
                    reference.Add("Angle_R", hushouguangquan_R.gameObject);
                }
                Transform hand_L = FinChildInTransform(parent_, "glow_L_hand");
                if (hand_L != null)
                {
                    reference.Add("Hand_L", hand_L.gameObject);
                }
                Transform hand_R = FinChildInTransform(parent_, "glow_R_hand");
                if (hand_R != null)
                {
                    reference.Add("Hand_R", hand_R.gameObject);
                }
            }
            if (parentName.Contains("zhikai"))//铠
            {
                Transform parent_ = parent.Find("Bip001");
                Transform hushouguangquan_L = FinChildInTransform(parent_, "guangquan_L_UpperArm_kai");
                if (hushouguangquan_L != null)
                {
                    reference.Add("Angle_L", hushouguangquan_L.gameObject);
                }
                Transform hushouguangquan_R = FinChildInTransform(parent_, "guangquan_R_UpperArm_kai");
                if (hushouguangquan_R != null)
                {
                    reference.Add("Angle_R", hushouguangquan_R.gameObject);
                }

                Transform caidai = FinChildInTransform(parent_, "caidai_Pelvis_kai");
                if (caidai != null)
                {
                    reference.Add("CaiDai", caidai.gameObject);
                }
                Transform huo = FinChildInTransform(parent_, "huo1_Spine1_kai");
                if (huo != null)
                {
                    reference.Add("huo", huo.gameObject);
                }

            }
            if (parentName.Contains("zhikui"))//头盔
            {
                Transform parent_ = parent.Find("Bip001");
                Transform hushouguangquan_L = FinChildInTransform(parent_, "guangquan_Head");
                if (hushouguangquan_L != null)
                {
                    reference.Add("Angle_Head", hushouguangquan_L.gameObject);
                }
                Transform hushouguangquan_R = FinChildInTransform(parent_, "fasheguang_Head");
                if (hushouguangquan_R != null)
                {
                    reference.Add("Guang_Head", hushouguangquan_R.gameObject);
                }


            }
            if (parentName.Contains("zhixue"))//之靴
            {
                Transform parent_ = parent.Find("Bip001");
                Transform hushouguangquan_L = FinChildInTransform(parent_, "guangquan_L_foot");
                if (hushouguangquan_L != null)
                {
                    reference.Add("Angle_L", hushouguangquan_L.gameObject);
                }
                Transform hushouguangquan_R = FinChildInTransform(parent_, "guangquan_R_foot");
                if (hushouguangquan_R != null)
                {
                    reference.Add("Angle_R", hushouguangquan_R.gameObject);
                }


            }
        }

        public static void FindGuangQuna_New(string parentName, Transform parent, ReferenceCollector reference)
        {
            if (parentName.Contains("hushou"))//护手
            {
                Transform parent_ = parent.Find("Bip001");
                Transform hushouguangquan_L = FinChildInTransform(parent_, "texiao_L_shoubu 1");
                if (hushouguangquan_L != null)
                {
                    reference.Add("Angle_L", hushouguangquan_L.gameObject);
                }
                Transform hushouguangquan_R = FinChildInTransform(parent_, "texiao_R_zhoubu");
                if (hushouguangquan_R != null)
                {
                    reference.Add("Angle_R", hushouguangquan_R.gameObject);
                }

            }
            if (parentName.Contains("zhikai"))//铠
            {
                Transform parent_ = parent.Find("Bip001");
                Transform hushouguangquan_L = FinChildInTransform(parent_, "texiao_L_shoubu");
                if (hushouguangquan_L != null)
                {
                    reference.Add("Angle_L", hushouguangquan_L.gameObject);
                }
                Transform hushouguangquan_R = FinChildInTransform(parent_, "texiao_R_jianbu");
                if (hushouguangquan_R != null)
                {
                    reference.Add("Angle_R", hushouguangquan_R.gameObject);
                }

                Transform caidai = FinChildInTransform(parent_, "texiao_tongyong");
                if (caidai != null)
                {
                    reference.Add("CaiDai", caidai.gameObject);
                }
              
            }
            if (parentName.Contains("zhikui"))//头盔
            {
                Transform parent_ = parent.Find("Bip001");
                Transform hushouguangquan_L = FinChildInTransform(parent_, "texiao_toubu");
                if (hushouguangquan_L != null)
                {
                    reference.Add("Angle_Head", hushouguangquan_L.gameObject);
                }
                //Transform hushouguangquan_R = FinChildInTransform(parent_, "fasheguang_Head");
                //if (hushouguangquan_R != null)
                //{
                //    reference.Add("Guang_Head", hushouguangquan_R.gameObject);
                //}


            }
            if (parentName.Contains("zhixue"))//之靴
            {
                Transform parent_ = parent.Find("Bip001");
                Transform hushouguangquan_L = FinChildInTransform(parent_, "texiao_leg_L");
                if (hushouguangquan_L != null)
                {
                    reference.Add("Angle_L", hushouguangquan_L.gameObject);
                }
                Transform hushouguangquan_R = FinChildInTransform(parent_, "texiao_leg_R");
                if (hushouguangquan_R != null)
                {
                    reference.Add("Angle_R", hushouguangquan_R.gameObject);
                }


            }
        }
        /// <summary>
        /// 递归查找子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform FinChildInTransform(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null)
                return child;
            for (int i = 0; i < parent.childCount; i++)
            {
                child = FinChildInTransform(parent.GetChild(i), childName);
                if (child == null)
                    continue;
                else
                    return child;
            }
            return null;
        }

        [MenuItem("Assets/修改/关闭SkinnedMotion")]
        static void CloseSkinnedMotion()
        {
            Log.DebugGreen($"GetSelectionDir:{GetSelectionDir()}");
            List<string> paths = GetPrefabsAndScenes(GetSelectionDir());
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                if (go != null)
                {
                    go = PrefabUtility.InstantiatePrefab(go) as GameObject;
                    SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                    for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                    {
                        skinnedMeshRenderers[i].skinnedMotionVectors = false;
                    }

                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);

                    GameObject.DestroyImmediate(go);
                }




            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}