using System;

using ETModel;
using UnityEngine;
using UnityEngine.Analytics;

namespace ETHotfix
{
	public static class Init
	{
		public static void Start()
		{
#if ILRuntime
			if (!Define.IsILRuntime)
			{
				Log.Error("mono层是mono模式, 但是Hotfix层是ILRuntime模式");
			}
#else
			if (Define.IsILRuntime)
			{
				Log.Error("mono层是ILRuntime模式, Hotfix层是mono模式");
			}
#endif
            // Debug.unityLogger.logEnabled = true;
           
            try
			{
				
				// 注册热更层回调
				ETModel.Game.Hotfix.Update = () => { Update(); };
				ETModel.Game.Hotfix.LateUpdate = () => { LateUpdate(); };
				ETModel.Game.Hotfix.FixedUpdate = () => { FixedUpdate(); };
				ETModel.Game.Hotfix.OnApplicationQuit += () => { OnApplicationQuit(); };

				Game.Scene.AddComponent<LogCollectionComponent>();

				Game.Scene.AddComponent<UIComponent>();
				Game.Scene.AddComponent<OpcodeTypeComponent>();
				Game.Scene.AddComponent<MessageDispatcherComponent>();
				Game.Scene.AddComponent<DeviceComponent>();
				// 加载热更配置
				Game.Scene.AddComponent<ConfigComponent>();
				//支付组件
				Game.Scene.AddComponent<TopUpComponent>();
                //添加语言组件
              //  Game.Scene.AddComponent<LuguageComponent>();
                //添加实体管理组件
                Game.Scene.AddComponent<UnitEntityComponent>();
                Game.Scene.AddComponent<SafeAreaComponent>();
                Game.Scene.AddComponent<TransferPointTools>();
                Game.Scene.AddComponent<SceneComponent>();
				Game.Scene.AddComponent<PreLoadComponent>();

                Game.Scene.AddComponent<TreasureMapComponent>();

                Game.Scene.AddComponent<GuideComponent>();

                Game.EventSystem.Run(EventIdType.InitSceneStart);

               
            }
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void Update()
		{
			try
			{
				Game.EventSystem.Update();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void LateUpdate()
		{
			try
			{
				Game.EventSystem.LateUpdate();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}
		public static void FixedUpdate()
		{
			try
			{
				Game.EventSystem.FixedUpdate();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		public static void OnApplicationQuit()
		{
			Game.Close();
			//断开连接
		}
	}
}