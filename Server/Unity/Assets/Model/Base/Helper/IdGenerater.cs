using System;
using System.Threading;
using CustomFrameWork;

namespace ETModel
{
	public static class IdGenerater
	{
		private static long instanceIdGenerator;
		
		private static long appId;
		
		public static long AppId
		{
			set
			{
				appId = value;
				instanceIdGenerator = appId << 48;
			}
		}

		private static ushort value;

		public static long GenerateId()
		{
			long time = CustomFrameWork.TimeHelper.ClientNowSeconds();

			return (appId << 48) + (time << 16) + ++value;
		}
		
		public static long GenerateInstanceId()
		{
			if(Thread.CurrentThread.ManagedThreadId != 1)
            {
				Log.Error($"禁止在非主线程调用函数({Thread.CurrentThread.ManagedThreadId})");
            }
            return ++instanceIdGenerator;
		}

		public static int GetAppId(long v)
		{
			return (int)((long)((ulong)v >> 48));
		}


		private static ushort valueNew;

		private static long lastGenerateIdNewTime = CustomFrameWork.TimeHelper.ClientNowSeconds();
		public static long GenerateIdNew()
		{
			// 毫秒时间戳
			long time = CustomFrameWork.TimeHelper.ClientNowSeconds();
			if (lastGenerateIdNewTime != time)
			{
				lastGenerateIdNewTime = time;
				valueNew = 0;
			}

			return (appId << 48) + (time << 16) + ++valueNew;
		}
	}
}