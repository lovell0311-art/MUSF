using System.Collections.Generic;
using System.IO;
using System.Linq;
using ETModel;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Object = UnityEngine.Object;

namespace ETEditor
{
	public class BundleInfo
	{
		public List<string> ParentPaths = new List<string>();
	}

	public enum PlatformType
	{
		None,
		Android,
		IOS,
		PC,
		MacOS,
	}
	
	public enum BuildType
	{
		Development,
		Release,
	}

	public class BuildEditor : EditorWindow
	{
		private readonly Dictionary<string, BundleInfo> dictionary = new Dictionary<string, BundleInfo>();
		private PlatformType activePlatform;
		private PlatformType platformType;
		private bool isBuildExe;
		private bool isContainAB;

        public  BuildResPath BuildResPath = BuildResPath.LingWu;

        private BuildType buildType= BuildType.Release;
		private BuildOptions buildOptions = BuildOptions.AllowDebugging | BuildOptions.Development;
		private BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;

		[MenuItem("Tools/打包工具")]
		public static void ShowWindow()
		{
			GetWindow(typeof(BuildEditor));

            UpLoadFilesEditor.InitPath();
        }
		private void OnEnable()
		{
#if UNITY_ANDROID
			activePlatform = PlatformType.Android;
#elif UNITY_IOS
			activePlatform = PlatformType.IOS;
#elif UNITY_STANDALONE_WIN
			activePlatform = PlatformType.PC;
#elif UNITY_STANDALONE_OSX
			activePlatform = PlatformType.MacOS;
#else
			activePlatform = PlatformType.None;
#endif
			platformType = activePlatform;
		}
		private void OnGUI() 
		{
			this.platformType = (PlatformType)EditorGUILayout.EnumPopup(platformType);
			this.isBuildExe = EditorGUILayout.Toggle("是否打包EXE: ", this.isBuildExe);
			this.isContainAB = EditorGUILayout.Toggle("是否同将资源打进EXE: ", this.isContainAB);

            this.BuildResPath = (BuildResPath)EditorGUILayout.EnumPopup("BuildResPath: ", this.BuildResPath);

            EditorGUILayout.TextField("资源路径:", BuildHelper.GetBuildRes(BuildResPath));

            this.buildType = (BuildType)EditorGUILayout.EnumPopup("BuildType: ", this.buildType);
			
			switch (buildType)
			{
				case BuildType.Development:
					this.buildOptions = BuildOptions.Development | BuildOptions.AutoRunPlayer | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
					break;
				case BuildType.Release:
					this.buildOptions = BuildOptions.None;
					break;
			}

			EditorGUILayout.LabelField("BuildAssetBundleOptions(可多选):");
			this.buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField(this.buildAssetBundleOptions);
			GUILayout.Space(5);
			if (GUILayout.Button("开始打包"))
			{
				if (this.platformType == PlatformType.None)
				{
					ShowNotification(new GUIContent("请选择打包平台!"));
					return;
				}
				if (platformType != activePlatform)
				{
					switch (EditorUtility.DisplayDialogComplex("警告!", $"当前目标平台为{activePlatform}, 如果切换到{platformType}, 可能需要较长加载时间", "切换", "取消", "不切换"))
					{
						case 0:
							activePlatform = platformType;
							break;
						case 1:
							return;
						case 2:
							platformType = activePlatform;
							break;
					}
				}
                UpLoadFilesEditor.InitPath();
                BuildHelper.Build(this.platformType, this.buildAssetBundleOptions, this.buildOptions, this.isBuildExe, this.isContainAB);
			}
			GUILayout.Space(30);

			
            if (GUILayout.Button("筛选变动资源"))
            {

                UpLoadFilesEditor.CreatDuiBiFiles();

            }

            GUILayout.Space(30);

			if (GUILayout.Button("上传资源"))
			{
				
               GetWindow(typeof(OSSFolderUploaderEditor),true,"上传资源");
               
            }
            GUILayout.Space(30);

            if (GUILayout.Button("一键更新资源"))
            {

                if (this.platformType == PlatformType.None)
                {
                    ShowNotification(new GUIContent("请选择打包平台!"));
                    return;
                }
                if (platformType != activePlatform)
                {
                    switch (EditorUtility.DisplayDialogComplex("警告!", $"当前目标平台为{activePlatform}, 如果切换到{platformType}, 可能需要较长加载时间", "切换", "取消", "不切换"))
                    {
                        case 0:
                            activePlatform = platformType;
                            break;
                        case 1:
                            return;
                        case 2:
                            platformType = activePlatform;
                            break;
                    }
                }
                UpLoadFilesEditor.InitPath();
                BuildHelper.Build(this.platformType, this.buildAssetBundleOptions, this.buildOptions, this.isBuildExe, this.isContainAB);

               
                UpLoadFilesEditor.CreatDuiBiFiles();
                Debug.Log("资源筛选完成");
                GetWindow(typeof(OSSFolderUploaderEditor), true, $"上传资源->{Application.productName}{platformType}");
              
                Debug.Log("更新完成");

            }
        }
      
    }
}
