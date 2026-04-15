using System;

namespace ETModel
{
    public class UnOrderMultiMapComponent<T,K>: UnOrderMultiMap<T,K>, IDisposable
    {
        public static UnOrderMultiMapComponent<T,K> Create()
        {
            return MonoPool<UnOrderMultiMapComponent<T, K>>.Instance.Fetch();
        }

        public void Dispose()
        {
            this.Clear();
            MonoPool<UnOrderMultiMapComponent<T, K>>.Instance.Recycle(this);
        }
    }
}