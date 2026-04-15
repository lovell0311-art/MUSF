using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;

namespace ETHotfix
{
    [EventMethod("UnloadEquipItem")]
    public class UnloadEquipItemEvent_EquipmentSetRemoveItem : ITEventMethodOnRun<ETModel.EventType.UnloadEquipItem>
    {
        public void OnRun(ETModel.EventType.UnloadEquipItem args)
        {
            var equipmentSet = args.unit.Player.GetCustomComponent<EquipmentSetComponent>();
            if (equipmentSet == null)
            {
                return;
            }
            // 套装组件移除物品
            equipmentSet.RemoveItem(args.item);
        }
    }
}
