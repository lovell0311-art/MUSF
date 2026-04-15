using System;
using System.Globalization;
using System.Text;

namespace CustomFrameWork
{
    public class Help_RandomHelper
    {
        private static readonly Random mRandom = new Random();

        public static UInt64 RangeUInt64()
        {
            byte[] bytes = new byte[8];
            mRandom.NextBytes(bytes);
            return bytes.ToUInt64();
        }

        public static Int64 RangeInt64()
        {
            byte[] bytes = new byte[8];
            mRandom.NextBytes(bytes);
            return bytes.ToInt64();
        }
        public static byte[] RangeByteArray(byte[] b_Bytes)
        {
            mRandom.NextBytes(b_Bytes);
            return b_Bytes;
        }
        public static string RangeString(int b_StringLength, Encoding b_Encoding)
        {
            string AllStr = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int MaxValue = AllStr.Length;
            StringBuilder mResult = new StringBuilder();
            for (int i = 0; i < b_StringLength; i++)
                mResult.Append(AllStr[Range(0, MaxValue)]);
            return mResult.ToString();
        }

        public static byte RangeByte()
        {
            byte[] bytes = new byte[1];
            mRandom.NextBytes(bytes);
            return bytes[0];
        }
        /// <summary>
        /// 获取lower与Upper之间的随机数
        /// </summary>
        /// <param name="b_Lower"></param>
        /// <param name="b_Upper"></param>
        /// <returns></returns>
        public static int Range(int b_Lower, int b_Upper)
        {
            if (b_Upper < b_Lower)
                throw new ArgumentException($"=>参数错误,b_Upper < b_Lower:{b_Upper} < {b_Lower}");

            int mResult = mRandom.Next(b_Lower, b_Upper);
            return mResult;
        }
        public static float Range(float b_Lower, float b_Upper, int b_PointLength = 2)
        {
            if (b_Upper < b_Lower)
                throw new ArgumentException($"=>参数错误,b_Upper < b_Lower:{b_Upper} < {b_Lower}");

            int Multiple = (int)Math.Pow(10, b_PointLength);
            float MultipleFloat = (float)1 / Multiple;
            int RandomInt = Range(0, ((int)((b_Upper - b_Lower) * Multiple)));

            float mResult = b_Lower + RandomInt * MultipleFloat;
            return mResult;
        }
        /// <summary>
        /// 在 大于等于b_Lower值 小于b_Upper值 之间的差值随机  差值不能大于int.MaxValue 否则无效
        /// </summary>
        /// <param name="b_Lower"></param>
        /// <param name="b_Upper"></param>
        /// <param name="b_PointLength"></param>
        /// <returns></returns>
        public static double Range(double b_Lower, double b_Upper, int b_PointLength = 2)
        {
            if (b_Upper < b_Lower)
                throw new ArgumentException($"=>参数错误,b_Upper < b_Lower:{b_Upper} < {b_Lower}");

            int Multiple = (int)Math.Pow(10, b_PointLength);
            double MultipleDouble = (double)1 / Multiple;
            int RandomInt = Range(0, (int)((b_Upper - b_Lower) * Multiple));

            double mResult = b_Lower + RandomInt * MultipleDouble;
            return mResult;
        }
    }
}