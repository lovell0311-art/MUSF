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
    /// 坐骑 炎狼兽 幻影 属性更新方法
    /// </summary>
    [ItemUpdateProp]
    public class Mounts_YanLangShou_HuanYing : Mounts
    {

        /// <summary>
        /// 应用装备属性到单位
        /// </summary>
        /// <param name="item"></param>
        /// <param name="equipCmt"></param>
        /// <param name="inPos"></param>
        public override void ApplyEquipProp(Item item, EquipmentComponent equipCmt, EquipPosition pos)
        {
            //if (item.GetProp(EItemValue.Durability) <= 0) return;
            base.ApplyEquipProp(item, equipCmt, pos);
            // 使用原来的属性，并根据需求，添加新的属性

            var gamePlayer = equipCmt.mPlayer.GetCustomComponent<GamePlayer>();

            gamePlayer.AddEquipProperty(E_GameProperty.PROP_HP_MAX, gamePlayer.Data.Level / 2);
            gamePlayer.AddEquipProperty(E_GameProperty.PROP_MP_MAX, gamePlayer.Data.Level / 2);
            gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, gamePlayer.Data.Level / 12);
            gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, gamePlayer.Data.Level / 12);
            gamePlayer.AddEquipProperty(E_GameProperty.MagicRate_Increase, gamePlayer.Data.Level / 25);
        }
    }
}
