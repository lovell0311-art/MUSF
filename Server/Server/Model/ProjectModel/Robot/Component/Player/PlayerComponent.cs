using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class PlayerComponentDestroySystem : DestroySystem<PlayerComponent>
    {
        public override void Destroy(PlayerComponent self)
        {
            if(self.LocalUnit != null)
            {
                self.LocalUnit.Dispose();
                self.LocalUnit = null;
            }
        }
    }


    public class PlayerComponent : Entity
    {
        /// <summary>
        /// 本地玩家
        /// </summary>
        public Unit LocalUnit;
    }
}
