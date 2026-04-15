using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ETModel.Robot
{
    public class MoveComponent : Entity
    {
        // 移动列表
        public LinkedList<Vector2Int> MovePointList = new LinkedList<Vector2Int>();

        public float MoveSpeed = 7.5f;
        public float RunSpeed = 14f;
        public bool IsRun
        {
            get
            {
                Unit unit = GetParent<Unit>();
                if (unit.CurrentMap == null) return false;
                if (unit.CurrentMap.IsSafeArea(unit.Position)) return false;
                RobotEquipmentComponent equipment = unit.GetComponent<RobotEquipmentComponent>();
                if (equipment == null) return false;
                if (equipment.EquipPartItemDict.ContainsKey(EquipPosition.Boots) ||
                    equipment.EquipPartItemDict.ContainsKey(EquipPosition.Mounts)) return true;
                return false;
            }
        }
        public long MoveTimeInterval
        {
            get
            {
                if(IsRun)
                {
                    return (long)(2000 / RunSpeed);
                }
                else
                {
                    return (long)(2000 / MoveSpeed);
                }
            }
        }

        /// <summary>
        /// 下一次传送时间
        /// </summary>
        public long NextDeliveryTime;


    }
}
