using CustomFrameWork.CustomInterface;
using System;

namespace CustomFrameWork.Baseic
{
    public abstract partial class ACustomComponent
    {
        /// <summary>
        /// 添加组件 实现添加事件 init(有参) awake(有参) start(无参) update(无参)  awake参数可与init一致 或者没有  不可不同
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <typeparam name="T">init awake参数类型</typeparam>
        /// <param name="b_Args">参数</param>
        /// <returns>组件</returns>
        public K AddCustomComponent<K, T>(T b_Args) where K : CustomComponent
        {
            Type type = typeof(K);
            if (_CustomComponentDic.TryGetValue(type, out CustomComponent mCustomComponent))
                return mCustomComponent as K;

            K mResult = Root.CreateBuilder.GetInstance<K>(type, false);
            _CustomComponentDic[type] = mResult;
            if (mResult is IAwakeComponent<T> component)
                component.Awake(this, b_Args);
            else
                mResult.Awake(this);

            Root.EventSystem.AddComponent(mResult, b_Args);

            return mResult;
        }
        /// <summary>
        /// 添加组件 实现添加事件 init(有参) awake(有参) start(无参) update(无参)  awake参数可与init一致 或者没有  不可不同
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <typeparam name="T1">init awake参数类型</typeparam>
        /// <typeparam name="T2">init awake参数类型</typeparam>
        /// <param name="b_Args1">第1个参数</param>
        /// <param name="b_Args2">第2个参数</param>
        /// <returns>组件</returns>
        public K AddCustomComponent<K, T1, T2>(T1 b_Args1, T2 b_Args2) where K : CustomComponent
        {
            Type type = typeof(K);
            if (_CustomComponentDic.TryGetValue(type, out CustomComponent mCustomComponent))
                return mCustomComponent as K;

            K mResult = Root.CreateBuilder.GetInstance<K>(type, false);
            _CustomComponentDic[type] = mResult;
            if (mResult is IAwakeComponent<T1, T2> component)
                component.Awake(this, b_Args1, b_Args2);
            else
                mResult.Awake(this);

            Root.EventSystem.AddComponent(mResult, b_Args1, b_Args2);
            return mResult;
        }
        /// <summary>
        /// 添加组件 实现添加事件 init(有参) awake(有参) start(无参) update(无参)  awake参数可与init一致 或者没有  不可不同
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <typeparam name="T1">init awake参数类型</typeparam>
        /// <typeparam name="T2">init awake参数类型</typeparam>
        /// <typeparam name="T3">init awake参数类型</typeparam>
        /// <param name="b_Args1">第1个参数</param>
        /// <param name="b_Args2">第2个参数</param>
        /// <param name="b_Args3">第3个参数</param>
        /// <returns>组件</returns>
        public K AddCustomComponent<K, T1, T2, T3>(T1 b_Args1, T2 b_Args2, T3 b_Args3) where K : CustomComponent
        {
            Type type = typeof(K);
            if (_CustomComponentDic.TryGetValue(type, out CustomComponent mCustomComponent))
                return mCustomComponent as K;

            K mResult = Root.CreateBuilder.GetInstance<K>(type, false);
            _CustomComponentDic[type] = mResult;
            if (mResult is IAwakeComponent<T1, T2, T3> component)
                component.Awake(this, b_Args1, b_Args2, b_Args3);
            else
                mResult.Awake(this);

            Root.EventSystem.AddComponent(mResult, b_Args1, b_Args2, b_Args3);
            return mResult;
        }
        /// <summary>
        /// 添加组件 实现添加事件 init(有参) awake(有参) start(无参) update(无参)  awake参数可与init一致 或者没有  不可不同
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <typeparam name="T1">init awake参数类型</typeparam>
        /// <typeparam name="T2">init awake参数类型</typeparam>
        /// <typeparam name="T3">init awake参数类型</typeparam>
        /// <typeparam name="T4">init awake参数类型</typeparam>
        /// <param name="b_Args1">第1个参数</param>
        /// <param name="b_Args2">第2个参数</param>
        /// <param name="b_Args3">第3个参数</param>
        /// <param name="b_Args4">第4个参数</param>
        /// <returns>组件</returns>
        public K AddCustomComponent<K, T1, T2, T3, T4>(T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4) where K : CustomComponent
        {
            Type type = typeof(K);
            if (_CustomComponentDic.TryGetValue(type, out CustomComponent mCustomComponent))
                return mCustomComponent as K;

            K mResult = Root.CreateBuilder.GetInstance<K>(type, false);
            _CustomComponentDic[type] = mResult;
            if (mResult is IAwakeComponent<T1, T2, T3, T4> component)
                component.Awake(this, b_Args1, b_Args2, b_Args3, b_Args4);
            else
                mResult.Awake(this);

            Root.EventSystem.AddComponent(mResult, b_Args1, b_Args2, b_Args3, b_Args4);
            return mResult;
        }
        /// <summary>
        /// 添加组件 实现添加事件 init(有参) awake(有参) start(无参) update(无参)  awake参数可与init一致 或者没有  不可不同
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <typeparam name="T1">init awake参数类型</typeparam>
        /// <typeparam name="T2">init awake参数类型</typeparam>
        /// <typeparam name="T3">init awake参数类型</typeparam>
        /// <typeparam name="T4">init awake参数类型</typeparam>
        /// <typeparam name="T5">init awake参数类型</typeparam>
        /// <param name="b_Args1">第1个参数</param>
        /// <param name="b_Args2">第2个参数</param>
        /// <param name="b_Args3">第3个参数</param>
        /// <param name="b_Args4">第4个参数</param>
        /// <param name="b_Args5">第5个参数</param>
        /// <returns>组件</returns>
        public K AddCustomComponent<K, T1, T2, T3, T4, T5>(T1 b_Args1, T2 b_Args2, T3 b_Args3, T4 b_Args4, T5 b_Args5) where K : CustomComponent
        {
            Type type = typeof(K);
            if (_CustomComponentDic.TryGetValue(type, out CustomComponent mCustomComponent))
                return mCustomComponent as K;

            K mResult = Root.CreateBuilder.GetInstance<K>(type, false);
            _CustomComponentDic[type] = mResult;
            if (mResult is IAwakeComponent<T1, T2, T3, T4, T5> component)
                component.Awake(this, b_Args1, b_Args2, b_Args3, b_Args4, b_Args5);
            else
                mResult.Awake(this);

            Root.EventSystem.AddComponent(mResult, b_Args1, b_Args2, b_Args3, b_Args4, b_Args5);
            return mResult;
        }
    }
}
