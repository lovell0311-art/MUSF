using System;
using System.IO;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
	public static class ConfigHelper
	{
		public static string GetText(string key)
		{
			try
			{
				return ETModel.ConfigHelper.GetText(key);
			}
			catch (Exception e)
			{
				string[] fallbackPaths =
				{
					Path.Combine(PathHelper.AppHotfixResPath, $"{key}.txt"),
					Path.Combine(PathHelper.AppResPath, $"{key}.txt"),
				};

				foreach (string path in fallbackPaths)
				{
					if (File.Exists(path))
					{
						return File.ReadAllText(path);
					}
				}

				throw new Exception($"load config file fail, key: {key}", e);
			}
		}

		public static T ToObject<T>(string str)
		{
			return JsonHelper.FromJson<T>(str);
		}
	}
}
