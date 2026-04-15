
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;

namespace CustomFrameWork
{
    public static class Help_ByteToObjectExpand
    {
        #region 没有异常检测 性能好 但可能崩程序 不安全代码unsafe 检测逻辑外置时使用
        public static long ToInt64(this byte[] b_Bytes)
        {
            return (long)b_Bytes.ToUInt64();
        }
        public static int ToInt32(this byte[] b_Bytes)
        {
            return (int)b_Bytes.ToUInt32();
        }
        public static short ToInt16(this byte[] b_Bytes)
        {
            return (short)b_Bytes.ToUInt16();
        }
        public static long ToInt64(this byte[] b_Bytes, int b_StartIndex)
        {
            return (long)b_Bytes.ToUInt64(b_StartIndex);
        }
        public static int ToInt32(this byte[] b_Bytes, int b_StartIndex)
        {
            return (int)b_Bytes.ToUInt32(b_StartIndex);
        }
        public static short ToInt16(this byte[] b_Bytes, int b_StartIndex)
        {
            return (short)b_Bytes.ToUInt16(b_StartIndex);
        }
        public static ulong ToUInt64(this byte[] b_Bytes)
        {
            byte[] temp = b_Bytes.GetRange(0, 8);
            ulong mResult = temp[0]
                        | ((ulong)temp[1] << 8)
                        | ((ulong)temp[2] << 16)
                        | ((ulong)temp[3] << 24)
                        | ((ulong)temp[4] << 32)
                        | ((ulong)temp[5] << 40)
                        | ((ulong)temp[6] << 48)
                        | ((ulong)temp[7] << 56);
            return mResult;
        }
        public static uint ToUInt32(this byte[] b_Bytes)
        {
            byte[] temp = b_Bytes.GetRange(0, 4);
            uint mResult = temp[0]
                       | ((uint)temp[1] << 8)
                       | ((uint)temp[2] << 16)
                       | ((uint)temp[3] << 24);
            return mResult;
        }
        public static ushort ToUInt16(this byte[] b_Bytes)
        {
            byte[] temp = b_Bytes.GetRange(0, 2);
            ushort mResult = (ushort)(temp[0] |
                                   ((uint)temp[1] << 8));
            return mResult;
        }

        public static ulong ToUInt64(this byte[] b_Bytes, int b_StartIndex)
        {
            byte[] temp = b_Bytes.GetRange(b_StartIndex, 8);
            ulong mResult = temp[0]
                        | ((ulong)temp[1] << 8)
                        | ((ulong)temp[2] << 16)
                        | ((ulong)temp[3] << 24)
                        | ((ulong)temp[4] << 32)
                        | ((ulong)temp[5] << 40)
                        | ((ulong)temp[6] << 48)
                        | ((ulong)temp[7] << 56);
            return mResult;
        }
        public static uint ToUInt32(this byte[] b_Bytes, int b_StartIndex)
        {
            byte[] temp = b_Bytes.GetRange(b_StartIndex, 4);
            uint mResult = temp[0]
                       | ((uint)temp[1] << 8)
                       | ((uint)temp[2] << 16)
                       | ((uint)temp[3] << 24);
            return mResult;
        }
        public static ushort ToUInt16(this byte[] b_Bytes, int b_StartIndex)
        {
            byte[] temp = b_Bytes.GetRange(b_StartIndex, 2);
            ushort mResult = (ushort)(temp[0]
                       | ((uint)temp[1] << 8));
            return mResult;
        }
        public static bool ToBool(this byte[] b_Bytes)
        {
            return b_Bytes.ToBool(0);
        }
        public static bool ToBool(this byte[] b_Bytes, int b_StartIndex)
        {
            bool mResult = (b_Bytes[b_StartIndex] == 1);
            return mResult;
        }
        #endregion


