using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ETModel;

namespace ETEditor
{

    /// <summary>
    /// 批量修改预制体名字
    /// </summary>
    public class RenameEditor : EditorWindow
    {  
        public string preFixName;
        bool isAddResourcesRecycle;
        bool isReNameSingle;
        private void OnGUI()
        {
            preFixName = EditorGUILayout.TextField("前缀名字", preFixName);
            isAddResourcesRecycle = EditorGUILayout.Toggle("是否添加资源回收组件",isAddResourcesRecycle);
            isReNameSingle = EditorGUILayout.Toggle("修改单个资源名字", isReNameSingle);
            if (GUILayout.Button("修改"))
            {
                if (string.IsNullOrEmpty(preFixName))
                {
                    GUILayout.Label("请输入一个名字");
                    return;
                }
                if (isReNameSingle)
                {
                    ReNameSingle();
                }
                else
                {
                    ReName();
                }
            }
        }
        public void ReNameSingle() 
        {
            string path1 = EquipsEditor.GetSelectionDir();
            //当前预制体
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
            if (!go.name.Contains($"{preFixName}"))
            {
                AssetDatabase.RenameAsset(path1, $"{preFixName}_" + go.name);//修改预制体的名字
            }
            if (isAddResourcesRecycle)
            {
                if (go.GetComponent<ResourcesRecycle>() == null)//添加资源回收组件
                {
                    go.AddComponent<ResourcesRecycle>();
                }
            }
        }
        public void ReName()
        {
            List<string> paths = EquipsEditor.GetPrefabsAndScenes(EquipsEditor.GetSelectionDir());
            foreach (string path in paths)
            {
                string path1 = path.Replace('\\', '/');
                //当前预制体
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path1);
                if (!go.name.Contains($"{preFixName}"))
                {
                    AssetDatabase.RenameAsset(path1, $"{preFixName}_" + go.name);//修改预制体的名字
                }
                if (isAddResourcesRecycle)
                {
                    if (go.GetComponent<ResourcesRecycle>() == null)//添加资源回收组件
                    {
                        go.AddComponent<ResourcesRecycle>();
                    }
                }
            }
            //保存修改
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/改名")]
        static void ShowReName()
        {
            GetWindow(typeof(RenameEditor));
        }
    }
}
