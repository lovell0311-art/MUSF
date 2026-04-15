using System.Collections.Generic;

namespace ETModel.Robot
{
    public class RobotItemComponent : Entity
    {
        public ItemConfig Config;
        /// <summary>
        /// 数量
        /// </summary>
        public long Count;
        /// <summary>
        /// 品质
        /// </summary>
        public int Quality;
        /// <summary>
        /// 保护时间
        /// </summary>
        public long ProtectTick;
        /// <summary>
        /// 击杀玩家
        /// </summary>
        public List<long> KillerId = new List<long>();
        /// <summary>
        /// 强化等级
        /// </summary>
        public int Level;
        /// <summary>
        /// 套装id
        /// </summary>
        public int SetId;

    }
}