        public static ulong ToUInt64(this byte[] b_Bytes, ref bool b_DataError)
        {
            byte[] temp = b_Bytes.GetRange(0, 8);
            if (temp == null)
            {
                b_DataError = true;
                return 0;
            }
            ulong mResult = temp[0]
                  | ((ulong)temp[1] << 8)
                  | ((ulong)temp[2] << 16)
                  | ((ulong)temp[3] << 24)
                  | ((ulong)temp[4] << 32)
                  | ((ulong)temp[5] << 40)
                  | ((ulong)temp[6] << 48)
                  | ((ulong)temp[7] << 56);
            return mResult;
        }
        public static uint ToUInt32(this byte[] b_Bytes, ref bool b_DataError)
        {
            byte[] temp = b_Bytes.GetRange(0, 4);
            if (temp == null)
            {
                b_DataError = true;
                return 0;
            }
            uint mResult = temp[0]
                  | ((uint)temp[1] << 8)
                  | ((uint)temp[2] << 16)
                  | ((uint)temp[3] << 24);
            return mResult;
        }
        public static ushort ToUInt16(this byte[] b_Bytes, ref bool b_DataError)
        {
            byte[] temp = b_Bytes.GetRange(0, 2);
            if (temp == null)
            {
                b_DataError = true;
                return 0;
            }
            ushort mResult = (ushort)(temp[0]
                             | ((uint)temp[1] << 8));
            return mResult;
        }
        public static ulong ToUInt64(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            byte[] temp = b_Bytes.GetRange(b_StartIndex, 8);
            if (temp == null)
            {
                b_DataError = true;
                return 0;
            }
            ulong mResult = temp[0]
                  | ((ulong)temp[1] << 8)
                  | ((ulong)temp[2] << 16)
                  | ((ulong)temp[3] << 24)
                  | ((ulong)temp[4] << 32)
                  | ((ulong)temp[5] << 40)
                  | ((ulong)temp[6] << 48)
                  | ((ulong)temp[7] << 56);
            return mResult;
        }
        public static uint ToUInt32(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            byte[] temp = b_Bytes.GetRange(b_StartIndex, 4);
            if (temp == null)
            {
                b_DataError = true;
                return 0;
            }
            uint mResult = temp[0]
                  | ((uint)temp[1] << 8)
                  | ((uint)temp[2] << 16)
                  | ((uint)temp[3] << 24);
            return mResult;
        }
        public static ushort ToUInt16(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            byte[] temp = b_Bytes.GetRange(b_StartIndex, 2);
            if (temp == null)
            {
                b_DataError = true;
                return 0;
            }
            ushort mResult = (ushort)(temp[0]
                             | ((uint)temp[1] << 8));
            return mResult;
        }
        public static long ToInt64(this byte[] b_Bytes, ref bool b_DataError)
        {
            return (long)b_Bytes.ToUInt64(ref b_DataError);
        }
        public static int ToInt32(this byte[] b_Bytes, ref bool b_DataError)
        {
            return (int)b_Bytes.ToUInt32(ref b_DataError);
        }
        public static short ToInt16(this byte[] b_Bytes, ref bool b_DataError)
        {
            return (short)b_Bytes.ToUInt16(ref b_DataError);
        }

        public static long ToInt64(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            return (long)b_Bytes.ToUInt64(b_StartIndex, ref b_DataError);
        }
        public static int ToInt32(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            return (int)b_Bytes.ToUInt32(b_StartIndex, ref b_DataError);
        }
        public static short ToInt16(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            return (short)b_Bytes.ToUInt16(b_StartIndex, ref b_DataError);
        }
        public static bool ToBool(this byte[] b_Bytes, ref bool b_DataError)
        {
            return b_Bytes.ToBool(0, ref b_DataError);
        }
        public static bool ToBool(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            if (b_StartIndex > b_Bytes.Length)
            {
                b_DataError = true;
                return false;
            }
            bool mResult = (b_Bytes[b_StartIndex] == 1);
            return mResult;
        }

