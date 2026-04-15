
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CustomFrameWork
{
    /// <summary>
    /// 复杂 Parse 工具类
    /// </summary>
    public class Help_ParseDictionaryHelper
    {
        /// <summary>
        /// Parse :TValueC 继承 TValueP 可用
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValueC"></typeparam>
        /// <typeparam name="TValueP"></typeparam>
        /// <param name="b_Object"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValueP> Parse<TKey, TValueC, TValueP>(Dictionary<TKey, TValueC> b_Object) where TValueC : TValueP
        {
            if (b_Object.Count <= 0)
            {
                return null;
            }
            Dictionary<TKey, TValueP> mResult = new Dictionary<TKey, TValueP>();
            using (var mEnumerator = b_Object.GetEnumerator())
            {
                while (mEnumerator.MoveNext())
                {
                    mResult[mEnumerator.Current.Key] = mEnumerator.Current.Value;
                }
            }
            return mResult;
        }


    }
}
