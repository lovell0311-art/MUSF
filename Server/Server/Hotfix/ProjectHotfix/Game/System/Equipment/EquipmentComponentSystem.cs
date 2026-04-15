using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ETHotfix
{
    [Timer(TimerType.JewelryDurabilityDown)]
    public class JewelryDurabilityDownTimer : ATimer<EquipmentComponent>
    {
        public override void Run(EquipmentComponent self)
        {
            if (self.Parent.OnlineStatus != EOnlineStatus.Online) return;   // 玩家正在进入游戏 或 正在下线

            // 降低耐久 翅膀、戒指、项链
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(self.mPlayer.SourceGameAreaId);
            var gamePlayer = self.mPlayer.GetCustomComponent<GamePlayer>();
            //var map = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(gamePlayer.UnitData.Index);
            int mapId = gamePlayer.UnitData.Index;

            MapComponent map = Help_MapHelper.GetMapByMapId(mServerArea, mapId, gamePlayer.InstanceId);
            if (map == null)
            {
                Log.Warning($"玩家不在地图中. self.mPlayer.GameUserId={self.mPlayer.GameUserId}");
                return;
            }

            C_FindTheWay2D pos = map.GetFindTheWay2D(gamePlayer);
            bool durChanged = false;
            if (pos != null && pos.IsSafeArea == false)
            {
                // 玩家不在安全区
                var leftRing = self.GetEquipItemByPosition(EquipPosition.LeftRing);
                var rightRing = self.GetEquipItemByPosition(EquipPosition.RightRing);
                var necklace = self.GetEquipItemByPosition(EquipPosition.Necklace);
                if (leftRing != null)
                {
                    if (leftRing.DurabilityDown(1, gamePlayer)) durChanged = true;
                }
                if (rightRing != null)
                {
                    if (rightRing.DurabilityDown(1, gamePlayer)) durChanged = true;
                }
                if (necklace != null)
                {
                    if (necklace.DurabilityDown(1, gamePlayer)) durChanged = true;
                }
            }
            var wing = self.GetEquipItemByPosition(EquipPosition.Wing);
            if (wing != null)
            {
                if (wing.DurabilityDown(1, gamePlayer)) durChanged = true;
            }
            if (durChanged == true)
            {
                // 耐久变动
                // TODO 更新装备属性
                self.ApplyEquipProp();
                // 通知玩家，属性变动

            }
            //检查装备是否都达到10级
            List<EquipPosition> items = new List<EquipPosition>()
            {
                EquipPosition.Helmet,
                EquipPosition.Armor,
                EquipPosition.Leggings,
                EquipPosition.HandGuard,
                EquipPosition.Boots,
            };
            int Count = 0;
            foreach (var Index in items)
            {
                var ItemInfo = self.GetEquipItemByPosition(Index);
                if (ItemInfo != null)
                {
                    if (ItemInfo.GetProp(EItemValue.Level) >= 10) Count += 1;
                }
            }
            if (Count >= 5)
            {
                ETModel.EventType.EquipmentRelatedSettings.Instance.player = self.mPlayer;
                ETModel.EventType.EquipmentRelatedSettings.Instance.item = null;
                ETModel.EventType.EquipmentRelatedSettings.Instance.TitleCount = 0;
                ETModel.EventType.EquipmentRelatedSettings.Instance.ItemCount = Count;
                Root.EventSystem.OnRun("EquipmentRelatedSettings", ETModel.EventType.EquipmentRelatedSettings.Instance);
            }
        }
    }



    [EventMethod(typeof(EquipmentComponent), EventSystemType.INIT)]
    public class EquipmentComponentEventOnInit : ITEventMethodOnInit<EquipmentComponent>
    {
        public void OnInit(EquipmentComponent b_Component)
        {
            b_Component.OnInit();
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000 * 10, TimerType.JewelryDurabilityDown, b_Component);
        }
    }

    [EventMethod(typeof(EquipmentComponent), EventSystemType.DISPOSE)]
    public class EquipmentComponentEventOnDispose : ITEventMethodOnDispose<EquipmentComponent>
    {
        public override void OnDispose(EquipmentComponent b_Component)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }
    /// <summary>
    /// 背包组件
    /// </summary>
    public static partial class EquipmentComponentSystem
    {
        public static void OnInit(this EquipmentComponent b_Component)
        {
            //Log.Debug("Package.Awake()");
            if (b_Component.EquipPartItemDict == null)
            {
                b_Component.EquipPartItemDict = new Dictionary<int, Item>();
            }
        }

        /// <summary>
        /// 初始化装备数据，玩家上线时执行
        /// </summary>
        /// <param name="b_Component"></param>
        /// <returns></returns>
        public static async Task<bool> Init(this EquipmentComponent b_Component)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.mPlayer.GameAreaId);
            DataCacheManageComponent dataCacheManage = b_Component.mPlayer.GetCustomComponent<DataCacheManageComponent>();
            C_DataCache<DBItemData> itemDataCache = await dataCacheManage.Append<DBItemData>(dBProxy2, p => p.GameUserId == b_Component.mPlayer.GameUserId && p.InComponent == EItemInComponent.Equipment && p.IsDispose == 0);
            // 取出装备栏中的所有物品
            List<DBItemData> allDBItemData = itemDataCache.DataQuery(p => p.GameUserId == b_Component.mPlayer.GameUserId && p.InComponent == EItemInComponent.Equipment && p.IsDispose == 0);
            for (int i = 0, count = allDBItemData.Count; i < count; i++)
            {
                DBItemData item = allDBItemData[i];
                Item curItem = ItemFactory.CreateFormDB(item.Id, b_Component.mPlayer);
                if (curItem == null)
                {
                    // TODO 存档损坏
                    // 需要将玩家踢下线
                    throw new Exception($"装备栏存档损坏，无法生成物品 uid={item.Id} gameUserId={b_Component.mPlayer.GameUserId}");
                }
                //将Item对象添加到mItemBox里
                if (!b_Component.EquipPartItemDict.TryAdd(curItem.data.posId, curItem))
                {
                    Log.Error($"装备栏存档损坏，物品重叠 uid={item.Id} posId={curItem.data.posId} gameUserId={b_Component.mPlayer.GameUserId}");
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
            {
                // 发布 EquipInitComplete 事件
                ETModel.EventType.EquipInitComplete.Instance.unit = b_Component.Parent.GetCustomComponent<GamePlayer>();
                Root.EventSystem.OnRun("EquipInitComplete", ETModel.EventType.EquipInitComplete.Instance);
            }
            return true;
        }

        public static Item GetEquipItemByPosition(this EquipmentComponent b_Component, EquipPosition position)
        {
            return b_Component.GetEquipItemByPosition((int)position);
        }

        public static Item GetEquipItemByPosition(this EquipmentComponent b_Component, int position)
        {
            if (IsTempSlot((EquipPosition)position))
            {
                if (b_Component.TempSlotDict.TryGetValue((EquipPosition)position, out Item item))
                {
                    return item;
                }
            }
            else if (b_Component.EquipPartItemDict.TryGetValue(position, out Item item))
            {
                return item;
            }
            return null;
        }

        public static IEnumerable<Item> Where(this EquipmentComponent b_Component, Func<Item, bool> predicate)
        {
            return b_Component.EquipPartItemDict.Values.Where(predicate);
        }

        /// <summary>
        /// 没有穿戴装备
        /// </summary>
        /// <param name="b_Component"></param>
        /// <returns></returns>
        public static bool EquipIsEmpty(this EquipmentComponent b_Component)
        {
            return b_Component.EquipPartItemDict.Count == 0;
        }

        /// <summary>
        /// 穿戴装备
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool EquipItem(this EquipmentComponent b_Component, Item item, EquipPosition position, string log)
        {
            //Log.Info("=================EquipItem()");
            if (IsTempSlot((EquipPosition)position))
            {
                if (!b_Component.TempSlotDict.ContainsKey(position))
                {
                    b_Component.TempSlotDict.Add(position, item);

                    // 发布 EquipItem 事件
                    ETModel.EventType.EquipItem.Instance.unit = b_Component.Parent.GetCustomComponent<GamePlayer>();
                    ETModel.EventType.EquipItem.Instance.item = item;
                    ETModel.EventType.EquipItem.Instance.position = position;
                    Root.EventSystem.OnRun("EquipItem", ETModel.EventType.EquipItem.Instance);

                    // 应用属性
                    b_Component.ApplyEquipProp();
                    //广播周围玩家
                    //Log.Info("装备完成，推送周围玩家");
                    G2C_UnitEquipLoad_notice notice = new G2C_UnitEquipLoad_notice();
                    notice.AllEquipStatus.Add(b_Component.GetEquipSlotStatus(item, position));
                    b_Component.NotifyAroundPlayer(notice);
                    item.SendAllPropertyData(b_Component.mPlayer);
                    item.SendAllEntryAttr(b_Component.mPlayer);
                }
            }
            else if (!b_Component.EquipPartItemDict.ContainsKey((int)position))
            {
                //检测是否达到装备条件
                if (b_Component.CheckEquipRule(item, position))
                {
                    //穿戴装备
                    b_Component.EquipPartItemDict.Add((int)position, item);
                    item.data.posId = (int)position;
                    item.data.InComponent = EItemInComponent.Equipment;
                    item.data.UserId = b_Component.mPlayer.UserId;
                    item.data.GameUserId = b_Component.mPlayer.GameUserId;


                    Log.PLog("Equipment", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品进入装备栏({log}) {item.ToLogString()}");

                    // 发布 EquipItem 事件
                    ETModel.EventType.EquipItem.Instance.unit = b_Component.Parent.GetCustomComponent<GamePlayer>();
                    ETModel.EventType.EquipItem.Instance.item = item;
                    ETModel.EventType.EquipItem.Instance.position = position;
                    Root.EventSystem.OnRun("EquipItem", ETModel.EventType.EquipItem.Instance);

                    item.SaveDB(b_Component.mPlayer);
                    // 应用属性
                    b_Component.ApplyEquipProp();
                    //广播周围玩家
                    //Log.Info("装备完成，推送周围玩家");
                    G2C_UnitEquipLoad_notice notice = new G2C_UnitEquipLoad_notice();
                    notice.AllEquipStatus.Add(b_Component.GetEquipSlotStatus(item, position));
                    b_Component.NotifyAroundPlayer(notice);
                    item.SendAllPropertyData(b_Component.mPlayer);
                    item.SendAllEntryAttr(b_Component.mPlayer);
                    //Log.Info("推送完毕");
                    return true;
                }
                else { return false; }
            }
            return false;
        }

        public static bool EquipItemFromBackpack(this EquipmentComponent b_Component, Item item, EquipPosition position, string log)
        {
            if (IsTempSlot((EquipPosition)position))
            {
                return b_Component.EquipItem(item, position, log);
            }
            else if (!b_Component.EquipPartItemDict.ContainsKey((int)position))
            {
                if (b_Component.CheckEquipRule(item, position))
                {
                    // 将背包中的装备移除
                    BackpackComponent backpackComponent = b_Component.mPlayer.GetCustomComponent<BackpackComponent>();
                    //从背包中移除物品
                    backpackComponent.RemoveItem(item, $"移到装备栏 position={position}");

                    return b_Component.EquipItem(item, position, log);
                }else { return false; }
            }
            return false;
        }

        /// <summary>
        /// 卸下装备
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="position">>卸下部件位置</param>
        /// <param name="backPackPosX">卸下时放回背包的位置X</param>
        /// <param name="backPackPosY">卸下时放回背包的位置Y</param>
        /// <returns></returns>
        public static bool UnloadEquipItemToBackpack(this EquipmentComponent b_Component, EquipPosition position, int backPackPosX, int backPackPosY, string log)
        {
            if (IsTempSlot(position))
            {
                if (b_Component.TempSlotDict.TryGetValue(position, out Item curItem))
                {
                    b_Component.TempSlotDict.Remove(position);

                    // 发布 UnloadEquipItem 事件
                    ETModel.EventType.UnloadEquipItem.Instance.unit = b_Component.Parent.GetCustomComponent<GamePlayer>();
                    ETModel.EventType.UnloadEquipItem.Instance.item = curItem;
                    ETModel.EventType.UnloadEquipItem.Instance.position = position;
                    Root.EventSystem.OnRun("UnloadEquipItem", ETModel.EventType.UnloadEquipItem.Instance);

                    // 应用属性
                    b_Component.ApplyEquipProp();
                    //广播周围玩家
                    //Log.Info("卸载成功，广播周围玩家和自己");
                    b_Component.NotifyAroundPlayer(new G2C_UnitEquipUnload_notice()
                    {
                        GameUserId = b_Component.mPlayer.GameUserId,
                        EquipPosition = (int)position,
                        EquipItemUUID = curItem.ItemUID
                    });
                    return true;
                }
            }
            else if (b_Component.EquipPartItemDict.TryGetValue((int)position, out Item curItem))
            {
                //检测当前背包是否能装下卸下的物品
                GamePlayer gamePlayer = b_Component.mPlayer.GetCustomComponent<GamePlayer>();
                var backpackComponent = b_Component.mPlayer.GetCustomComponent<BackpackComponent>();
                if (backpackComponent != null)
                {
                    if (backpackComponent.mItemBox.CheckStatus(curItem.ConfigData.X, curItem.ConfigData.Y, backPackPosX, backPackPosY))
                    {
                        b_Component.EquipPartItemDict.Remove((int)position);
                        curItem.data.posId = 0;
                        curItem.data.InComponent = EItemInComponent.None;
                        Log.PLog("Equipment", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品离开装备栏({log}) {curItem.ToLogString()}");

                        // 发布 UnloadEquipItem 事件
                        ETModel.EventType.UnloadEquipItem.Instance.unit = b_Component.Parent.GetCustomComponent<GamePlayer>();
                        ETModel.EventType.UnloadEquipItem.Instance.item = curItem;
                        ETModel.EventType.UnloadEquipItem.Instance.position = (EquipPosition)position;
                        Root.EventSystem.OnRun("UnloadEquipItem", ETModel.EventType.UnloadEquipItem.Instance);

                        //Log.Info("卸下完成，存储数据");
                        curItem.OnlySaveDB();
                        // 应用属性
                        b_Component.ApplyEquipProp();
                        //广播周围玩家
                        //Log.Info("卸载成功，广播周围玩家和自己");
                        b_Component.NotifyAroundPlayer(new G2C_UnitEquipUnload_notice()
                        {
                            GameUserId = b_Component.mPlayer.GameUserId,
                            EquipPosition = (int)position,
                            EquipItemUUID = curItem.ItemUID
                        });

                        backpackComponent.AddItem(curItem, backPackPosX, backPackPosY, "装备栏移出");

                        return true;
                    }
                    else { Log.Info("背包当前位置无法装下卸载的装备，卸载失败"); return false; }
                }
                else return false;
            }
            //背包当前位置没有物品，默认返回true
            return true;
        }

        public static Item UnloadEquipItem(this EquipmentComponent b_Component, EquipPosition position, string log)
        {
            if (IsTempSlot(position))
            {
                if (b_Component.TempSlotDict.TryGetValue(position, out Item curItem))
                {
                    b_Component.TempSlotDict.Remove(position);
                    curItem.data.posId = 0;
                    curItem.data.InComponent = EItemInComponent.None;
                    Log.PLog("Equipment", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品离开临时装备栏({log}) {curItem.ToLogString()}");

                    // 发布 UnloadEquipItem 事件
                    ETModel.EventType.UnloadEquipItem.Instance.unit = b_Component.Parent.GetCustomComponent<GamePlayer>();
                    ETModel.EventType.UnloadEquipItem.Instance.item = curItem;
                    ETModel.EventType.UnloadEquipItem.Instance.position = position;
                    Root.EventSystem.OnRun("UnloadEquipItem", ETModel.EventType.UnloadEquipItem.Instance);

                    // 应用属性
                    b_Component.ApplyEquipProp();
                    //广播周围玩家
                    b_Component.NotifyAroundPlayer(new G2C_UnitEquipUnload_notice()
                    {
                        GameUserId = b_Component.mPlayer.GameUserId,
                        EquipPosition = (int)position,
                        EquipItemUUID = curItem.ItemUID
                    });

                    return curItem;
                }
                return null;
            }
            else if (b_Component.EquipPartItemDict.TryGetValue((int)position, out Item curItem))
            {
                GamePlayer gamePlayer = b_Component.mPlayer.GetCustomComponent<GamePlayer>();
                b_Component.EquipPartItemDict.Remove((int)position);
                curItem.data.posId = 0;
                curItem.data.InComponent = EItemInComponent.None;
                Log.PLog("Equipment", $"a:{b_Component.mPlayer.UserId} r:{b_Component.mPlayer.GameUserId} 物品离开装备栏({log}) {curItem.ToLogString()}");

                // 发布 UnloadEquipItem 事件
                ETModel.EventType.UnloadEquipItem.Instance.unit = b_Component.Parent.GetCustomComponent<GamePlayer>();
                ETModel.EventType.UnloadEquipItem.Instance.item = curItem;
                ETModel.EventType.UnloadEquipItem.Instance.position = (EquipPosition)position;
                Root.EventSystem.OnRun("UnloadEquipItem", ETModel.EventType.UnloadEquipItem.Instance);

                // 应用属性
                b_Component.ApplyEquipProp();
                //广播周围玩家
                //Log.Info("卸载成功，广播周围玩家和自己");
                b_Component.NotifyAroundPlayer(new G2C_UnitEquipUnload_notice()
                {
                    GameUserId = b_Component.mPlayer.GameUserId,
                    EquipPosition = (int)position,
                    EquipItemUUID = curItem.ItemUID
                });


                return curItem;
            }
            return null;
        }

        public static bool IsTempSlot(EquipPosition position)
        {
            return position > EquipPosition.TempSlot;
        }
        /// <summary>
        /// 检查物品是否能穿戴
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool CheckEquipRule(this EquipmentComponent b_Component, Item item, EquipPosition position)
        {
            //Log.Info("EquipmentComponent:正在检查物品是否能穿戴");
            var configData = item.ConfigData;
            if (configData.Slot == (int)EquipPosition.None) { Log.Info("物品不能装备"); return false; }
            var mGamePlayer = b_Component.mPlayer.GetCustomComponent<GamePlayer>();
            //检查玩家职业与转职次数是否符合要求
            if (item.ConfigData.UseRole.TryGetValue(mGamePlayer.Data.PlayerTypeId, out int reqValue))
            {
                if (reqValue < 0) { Log.Info("物品不符合该职业"); return false; }
                if (reqValue > mGamePlayer.Data.OccupationLevel) { Log.Info("未达到转职次数"); return false; }
            }
            else
            {
                // 物品不符合该职业
                return false;
            }
            //装备部位特殊判断逻辑
            bool posCanEquip = true;
            if ((int)position != configData.Slot)
            {
                posCanEquip = false;
                //戒指可以左右互戴
                if ((int)position == (int)EquipPosition.LeftRing || (int)position == (int)EquipPosition.RightRing)
                {
                    if (configData.Slot == (int)EquipPosition.LeftRing || configData.Slot == (int)EquipPosition.RightRing)
                    {
                        posCanEquip = true;
                    }
                }
                //判断剑士、魔剑士可以装备两个单手武器
                if ((int)position == (int)EquipPosition.Shield && item.ConfigData.Slot == (int)EquipPosition.Weapon && configData.TwoHand == 0)
                {
                    //Log.Info($"判断是否是剑士、魔剑士：mGamePlayer.GameHeroType=" + mGamePlayer.Data.PlayerType); 
                    if (mGamePlayer.Data.PlayerTypeId == (int)E_GameOccupation.Spellsword || mGamePlayer.Data.PlayerTypeId == (int)E_GameOccupation.Swordsman)
                    {
                        //剑士和魔剑士可以装备单手武器到左手
                        posCanEquip = true;
                    }
                }

                //特殊判断逻辑结束
            }
            if (!posCanEquip) { return false; }

            //else { Log.Info("未检测到职业要求配置"); return false; }
            //玩家属性要求
            if (configData.ReqLvl > mGamePlayer.Data.Level) { return false; }
            if (configData.ReqAgi > mGamePlayer.GetNumerial(E_GameProperty.Property_Agility)) { return false; }
            if (configData.ReqCom > mGamePlayer.GetNumerial(E_GameProperty.Property_Command)) { return false; }
            if (configData.ReqEne > mGamePlayer.GetNumerial(E_GameProperty.Property_Willpower)) { return false; }
            if (configData.ReqStr > mGamePlayer.GetNumerial(E_GameProperty.Property_Strength)) { return false; }
            if (configData.ReqVit > mGamePlayer.GetNumerial(E_GameProperty.Property_BoneGas)) { return false; }
            //判断双手武器无法装备盾牌
            if (configData.Slot == (int)EquipPosition.Shield && b_Component.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out Item curWeapon))
            {
                if (curWeapon.ConfigData.TwoHand == 1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 更新背包数据到数据库
        /// </summary>
        /// <param name="b_component"></param>
        public static void SaveDB(this EquipmentComponent b_Component)
        {
            DataCacheManageComponent mDataCacheManageComponent = b_Component.mPlayer.AddCustomComponent<DataCacheManageComponent>();
            var dataCache = mDataCacheManageComponent.Get<DBEquipmentItem>();
            var backPackDatas = dataCache.DataQuery(p => p.GameUserId == b_Component.mPlayer.GameUserId);

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.mPlayer.GameAreaId);

            if (backPackDatas.Count == 0)
            {
                dataCache.DataAdd(b_Component.Equipment_DB);
                dBProxy.Save(b_Component.Equipment_DB).Coroutine();
            }
            else
            {
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Component.mPlayer.GameAreaId);
                mWriteDataComponent.Save(b_Component.Equipment_DB, dBProxy).Coroutine();
            }
        }

        /// <summary>
        /// 将当前协议广播给周围的玩家和自己 穿戴和卸下装备时使用
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_ActorMessage"></param>
        public static void NotifyAroundPlayer(this EquipmentComponent b_Component, IActorMessage b_ActorMessage)
        {
            //获得角色当前所在地图和坐标
            var mGamePlayer = b_Component.mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null) return;

            int mapIndex = mGamePlayer.UnitData.Index;

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mGamePlayer.Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                Log.Error($"找不到区服");
                return;
            }
            //获得当前地图组件
            //if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mapIndex, out var mapComponent))
            //{
            //    C_FindTheWay2D b_Source = mapComponent.GetFindTheWay2D(x, y);
            //    mapComponent.SendNotice(b_Source, b_ActorMessage);
            //}
            MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mapIndex, mGamePlayer.Player.GameUserId);
            if (mapComponent != null)
            {
                C_FindTheWay2D b_Source = mapComponent.GetFindTheWay2D(mGamePlayer);
                mapComponent.SendNotice(b_Source, b_ActorMessage);
            }
        }

        public static Struct_ItemInSlot_Status GetEquipSlotStatus(this EquipmentComponent b_Component, Item item, EquipPosition position)
        {
            return new Struct_ItemInSlot_Status()
            {
                GameUserId = b_Component.mPlayer.GameUserId,
                ConfigID = item.ConfigID,
                ItemUID = item.ItemUID,
                SlotID = (int)position,//传入position，因为有可能配置表Slot和角色Slot不一致，如左右戒指可相互装备
                ItemLevel = item.GetProp(EItemValue.Level)
            };
        }

        public static G2C_UnitEquipLoad_notice GetAllEquipSlotStatus(this EquipmentComponent b_Component, G2C_UnitEquipLoad_notice notice)
        {
            foreach (var item in b_Component.EquipPartItemDict)
            {
                notice.AllEquipStatus.Add(b_Component.GetEquipSlotStatus(item.Value, (EquipPosition)Convert.ToInt32(item.Key)));
            }
            foreach (var item in b_Component.TempSlotDict)
            {
                notice.AllEquipStatus.Add(b_Component.GetEquipSlotStatus(item.Value, (EquipPosition)Convert.ToInt32(item.Key)));
            }
            var Blood = b_Component.mPlayer.GetCustomComponent<PlayerBloodAwakeningComponent>();
            if (Blood != null)
            {
                notice.AllEquipStatus.Add(new Struct_ItemInSlot_Status()
                {
                    GameUserId = b_Component.mPlayer.GameUserId,
                    ConfigID = Blood.UseBloodAwakeningId,
                    ItemUID = 0,
                    SlotID = (int)SynchronizationType.Blood,
                    ItemLevel = 0
                });
            }
            return notice;
        }

        /// <summary>
        /// 推送自己全部的装备给自己
        /// </summary>
        /// <param name="b_Component"></param>
        public static void NotifyAllEquip(this EquipmentComponent b_Component)
        {
            b_Component.NotifyAllEquipToPlayer(b_Component.mPlayer);
            foreach (var item in b_Component.EquipPartItemDict)
            {
                item.Value.SendAllPropertyData(b_Component.mPlayer);
                item.Value.SendAllEntryAttr(b_Component.mPlayer);
            }
        }

        /// <summary>
        /// 推送全部装备信息给指定Player对象
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="targetPlayer"></param>
        public static void NotifyAllEquipToPlayer(this EquipmentComponent b_Component, Player targetPlayer)
        {
            G2C_UnitEquipLoad_notice notice = new G2C_UnitEquipLoad_notice();
            notice = b_Component.GetAllEquipSlotStatus(notice);
            targetPlayer.Send(notice);
        }

        /// <summary>
        /// 应用装备属性
        /// </summary>
        /// <param name="self"></param>
        /// <param name="isInit">正在初始化，不会重新计算hp、mp、ag、sd</param>
        public static void ApplyEquipProp(this EquipmentComponent self, bool isInit = false)
        {
            var gamePlayer = self.mPlayer.GetCustomComponent<GamePlayer>();
            // TODO 记录属性
            int oldHPMax = 0;
            int oldMPMax = 0;
            int oldAGMax = 0;
            int oldSDMax = 0;

            bool fullHP = false;
            bool fullMP = false;
            bool fullAG = false;
            bool fullSD = false;

            if (isInit == false)
            {
                oldHPMax = gamePlayer.GetNumerial(E_GameProperty.PROP_HP_MAX);
                oldMPMax = gamePlayer.GetNumerial(E_GameProperty.PROP_MP_MAX);
                oldAGMax = gamePlayer.GetNumerial(E_GameProperty.PROP_AG_MAX);
                oldSDMax = gamePlayer.GetNumerial(E_GameProperty.PROP_SD_MAX);

                fullHP = (oldHPMax <= gamePlayer.UnitData.Hp);
                fullMP = (oldMPMax <= gamePlayer.UnitData.Mp);
                fullAG = (oldAGMax <= gamePlayer.UnitData.AG);
                fullSD = (oldSDMax <= gamePlayer.UnitData.SD);
            }

            (gamePlayer.EquipPropertyDic, self.EquipPropertyCacheDic) = (self.EquipPropertyCacheDic,gamePlayer.EquipPropertyDic);

            gamePlayer.EquipPropertyDic.Clear();
            foreach (var kv in self.EquipPartItemDict)
            {
                kv.Value.GetUpdatePropHandler().ApplyEquipProp(kv.Value, self, (EquipPosition)kv.Key);
            }
            foreach (var kv in self.TempSlotDict)
            {
                kv.Value.GetUpdatePropHandler().ApplyEquipProp(kv.Value, self, kv.Key);
            }
            var mEquipmentSetComponent = self.mPlayer.GetCustomComponent<EquipmentSetComponent>();
            // TODO 更新套装属性
            mEquipmentSetComponent?.ApplyEquipSetProp();
            //更新宠物卓越到角色
            mEquipmentSetComponent?.ApplyEquipSetPets();
            //更新VIP特权
            mEquipmentSetComponent?.ApplyEquipSetVip();
            //更新称号属性
            mEquipmentSetComponent?.ApplyEquipSetTitle();
            //应用血脉属性
            mEquipmentSetComponent?.ApplyEquipBloodAwakening();
            //更新强化套装属性
            self.ApplyEquipSetStrengthen();
            //更新整套防御力增加
            self.ApplyEquipKindProp();

            // TODO 检查装备变动的属性
            Dictionary<E_GameProperty, int> changeProp = self.EquipPropertyCacheDic;
            foreach (var kv in gamePlayer.EquipPropertyDic)
            {
                if(changeProp.TryGetValue(kv.Key, out int value))
                {
                    if(kv.Value == value)
                    {
                        changeProp.Remove(kv.Key);
                    }
                }
                else
                {
                    changeProp.Add(kv.Key, 0);
                }
            }
            // 告诉gamePlayer，哪些属性变了
            foreach(E_GameProperty k in changeProp.Keys)
            {
                gamePlayer.ChangeNumerialType(k);
            }


            if (isInit == false)
            {

                bool BaseAttrChanged = false;
                #region 属性变动通知
                G2C_ChangeValue_notice mChangeValueMessage = EquipmentComponent.g2C_ChangeValue_notice;
                mChangeValueMessage.Info.Clear();
                if (changeProp.ContainsKey(E_GameProperty.Property_Strength))
                {
                    BaseAttrChanged = true;
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    // 当前值
                    mPropertyData.Key = (int)E_GameProperty.Property_Strength;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.Property_Strength);
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (changeProp.ContainsKey(E_GameProperty.Property_Willpower))
                {
                    BaseAttrChanged = true;
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    // 当前值
                    mPropertyData.Key = (int)E_GameProperty.Property_Willpower;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.Property_Willpower);
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (changeProp.ContainsKey(E_GameProperty.Property_Agility))
                {
                    BaseAttrChanged = true;
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    // 当前值
                    mPropertyData.Key = (int)E_GameProperty.Property_Agility;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.Property_Agility);
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (changeProp.ContainsKey(E_GameProperty.Property_BoneGas))
                {
                    BaseAttrChanged = true;
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    // 当前值
                    mPropertyData.Key = (int)E_GameProperty.Property_BoneGas;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.Property_BoneGas);
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (changeProp.ContainsKey(E_GameProperty.Property_Command))
                {
                    BaseAttrChanged = true;
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    // 当前值
                    mPropertyData.Key = (int)E_GameProperty.Property_Command;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.Property_Command);
                    mChangeValueMessage.Info.Add(mPropertyData);
                }

                if (BaseAttrChanged == true)
                {
                    // 玩家的基础属性变了，需要重新计算
                    gamePlayer.DataUpdateProperty();
                }

                // 角色基础属性变动会影响生命，魔法这些。
                // 血量，魔法 更新放到最后

                int hpMax = gamePlayer.GetNumerial(E_GameProperty.PROP_HP_MAX);
                int mpMax = gamePlayer.GetNumerial(E_GameProperty.PROP_MP_MAX);
                int agMax = gamePlayer.GetNumerial(E_GameProperty.PROP_AG_MAX);
                int sdMax = gamePlayer.GetNumerial(E_GameProperty.PROP_SD_MAX);

                if (hpMax != oldHPMax)
                {
                    if (hpMax > oldHPMax && fullHP)
                    {
                        gamePlayer.UnitData.Hp = hpMax;
                    }
                    // 当前值
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_HP;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.PROP_HP);
                    mChangeValueMessage.Info.Add(mPropertyData);
                    // 最大值
                    mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_HP_MAX;
                    mPropertyData.Value = hpMax;
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (mpMax != oldMPMax)
                {
                    if (mpMax > oldMPMax && fullMP)
                    {
                        gamePlayer.UnitData.Mp = mpMax;
                    }
                    // 当前值
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_MP;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.PROP_MP);
                    mChangeValueMessage.Info.Add(mPropertyData);
                    // 最大值
                    mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_MP_MAX;
                    mPropertyData.Value = mpMax;
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (agMax != oldAGMax)
                {
                    if (agMax > oldAGMax && fullAG)
                    {
                        gamePlayer.UnitData.AG = agMax;
                    }
                    // 当前值
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_AG;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.PROP_AG);
                    mChangeValueMessage.Info.Add(mPropertyData);
                    // 最大值
                    mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_AG_MAX;
                    mPropertyData.Value = agMax;
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (sdMax != oldSDMax)
                {
                    if (sdMax > oldSDMax && fullSD)
                    {
                        gamePlayer.UnitData.SD = sdMax;
                    }
                    // 当前值
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_SD;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.PROP_SD);
                    mChangeValueMessage.Info.Add(mPropertyData);
                    // 最大值
                    mPropertyData = new G2C_BattleKVData();
                    mPropertyData.Key = (int)E_GameProperty.PROP_SD_MAX;
                    mPropertyData.Value = sdMax;
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (changeProp.ContainsKey(E_GameProperty.AttackSpeed))
                {
                    // 攻击速度 变动
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    // 当前值
                    mPropertyData.Key = (int)E_GameProperty.AttackSpeed;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.AttackSpeed);
                    mChangeValueMessage.Info.Add(mPropertyData);
                }
                if (changeProp.ContainsKey(E_GameProperty.ExperienceBonus))
                {
                    // 经验加成百分比 变动
                    G2C_BattleKVData mPropertyData = new G2C_BattleKVData();
                    // 当前值
                    mPropertyData.Key = (int)E_GameProperty.ExperienceBonus;
                    mPropertyData.Value = gamePlayer.GetNumerial(E_GameProperty.ExperienceBonus);
                    mChangeValueMessage.Info.Add(mPropertyData);
                }

                if (mChangeValueMessage.Info.Count != 0)
                {
                    mChangeValueMessage.GameUserId = gamePlayer.InstanceId;
                    self.Parent.NotifyAroundPlayer(mChangeValueMessage);
                }
                #endregion
            }


        }
        /// <summary>
        /// 强化套装属性计算
        /// </summary>
        /// <param name="self"></param>
        private static void ApplyEquipSetStrengthen(this EquipmentComponent self)
        {
            var gamePlayer = self.mPlayer.GetCustomComponent<GamePlayer>();
            var heroType = gamePlayer.GameHeroType;
            int targetCount = (heroType == E_GameOccupation.Spellsword || heroType == E_GameOccupation.Combat) ? 4 : 5; //目标装备数
            int curCount = 0; //目前装备数
            int maxLevel = 0;
            foreach (var item in self.EquipPartItemDict)
            {
                if (item.Value.ConfigData.Slot >= 3 && item.Value.ConfigData.Slot <= 7) //5件装备
                {
                    //记录装备中最小Level
                    int curLevel = item.Value.GetProp(EItemValue.Level);
                    if (curLevel >= 9)
                    {
                        curCount++;
                        if (maxLevel == 0)
                        {
                            maxLevel = curLevel;
                        }
                        else
                        {
                            if (maxLevel > curLevel)
                            {
                                maxLevel = curLevel;
                            }
                        }
                    }
                }
            }

            //达不到要求的装备数
            if (curCount < targetCount)
            {
                return;
            }

            //判断强化套装属性
            int attrValue = 0;
            if (maxLevel <= 9)
            {
                attrValue = 5;
            }
            else if (maxLevel <= 11)
            {
                attrValue = 6;
            }
            else if (maxLevel <= 13)
            {
                attrValue = 8;
            }
            else if (maxLevel <= 15)
            {
                attrValue = 10;
            }
            if (attrValue > 0)
            {
                gamePlayer.AddEquipProperty(E_GameProperty.PROP_HP_MAXPct, attrValue);  //HP百分比加成
                gamePlayer.AddEquipProperty(E_GameProperty.MinAtteckPct, attrValue);  //攻击/防御百分比加成
                gamePlayer.AddEquipProperty(E_GameProperty.MaxAtteckPct, attrValue);
                gamePlayer.AddEquipProperty(E_GameProperty.MinMagicAtteckPct, attrValue);
                gamePlayer.AddEquipProperty(E_GameProperty.MaxMagicAtteckPct, attrValue);
                gamePlayer.AddEquipProperty(E_GameProperty.DefensePct, attrValue);  //防御力加成
            }
        }

        /// <summary>
        /// 整套防御力增加
        /// </summary>
        /// <param name="self"></param>
        private static void ApplyEquipKindProp(this EquipmentComponent self)
        {
            GamePlayer gamePlayer = self.mPlayer.GetCustomComponent<GamePlayer>();
            int def = gamePlayer.GetNumerial(E_GameProperty.SpecialDefenseRate);
            if (def > 0)
            {
                // 整套防御力增加
                using ListComponent<Item> equipList = ListComponent<Item>.Create();
                Item equip = null;
                if ((equip = self.GetEquipItemByPosition(EquipPosition.Helmet)) != null)
                {
                    equipList.Add(equip);
                }
                if ((equip = self.GetEquipItemByPosition(EquipPosition.Armor)) != null)
                {
                    equipList.Add(equip);
                }
                else
                {
                    return;
                }
                if ((equip = self.GetEquipItemByPosition(EquipPosition.Leggings)) != null)
                {
                    equipList.Add(equip);
                }
                else
                {
                    return;
                }
                if ((equip = self.GetEquipItemByPosition(EquipPosition.HandGuard)) != null)
                {
                    equipList.Add(equip);
                }
                else
                {
                    return;
                }
                if ((equip = self.GetEquipItemByPosition(EquipPosition.Boots)) != null)
                {
                    equipList.Add(equip);
                }
                else
                {
                    return;
                }

                if ((E_GameOccupation)gamePlayer.Data.PlayerTypeId == E_GameOccupation.Spellsword)
                {
                    // 魔剑士没有头
                    if (equipList.Count < 4) return;
                }
                else
                {
                    if (equipList.Count < 5) return;
                }

                int kindId = equipList[0].ConfigData.KindId;
                for(int i = 1;i< equipList.Count;++i)
                {
                    if (kindId != equipList[i].ConfigData.KindId) return;
                }

                // 条件判断完成 可以添加属性
                gamePlayer.AddEquipProperty(E_GameProperty.Defense, def);
            }
        }
    }
}
