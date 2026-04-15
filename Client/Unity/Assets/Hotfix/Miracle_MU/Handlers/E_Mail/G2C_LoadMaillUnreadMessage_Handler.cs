using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_LoadMaillUnreadMessage_Handler : AMHandler<G2C_LoadMaillUnreadMessage>
    {
        protected override void Run(ETModel.Session session, G2C_LoadMaillUnreadMessage message)
        {
            ////∫Ïµ„Ã· æ
            RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Email, 1);
            UIMainComponent.Instance?.RedDotFriendCheack();
        }
    }

}
