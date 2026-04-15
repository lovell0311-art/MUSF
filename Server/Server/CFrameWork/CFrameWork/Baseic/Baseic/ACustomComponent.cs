using System;
using System.Collections.Generic;
using System.Linq;
#if !SERVER
using Object = UnityEngine.Object;
#else
using Object = System.Object;
#endif
namespace CustomFrameWork.Baseic
{
    /// <summary>
    /// 组件
    /// </summary>
    public abstract partial class ACustomComponent : ADataContext
    {
        private readonly Dictionary<Type, CustomComponent> _CustomComponentDic = new Dictionary<Type, CustomComponent>();

        /// <summary>
        /// 添加组件 实现添加事件 init(无参) awake(无参) start(无参) update(无参)
        /// </summary>
        /// <typeparam name="K">组件类型</typeparam>
        /// <returns>组件</returns>
        public K AddCustomComponent<K>() where K : CustomComponent
        {
            Type type = typeof(K);
            if (_CustomComponentDic.TryGetValue(type, out CustomComponent mCustomComponent))
                return mCustomComponent as K;

            K mResult = Root.CreateBuilder.GetInstance<K>(type, false);
            _CustomComponentDic[type] = mResult;
            mResult.Awake(this);

            Root.EventSystem.AddComponent<K>(mResult);
            return mResult;
        }

        /// <summary>
        /// 获取对应组件
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K GetCustomComponent<K>() where K : CustomComponent
        {
            if (_CustomComponentDic.TryGetValue(typeof(K), out CustomComponent mCustomComponent))
                return (K)mCustomComponent;
            return default;
        }
        /// <summary>
        /// 注：适合 awake 无参组件使用 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K EnsureGetCustomComponent<K>() where K : CustomComponent
        {
            if (_CustomComponentDic.TryGetValue(typeof(K), out CustomComponent mCustomComponent))
                return (K)mCustomComponent;
            return AddCustomComponent<K>();
        }
        public bool RemoveCustomComponent<K>() where K : CustomComponent
        {
            Type type = typeof(K);
            K mResult = null;
            if (_CustomComponentDic.TryGetValue(type, out CustomComponent mCustomComponent))
                mResult = mCustomComponent as K;
            if (mResult != null)
            {
                _CustomComponentDic.Remove(type);
                mResult.Dispose();
                return true;
            }
            return false;
        }
        public void RemoveAllCustomComponent()
        {
            if (_CustomComponentDic.Count > 0)
            {
                ACustomComponent[] mACustomComponentArray = _CustomComponentDic.Values.ToArray();
                for (int i = 0, len = mACustomComponentArray.Length; i < len; i++)
                    mACustomComponentArray[i].Dispose();
                _CustomComponentDic.Clear();
            }
        }
        /// <summary>
        /// 清理回收当前对象Dispose
        /// </summary>
        public override void Dispose()
        {
            RemoveAllCustomComponent();
            Root.EventSystem.OnRun(this, EventSystemType.DISPOSE);
            Root.EventSystem.RemoveComponent(this as CustomComponent);
            base.Dispose();
        }
    }
}
