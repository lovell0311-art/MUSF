using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_CopyRoomStateUpdate_notice_Handler : AMHandler<G2C_CopyRoomStateUpdate_notice>
    {
        protected override void Run(ETModel.Session session, G2C_CopyRoomStateUpdate_notice message)
        {
            UIMainComponent.Instance.xueSeStata = message.State;
         
            switch (message.State)
            {
                case 1:
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"攻破了血色城门");
                    UIMainComponent.Instance.ChangeXueSeAstar(true, true);
                    break;
                case 2:
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"打开了水晶灵柩");
                    break;
                default:
                    break;
            }
        }
    }

}
