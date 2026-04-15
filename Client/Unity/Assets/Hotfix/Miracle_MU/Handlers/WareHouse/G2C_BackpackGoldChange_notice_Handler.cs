using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_BackpackGoldChange_notice_Handler : AMHandler<G2C_BackpackGoldChange_notice>
    {
        protected override void Run(ETModel.Session session, G2C_BackpackGoldChange_notice message)
        {
         //   Log.DebugGreen($"玩家金币变化：{message.Gold}");
            UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.GoldCoin,message.Gold);
            Game.EventCenter.EventTrigger<long>(EventTypeId.GLOD_CHANGE, message.Gold);//监听 金币变动
        }
    }
}
