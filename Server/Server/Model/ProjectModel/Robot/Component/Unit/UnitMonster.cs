using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class UnitMonsterAwakeSystem : AwakeSystem<UnitMonster, Enemy_InfoConfig>
    {
        public override void Awake(UnitMonster self, Enemy_InfoConfig config)
        {
            self.Config = config;
        }
    }

    [ObjectSystem]
    public class UnitMonsterDestroySystem : DestroySystem<UnitMonster>
    {
        public override void Destroy(UnitMonster self)
        {
            self.Config = null;
        }

    }



    public class UnitMonster : Entity
    {
        public Enemy_InfoConfig Config;
    }
}
