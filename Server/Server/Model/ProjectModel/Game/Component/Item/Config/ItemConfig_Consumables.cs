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
        /// 操作值1
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 操作值2
        /// </summary>
        public int Value2 { get; set; }
        /// <summary>
        /// 使用方法
        /// </summary>
        public string UseMethod { get; set; }
    }
}
