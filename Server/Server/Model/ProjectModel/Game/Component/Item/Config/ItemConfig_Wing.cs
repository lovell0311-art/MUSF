using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 物品基础配置
    /// </summary>
    public partial class ItemConfig
    {
        /// <summary>
        /// 伤害提高百分比
        /// </summary>
        public int DamagePct { get; set; }
        /// <summary>
        /// 伤害吸收
        /// </summary>
        public int DamageAbsPct { get; set; }
        /// <summary>
        /// 翅膀等级(几代翅膀)
        /// </summary>
        public int WingLevel { get; set; }

        /// <summary>
        /// 基础属性
        /// </summary>
        public List<int> BaseAttrId { get; set; } = new List<int>();
        /// <summary>
        /// 特殊属性
        /// </summary>
        public List<int> SpecialAttrId { get; set; } = new List<int>();
    }
}
