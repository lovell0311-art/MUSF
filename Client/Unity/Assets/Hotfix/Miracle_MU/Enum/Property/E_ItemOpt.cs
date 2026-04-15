using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 物品追加属性
    /// </summary>
    public enum E_ItemOpt
    {
        None = 0,
        /// <summary>
        /// 追加攻击力   4 8 12 16
        /// </summary>
        ATTACK = 1,
        /// <summary>
        /// 追加魔法攻击力   4 8 12 16
        /// </summary>
        MAGIC_ATTACK = 2,
        /// <summary>
        /// 追加防御率   4 8 12 16
        /// </summary>
        DEFENCE_RATE = 3,
        /// <summary>
        /// 追加防御力   4 8 12 16
        /// </summary>
        DEFENCE = 4,
        /// <summary>
        /// 生命自动回复  1 2 3 4
        /// </summary>
        HP_AUTO_RECOVERY = 5,
        /// <summary>
        /// 攻击力/魔法攻击力/诅咒能力增加 4 8 12 16
        /// </summary>
        ALL_ATTACK = 6,
        /// <summary>
        /// 追加防御力   5 10 15 20
        /// </summary>
        DEFENCE_2 = 7,
    };
}
