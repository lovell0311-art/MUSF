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
    /// 坐骑属性更新方法
    /// </summary>
    [ItemUpdateProp]
    public class Mounts : Default
    {

        /// <summary>
        /// 更新耐久 重写耐久
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void UpdateDurability(Item item)
        {
            item.SetProp(EItemValue.DurabilityMax, 0);
        }

        /// <summary>
        /// 更新 维修价格
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void UpdateRepairMoney(Item item)
        {
            item.SetProp(EItemValue.RepairMoney, 0);
        }
        public override void ApplyEquipProp(Item item, EquipmentComponent equipCmt, EquipPosition pos)
        {
            //if (item.GetProp(EItemValue.Durability) <= 0) return;
            base.ApplyEquipProp(item, equipCmt, pos);
            // 使用原来的属性，并根据需求，添加新的属性

            var gamePlayer = equipCmt.mPlayer.GetCustomComponent<GamePlayer>();
            int Advanced = item.GetProp(EItemValue.Advanced);
            int ValueDefense =  Advanced * 10;

            gamePlayer.AddEquipProperty(E_GameProperty.Defense, ValueDefense);

        }
    }
}
