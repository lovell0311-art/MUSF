
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;

namespace CustomFrameWork
{
    public static class Help_ObjectToByteExpand
    {
        public static byte[] ToBytes(this ulong b_UInt64)
        {
            byte[] mResult = new byte[8];
            mResult[0] = (byte)((b_UInt64) & byte.MaxValue);
            mResult[1] = (byte)((b_UInt64 >> 8) & byte.MaxValue);
            mResult[2] = (byte)((b_UInt64 >> 16) & byte.MaxValue);
            mResult[3] = (byte)((b_UInt64 >> 24) & byte.MaxValue);
            mResult[4] = (byte)((b_UInt64 >> 32) & byte.MaxValue);
            mResult[5] = (byte)((b_UInt64 >> 40) & byte.MaxValue);
            mResult[6] = (byte)((b_UInt64 >> 48) & byte.MaxValue);
            mResult[7] = (byte)((b_UInt64 >> 56) & byte.MaxValue);
            return mResult;
            
        }
        public static byte[] ToBytes(this uint b_UInt32)
        {
            byte[] mResult = new byte[4];
            mResult[0] = (byte)((b_UInt32) & byte.MaxValue);
            mResult[1] = (byte)((b_UInt32 >> 8) & byte.MaxValue);
            mResult[2] = (byte)((b_UInt32 >> 16) & byte.MaxValue);
            mResult[3] = (byte)((b_UInt32 >> 24) & byte.MaxValue);
            return mResult;
          
        }
        public static byte[] ToBytes(this ushort b_UInt16)
        {
            byte[] mResult = new byte[2];
            mResult[0] = (byte)((b_UInt16) & byte.MaxValue);
            mResult[1] = (byte)((b_UInt16 >> 8) & byte.MaxValue);
            return mResult;
        }
        public static byte[] ToBytes(this long b_Int64)
        {
            return ((ulong)b_Int64).ToBytes();
        }
        public static byte[] ToBytes(this int b_Int32)
        {
            return ((uint)b_Int32).ToBytes();
        }
        public static byte[] ToBytes(this short b_Int16)
        {
            return ((ushort)b_Int16).ToBytes();
        }
        public static byte ToByte(this bool b_bool)
        {
            byte mResult = (byte)(b_bool ? 1 : 0);
            return mResult;
        }
        public static byte[] ToBytes(this string b_string, Encoding b_Encoding)
        {
            byte[] mResult = b_Encoding.GetBytes(b_string);
            return mResult;
        }
        public static byte[] ToBytes(this string b_string, int b_Length, Encoding b_Encoding)
        {
            byte[] mResult = new byte[b_Length];
            byte[] mDataBytes = b_Encoding.GetBytes(b_string);
            Array.Copy(mDataBytes, 0, mResult, 0, mDataBytes.Length);
            return mResult;
        }



        public static byte[] ToBytes(this float b_Float)
        {
            byte[] mResult = BitConverter.GetBytes(b_Float);
            return mResult;
        }
        public static byte[] ToBytes(this double b_Double)
        {
            byte[] mResult = BitConverter.GetBytes(b_Double);
            return mResult;
        }

        public static List<byte> ToBytes(this List<float> b_Floats)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_Floats.Count; i < len; i++)
            {
                mResult.AddRange(b_Floats[i].ToBytes());
            }
            return mResult;
        }
        public static List<byte> ToBytes(this List<double> b_Doubles)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_Doubles.Count; i < len; i++)
            {
                mResult.AddRange(b_Doubles[i].ToBytes());
            }
            return mResult;
        }
        public static List<byte> ToBytes(this List<long> b_Int64s)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_Int64s.Count; i < len; i++)
            {
                mResult.AddRange(b_Int64s[i].ToBytes());
            }
            return mResult;
        }
        public static List<byte> ToBytes(this List<ulong> b_UInt64s)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_UInt64s.Count; i < len; i++)
            {
                mResult.AddRange(b_UInt64s[i].ToBytes());
            }
            return mResult;
        }
        public static List<byte> ToBytes(this List<int> b_Int32s)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_Int32s.Count; i < len; i++)
            {
                mResult.AddRange(b_Int32s[i].ToBytes());
            }
            return mResult;
        }
        public static List<byte> ToBytes(this List<uint> b_UInt32s)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_UInt32s.Count; i < len; i++)
            {
                mResult.AddRange(b_UInt32s[i].ToBytes());
            }
            return mResult;
        }
        public static List<byte> ToBytes(this List<short> b_Int16s)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_Int16s.Count; i < len; i++)
            {
                mResult.AddRange(b_Int16s[i].ToBytes());
            }
            return mResult;
        }
        public static List<byte> ToBytes(this List<ushort> b_UInt16s)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_UInt16s.Count; i < len; i++)
            {
                mResult.AddRange(b_UInt16s[i].ToBytes());
            }
            return mResult;
        }
        public static List<byte> ToBytes(this List<bool> b_Bools)
        {
            List<byte> mResult = new List<byte>();
            for (int i = 0, len = b_Bools.Count; i < len; i++)
            {
                mResult.Add(b_Bools[i].ToByte());
            }
            return mResult;
        }
    }
}
