#define NeedCaChe

using System;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using System.Collections.Generic;

namespace CustomFrameWork
{
    /// <summary>
    /// 数据对象根类
    /// 本CS文件外的代码不可直接继承此类 
    /// 可继承子类:ADataContext....
    /// </summary>
    public abstract class ADataContextSource : IDisposable
    {
        /// <summary>
        /// 对象池最大数量 超过就不会返回到对象池了
        /// </summary>
        protected virtual int PoolCountMax { get; } = 5000;

        /// <summary>
        /// 唯一id
        /// <para>使用对象池，需要有唯一id来区分是不是从对象池中取出的新对象</para>
        /// <para>只要是从对象池中取出来的对象，这个id都不相同</para>
        /// </summary>
        public long Id { get; private set; }
        /// <summary>
        /// 本文件自用方法 外面不要调用 请使用可继承子类中的ContextAwake
        /// </summary>
        public void DataContextSourceAwake() { IsDisposeable = false; }

        /// <summary>
        /// 重置唯一id
        /// </summary>
        public void ResetUniqueId()
        {
#if SERVER
            if (Id == 0) Id = Help_UniqueValueHelper.GetServerUniqueValue();
#else
            if (Id == 0) Id = Help_UniqueValueHelper.GetUniqueValue();
#endif
        }
        /// <summary>
        /// 当前数据是否清理了
        /// true:清理了 false:没有清理
        /// </summary>
        public bool IsDisposeable { get; private set; } = false;
        public virtual void Dispose()
        {
            if (IsDisposeable)
            {
#if !SERVER
                LogToolComponent.LogError($"{this.GetType().Name} IsDisposeable 提前释放");
#else
                LogToolComponent.Error($"{this.GetType().Name} IsDisposeable 提前释放");
#endif
                return;
            }
            IsDisposeable = true;
            Id = 0;

            if (PoolCountMax <= 0) return;

            bool mResult = Root.CreateBuilder.AddInstance(this, PoolCountMax);
            if (mResult)
            {
                GC.SuppressFinalize(this);
            }
        }
    }

    public class CustomCreateBuilder : IDisposable
    {
        public CustomCreateBuilder()
        {
#if NeedCaChe
            mKeyValuePairs = new Dictionary<Type, Queue<ADataContextSource>>();
            mADataContextInspects = new HashSet<Type>();
#endif
            IsDisposable = false;
        }

#if NeedCaChe
        private readonly Dictionary<Type, Queue<ADataContextSource>> mKeyValuePairs;
        private readonly HashSet<Type> mADataContextInspects;
#endif

        public bool AddInstance<K>(K b_t, int b_PoolCountMax = 1000) where K : ADataContextSource
        {
#if NeedCaChe
            Type type = b_t.GetType();
            if (mADataContextInspects.Contains(type))
            {
                Queue<ADataContextSource> mQueue = mKeyValuePairs[type];
                // 对象池中数量少于目标数 才可以回收
                if (mQueue.Count < b_PoolCountMax)
                {
                    mQueue.Enqueue(b_t);
                    return true;
                }
            }
            else
            {
                throw new Exception($"=>Type 类型不存在 :{type}");
            }
#endif
            return false;
        }

        bool IsDisposable = false;
        public void Dispose()
        {
            if (IsDisposable) return;
#if NeedCaChe
            mADataContextInspects.Clear();
            mKeyValuePairs.Clear();
#endif
            IsDisposable = true;
        }

        public void Clear()
        {
#if NeedCaChe
            foreach (var value in mKeyValuePairs.Values)
            {
                value.Clear();
            }
#endif
        }

