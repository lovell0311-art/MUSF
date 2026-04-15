using System;
using System.Collections.Generic;

namespace ETModel
{
    public class ListComponent<T>: List<T>, IDisposable
    {
        public static ListComponent<T> Create()
        {
            return MonoPool<ListComponent<T>>.Instance.Fetch();
        }

        public void Dispose()
        {
            this.Clear();
            MonoPool<ListComponent<T>>.Instance.Recycle(this);
        }
    }
}