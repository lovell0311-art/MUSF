using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
	[ObjectSystem]
	public class UiBundleDownloaderComponentAwakeSystem : AwakeSystem<BundleDownloaderComponent>
	{
		public override void Awake(BundleDownloaderComponent self)
		{
			self.bundles = new Queue<string>();
			self.downloadedBundles = new HashSet<string>();
			self.downloadingBundle = "";
		}
	}

	/// <summary>
	/// 用来对比web端的资源，比较md5，对比下载资源
	/// </summary>
	public class BundleDownloaderComponent : Component
	{
		private VersionConfig remoteVersionConfig;
		private string remoteVersionText;
		
		public Queue<string> bundles;

		public long TotalSize;

		public HashSet<string> downloadedBundles;

		public string downloadingBundle;

		public UnityWebRequestAsync webRequest;

		private static string NormalizeVersionText(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return string.Empty;
			}

			return text.Replace("\r\n", "\n").Trim();
		}

		private void TraceUpdate(string message)
		{
			try
			{
				Directory.CreateDirectory(PathHelper.AppHotfixResPath);
				string tracePath = Path.Combine(PathHelper.AppHotfixResPath, "update-trace.txt");
				using (FileStream fs = new FileStream(tracePath, FileMode.Append, FileAccess.Write, FileShare.Read))
				using (StreamWriter writer = new StreamWriter(fs))
				{
					writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}");
				}
			}
			catch
			{
			}
		}

		private void PersistHotfixVersionFile()
		{
			if (this.remoteVersionConfig == null)
			{
				Debug.LogWarning("PersistHotfixVersionFile skipped: remoteVersionConfig is null");
				this.TraceUpdate("PersistHotfixVersionFile skipped: remoteVersionConfig is null");
				return;
			}

			try
			{
				Directory.CreateDirectory(PathHelper.AppHotfixResPath);
				string hotfixVersionPath = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");
				string json = string.IsNullOrWhiteSpace(this.remoteVersionText)
					? JsonHelper.ToJson(this.remoteVersionConfig)
					: this.remoteVersionText;
				using (FileStream fs = new FileStream(hotfixVersionPath, FileMode.Create, FileAccess.Write, FileShare.Read))
				using (StreamWriter writer = new StreamWriter(fs, new System.Text.UTF8Encoding(false)))
				{
					writer.Write(json);
					writer.Flush();
					fs.Flush(true);
				}
				Debug.LogWarning($"PersistHotfixVersionFile ok: {hotfixVersionPath} version={this.remoteVersionConfig.Version} len={json.Length}");
				this.TraceUpdate($"PersistHotfixVersionFile ok version={this.remoteVersionConfig.Version} len={json.Length} path={hotfixVersionPath}");
			}
			catch (Exception e)
			{
				this.TraceUpdate($"PersistHotfixVersionFile error {e.Message}");
				Log.Error($"persist hotfix Version.txt error:\n{e}");
			}
		}

		public async ETTask StartAsync()
		{
			// 获取远程的Version.txt
			string versionUrl = "";
			this.TraceUpdate("StartAsync begin");
			try
			{
				using (UnityWebRequestAsync webRequestAsync = ComponentFactory.Create<UnityWebRequestAsync>())
				{
                    //UnityWebRequest UnityWebRequest www下载
                    versionUrl = GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + "Version.txt";
                 //   Log.DebugBrown($"远程:{versionUrl}");
                    await webRequestAsync.DownloadAsync(versionUrl);
					this.remoteVersionText = webRequestAsync.Request.downloadHandler.text;
					remoteVersionConfig = JsonHelper.FromJson<VersionConfig>(this.remoteVersionText);
					this.TraceUpdate($"Remote Version.txt fetched version={remoteVersionConfig?.Version ?? 0} url={versionUrl} len={this.remoteVersionText?.Length ?? 0}");
					//Log.Debug(JsonHelper.ToJson(this.VersionConfig));
				}

			}
			catch (Exception e)
			{
				throw new Exception($"url: {versionUrl}", e);
			}

			// 获取包内 StreamingAssets 的 Version.txt
			try
			{
				VersionConfig streamingVersionConfig = null;
				string versionPath = Path.Combine(PathHelper.AppResPath4Web, "Version.txt");
				this.TraceUpdate($"Load streaming Version.txt begin path={versionPath}");
				if (!string.IsNullOrWhiteSpace(versionPath))
				{
					try
					{
						using (UnityWebRequestAsync request = ComponentFactory.Create<UnityWebRequestAsync>())
						{
							await request.DownloadAsync(versionPath);
							streamingVersionConfig = JsonHelper.FromJson<VersionConfig>(request.Request.downloadHandler.text);
							this.TraceUpdate($"Load streaming Version.txt ok version={streamingVersionConfig?.Version ?? 0} len={request.Request.downloadHandler.text?.Length ?? 0}");
						}
					}
					catch
					{
						streamingVersionConfig = null;
						this.TraceUpdate("Load streaming Version.txt failed");
					}
				}

				VersionConfig hotfixVersionConfig = null;
				string hotfixVersionPath = Path.Combine(PathHelper.AppHotfixResPath, "Version.txt");
				string hotfixVersionText = string.Empty;
				bool hotfixVersionExists = File.Exists(hotfixVersionPath);
				this.TraceUpdate($"Load hotfix Version.txt begin path={hotfixVersionPath} exists={hotfixVersionExists}");
				if (hotfixVersionExists)
				{
					try
					{
						hotfixVersionText = File.ReadAllText(hotfixVersionPath);
						hotfixVersionConfig = JsonHelper.FromJson<VersionConfig>(hotfixVersionText);
						this.TraceUpdate($"Load hotfix Version.txt ok version={hotfixVersionConfig?.Version ?? 0} len={hotfixVersionText.Length}");
					}
					catch
					{
						hotfixVersionConfig = null;
						this.TraceUpdate("Load hotfix Version.txt failed");
					}
				}
				this.TraceUpdate("Load hotfix Version.txt stage complete");

				this.TraceUpdate($"Normalize version text begin remoteLen={this.remoteVersionText?.Length ?? 0} hotfixLen={hotfixVersionText?.Length ?? 0}");
				string normalizedRemoteVersionText = NormalizeVersionText(this.remoteVersionText);
				string normalizedHotfixVersionText = NormalizeVersionText(hotfixVersionText);
				this.TraceUpdate($"Normalize version text end remoteLen={normalizedRemoteVersionText.Length} hotfixLen={normalizedHotfixVersionText.Length}");
				if (!string.IsNullOrEmpty(normalizedRemoteVersionText) &&
					string.Equals(normalizedRemoteVersionText, normalizedHotfixVersionText, StringComparison.Ordinal))
				{
					streamingVersionConfig = hotfixVersionConfig ?? this.remoteVersionConfig;
					Debug.LogWarning($"BundleDownloader trust exact hotfix Version.txt: version={streamingVersionConfig?.Version ?? 0} path={hotfixVersionPath}");
					this.TraceUpdate($"Trust exact hotfix Version.txt version={streamingVersionConfig?.Version ?? 0} path={hotfixVersionPath}");
				}
				this.TraceUpdate($"Compare version numbers remote={remoteVersionConfig.Version} streaming={streamingVersionConfig?.Version ?? 0} hotfix={hotfixVersionConfig?.Version ?? 0}");

				// Android 首装时，包内 StreamingAssets 不能直接当作本地热更目录使用。
				// 如果持久化目录里已经有与远端同版本的 Version.txt，就直接信任本地热更目录，
				// 避免每次冷启动都对数千个 bundle 做全量 MD5 扫描。
#if UNITY_ANDROID && !UNITY_EDITOR
				if (hotfixVersionConfig != null && remoteVersionConfig.Version == hotfixVersionConfig.Version)
				{
					Debug.LogWarning($"BundleDownloader trust hotfix Version.txt: version={hotfixVersionConfig.Version} path={hotfixVersionPath}");
					this.TraceUpdate($"Trust hotfix Version.txt by version version={hotfixVersionConfig.Version} path={hotfixVersionPath}");
					streamingVersionConfig = hotfixVersionConfig;
				}
#endif

				//远程与本地的资源文件一样
				if (streamingVersionConfig != null && remoteVersionConfig.Version == streamingVersionConfig.Version)
				{
					this.TraceUpdate($"Skip update remote={remoteVersionConfig.Version} local={streamingVersionConfig.Version}");
					Debug.LogWarning($"BundleDownloader skip update: remote={remoteVersionConfig.Version} local={streamingVersionConfig.Version}");
					this.PersistHotfixVersionFile();
					return;
				}

				// 删掉远程不存在的文件
				DirectoryInfo directoryInfo = new DirectoryInfo(PathHelper.AppHotfixResPath);
				this.TraceUpdate($"Hotfix dir scan path={directoryInfo.FullName} exists={directoryInfo.Exists}");
            //如果存放热更新资源的文件夹存在
				if (directoryInfo.Exists)
				{
                //获取到里面的所有子文件
					FileInfo[] fileInfos = directoryInfo.GetFiles();
					foreach (FileInfo fileInfo in fileInfos)
					{
                    //就是远程下载下来的版本文件 json反序列化成一个实体类 FileInfoDict这个字典就包含了所有AB文件信息
						if (remoteVersionConfig.FileInfoDict.ContainsKey(fileInfo.Name))
						{
							continue;
						}

						if (fileInfo.Name == "Version.txt")
						{
							continue;
						}
					//if (fileInfo.Name == "ApkConfig.txt")
					//{
					//	continue;
					//}
					//为什么要删除掉远程不存在的文件呢 主要是为了避免加载资源出错 资源重复 会导致到报错
						fileInfo.Delete();
					}
				}
            //如果保存热更新资源的文件夹不存在的话 通过Create进行创建
				else
				{
					directoryInfo.Create();
					this.TraceUpdate("Hotfix dir created");
				}

			

				// 对比MD5
				this.TraceUpdate($"MD5 diff begin remoteFiles={remoteVersionConfig.FileInfoDict.Count}");
				foreach (FileVersionInfo fileVersionInfo in remoteVersionConfig.FileInfoDict.Values)
				{
				// 对比md5 跟我们本地的文件进行MD5对比
					string localFileMD5 = BundleHelper.GetBundleMD5(streamingVersionConfig, fileVersionInfo.File);
                //如果相等 就忽略 表示两个版本中 这个文件并未做任何改动
					if (fileVersionInfo.MD5 == localFileMD5)
					{
						continue;
					}
				
                //如果两个文件的MD5不一致 把要下载的文件 压入到队列
					this.bundles.Enqueue(fileVersionInfo.File);
                //下载的总大小也加上这个文件的大小
					this.TotalSize += fileVersionInfo.Size;
				}
				this.TraceUpdate($"MD5 diff end bundleCount={this.bundles.Count} totalSize={this.TotalSize}");

				// 远端版本号已变化，但经过 MD5 对比后没有实际差异文件时，
				// 也要把最新 Version.txt 落到热更目录，避免下次冷启动继续全量扫描。
				if (this.bundles.Count == 0)
				{
					Debug.LogWarning($"BundleDownloader no bundle diff: remote={remoteVersionConfig.Version} streaming={streamingVersionConfig?.Version ?? 0} hotfix={hotfixVersionConfig?.Version ?? 0}");
					this.TraceUpdate($"No bundle diff remote={remoteVersionConfig.Version} streaming={streamingVersionConfig?.Version ?? 0} hotfix={hotfixVersionConfig?.Version ?? 0}");
					this.PersistHotfixVersionFile();
				}
				else
				{
					this.TraceUpdate($"Bundle diff count={this.bundles.Count} totalSize={this.TotalSize} remote={remoteVersionConfig.Version} streaming={streamingVersionConfig?.Version ?? 0} hotfix={hotfixVersionConfig?.Version ?? 0}");
				}
			}
			catch (Exception e)
			{
				this.TraceUpdate($"StartAsync post-fetch exception {e.GetType().Name}: {e.Message}");
				throw;
			}
		}

        //下载进度0%-100%
		public int Progress
		{
			get
			{
				if (this.TotalSize == 0)
				{
					return 0;
				}

				long alreadyDownloadBytes = 0;
                //已经下载的文件大小
				foreach (string downloadedBundle in this.downloadedBundles)
				{
					long size = this.remoteVersionConfig.FileInfoDict[downloadedBundle].Size;
					alreadyDownloadBytes += size;
				}
                //包括正在下载的文件大小
				if (this.webRequest != null)
				{
					alreadyDownloadBytes += (long)this.webRequest.Request.downloadedBytes;
				}
				//当前下载大小=已经下载的+当前下载中的
				//当前下载的大小/总的大小=下载进度
				return (int)(alreadyDownloadBytes * 100f / this.TotalSize);
			}
		}

		public async ETTask DownloadAsync()
		{
			if (this.bundles.Count == 0 && this.downloadingBundle == "")
			{
				this.TraceUpdate("DownloadAsync skip: no pending bundle");
				return;
			}

			try
			{
				this.TraceUpdate($"DownloadAsync begin pending={this.bundles.Count} totalSize={this.TotalSize}");
				while (true)
				{
					if (this.bundles.Count == 0)
					{
						break;
					}

                    //从队列 出列 获取到要下载的文件
					this.downloadingBundle = this.bundles.Dequeue();
					this.TraceUpdate($"Download bundle begin file={this.downloadingBundle} left={this.bundles.Count}");

					while (true)
					{
						try
						{
                            //UnityWebRequest进行下载请求
                            using (this.webRequest = ComponentFactory.Create<UnityWebRequestAsync>())
							{
                                //要下载的路径:GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + this.downloadingBundle
                                await this.webRequest.DownloadAsync(GlobalConfigComponent.Instance.GlobalProto.GetUrl() + "StreamingAssets/" + this.downloadingBundle);
								byte[] data = this.webRequest.Request.downloadHandler.data;

                                //要写入本地文件夹的路径
								string path = Path.Combine(PathHelper.AppHotfixResPath, this.downloadingBundle);
                                string folder = Path.GetDirectoryName(path);
                                if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                                {
                                    Directory.CreateDirectory(folder);
                                }
                                using (FileStream fs = new FileStream(path, FileMode.Create))
								{
									fs.Write(data, 0, data.Length);
								}
							}
						}
						catch (Exception e)
						{
							Log.Error($"download bundle error: {this.downloadingBundle}\n{e}");
							continue;
						}

						break;
					}
                    //每下载一个 就会添加到哈希表中
					this.downloadedBundles.Add(this.downloadingBundle);
					this.TraceUpdate($"Download bundle ok file={this.downloadingBundle} downloaded={this.downloadedBundles.Count}");
					this.downloadingBundle = "";
					this.webRequest = null;
				}

				Debug.LogWarning($"BundleDownloader download complete: files={this.downloadedBundles.Count} version={this.remoteVersionConfig?.Version ?? 0}");
				this.TraceUpdate($"Download complete files={this.downloadedBundles.Count} version={this.remoteVersionConfig?.Version ?? 0}");
				this.PersistHotfixVersionFile();
			}
			catch (Exception e)
			{
				this.TraceUpdate($"DownloadAsync exception {e.Message}");
				Log.Error(e);
			}
		}
	}
}
