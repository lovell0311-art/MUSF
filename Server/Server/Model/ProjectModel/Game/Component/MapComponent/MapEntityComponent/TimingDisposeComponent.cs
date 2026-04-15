using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 定时销毁父对象
    /// </summary>
    public class TimingDisposeComponent : TCustomComponent<CustomComponent>
    {
        public long TimerId = 0;
    }
}
