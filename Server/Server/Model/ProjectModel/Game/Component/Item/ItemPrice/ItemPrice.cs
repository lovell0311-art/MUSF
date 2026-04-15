using System;
using System.Collections.Generic;
using System.Text;
using Maths;

namespace ETModel
{
    /// <summary>
    /// 物品价格
    /// </summary>
    public class ItemPrice
    {
        /// <summary>
        /// 使用公式
        /// </summary>
        public bool UseFormula;
        public MathParser Formula;
        public List<int> Value;
    }
}