        public K GetInstance<K>(bool b_RunContextAwake = true) where K : ADataContext
        {
            K mResult;
#if NeedCaChe
            Type type = typeof(K);
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(type))
            {
                mQueue = mKeyValuePairs[type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[type] = mQueue;
                mADataContextInspects.Add(type);
            }
            if (mQueue.Count > 0)
                mResult = mQueue.Dequeue() as K;
            else
#endif
                mResult = Activator.CreateInstance<K>();
            mResult.ResetUniqueId();
            if (b_RunContextAwake)
            {
                mResult.ContextAwake();
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T>(T args, bool b_RunContextAwake = true) where K : ADataContext<T>
        {
            K mResult;
#if NeedCaChe
            Type type = typeof(K);
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(type))
            {
                mQueue = mKeyValuePairs[type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[type] = mQueue;
                mADataContextInspects.Add(type);
            }
            if (mQueue.Count > 0)
                mResult = mQueue.Dequeue() as K;
            else
#endif
                mResult = Activator.CreateInstance<K>();
            mResult.ResetUniqueId();
            if (b_RunContextAwake)
            {
                mResult.ContextAwake(args);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T1, T2>(T1 b_args1, T2 b_args2, bool b_RunContextAwake = true) where K : ADataContext<T1, T2>
        {
            K mResult;
#if NeedCaChe
            Type type = typeof(K);
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(type))
            {
                mQueue = mKeyValuePairs[type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[type] = mQueue;
                mADataContextInspects.Add(type);
            }
            if (mQueue.Count > 0)
                mResult = mQueue.Dequeue() as K;
            else
#endif
                mResult = Activator.CreateInstance<K>();
            mResult.ResetUniqueId();
            if (b_RunContextAwake)
            {
                mResult.ContextAwake(b_args1, b_args2);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T1, T2, T3>(T1 b_args1, T2 b_args2, T3 b_args3, bool b_RunContextAwake = true) where K : ADataContext<T1, T2, T3>
        {
            K mResult;
#if NeedCaChe
            Type type = typeof(K);
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(type))
            {
                mQueue = mKeyValuePairs[type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[type] = mQueue;
                mADataContextInspects.Add(type);
            }
            if (mQueue.Count > 0)
                mResult = mQueue.Dequeue() as K;
            else
#endif
                mResult = Activator.CreateInstance<K>();
            mResult.ResetUniqueId();
            if (b_RunContextAwake)
            {
                mResult.ContextAwake(b_args1, b_args2, b_args3);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T1, T2, T3, T4>(T1 b_args1, T2 b_args2, T3 b_args3, T4 b_args4, bool b_RunContextAwake = true) where K : ADataContext<T1, T2, T3, T4>
        {
            K mResult;
#if NeedCaChe
            Type type = typeof(K);
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(type))
            {
                mQueue = mKeyValuePairs[type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[type] = mQueue;
                mADataContextInspects.Add(type);
            }
            if (mQueue.Count > 0)
                mResult = mQueue.Dequeue() as K;
            else
#endif
                mResult = Activator.CreateInstance<K>();
            mResult.ResetUniqueId();
            if (b_RunContextAwake)
            {
                mResult.ContextAwake(b_args1, b_args2, b_args3, b_args4);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T1, T2, T3, T4, T5>(T1 b_args1, T2 b_args2, T3 b_args3, T4 b_args4, T5 b_args5, bool b_RunContextAwake = true) where K : ADataContext<T1, T2, T3, T4, T5>
        {
            K mResult;
#if NeedCaChe
            Type type = typeof(K);
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(type))
            {
                mQueue = mKeyValuePairs[type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[type] = mQueue;
                mADataContextInspects.Add(type);
            }
            if (mQueue.Count > 0)
                mResult = mQueue.Dequeue() as K;
            else
#endif
                mResult = Activator.CreateInstance<K>();
            mResult.ResetUniqueId();
            if (b_RunContextAwake)
            {
                mResult.ContextAwake(b_args1, b_args2, b_args3, b_args4, b_args5);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }



        public K GetInstance<K>(Type b_Type, bool b_RunContextAwake = true) where K : ADataContext
        {
            K mResult;
#if NeedCaChe
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(b_Type))
            {
                mQueue = mKeyValuePairs[b_Type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[b_Type] = mQueue;
                mADataContextInspects.Add(b_Type);
            }
            ADataContextSource mTempObject = null;
            if (mQueue.Count > 0)
                mTempObject = mQueue.Dequeue();
            else
#endif
                mTempObject = (ADataContextSource)Activator.CreateInstance(b_Type);

            mResult = mTempObject as K;
            if (mResult != null) mResult.ResetUniqueId();
            if (mResult == null) mTempObject.Dispose();
            else if (b_RunContextAwake)
            {
                mResult.ContextAwake();
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T>(Type b_Type, T args, bool b_RunContextAwake = true) where K : ADataContext<T>
        {
            K mResult;
#if NeedCaChe
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(b_Type))
            {
                mQueue = mKeyValuePairs[b_Type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[b_Type] = mQueue;
                mADataContextInspects.Add(b_Type);
            }
            ADataContextSource mTempObject = null;
            if (mQueue.Count > 0)
                mTempObject = mQueue.Dequeue();
            else
#endif
                mTempObject = (ADataContextSource)Activator.CreateInstance(b_Type);

            mResult = mTempObject as K;
            if (mResult != null) mResult.ResetUniqueId();
            if (mResult == null) mTempObject.Dispose();
            else if (b_RunContextAwake)
            {
                mResult.ContextAwake(args);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T1, T2>(Type b_Type, T1 b_args1, T2 b_args2, bool b_RunContextAwake = true) where K : ADataContext<T1, T2>
        {
            K mResult;
#if NeedCaChe
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(b_Type))
            {
                mQueue = mKeyValuePairs[b_Type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[b_Type] = mQueue;
                mADataContextInspects.Add(b_Type);
            }
            ADataContextSource mTempObject = null;
            if (mQueue.Count > 0)
                mTempObject = mQueue.Dequeue();
            else
#endif
                mTempObject = (ADataContextSource)Activator.CreateInstance(b_Type);

            mResult = mTempObject as K;
            if (mResult != null) mResult.ResetUniqueId();
            if (mResult == null) mTempObject.Dispose();
            else if (b_RunContextAwake)
            {
                mResult.ContextAwake(b_args1, b_args2);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T1, T2, T3>(Type b_Type, T1 b_args1, T2 b_args2, T3 b_args3, bool b_RunContextAwake = true) where K : ADataContext<T1, T2, T3>
        {
            K mResult;
#if NeedCaChe
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(b_Type))
            {
                mQueue = mKeyValuePairs[b_Type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[b_Type] = mQueue;
                mADataContextInspects.Add(b_Type);
            }
            ADataContextSource mTempObject = null;
            if (mQueue.Count > 0)
                mTempObject = mQueue.Dequeue();
            else
#endif
                mTempObject = (ADataContextSource)Activator.CreateInstance(b_Type);

            mResult = mTempObject as K;
            if (mResult != null) mResult.ResetUniqueId();
            if (mResult == null) mTempObject.Dispose();
            else if (b_RunContextAwake)
            {
                mResult.ContextAwake(b_args1, b_args2, b_args3);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T1, T2, T3, T4>(Type b_Type, T1 b_args1, T2 b_args2, T3 b_args3, T4 b_args4, bool b_RunContextAwake = true) where K : ADataContext<T1, T2, T3, T4>
        {
            K mResult;
#if NeedCaChe
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(b_Type))
            {
                mQueue = mKeyValuePairs[b_Type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[b_Type] = mQueue;
                mADataContextInspects.Add(b_Type);
            }
            ADataContextSource mTempObject = null;
            if (mQueue.Count > 0)
                mTempObject = mQueue.Dequeue();
            else
#endif
                mTempObject = (ADataContextSource)Activator.CreateInstance(b_Type);

            mResult = mTempObject as K;
            if (mResult != null) mResult.ResetUniqueId();
            if (mResult == null) mTempObject.Dispose();
            else if (b_RunContextAwake)
            {
                mResult.ContextAwake(b_args1, b_args2, b_args3, b_args4);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
        public K GetInstance<K, T1, T2, T3, T4, T5>(Type b_Type, T1 b_args1, T2 b_args2, T3 b_args3, T4 b_args4, T5 b_args5, bool b_RunContextAwake = true) where K : ADataContext<T1, T2, T3, T4, T5>
        {
            K mResult;
#if NeedCaChe
            Queue<ADataContextSource> mQueue;
            if (mADataContextInspects.Contains(b_Type))
            {
                mQueue = mKeyValuePairs[b_Type];
            }
            else
            {
                mQueue = new Queue<ADataContextSource>();
                mKeyValuePairs[b_Type] = mQueue;
                mADataContextInspects.Add(b_Type);
            }
            ADataContextSource mTempObject = null;
            if (mQueue.Count > 0)
                mTempObject = mQueue.Dequeue();
            else
#endif
                mTempObject = (ADataContextSource)Activator.CreateInstance(b_Type);

            mResult = mTempObject as K;
            if (mResult != null) mResult.ResetUniqueId();
            if (mResult == null) mTempObject.Dispose();
            else if (b_RunContextAwake)
            {
                mResult.ContextAwake(b_args1, b_args2, b_args3, b_args4, b_args5);
                mResult.DataContextSourceAwake();
            }
            return mResult;
        }
    }


    public abstract class ADataContext : ADataContextSource
    {
        public virtual void ContextAwake() { }
    }
    public abstract class ADataContext<T> : ADataContextSource
    {
        public abstract void ContextAwake(T b_Args);
    }
    public abstract class ADataContext<T1, T2> : ADataContextSource
    {
        public abstract void ContextAwake(T1 b_Args, T2 b_args2);
    }
    public abstract class ADataContext<T1, T2, T3> : ADataContextSource
    {
        public abstract void ContextAwake(T1 b_Args, T2 b_args2, T3 b_args3);
    }
    public abstract class ADataContext<T1, T2, T3, T4> : ADataContextSource
    {
        public abstract void ContextAwake(T1 b_Args, T2 b_args2, T3 b_args3, T4 b_args4);
    }
    public abstract class ADataContext<T1, T2, T3, T4, T5> : ADataContextSource
    {
        public abstract void ContextAwake(T1 b_Args, T2 b_args2, T3 b_args3, T4 b_args4, T5 b_args5);
    }
}

