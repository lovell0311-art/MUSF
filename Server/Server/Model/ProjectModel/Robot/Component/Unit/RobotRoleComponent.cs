using System;
using System.Collections.Generic;
using System.Text;


namespace ETModel.Robot
{
    public struct AddPointWeight
    {
        /// <summary>
        /// 力量
        /// </summary>
        public int Strength;
        /// <summary>
        /// 智力
        /// </summary>
        public int Willpower;
        /// <summary>
        /// 体力
        /// </summary>
        public int BoneGas;
        /// <summary>
        /// 敏捷
        /// </summary>
        public int Agility;
        /// <summary>
        /// 统帅
        /// </summary>
        public int Command;

    }

    [ObjectSystem]
    public class RobotRoleComponentAwakeSystem : AwakeSystem<RobotRoleComponent,CreateRole_InfoConfig>
    {
        public override void Awake(RobotRoleComponent self, CreateRole_InfoConfig config)
        {
            self.Config = config;
            self.AddPointWeight.Strength = config.Strength;
            self.AddPointWeight.Willpower = config.Willpower;
            self.AddPointWeight.BoneGas = config.BoneGas;
            self.AddPointWeight.Agility = config.Agility;
            self.AddPointWeight.Command = config.Command;
        }

    }

    [ObjectSystem]
    public class RobotRoleComponentDestroySystem : DestroySystem<RobotRoleComponent>
    {
        public override void Destroy(RobotRoleComponent self)
        {
            self.Config = null;
            self.Level = 0;
            self.OccupationLevel = 0;
            self.PkPoint = 0;
            self.Name = null;
        }
    }


    public class RobotRoleComponent : Entity
    {
        public string Name;
        
        public CreateRole_InfoConfig Config;
        public E_GameOccupation RoleType => (E_GameOccupation)Config.Id;
        public int Level;//角色等级
        public int OccupationLevel;// 转职次数
        public int PkPoint; // 红名点数

        public AddPointWeight AddPointWeight;   // 加点权重


    }
}
