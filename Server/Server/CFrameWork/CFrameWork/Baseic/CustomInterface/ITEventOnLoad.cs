
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
    public abstract class ITEventMethodOnLoad<K> : ITEventMethodOnLoad where K : ACustomComponent
    {
        public void OnLoad(ACustomComponent b_CustomComponent)
        {
            OnLoad((K)b_CustomComponent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_CustomComponent"></param>
        public abstract void OnLoad(K b_CustomComponent);
    }

}
