using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class UnitAwakeSystem : AwakeSystem<Unit>
    {
        public override void Awake(Unit self)
        {

        }
    }

    [ObjectSystem]
    public class UnitDestroySystem : DestroySystem<Unit>
    {
        public override void Destroy(Unit self)
        {
            if (self.CurrentMap != null)
            {
                self.CurrentMap.UnitLeave(self);
                self.CurrentMap = null;
            }
            self.UnitType = UnitType.None;
            self.Position.x = 0;
            self.Position.y = 0;
            self.IgnoreCollision = false;
        }
    }

    public static partial class UnitSystem
    {
    }
}
