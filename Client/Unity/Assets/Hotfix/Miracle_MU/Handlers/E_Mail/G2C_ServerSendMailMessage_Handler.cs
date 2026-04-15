using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 新邮件通知
    /// </summary>
    [MessageHandler]
    public class G2C_ServerSendMailMessage_Handler : AMHandler<G2C_ServerSendMailMessage>
    {
        protected override void Run(ETModel.Session session, G2C_ServerSendMailMessage message)
        {
            //延迟一秒提示红点
            TimerComponent.Instance.RegisterTimeCallBack(1000, () =>
            {
                ////红点提示
                RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Email, 1);
                UIMainComponent.Instance?.RedDotFriendCheack();
            }
            );
           
        }
    }
}
