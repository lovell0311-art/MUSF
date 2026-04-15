using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 强化物品枚举
    /// </summary>
    public enum EItemStrengthen
    {
        /// <summary>
        /// 祝福宝石
        /// </summary>
        BLESSING_GEMS = 280003,
        /// <summary>
        /// 灵魂宝石
        /// </summary>
        SOUL_GEMS = 280004,
        /// <summary>
        /// 生命宝石
        /// </summary>
        ANIMA_GEMS = 280005,
        /// <summary>
        /// 玛雅宝石
        /// </summary>
        MAYA_GEMS = 280001,
        /// <summary>
        /// 创造宝石
        /// </summary>
        CREATE_GEMS = 280006,
        /// <summary>
        /// 守护宝石
        /// </summary>
        GUARDIAN_GEMS = 280007,
        /// <summary>
        /// 再生原石
        /// </summary>
        ORECYCLED_GEMS = 280002,
        /// <summary>
        /// 再生宝石
        /// </summary>
        RECYCLED_GEMS = 280011,
        /// <summary>
        /// 初级进化宝石
        /// </summary>
        ELEMENTARY_EVOLUTION_GEMS = 280012,
        /// <summary>
        /// 高级进化宝石
        /// </summary>
        ADVANCED_EVOLUTION_GEMS = 280013,
        /// <summary>
        /// 幸运宝石
        /// </summary>
        LUCKY_GEMS = 280021,
        /// <summary>
        /// 卓越宝石
        /// </summary>
        EXC_GEMS = 280022,
        /// <summary>
        /// 解绑宝石
        /// </summary>
        UNBIND_GEMS = 280023,
        /// <summary>
        /// 光之石
        /// </summary>
        LIGHT_STONE = 270001,
        /// <summary>
        /// 光之石强化符文
        /// </summary>
        LIGHT_STONE_STRENGTHEN_RUNE = 320302,
        /// <summary>
        /// 低级魔晶石
        /// </summary>
        LOW_MAGIC_STONE = 280017,
        /// <summary>
        /// 中级魔晶石
        /// </summary>
        MID_MAGIC_STONE = 280018,
        /// <summary>
        /// 高级魔晶石
        /// </summary>
        HIGH_MAGIC_STONE = 280019,

        /// <summary>
        /// 翅膀重置卡
        /// </summary>
        Wing_ResetCard = 310074,

        /// <summary>
        /// 幸运符咒
        /// </summary>
        LUKCY_RULE = 320120,
        /// <summary>
        /// 幸运符咒 +10%
        /// </summary>
        LUKCY_RULE_10 = 320400,
        /// <summary>
        /// 合成保护符咒，只用于强化保护等级不变，超过10级等级减一装备不会碎
        /// </summary>
        PROTECT_RULE = 320141,
        /// <summary>
        /// 魔晶石
        /// </summary>
        MAGIC_STONE = 280020,
        /// <summary>
        /// 保护符咒,所有合成失败只消耗这个保护符咒，其余不变动
        /// </summary>
        Protection_Charm = 320318,
        /// <summary>
        /// 保护主要装备失败扣除其余道具
        /// </summary>
        MainEquipmentProtection = 320141,
    };
    /// <summary>
    /// 合成方法指定特殊处理逻辑
    /// </summary>
    public enum SynthesisType
    {
        /// <summary>
        /// 一般合成-洗点果实合成
        /// </summary>
        GuoShiSynthesis = 0,
    }
}
