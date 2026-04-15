using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ETModel;
using static UnityEditor.Progress;

namespace ETEditor
{
    public static class AddMaterialCopyEditor
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

        [MenuItem("Assets/AddMaterialCopy")]
        static void EitorEquipPrefab()
        {

            List<string> paths = EquipsEditor.GetPrefabsAndScenes(GetSelectionDir());
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                if (go != null)
                {
                    go = PrefabUtility.InstantiatePrefab(go) as GameObject;

                    if (go.name.Contains("Guard") || go.name.Contains("Wing") || go.name.Contains("Flag")) continue;

                    if (go.name.Contains("Suit"))
                    { 
                    
                    }


                    if (go.transform.Find("World") is Transform world_)
                    {
                        if (world_.GetComponent<MaterialCopy>() == null)
                        {
                            if (go.name.Contains("Suit"))
                            {
                                MaterialCopy materialCopy= world_.gameObject.AddComponent<MaterialCopy>();
                               /* SkinnedMeshRenderer[] skinnedMeshRenderers=world_.GetComponentsInChildren<SkinnedMeshRenderer>();
                                materialCopy.skinnedMeshRenderers=new SkinnedMeshRenderer[skinnedMeshRenderers.Length];
                                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                                {
                                    skinnedMeshRenderers[i] = skinnedMeshRenderers[i];
                                }*/
                            }
                            else
                            {

                                MaterialCopy materialCopy = world_.gameObject.AddComponent<MaterialCopy>();
                                ReferenceCollector collector = world_.GetReferenceCollector();
                                var obj = collector.GetAssets(MonoReferenceType.Object);
                                materialCopy.meshs = new MeshRenderer[obj.Length];
                                for (int i = 0; i < obj.Length; i++)
                                {
                                    materialCopy.meshs[i] = ((GameObject)obj[i]).GetComponent<MeshRenderer>();
                                }
                            }
                           
                        }
                    }
                    if (go.transform.Find("UI") is Transform ui_)
                    {
                        if (ui_.GetComponent<MaterialCopy>() == null)
                        {
                            if (go.name.Contains("Suit"))
                            {
                                ui_.gameObject.AddComponent<MaterialCopy>();
                            }
                            else
                            {
                                MaterialCopy materialCopy = ui_.gameObject.AddComponent<MaterialCopy>();
                                ReferenceCollector collector = ui_.GetReferenceCollector();
                                var obj = collector.GetAssets(MonoReferenceType.Object);
                                materialCopy.meshs = new MeshRenderer[obj.Length];
                                for (int i = 0; i < obj.Length; i++)
                                {
                                    materialCopy.meshs[i] = ((GameObject)obj[i]).GetComponent<MeshRenderer>();
                                }
                            }

                        }
                    }
                    if (go.transform.Find("Back") is Transform back_)
                    {
                       
                        if (back_.GetComponent<MaterialCopy>() == null)
                        {
                            if (go.name.Contains("Suit"))
                            {
                                back_.gameObject.AddComponent<MaterialCopy>();
                            }
                            else
                            {
                                MaterialCopy materialCopy = back_.gameObject.AddComponent<MaterialCopy>();
                                ReferenceCollector collector = back_.GetReferenceCollector();
                                var obj = collector.GetAssets(MonoReferenceType.Object);
                                materialCopy.meshs = new MeshRenderer[obj.Length];
                                for (int i = 0; i < obj.Length; i++)
                                {
                                    materialCopy.meshs[i] = ((GameObject)obj[i]).GetComponent<MeshRenderer>();
                                }
                            }

                        }
                    }
                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path1, InteractionMode.AutomatedAction);
                    GameObject.DestroyImmediate(go);
                }
            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
      
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            Debug.Log("标记完成");
        }
    }
}
