using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class UnitBackpackComponentAwakeSystem : AwakeSystem<RobotBackpackComponent>
    {
        public override void Awake(RobotBackpackComponent self)
        {
            self.IsFull = false;
            self.Init();
        }
    }

    public static partial class UnitBackpackComponentSystem
    {


    }
}
