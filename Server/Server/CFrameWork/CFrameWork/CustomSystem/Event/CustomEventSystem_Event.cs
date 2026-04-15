
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Linq;

namespace CustomFrameWork
{
    public partial class CustomEventSystem
    {
        private readonly Dictionary<Type, object> mSingleObjectMemoryDic = new Dictionary<Type, object>();
        /// <summary>
        /// 注册类型对象
        /// </summary>
        /// <param name="b_RegisterType">目标对象</param>
        /// <param name="b_IsOverride">是否重写对象</param>
        private void Register(Type b_RegisterType, bool b_IsOverride = false)
        {
            if (b_IsOverride == false)
            {
                if (mSingleObjectMemoryDic.ContainsKey(b_RegisterType))
                {
                    return;
                }
            }
            mSingleObjectMemoryDic[b_RegisterType] = null;
        }
        /// <summary>
        /// 取出值对象
        /// </summary>
        /// <param name="b_RegisterType">键</param>
        /// <returns>值</returns>
        private object Resolve(Type b_RegisterType)
        {
            if (mSingleObjectMemoryDic.TryGetValue(b_RegisterType, out object mResult))
            {
                if (mResult == null)
                {
                    mResult = mSingleObjectMemoryDic[b_RegisterType] = Activator.CreateInstance(b_RegisterType);
                }
                return mResult;
            }
            return null;
        }

        /// <summary>
        /// 全部的事件方法
        /// </summary>
        private readonly Dictionary<Type, Dictionary<int, Type>> mAllEventTypeMethodDic = new Dictionary<Type, Dictionary<int, Type>>();
        private readonly Dictionary<string, List<Type>> mAllEventNameMethodDic = new Dictionary<string, List<Type>>();
        /// <summary>
        /// 添加dll库里的事件方法
        /// </summary>
        /// <param name="b_LoadAssembly">库</param>
        /// <param name="b_IsOverride">是否重写数据 true将清理事件缓存</param>
        public void AddEventSystem(Assembly b_LoadAssembly, bool b_IsOverride = false)
        {
            if (b_IsOverride)
            {
                mSingleObjectMemoryDic.Clear();
                mAllEventTypeMethodDic.Clear();
                mAllEventNameMethodDic.Clear();
            }
            Type[] types = b_LoadAssembly?.GetTypes();
            AddEventSystem(types);
        }
        public void AddEventSystem(Type[] b_LoadTypes, bool b_IsOverride = false)
        {
            if (b_IsOverride)
            {
                mSingleObjectMemoryDic.Clear();
                mAllEventTypeMethodDic.Clear();
                mAllEventNameMethodDic.Clear();
            }
            for (int i = 0, len = b_LoadTypes.Length; i < len; i++)
            {
                Type type = b_LoadTypes[i];

                if (type.IsDefined(typeof(EventMethodAttribute), false))
                {
                    object[] mAttributes = type.GetCustomAttributes(typeof(EventMethodAttribute), false);

                    //注册类型 指向同一对象
                    Register(type);
                    for (int j = 0, jlen = mAttributes.Length; j < jlen; j++)
                    {
                        EventMethodAttribute mEventMethod = mAttributes[j] as EventMethodAttribute;
                        if (mEventMethod.IsByEventType)
                        {
                            if (!mAllEventTypeMethodDic.TryGetValue(mEventMethod.Type, out Dictionary<int, Type> mResult))
                                mResult = mAllEventTypeMethodDic[mEventMethod.Type] = new Dictionary<int, Type>();
                            //将同一对象的事件指向同一对象类型 取出同一对象
                            if (mResult.ContainsKey((int)mEventMethod.EventSystemType))
                            {
                                MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                                throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:已经存在对象的事件,{mEventMethod.Type}-{mEventMethod.EventSystemType} 注册失败\n{Environment.StackTrace}\n\n");
                            }
                            mResult[(int)mEventMethod.EventSystemType] = type;
                        }
                        else
                        {
                            if (mAllEventNameMethodDic.TryGetValue(mEventMethod.EventName, out var mEventTypeList) == false)
                            {
                                mEventTypeList = mAllEventNameMethodDic[mEventMethod.EventName] = new List<Type>();
                            }
                            if (mEventTypeList.Contains(type) == false)
                            {
                                mEventTypeList.Add(type);
                            }
                        }
                    }
                }
            }
        }



