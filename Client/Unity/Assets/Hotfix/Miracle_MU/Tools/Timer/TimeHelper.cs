
using System;

namespace ETHotfix
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
            if(timeSpan.Subtract(CurtimeSpan).Ticks < 0)
            {
                return new TimeSpan();
            }
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

        /// <summary>
        /// 拿服务器的时间与当前做判断
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public static bool CompareTimestamps(long times)
        {
            // 获取当前UTC时间
            DateTime utcNow = DateTime.UtcNow;

            // 转换为Unix时间戳（以秒为单位）
            return times > (utcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        /// <summary>
        /// 拿服务器时间与当前时间做比较
        /// </summary>
        /// <param name="givenTimestamp"></param>
        /// <returns></returns>
        public static bool CompareTimestamps10(long givenTimestamp)
        {
            // 获取当前时间
            DateTime currentTime = DateTime.UtcNow; // 使用UTC时间避免时区差异影响

            // 将当前时间转换为10位时间戳（秒）
            long currentTimestamp = (long)(currentTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            // 比较给定的时间戳与当前时间戳
            if (givenTimestamp > currentTimestamp)
            {

                Log.DebugBrown("给定的时间戳在当前时间之后");
                return true;
            }
            else if (givenTimestamp < currentTimestamp)
            {
                Log.DebugBrown("给定的时间戳在当前时间之前");

                return false;
            }
            else
            {
                Log.DebugBrown("给定的时间戳与当前时间相同");
                return true;
            }
        }
        /// <summary>
        /// 将10位时间戳转换日期
        /// </summary>
        /// <param name="givenTimestamp"></param>
        /// <returns></returns>
        public static string GetTime(long givenTimestamp)
        {
            System.DateTime starttime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            System.DateTime dt = starttime.AddSeconds(givenTimestamp);
            string t = dt.ToString("yyyy/MM/dd HH:mm:ss");
            Log.DebugBrown("时间" + t);
            return t;
        }

        /// <summary>
        /// 给定一个int转换成时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ConvertSecondsToTime(int time)
        {
            int days = 0,hours =0, minutes = 0,seconds = 0;
            days = time / (24 * 3600);
            time %= (24 * 3600);
            hours = time / 3600;
            time %= 3600;
            minutes = time / 60;
            seconds = time % 60;
            return $"{days}天 {hours}小时 {minutes}分钟 {seconds}秒";

        }

        /// <summary>
        /// 给定10位的时间戳与本地时间算出时间差
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string DifferenceSecondsToTime(long time)
        {
            // 假设你有这个10位的时间戳
            long timestampInSeconds = time; // 这个时间戳对应于2023-01-01T00:00:00Z

            // 将时间戳转换为DateTime
            DateTime utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestampInSeconds);

            // 转换为本地时间
            DateTime localDateTime = utcDateTime.ToLocalTime();

            // 获取当前本地时间
            DateTime nowLocalDateTime = DateTime.Now;

            // 计算时间差
            TimeSpan difference = nowLocalDateTime - localDateTime;
            return difference.Days + "天" + difference.Hours + "时" + difference.Minutes + "分" + difference.Seconds + "秒";

        }
    }
}