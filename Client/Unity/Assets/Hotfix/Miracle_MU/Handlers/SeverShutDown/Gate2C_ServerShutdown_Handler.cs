using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Gate2C_ServerShutdown_Handler : AMHandler<Gate2C_ServerShutdown>
    {
        protected override void Run(ETModel.Session session, Gate2C_ServerShutdown message)
        {
            UIMainComponent.Instance.isRollOver = false;
            UIMainComponent.Instance.ShowNotice($"<color=red>服务器以关闭 请重进游戏</color>");
        }
    }
}
