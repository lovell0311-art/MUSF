using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ETHotfix
{
    /// <summary>
    /// 仓库组件
    /// </summary>
    public static partial class WarehouseComponentSystem
    {
        private static DBWarehouseItem InitDB(this WarehouseComponent b_Component)
        {
            DBWarehouseItem data = ComponentFactory.Create<DBWarehouseItem>();
            data.Id = IdGeneraterNew.Instance.GenerateUnitId(b_Component.mPlayer.GameAreaId);
            data.UserId = b_Component.mPlayer.UserId;
            data.PageHeight = b_Component.mItemBox.GetPageHeight();
            data.Capacity = b_Component.mItemBox.GetItemBoxList().Capacity;
            data.Width = b_Component.mItemBox.GetWidth();
            return data;
        }

        public static async Task<bool> Init(this WarehouseComponent b_Component)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.mPlayer.AddCustomComponent<DataCacheManageComponent>();
            C_DataCache<DBWarehouseItem> dataCache = mDataCacheComponent.Get<DBWarehouseItem>();
            DBProxyComponent dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.mPlayer.GameAreaId);
            if (dataCache == null)
            {
                dataCache = await mDataCacheComponent.Add<DBWarehouseItem>(dBProxy2, p => p.UserId == b_Component.mPlayer.UserId);
            }
            List<DBWarehouseItem> allDBWarehouseItem = dataCache.DataQuery(p => p.UserId == b_Component.mPlayer.UserId);
            DBWarehouseItem data = null;
            if(allDBWarehouseItem.Count != 0)
            {
                data = allDBWarehouseItem[0];
            }
            b_Component.InitItemBox(data);
            if (data == null)
            {
                data = b_Component.InitDB();
            }
            b_Component.Warehouse_DB = data;
            b_Component.mItemDict.Clear();

            C_DataCache<DBItemData> itemDataCache = await mDataCacheComponent.Append<DBItemData>(dBProxy2, p => p.UserId == b_Component.mPlayer.UserId && p.InComponent == EItemInComponent.Warehouse && p.IsDispose == 0);
            // 取出装备栏中的所有物品
            List<DBItemData> allDBItemData = itemDataCache.DataQuery(p => p.UserId == b_Component.mPlayer.UserId && p.InComponent == EItemInComponent.Warehouse && p.IsDispose == 0);
            for (int i = 0, count = allDBItemData.Count; i < count; i++)
            {
                DBItemData item = allDBItemData[i];
                Item curItem = ItemFactory.CreateFormDB(item.Id, b_Component.mPlayer);
                if (curItem == null)
                {
                    // TODO 存档损坏
                    // 需要将玩家踢下线
                    throw new Exception($"仓库存档损坏，无法生成物品 uid={item.Id} gameUserId={b_Component.mPlayer.GameUserId}");
                }
                //将Item对象添加到mItemBox里
                bool addResult = b_Component.AddItem(curItem, curItem.data.posX, curItem.data.posY,"", false);
                if (!addResult)
                {
                    // 这个物品先不加载，啥时位置空了，再加载
                    Log.Error($"仓库存档损坏，物品重叠 uid={curItem.ItemUID} posX={curItem.data.posX} posY={curItem.data.posY} gameUserId={b_Component.mPlayer.GameUserId}");
                    // 数据加载完成后，将添加失败的物品，以邮件形式还给玩家
                    curItem.data.InComponent = EItemInComponent.Lost;
                    curItem.data.posX = 0;
                    curItem.data.posY = 0;
                    curItem.data.posId = 0;
                    curItem.data.GameUserId = b_Component.mPlayer.GameUserId;
                    curItem.OnlySaveDB();
                    curItem.Dispose();
                }
            }
            return true;
        }

        /// <summary>
        /// 初始化玩家背包
        /// </summary>
        /// <param name="b_Component"></param>
        private static void InitItemBox(this WarehouseComponent b_Component, DBWarehouseItem data)
        {
            //Log.Debug("m_Player.UserId: " + b_Component.mPlayer.GameUserId);
            //Log.Debug("m_Player.name: " + b_Component.mPlayer.GetComponent<GamePlayer>().Data.NickName);
            b_Component.mItemBox = new ItemsBoxStatus();
            if (data == null)
            {
                b_Component.mItemBox.Init(WarehouseComponent.I_WarehouseWidth, WarehouseComponent.I_WarehouseWidth * WarehouseComponent.I_WarehouseHigh);
            }
            else {
                b_Component.mItemBox.Init(data.Width, data.Capacity);
            }
        }

        /// <summary>
        /// 不指定位置添加到背包，一般用于捡取物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool AddItem(this WarehouseComponent b_Component, Item item,string log)
        {
            int posX = 0, posY = 0;
            if (b_Component.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y,ref posX,ref posY))
            {
                return b_Component.AddItem(item, posX, posY,log);
            }
            return false;
        }

        /// <summary>
        /// 添加物品，并加入数据库
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="addDB">是否添加并保存到数据库，初次读取数据添加到玩家背包时不需要保存数据库</param>
        /// <returns></returns>
        public static bool AddItem(this WarehouseComponent b_Component,Item item,int posX,int posY,string log,bool addDB = true)
        {
            if (b_Component.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, posX, posY))
            {
                if (!b_Component.mItemDict.ContainsKey(item.ItemUID))
                {
                    b_Component.mItemDict.Add(item.ItemUID, item);
                }
                item.data.posX = posX;
                item.data.posY = posY;
                item.data.InComponent = EItemInComponent.Warehouse;
                item.data.GameUserId = b_Component.mPlayer.GameUserId;
                item.data.UserId = b_Component.mPlayer.UserId;
                if (addDB)
                {
                    item.SaveDB(b_Component.mPlayer);
                    //推送给玩家
                    var allItems = new Google.Protobuf.Collections.RepeatedField<Struct_ItemInBackpack_Status>();
                    allItems.Add(b_Component.Item2BackpackStatusData(item));
                    b_Component.mPlayer.Send(new G2C_AddWarehouseItem_notice()
                    {
                        AllItems = allItems
                    });
                    item.SendAllPropertyData(b_Component.mPlayer);
                    item.SendAllEntryAttr(b_Component.mPlayer);

                    Log.PLog("Warehouse", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品进入仓库({log}) {item.ToLogString()}");
                    return true;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 可以添加物品，添加前进行判断，如果中间使用 await。需要将要添加的格子锁住，防止其他代码添加物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public static bool CanAddItem(this WarehouseComponent b_Component, Item item, int posX, int posY)
        {
            if (b_Component.mItemDict.ContainsKey(item.ItemUID))
            {
                // 背包中有这个物品了
                return false;
            }
            if (!b_Component.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, posX, posY))
            {
                // 位置放不下这个物品
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="dbData"></param>
        /// <returns></returns>
        public static Item RemoveItem(this WarehouseComponent b_Component,Item itemData,string log)
        {
            long itemUID = itemData.ItemUID;
            if (b_Component.mItemDict.ContainsKey(itemUID))
            {
                //注：Item暂不做处理，有可能进入仓库或地面
                b_Component.mItemBox.RemoveItem(itemData.ConfigData.X, itemData.ConfigData.Y, itemData.data.posX, itemData.data.posY);
                b_Component.mItemDict.Remove(itemUID);

                //数据库处理
                itemData.data.posX = 0;
                itemData.data.posY = 0;
                itemData.data.InComponent = EItemInComponent.None;
                itemData.OnlySaveDB();
                Log.PLog("Warehouse", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品离开仓库({log}) {itemData.ToLogString()}");

                //发送背包物品离开推送给玩家
                b_Component.mPlayer.Send(new G2C_DelWarehouseItem_notice() {
                    GameUserId = b_Component.mPlayer.GameUserId,
                    LeaveItemUUID = itemUID
                });

                return itemData;
            }
            else {
                Log.Debug("Error:找不到ItemUID，无法删除：" + itemData.ItemUID);
                return null;
            }
        }

        /// <summary>
        /// 移动物品,修改位置并推送给玩家
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="dbData"></param>
        /// <returns></returns>
        public static bool MoveItem(this WarehouseComponent b_Component, Item dbData,int targetPosX,int targetPosY)
        {
            if (b_Component.mItemDict.ContainsKey(dbData.ItemUID))
            {
                Item item = b_Component.mItemDict[dbData.ItemUID];
                b_Component.mItemBox.RemoveItem(item.ConfigData.X, item.ConfigData.Y, item.data.posX, item.data.posY);
                if (b_Component.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, targetPosX, targetPosY))
                {
                    //数据库处理
                    item.data.posX = targetPosX;
                    item.data.posY = targetPosY;
                    item.OnlySaveDB();
                }
                else {
                    //若无法添加则还原移除的状态
                    Log.Debug("移动物品失败，还原被移动的状态:" + dbData.ItemUID);
                    b_Component.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, item.data.posX, item.data.posY);
                    return false;
                }

                //发送推送给玩家
                b_Component.mPlayer.Send(new G2C_MoveWarehouseItem_notice() {
                    GameUserId = b_Component.mPlayer.GameUserId,
                    ItemUUID = item.ItemUID,
                    PosInBackpackX = item.data.posX,
                    PosInBackpackY = item.data.posY
                });

                return true;
            }
            else
            {
                Log.Debug("Error:找不到ItemUID，无法删除：" + dbData.ItemUID);
                return false;
            }
        }

        /// <summary>
        /// 推送背包中的所有物品，一般在选择角色进入场景后调用
        /// </summary>
        /// <param name="b_component"></param>
        public static void NotifyAllItem(this WarehouseComponent b_Component)
        {
            G2C_AddWarehouseItem_notice sendData = new G2C_AddWarehouseItem_notice();
            //sendData.AllItems = new Google.Protobuf.Collections.RepeatedField<Struct_ItemInBackpack_Status>();
            foreach (var item in b_Component.mItemDict)
            {
                sendData.AllItems.Add(b_Component.Item2BackpackStatusData(item.Value));
            }
            if (sendData.AllItems.count > 0)
            {
                b_Component.mPlayer.Send(sendData);
            }
            foreach (var item in b_Component.mItemDict)
            {
                item.Value.SendAllPropertyData(b_Component.mPlayer);
                item.Value.SendAllEntryAttr(b_Component.mPlayer);
            }
        }

        /// <summary>
        /// 扩展仓库
        /// </summary>
        /// <param name="b_Component"></param>
        public static int ExtendedWarehouse(this WarehouseComponent b_Component)
        {
            //添加两行
            if (b_Component.Warehouse_DB.Capacity < WarehouseComponent.I_MaxExtenedCapacity)
            {
                b_Component.mItemBox.IncreaseSize(b_Component.mItemBox.GetWidth() * 2);
                b_Component.Warehouse_DB.Capacity = b_Component.mItemBox.GetItemBoxList().Capacity;
                b_Component.SaveDB();
                //通知玩家仓库扩张
                b_Component.mPlayer.Send(new G2C_WarehouseExtension_notice() { 
                    Capacity = b_Component.Warehouse_DB.Capacity
                });
                return ErrorCodeHotfix.ERR_Success;
            }
            else {
                return 99;//error:扩充失败，扩充量到上限
            }
           
            
        }

        /// <summary>
        /// 更新背包数据到数据库
        /// </summary>
        /// <param name="b_component"></param>
        public static void SaveDB(this WarehouseComponent b_Component)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Component.mPlayer.GameAreaId);
            mWriteDataComponent.Save(b_Component.Warehouse_DB, dBProxy).Coroutine();

            //DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            //var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.mPlayer.GameAreaId);
            //if (ItemFactory.GetData(item.ItemUID, player) != null)
            //{
            //    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(item.data.GameAreaId);
            //    mWriteDataComponent.Save(item.data, dBProxy).Coroutine();
            //}
            //else
            //{
            //    b_Component.mPlayer.AddCustomComponent<DataCacheManageComponent>().Get<DBWarehouseItem>().DataAdd(item.data);
            //    dBProxy.SaveAsync(b_Component.Warehouse_DB);
            //}
        }

        /// <summary>
        /// 背包是否拥有物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="itemUID">物品UID</param>
        /// <returns></returns>
        public static bool HaveItem(this WarehouseComponent b_Component,long itemUID)
        {
            return b_Component.mItemDict.ContainsKey(itemUID);
        }

        public static Item GetItemByUID(this WarehouseComponent b_Component, long itemUID)
        {
            if (b_Component.HaveItem(itemUID))
            {
                return b_Component.mItemDict[itemUID];
            }
            return null;
        }

        public static Struct_ItemInBackpack_Status Item2BackpackStatusData(this WarehouseComponent b_component,Item itemData)
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

        public static void Destroy(this WarehouseComponent b_Component)
        {
            b_Component.mItemBox = null;
            b_Component.mItemDict = null;
        }


        #region Coin
        /// <summary>
        /// 增加金币(仓库)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <param name="log"></param>
        public static void IncreaseCoin(this WarehouseComponent self,int value,string log)
        {
#pragma warning disable CS0618
            long oldCoin = self.Warehouse_DB.Coin;
            self.Warehouse_DB.Coin += value;
            Log.PLog("Warehouse", $"a:{self.mPlayer.UserId} r:{self.mPlayer.GameUserId} 增加金币(仓库)({log}) {oldCoin} + {value} = {self.Warehouse_DB.Coin}");
            self.SaveDB();
            WarehouseComponent.g2C_WarehouseGoldChange_notice.GameUserId = self.mPlayer.GameUserId;
            WarehouseComponent.g2C_WarehouseGoldChange_notice.Gold = self.Warehouse_DB.Coin;
            self.mPlayer.Send(WarehouseComponent.g2C_WarehouseGoldChange_notice);
#pragma warning restore CS0618
        }

        /// <summary>
        /// 可以扣除金币(仓库)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CanDeductCoin(this WarehouseComponent self, int value)
        {
#pragma warning disable CS0618
            if (value > self.Warehouse_DB.Coin) return false;
#pragma warning restore CS0618
            return true;
        }

        /// <summary>
        /// 扣除金币(仓库)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        /// <param name="log"></param>
        public static bool DeductCoin(this WarehouseComponent self, int value, string log)
        {
            if (!self.CanDeductCoin(value)) return false;
#pragma warning disable CS0618
            long oldCoin = self.Warehouse_DB.Coin;
            self.Warehouse_DB.Coin -= value;
            Log.PLog("Warehouse", $"a:{self.mPlayer.UserId} r:{self.mPlayer.GameUserId} 扣除金币(仓库)({log}) {oldCoin} - {value} = {self.Warehouse_DB.Coin}");
            self.SaveDB();
            WarehouseComponent.g2C_WarehouseGoldChange_notice.GameUserId = self.mPlayer.GameUserId;
            WarehouseComponent.g2C_WarehouseGoldChange_notice.Gold = self.Warehouse_DB.Coin;
            self.mPlayer.Send(WarehouseComponent.g2C_WarehouseGoldChange_notice);
#pragma warning restore CS0618
            return true;
        }
        #endregion

    }
}
