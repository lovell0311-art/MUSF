
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using CustomFrameWork.Component;
using System.IO;
using System.Security.Cryptography;

namespace CustomFrameWork
{
	public static class MD5Helper
	{
		public static string FileMD5(string filePath)
		{
			byte[] retVal;
			using (FileStream file = new FileStream(filePath, FileMode.Open))
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				retVal = md5.ComputeHash(file);
			}
			return retVal.ToHex("x2");
		}

        public static string GetMD5Hash(string str)
        {
            //就是比string往后一直加要好的优化容器
            StringBuilder sb = new StringBuilder();
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                //将输入的字符串转换为字节数组并计算哈希
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                //x 为十六进制
                //2为 每次都是两位数
                //假设有两个数10和26 正常情况十六进制显示为0xA 0x1A 这样看起来不整齐 为了好看，可以指定"X2" 这些显示出来就是 0x0A 0x1A
                //遍历哈希数据的每一个字节
                //并将每一个字符串格式化为十六进制的字符串
                int length = data.Length;
                for (int i = 0; i < length; i++)
                {
                    sb.Append(data[i].ToString("X2"));
                }
            }
            return sb.ToString();
        }
    }
}
