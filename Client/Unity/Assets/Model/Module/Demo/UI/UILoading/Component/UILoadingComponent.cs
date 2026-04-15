using System;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
	[ObjectSystem]
	public class UiLoadingComponentAwakeSystem : AwakeSystem<UILoadingComponent>
	{
		public override void Awake(UILoadingComponent self)
		{
			self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
			self.text = self.collector.GetText("info");
			//self.Handle = self.collector.GetText("Handle");
			self.progress = self.collector.GetGameObject("Slider").GetComponent<Slider>();
		}
	}

	[ObjectSystem]
	public class UiLoadingComponentStartSystem : StartSystem<UILoadingComponent>
	{
		public override void Start(UILoadingComponent self)
		{
			StartAsync(self).Coroutine();
		}
		
		public async ETVoid StartAsync(UILoadingComponent self)
		{
			TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();
			long instanceId = self.InstanceId;
			while (true)
			{
				//等待1000毫秒
				await timerComponent.WaitAsync(1000);

				if (self.InstanceId != instanceId)
				{
					return;
				}

				BundleDownloaderComponent bundleDownloaderComponent = Game.Scene.GetComponent<BundleDownloaderComponent>();
				if (bundleDownloaderComponent == null)
				{
					continue;
				}
				
				self.progress.value = bundleDownloaderComponent.Progress;
				//self.Handle.text = $"{bundleDownloaderComponent.Progress}%";

				self.text.text = $"正在为您检查/更新游戏资源...{self.TextFromBytesSize((bundleDownloaderComponent.TotalSize*bundleDownloaderComponent.Progress)/100)}/{self.TextFromBytesSize(bundleDownloaderComponent.TotalSize)}\t{bundleDownloaderComponent.Progress}%";
			}
		}
	}

	public class UILoadingComponent : Component
	{
		public ReferenceCollector collector;
		public Text text, Handle;
		public Slider progress;

		public string TextFromBytesSize(long size_num)
		{
			long data_size = size_num;
			if (data_size < 1024)
			{
				// B
				return data_size + "B";
			}
			if (data_size / 1024 < 1024)
			{
				// KB
				return String.Format("{0:F2}KB", data_size / 1024f);
			}
			data_size /= 1024;
			if (data_size / 1024 < 1024)
			{
				// MB
				return String.Format("{0:F2}MB", data_size / 1024f);
			}
			data_size /= 1024;
			// G
			return String.Format("{0:F2}G", data_size / 1024f);
		}

		public override void Dispose()
        {
			if (this.IsDisposed) return;
            base.Dispose();
			progress.value = 1;
			//Handle.text = string.Empty;
        }
    }
}
