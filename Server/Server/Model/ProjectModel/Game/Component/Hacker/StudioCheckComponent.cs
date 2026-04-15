using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 工作室检测
    /// </summary>
    [PrivateObject]
    public class StudioCheckComponent : TCustomComponent<Player>
    {
        public long timerId = 0;
    }
}
