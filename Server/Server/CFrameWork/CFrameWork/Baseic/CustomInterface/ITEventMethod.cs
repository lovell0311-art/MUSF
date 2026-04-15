
using System;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using System.Text;

namespace CustomFrameWork.Baseic
{
    /// <summary>
    /// 事件基类接口
    /// </summary>
    public interface ITEventMethod
    {

    }
    public interface ITEventMethodOnDispose
    {
        void OnDispose(ACustomComponent b_CustomComponent);
    }
    public interface ITEventMethodOnLoad
    {
        void OnLoad(ACustomComponent b_CustomComponent);
    }
}
