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
    /// 坐骑黑王马属性更新方法
    /// </summary>
    [ItemUpdateProp]
    public class Mounts_HeiWangMa : Mounts
    {

        /// <summary>
        /// 更新开始
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void StartUpdate(Item item)
        {
            if(item.GetProp(EItemValue.MountsLevel) < 1)
            {
                item.SetProp(EItemValue.MountsLevel, 1);
            }
        }

        /// <summary>
        /// 更新 维修价格
        /// </summary>
        /// <param name="item">更新的物品</param>
//         public override void UpdateRepairMoney(Item item)
//         {
//             int repairMoney = 0;
//             if (item.HaveDurability() == true)
//             {
//                 float durMax = item.GetProp(EItemValue.DurabilityMax);
//                 float dur = item.GetProp(EItemValue.Durability);
//                 int buyMoney = item.GetProp(EItemValue.BuyMoney);
// 
//                 float lc4;
//                 float lc5 = 0;
//                 lc4 = durMax;
// 
//                 if (dur == lc4)
//                 {
//                     item.SetProp(EItemValue.RepairMoney, 0);
//                     return;
//                 }
// 
//                 float lc6 = 1.0f - dur / lc4;
//                 int lc7;
// 
//                 // diff
//                 lc7 = buyMoney;
// 
// 
//                 if (lc7 > 400000000) lc7 = 400000000;
// 
//                 if (lc7 >= 1000)
//                 {
//                     lc7 = lc7 / 100 * 100;
//                 }
//                 else if (lc7 >= 100)
//                 {
//                     lc7 = lc7 / 10 * 10;
//                 }
//                 float lc8 = (float)Math.Sqrt(lc7);
//                 float lc9 = (float)Math.Sqrt(Math.Sqrt(lc7));
//                 lc5 = 3.0f * lc8 * lc9;
//                 lc5 *= lc6;
//                 lc5 += 1.0f;
// 
//                 if (dur <= 0.0f)
//                 {
//                     // diff 死亡后，复活价格
//                     lc5 *= 2f;
//                 }
// 
//                 /* 源代码
//                 if (RequestPos == TRUE)
//                 {
//                     // 在背包中修复，没在npc修复
//                     lc5 += lc5 * 2; //season4 changed
//                     
//                     // 1.03 版本
//                     lc5 += lc5
//                 }
//                 */
//                 repairMoney = (int)lc5;
//                 //item.SetProp(EItemValue.RepairMoney, (int)lc5);
//             }
//             else
//             {
//                 repairMoney = 0;
//             }
// 
//             if (repairMoney > 1000)
//             {
//                 repairMoney = repairMoney / 100 * 100;
//             }
//             else if (repairMoney > 100)
//             {
//                 repairMoney = repairMoney / 10 * 10;
//             }
//             item.SetProp(EItemValue.RepairMoney, repairMoney);
//         }

        /// <summary>
        /// 更新 佩戴/使用 需求
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void UpdateRequire(Item item)
        {
            int mountsLevel = item.GetProp(EItemValue.MountsLevel);

            item.SetProp(EItemValue.RequireLevel, mountsLevel * 2 + item.ConfigData.ReqLvl);
        }

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

            int mountsLevel = item.GetProp(EItemValue.MountsLevel);
            int agi = gamePlayer.GetNumerial(E_GameProperty.Property_Agility);
            int Advanced = item.GetProp(EItemValue.Advanced);
            int ValueDefense = agi / 20 + 5 + mountsLevel * 2 + Advanced*10;

            gamePlayer.AddEquipProperty(E_GameProperty.Defense, ValueDefense);
            gamePlayer.AddEquipProperty(E_GameProperty.DamageAbsPct_Mounts, (mountsLevel + 10) / 2);

        }
    }
}
