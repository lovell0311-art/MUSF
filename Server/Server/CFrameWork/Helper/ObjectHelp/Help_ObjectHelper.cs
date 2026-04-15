
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CustomFrameWork
{
    public class Help_ObjectHelper
    {
        /// <summary>
        /// 交换两个 参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="b_Args1">参数</param>
        /// <param name="b_Args2">参数</param>
        public static void Swap<T>(ref T b_Args1, ref T b_Args2)
        {
            T temp = b_Args1;
            b_Args1 = b_Args2;
            b_Args2 = temp;
        }

        /// <summary>
        /// 数据拷贝 被拷贝的数据类型需要可以序列化
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="b_Object">被拷贝的数据源</param>
        /// <param name="b_HasSerializableAttribute">是否标记了序列化特性 :Serializable</param>
        /// <returns>返回拷贝的数据</returns>
        public static T Clone<T>(T b_Object, bool b_HasSerializableAttribute) where T : class, ICloneable
        {
            if (b_HasSerializableAttribute)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, b_Object);
                    memoryStream.Position = 0;
                    return formatter.Deserialize(memoryStream) as T;
                }
            }
            else
            {
                return Help_JsonSerializeHelper.DeSerialize<T>(Help_JsonSerializeHelper.Serialize(b_Object));
            }
        }


    }
}
