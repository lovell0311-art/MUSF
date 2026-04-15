using System;
using System.Collections.Generic;
using global::Google.Protobuf.Collections;

namespace ETModel
{
    public class RepeatedFieldComponent<T>: RepeatedField<T>, IDisposable
    {
        public static RepeatedFieldComponent<T> Create()
        {
            return MonoPool<RepeatedFieldComponent<T>>.Instance.Fetch();
        }

        public void Dispose()
        {
            this.Clear();
            MonoPool<RepeatedFieldComponent<T>>.Instance.Recycle(this);
        }
    }
}