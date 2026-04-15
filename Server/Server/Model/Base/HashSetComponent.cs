using System;
using System.Collections.Generic;

namespace ETModel
{
    public class HashSetComponent<T>: HashSet<T>, IDisposable
    {
        public static HashSetComponent<T> Create()
        {
            return MonoPool<HashSetComponent<T>>.Instance.Fetch();
        }

        public void Dispose()
        {
            this.Clear();
            MonoPool<HashSetComponent<T>>.Instance.Recycle(this);
        }
    }
}