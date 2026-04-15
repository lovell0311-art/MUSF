using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    public class RobotEquipmentComponent : Entity
    {
        public Dictionary<EquipPosition,RobotItem> EquipPartItemDict = new Dictionary<EquipPosition,RobotItem>();

        public bool EquipItem(RobotItem item,EquipPosition position)
        {
            if(EquipPartItemDict.TryAdd(position, item))
            {
                item.InComponent = EItemInComponent.Equipment;
                item.PosId = (int)position;
                return true;
            }
            return false;
        }

        public bool UnloadEquipItem(EquipPosition position)
        {
            if(EquipPartItemDict.TryGetValue(position, out RobotItem item))
            {
                EquipPartItemDict.Remove(position);
                item.Dispose();
                return true;
            }
            return false;
        }

        public RobotItem GetItem(EquipPosition position)
        {
            RobotItem item = null;
            EquipPartItemDict.TryGetValue(position, out item);
            return item;
        }

    }
}
