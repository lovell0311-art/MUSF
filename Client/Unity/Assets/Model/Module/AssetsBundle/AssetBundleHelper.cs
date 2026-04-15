using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace ETModel
{

	[ObjectSystem]
	public class ABInfoAwakeSystem : AwakeSystem<ABInfo, string, AssetBundle>
	{
		public override void Awake(ABInfo self, string abName, AssetBundle a)
		{
			self.AssetBundle = a;
			self.Name = abName;
			self.RefCount = 1;
		}
	}
	/// <summary>
	/// 用于字符串转换，减少GC
	/// </summary>
	public static class AssetBundleHelper 
    {
		public static readonly Dictionary<int, string> IntToStringDict = new Dictionary<int, string>();

		public static readonly Dictionary<string, string> StringToABDict = new Dictionary<string, string>();

		public static readonly Dictionary<string, string> BundleNameToLowerDict = new Dictionary<string, string>()
		{
			{ "StreamingAssets", "StreamingAssets" }
		};

		// 缓存包依赖，不用每次计算
		public static Dictionary<string, string[]> DependenciesCache = new Dictionary<string, string[]>();

		public static string IntToString(this int value)
		{
			string result;
			if (IntToStringDict.TryGetValue(value, out result))
			{
				return result;
			}

			result = value.ToString();
			IntToStringDict[value] = result;
			return result;
		}

		public static string StringToAB(this string value)
		{
             string result;
            if (StringToABDict.TryGetValue(value, out  result))
			{
				return result;
			}
           
            result = value + ".unity3d";
			StringToABDict[value] = result;
         
			return result;
		}

		public static string IntToAB(this int value)
		{
			return value.IntToString().StringToAB();
		}

		public static string BundleNameToLower(this string value)
		{
			string result;
			if (BundleNameToLowerDict.TryGetValue(value, out result))
			{
				return result;
			}

			result = value.ToLower();
			BundleNameToLowerDict[value] = result;
			return result;
		}

		public static string[] GetDependencies(string assetBundleName)
		{
			string[] dependencies = new string[0];
			if (DependenciesCache.TryGetValue(assetBundleName, out dependencies))
			{
				return dependencies;
			}
			if (!Define.IsAsync)
			{
#if UNITY_EDITOR
			
				dependencies = AssetDatabase.GetAssetBundleDependencies(assetBundleName, true);
#endif
			}
			else
			{
              //  Log.DebugGreen($"assetBundleName:{assetBundleName}");
                //获取到要加载的资源的引用 先加载所有引用 然后再去加载资源 不然就会导致到加载出来的资源丢失图片 丢失音效...
                dependencies = AssetBundleComponent.AssetBundleManifestObject.GetAllDependencies(assetBundleName);
			}
			DependenciesCache.Add(assetBundleName, dependencies);
			return dependencies;
		}

		public static string[] GetSortedDependencies(string assetBundleName)
		{
			Dictionary<string, int> info = new Dictionary<string, int>();
			List<string> parents = new List<string>();

			CollectDependencies(parents, assetBundleName, info);
			string[] ss = info.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
			return ss;
		}

		public static void CollectDependencies(List<string> parents, string assetBundleName, Dictionary<string, int> info)
		{
			parents.Add(assetBundleName);
			string[] deps = GetDependencies(assetBundleName);
			foreach (string parent in parents)
			{
				if (!info.ContainsKey(parent))
				{
					info[parent] = 0;
				}
				info[parent] += deps.Length;
			}

			//对依赖的文件进行再次分析
			foreach (string dep in deps)
			{
				if (parents.Contains(dep))
				{
					throw new Exception($"包有循环依赖，请重新标记: {assetBundleName} {dep}");
				}
				//再去分析每个依赖的包 是否还有依赖
				CollectDependencies(parents, dep, info);
			}
			parents.RemoveAt(parents.Count - 1);
		}
	}
}