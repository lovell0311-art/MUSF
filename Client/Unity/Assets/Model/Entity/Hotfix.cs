using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if !ILRuntime
using System.Reflection;
#endif

namespace ETModel
{
	public sealed class Hotfix: Object
	{

#if ILRuntime
        private ILRuntime.Runtime.Enviorment.AppDomain appDomain;
        private MemoryStream dllStream;
        private MemoryStream pdbStream;
#else
		private Assembly assembly;
#endif

       

        private IStaticMethod start;
		private List<Type> hotfixTypes;

		public Action Update;
		public Action LateUpdate;
		public Action FixedUpdate;
		public Action OnApplicationQuit;

        public Action OnApplicationBackground;
        public Action OnApplicationForeground;

        public void GotoHotfix()
		{

#if ILRuntime
            ILHelper.InitILRuntime(this.appDomain);

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            this.appDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
#endif
            this.start.Run();
		}

		public List<Type> GetHotfixTypes()
		{
			return this.hotfixTypes;
		}

		public void LoadHotfixAssembly()
		{
			//加载AB包
			Game.Scene.GetComponent<AssetBundleComponent>().LoadBundle($"code.unity3d");
			//加载AB包中的资源
			GameObject code = (GameObject)Game.Scene.GetComponent<AssetBundleComponent>().
				GetAsset("code.unity3d", "Code");
            /* byte[] assBytes = code.Get<TextAsset>("Hotfix.dll").bytes;
             byte[] pdbBytes = code.Get<TextAsset>("Hotfix.pdb").bytes;*/

            ReferenceCollector referenceCollector = code.GetReferenceCollector();
            byte[] assBytes = referenceCollector.GetTextAsset("Hotfix.dll").bytes;

            byte[] pdbBytes = referenceCollector.GetTextAsset("Hotfix.pdb").bytes;

#if ILRuntime
          
            this.appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

            this.dllStream = new MemoryStream(assBytes);
            this.pdbStream = new MemoryStream(pdbBytes);
            //加载热更新的dll
            //this.appDomain.LoadAssembly(this.dllStream, this.pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            this.appDomain.LoadAssembly(this.dllStream,null, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

            //获取静态方法 start  ETHotfix.Init->Start
            this.start = new ILStaticMethod(this.appDomain, "ETHotfix.Init", "Start", 0);

            this.hotfixTypes = this.appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();
#else
          

			this.assembly = Assembly.Load(assBytes, pdbBytes);

			Type hotfixInit = this.assembly.GetType("ETHotfix.Init");
			this.start = new MonoStaticMethod(hotfixInit, "Start");
			
			this.hotfixTypes = ReflectionTypeHelper.GetLoadableTypes(this.assembly, "Hotfix.LoadHotfixAssembly").ToList();
#endif
            //卸载AB包
            Game.Scene.GetComponent<AssetBundleComponent>().UnloadBundle($"code.unity3d");

            //初始化IL
            Game.Hotfix.GotoHotfix();
        }
	}
}