        public static string ToString(this byte[] b_Bytes, Encoding b_Encoding)
        {
            string mResult = b_Encoding.GetString(b_Bytes);
            return mResult;
        }
        public static string ToString(this byte[] b_Bytes, int b_StartIndex, Encoding b_Encoding)
        {
            string mResult = b_Bytes.ToString(b_StartIndex, b_Bytes.Length - b_StartIndex, b_Encoding);
            return mResult;
        }
        public static string ToString(this byte[] b_Bytes, int b_StartIndex, int b_Length, Encoding b_Encoding)
        {
            byte[] temp = b_Bytes.GetRange(b_StartIndex, b_Length);
            string mResult = temp.ToString(b_Encoding);
            return mResult;
        }

        public static byte[] GetRange(this byte[] b_Bytes, int b_StartIndex, int b_Length)
        {
            if (b_Bytes.Length < (b_StartIndex + b_Length)) return null;

            byte[] mResult = new byte[b_Length];
            for (int i = 0; i < b_Length; i++)
                mResult[i] = b_Bytes[b_StartIndex + i];
            return mResult;
        }




        public static float ToFloat(this byte[] b_Bytes, ref bool b_DataError)
        {
            float mResult = b_Bytes.ToFloat(0, ref b_DataError);
            return mResult;
        }
        public static float ToFloat(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            if (b_StartIndex + 4 > b_Bytes.Length)
            {
                b_DataError = true;
                return 0;
            }
            float mResult = BitConverter.ToSingle(b_Bytes, b_StartIndex);
            return mResult;
        }
        public static double ToDouble(this byte[] b_Bytes, ref bool b_DataError)
        {
            double mResult = b_Bytes.ToDouble(0, ref b_DataError);
            return mResult;
        }
        public static double ToDouble(this byte[] b_Bytes, int b_StartIndex, ref bool b_DataError)
        {
            if (b_StartIndex + 8 > b_Bytes.Length)
            {
                b_DataError = true;
                return 0;
            }
            double mResult = BitConverter.ToDouble(b_Bytes, b_StartIndex);
            return mResult;
        }
        public static List<float> ToFloats(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<float> mResult = new List<float>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToFloat(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 4;
            }
            return mResult;
        }
        public static List<double> ToDoubles(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<double> mResult = new List<double>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToDouble(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 8;
            }
            return mResult;
        }


        public static List<long> ToInt64s(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<long> mResult = new List<long>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToInt64(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 8;
            }
            return mResult;
        }
        public static List<ulong> ToUInt64s(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<ulong> mResult = new List<ulong>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToUInt64(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 8;
            }
            return mResult;
        }
        public static List<int> ToInt32s(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<int> mResult = new List<int>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToInt32(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 4;
            }
            return mResult;
        }
        public static List<uint> ToUInt32s(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<uint> mResult = new List<uint>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToUInt32(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 4;
            }
            return mResult;
        }
        public static List<short> ToInt16s(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<short> mResult = new List<short>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToInt16(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 2;
            }
            return mResult;
        }
        public static List<ushort> ToUInt16s(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<ushort> mResult = new List<ushort>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToUInt16(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 2;
            }
            return mResult;
        }
        public static List<byte> ToUInt8s(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<byte> mResult = new List<byte>();
            byte[] temp = b_Bytes.GetRange(b_Index, b_Length);
            if (temp == null) return null;
            mResult.AddRange(temp);
            b_Index += b_Length;
            return mResult;
        }
        public static List<sbyte> ToInt8s(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<sbyte> mResult = new List<sbyte>();
            byte[] temp = b_Bytes.GetRange(b_Index, b_Length);
            if (temp == null) return null;
            for (int i = 0, len = temp.Length; i < len; i++)
            {
                mResult.Add((sbyte)temp[i]);
            }
            b_Index += b_Length;
            return mResult;
        }
        public static List<bool> ToBools(this byte[] b_Bytes, ref int b_Index, int b_Length)
        {
            List<bool> mResult = new List<bool>();
            bool mDataError = false;
            for (int i = 0; i < b_Length; i++)
            {
                mResult.Add(b_Bytes.ToBool(b_Index, ref mDataError));
                if (mDataError) return null;
                b_Index += 1;
            }
            return mResult;
        }
    }
}
