
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using CustomFrameWork.Component;

namespace CustomFrameWork
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
			long time = TimeHelper.ClientNowSeconds();

			return (appId << 48) + (time << 16) + ++value;
		}

		public static long GenerateInstanceId()
		{
			return ++instanceIdGenerator;
		}

		public static int GetAppId(long v)
		{
			return (int)((long)((ulong)v >> 48));
		}


		private static ushort valueNew;

		private static long lastGenerateIdNewTime = TimeHelper.ClientNowSeconds();
		public static long GenerateIdNew()
		{
			// 毫秒时间戳
			long time = TimeHelper.ClientNowSeconds();
			if (lastGenerateIdNewTime != time)
			{
				lastGenerateIdNewTime = time;
				valueNew = 0;
			}

			return (appId << 48) + (time << 16) + ++valueNew;
		}
	}
}
