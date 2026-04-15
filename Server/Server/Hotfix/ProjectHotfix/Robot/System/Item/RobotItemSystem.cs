using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class RobotItemAwakeSystem:AwakeSystem<RobotItem,ItemConfig,Scene>
    {
        public override void Awake(RobotItem self, ItemConfig config, Scene clientScene)
        {
            self.Config = config;
            clientScene.GetComponent<RobotItemManager>().AllItems.Add(self.Id, self);
        }
    }

    [ObjectSystem]
    public class RobotItemDestroySystem : DestroySystem<RobotItem>
    {
        public override void Destroy(RobotItem self)
        {
            Scene clientScene = self.ClientScene();
            clientScene.GetComponent<RobotItemManager>().AllItems.Remove(self.Id);
            self.Config = null;
        }
    }


    public static partial class RobotItemSystem
    {
        /// <summary>
        /// 可以出售
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool CanSell(this RobotItem self)
        {
            NumericComponent numeric = self.GetComponent<NumericComponent>();
            if (numeric.GetAsInt((int)EItemValue.IsTask) == 1) return false;    // 任务物品，无法出售
            if (numeric.GetAsInt((int)EItemValue.SellMoney) <= 0) return false; // 售价为 0 的物品，无法出售
            return true;
        }

        /// <summary>
        /// 可以使用
        /// </summary>
        /// <param name="self"></param>
        /// <param name="localUnit"></param>
        /// <returns></returns>
        public static bool CanUse(this RobotItem self, Unit localUnit)
        {
            RobotRoleComponent role = localUnit.GetComponent<RobotRoleComponent>();
            if (self.Config.UseRole.Count != 0)
            {
                if (!self.Config.UseRole.TryGetValue((int)role.RoleType, out var level) || level < 0 || level > role.OccupationLevel)
                {
                    return false;
                }
            }
            NumericComponent numeric = localUnit.GetComponent<NumericComponent>();
            if (self.Config.ReqLvl > numeric.GetAsInt((int)E_GameProperty.Level)) { return false; }


            bool ret = true;
            bool update = false;

            if (self.Config.ReqAgi > numeric.GetAsInt((int)E_GameProperty.Property_Agility)) 
            {
                ret = false;
                role.AddPointWeight.Agility += numeric.GetAsInt((int)E_GameProperty.Property_Agility) - self.Config.ReqAgi;
                if (role.AddPointWeight.Agility > 1000000) update = true;
            }
            if (self.Config.ReqCom > numeric.GetAsInt((int)E_GameProperty.Property_Command))
            {
                ret = false;
                role.AddPointWeight.Command += numeric.GetAsInt((int)E_GameProperty.Property_Command) - self.Config.ReqCom;
                if (role.AddPointWeight.Command > 1000000) update = true;
            }
            if (self.Config.ReqEne > numeric.GetAsInt((int)E_GameProperty.Property_Willpower))
            {
                ret = false;
                role.AddPointWeight.Willpower += numeric.GetAsInt((int)E_GameProperty.Property_Willpower) - self.Config.ReqEne;
                if (role.AddPointWeight.Willpower > 1000000) update = true;
            }
            if (self.Config.ReqStr > numeric.GetAsInt((int)E_GameProperty.Property_Strength))
            {
                ret = false;
                role.AddPointWeight.Strength += numeric.GetAsInt((int)E_GameProperty.Property_Strength) - self.Config.ReqStr;
                if (role.AddPointWeight.Strength > 1000000) update = true;
            }
            if (self.Config.ReqVit > numeric.GetAsInt((int)E_GameProperty.Property_BoneGas))
            {
                ret = false;
                role.AddPointWeight.BoneGas += numeric.GetAsInt((int)E_GameProperty.Property_BoneGas) - self.Config.ReqVit;
                if (role.AddPointWeight.BoneGas > 1000000) update = true;
            }
            if(update)
            {
                role.AddPointWeight.Agility /= 100;
                role.AddPointWeight.Command /= 100;
                role.AddPointWeight.Willpower /= 100;
                role.AddPointWeight.Strength /= 100;
                role.AddPointWeight.BoneGas /= 100;
            }

            return ret;
        }
    }
}
