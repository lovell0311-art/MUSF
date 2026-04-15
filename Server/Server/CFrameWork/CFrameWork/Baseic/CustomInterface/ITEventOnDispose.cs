
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomFrameWork.Baseic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public abstract class ITEventMethodOnDispose<K> : ITEventMethodOnDispose where K : ACustomComponent
    {
        public void OnDispose(ACustomComponent b_CustomComponent)
        {
            OnDispose((K)b_CustomComponent);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        public abstract void OnDispose(K b_CustomComponent);
    }
}
