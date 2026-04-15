using System;
using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
	public abstract class ACategory : Object
	{
		public abstract Type ConfigType { get; }
	}

	/// <summary>
	/// 管理该所有的配置
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ACategory<T> : ACategory where T : IConfig
	{
		public readonly Dictionary<int, T> JsonDic = new Dictionary<int, T>();

		public override void BeginInit()
		{
			string configStr = ConfigHelper.GetText(typeof(T).Name);

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
					this.JsonDic.Add(t.Id, t);
				}
				catch (Exception e)
				{
					throw new Exception($"parser json fail: {str}", e);
				}
			}
		}

		public override Type ConfigType
		{
			get
			{
				return typeof(T);
			}
		}

		public override void EndInit()
		{
		}
	}
}