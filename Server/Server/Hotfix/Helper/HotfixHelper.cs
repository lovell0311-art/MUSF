using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.IO;
using System.Reflection;

namespace ETHotfix
{
	public static class HotfixHelper
	{
		public static object Create(object old)
		{
			Assembly assembly = typeof(HotfixHelper).Assembly;
			string objectName = old.GetType().FullName;
			return Activator.CreateInstance(assembly.GetType(objectName));
		}

		public static string ToIntString(this Enum e)
		{
			return Convert.ToInt32(e).ToString();
		}

		public static T Clone<T>(this T oType)
		{
			using (MemoryStream memoryStream = new MemoryStream(oType.ToBson()))
			{
				return (T)BsonSerializer.Deserialize(memoryStream, typeof(T));
			}
		}
	}
}
