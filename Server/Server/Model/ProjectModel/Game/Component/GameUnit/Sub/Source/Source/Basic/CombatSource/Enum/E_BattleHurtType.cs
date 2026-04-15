using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
   
    /// <summary>
    /// 当前伤害类型
    /// </summary>
    public enum E_BattleHurtType
    {
        /// <summary>
        /// 公用的,共享的
        /// </summary>
        COMMON,
        /// <summary>
        /// 物理伤害 physics
        /// </summary>
        PHYSIC,
        /// <summary>
        /// 法术伤害
        /// </summary>
        MAGIC,
        /// <summary>
        /// 真实伤害
        /// </summary>
        REAL
    }
}