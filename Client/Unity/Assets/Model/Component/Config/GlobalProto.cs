using System;
using System.Net;

namespace ETModel
{
	public class GlobalProto
	{
		//资源路径
		public string AssetBundleServerUrl;
		//服务器地址
		public string Address;

		private const string AndroidEmulatorHost = "10.0.2.2";
		private const string LocalLoopbackHost = "localhost";

		//获取资源路径
		public string GetUrl()
		{
			string url = NormalizeAssetBundleServerUrl(this.AssetBundleServerUrl);
#if UNITY_ANDROID
			url += "Android/";
#elif UNITY_IOS
			url += "IOS/";
#elif UNITY_WEBGL
			url += "WebGL/";
#elif UNITY_STANDALONE_OSX
			url += "MacOS/";
#else
			url += "PC/";
#endif
			return url;
		}

		private static string NormalizeAssetBundleServerUrl(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return string.Empty;
			}

			if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
			{
				return EnsureTrailingSlash(url);
			}

#if UNITY_ANDROID && !UNITY_EDITOR
			if (string.Equals(uri.Host, IPAddress.Loopback.ToString(), StringComparison.OrdinalIgnoreCase) ||
				string.Equals(uri.Host, LocalLoopbackHost, StringComparison.OrdinalIgnoreCase))
			{
				uri = new UriBuilder(uri) { Host = AndroidEmulatorHost }.Uri;
			}
#elif UNITY_EDITOR || UNITY_STANDALONE
			if (string.Equals(uri.Host, AndroidEmulatorHost, StringComparison.OrdinalIgnoreCase))
			{
				uri = new UriBuilder(uri) { Host = LocalLoopbackHost }.Uri;
			}
#endif

			return EnsureTrailingSlash(uri.AbsoluteUri);
		}

		private static string EnsureTrailingSlash(string url)
		{
			return url.EndsWith("/", StringComparison.Ordinal) ? url : $"{url}/";
		}
	}
}
