using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ETModel
{

	public class ABInfo : Component
	{
		public string Name { get; set; }
		//引用计数
		public int RefCount { get; set; }
		//AB资源引用
		public AssetBundle AssetBundle;

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			//Log.Info("移除资源包 " + Name);
			if (this.AssetBundle != null)
			{
				this.AssetBundle.Unload(true);
			}

			this.RefCount = 0;
			this.Name = "";
		}
	}
	[ObjectSystem]
	public class AssetBundleComponentAwake : AwakeSystem<AssetBundleComponent>
	{
		public override void Awake(AssetBundleComponent self)
		{
			self.Awake();
		}
	}



	/// <summary>
	/// AssetBundle资源管理类组件
	/// </summary>
	public class AssetBundleComponent : Component
	{
		public static AssetBundleComponent Instance { get; private set; }
		public static AssetBundleManifest AssetBundleManifestObject { get; set; }

		//缓存AB中所有的资源文件
		private readonly Dictionary<string, Dictionary<string, UnityEngine.Object>> resourceCache = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

		//已经加载的AB缓存
		private readonly Dictionary<string, ABInfo> bundles = new Dictionary<string, ABInfo>();

		private readonly Dictionary<string, List<ETTaskCompletionSource<int>>> LoadAllTasks = new Dictionary<string, List<ETTaskCompletionSource<int>>>();
		public void Awake()
		{
			Instance = this;
		}
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			foreach (var abInfo in this.bundles)
			{
				abInfo.Value.Dispose();
			}

			this.bundles.Clear();
			this.resourceCache.Clear();
		}

		public UnityEngine.Object GetAsset(string bundleName, string prefab)
		{

			Dictionary<string, UnityEngine.Object> dict;
			//Log.Info("GetAsset  " + bundleName.BundleNameToLower());
			if (!this.resourceCache.TryGetValue(bundleName.BundleNameToLower(), out dict))
			{
				//throw new Exception($"not found asset: {bundleName} {prefab}");
				//Log.DebugRed($"not found asset: {bundleName} {prefab}");
				return null;
			}

			UnityEngine.Object resource = null;
			if (!dict.TryGetValue(prefab, out resource))
			{
				//throw new Exception($"not found asset: {bundleName} {prefab}");
				//Log.DebugRed($"not found asset: {bundleName} {prefab}");
			}


			return resource;
		}


		public void UnloadBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.BundleNameToLower();

			string[] dependencies = AssetBundleHelper.GetSortedDependencies(assetBundleName);

			foreach (string dependency in dependencies)
			{
				this.UnloadOneBundle(dependency);
			}
		}

		public void UnloadOneBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.BundleNameToLower();
			ABInfo abInfo;
			if (!this.bundles.TryGetValue(assetBundleName, out abInfo))
			{
				return;
			}
			//Log.Info("UnloadOneBundle      " + assetBundleName);

			//Log.DebugWhtie($"---------- unload one bundle {assetBundleName} refcount: {abInfo.RefCount - 1}");

			--abInfo.RefCount;

			if (abInfo.RefCount > 0)
			{
				return;
			}


			this.bundles.Remove(assetBundleName);
			abInfo.Dispose();

			if (!this.resourceCache.TryGetValue(assetBundleName, out Dictionary<string, UnityEngine.Object> dict)) return;
			dict.Clear();

			this.resourceCache.Remove(assetBundleName);

			// Log.DebugGreen($"cache count: {assetBundleName}");
		}

		/// <summary>
		/// 同步加载assetbundle
		/// </summary>
		/// <param name="assetBundleName"></param>
		/// <returns></returns>
		public void LoadBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.ToLower();

			string[] dependencies = AssetBundleHelper.GetSortedDependencies(assetBundleName);
			//Log.Debug($"-----------dep load {assetBundleName} dep: {dependencies.ToList().ListToString()}");
			foreach (string dependency in dependencies)
			{
				if (string.IsNullOrEmpty(dependency))
				{
					continue;
				}
				this.LoadOneBundle(dependency);
			}
		}

		public void AddResource(string bundleName, string assetName, UnityEngine.Object resource)
		{
			//Log.Info("AddResource   1"  + bundleName + "  assetName = " + assetName);

			Dictionary<string, UnityEngine.Object> dict;
			if (!this.resourceCache.TryGetValue(bundleName.BundleNameToLower(), out dict))
			{
				dict = new Dictionary<string, UnityEngine.Object>();
				this.resourceCache[bundleName] = dict;
			}

			dict[assetName] = resource;
		}

		public void LoadOneBundle(string assetBundleName)
		{
			//        if (IsLoadBundle(assetBundleName))
			//        {
			////Log.Info("LoadOneBundle  资源包已经加载  " + assetBundleName);
			//return;
			//        }
			ABInfo abInfo;
			if (this.bundles.TryGetValue(assetBundleName, out abInfo))
			{
				++abInfo.RefCount;
				return;
			}
			//这个变量表示是加载AB包 还是直接加载资源
			if (!Define.IsAsync)
			{
				string[] realPath = null;
#if UNITY_EDITOR
				realPath = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
				foreach (string s in realPath)
				{
					string assetName = Path.GetFileNameWithoutExtension(s);
					UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
					AddResource(assetBundleName, assetName, resource);
				}

				abInfo = ComponentFactory.CreateWithParent<ABInfo, string, AssetBundle>(this, assetBundleName, null);
				abInfo.Parent = this;
				this.bundles[assetBundleName] = abInfo;
#endif
				return;
			}

			string p = Path.Combine(PathHelper.AppHotfixResPath, assetBundleName);
			AssetBundle assetBundle = null;
			Log.Info("LoadOneBundle -------------------------------- " + assetBundleName);

			if (File.Exists(p))
			{
				Log.Info($"LoadOneBundle path [hotfix] {p}");
				assetBundle = AssetBundle.LoadFromFile(p);
			}
			else
			{
				p = Path.Combine(PathHelper.AppResPath, assetBundleName);
				Log.Info($"LoadOneBundle path [streaming] {p}");
				assetBundle = AssetBundle.LoadFromFile(p);
			}

			if (assetBundle == null)
			{
				//throw new Exception($"assets bundle not found: {assetBundleName}");
				Log.DebugRed($"assets bundle not found: {assetBundleName}");
				return;
			}

			if (!assetBundle.isStreamedSceneAssetBundle)
			{
				// 异步load资源到内存cache住
				UnityEngine.Object[] assets = assetBundle.LoadAllAssets();
				foreach (UnityEngine.Object asset in assets)
				{
					AddResource(assetBundleName, asset.name, asset);
				}
			}
			//实际上是对ABInfo内部成员进行赋值
			abInfo = ComponentFactory.CreateWithParent<ABInfo, string, AssetBundle>(this, assetBundleName, assetBundle);
			abInfo.Parent = this;
			Log.Info("LoadOneBundle 加入缓存-------------------------------- " + assetBundleName);

			this.bundles[assetBundleName] = abInfo;
		}

		///// <summary>
		///// 异步加载assetbundle
		///// </summary>
		///// <param name="assetBundleName"></param>
		///// <returns></returns>
		//public bool IsLoadBundle(string assetBundleName)
		//{
		//	return this.bundles.ContainsKey(assetBundleName);
		//}

		public ETTaskCompletionSource<int> CreateTask(string assetBundleName)
		{
			var tcs = new ETTaskCompletionSource<int>();
			if (LoadAllTasks.ContainsKey(assetBundleName))
			{
				LoadAllTasks[assetBundleName].Add(tcs);
			}
			else
			{
				var list = new List<ETTaskCompletionSource<int>>();
				LoadAllTasks.Add(assetBundleName, list);
				list.Add(tcs);
			}
			return tcs;
		}

		public void CallAllTask(string assetBundleName)
		{
			if (LoadAllTasks.ContainsKey(assetBundleName))
			{
				var list = LoadAllTasks[assetBundleName];
				for (int i = 0; i < list.Count; i++)
				{
					list[i].SetResult(1);
				}
				//Log.Info("通知回调数量  " + list.Count);
				list.Clear();
			}
		}

		public async ETTask LHLoadBundleAsync(string assetBundleName)
		{
			//Log.Info("LoadBundleAsync "  + assetBundleName);
			assetBundleName = assetBundleName.ToLower();
			this.bundles.TryGetValue(assetBundleName, out ABInfo aBInfo);
			if (aBInfo != null)
			{
				aBInfo.RefCount++;
				await ETTask.CompletedTask;
				return;
			}

			var task = CreateTask(assetBundleName);
			LHLoadBundleAsync2(assetBundleName).Coroutine();
			await task.Task;
		}

		public async ETTask LHLoadBundleAsync2(string assetBundleName)
		{
			//await TimerComponent.Instance.WaitAsync(100);
			//查看资源是否已经加载完成 -----------------
			//Log.Info("LHLoadBundleAsync2 -------------------------------- " + assetBundleName);

			//if (IsLoadBundle(assetBundleName) )
			//         {
			//	//Log.Info("资源已加载  " + assetBundleName);
			//	await ETTask.CompletedTask;
			//	//资源已经加载完成---- 直接广播完成
			//	CallAllTask(assetBundleName);
			//	return;
			//         }

			//资源未加载完成---- 查看是否正在加载资源
			this.bundles.TryGetValue(assetBundleName, out ABInfo info);
			if (info == null) // 当前引用小于0 证明没有加载过资源
			{
				info = ComponentFactory.CreateWithParent<ABInfo, string>(this, assetBundleName);
				info.Parent = this;
				bundles[assetBundleName] = info;

				//加载资源
				string[] dependencies = AssetBundleHelper.GetSortedDependencies(assetBundleName);
				//Log.Info("LHLoadBundleAsync3  ---------------------------   " + assetBundleName + "  " + dependencies.Length);

				foreach (string dependency in dependencies)
				{
					if (string.IsNullOrEmpty(dependency))
					{
						continue;
					}

					await LHLoadOneBundleAsync(dependency, info);
				}
				//Log.Info("加载完成所有  通知所有回调  执行完成 " + assetBundleName);
				CallAllTask(assetBundleName);
			}
			else
			{
				if (info.AssetBundle)
				{
					info.RefCount++;
					await ETTask.CompletedTask;
					CallAllTask(assetBundleName);
				}
				else
				{
					info.RefCount++;
				}
			}
		}

		public async ETTask LHLoadOneBundleAsync(string assetBundleName, ABInfo abInfo)
		{
			//Log.Info("LHLoadOneBundleAsync   " + assetBundleName);

			if (resourceCache.ContainsKey(assetBundleName))
			{
				await ETTask.CompletedTask;
				//Log.Info("LHLoadOneBundleAsync 已经存在  " + assetBundleName);
				return;
			}

			if (!Define.IsAsync)
			{
				string[] realPath = null;
#if UNITY_EDITOR
				realPath = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
				//Log.Info("LHLoadOneBundleAsync 3  " + assetBundleName + "  " + realPath.Length);

				for (int i = 0; i < realPath.Length; i++)
				{
					string s = realPath[i];
					//Log.Info("LHLoadOneBundleAsync 4  " + assetBundleName + "  " + s);

					string assetName = Path.GetFileNameWithoutExtension(s);
					UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
					AddResource(assetBundleName, assetName, resource);
				}

				//abInfo = ComponentFactory.CreateWithParent<ABInfo, string, AssetBundle>(this, assetBundleName, null);
				//abInfo.Parent = this;
				//this.bundles[assetBundleName] = abInfo;
#endif
				return;
			}

			string p = Path.Combine(PathHelper.AppHotfixResPath, assetBundleName);
			AssetBundle assetBundle = null;
			if (!File.Exists(p))
			{
				p = Path.Combine(PathHelper.AppResPath, assetBundleName);
			}

			using (AssetsBundleLoaderAsync assetsBundleLoaderAsync = ComponentFactory.Create<AssetsBundleLoaderAsync>())
			{
				assetBundle = await assetsBundleLoaderAsync.LoadAsync(p);
			}

			if (assetBundle == null)
			{
				//throw new Exception($"assets bundle not found: {assetBundleName}");
				Log.DebugRed($"assets bundle not found: {assetBundleName}");

				return;

			}

			if (!assetBundle.isStreamedSceneAssetBundle)
			{
				// 异步load资源到内存cache住
				UnityEngine.Object[] assets;
				using (AssetsLoaderAsync assetsLoaderAsync = ComponentFactory.Create<AssetsLoaderAsync, AssetBundle>(assetBundle))
				{
					assets = await assetsLoaderAsync.LoadAllAssetsAsync();
				}
				foreach (UnityEngine.Object asset in assets)
				{
					AddResource(assetBundleName, asset.name, asset);
				}
			}

			//assetBundle = await AssetBundleHelper.UnityLoadBundleAsync(p);
			//if (assetBundle == null)
			//{
			//	// 获取资源的时候会抛异常，这个地方不直接抛异常，因为有些地方需要Load之后判断是否Load成功
			//	Log.Warning($"assets bundle not found: {assetBundleName}");
			//	await ETTask.CompletedTask;
			//	return;
			//}

			//if (!assetBundle.isStreamedSceneAssetBundle)
			//{
			//	Log.Info("assetBundle   " + assetBundle);
			//	// 异步load资源到内存cache住
			//	var assets = await AssetBundleHelper.UnityLoadAssetAsync(assetBundle);

			//	foreach (UnityEngine.Object asset in assets)
			//	{
			//		Log.Info("assetBundleName   " + asset.name);

			//		AddResource(assetBundleName, asset.name, asset);
			//	}
			//}
			abInfo.AssetBundle = assetBundle;
		}


		///// <summary>
		///// 异步加载assetbundle
		///// </summary>
		///// <param name="assetBundleName"></param>
		///// <returns></returns>
		//public async ETTask LoadBundleAsync(string assetBundleName)
		//{
		//	// Log.Info("LoadBundleAsync "  + assetBundleName);
		//	assetBundleName = assetBundleName.ToLower();
		//	if (IsLoadBundle(assetBundleName)) return;

		//	string[] dependencies = AssetBundleHelper.GetSortedDependencies(assetBundleName);
		//	foreach (string dependency in dependencies)
		//	{
		//		if (string.IsNullOrEmpty(dependency))
		//		{
		//			continue;
		//		}

		//		await LoadOneBundleAsync(dependency);
		//	}
		//}


		//		public async ETTask LoadOneBundleAsync(string assetBundleName)
		//		{

		//            Log.Info("LoadOneBundleAsync   1"  + assetBundleName);

		//			ABInfo abInfo;
		//			if (this.bundles.TryGetValue(assetBundleName, out abInfo))
		//			{
		//				++abInfo.RefCount;
		//				return;
		//			}
		//			if (IsLoadBundle(assetBundleName))
		//			{
		//				++abInfo.RefCount;
		//				return;
		//			}

		//			if (!Define.IsAsync)
		//			{
		//				string[] realPath = null;
		//#if UNITY_EDITOR
		//				realPath = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
		//				foreach (string s in realPath)
		//				{
		//					string assetName = Path.GetFileNameWithoutExtension(s);
		//					UnityEngine.Object resource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
		//					AddResource(assetBundleName, assetName, resource);
		//				}

		//				abInfo = ComponentFactory.CreateWithParent<ABInfo, string, AssetBundle>(this, assetBundleName, null);
		//				abInfo.Parent = this;
		//				this.bundles[assetBundleName] = abInfo;
		//#endif
		//				return;
		//			}

		//			string p = Path.Combine(PathHelper.AppHotfixResPath, assetBundleName);
		//			AssetBundle assetBundle = null;
		//			if (!File.Exists(p))
		//			{
		//				p = Path.Combine(PathHelper.AppResPath, assetBundleName);
		//			}

		//			using (AssetsBundleLoaderAsync assetsBundleLoaderAsync = ComponentFactory.Create<AssetsBundleLoaderAsync>())
		//			{
		//				assetBundle = await assetsBundleLoaderAsync.LoadAsync(p);
		//			}

		//			if (assetBundle == null)
		//			{
		//				//throw new Exception($"assets bundle not found: {assetBundleName}");
		//				Log.DebugRed($"assets bundle not found: {assetBundleName}");

		//				return;

		//			}

		//			if (!assetBundle.isStreamedSceneAssetBundle)
		//			{
		//				// 异步load资源到内存cache住
		//				UnityEngine.Object[] assets;
		//				using (AssetsLoaderAsync assetsLoaderAsync = ComponentFactory.Create<AssetsLoaderAsync, AssetBundle>(assetBundle))
		//				{
		//					assets = await assetsLoaderAsync.LoadAllAssetsAsync();
		//				}
		//				foreach (UnityEngine.Object asset in assets)
		//				{
		//					AddResource(assetBundleName, asset.name, asset);
		//				}
		//			}

		//			abInfo = ComponentFactory.CreateWithParent<ABInfo, string, AssetBundle>(this, assetBundleName, assetBundle);
		//			abInfo.Parent = this;
		//			bundles[assetBundleName] = abInfo;
		//		}

		public string DebugString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (ABInfo abInfo in this.bundles.Values)
			{
				sb.Append($"{abInfo.Name}:{abInfo.RefCount}\n");
			}
			return sb.ToString();
		}

	}
}
