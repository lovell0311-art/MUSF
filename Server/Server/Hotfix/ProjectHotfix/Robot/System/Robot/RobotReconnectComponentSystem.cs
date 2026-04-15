using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using System;


namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class RobotReconnectComponentAwakeSystem : AwakeSystem<RobotReconnectComponent, Robot_AccountConfig>
    {
        public override void Awake(RobotReconnectComponent self, Robot_AccountConfig config)
        {
            self.Config = config;
        }
    }


    [ObjectSystem]
    public class RobotReconnectComponentDestroySystem : DestroySystem<RobotReconnectComponent>
    {
        public override void Destroy(RobotReconnectComponent self)
        {
            if (self.Config != null)
            {
                RobotManagerComponent.Instance.AddToLoginList(self.Config).Coroutine();
            }
            self.Config = null;
        }
    }


    public static partial class RobotReconnectComponentSystem
    {
       

    }
}
