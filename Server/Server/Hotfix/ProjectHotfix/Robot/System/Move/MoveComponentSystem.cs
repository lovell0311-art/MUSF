using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class MoveComponentDestroySystem : DestroySystem<MoveComponent>
    {
        public override void Destroy(MoveComponent self)
        {
            self.MovePointList.Clear();
        }
    }


    public static partial class MoveComponentSystem
    {
        public static void StopMove(this MoveComponent self)
        {
            if(self.MovePointList.Count != 0)
            {
                self.MovePointList.Clear();
            }
        }

    }
}
