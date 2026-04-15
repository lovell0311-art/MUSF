using System;
using System.Collections.Generic;

namespace ETModel
{
    public class DictionaryComponent<T,K>: Dictionary<T,K>, IDisposable
    {
        public static DictionaryComponent<T,K> Create()
        {
            return MonoPool<DictionaryComponent<T, K>>.Instance.Fetch();
        }

        public void Dispose()
        {
            this.Clear();
            MonoPool<DictionaryComponent<T, K>>.Instance.Recycle(this);
        }
    }
}