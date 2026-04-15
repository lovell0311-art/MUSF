
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CustomFrameWork
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EventSystemType
    {
        /// <summary>
        /// None 事件
        /// </summary>
        NONE,
        /// <summary>
        /// Init 事件 awake之后 Start之前
        /// </summary>
        INIT,
        /// <summary>
        /// Load 事件 热更代码
        /// </summary>
        LOAD,
        /// <summary>
        /// 主工程方法  Server 没有 hotfix 节点
        /// </summary>
        START,
        /// <summary>
        /// 主工程方法 Server 没有 hotfix 节点
        /// </summary>
        UPDATE,

        /// <summary>
        /// Dispose 事件 热更代码
        /// </summary>
        DISPOSE,
    }
    public partial class CustomEventSystem : IDisposable
    {
        private Queue<CustomComponent> mStartMethodQueue = new Queue<CustomComponent>();
        private Dictionary<long, CustomComponent> mStartMethodDic = new Dictionary<long, CustomComponent>();

        private CustomComponent[] mUpdateMethodArray = new CustomComponent[] { };
        private Dictionary<long, CustomComponent> mUpdateMethodDic = new Dictionary<long, CustomComponent>();
        private bool mIsDataChange = false;

        private Queue<long> mLoadMethodQueue = new Queue<long>();
        private Dictionary<long, CustomComponent> mLoadMethodDic = new Dictionary<long, CustomComponent>();
        /// <summary>
        /// 构造函数 初始化事件缓存
        /// </summary>
        public CustomEventSystem()
        {
            IsDisposeable = false;
            mIsDataChange = true;

            //LoadDllCode();
        }
        public void LoadDllCode()
        {
            AddEventSystem(typeof(Root).Assembly, true);
#if SERVER
            AddEventSystem(Root.HotfixAssembly);
#endif
        }

        public void Load()
        {
            foreach(long component in mLoadMethodDic.Keys)
            {
                mLoadMethodQueue.Enqueue(component);
            }
            while(mLoadMethodQueue.Count > 0)
            {
                long componentId = mLoadMethodQueue.Dequeue();
                if (!mLoadMethodDic.TryGetValue(componentId, out CustomComponent component)) continue;
                if (component.IsDisposeable) continue;

                OnRun(component, EventSystemType.LOAD);
            }
        }

        #region AddComponent args
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <param name="b_CustomComponent">组件</param>
        public void AddComponent<K>(K b_CustomComponent) where K : CustomComponent
        {
            OnRun(b_CustomComponent, EventSystemType.INIT);
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                int[] mResult = temp.Keys.ToArray();
                for (int i = 0, len = mResult.Length; i < len; i++)
                {
                    EventSystemType mEventSystemType = (EventSystemType)mResult[i];
                    switch (mEventSystemType)
                    {
                        case EventSystemType.NONE:
                        case EventSystemType.INIT:
                        case EventSystemType.DISPOSE:
                            break;
                        case EventSystemType.LOAD:
                            mLoadMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.START:
                            mStartMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.UPDATE:
                            mUpdateMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            mIsDataChange = true;
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{mEventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="b_CustomComponent"></param>
        /// <param name="b_Args"></param>
        public void AddComponent<K, T>(K b_CustomComponent, T b_Args) where K : CustomComponent
        {
            OnRun(b_CustomComponent, EventSystemType.INIT, b_Args);
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                int[] mResult = temp.Keys.ToArray();
                for (int i = 0, len = mResult.Length; i < len; i++)
                {
                    EventSystemType mEventSystemType = (EventSystemType)mResult[i];
                    switch (mEventSystemType)
                    {
                        case EventSystemType.NONE:
                        case EventSystemType.INIT:
                            break;
                        case EventSystemType.LOAD:
                            mLoadMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.START:
                            mStartMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.UPDATE:
                            mUpdateMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            mIsDataChange = true;
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{mEventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void AddComponent<K, T1, T2>(K b_CustomComponent, T1 b_Args1, T2 b_Args2) where K : CustomComponent
        {
            OnRun(b_CustomComponent, EventSystemType.INIT, b_Args1, b_Args2);
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                int[] mResult = temp.Keys.ToArray();
                for (int i = 0, len = mResult.Length; i < len; i++)
                {
                    EventSystemType mEventSystemType = (EventSystemType)mResult[i];
                    switch (mEventSystemType)
                    {
                        case EventSystemType.NONE:
                        case EventSystemType.INIT:
                            break;
                        case EventSystemType.LOAD:
                            mLoadMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.START:
                            mStartMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.UPDATE:
                            mUpdateMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            mIsDataChange = true;
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{mEventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void AddComponent<K, T1, T2, T3>(K b_CustomComponent, T1 b_Args1, T2 b_Args2, T3 b_Args3) where K : CustomComponent
        {
            OnRun(b_CustomComponent, EventSystemType.INIT, b_Args1, b_Args2, b_Args3);
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                int[] mResult = temp.Keys.ToArray();
                for (int i = 0, len = mResult.Length; i < len; i++)
                {
                    EventSystemType mEventSystemType = (EventSystemType)mResult[i];
                    switch (mEventSystemType)
                    {
                        case EventSystemType.NONE:
                        case EventSystemType.INIT:
                            break;
                        case EventSystemType.LOAD:
                            mLoadMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.START:
                            mStartMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.UPDATE:
                            mUpdateMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            mIsDataChange = true;
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{mEventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void AddComponent<K, T1, T2, T3, T4>(K b_CustomComponent, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4) where K : CustomComponent
        {
            OnRun(b_CustomComponent, EventSystemType.INIT, b_Args1, b_Args2, b_Args3, b_Args4);
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                int[] mResult = temp.Keys.ToArray();
                for (int i = 0, len = mResult.Length; i < len; i++)
                {
                    EventSystemType mEventSystemType = (EventSystemType)mResult[i];
                    switch (mEventSystemType)
                    {
                        case EventSystemType.NONE:
                        case EventSystemType.INIT:
                            break;
                        case EventSystemType.LOAD:
                            mLoadMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.START:
                            mStartMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.UPDATE:
                            mUpdateMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            mIsDataChange = true;
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{mEventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void AddComponent<K, T1, T2, T3, T4, T5>(K b_CustomComponent, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4, T5 b_Args5) where K : CustomComponent
        {
            OnRun(b_CustomComponent, EventSystemType.INIT, b_Args1, b_Args2, b_Args3, b_Args4, b_Args5);
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                int[] mResult = temp.Keys.ToArray();
                for (int i = 0, len = mResult.Length; i < len; i++)
                {
                    EventSystemType mEventSystemType = (EventSystemType)mResult[i];
                    switch (mEventSystemType)
                    {
                        case EventSystemType.NONE:
                        case EventSystemType.INIT:
                            break;
                        case EventSystemType.LOAD:
                            mLoadMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.START:
                            mStartMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            break;
                        case EventSystemType.UPDATE:
                            mUpdateMethodDic[b_CustomComponent.Id] = b_CustomComponent;
                            mIsDataChange = true;
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{mEventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        #endregion
        public void RemoveComponent(CustomComponent b_CustomComponent)
        {
            if (b_CustomComponent == null) return;
            if (mUpdateMethodDic.ContainsKey(b_CustomComponent.Id))
            {
                mUpdateMethodDic.Remove(b_CustomComponent.Id);
                mIsDataChange = true;
            }

            if (mStartMethodDic.ContainsKey(b_CustomComponent.Id))
            {
                mStartMethodDic.Remove(b_CustomComponent.Id);

                mStartMethodQueue = new Queue<CustomComponent>(mStartMethodDic.Values);
            }

            if (mLoadMethodDic.ContainsKey(b_CustomComponent.Id))
            {
                mLoadMethodDic.Remove(b_CustomComponent.Id);
            }
        }

        private bool IsDisposeable = false;
        public void Dispose()
        {
            if (IsDisposeable) return;
            mStartMethodQueue.Clear();
            mStartMethodDic.Clear();
            mUpdateMethodDic.Clear();
            mUpdateMethodArray = null;

            mSingleObjectMemoryDic.Clear();
            mAllEventTypeMethodDic.Clear();
            mAllEventNameMethodDic.Clear();
            IsDisposeable = true;
        }
    }
}
