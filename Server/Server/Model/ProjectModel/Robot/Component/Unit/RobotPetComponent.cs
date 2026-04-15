using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class RobotPetComponentAwakeSystem : AwakeSystem<RobotPetComponent, Pets_InfoConfig>
    {
        public override void Awake(RobotPetComponent self, Pets_InfoConfig config)
        {
            self.Config = config;
            self.OwnerId = 0;
        }
    }

    [ObjectSystem]
    public class RobotPetComponentDestroySystem : DestroySystem<RobotPetComponent>
    {
        public override void Destroy(RobotPetComponent self)
        {
            self.Config = null;
            self.OwnerId = 0;
        }
    }



    public class RobotPetComponent : Entity
    {
        public Pets_InfoConfig Config;
        /// <summary>
        /// 主人id
        /// </summary>
        public long OwnerId;
    }
}
