using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;


namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class RobotItemComponentAwakeSystem : AwakeSystem<RobotItemComponent, ItemConfig>
    {
        public override void Awake(RobotItemComponent self, ItemConfig config)
        {
            self.Config = config;
        }
    }

    [ObjectSystem]
    public class RobotItemComponentDestroySystem : DestroySystem<RobotItemComponent>
    {
        public override void Destroy(RobotItemComponent self)
        {
            self.Config = null;
            self.Count = 0;
            self.Quality = 0;
            self.ProtectTick = 0;
            self.KillerId.Clear();
            self.Level = 0;
            self.SetId = 0;
    }
}

    public static partial class RobotItemComponentSystem
    {
    }
}
