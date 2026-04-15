using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.EventType;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{

    [EventMethod("PlayerReadyComplete")]
    public class PlayerReadyComplete_TreasureMap : ITEventMethodOnRun<PlayerReadyComplete>
    {
        public void OnRun(PlayerReadyComplete args)
        {
            InitTreasureMap(args.player);
            InitTreasure(args.player, 310075,ETreasureType.LongWangBaoZang);
            InitTreasure(args.player, 310076, ETreasureType.FuHuaDan);
            InitTreasure(args.player, 310086, ETreasureType.NaJie);
            InitTreasure(args.player, 310087, ETreasureType.XiaoTianShi);
        }

        #region 藏宝图
        public void InitTreasureMap(Player player)
        {
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            var allItems = backpack.GetAllItemByConfigID(310073);      // 藏宝图
            if (allItems == null || allItems.Count == 0) return;


            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(player.SourceGameAreaId);
            MapManageComponent mapManage = mServerArea.GetCustomComponent<MapManageComponent>();
            BatteCopyManagerComponent copyManager = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            var copyComponent = copyManager.Get((int)CopyType.CangBaoTu);
            //先检查物品是否已经开启了副本
            foreach (var roomList in copyComponent.battleCopyRoomDic.Values)
            {
                if (roomList.Count > 0)
                {
                    var curCopyRoom = roomList[0];
                    if(allItems.TryGetValue(curCopyRoom.BelongItemID,out Item item))
                    {
                        item.SetProp(EItemValue.TreasurePosX, curCopyRoom.BelongGameNpc.X);
                        item.SetProp(EItemValue.TreasurePosY, curCopyRoom.BelongGameNpc.Y);
                        item.SetProp(EItemValue.IsUsing, 1);
                        item.SendAllPropertyData(player);
                        item.AddCustomComponent<ItemTimeLimitComponent>();

                        //通知玩家坐标
                        G2C_CangBaoTuPosUpdate_notice notice2 = new G2C_CangBaoTuPosUpdate_notice()
                        {
                            MapIndex = curCopyRoom.NpcMapId,
                            PosX = curCopyRoom.BelongGameNpc.X,
                            PosY = curCopyRoom.BelongGameNpc.Y,
                            NpcConfigID = curCopyRoom.BelongGameNpc.Config.Id,
                            TreasureType = ETreasureType.TreasureMap,
                            Id = item.ItemUID,
                        };
                        player.Send(notice2);
                    }
                }
            }

            foreach (Item item in allItems.Values)
            {
                if(item.GetProp(EItemValue.IsUsing) == 0)
                {
                    int zoneId = item.GetProp(EItemValue.TreasureZoneId);
                    if (zoneId != 0)
                    {
                        if (zoneId != mServerArea.GameAreaRouteId)
                        {
                            item.SetProp(EItemValue.IsUsing, 1);
                            item.SendAllPropertyData(player);
                            item.AddCustomComponent<ItemTimeLimitComponent>();
                        }
                        else if(item.GetProp(EItemValue.TimeLimit) < TimeHelper.ClientNowSeconds())
                        {
                            item.SetProp(EItemValue.IsUsing, 1);
                            item.SendAllPropertyData(player);
                            item.AddCustomComponent<ItemTimeLimitComponent>();
                        }
                        else
                        {
                            item.SetProp(EItemValue.IsUsing, 0);
                            // 服务器重启，藏宝图已失效
                            item.SetProp(EItemValue.TimeLimit, 0);
                            item.SetProp(EItemValue.TreasureMapId, 0);
                            item.SetProp(EItemValue.TreasureZoneId, 0);
                            item.SetProp(EItemValue.TreasureNpcConfigId, 0);
                            item.SetProp(EItemValue.TreasurePosX, 0);
                            item.SetProp(EItemValue.TreasurePosY, 0);
                            item.OnlySaveDB();
                            item.SendAllPropertyData(player);
                        }
                    }
                    
                }
            }
        }
        #endregion

        #region 宝藏

        public void InitTreasure(Player player,int configId, int treasureType)
        {
            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            var allItems = backpack.GetAllItemByConfigID(configId);      // 藏宝图
            if (allItems == null || allItems.Count == 0) return;

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(player.SourceGameAreaId);
            MapManageComponent mapManage = mServerArea.GetCustomComponent<MapManageComponent>();


            using ListComponent<Item> closeItems = ListComponent<Item>.Create();    // 无效的物品，可能服务器重启了
            foreach(Item item in allItems.Values)
            {
                int zoneId = item.GetProp(EItemValue.TreasureZoneId);
                if (zoneId != 0)
                {
                    if (zoneId == mServerArea.GameAreaRouteId)
                    {
                        int mapId = item.GetProp(EItemValue.TreasureMapId);
                        MapComponent map = mapManage.GetMapByMapIndex(mapId);
                        if(map == null)
                        {
                            closeItems.Add(item);
                            continue;
                        }
                        long enemyId = item.GetProp(EItemValue.TreasureKeyIdA);
                        enemyId = enemyId << 32;
                        enemyId |= (long)item.GetProp(EItemValue.TreasureKeyIdB);
                        EnemyComponent enemyCom = map.GetCustomComponent<EnemyComponent>();
                        if(!enemyCom.AllEnemyDic.TryGetValue(enemyId,out Enemy monster))
                        {
                            closeItems.Add(item);
                            continue;
                        }
                        item.SetProp(EItemValue.TreasurePosX, monster.Position.x);
                        item.SetProp(EItemValue.TreasurePosY, monster.Position.y);
                        item.SetProp(EItemValue.IsUsing, 1);
                        item.SendAllPropertyData(player);

                        //通知玩家坐标
                        G2C_CangBaoTuPosUpdate_notice notice2 = new G2C_CangBaoTuPosUpdate_notice()
                        {
                            MapIndex = mapId,
                            PosX = monster.Position.x,
                            PosY = monster.Position.y,
                            NpcConfigID = item.GetProp(EItemValue.TreasureNpcConfigId),
                            TreasureType = treasureType,
                            Id = item.ItemUID,
                        };
                        player.Send(notice2);
                    }
                    else
                    {
                        item.SetProp(EItemValue.IsUsing, 1);
                        item.SendAllPropertyData(player);
                    }

                }
            }

            foreach(Item item in closeItems)
            {
                item.SetProp(EItemValue.IsUsing, 0);
                // 服务器重启，宝藏已失效
                item.SetProp(EItemValue.TreasureMapId, 0);
                item.SetProp(EItemValue.TreasureZoneId, 0);
                item.SetProp(EItemValue.TreasureNpcConfigId, 0);
                item.SetProp(EItemValue.TreasureKeyIdA, 0);
                item.SetProp(EItemValue.TreasureKeyIdB, 0);
                item.SetProp(EItemValue.TreasurePosX, 0);
                item.SetProp(EItemValue.TreasurePosY, 0);
                item.OnlySaveDB();
                item.SendAllPropertyData(player);
            }
        }

        #endregion
    }
}
