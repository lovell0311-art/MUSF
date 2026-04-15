
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// 单一对象存储器 全局通用使用 非全局不用
    /// </summary>
    public class SingleObjectComponent : TCustomComponent<MainFactory>
    {
        private readonly Dictionary<Type, object> mSingleObjectMemoryDic = new Dictionary<Type, object>();
        /// <summary>
        /// 初始化
        /// </summary>
        public override void Awake()
        {
            mSingleObjectMemoryDic.Clear();
        }


        /// <summary>
        /// 注册类型对象
        /// </summary>
        /// <param name="b_RegisterType">目标对象</param>
        /// <param name="b_IsOverride">是否重写对象</param>
        public void Register(Type b_RegisterType, bool b_IsOverride = false)
        {
            if (b_IsOverride == false)
            {
                if (mSingleObjectMemoryDic.ContainsKey(b_RegisterType))
                {
                    return;
                }
            }
            mSingleObjectMemoryDic[b_RegisterType] = Activator.CreateInstance(b_RegisterType);
        }
        /// <summary>
        /// 取出值对象
        /// </summary>
        /// <param name="b_RegisterType">键</param>
        /// <returns>值</returns>
        public object Resolve(Type b_RegisterType)
        {
            if (mSingleObjectMemoryDic.TryGetValue(b_RegisterType, out object mResult))
            {
                return mResult;
            }
            return null;
        }
        /// <summary>
        /// 清理
        /// </summary>
        public override void Clear()
        {
            mSingleObjectMemoryDic.Clear();
        }
    }
}
