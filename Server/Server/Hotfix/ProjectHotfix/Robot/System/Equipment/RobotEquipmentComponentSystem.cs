using ETModel;
using ETModel.Robot;
using System.Linq;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class RobotEquipmentComponentAwakeSystem : AwakeSystem<RobotEquipmentComponent>
    {
        public override void Awake(RobotEquipmentComponent self)
        {

        }
    }


    [ObjectSystem]
    public class RobotEquipmentComponentDestroySystem : DestroySystem<RobotEquipmentComponent>
    {
        public override void Destroy(RobotEquipmentComponent self)
        {
            foreach(RobotItem item in self.EquipPartItemDict.Values.ToArray())
            {
                item.Dispose();
            }
            self.EquipPartItemDict.Clear();
        }
    }


    public static class RobotEquipmentComponentSystem
    {
        /// <summary>
        /// 可以装备
        /// </summary>
        /// <param name="self"></param>
        /// <param name="localUnit"></param>
        /// <returns></returns>
        public static bool CanEquip(this RobotEquipmentComponent self, RobotItem item)
        {
            Unit unit = self.GetParent<Unit>();
            if (!item.CanUse(unit)) return false;
            if (item.Config.Slot == (int)EquipPosition.Shield)
            {
                // 盾牌
                RobotEquipmentComponent equipment = unit.GetComponent<RobotEquipmentComponent>();
                RobotItem weapon = equipment.GetItem(EquipPosition.Weapon);
                if (weapon == null) return true;
                if (weapon.Config.TwoHand == 1) return false;
            }
            else if(item.Config.Slot == (int)EquipPosition.Weapon)
            {
                // 武器
                if (item.Config.TwoHand == 1)
                {
                    // 双手武器
                    RobotEquipmentComponent equipment = unit.GetComponent<RobotEquipmentComponent>();
                    RobotItem shield = equipment.GetItem(EquipPosition.Shield);
                    if (shield != null) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 将来可以装备
        /// </summary>
        /// <param name="self"></param>
        /// <param name="localUnit"></param>
        /// <returns></returns>
        public static bool CanEquipInFuture(this RobotEquipmentComponent self, RobotItem item)
        {
            Unit unit = self.GetParent<Unit>();
            RobotRoleComponent role = unit.GetComponent<RobotRoleComponent>();
            if (item.Config.UseRole.Count != 0)
            {
                if (!item.Config.UseRole.TryGetValue((int)role.RoleType, out var level) || level < 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
