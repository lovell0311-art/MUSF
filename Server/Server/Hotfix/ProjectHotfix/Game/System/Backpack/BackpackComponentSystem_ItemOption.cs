using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static partial class BackpackComponentSystem
    {
        /// <summary>
        /// 消耗物品，会主动判断物品是否消耗完并移除背包，消耗完的物品会从数据库销毁
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="itemUID"></param>
        /// <param name="log">使用原因</param>
        /// <param name="useCount"></param>
        /// <returns>true-正常消耗完毕 false-物品不存在或数量不足</returns>
        public static bool UseItem(this BackpackComponent b_Component, long itemUID, string log, int useCount = 1)
        {
            if (b_Component.mItemDict.TryGetValue(itemUID, out Item item))
            {
                int curCount = item.GetProp(EItemValue.Quantity);
                if (curCount > useCount)
                {
                    //数量足够，不需销毁
                    b_Component.SetItemQuantity(item, curCount - useCount);
                    return true;
                }
                else if (curCount == useCount)
                {
                    b_Component.DeleteItem(item, log);
                    return true;
                }
                return false;
            }
            else return false;
        }


        public static bool UseItem(this BackpackComponent self, Item item, string log, int useCount = 1)
        {
            if (self.mItemDict.TryGetValue(item.ItemUID, out Item item2))
            {
                if(item != item2)
                    throw new Exception($"进程中出现两个相同的物品，item.ItemUID={item.ItemUID},UserId={self.mPlayer.UserId},GameUserId={self.mPlayer.GameUserId}");

                int curCount = item.GetProp(EItemValue.Quantity);
                if (curCount > useCount)
                {
                    //数量足够，不需销毁
                    self.SetItemQuantity(item, curCount - useCount);
                    return true;
                }
                else if (curCount == useCount)
                {
                    self.DeleteItem(item, log);
                    return true;
                }
                return false;
            }
            else return false;
        }

        /// <summary>
        /// 通过配置id使用物品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="configId"></param>
        /// <param name="log"></param>
        /// <param name="useCount"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool UseItemByConfigId(this BackpackComponent self, int configId, string log, int useCount = 1, Func<Item, bool> predicate = null)
        {
            if (!self._ConfigId2Item.TryGetValue(configId, out var dict))
            {
                return false;
            }
            using ListComponent<Item> itemList = ListComponent<Item>.Create();
            int itemCount = 0;
            if (predicate == null)
            {
                foreach (var item in dict.Values)
                {
                    itemCount += item.GetProp(EItemValue.Quantity);
                    itemList.Add(item);
                    if (itemCount >= useCount) break;
                }
            }
            else
            {
                foreach (var item in dict.Values)
                {
                    if (!predicate(item)) continue;
                    itemCount += item.GetProp(EItemValue.Quantity);
                    itemList.Add(item);
                    if (itemCount >= useCount) break;
                }
            }
            if (itemCount < useCount)
            {
                // 数量不够
                return false;
            }
            // TODO 开始扣除物品
            itemCount = useCount;
            for (int i = 0; i < itemList.Count; i++)
            {
                Item item = itemList[i];
                int curCount = item.GetProp(EItemValue.Quantity);
                if (itemCount >= curCount)
                {
                    itemCount -= curCount;
                    self.DeleteItem(item, log);
                }
                else
                {
                    // 最后一个物品
                    self.SetItemQuantity(item, curCount - itemCount);
                }
            }
            return true;
        }

        /// <summary>
        /// 设置物品数量
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public static void SetItemQuantity(this BackpackComponent self,Item item,int count)
        {
            if(!self.mItemDict.TryGetValue(item.ItemUID,out Item item2))
            {
                Log.Error($"背包中不存在的物品，item.ItemUID={item.ItemUID},UserId={self.mPlayer.UserId},GameUserId={self.mPlayer.GameUserId}");
                return;
            }
            if(item != item2)
            {
                throw new Exception($"进程中出现两个相同的物品，item.ItemUID={item.ItemUID},UserId={self.mPlayer.UserId},GameUserId={self.mPlayer.GameUserId}");
            }
            int oldCount = item.GetProp(EItemValue.Quantity);
            item.SetProp(EItemValue.Quantity, count);
            item.OnlyUpdateMoney();
            item.OnlySaveDB();

            Item.G2CItemsPropChange_notice.PropList.Clear();
            Item.G2CItemsPropChange_notice.PropList.Add(new Struct_Property()
            {
                PropID = (int)EItemValue.Quantity,
                Value = item.GetProp(EItemValue.Quantity)
            });
            Item.G2CItemsPropChange_notice.PropList.Add(new Struct_Property()
            {
                PropID = (int)EItemValue.BuyMoney,
                Value = item.GetProp(EItemValue.BuyMoney)
            });
            Item.G2CItemsPropChange_notice.PropList.Add(new Struct_Property()
            {
                PropID = (int)EItemValue.SellMoney,
                Value = item.GetProp(EItemValue.SellMoney)
            });
            Item.G2CItemsPropChange_notice.PropList.Add(new Struct_Property()
            {
                PropID = (int)EItemValue.RepairMoney,
                Value = item.GetProp(EItemValue.RepairMoney)
            });

            Item.G2CItemsPropChange_notice.ItemUUID = item.ItemUID;
            Item.G2CItemsPropChange_notice.GameUserId = item.data.GameUserId;
            self.mPlayer.Send(Item.G2CItemsPropChange_notice);

            // 发布 ItemCountChangeInBackpack 事件
            ETModel.EventType.ItemCountChangeInBackpack.Instance.player = self.Parent;
            ETModel.EventType.ItemCountChangeInBackpack.Instance.item = item;
            ETModel.EventType.ItemCountChangeInBackpack.Instance.oldCount = oldCount;
            Root.EventSystem.OnRun("ItemCountChangeInBackpack", ETModel.EventType.ItemCountChangeInBackpack.Instance);
        }


    }
}
