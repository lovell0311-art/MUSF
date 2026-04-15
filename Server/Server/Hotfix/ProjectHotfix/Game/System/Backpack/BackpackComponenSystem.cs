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
using TencentCloud.Cynosdb.V20190107.Models;
using TencentCloud.Tsf.V20180326.Models;
using static ETModel.ItemsBoxStatus;
using ETModel.EventType;

namespace ETHotfix
{
    /// <summary>
    /// 背包组件
    /// </summary>
    public static partial class BackpackComponentSystem
    {
        /// <summary>
        /// 初始化背包数据，只有在新注册玩家进入游戏时才会执行这里
        /// </summary>
        /// <param name="b_Component"></param>
        /// <returns></returns>
        public async static Task<bool> Init(this BackpackComponent b_Component)
        {
            b_Component.InitItemBox();
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.mPlayer.GameAreaId);
            DataCacheManageComponent dataCacheManage = b_Component.mPlayer.GetCustomComponent<DataCacheManageComponent>();
            C_DataCache<DBItemData> itemDataCache = await dataCacheManage.Append<DBItemData>(dBProxy2, p => p.GameUserId == b_Component.mPlayer.GameUserId && (p.InComponent == EItemInComponent.Backpack|| p.InComponent == EItemInComponent.Mount) && p.IsDispose == 0);
            // 取出背包中的所有物品
            List<DBItemData> allDBItemData = itemDataCache.DataQuery(p => p.GameUserId == b_Component.mPlayer.GameUserId && (p.InComponent == EItemInComponent.Backpack || p.InComponent == EItemInComponent.Mount) && p.IsDispose == 0);
            for (int i = 0, count = allDBItemData.Count; i < count; i++)
            {
                DBItemData item = allDBItemData[i];
                Item curItem = ItemFactory.CreateFormDB(item.Id, b_Component.mPlayer);
                if (curItem == null)
                {
                    // TODO 存档损坏
                    // 需要将玩家踢下线
                    throw new Exception($"背包存档损坏，无法生成物品 uid={item.Id} gameUserId={b_Component.mPlayer.GameUserId}");
                }
                if (curItem.data.InComponent == EItemInComponent.Mount)
                {
                    curItem.SetProp(EItemValue.IsUsing, 0);
                    b_Component.AddMount(curItem);
                    continue;
                }
                //将Item对象添加到mItemBox里
                bool addResult = b_Component.AddItem(curItem, curItem.data.posX, curItem.data.posY, "", false, false);
                if (!addResult)
                {
                    Log.Error($"背包存档损坏，物品重叠 uid={item.Id} posX={curItem.data.posX} posY={curItem.data.posY} ");
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

        public static DBBackpackItem InitDB(this BackpackComponent b_Component)
        {
            DBBackpackItem data = ComponentFactory.Create<DBBackpackItem>();
            data.GameUserId = b_Component.mPlayer.GameUserId;
            data.PageHeight = b_Component.mItemBox.GetPageHeight();
            data.Capacity = b_Component.mItemBox.GetItemBoxList().Capacity;
            data.Width = b_Component.mItemBox.GetWidth();
            return data;
        }

        /// <summary>
        /// 初始化玩家背包
        /// </summary>
        /// <param name="b_Component"></param>
        private static void InitItemBox(this BackpackComponent b_Component)
        {
            //Log.Debug("m_Player.UserId: " + b_Component.mPlayer.GameUserId);
            //Log.Debug("m_Player.name: " + b_Component.mPlayer.GetComponent<GamePlayer>().Data.NickName);
            b_Component.mItemBox = new ItemsBoxStatus();
            b_Component.mItemBox.Init(BackpackComponent.I_PackageWidth, BackpackComponent.I_PackageWidth * BackpackComponent.I_PackageHigh);
        }

        /// <summary>
        /// 不指定位置添加到背包，一般用于捡取物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <param name="log">添加原因</param>
        /// <returns></returns>
        public static bool AddItem(this BackpackComponent b_Component, Item item, string log, bool autoStack = true)
        {
            int posX = 0, posY = 0;
            if (b_Component.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, ref posX, ref posY))
            {
                return b_Component.AddItem(item, posX, posY, log, autoStack);
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
        /// <param name="log">添加原因</param>
        /// <param name="addDB">是否添加并保存到数据库，初次读取数据添加到玩家背包时不需要保存数据库</param>
        /// <returns></returns>
        public static bool AddItem(this BackpackComponent b_Component, Item item, int posX, int posY, string log, bool autoStack = false, bool addDB = true)
        {
            if (addDB == true)
            {
                if (b_Component.Parent.OnlineStatus == EOnlineStatus.Ready)
                {
                    throw new Exception($"'BackpackComponent' 正在加载数据，外部禁止操作!({log}) r:{b_Component.Parent.GameUserId} ({item.ToLogString()})");
                }
            }
            if (b_Component.mItemDict.ContainsKey(item.ItemUID))
            {
                // 背包中有这个物品了
                return false;
            }
            if (addDB)
            {
                #region 物品进入背包路由规则
                BackpackRouterRuleCreateBuilder routerRule = Root.MainFactory.GetCustomComponent<BackpackRouterRuleCreateBuilder>();
                if (routerRule.RouterRuleDict.TryGetValue(item.ConfigID, out IBackpackRouterHandler handler))
                {
                    long instanceId = item.Id;
                    handler.Enter(b_Component, item, posX, posY, log);
                    if (instanceId != item.Id) return true;
                    if (item.IsDisposeable) return true;
                }
                #endregion
            }
            if (autoStack)
            {
                #region 物品自动堆叠
                if (item.ConfigData.StackSize > 1)
                {
                    int quantity = item.GetProp(EItemValue.Quantity);
                    int level = item.GetProp(EItemValue.Level);
                    int isBind = item.GetProp(EItemValue.IsBind);
                    var allItemDict = b_Component.GetAllItemByConfigID(item.ConfigID);
                    if (allItemDict != null)
                    {
                        foreach (Item item2 in allItemDict.Values)
                        {
                            if (item2.GetProp(EItemValue.Level) != level) continue;
                            if (item2.GetProp(EItemValue.IsBind) != isBind) continue;
                            if (quantity <= 0) break;
                            int oldQuantity = quantity;
                            int count = 0;
                            int quantity2 = item2.GetProp(EItemValue.Quantity);
                            if (item.ConfigData.StackSize <= quantity2) continue;
                            if ((item.ConfigData.StackSize - quantity2) <= quantity)
                            {
                                count = item.ConfigData.StackSize - quantity2;
                                quantity -= count;
                                item2.SetProp(EItemValue.Quantity, item.ConfigData.StackSize);
                                item2.UpdateProp();
                                item2.OnlySaveDB();
                                item2.SendAllPropertyData(b_Component.mPlayer);
                            }
                            else
                            {
                                count = quantity;
                                quantity -= count;
                                item2.SetProp(EItemValue.Quantity, quantity2 + count);
                                item2.UpdateProp();
                                item2.OnlySaveDB();
                                item2.SendAllPropertyData(b_Component.mPlayer);
                            }
                            Log.PLog("Backpack", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品进入背包，自动堆叠({log}) ({item.ToLogString()}) => ({item2.ToLogString()}) ({oldQuantity} - {count} = {quantity})");

                            // 发布 ItemCountChangeInBackpack 事件
                            ETModel.EventType.ItemCountChangeInBackpack.Instance.player = b_Component.Parent;
                            ETModel.EventType.ItemCountChangeInBackpack.Instance.item = item2;
                            ETModel.EventType.ItemCountChangeInBackpack.Instance.oldCount = quantity2;
                            Root.EventSystem.OnRun("ItemCountChangeInBackpack", ETModel.EventType.ItemCountChangeInBackpack.Instance);
                        }
                        item.SetProp(EItemValue.Quantity, quantity);
                        if (quantity <= 0)
                        {
                            if (item.NotSevedToDB)
                            {
                                item.Dispose();
                            }
                            else
                            {
                                item.DisposeDB("自动堆叠");
                            }
                            return true;
                        }
                        item.UpdateProp();
                    }
                }
                #endregion
            }

            if (b_Component.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, posX, posY))
            {
                b_Component.mItemDict.Add(item.ItemUID, item);
                // ConfigId 映射添加
                if (!b_Component._ConfigId2Item.TryGetValue(item.ConfigID, out var dict))
                {
                    dict = new Dictionary<long, Item>();
                    b_Component._ConfigId2Item.Add(item.ConfigID, dict);
                }
                dict.Add(item.ItemUID, item);

                item.data.InComponent = EItemInComponent.Backpack;
                item.data.posX = posX;
                item.data.posY = posY;
                item.data.GameUserId = b_Component.mPlayer.GameUserId;
                item.data.UserId = b_Component.mPlayer.UserId;
                if (addDB)
                {
                    //数据库操作
                    item.SaveDB(b_Component.mPlayer);

                    //推送给玩家
                    var allItems = new Google.Protobuf.Collections.RepeatedField<Struct_ItemInBackpack_Status>();
                    allItems.Add(b_Component.Item2BackpackStatusData(item));
                    b_Component.mPlayer.Send(new G2C_ItemsIntoBackpack_notice()
                    {
                        AllItems = allItems
                    });
                    item.SendAllPropertyData(b_Component.mPlayer);
                    item.SendAllEntryAttr(b_Component.mPlayer);
                    Log.PLog("Backpack", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品进入背包({log}) {item.ToLogString()}");

                    // 发布 BackpackAddItem 事件
                    ETModel.EventType.BackpackAddItem.Instance.player = b_Component.Parent;
                    ETModel.EventType.BackpackAddItem.Instance.item = item;
                    ETModel.EventType.BackpackAddItem.Instance.posX = posX;
                    ETModel.EventType.BackpackAddItem.Instance.posY = posY;
                    Root.EventSystem.OnRun("BackpackAddItem", ETModel.EventType.BackpackAddItem.Instance);

                    EquipmentRelatedSettings.Instance.player = b_Component.Parent;
                    EquipmentRelatedSettings.Instance.item = item;
                    EquipmentRelatedSettings.Instance.TitleCount = 0;
                    EquipmentRelatedSettings.Instance.ItemCount = 0;
                    Root.EventSystem.OnRun("EquipmentRelatedSettings", EquipmentRelatedSettings.Instance);
                    return true;
                }
                return true;
            }
            return false;
        }
        public static bool PutItOnTheShelf(this BackpackComponent b_Component, Item itemData, string log)
        {
            if (b_Component.Parent.OnlineStatus == EOnlineStatus.Ready)
            {
                throw new Exception($"'BackpackComponent' 正在加载数据，外部禁止操作!({log}) r:{b_Component.Parent.GameUserId} ({itemData.ToLogString()})");
            }
            long itemUID = itemData.ItemUID;
            if (b_Component.mItemDict.ContainsKey(itemUID))
            {
                //注：Item暂不做处理，有可能进入仓库或地面
                b_Component.mItemBox.RemoveItem(itemData.ConfigData.X, itemData.ConfigData.Y, itemData.data.posX, itemData.data.posY);
                b_Component.mItemDict.Remove(itemUID);

                // ConfigId 映射删除
                var dict = b_Component._ConfigId2Item[itemData.ConfigID];
                dict.Remove(itemUID);
                if (dict.Count == 0)
                {
                    b_Component._ConfigId2Item.Remove(itemData.ConfigID);
                }

                //发送背包物品离开推送给玩家
                b_Component.mPlayer.Send(new G2C_ItemsLeaveBackpack_notice()
                {
                    GameUserId = b_Component.mPlayer.GameUserId,
                    LeaveItemUUID = itemUID
                });

                Log.PLog("Backpack", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品离开背包({log}) {itemData.ToLogString()}上架藏宝阁");

                return true;
            }
            else
            {
                Log.Debug("Error:找不到ItemUID，无法删除：" + itemData.ItemUID);
                return false;
            }
        }
        /// <summary>
        /// 将物品移出背包
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="itemData"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static Item RemoveItem(this BackpackComponent b_Component, Item itemData, string log)
        {
            if (b_Component.Parent.OnlineStatus == EOnlineStatus.Ready)
            {
                throw new Exception($"'BackpackComponent' 正在加载数据，外部禁止操作!({log}) r:{b_Component.Parent.GameUserId} ({itemData.ToLogString()})");
            }
            long itemUID = itemData.ItemUID;
            if (b_Component.mItemDict.ContainsKey(itemUID))
            {
                //注：Item暂不做处理，有可能进入仓库或地面
                b_Component.mItemBox.RemoveItem(itemData.ConfigData.X, itemData.ConfigData.Y, itemData.data.posX, itemData.data.posY);
                b_Component.mItemDict.Remove(itemUID);

                // ConfigId 映射删除
                var dict = b_Component._ConfigId2Item[itemData.ConfigID];
                dict.Remove(itemUID);
                if (dict.Count == 0)
                {
                    b_Component._ConfigId2Item.Remove(itemData.ConfigID);
                }

                //发送背包物品离开推送给玩家
                b_Component.mPlayer.Send(new G2C_ItemsLeaveBackpack_notice()
                {
                    GameUserId = b_Component.mPlayer.GameUserId,
                    LeaveItemUUID = itemUID
                });

                itemData.data.InComponent = EItemInComponent.None;
                itemData.data.posX = 0;
                itemData.data.posY = 0;
                itemData.OnlySaveDB();  // 标记变动
                Log.PLog("Backpack", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品离开背包({log}) {itemData.ToLogString()}");

                // 发布 BackpackDeleteItem 事件
                ETModel.EventType.BackpackRemoveItem.Instance.player = b_Component.Parent;
                ETModel.EventType.BackpackRemoveItem.Instance.item = itemData;
                Root.EventSystem.OnRun("BackpackRemoveItem", ETModel.EventType.BackpackRemoveItem.Instance);

                return itemData;
            }
            else
            {
                Log.Debug("Error:找不到ItemUID，无法删除：" + itemData.ItemUID);
                return null;
            }
        }

        /// <summary>
        /// 可以添加物品，添加前进行判断，如果中间使用 await。需要将要添加的格子锁住，防止其他代码添加物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public static bool CanAddItem(this BackpackComponent b_Component, Item item, int posX, int posY)
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

        public static bool CanAddItem(this BackpackComponent b_Component, Item item)
        {
            int x = 0, y = 0;
            if (!b_Component.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, ref x, ref y))
            {
                // 位置放不下这个物品
                return false;
            }
            return true;
        }

        /// <summary>
        /// 可以添加物品，添加前进行判断，如果中间使用 await。需要将要添加的格子锁住，防止其他代码添加物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public static bool CanAddItem(this BackpackComponent b_Component, int configId, int posX, int posY)
        {
            ItemConfigManagerComponent itemConfigManager = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>();
            ItemConfig conf = itemConfigManager.Get(configId);
            if (conf == null) return false; // 物品不存在
            if (!b_Component.mItemBox.CheckStatus(conf.X, conf.Y, posX, posY))
            {
                // 位置放不下这个物品
                return false;
            }
            return true;
        }

        /// <summary>
        /// 可以添加多个物品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="allItem"></param>
        /// <returns></returns>
        public static bool CanAddItemMany(this BackpackComponent self, List<Item> allItem)
        {
            using ItemsBoxStatus.LockList lockList = new ItemsBoxStatus.LockList();
            foreach (Item item in allItem)
            {
                int x = 0;
                int y = 0;
                if (self.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, ref x, ref y))
                {
                    lockList.Add(self.mItemBox.LockGrid(item.ConfigData.X, item.ConfigData.Y, x, y));
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 可以添加多个物品并锁住
        /// </summary>
        /// <param name="self"></param>
        /// <param name="allItem"></param>
        /// <param name="lockList">ItemsBoxStatus.LockList</param>
        /// <param name="posList"></param>
        /// <returns></returns>
        public static bool CanAddItemManyAndLock(this BackpackComponent self, List<Item> allItem, ref List<Lock> lockList, ref List<(int x, int y)> posList)
        {
            foreach (Item item in allItem)
            {
                int x = 0;
                int y = 0;
                if (self.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, ref x, ref y))
                {
                    posList.Add((x, y));
                    lockList.Add(self.mItemBox.LockGrid(item.ConfigData.X, item.ConfigData.Y, x, y));
                }
                else
                {
                    foreach (ItemsBoxStatus.Lock v in lockList)
                    {
                        v.Dispose();
                    }
                    lockList.Clear();
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 删除物品，并从数据库中删除
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="dbData"></param>
        /// <param name="log">移除原因</param>
        /// <returns></returns>
        public static bool DeleteItem(this BackpackComponent b_Component, Item itemData, string log)
        {
            if (b_Component.RemoveItem(itemData, log) == null)
            {
                return false;
            }
            else
            {
                itemData.DisposeDB(log);
                return true;
            }
        }

        /// <summary>
        /// 移动物品,修改位置并推送给玩家
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="dbData"></param>
        /// <returns></returns>
        public static bool MoveItem(this BackpackComponent b_Component, Item dbData, int targetPosX, int targetPosY)
        {
            if (b_Component.Parent.OnlineStatus == EOnlineStatus.Ready)
            {
                throw new Exception($"'BackpackComponent' 正在加载数据，外部禁止操作! r:{b_Component.Parent.GameUserId} ({dbData.ToLogString()})");
            }
            if (b_Component.mItemDict.TryGetValue(dbData.ItemUID, out Item item))
            {
                b_Component.mItemBox.RemoveItem(item.ConfigData.X, item.ConfigData.Y, item.data.posX, item.data.posY);
                if (b_Component.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, targetPosX, targetPosY))
                {
                    item.data.posX = targetPosX;
                    item.data.posY = targetPosY;
                    item.SaveDB(b_Component.mPlayer);
                }
                else
                {
                    //若无法添加则还原移除的状态
                    b_Component.mItemBox.AddItem(item.ConfigData.X, item.ConfigData.Y, item.data.posX, item.data.posY);
                    return false;
                }

                //数据库处理
                item.OnlySaveDB();

                //发送推送给玩家
                b_Component.mPlayer.Send(new G2C_ItemsLocationChangeBackpack_notice()
                {
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
        /// 从背包根据ConfigID查找物品，并返回指定等级的物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="configID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Item GetLevelItemFromConfigID(this BackpackComponent b_Component, int configID, int level)
        {
            if (b_Component._ConfigId2Item.TryGetValue(configID, out var dict))
            {
                foreach (var item in dict.Values)
                {
                    if (item.GetProp(EItemValue.Level) == level)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 从背包根据ConfigID查找物品，并返回指定等级的物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="configID"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static List<Item> GetLevelItemsFromConfigID(this BackpackComponent b_Component, int configID, int level)
        {
            List<Item> result = new List<Item>();
            if (b_Component._ConfigId2Item.TryGetValue(configID, out var dict))
            {
                foreach (var item in dict.Values)
                {
                    if (item.GetProp(EItemValue.Level) == level)
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public static Dictionary<long, Item> GetAllItemByConfigID(this BackpackComponent b_Component, int configID)
        {
            if (b_Component._ConfigId2Item.TryGetValue(configID, out var dict))
            {
                return dict;
            }
            return null;
        }


        public static IEnumerable<Item> Where(this BackpackComponent b_Component, Func<Item, bool> predicate)
        {
            return b_Component.mItemDict.Values.Where(predicate);
        }

        /// <summary>
        /// 推送背包中的所有物品，一般在选择角色进入场景后调用
        /// </summary>
        /// <param name="b_component"></param>
        public static void NotifyAllItem(this BackpackComponent b_Component)
        {
            G2C_ItemsIntoBackpack_notice sendData = new G2C_ItemsIntoBackpack_notice();
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
        /// 更新背包数据到数据库
        /// </summary>
        /// <param name="b_component"></param>
        public static void SaveDB(this BackpackComponent b_Component)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.mPlayer.GameAreaId);

            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Component.mPlayer.GameAreaId);
            mWriteDataComponent.Save(b_Component.BackPack_DB, dBProxy).Coroutine();
        }
        /// <summary>
        /// 背包是否拥有物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="itemUID">物品UID</param>
        /// <returns></returns>
        public static bool HaveItem(this BackpackComponent b_Component, long itemUID)
        {
            return b_Component.mItemDict.ContainsKey(itemUID);
        }

        public static Item GetItemByUID(this BackpackComponent b_Component, long itemUID)
        {
            if (b_Component.HaveItem(itemUID))
            {
                return b_Component.mItemDict[itemUID];
            }
            return null;
        }

        public static Struct_ItemInBackpack_Status Item2BackpackStatusData(this BackpackComponent b_component, Item itemData)
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

        public static void Destroy(this BackpackComponent b_Component)
        {
            b_Component.mItemBox = null;
            b_Component.mItemDict = null;
        }
        #region 坐骑改版2024.11.25
        public static bool AddMount(this BackpackComponent b_Component, long MountId, out int ErrorId)
        {
            if (b_Component.mMountItemDict == null) b_Component.mMountItemDict = new Dictionary<long, Item>();

            if (b_Component.mItemDict.TryGetValue(MountId, out var MountInfo))
            {
                b_Component.RemoveItem(MountInfo, "移到仓库");
                MountInfo.data.InComponent = EItemInComponent.Mount;
                MountInfo.data.posX = 0;
                MountInfo.data.posY = 0;
                b_Component.mMountItemDict.Add(MountId, MountInfo);
                MountInfo.SaveDBNow().Coroutine();
                ErrorId = 0;
                return true;
            }
            ErrorId = 2302;
            return false;
        }
        public static bool AddMount(this BackpackComponent b_Component, Item Mount)
        {
            if (b_Component.mMountItemDict == null) b_Component.mMountItemDict = new Dictionary<long, Item>();

            if (!b_Component.mMountItemDict.ContainsKey(Mount.ItemUID))
            {
                b_Component.mMountItemDict.Add(Mount.ItemUID, Mount);
                return true;
            }
            Log.PLog($"坐骑重复 玩家:{b_Component.mPlayer.GameUserId}坐骑:{Mount.ItemUID}");
            return false;
        }
        public static bool RemoveMount(this BackpackComponent b_Component, long MountId, out int ErrorId)
        {
            if (b_Component.mMountItemDict.TryGetValue(MountId, out var MountInfo))
            {
                if (MountInfo.GetProp(EItemValue.IsUsing) != 0)
                {
                    ErrorId = 2301;
                    return false;
                }

                b_Component.mMountItemDict.Remove((long)MountId);
                MountInfo.data.InComponent = EItemInComponent.Backpack;
                b_Component.AddItem(MountInfo, "坐骑回收到背包");

                ErrorId = 0;
                return true;
            }
            ErrorId = 2200;
            return false;
        }
        public static bool FortifiedMount(this BackpackComponent b_Component, long MountId, out int level, out int ErrorId)
        {
            if (b_Component.mMountItemDict.TryGetValue(MountId, out var MountInfo))
            {
                if (MountInfo.GetProp(EItemValue.Level) < 15)
                {
                    Dictionary<int, int> Three = new Dictionary<int, int>() { { 280003, 1 }, { 280004, 1 } };
                    Dictionary<int, PetsItem> petsItem = new Dictionary<int, PetsItem>();
                    //整理背包里所需要的道具
                    foreach (var Item in b_Component.mItemDict)
                    {
                        if (Three.ContainsKey(Item.Value.ConfigID))
                        {
                            if (petsItem.TryGetValue(Item.Value.ConfigID, out PetsItem petsItem2) == false)
                            {
                                petsItem2 = new PetsItem();
                                petsItem2.ItemConfingID = Item.Value.ConfigID;
                                petsItem2.ItemID = Item.Key;
                                petsItem2.ItemCnt = Item.Value.GetProp(EItemValue.Quantity);
                                petsItem.Add(Item.Value.ConfigID, petsItem2);
                            }
                            else
                                petsItem2.ItemCnt += Item.Value.GetProp(EItemValue.Quantity);
                        }
                    }
                    if (petsItem.Count < Three.Count)
                    {
                        level = MountInfo.GetProp(EItemValue.Level);
                        ErrorId = 1613;
                        return false;
                    }
                    //检查数量
                    foreach (var item in petsItem)
                    {
                        if (Three.TryGetValue(item.Value.ItemConfingID, out int Cnt))
                        {
                            if (Cnt > item.Value.ItemCnt)
                            {
                                level = MountInfo.GetProp(EItemValue.Level);
                                ErrorId = 1613;
                                return false;
                            }
                        }
                    }
                    //扣除道具
                    foreach (var item in petsItem)
                    {
                        var ItemList = b_Component.GetAllItemByConfigID(item.Value.ItemConfingID);
                        if (ItemList != null)
                        {
                            int Cnt = Three[item.Value.ItemConfingID];

                            foreach (var item2 in ItemList)
                            {
                                if (item2.Value.GetProp(EItemValue.Quantity) >= Cnt)
                                {
                                    if (!b_Component.UseItem(item2.Value, "坐骑强化", Cnt))
                                    {
                                        //代码到这里表示代码出了异常
                                        Log.Error($"宠物强化，使用道具失败GamePlayer:{b_Component.Parent.GameUserId}");
                                    }
                                    Cnt = 0;
                                    break;
                                }
                                else
                                {
                                    Cnt -= item2.Value.GetProp(EItemValue.Quantity);
                                    if (!b_Component.UseItem(item2.Value, "坐骑强化", item2.Value.GetProp(EItemValue.Quantity)))
                                    {
                                        //代码到这里表示代码出了异常
                                        Log.Error($"宠物强化，使用道具失败GamePlayer:{b_Component.Parent.GameUserId}");
                                    }

                                }
                            }
                        }
                        else
                        {
                            level = MountInfo.GetProp(EItemValue.Level);
                            ErrorId = 1611;
                            return false;
                        }
                    }
                    //对比成功率

                    int Rate = RandomHelper.RandomNumber(1, 101);
                    int Level = MountInfo.GetProp(EItemValue.Level);
                    int MountRate = 100 - Level * 5;
                    if (MountRate >= Rate)//成功
                    {
                        MountInfo.SetProp(EItemValue.Level, Level + 1);
                        ErrorId = 1612;

                    }
                    else//失败
                    {
                        if (Level >= 7)
                            Level -= 2;
                        else
                            Level--;
                        MountInfo.SetProp(EItemValue.Level, Level);
                        ErrorId = 1611;
                    }

                    MountInfo.SaveDBNow().Coroutine();
                    var equipComponent = b_Component.Parent.GetCustomComponent<EquipmentComponent>();
                    if (equipComponent != null)
                    {
                        equipComponent.ApplyEquipProp();
                    }
                    level = MountInfo.GetProp(EItemValue.Level);
                    return true;

                }
                level = MountInfo.GetProp(EItemValue.Level);
                ErrorId = 3009;
                return false;
            }
            level = 0;
            ErrorId = 2200;
            return false;
        }
        public static bool AdvancedMount(this BackpackComponent b_Component, long MountId, out int ErrorId)
        {
            if (b_Component.mMountItemDict.TryGetValue(MountId, out var MountInfo))
            {
                {
                    Dictionary<int, int> Three = new Dictionary<int, int>() { { 280003, 1 }, { 280004, 1 }, { 280001, 1 }, { 280006, 1 } };
                    Dictionary<int, PetsItem> petsItem = new Dictionary<int, PetsItem>();
                    //整理背包里所需要的道具
                    foreach (var Item in b_Component.mItemDict)
                    {
                        if (Three.ContainsKey(Item.Value.ConfigID))
                        {
                            if (petsItem.TryGetValue(Item.Value.ConfigID, out PetsItem petsItem2) == false)
                            {
                                petsItem2 = new PetsItem();
                                petsItem2.ItemConfingID = Item.Value.ConfigID;
                                petsItem2.ItemID = Item.Key;
                                petsItem2.ItemCnt = Item.Value.GetProp(EItemValue.Quantity);
                                petsItem.Add(Item.Value.ConfigID, petsItem2);
                            }
                            else
                                petsItem2.ItemCnt += Item.Value.GetProp(EItemValue.Quantity);
                        }
                    }
                    if (petsItem.Count < Three.Count)
                    {
                        ErrorId = 1613;
                        return false;
                    }
                    //检查数量
                    foreach (var item in petsItem)
                    {
                        if (Three.TryGetValue(item.Value.ItemConfingID, out int Cnt))
                        {
                            if (Cnt > item.Value.ItemCnt)
                            {
                                ErrorId = 1613;
                                return false;
                            }
                        }
                    }
                    //扣除道具
                    foreach (var item in petsItem)
                    {
                        var ItemList = b_Component.GetAllItemByConfigID(item.Value.ItemConfingID);
                        if (ItemList != null)
                        {
                            int Cnt = Three[item.Value.ItemConfingID];

                            foreach (var item2 in ItemList)
                            {
                                if (item2.Value.GetProp(EItemValue.Quantity) >= Cnt)
                                {
                                    if (!b_Component.UseItem(item2.Value, "坐骑强化", Cnt))
                                    {
                                        //代码到这里表示代码出了异常
                                        Log.Error($"宠物强化，使用道具失败GamePlayer:{b_Component.Parent.GameUserId}");
                                    }
                                    Cnt = 0;
                                    break;
                                }
                                else
                                {
                                    Cnt -= item2.Value.GetProp(EItemValue.Quantity);
                                    if (!b_Component.UseItem(item2.Value, "坐骑强化", item2.Value.GetProp(EItemValue.Quantity)))
                                    {
                                        //代码到这里表示代码出了异常
                                        Log.Error($"宠物强化，使用道具失败GamePlayer:{b_Component.Parent.GameUserId}");
                                    }

                                }
                            }
                        }
                        else
                        {
                            ErrorId = 1611;
                            return false;
                        }
                    }

                    MountInfo.SetProp(EItemValue.Advanced, MountInfo.GetProp(EItemValue.Advanced) + 1);
                    ErrorId = 3701;
                    MountInfo.SaveDBNow().Coroutine();
                    var equipComponent = b_Component.Parent.GetCustomComponent<EquipmentComponent>();
                    if (equipComponent != null)
                    {
                        equipComponent.ApplyEquipProp();
                    }

                    return true;

                }
            }
            ErrorId = 2200;
            return false;
        }
        public static bool UseAMount(this BackpackComponent b_Component, long MountId, out int ErrorId)
        {
            if (b_Component.mMountItemDict.TryGetValue(MountId, out var MountInfo))
            {
                var equipCmt = b_Component.mPlayer.GetCustomComponent<EquipmentComponent>();
                var mounts = equipCmt.GetEquipItemByPosition(EquipPosition.Mounts);
                if (mounts != null)
                {
                    if (mounts.ItemUID == MountId)
                    {
                        MountInfo.SetProp(EItemValue.IsUsing, 1, b_Component.Parent);
                        ErrorId = 0;
                        return true;
                    }

                    if (!b_Component.TryRecallMountedMountToPanel(equipCmt, mounts.ItemUID, "切换坐骑，先卸下当前坐骑", out ErrorId))
                    {
                        return false;
                    }

                    if (!b_Component.mMountItemDict.TryGetValue(MountId, out MountInfo))
                    {
                        ErrorId = 2200;
                        return false;
                    }
                }

                // 骑上坐骑
                equipCmt.EquipItem(MountInfo, EquipPosition.Mounts, "装备坐骑，用来给玩家加属性");
                MountInfo.SetProp(EItemValue.IsUsing, 1, b_Component.Parent);
                ErrorId = 0;
                return true;
            }
            ErrorId = 2200;
            return false;
        }
        public static bool RecallAMount(this BackpackComponent b_Component, long MountId, out int ErrorId)
        {
            var equipCmt = b_Component.mPlayer.GetCustomComponent<EquipmentComponent>();
            if (b_Component.mMountItemDict.TryGetValue(MountId, out var MountInfo))
            {
                var mounts = equipCmt.GetEquipItemByPosition(EquipPosition.Mounts);
                if (mounts != null && MountInfo.ItemUID == mounts.ItemUID)
                {
                    return b_Component.TryRecallMountedMountToPanel(equipCmt, MountId, "卸下坐骑", out ErrorId);
                }
                ErrorId = 2301;
                return false;
            }

            return b_Component.TryRecallMountedMountToPanel(equipCmt, MountId, "卸下坐骑", out ErrorId);
        }
        public static Dictionary<long, int> GetMountList(this BackpackComponent b_Component)
        {
            Dictionary<long, int> keyValuePairs = new Dictionary<long, int>();
            if (b_Component.mMountItemDict != null)
            {
                foreach (var Mount in b_Component.mMountItemDict)
                {
                    keyValuePairs.Add(Mount.Value.ItemUID, Mount.Value.ConfigID);
                }

                return keyValuePairs;
            }
            return null;
        }
        public static Item GetMountInfo(this BackpackComponent b_Component, long MountId)
        {
            if (b_Component.mMountItemDict.TryGetValue(MountId, out var item))
                return item;
            return null;
        }

        private static bool TryRecallMountedMountToPanel(this BackpackComponent b_Component, EquipmentComponent equipCmt, long MountId, string log, out int ErrorId)
        {
            Item mountedItem = equipCmt.GetEquipItemByPosition(EquipPosition.Mounts);
            if (mountedItem == null)
            {
                ErrorId = 2200;
                return false;
            }

            if (mountedItem.ItemUID != MountId)
            {
                ErrorId = 2301;
                return false;
            }

            Item recalledMount = equipCmt.UnloadEquipItem(EquipPosition.Mounts, log);
            if (recalledMount == null)
            {
                ErrorId = 2200;
                return false;
            }

            recalledMount.SetProp(EItemValue.IsUsing, 0, b_Component.Parent);
            recalledMount.data.InComponent = EItemInComponent.Mount;
            recalledMount.data.posId = 0;
            recalledMount.data.posX = 0;
            recalledMount.data.posY = 0;

            if (b_Component.mMountItemDict == null)
            {
                b_Component.mMountItemDict = new Dictionary<long, Item>();
            }
            b_Component.mMountItemDict[recalledMount.ItemUID] = recalledMount;

            recalledMount.SaveDBNow().Coroutine();
            ErrorId = 0;
            return true;
        }
        #endregion
    }
}
