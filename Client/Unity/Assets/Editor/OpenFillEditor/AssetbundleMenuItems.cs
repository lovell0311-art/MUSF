using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using ETModel;

/// <summary>
/// Assetbundle相关菜单项
/// </summary>

namespace ETEditor
{
    // unity editor启动和运行时调用静态构造函数
    [InitializeOnLoad]
    public class AssetBundleMenuItems
    {
        //%:ctrl,#:shift,&:alt
      
        const string kToolsCopyAssetbundles = "FolderTools/Copy To StreamingAssets";
        const string kToolsOpenAssets = "FolderTools/Open Assets";
        const string kToolsOpenExcel = "FolderTools/Open Excel";
        const string kToolsOpenOutput = "FolderTools/Open Current Output";
        const string kToolsOpenPerisitentData = "FolderTools/Open PersistentData";
        const string kToolsClearOutput = "FolderTools/Clear Current Output";
        const string kToolsClearStreamingAssets = "FolderTools/Clear StreamingAssets";
        const string kToolsClearPersistentAssets = "FolderTools/Clear PersistentData";


        [MenuItem(kToolsOpenExcel, false, 1100)]
        static public void ToolsOpenExcel()//打开excel文件夹
        {
            EditorUtils.ExplorerFolder("../Excel");
        }
        [MenuItem(kToolsOpenAssets, false, 1200)]
        static public void ToolsOpenAssets()
        {
            EditorUtils.ExplorerFolder("../");
        }
        [MenuItem(kToolsOpenOutput, false, 1201)]
        static public void ToolsOpenOutput()
        {
            string outputPath = PackageUtils.GetCurBuildSettingAssetBundleOutputPath();
            EditorUtils.ExplorerFolder(outputPath);
        }

        [MenuItem(kToolsOpenPerisitentData, false, 1202)]
        static public void ToolsOpenPerisitentData()
        {
            EditorUtils.ExplorerFolder(Application.persistentDataPath);
        }

        [MenuItem(kToolsClearOutput, false, 1302)]
        static public void ToolsClearOutput()
        {
            var buildTargetName = PackageUtils.GetCurPlatformName();
            bool checkClear = EditorUtility.DisplayDialog("ClearOutput Warning",
                string.Format("Clear output assetbundles will force to rebuild all : \n\nplatform : {0} \n\n continue ?", buildTargetName),
                "Yes", "No");
            if (!checkClear)
            {
                return;
            }
            string outputPath = PackageUtils.GetCurBuildSettingAssetBundleOutputPath();
            GameUtility.SafeDeleteDir(outputPath);
            Debug.Log(string.Format("Clear done : {0}", outputPath));
        }

        [MenuItem(kToolsClearStreamingAssets, false, 1303)]
        static public void ToolsClearStreamingAssets()
        {
            bool checkClear = EditorUtility.DisplayDialog("ClearStreamingAssets Warning",
                "Clear streaming assets assetbundles will lost the latest player build info, continue ?",
                "Yes", "No");
            if (!checkClear)
            {
                return;
            }
            string outputPath = Application.streamingAssetsPath;
            GameUtility.SafeClearDir(outputPath);
            AssetDatabase.Refresh();
            Debug.Log(string.Format("Clear {0} assetbundle streaming assets done!", PackageUtils.GetCurPlatformName()));
        }

        [MenuItem(kToolsClearPersistentAssets, false, 1301)]
        static public void ToolsClearPersistentAssets()
        {
            bool checkClear = EditorUtility.DisplayDialog("ClearPersistentAssets Warning",
                "Clear persistent assetbundles will force to update all assetbundles that difference with streaming assets assetbundles, continue ?",
                "Yes", "No");
            if (!checkClear)
            {
                return;
            }

            string outputPath = Application.persistentDataPath;
            GameUtility.SafeDeleteDir(outputPath);
            Debug.Log(string.Format("Clear {0} assetbundle persistent assets done!", PackageUtils.GetCurPlatformName()));
        }


        [MenuItem("FolderTools/Clear All PlayerPrefs", false, 1300)]
        public static void ClearAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}