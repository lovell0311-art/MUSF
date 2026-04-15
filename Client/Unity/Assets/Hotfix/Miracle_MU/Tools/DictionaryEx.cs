using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 扩展方法解决foreach导致的GC (尽量不适应Lambda表达式 lambda表达式也会产生GC！)
    /// </summary>
    public static class DictionaryEx
    {
        /// <summary>
        /// 提供一个方法遍历所有项
        /// </summary>
        public static void Foreach<TKey, TValue>(this Dictionary<TKey, TValue> dic, Action<TKey, TValue> action, int maxCount = 1000)
        {
            if (action == null) return;
            var enumerator = dic.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext() && i++ < maxCount)
            {
                action(enumerator.Current.Key, enumerator.Current.Value);
            }
        }

        /// <summary>
        /// 提供一个方法遍历所有key值
        /// </summary>
        public static void ForeachKey<TKey, TValue>(this Dictionary<TKey, TValue> dic, Action<TKey> action, int maxCount = 1000)
        {
            if (action == null) return;
            var enumerator = dic.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext() && i++ < maxCount)
            {
                action(enumerator.Current.Key);
            }
        }

        /// <summary>
        /// 提供一个方法遍历所有value值
        /// </summary>
        public static void ForeachValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, Action<TValue> action, int maxCount = 1000)
        {
            if (action == null) return;
            var enumerator = dic.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext() && i++ < maxCount)
            {
                action(enumerator.Current.Value);
            }
        }

        
    }
}