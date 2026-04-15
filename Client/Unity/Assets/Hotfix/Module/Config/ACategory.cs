using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;

namespace ETHotfix
{
	public abstract class ACategory : Object
	{
		public abstract Type ConfigType { get; }
		public abstract IConfig GetOne();
		public abstract IConfig[] GetAll();
		public abstract IConfig TryGet(int type);
	}

	/// <summary>
	/// 管理该所有的配置
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ACategory<T>: ACategory where T : IConfig
	{
		protected Dictionary<long, IConfig> dict;
	

		public override void BeginInit()
		{
			this.dict = new Dictionary<long, IConfig>();
		

			string configStr = ConfigHelper.GetText(typeof (T).Name);

			foreach (string str in configStr.Split(new[] { "\n" }, StringSplitOptions.None))
			{
				try
				{
					string str2 = str.Trim();
					if (str2 == "")
					{
						continue;
					}
					T t = ConfigHelper.ToObject<T>(str2);
					this.dict.Add(t.Id, t);
					
				}
				catch (Exception e)
				{
				//	Log.Debug($"ConfigHelper.ToObject<T>(str2):{ConfigHelper.ToObject<T>(str.Trim())}");
				//	Log.DebugGreen($"configStr:{configStr}->{str}");
					throw new Exception($"parser json fail: {str}", e);
				}
			}
		}

		public override Type ConfigType
		{
			get
			{
				return typeof (T);
			}
		}

		public override void EndInit()
		{
		}
		/// <summary>
		/// 根据ID 获取
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override IConfig TryGet(int type)
		{
			IConfig t;
			if (!this.dict.TryGetValue(type, out t))
			{
				return null;
			}
			return t;
		}
		/// <summary>
		/// 获取所有
		/// </summary>
		/// <returns></returns>
		public override IConfig[] GetAll()
		{
			return this.dict.Values.ToArray();
		}
		/// <summary>
		/// 获取第一个
		/// </summary>
		/// <returns></returns>
		public override IConfig GetOne()
		{
			return this.dict.Values.First();
		}

		
	}
}