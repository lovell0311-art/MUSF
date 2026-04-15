using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using TencentCloud.Bda.V20200324.Models;

namespace ETHotfix.ItemUpdateProp
{
    /// <summary>
    /// 坐骑黑王马属性更新方法
    /// </summary>
    [ItemUpdateProp]
    public class Mounts_TianYing : Mounts
    {

        /// <summary>
        /// 更新开始
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void StartUpdate(Item item)
        {
            if (item.GetProp(EItemValue.MountsLevel) < 1)
            {
                item.SetProp(EItemValue.MountsLevel, 1);
            }
        }  /// <summary>
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

            int mLevel = item.GetProp(EItemValue.MountsLevel);
            int mCommand = gamePlayer.GetNumerial(E_GameProperty.Property_Command);
            int Advanced = item.GetProp(EItemValue.Advanced);
            int ValueDefense = Advanced * 10;

            gamePlayer.AddEquipProperty(E_GameProperty.Defense, ValueDefense);
            switch (item.ConfigID)
            {
                case 260015:
                    {
                        int mMinAtteck = 40, mMaxAtteck = 50, mAttackSpeed = 2;
                        gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, mMinAtteck + (int)(mLevel * 1.5f) + mCommand / 10);
                        gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, mMaxAtteck + (int)(mLevel * 1.5f) + mCommand / 5);
                        gamePlayer.AddEquipProperty(E_GameProperty.AttackSpeed, mAttackSpeed + (int)(mLevel * 0.8f) + mCommand / 50);
                        gamePlayer.AddEquipProperty(E_GameProperty.AtteckSuccessRate, 10 + mLevel * 16);
                    }
                    break;
                case 260019:
                    {
                        int mMinAtteck = 60, mMaxAtteck = 80, mAttackSpeed = 3;
                        gamePlayer.AddEquipProperty(E_GameProperty.MinAtteck, mMinAtteck + (int)(mLevel * 1.5f) + mCommand / 10);
                        gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteck, mMaxAtteck + (int)(mLevel * 1.5f) + mCommand / 5);
                        gamePlayer.AddEquipProperty(E_GameProperty.AttackSpeed, mAttackSpeed + (int)(mLevel * 0.8f) + mCommand / 50);
                        gamePlayer.AddEquipProperty(E_GameProperty.AtteckSuccessRate, 10 + mLevel * 16);
                    }
                    break;
                default:
                    break;
            }

        }
    }
}
