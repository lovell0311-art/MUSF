using System;
using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;


namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class RobotSkillAwakeSystem : AwakeSystem<RobotSkill>
    {
        public override void Awake(RobotSkill self)
        {
            self.LastUseTime = 0;
        }
    }

    [ObjectSystem]
    public class RobotSkillDestroySystem : DestroySystem<RobotSkill>
    {
        public override void Destroy(RobotSkill self)
        {
            self.ConfigId = 0;
            self.Name = "";
            self.SkillType = RobotSkillType.Attack;
            self.Distance = 0;
            self.CoolTime = 0;
            self.Consume.Clear();
            self.UseStandard.Clear();
            self.LastUseTime = 0;
    }
}

    public static partial class RobotSkillSystem
    {
        /// <summary>
        /// 可以使用这个技能
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool CanUse(this RobotSkill self)
        {
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(self.ClientScene());
            NumericComponent numeric = localUnit.GetComponent<NumericComponent>();

            foreach(var kv in self.Consume)
            {
                switch(kv.Key)
                {
                    case RobotSkillConsume.MP:
                        if (kv.Value > numeric.GetAsInt((int)E_GameProperty.PROP_MP)) return false;
                        break;
                    case RobotSkillConsume.AG:
                        if (kv.Value > numeric.GetAsInt((int)E_GameProperty.PROP_AG)) return false;
                        break;
                }
            }

            foreach(var kv in self.UseStandard)
            {
                switch (kv.Key)
                {
                    case RobotSkillUseStandard.Level:
                        if (kv.Value > numeric.GetAsInt((int)E_GameProperty.Level)) return false;
                        break;
                    case RobotSkillUseStandard.Strength:
                        if (kv.Value > numeric.GetAsInt((int)E_GameProperty.Property_Strength)) return false;
                        break;
                    case RobotSkillUseStandard.Willpower:
                        if (kv.Value > numeric.GetAsInt((int)E_GameProperty.Property_Willpower)) return false;
                        break;
                    case RobotSkillUseStandard.Agility:
                        if (kv.Value > numeric.GetAsInt((int)E_GameProperty.Property_Agility)) return false;
                        break;
                    case RobotSkillUseStandard.Command:
                        if (kv.Value > numeric.GetAsInt((int)E_GameProperty.Property_Command)) return false;
                        break;
                    case RobotSkillUseStandard.BoneGas:
                        if (kv.Value > numeric.GetAsInt((int)E_GameProperty.Property_BoneGas)) return false;
                        break;
                }
            }

            switch (self.ConfigId)
            {
                case 205:
                case 201:
                case 214:
                case 218:
                case 216:
                    {
                        RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
                        RobotItem weapon = equipment.GetItem(EquipPosition.Weapon);
                        RobotItem shield = equipment.GetItem(EquipPosition.Shield);
                        if (weapon == null || shield == null) return false;
                        if (!(weapon.Type == EItemType.Bows && (shield.Type == EItemType.Arrow || shield.Config.Id == 40019)) ||
                            !(weapon.Type == EItemType.Crossbows && (shield.Type == EItemType.Arrow || shield.Config.Id == 50012))) return false;
                    }
                    break;
            }

            
            

            long nowTime = Help_TimeHelper.GetNow();
            if (self.LastUseTime + self.CoolTime > nowTime) return false;
            return true;
        }



    }
}
