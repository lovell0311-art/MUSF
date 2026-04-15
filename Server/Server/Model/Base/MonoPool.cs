using System;
using System.Collections.Generic;

namespace ETModel
{
    public class MonoPool<T>: IDisposable where T : class
    {
        private readonly Queue<T> pool = new Queue<T>();

        public static MonoPool<T> Instance = new MonoPool<T>();
        
        private MonoPool()
        {
        }

        public T Fetch()
        {
            if (pool.Count == 0)
            {
                return Activator.CreateInstance(typeof(T)) as T;
            }
            return pool.Dequeue();
        }

        public void Recycle(T obj)
        {
            pool.Enqueue(obj);
        }

        public void Dispose()
        {
            this.pool.Clear();
        }
    }
}