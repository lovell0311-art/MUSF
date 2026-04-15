using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CustomFrameWork
{
    /// <summary>
    /// TripleDES 3重密钥加密
    /// 对称加密算法的优点是速度快，
    /// 缺点是密钥管理不方便，要求共享密钥。
    /// 可逆对称加密  密钥长度24
    /// </summary>
    public class Help_TripleDesEncryptionHelper
    {
        private static byte[] _rgbKey;
        private static byte[] _rgbIV;
        /// <summary>
        /// 设置Key IV
        /// </summary>
        /// <param name="b_Key"></param>
        /// <param name="b_IV"></param>
        /// <param name="b_Encoding"></param>
        public static void SetKeyAndIV(string b_Key, string b_IV, Encoding b_Encoding)
        {
            _rgbKey = b_Encoding.GetBytes(b_Key.Substring(0, 24));
            _rgbIV = b_Encoding.GetBytes(b_IV.Insert(1, "w").Substring(0, 8));
        }

        /// <summary>
        /// 对称加密算法
        /// </summary>
        /// <param name="b_Info"></param>
        /// <param name="b_Encoding"></param>
        /// <returns></returns>
        public static string Encrypt(string b_Info, Encoding b_Encoding)
        {
            using (TripleDES dsp = TripleDES.Create())
            {
                byte[] buffer = b_Encoding.GetBytes(b_Info);
                dsp.Mode = CipherMode.ECB;
                dsp.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = dsp.CreateEncryptor(_rgbKey, _rgbIV);
                byte[] mResult = cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
                return Convert.ToBase64String(mResult);
            }
        }
        public static string Decrypt(string b_Info, Encoding b_Encoding)
        {
            using (TripleDES dsp = TripleDES.Create())
            {
                byte[] buffer = Convert.FromBase64String(b_Info);
                dsp.Mode = CipherMode.ECB;
                dsp.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = dsp.CreateDecryptor(_rgbKey, _rgbIV);
                byte[] mResult = cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
                return b_Encoding.GetString(mResult);
            }
        }
        public static byte[] Encrypt(byte[] buffer)
        {
            using (TripleDES dsp = TripleDES.Create())
            {
                dsp.Mode = CipherMode.ECB;
                dsp.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = dsp.CreateEncryptor(_rgbKey, _rgbIV);
                byte[] mResult = cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
                return mResult;
            }
        }
        public static byte[] Decrypt(byte[] buffer)
        {
            using (TripleDES dsp = TripleDES.Create())
            {
                dsp.Mode = CipherMode.ECB;
                dsp.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = dsp.CreateDecryptor(_rgbKey, _rgbIV);
                byte[] mResult = cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
                return mResult;
            }
        }
    }
}
