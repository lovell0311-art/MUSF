/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using System.IO;
#endif


public class IosAutoBuild
{
    [PostProcessBuild(9999)]

    public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
    {
        if (target != BuildTarget.iOS)
            return;
#if UNITY_IOS
        string projPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);
        string targetGuid = proj.TargetGuidByName("Unity-iPhone");
        //添加Other Linker Flags
        proj.AddBuildProperty(targetGuid, "Other Linker Flags", "-Objc -all_load -framework CoreTelephony");
        //修改部分设置
        proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");


        //添加代码库
        proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libiconv.tbd", "Frameworks/libiconv.tbd", PBXSourceTree.Sdk));
        proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libsqlite3.tbd", "Frameworks/libsqlite3.tbd", PBXSourceTree.Sdk));
        proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));
        proj.AddFileToBuild(targetGuid, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
        //添加Frameworks
        proj.AddFrameworkToProject(targetGuid, "Security.framework", false);
        proj.AddFrameworkToProject(targetGuid, "SystemConfiguration.framework", false);
        proj.AddFrameworkToProject(targetGuid, "CoreGraphics.Framework", false);
        proj.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
        proj.AddFrameworkToProject(targetGuid, "CFNetwork.framework", false);
        proj.AddFrameworkToProject(targetGuid, "WebKit.framework", false);


        //修改Info.plist
        string plistPath = Path.Combine(pathToBuildProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        PlistElementDict rootDict = plist.root;
        PlistElementArray urlTypes = rootDict.CreateArray("CFBundleURLTypes");
        var urlItem2 = urlTypes.AddDict();
        urlItem2.SetString("CFBundleTypeRole", "Editor");
        urlItem2.SetString("CFBundleURLName", "com.qijimu.qijimu");
        //1
        // urlItem2.SetString("CFBundleURLSchemes", "wx47074c4bb44164fa");
        //2,具体那个有效果
        var schemes2 = urlItem2.CreateArray("CFBundleURLSchemes");
        schemes2.AddString("wx47074c4bb44164fa");

        //添加白名单
        PlistElementArray queriesSchemes = rootDict.CreateArray("LSApplicationQueriesSchemes");
        queriesSchemes.AddString("weixin");
        queriesSchemes.AddString("wechat");
        queriesSchemes.AddString("weixinULAPI");

        plist.WriteToFile(plistPath);
#endif
    }

}
 */   
