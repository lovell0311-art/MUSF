using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static class ItemSetSystem
    {
        /// <summary>
        /// 添加物品，不对属性词条进行更新
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item">需要添加的物品</param>
        public static void AddItemNotUpdate(this ItemSet self, Item item)
        {
            if (self.ConfigId != item.GetProp(EItemValue.SetId))
            {
                // 套装id不一致
                return;
            }
            if (self.HaveItem(item))
            {
                // 已经添加过这个物品了
                return;
            }
            self.__HaveItem.Add(item.ItemUID);
            if (self.ItemId2Count.ContainsKey(item.ConfigID))
            {
                self.ItemId2Count[item.ConfigID] += 1;
            }
            else
            {
                self.ItemId2Count.Add(item.ConfigID, 1);
                self.NeedUpdate = true;
            }
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item">需要添加的物品</param>
        public static void AddItem(this ItemSet self,Item item)
        {
            self.AddItemNotUpdate(item);
            if(self.NeedUpdate)
            {
                self.UpdateAttrEntry();
            }
        }

        /// <summary>
        /// 移除物品，不对属性词条进行更新
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item">需要移除的物品</param>
        public static void RemoveItemNotUpdate(this ItemSet self, Item item)
        {
            if (self.ConfigId != item.GetProp(EItemValue.SetId))
            {
                // 套装id不一致
                return;
            }

            if (!self.HaveItem(item))
            {
                // 没有这个物品，无法移除
                return;
            }
            self.__HaveItem.Remove(item.ItemUID);
            if (self.ItemId2Count.TryGetValue(item.ConfigID, out var count))
            {
                if (count <= 1)
                {
                    self.ItemId2Count.Remove(item.ConfigID);
                    self.NeedUpdate = true;
                }
                else
                {
                    self.ItemId2Count[item.ConfigID] = (count - 1);
                }
            }
            else
            {
                Log.Error("ItemId2Count 结构损坏");
            }
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item">需要移除的物品</param>
        public static void RemoveItem(this ItemSet self,Item item)
        {
            self.RemoveItemNotUpdate(item);
            if(self.NeedUpdate)
            {
                self.UpdateAttrEntry();
            }
        }

        /// <summary>
        /// 更新属性词条
        /// </summary>
        /// <param name="self"></param>
        public static void UpdateAttrEntry(this ItemSet self)
        {
            self.NeedUpdate = false;
            self.ValidAttrEntryId.Clear();
            if(!self.IsValid())
            {
                return;
            }
            #region AddEntry
            List<int>[] attrId = new List<int>[] { 
                null,
                null,
                self.Config.AttrId2,
                self.Config.AttrId3,
                self.Config.AttrId4,
                self.Config.AttrId5,
                self.Config.AttrId6,
                self.Config.AttrId7,
                self.Config.AttrId8,
                self.Config.AttrId9,
                self.Config.AttrId10
            };
            for(int i = 2;i <= self.ItemId2Count.Count;++i)
            {
                for (int j = 0, ct = attrId[i].Count; j < ct; ++j)
                {
                    self.ValidAttrEntryId.Add(attrId[i][j]);
                }
            }
            #endregion

        }

        #region Core
        public static bool HaveItem(this ItemSet self,Item item)
        {
            return self.__HaveItem.Contains(item.ItemUID);
        }

        /// <summary>
        /// 有效的套装
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsValid(this ItemSet self)
        {
            return self.ItemId2Count.Count > 1;
        }

        #endregion
    }
}
