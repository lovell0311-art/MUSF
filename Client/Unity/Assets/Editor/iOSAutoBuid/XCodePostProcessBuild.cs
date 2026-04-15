using UnityEditor;
using System.IO;
using UnityEngine;





#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif

public static class XCodePostProcessBuild
{
/*#if UNITY_IOS
    private static readonly string[] csAddFrameworks = new string[]
    {
        "Security.framework","WebKit.framework", "CoreGraphics.framework", "CoreTelephony.framework", "libz.tbd" ,"libsqlite3.0.tbd", "libc++.tbd"
        , "SystemConfiguration.framework", "QuartzCore.framework", "CoreText.framework", "UIKit.framework", "Foundation.framework", "CFNetwork.framework"
        , "CoreMotion.framework", "AlipaySDK.framework","Security.framework",
    };

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (BuildTarget.iOS != buildTarget)
        {
            return;
        }
        string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        SetFrameworksAndBuildSettings(projectPath);
        SetInfoList(pathToBuiltProject, "com.hjsk.jzsj", "wx47074c4bb44164fa");

    }
   
    private static void SetFrameworksAndBuildSettings(string path)
    {
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(path));
        string target = proj.GetUnityMainTargetGuid();
        //string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
        
        Debug.Log("Target Name is " + target);
        // 设置 BuildSettings
        proj.AddBuildProperty(target, "Other Linker Flags", "-Objc");
        proj.AddBuildProperty(target, "Other Linker Flags", "-all_load");
        proj.AddBuildProperty(target, "Other Linker Flags", "-framework");
        proj.AddBuildProperty(target, "Other Linker Flags", "CoreTelephony");
        proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

        //根据微信SDK文档的要求，加入相关的Frameworks
        for (int i = 0; i < csAddFrameworks.Length; ++i)
        {
            if (!proj.ContainsFramework(target, csAddFrameworks[i]))
                proj.AddFrameworkToProject(target, csAddFrameworks[i], false);
        }

        File.WriteAllText(path, proj.WriteToString());
    }

    public static void SetInfoList(string buildPath, string wxUrlName, string wxScheme)
    {
        string listPath = buildPath + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(listPath));



        // 在“info”标签栏的“URL type“添加“URL scheme”,值为你在微信后台注册的应用程序的 AppID
        PlistElementArray urlArray = plist.root.CreateArray("CFBundleURLTypes");
        PlistElementDict dict = urlArray.AddDict();
        dict.SetString("CFBundleTypeRole", "Editor");
        dict.SetString("CFBundleURLName", wxUrlName);
        PlistElementArray urlSchemes = dict.CreateArray("CFBundleURLSchemes");
        urlSchemes.AddString(wxScheme);

        // 在“info”标签栏的“URL type“添加“URL scheme”,原本只有微信，支付宝这里新增的
        PlistElementArray urlArray2 = plist.root.values["CFBundleURLTypes"].AsArray();
        PlistElementDict dict2 = urlArray2.AddDict();
        dict2.SetString("CFBundleTypeRole", "Editor");
        dict2.SetString("CFBundleURLName", wxUrlName);
        PlistElementArray urlSchemes2 = dict2.CreateArray("CFBundleURLSchemes");
        urlSchemes2.AddString("2021003108650952");


        // 在 “info”标签栏的“LSApplicationQueriesSchemes“添加weixin wechat和weixinULAPI
        PlistElementArray wxArray = plist.root.CreateArray("LSApplicationQueriesSchemes");
        wxArray.AddString("weixin");
        wxArray.AddString("wechat");
        wxArray.AddString("weixinULAPI");
        wxArray.AddString("alipay");

        File.WriteAllText(listPath, plist.WriteToString());
    }

   

#endif*/
}