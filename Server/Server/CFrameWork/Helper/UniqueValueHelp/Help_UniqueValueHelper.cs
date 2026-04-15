
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using CustomFrameWork.Component;

namespace CustomFrameWork
{
    public class Help_UniqueValueHelper
    {
        private static ulong _ServerId = 0;

        private static ulong mCurrentIndex = 0;

        public static void Init()
        {
            _ServerId = (ulong)OptionComponent.Options.AppId;
        }


        /// <summary>
        /// 注: 获取的数不为0
        /// </summary>
        /// <returns></returns>
        public static long GetUniqueValue()
        {
            if (mCurrentIndex > uint.MaxValue - 1) mCurrentIndex = 0;
            mCurrentIndex++;
            return (long)(_ServerId << 32 | mCurrentIndex);
        }
        public static long GetServerId(long b_UniqueValue)
        {
            return (long)((ulong)b_UniqueValue >> 32);
        }

        private static ulong IncrementValue = 0;
        private static long lastGenerateIdNewTime = Help_TimeHelper.GetNowSecond2020();
        public static long GetServerUniqueValue()
        {
            long mTick2020 = Help_TimeHelper.GetNowSecond2020();
            if (mTick2020 > lastGenerateIdNewTime)
            {
                lastGenerateIdNewTime = mTick2020;
                IncrementValue = 0;
            }

            IncrementValue++;
            if (IncrementValue > ushort.MaxValue - 1)
            {
                IncrementValue = 0;
                lastGenerateIdNewTime++;// 借用下一秒
            }

            return (long)((_ServerId << 48) | (IncrementValue << 32) | (ulong)lastGenerateIdNewTime);
        }
        public static long GetServerIdByServerUniqueValue(long b_UniqueValue)
        {
            return (long)((ulong)b_UniqueValue >> 48);
        }

        public static long GetUniqueValueByTime()
        {
            long mTick2020 = Help_TimeHelper.GetNowSecond2020();
            if (mTick2020 > lastGenerateIdNewTime)
            {
                lastGenerateIdNewTime = mTick2020;
                IncrementValue = 0;
            }

            IncrementValue++;
            if (IncrementValue > ushort.MaxValue - 1)
            {
                IncrementValue = 0;
                lastGenerateIdNewTime++;// 借用下一秒
            }

            return (long)((IncrementValue << 48) | (ulong)lastGenerateIdNewTime);
        }
    }
}
