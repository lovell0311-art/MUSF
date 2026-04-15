using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

namespace ETEditor
{

 
    /// <summary>
    /// 设置宏定义窗口
    /// </summary>
    public class SettingDefinitionWindows : EditorWindow
    {
        private List<MacroItem> macroItemList = new List<MacroItem>();
        private Dictionary<string, bool> dic = new Dictionary<string, bool>();

        private string Macro = null;
        BuildTargetGroup savaTarget= BuildTargetGroup.Standalone;
        private void OnEnable()
        {

#if UNITY_ANDROID
			 savaTarget = BuildTargetGroup.Android;
#elif UNITY_IOS
            savaTarget = BuildTargetGroup.iOS;
#elif UNITY_STANDALONE_WIN
			savaTarget= BuildTargetGroup.Standalone;
#else
			savaTarget= BuildTargetGroup.Standalone;
#endif
            Macro = PlayerSettings.GetScriptingDefineSymbolsForGroup(savaTarget);//获取宏信息 参数 获取那个平台下的
            macroItemList.Clear();//每次打开一次 窗口 都需要将数据重置
          //  macroItemList.Add(new MacroItem() { Name = "NET452", DisplayName = "NET452", isDebug=true,isRelease=true, isILRuntime = false });
            macroItemList.Add(new MacroItem() { Name = "ILRuntime", DisplayName = "ILRuntime", isDebug=false,isRelease=true,isILRuntime=true});
           // macroItemList.Add(new MacroItem() { Name = "DEBUG_MODEL", DisplayName = "打印Model日志" ,isDebug=true,isRelease=false, isILRuntime = false  });
            //macroItemList.Add(new MacroItem() { Name = "DEBUG_HOTFIX", DisplayName = "打印HotFix日志" ,isDebug=true,isRelease=false, isILRuntime = false });
          //  macroItemList.Add(new MacroItem() { Name = "ASYNC", DisplayName = "ISASYNC", isDebug=false,isRelease=false});

            for (int i = 0; i < macroItemList.Count; i++)
            {
                if (!string.IsNullOrEmpty(Macro) && Macro.IndexOf(macroItemList[i].Name) != -1)
                {
                    dic[macroItemList[i].Name] = true;
                }
                else
                {
                    dic[macroItemList[i].Name] = false;
                }
            }
        }
        /// <summary>
        /// 绘制窗口条目
        /// </summary>
        private void OnGUI()
        {
            for (int i = 0; i < macroItemList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("box");//开启一个水平行   备注:必须成对出现
                dic[macroItemList[i].Name] = GUILayout.Toggle(dic[macroItemList[i].Name], macroItemList[i].DisplayName);
                EditorGUILayout.EndHorizontal();//结束这个水平行
            }
            EditorGUILayout.BeginHorizontal();//开启一个水平行   备注:必须成对出现
            if (GUILayout.Button("保存", GUILayout.Width(100)))
            {
                SaveMacro();
            }
            /*if (GUILayout.Button("调试模式", GUILayout.Width(100)))
            {
                for (int i = 0; i < macroItemList.Count; i++)
                {
                    dic[macroItemList[i].Name] = macroItemList[i].isDebug;
                }
                SaveMacro();
            }
            if (GUILayout.Button("发布模式", GUILayout.Width(100)))
            {
                for (int i = 0; i < macroItemList.Count; i++)
                {
                    dic[macroItemList[i].Name] = macroItemList[i].isRelease;
                }
                SaveMacro();
            } */
            //if (GUILayout.Button("IsIsILRuntime", GUILayout.Width(100)))
            //{
            //    for (int i = 0; i < macroItemList.Count; i++)
            //    {
            //        dic[macroItemList[i].Name] = macroItemList[i].isILRuntime;
            //    }
            //    SaveMacro();
            //}
            EditorGUILayout.EndHorizontal();//结束这个水平行

        }
        /// <summary>
        /// 保存宏信息
        /// </summary>
        public void SaveMacro()
        {
            Macro = string.Empty;
            foreach (var item in dic)
            {
                if (item.Value)
                {
                    Macro += string.Format("{0};", item.Key);
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(savaTarget, Macro);//将信息保存到宏信息里. 参数1:保存到哪个平台  参数2:要保存的内容.
        }


        static BuildTargetGroup savaMonoTarget = BuildTargetGroup.Standalone;
        private static string MacroMono = null;
        private static List<MacroItem> macroMonoItemList = new List<MacroItem>();
        private static Dictionary<string, bool> Monodic = new Dictionary<string, bool>();
        public static void ChangeMacro(int typeindex)
        {
#if UNITY_ANDROID
            savaMonoTarget = BuildTargetGroup.Android;
#elif UNITY_IOS
            savaMonoTarget = BuildTargetGroup.iOS;
#elif UNITY_STANDALONE_WIN
			savaMonoTarget= BuildTargetGroup.Standalone;
#else
			savaMonoTarget= BuildTargetGroup.Standalone;
#endif

            
            MacroMono = PlayerSettings.GetScriptingDefineSymbolsForGroup(savaMonoTarget);//获取宏信息 参数 获取那个平台下的
            Debug.Log(MacroMono);
            return;
            /*for (int i = 0; i < macroMonoItemList.Count; i++)
            {
                if (!string.IsNullOrEmpty(MacroMono) && MacroMono.IndexOf(macroMonoItemList[i].Name) != -1)
                {
                    Monodic[macroMonoItemList[i].Name] = true;
                }
                else
                {
                    Monodic[macroMonoItemList[i].Name] = false;
                }
            }*/

            if (typeindex == 0)
            {
                //移除宏
                if (!string.IsNullOrEmpty(MacroMono) && MacroMono.IndexOf("ILRuntime") != -1)
                {
                    
                }

            }
            else if (typeindex == 1)
            {

                if (MacroMono.IndexOf("ILRuntime") != -1)
                {
                    //已经添加了 该宏
                    return;
                }
                //新加ILRuntime 宏
                MacroMono += string.Format("{0};", "ILRuntime");
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(savaMonoTarget, MacroMono);//将信息保存到宏信息里. 参数1:保存到哪个平台  参数2:要保存的内容.
        }

    }
    /// <summary>
    /// 宏元素.
    /// </summary>
    public class MacroItem
    {
        /// <summary>
        /// 宏名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 窗口上显示的名称
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// 是否调试
        /// </summary>
        public bool isDebug;

        /// <summary>
        /// 是否发布
        /// </summary>
        public bool isRelease;
        public bool isILRuntime;

    }

    /// <summary>
    /// 自定义菜单类.
    /// </summary>
    public class Menu
    {
        [MenuItem("Tools/SettingDefinition")]
        public static void Settings()
        {
            SettingDefinitionWindows sw = EditorWindow.GetWindow<SettingDefinitionWindows>();//获取指定类型的窗口.
            sw.titleContent = new GUIContent("设置宏窗口");
            sw.Show();
        }
    }

}