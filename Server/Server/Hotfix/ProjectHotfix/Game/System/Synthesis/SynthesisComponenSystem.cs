using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ETHotfix
{
    /// <summary>
    /// 背包组件
    /// </summary>
    public static partial class SynthesisComponentSystem
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="b_Component"></param>
        /// <returns></returns>
        public static void Init(this SynthesisComponent b_Component)
        {
            b_Component.mItemDict.Clear();
            b_Component.InitItemBox();
        }

        private static void InitItemBox(this SynthesisComponent b_Component)
        {
            //Log.Debug("m_Player.UserId: " + b_Component.mPlayer.GameUserId);
            //Log.Debug("m_Player.name: " + b_Component.mPlayer.GetComponent<GamePlayer>().Data.NickName);
            b_Component.mItemBox = new ItemsBoxStatus();
            b_Component.mItemBox.Init(SynthesisComponent.I_PackageWidth, SynthesisComponent.I_PackageWidth * SynthesisComponent.I_PackageHigh);
        }
        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param> 
        public static void AddItem(this SynthesisComponent b_Component, Item item)
        {
            if (!b_Component.mItemDict.ContainsKey(item.ItemUID))
            {
                b_Component.mItemDict.Add(item.ItemUID, item);
                //若不在ItemBox里，标记为-1，否则后面会改pos
                item.data.posX = -1;
                item.data.posY = -1;
            }
        }

        /// <summary>
        /// 添加合成结果物品，合成成功后调用
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool AddResultItem(this SynthesisComponent b_Component, Item item)
        {
            int posX = 0, posY = 0;
            if (b_Component.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, ref posX, ref posY))
            {
                return b_Component.AddResultItem(item, posX, posY);
            }
            else
                Log.Warning($"需要加入合成空间的物品:{item.ItemUID},由于空间不足放入失败");
            return false;
        }
        /// <summary>
        /// 添加合成结果物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool AddResultItem(this SynthesisComponent b_Component, Item item, int posX, int posY)
        {
            if (b_Component.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, posX, posY))
            {
                b_Component.AddItem(item);
                item.data.posX = posX;
                item.data.posY = posY;
                item.data.GameUserId = b_Component.mPlayer.GameUserId;
                return true;
            }
            return false;
        }

        public static Item DeleteItem(this SynthesisComponent b_Component, long itemUID)
        {
            Item curItem = b_Component.GetItemByUID(itemUID);
            if (curItem != null)
            {
                return b_Component.DeleteItem(curItem);
            }
            return null;
        }
        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="dbData"></param>
        /// <returns></returns>
        public static Item DeleteItem(this SynthesisComponent b_Component, Item itemData)
        {
            if (itemData.data.posX >= 0)
            {
                b_Component.mItemBox.RemoveItem(itemData.ConfigData.X, itemData.ConfigData.Y, itemData.data.posX, itemData.data.posY);
            }
            long itemUID = itemData.ItemUID;
            if (b_Component.mItemDict.ContainsKey(itemUID))
            {
                b_Component.mItemDict.Remove(itemUID);
                return itemData;
            }
            else
            {
                Log.Debug("Error:找不到ItemUID，无法删除：" + itemData.ItemUID);
                return null;
            }
        }


        /// <summary>
        /// 是否拥有物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="itemUID">物品UID</param>
        /// <returns></returns>
        public static bool HaveItem(this SynthesisComponent b_Component, long itemUID)
        {
            return b_Component.mItemDict.ContainsKey(itemUID);
        }

        public static Item GetItemByUID(this SynthesisComponent b_Component, long itemUID)
        {
            if (b_Component.HaveItem(itemUID))
            {
                return b_Component.mItemDict[itemUID];
            }
            return null;
        }

        public static Struct_ItemInBackpack_Status Item2BackpackStatusData(this SynthesisComponent b_component, Item itemData)
        {
            return new Struct_ItemInBackpack_Status()
            {
                GameUserId = b_component.mPlayer.GameUserId,
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

        /// <summary>
        /// 推送全部物品属性
        /// </summary>
        /// <param name="b_component"></param>
        public static void SendAllItemProp(this SynthesisComponent b_Component)
        {
            foreach (Item item in b_Component.mItemDict.Values)
            {
                item.SendAllPropertyData(b_Component.Parent);
            }
        }


        public static void Dispose(this SynthesisComponent b_Component)
        {

        }

        /// <summary>
        /// 检查金币数是否达到要求
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="needGold"></param>
        /// <returns></returns>
        public static bool CheckGold(this SynthesisComponent b_Component, int needGold)
        {
            if (needGold == 0) { return true; }
            return needGold <= b_Component.mGamePlayer.Data.GoldCoin;
        }

        /// <summary>
        /// 消耗金币
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="needGold"></param>
        /// <returns></returns>
        public static bool UseGold(this SynthesisComponent b_Component, int needGold)
        {
            if (needGold == 0) { return true; }
            if (b_Component.CheckGold(needGold))
            {
                //b_Component.mGamePlayer.Data.GoldCoin -= needGold;
                b_Component.mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, -needGold, "SynthesisComponent");

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Component.mPlayer.GameAreaId);
                mWriteDataComponent.Save(b_Component.mGamePlayer.Data, dBProxy).Coroutine();

                G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
                mGoldCoinData.Key = (int)E_GameProperty.GoldCoin;
                mGoldCoinData.Value = b_Component.mGamePlayer.Data.GoldCoin;
                mChangeValueMessage.Info.Add(mGoldCoinData);

                b_Component.mPlayer.Send(mChangeValueMessage);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 根据合成表检查物品ID和数量是否正确
        /// </summary>
        /// <param name="needItemDict"></param>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public static bool CheckItem(this SynthesisComponent b_Component, Dictionary<int, int> needItemDict, List<Item> itemList)
        {
            Dictionary<int, int> needItemDictClone = new Dictionary<int, int>(needItemDict);
            for (int i = 0; i < itemList.Count; i++)
            {
                if (needItemDictClone.ContainsKey(itemList[i].ConfigID))
                {
                    needItemDictClone[itemList[i].ConfigID] -= itemList[i].GetProp(EItemValue.Quantity);
                }
            }

            foreach (var item in needItemDictClone)
            {
                if (item.Key == (int)EItemStrengthen.MAYA_GEMS)
                {
                    if (item.Value > 0)
                    {
                        return false;
                    }
                }
                else if (item.Value != 0)
                {
                    return false;
                }

            }
            return true;
        }

        /// <summary>
        /// 合成成功后消耗所有材料
        /// 注：只消耗needItemDict里面含有的材料，needItemDict一般从合成配置表中获取
        /// 从itemList里面删除，所以不需要消耗的材料要从itemList里提前删掉
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="needItemDict"></param>
        /// <param name="itemList"></param>
        /// <param name="log">合成方法</param>
        /// <returns></returns>
        public static bool UseAllItem(this SynthesisComponent b_Component, Dictionary<int, int> needItemDict, List<Item> itemList, string log)
        {
            Dictionary<int, int> needItemDictClone = new Dictionary<int, int>(needItemDict);
            for (int i = (itemList.Count - 1); i >= 0; i--)
            {
                if (needItemDictClone.TryGetValue(itemList[i].ConfigID, out int useCount))
                {
                    int curItemCount = itemList[i].GetProp(EItemValue.Quantity);
                    if (useCount > curItemCount)
                    {
                        needItemDictClone[itemList[i].ConfigID] -= itemList[i].GetProp(EItemValue.Quantity);
                        b_Component.UseItem(itemList[i].ItemUID, log, curItemCount);
                    }
                    else
                    {
                        needItemDictClone[itemList[i].ConfigID] = 0;
                        b_Component.UseItem(itemList[i].ItemUID, log, useCount);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 消耗物品，会主动判断物品是否消耗完并移除背包，消耗完的物品会从数据库销毁
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="itemUID"></param>
        /// <param name="log">使用原因</param>
        /// <param name="useCount"></param>
        /// <returns>true-正常消耗完毕 false-物品不存在或数量不足</returns>
        public static bool UseItem(this SynthesisComponent b_Component, long itemUID, string log, int useCount = 1)
        {
            if (b_Component.mItemDict.TryGetValue(itemUID, out Item item))
            {
                int curCount = item.GetProp(EItemValue.Quantity);
                if (curCount > useCount)
                {
                    //数量足够，不需销毁
                    item.SetProp(EItemValue.Quantity, curCount - useCount, b_Component.mPlayer);
                    item.OnlyUpdateMoney();
                    return true;
                }
                else if (curCount == useCount)
                {
                    b_Component.DeleteItem(item).DisposeDB(log);
                    return true;
                }
                return false;
            }
            else return false;
        }

        /// <summary>
        /// 将所有物品返回背包，合成成功以及玩家下线调用
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="log">原因</param>
        public static void BackAllItemToBackpack(this SynthesisComponent b_Component, string log)
        {
            BackpackComponent backpack = b_Component.mPlayer.GetCustomComponent<BackpackComponent>();
            foreach (var item in b_Component.mItemDict)
            {
                Item curItem = b_Component.DeleteItem(item.Value);
                if (!backpack.AddItem(curItem, log))
                {
                    //放不进背包，下次登录，通过邮件方式归还物品
                    curItem.data.InComponent = EItemInComponent.Lost;
                    curItem.data.posX = 0;
                    curItem.data.posY = 0;
                    curItem.data.posId = 0;
                    curItem.data.GameUserId = b_Component.mPlayer.GameUserId;
                    curItem.OnlySaveDB();
                    curItem.Dispose();
                }
            }
        }

        public static void PlayerDispose(this SynthesisComponent b_Component)
        {
            b_Component.BackAllItemToBackpack("玩家下线，物品归还到背包");
        }

    }
}
