
using System;

namespace ETModel
{
	public static class TimeHelper
    {
        //C#中时间的Ticks属性是一个很大的长整数，单位是 100 毫微秒。

        //1秒=1000毫秒；
        //1毫秒=1000微秒；
        //1微秒=1纳秒
        //而1毫秒 = 10000ticks；所以1ticks=100纳秒=0.1微秒
        //ticks这个属性值是指从0001年1月1日12：00:00开始到此时的以ticks为单位的时间，就是以ticks表示的时间的间隔数。
        //使用DateTime.Now.Ticks返回的是一个long型的数值。
        //而如果是UtcNow,就是格林治时间 从1970, 1, 1开始 
        private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        /// <summary>
        /// 1970年的utc
        /// </summary>
        private static readonly long Epoch1970_Ticks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        /// <summary>
        /// 2020年的utc
        /// </summary>
        private static readonly long Epoch2020_Ticks = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        /// <summary>
        /// 今年的Utc
        /// </summary>
        private static readonly long EpochThisYear_Ticks = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        /// <summary>
        /// 客户端时间
        /// </summary>
        /// <returns></returns>
        public static long ClientNow()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000;
		}

		public static long ClientNowSeconds()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000000;
		}

        /// <summary>
        /// 获得当前时间戳 
        /// </summary>
        /// <returns></returns>
		public static long Now()
		{
			return ClientNow();
		}

        /// <summary>
        /// 获取时间戳 单位:毫秒
        /// </summary>
        /// <param name="b_DifferenceTick">差异时间</param>
        /// <returns></returns>
        public static long GetNow(long b_DifferenceTick = 0)
        {
            return (DateTime.UtcNow.Ticks - Epoch1970_Ticks + b_DifferenceTick) / 10000;
        }
        /// <summary>
        /// 根据时间戳获取时间 单位:毫秒    GetNow 逆向
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDateTime(long b_Second)
        {
            return new DateTime(b_Second * 10000 + Epoch1970_Ticks);
        }
        /// <summary>
        /// 获取时间戳 单位:秒
        /// </summary>
        /// <param name="b_DifferenceTick">差异时间</param>
        /// <returns></returns>
        public static long GetNowSecond(long b_DifferenceTick = 0)
        {
            return (DateTime.UtcNow.Ticks - Epoch1970_Ticks + b_DifferenceTick) / 10000000;
        }
        /// <summary>
        /// 根据时间戳获取时间 单位:秒    GetNowSecond 逆向
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDateTimeBySecond(long b_Second)
        {
            return new DateTime(b_Second * 10000000 + Epoch1970_Ticks);
        }
        /// <summary>
        /// 根据时间戳获取时间 单位:毫秒
        /// </summary>
        /// <param name="Milliseconds">时间戳 毫秒</param>
        /// <returns></returns>
        public static DateTime GetDateTime_Milliseconds(long Milliseconds)
        {
            DateTime startTime =TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));//获取时间戳
            return startTime.AddMilliseconds(Milliseconds);
        }
        /// <summary>
        /// 根据时间戳获取时间 单位:秒
        /// </summary>
        /// <param name="Milliseconds">时间戳 秒</param>
        /// <returns></returns>
        public static DateTime GetDateTime_Seconds(long Seconds)
        {
            DateTime startTime =TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));//获取时间戳
            return startTime.AddSeconds(Seconds);
        }
        /// <summary>
        /// 更具时间戳 毫秒，获取间隔时间
        /// </summary>
        /// <param name="Milliseconds">时间戳 毫秒</param>
        /// <returns>间隔时间TimeSpan span 
        /// {span.Days}天{span.Hours}小时{span.Minutes}分钟{span.Seconds}秒</returns>
        public static TimeSpan GetSpacingTime_Milliseconds(long Milliseconds) 
        {
            //获取时间戳 毫秒
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(Milliseconds);
            DateTime curdateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            TimeSpan timeSpan = new TimeSpan(startTime.Ticks);
            TimeSpan CurtimeSpan = new TimeSpan(curdateTime.Ticks);
            return timeSpan.Subtract(CurtimeSpan).Duration();
        }
        /// <summary>
        /// 更具时间戳 秒，获取间隔时间
        /// </summary>
        /// <param name="Seconds">时间戳 秒</param>
        /// <returns>间隔时间TimeSpan span 
        /// {span.Days}天{span.Hours}小时{span.Minutes}分钟{span.Seconds}秒</returns>
        public static TimeSpan GetSpacingTime_Seconds(long Seconds) 
        {
            //获取时间戳 秒
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(Seconds);
            DateTime curdateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            TimeSpan timeSpan = new TimeSpan(startTime.Ticks);
            TimeSpan CurtimeSpan = new TimeSpan(curdateTime.Ticks);
            return timeSpan.Subtract(CurtimeSpan).Duration();
        }

        /// <summary>
        /// 获取当前时间  十位数
        /// </summary>
        /// <returns></returns>
        public static int GetTimestamp()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);//ToUniversalTime()转换为标准时区的时间,去掉的话直接就用北京时间
                                                                                    // return (long)ts.TotalMilliseconds; //精确到毫秒
            return (int)ts.TotalSeconds;//获取10位
        }
    }
}