        public void OnRun<K>(K b_CustomComponent, EventSystemType b_EventSystemType) where K : ACustomComponent
        {
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                if (temp.TryGetValue((int)b_EventSystemType, out Type mResult))
                {
                    switch (b_EventSystemType)
                    {
                        case EventSystemType.INIT:
                            ((ITEventMethodOnInit<K>)Resolve(mResult)).OnInit(b_CustomComponent);
                            break;
                        case EventSystemType.LOAD:
                            ((ITEventMethodOnLoad)Resolve(mResult)).OnLoad(b_CustomComponent);
                            break;
                        case EventSystemType.DISPOSE:
                            ((ITEventMethodOnDispose)Resolve(mResult)).OnDispose(b_CustomComponent);
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{b_EventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void OnRun<K, T>(K b_CustomComponent, EventSystemType b_EventSystemType, T b_Args) where K : ACustomComponent
        {
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                if (temp.TryGetValue((int)b_EventSystemType, out Type mResult))
                {
                    switch (b_EventSystemType)
                    {
                        case EventSystemType.INIT:
                            ((ITEventMethodOnInit<K, T>)Resolve(mResult)).OnInit(b_CustomComponent, b_Args);
                            break;
                        case EventSystemType.LOAD:
                            ((ITEventMethodOnLoad)Resolve(mResult)).OnLoad(b_CustomComponent);
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{b_EventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void OnRun<K, T1, T2>(K b_CustomComponent, EventSystemType b_EventSystemType, T1 b_Args1, T2 b_Args2) where K : ACustomComponent
        {
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                if (temp.TryGetValue((int)b_EventSystemType, out Type mResult))
                {
                    switch (b_EventSystemType)
                    {
                        case EventSystemType.INIT:
                            ((ITEventMethodOnInit<K, T1, T2>)Resolve(mResult)).OnInit(b_CustomComponent, b_Args1, b_Args2);
                            break;
                        case EventSystemType.LOAD:
                            ((ITEventMethodOnLoad)Resolve(mResult)).OnLoad(b_CustomComponent);
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{b_EventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void OnRun<K, T1, T2, T3>(K b_CustomComponent, EventSystemType b_EventSystemType, T1 b_Args1, T2 b_Args2, T3 b_Args3) where K : ACustomComponent
        {
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                if (temp.TryGetValue((int)b_EventSystemType, out Type mResult))
                {
                    switch (b_EventSystemType)
                    {
                        case EventSystemType.INIT:
                            ((ITEventMethodOnInit<K, T1, T2, T3>)Resolve(mResult)).OnInit(b_CustomComponent, b_Args1, b_Args2, b_Args3);
                            break;
                        case EventSystemType.LOAD:
                            ((ITEventMethodOnLoad)Resolve(mResult)).OnLoad(b_CustomComponent);
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{b_EventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void OnRun<K, T1, T2, T3, T4>(K b_CustomComponent, EventSystemType b_EventSystemType, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4) where K : ACustomComponent
        {
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                if (temp.TryGetValue((int)b_EventSystemType, out Type mResult))
                {
                    switch (b_EventSystemType)
                    {
                        case EventSystemType.INIT:
                            ((ITEventMethodOnInit<K, T1, T2, T3, T4>)Resolve(mResult)).OnInit(b_CustomComponent, b_Args1, b_Args2, b_Args3, b_Args4);
                            break;
                        case EventSystemType.LOAD:
                            ((ITEventMethodOnLoad)Resolve(mResult)).OnLoad(b_CustomComponent);
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{b_EventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void OnRun<K, T1, T2, T3, T4, T5>(K b_CustomComponent, EventSystemType b_EventSystemType, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4, T5 b_Args5) where K : ACustomComponent
        {
            if (mAllEventTypeMethodDic.TryGetValue(b_CustomComponent.GetType(), out Dictionary<int, Type> temp))
            {
                if (temp.TryGetValue((int)b_EventSystemType, out Type mResult))
                {
                    switch (b_EventSystemType)
                    {
                        case EventSystemType.INIT:
                            ((ITEventMethodOnInit<K, T1, T2, T3, T4, T5>)Resolve(mResult)).OnInit(b_CustomComponent, b_Args1, b_Args2, b_Args3, b_Args4, b_Args5);
                            break;
                        case EventSystemType.LOAD:
                            ((ITEventMethodOnLoad)Resolve(mResult)).OnLoad(b_CustomComponent);
                            break;
                        default:
                            MethodBase mMethodBase = new StackTrace().GetFrame(1).GetMethod();
                            throw new Exception($"=>{MethodInfo.GetCurrentMethod().Name}:\tTime:{DateTime.Now}\n   {mMethodBase.Name}({string.Join(",", mMethodBase.GetParameters().ToList())})方法\n   输出日志信息:没有实现事件类型:{b_EventSystemType}\n{Environment.StackTrace}\n\n");
                    }
                }
            }
        }
        public void OnRun(string b_EventName)
        {
            if (mAllEventNameMethodDic.TryGetValue(b_EventName, out var mResultlist))
            {
                for (int i = 0, len = mResultlist.Count; i < len; i++)
                {
                    var mResult = mResultlist[i];
                    try
                    {
                        ((ITEventMethodOnRun)Resolve(mResult)).OnRun();
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Event '{b_EventName}' Exception.", e);
                    }
                }
            }
        }
        public void OnRun<T>(string b_EventName, T b_Args)
        {
            if (mAllEventNameMethodDic.TryGetValue(b_EventName, out var mResultlist))
            {
                for (int i = 0, len = mResultlist.Count; i < len; i++)
                {
                    var mResult = mResultlist[i];
                    try
                    {
                        ((ITEventMethodOnRun<T>)Resolve(mResult)).OnRun(b_Args);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Event '{b_EventName}' Exception.", e);
                    }
                }
            }
        }
        public void OnRun<T1, T2>(string b_EventName, T1 b_Args1, T2 b_Args2)
        {
            if (mAllEventNameMethodDic.TryGetValue(b_EventName, out var mResultlist))
            {
                for (int i = 0, len = mResultlist.Count; i < len; i++)
                {
                    var mResult = mResultlist[i];
                    try
                    {
                        ((ITEventMethodOnRun<T1, T2>)Resolve(mResult)).OnRun(b_Args1, b_Args2);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Event '{b_EventName}' Exception.", e);
                    }
                }
            }
        }
        public void OnRun<T1, T2, T3>(string b_EventName, T1 b_Args1, T2 b_Args2, T3 b_Args3)
        {
            if (mAllEventNameMethodDic.TryGetValue(b_EventName, out var mResultlist))
            {
                for (int i = 0, len = mResultlist.Count; i < len; i++)
                {
                    var mResult = mResultlist[i];
                    try
                    {
                        ((ITEventMethodOnRun<T1, T2, T3>)Resolve(mResult)).OnRun(b_Args1, b_Args2, b_Args3);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Event '{b_EventName}' Exception.", e);
                    }
                }
            }
        }
        public void OnRun<T1, T2, T3, T4>(string b_EventName, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4)
        {
            if (mAllEventNameMethodDic.TryGetValue(b_EventName, out var mResultlist))
            {
                for (int i = 0, len = mResultlist.Count; i < len; i++)
                {
                    var mResult = mResultlist[i];
                    try
                    {
                        ((ITEventMethodOnRun<T1, T2, T3, T4>)Resolve(mResult)).OnRun(b_Args1, b_Args2, b_Args3, b_Args4);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Event '{b_EventName}' Exception.",e);
                    }
                }
            }
        }
        public void OnRun<T1, T2, T3, T4, T5>(string b_EventName, T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4, T5 b_Args5)
        {
            if (mAllEventNameMethodDic.TryGetValue(b_EventName, out var mResultlist))
            {
                for (int i = 0, len = mResultlist.Count; i < len; i++)
                {
                    var mResult = mResultlist[i];
                    try
                    {
                        ((ITEventMethodOnRun<T1, T2, T3, T4, T5>)Resolve(mResult)).OnRun(b_Args1, b_Args2, b_Args3, b_Args4, b_Args5);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Event '{b_EventName}' Exception.", e);
                    }
                }
            }
        }
    }
}
