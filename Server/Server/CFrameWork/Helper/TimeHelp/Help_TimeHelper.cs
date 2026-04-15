
using System;
using System.Collections.Generic;
using CustomFrameWork.Component;
using System.Text;
using Microsoft.VisualBasic;


namespace CustomFrameWork
{
    /// <summary>
    /// 时间工具类
    /// </summary>
    public class Help_TimeHelper
    {

        private static DateTime _1970DataTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        private static long _1970DataTimeTicks = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks;

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
        /// 获取时间戳 单位:毫秒
        /// </summary>
        /// <param name="b_TargetTick">目标时间时间</param>
        /// <returns></returns>
        public static long GetTargetTick(long b_TargetTick)
        {
            return (b_TargetTick - Epoch1970_Ticks) / 10000;
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

        public static long ConvertDateTimeToLong(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(_1970DataTime);
            long t = time.Ticks - startTime.Ticks;
            return t;
        }
        /// <summary>
        /// 日期转化时间秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long DateConversionTime(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(_1970DataTime);
            long t = (time.Ticks - startTime.Ticks) / 10000000;
            return t;
        }
        //获取当前时间戳
        public static long GetCurrenTimeStamp() //除以10000 就是毫秒
        {
            return DateTime.UtcNow.Ticks - _1970DataTimeTicks;
        }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(_1970DataTime);
            return dtStart.AddTicks(timeStamp);
        }

        /// <summary>
        /// 获取两个时间戳经过了多少秒
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public static int GetSecond(long startTime, long endTime)
        {
            return (int)((endTime - startTime) / 10000000);
        }



        public static long GetNow2020(long b_DifferenceTick = 0)
        {
            return (DateTime.UtcNow.Ticks - Epoch2020_Ticks + b_DifferenceTick) / 10000;
        }
        public static long GetNowSecond2020(long b_DifferenceTick = 0)
        {
            return (DateTime.UtcNow.Ticks - Epoch2020_Ticks + b_DifferenceTick) / 10000000;
        }
        public static DateTime GetDateTime2020(long b_Second)
        {
            return new DateTime(b_Second * 10000 + Epoch2020_Ticks);
        }
        public static DateTime GetDateTimeBySecond2020(long b_Second)
        {
            return new DateTime(b_Second * 10000000 + Epoch2020_Ticks);
        }
    }
}
