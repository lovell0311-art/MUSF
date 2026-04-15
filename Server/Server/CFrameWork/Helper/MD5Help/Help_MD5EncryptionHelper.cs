using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork.Component;

namespace CustomFrameWork
{
    public class Help_MD5EncryptionHelper
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="b_Info">需要加密的字符串</param>
        /// <param name="b_Encode">字符的编码</param>
        /// <param name="b_HashLength">字符的编码32位还是16位</param>
        /// <returns>MD5</returns>
        public static string MD5EncryptionGetString(string b_Info, Encoding b_Encode, int b_HashLength = 32)
        {
            byte[] buffer = b_Encode.GetBytes(b_Info);
            return MD5EncryptionGetString(buffer, b_HashLength);
        }
        /// <summary>
        /// MD5文件读取加密 无文件锁
        /// </summary>
        /// <param name="b_FilePath">路径</param>
        /// <param name="b_HashLength">字符的编码32位还是16位</param>
        /// <returns>MD5</returns>
        public static async Task<string> MD5EncryptionGetString(string b_FilePath, int b_HashLength = 32)
        {
            byte[] buffer = await File.ReadAllBytesAsync(b_FilePath);
            return MD5EncryptionGetString(buffer, b_HashLength);
        }
        public static string MD5EncryptionGetString(byte[] b_Buffer, int b_HashLength = 32)
        {
            if (b_Buffer == null || b_Buffer.Length == 0) return null;
            using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())
            {
                byte[] data = md5Hasher.ComputeHash(b_Buffer);
                if (b_HashLength == 32)
                    return BitConverter.ToString(data).Replace("-", "");
                else
                    return BitConverter.ToString(data, 4, 8).Replace("-", "");
            }
        }
        public static byte[] MD5EncryptionGetBytes(byte[] b_Buffer, int b_HashLength = 32)
        {
            if (b_Buffer == null || b_Buffer.Length == 0) return null;
            using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())
            {
                byte[] data = md5Hasher.ComputeHash(b_Buffer);
                if (b_HashLength == 32)
                    return data;
                else
                {
                    byte[] GetRange(byte[] b_Bytes, int b_StartIndex, int b_Length)
                    {
                        if (b_Bytes.Length < (b_StartIndex + b_Length)) return null;

                        byte[] mResult = new byte[b_Length];
                        for (int i = 0; i < b_Length; i++)
                            mResult[i] = b_Bytes[b_StartIndex + i];
                        return mResult;
                    }

                    return GetRange(data, 4, 8);
                }
            }
        }
    }
}
