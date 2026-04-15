using System;
using System.Collections.Generic;

namespace ETModel
{
    public class RangeKey : IComparable<RangeKey>
    {
        private int begin;
        private int end;

        public int Rate => end - begin + 1;

        public RangeKey(int v)
        {
            begin = v;
            end = v;
        }

        public RangeKey(int _begin, int _end)
        {
            begin = _begin;
            end = _end;
        }

        public void SetKey(int key)
        {
            begin = key;
            end = key;
        }

        public int CompareTo(RangeKey other)
        {
            if (begin > other.end) return 1;
            if (end < other.begin) return -1;

            if(begin <= other.end) return end >= other.begin ? 0 : -1;
            if(end >= other.begin) return begin <= other.end ? 0 : 1;
            return 0;
        }

        public override string ToString()
        {
            return $"[{begin},{end}]";
        }
    }


    /// <summary>
    /// 随机选择器
    /// </summary>
    public class RandomSelector<T>
    {
        private SortedDictionary<RangeKey,T> allItem;
        /// <summary>
        /// 总概率
        /// </summary>
        private int totalRate;

        private Random random = new Random();

        private RangeKey tmpKey = new RangeKey(0);

        public int Count => allItem.Count;
        public SortedDictionary<RangeKey,T>.KeyCollection Keys => allItem.Keys;
        public SortedDictionary<RangeKey, T>.ValueCollection Values => allItem.Values;

        public RandomSelector()
        {
            allItem = new SortedDictionary<RangeKey, T>();
            totalRate = 0;
        }

        public RandomSelector(RandomSelector<T> other)
        {
            allItem = new SortedDictionary<RangeKey, T>(other.allItem);
            totalRate = other.totalRate;
        }

        public void Clear()
        {
            allItem.Clear();
            totalRate = 0;
        }

        /// <summary>
        /// 将项目添加到随机池子中
        /// </summary>
        /// <param name="item">项目</param>
        /// <param name="rate">概率，值越大，被选中的概率越高</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Add(T item, int rate)
        {
            if (rate <= 0) return;
            var key = new RangeKey(totalRate, totalRate + rate - 1);
            allItem.Add(key, item);
            totalRate += rate;
        }

        public bool TryGetValue(out T item)
        {
            tmpKey.SetKey(GetRandKey());
            return allItem.TryGetValue(tmpKey, out item);
        }

        public bool TryGetValueAndRemove(out T item)
        {
            tmpKey.SetKey(GetRandKey());
            if(allItem.TryGetValue(tmpKey, out item))
            {
                allItem.Remove(tmpKey);
                var tmpItem = allItem;
                allItem = new SortedDictionary<RangeKey, T>();
                Clear();
                for(var iter = tmpItem.GetEnumerator(); iter.MoveNext(); )
                {
                    Add(iter.Current.Value,iter.Current.Key.Rate);
                }
                return true;
            }
            return false;
        }


        private int GetRandKey()
        {
            return random.Next(0, totalRate);
        }

    }
}
