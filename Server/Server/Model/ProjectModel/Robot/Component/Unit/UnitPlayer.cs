using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    public class UnitPlayerAwakeSystem : AwakeSystem<UnitPlayer, CreateRole_InfoConfig>
    {
        public override void Awake(UnitPlayer self, CreateRole_InfoConfig config)
        {
            self.Config = config;
        }
    }

    public class UnitPlayer : Entity
    {
        public CreateRole_InfoConfig Config;
    }
}
