using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    public enum RobotSkillType
    {
        /// <summary>
        /// 攻击技能
        /// </summary>
        Attack = 1, 
        /// <summary>
        /// 辅助技能
        /// </summary>
        Auxiliary = 2,
    }

    public enum RobotSkillConsume
    {
        MP = 1,
        AG = 2,
    }

    public enum RobotSkillUseStandard
    {
        Level = 1,
        /// <summary>
        /// 力量
        /// </summary>
        Strength = 2,
        /// <summary>
        /// 智力
        /// </summary>
        Willpower = 3,
        /// <summary>
        /// 敏捷
        /// </summary>
        Agility = 4,
        /// <summary>
        /// 统帅
        /// </summary>
        Command = 5,
        /// <summary>
        /// 体力
        /// </summary>
        BoneGas = 6,
    }

    public class RobotSkill : Entity
    {
        public int ConfigId;
        public string Name;
        public RobotSkillType SkillType;
        public int Distance;
        public int CoolTime;
        public Dictionary<RobotSkillConsume, int> Consume = new Dictionary<RobotSkillConsume, int>();
        public Dictionary<RobotSkillUseStandard, int> UseStandard = new Dictionary<RobotSkillUseStandard, int>();

        public long LastUseTime;
    }
}
