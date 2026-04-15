using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;

namespace ETHotfix
{
    [EventMethod("EquipItem")]
    public class EquipItemEvent_EquipmentSetAddItem : ITEventMethodOnRun<ETModel.EventType.EquipItem>
    {
        public void OnRun(ETModel.EventType.EquipItem args)
        {
            var equipmentSet = args.unit.Player.GetCustomComponent<EquipmentSetComponent>();
            if (equipmentSet == null)
            {
                return;
            }
            // 将物品添加到 EquipmentSetComponent ，用来计算套装属性
            equipmentSet.AddItem(args.item);
        }
    }
}
