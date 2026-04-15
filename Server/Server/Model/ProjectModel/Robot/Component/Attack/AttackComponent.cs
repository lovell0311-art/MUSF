using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class AttackComponentDestroySystem : DestroySystem<AttackComponent>
    {
        public override void Destroy(AttackComponent self)
        {
            self.NextAttackTime = 0;
            self.NextAttackType = 0;
        }

    }


    public class AttackComponent : Entity
    {
        /// <summary>
        /// 普攻攻击距离
        /// </summary>
        public int NormalAttackDistance = 2;

        public int AttackDistance = 2;

        public long NextAttackTime = 0;
        /// <summary>
        /// 下次攻击类型
        /// </summary>
        public int NextAttackType = 0;
    }
}
