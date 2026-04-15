using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 物品属性更新处理
    /// </summary>
    public interface IItemUpdatePropHandler
    {
        /// <summary>
        /// 更新全部属性
        /// </summary>
        /// <param name="item">更新的物品</param>
        public void UpdateProp(Item item);

        /// <summary>
        /// 应用装备属性。特殊的装备，自定义方法，给单位添加属性
        /// </summary>
        /// <param name="item"></param>
        /// <param name="equipCmt"></param>
        /// <param name="pos"></param>
        public void ApplyEquipProp(Item item, EquipmentComponent equipCmt, EquipPosition pos);
    }
}
