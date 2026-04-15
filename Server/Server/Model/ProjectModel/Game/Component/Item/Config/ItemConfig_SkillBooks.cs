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
        /// 职业类型2技能id
        /// </summary>
        public Dictionary<E_GameOccupation, int> GameOccupation2SkillId { get; set; } = new Dictionary<E_GameOccupation, int>();

    }
}
