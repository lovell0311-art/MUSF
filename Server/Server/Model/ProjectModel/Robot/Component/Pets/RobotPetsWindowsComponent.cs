using System;
using System.Collections.Generic;
using System.Linq;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class RobotPetsWindowsComponentAwakeSystem : AwakeSystem<RobotPetsWindowsComponent>
    {
        public override void Awake(RobotPetsWindowsComponent self)
        {
        }

    }

    [ObjectSystem]
    public class RobotPetsWindowsComponentDestroySystem : DestroySystem<RobotPetsWindowsComponent>
    {
        public override void Destroy(RobotPetsWindowsComponent self)
        {
            foreach(RobotPetsInfo info in self.PetsDict.Values.ToArray())
            {
                info.Dispose();
            }
            self.PetsDict.Clear();
            self.FirstPetsId = 0;
        }
    }



    public class RobotPetsWindowsComponent : Entity
    {
        public Dictionary<long,RobotPetsInfo> PetsDict = new Dictionary<long,RobotPetsInfo>();
        public long FirstPetsId = 0;
    }
}
