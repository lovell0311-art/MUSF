using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomFrameWork.Component;
using static ETHotfix.MapManageComponentSystem;
using CustomFrameWork;

namespace ETHotfix
{
    public static partial class NpcShopComponentSystem
    {
        /// <summary>
        /// 初始化，并根据配置表生成商品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="parent"></param>
        public static void InitShop(this NpcShopComponent self, GameNpc parent)
        {
            self.Parent = parent;
            self.itemBox = new ItemsBoxStatus();
            self.mItemDict.Clear();
            self.mShopConfigDict.Clear();
            self.itemBox.Init(NpcShopComponent.I_WarehouseWidth, NpcShopComponent.I_WarehouseWidth * NpcShopComponent.I_WarehouseHigh);
            var curConfig = parent.Config;
            var npcShopConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Npc_Shop_InfoConfigJson>().JsonDic;
            for (int i = 0; i < curConfig.EquipDataList.Count; i++)
            {
                int curIndex = curConfig.EquipDataList[i];
                if (npcShopConfig.TryGetValue(curIndex, out var shopConfig))
                {
                    Item curShopItem = self.CreateShopItem(shopConfig,0);
                    if (curShopItem != null)
                    {
                        // 设置商品价格
                        curShopItem.SetProp(EItemValue.BuyMoney, shopConfig.Price);

                        var itemConfig = curShopItem.ConfigData;
                        int posX = 0, posY = 0;
                        //判断物品能否装到商人背包去
                        if (self.itemBox.AddItem(itemConfig.X, itemConfig.Y,ref posX,ref posY))
                        {
                            self.mItemDict[curShopItem.ItemUID] = curShopItem;
                            self.mShopConfigDict[curShopItem.ItemUID] = shopConfig;
                            curShopItem.data.posX = posX;
                            curShopItem.data.posY = posY;
                        }
                        else {
                            curShopItem.Dispose();
                            Log.Warning($"配置的物品装不到商人背包里，已抛弃 parent.Config.Id={parent.Config.Id}");
                        }
                    }
                    else {
                        Log.Error($"商品配置表错误，未找到商品ConfigID配置 parent.Config.Id={parent.Config.Id},curIndex={curIndex},shopConfig.itemID={shopConfig.itemID}");
                    }
                }
                else
                {
                    Log.Error($"商品配置表错误，未找到对应商品ID parent.Config.Id={parent.Config.Id},curIndex={curIndex}");
                }
            }
        }
        public static Struct_ItemInBackpack_Status Item2BackpackStatusData(this NpcShopComponent b_component, Item itemData)
        {
            return new Struct_ItemInBackpack_Status()
            {
                GameUserId = b_component.Parent.Id,
                ItemUID = itemData.ItemUID,
                ConfigID = itemData.ConfigID,
                Type = (int)itemData.Type,
                PosInBackpackX = itemData.data.posX,
                PosInBackpackY = itemData.data.posY,
                Width = itemData.ConfigData.X,
                Height = itemData.ConfigData.Y,
                ItemLevel = itemData.GetProp(EItemValue.Level),
                Quantity = itemData.GetProp(EItemValue.Quantity)
        };
        }

        public static Item CreateShopItem(this NpcShopComponent b_component, Npc_Shop_InfoConfig shopConfig, int zone)
        {
            Item curShopItem = ItemFactory.TryCreate(shopConfig.itemID, zone, shopConfig.Quantity > 1 ? shopConfig.Quantity : 1);
            if (curShopItem == null)
            {
                return null;
            }

            //添加商品属性
            curShopItem.SetProp(EItemValue.OptValue, shopConfig.OptValue);
            curShopItem.SetProp(EItemValue.OptLevel, shopConfig.OptLevel);
            curShopItem.SetProp(EItemValue.OrecycledID, shopConfig.OptionRebirth);
            curShopItem.SetProp(EItemValue.OrecycledLevel, shopConfig.OptionRebirthLevel);
            curShopItem.SetProp(EItemValue.SetId, shopConfig.SetID);
            curShopItem.SetProp(EItemValue.Level, shopConfig.Level);
            curShopItem.SetProp(EItemValue.SkillId, curShopItem.ConfigData.Skill);
            curShopItem.UpdateProp();
            return curShopItem;
        }
    }

}