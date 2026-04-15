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
        /// 攻击力/魔法攻击力/诅咒力 最小
        /// </summary>
        public int AllAttackMin { get; set; }
        /// <summary>
        /// 攻击力/魔法攻击力/诅咒力 最大
        /// </summary>
        public int AllAttackMax { get; set; }
        /// <summary>
        /// 属性防御
        /// </summary>
        public int AttrDefense { get; set; }
    }
}
