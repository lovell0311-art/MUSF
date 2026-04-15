using System;
using System.IO;
using UnityEngine;

namespace ETModel
{
	public static class ConfigHelper
	{
        //获取配置文本
		public static string GetText(string key)
		{
			try
			{
				AssetBundleComponent assetBundleComponent = Game.Scene.GetComponent<AssetBundleComponent>();
				if (assetBundleComponent == null)
				{
					throw new Exception("AssetBundleComponent is null");
				}

                //这里指向了Bundles/Independent/Config预制件 传递参数UnitConfig或者BuffConfig
                //或者以后其他自定义的,即可访问他们身上挂载的配置文本
                GameObject config = assetBundleComponent.GetAsset("config.unity3d", "Config") as GameObject;
				TextAsset textAsset = null;
				if (config != null)
				{
					ReferenceCollector referenceCollector = config.GetReferenceCollector();
					if (referenceCollector != null)
					{
						textAsset = referenceCollector.GetTextAsset(key);
					}
				}

				if (textAsset == null)
				{
					textAsset = assetBundleComponent.GetAsset("config.unity3d", key) as TextAsset;
				}

				if (textAsset != null)
				{
					return textAsset.text;
				}

				string hotfixPath = Path.Combine(PathHelper.AppHotfixResPath, $"{key}.txt");
				if (File.Exists(hotfixPath))
				{
					return File.ReadAllText(hotfixPath);
				}

				string streamingPath = Path.Combine(PathHelper.AppResPath, $"{key}.txt");
				if (File.Exists(streamingPath))
				{
					return File.ReadAllText(streamingPath);
				}

				throw new Exception($"config text asset missing: {key}");
			}
			catch (Exception e)
			{
				throw new Exception($"load config file fail, key: {key}", e);
			}
		}

        //获取全局配置
        //从Resources文件夹下获取KV预制件,其身上挂载的GlobalProto引用指向了Res/Config/GlobalProto文本
        //暂时只包含了文件服务器URL的配置以及远程服务器的IP与端口
        public static string GetGlobal()
		{
			try
			{
				GameObject config = (GameObject)ResourcesHelper.Load("KV");
				string configStr = config.GetReferenceCollector().GetTextAsset("GlobalProto").text;
				return configStr;
			}
			catch (Exception e)
			{
				throw new Exception($"load global config file fail", e);
			}
		}

		public static T ToObject<T>(string str)
		{
			//Log.DebugBrown($"{str}");
		    return JsonHelper.FromJson<T>(str);
			//return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
			
		}
	}
}
