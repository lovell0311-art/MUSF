using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETHotfix.ItemUpdateProp
{
    /// <summary>
    /// 守护属性更新方法
    /// </summary>
    [ItemUpdateProp]
    public class Guard : Default
    {

        /// <summary>
        /// 更新耐久 重写耐久
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void UpdateDurability(Item item)
        {
            item.SetProp(EItemValue.DurabilityMax, item.ConfigData.Life);
        }

        /// <summary>
        /// 更新 维修价格
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void UpdateRepairMoney(Item item)
        {
            item.SetProp(EItemValue.RepairMoney, 0);
        }


    }
}
