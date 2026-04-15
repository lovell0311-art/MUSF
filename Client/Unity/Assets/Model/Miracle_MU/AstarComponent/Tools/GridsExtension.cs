
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    public static class GridsExtension
    {
        /// <summary>
        /// 添加元素（如果已经存在则不需要重复添加）
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        public static void AddUnique(this List<Vector2Int> self, Vector2Int other)
        {
            if (!self.Contains(other))
            { 
             self.Add(other);
            }
        }
        /// <summary>
        /// 添加格子
        /// </summary>
        /// <param name="self"></param>
        /// <param name="others"></param>

        public static void AddUnique(this List<Vector2Int> self,List<Vector2Int> others)
        {
            if (others == null) return;
            for (int i = 0, length=others.Count; i < length; i++)
            {
                var other = others[i];
                if (!self.Contains(other))
                {
                    self.Add(other);
                }
            }
        }
        /// <summary>
        /// 偏移
        /// </summary>
        /// <param name="self"></param>
        /// <param name="offset"></param>
        public static void Offset(this List<Vector2Int> self, Vector2Int offset)
        {
       
            for (int i = 0,length=self.Count; i < length; i++)
            {
                self[i]+=offset;
               
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        public static void Except(this List<Vector2Int> self,List<Vector2Int> other) 
        {
            if (other == null) return;
            for (int i = 0; i < other.Count; i++)
            {
                if (self.Contains(other[i]))
                {
                    self.Remove(other[i]);
                }
            }
        }
    }
}