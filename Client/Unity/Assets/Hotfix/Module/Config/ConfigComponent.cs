using System;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[ObjectSystem]
	public class ConfigComponentAwakeSystem : AwakeSystem<ConfigComponent>
	{
		public override void Awake(ConfigComponent self)
		{
			self.Awake();
		}
	}

	[ObjectSystem]
	public class ConfigComponentLoadSystem : LoadSystem<ConfigComponent>
	{
		public override void Load(ConfigComponent self)
		{
			self.Load();
		}
	}

	/// <summary>
	/// Config组件会扫描所有的有ConfigAttribute标签的配置,加载进来
	/// </summary>
	public class ConfigComponent: Component
	{
		private readonly Dictionary<Type, ACategory> allConfig = new Dictionary<Type, ACategory>();

		public static ConfigComponent Instance;
		public void Awake()
		{
			Instance = this;
			ETModel.Game.Scene.GetComponent<AssetBundleComponent>().LoadBundle("config.unity3d");
			this.Load();
			ETModel.Game.Scene.GetComponent<AssetBundleComponent>().UnloadBundle("config.unity3d");
		}

		public void Load()
		{
			this.allConfig.Clear();
			List<Type> types = Game.EventSystem.GetTypes();
			
			foreach (Type type in types)
			{
				object[] attrs = type.GetCustomAttributes(typeof (ConfigAttribute), false);
				if (attrs.Length == 0)
				{
					continue;
				}

                ConfigAttribute configAttribute = attrs[0] as ConfigAttribute;
                //只加载指定的配置

                //if (!configAttribute.Type.Is(AppType.ClientH))
                //{
                //    continue;
                //}

                object obj = Activator.CreateInstance(type);

				ACategory iCategory = obj as ACategory;
				if (iCategory == null)
				{
					throw new Exception($"class: {type.Name} not inherit from ACategory");
				}
				try
				{
					iCategory.BeginInit();
					iCategory.EndInit();
					this.allConfig[iCategory.ConfigType] = iCategory;
				}
				catch (Exception e)
				{
					Log.Error($"skip config load: {type.FullName}\n{e}");
				}
			}
		}
		/// <summary>
		/// 根据类型 获取其缓存的配置的第一条数据
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IConfig GetOne(Type type)
		{
			ACategory configCategory;
			if (!this.allConfig.TryGetValue(type, out configCategory))
			{
				throw new Exception($"ConfigComponent not found key: {type.FullName}");
			}
			return configCategory.GetOne();
		}
		/// <summary>
		/// 根据类型 获取其缓存的配置 再根据ID获取对应的数据
		/// </summary>
		/// <param name="type">配置表类型</param>
		/// <param name="id">该条数据得id</param>
		/// <returns></returns>
		public IConfig Get(Type type, int id)
		{
			ACategory configCategory;
			if (!this.allConfig.TryGetValue(type, out configCategory))
			{
				throw new Exception($"ConfigComponent not found key: {type.FullName}");
			}

			return configCategory.TryGet(id);
		}
		/// <summary>
		/// 根据 配置表类型和id 获取该条数据
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public IConfig TryGet(Type type, int id)
		{
			ACategory configCategory;
			if (!this.allConfig.TryGetValue(type, out configCategory))
			{
				return null;
			}
			return configCategory.TryGet(id);
		}
		/// <summary>
		/// 根据id 从配置表中获取数据
		/// </summary>
		/// <typeparam name="T">配置表类型</typeparam>
		/// <param name="id">在配置表中的 id</param>
		/// <returns></returns>
		public T GetItem<T>(int id) where T : IConfig 
		{
			ACategory configCategory;
			if (!this.allConfig.TryGetValue(typeof(T),out configCategory))
			{
				throw new Exception($"ConfigComponent not found key: {typeof(T).FullName}");
			}
			var temp = configCategory.TryGet(id);
			if (temp == null)
			{
				return default(T);
			}
			return (T)temp;
		}
		/// <summary>
		/// 获得配置表中得所有 数据
		/// </summary>
		/// <param name="type">配置表类型</param>
		/// <returns></returns>
		public IConfig[] GetAll(Type type)
		{
			ACategory configCategory;
			if (!this.allConfig.TryGetValue(type, out configCategory))
			{
				throw new Exception($"ConfigComponent not found key: {type.FullName}");
			}
			return configCategory.GetAll();
		}
		public IConfig[] GetAll<T>() where T : IConfig
		{
			ACategory configCategory;
			if (!this.allConfig.TryGetValue(typeof(T), out configCategory))
			{
				throw new Exception($"ConfigComponent not found key: {typeof(T).FullName}");
			}

			return configCategory.GetAll();
		}

		public ACategory GetCategory(Type type)
		{
			ACategory configCategory;
			bool ret = this.allConfig.TryGetValue(type, out configCategory);
			return ret? configCategory : null;
		}
	}
}
