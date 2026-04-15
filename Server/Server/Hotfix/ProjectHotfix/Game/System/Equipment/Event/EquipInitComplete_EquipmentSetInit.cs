using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;

namespace ETHotfix
{
    [EventMethod("EquipInitComplete")]
    public class EquipInitComplete_EquipmentSetInit : ITEventMethodOnRun<ETModel.EventType.EquipInitComplete>
    {
        public void OnRun(ETModel.EventType.EquipInitComplete args)
        {
            var equipmentSet = args.unit.Player.GetCustomComponent<EquipmentSetComponent>();
            var equipment = args.unit.Player.GetCustomComponent<EquipmentComponent>();
            if (equipmentSet == null || equipment == null)
            {
                return;
            }
            // 初始化 EquipmentSetComponent
            foreach(var item in equipment.EquipPartItemDict.Values)
            {
                equipmentSet.AddItemNotUpdate(item);
            }
            foreach(var itemSet in equipmentSet.AllItemSet.Values)
            {
                if(itemSet.NeedUpdate)
                {
                    itemSet.UpdateAttrEntry();
                }
            }
        }
    }
}
