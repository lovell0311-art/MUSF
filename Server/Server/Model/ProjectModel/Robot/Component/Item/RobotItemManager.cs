using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class RobotItemManagerAwakeSystem : AwakeSystem<RobotItemManager>
    {
        public override void Awake(RobotItemManager self)
        {
        }
    }

    [ObjectSystem]
    public class RobotItemManagerDestroySystem : DestroySystem<RobotItemManager>
    {
        public override void Destroy(RobotItemManager self)
        {
            self.AllItems.Clear();
        }
    }

    public class RobotItemManager : Entity
    {
        public Dictionary<long,RobotItem> AllItems = new Dictionary<long,RobotItem>();
    }
}
