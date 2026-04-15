using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{

    /// <summary>
    /// 实名认证
    /// </summary>
    [MessageHandler]
    public class C2G_NeedIdCardInspectMessage_Handler : AMHandler<C2G_NeedIdCardInspectMessage>
    {
        protected override void Run(ETModel.Session session, C2G_NeedIdCardInspectMessage message)
        {
            Log.DebugGreen($"Account：{message.Account}");
            UIComponent.Instance.VisibleUI(UIType.UIRealName,message.Account);

        }
    }
}
