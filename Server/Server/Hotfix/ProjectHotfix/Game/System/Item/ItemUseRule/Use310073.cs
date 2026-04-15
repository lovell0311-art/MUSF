
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 藏宝图
    /// </summary>
    [ItemUseRule(typeof(Use310073))]
    public class Use310073 : C_ItemUseRule<Player, Item, IResponse>
    {
        const int DisposeTime = 1800000;
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            
            
            List<int> randomNPCID = new List<int>() { 10045, 10046, 10047,10054,10055,10056, 10057,10058 };
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                // 参数错误
                b_Response.Error = 2803;
                return;
            }
            if(b_Item.GetProp(EItemValue.IsUsing) == 1)
            {
                // 藏宝图已开启副本
                b_Response.Error = 2804;
                return;
            }
            
            MapManageComponent mapManage = mServerArea.GetCustomComponent<MapManageComponent>();
            BatteCopyManagerComponent copyManager = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            var copyComponent = copyManager.Get((int)CopyType.CangBaoTu);
 
            //开启副本
            int randomIndex = RandomHelper.RandomNumber(1, 4);//随机1~3地图 勇者 仙踪 幻术
            MapComponent map = mapManage.GetMapByMapIndex(randomIndex);
            int pointMaxID = 0;
            int randomPointID = 0;
            int npcPosX = -1;
            int npcPosY = -1;
            //随机NPC生成的点位
            switch (randomIndex)
            {
                case ConstMapId.YongZheDaLu:
                    var yongDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Yong_ShuaGuaiConfigJson>().JsonDic;
                    pointMaxID = yongDic.Count;
                    randomPointID = RandomHelper.RandomNumber(1, pointMaxID + 1);
                    if (yongDic.TryGetValue(randomPointID, out var value))
                    {
                        npcPosX = value.PosX;
                        npcPosY = value.PosY;
                    }
                    break;
                case ConstMapId.XianZongLin:
                    var xianDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<XianLin_ShuaGuaiConfigJson>().JsonDic;
                    pointMaxID = xianDic.Count;
                    randomPointID = RandomHelper.RandomNumber(1, pointMaxID + 1);
                    if (xianDic.TryGetValue(randomPointID, out var value2))
                    {
                        npcPosX = value2.PosX;
                        npcPosY = value2.PosY;
                    }
                    break;
                case ConstMapId.HuanShuYuan:
                    var huanDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<HuanShuYuan_ShaGuaiConfigJson>().JsonDic;
                    pointMaxID = huanDic.Count;
                    randomPointID = RandomHelper.RandomNumber(1, pointMaxID + 1);
                    if (huanDic.TryGetValue(randomPointID, out var value3))
                    {
                        npcPosX = value3.PosX;
                        npcPosY = value3.PosY;
                    }
                    break;
                default:
                    b_Response.Error = 99;
                    return;
            }
            if (npcPosX == -1 || npcPosY == -1)
            {
                b_Response.Error = 2805;
                return;
            }
            //随机生成副本NPC
            int copyRandomIndex = RandomHelper.RandomNumber(0, randomNPCID.Count);
            int npcID = randomNPCID[copyRandomIndex];
            //创建副本NPC
            GameNpc gameNpc = await CreateNPC(map, npcID, npcPosX, npcPosY, b_Player);
            if (gameNpc == null)
            {
                b_Response.Error = 2806;
                return;
            }
            //开启副本地图
            
            BattleCopyRoom battleCopyRoom = Root.CreateBuilder.GetInstance<BattleCopyRoom, BattleCopyComponent>(copyComponent);
            MapManageComponent mapManageComponent = mServerArea.GetCustomComponent<MapManageComponent>();
            battleCopyRoom.mapComponent = mapManageComponent.Copy(copyComponent.DecisionTransferPointId((int)CopyType.CangBaoTu));
            battleCopyRoom.level = 0;
            battleCopyRoom.round = 0;
            battleCopyRoom.BelongGameUserId = b_Player.GameUserId;
            battleCopyRoom.BelongItemID = b_Item.ItemUID;
            battleCopyRoom.BelongGameNpc = gameNpc;
            battleCopyRoom.NpcMapId = randomIndex;
            battleCopyRoom.mapComponent.Awake(mapManageComponent);
            Dictionary<int,BattleCopyRoom> battleCopyRooms = new Dictionary<int, BattleCopyRoom>();
            battleCopyRooms.Add(0,battleCopyRoom);
            copyComponent.battleCopyRoomDic.Add(gameNpc.Id, battleCopyRooms);
            battleCopyRoom.MobBossId = copyRandomIndex;
            ////创建怪物
            //var enemyList = new List<List<int>> { new List<int>() { 213, 138, 139, 140, 141 },
            //                                      new List<int>() { 547, 305, 306, 307, 309 },
            //                                      new List<int>() { 548, 397, 399, 510, 400 }};
            //var enemyPosList = new List<Vector2Int> { new Vector2Int(64, 50), new Vector2Int(64, 48), new Vector2Int(64, 52), new Vector2Int(62, 59), new Vector2Int(66, 59) };
            //var enemyJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;
            ////每个点放入怪物
            //try
            //{
            //    for (int i = 0; i < enemyPosList.Count; i++)
            //    {
            //        Vector2Int curPos = enemyPosList[i];
            //        if (enemyJsonDic.TryGetValue(enemyList[copyRandomIndex][i], out var curEnemyConfig) == false)
            //        {
            //            b_Response.Error = 2807;
            //            return;
            //        }

            //        CreateMonster(battleCopyRoom.mapComponent, curEnemyConfig, curPos.x, curPos.y);
            //    }
            //}
            //catch (Exception e)
            //{
            //    b_Response.Error = 2808;
            //    return;
            //}
            long time = Help_TimeHelper.GetNowSecond() + 1800;
            b_Item.SetProp(EItemValue.TimeLimit, (int)time);
            b_Item.SetProp(EItemValue.TreasureMapId, randomIndex);
            b_Item.SetProp(EItemValue.TreasureZoneId, mServerArea.GameAreaRouteId);
            b_Item.SetProp(EItemValue.TreasureNpcConfigId, npcID);
            b_Item.SetProp(EItemValue.TreasurePosX, npcPosX);
            b_Item.SetProp(EItemValue.TreasurePosY, npcPosY);
            b_Item.SetProp(EItemValue.IsUsing, 1);
            b_Item.OnlySaveDB();
            b_Item.SendAllPropertyData(b_Player);

            b_Item.AddCustomComponent<ItemTimeLimitComponent>();
            //通知玩家坐标
            G2C_CangBaoTuPosUpdate_notice notice = new G2C_CangBaoTuPosUpdate_notice()
            {
                MapIndex = randomIndex,
                PosX = npcPosX, PosY = npcPosY,
                NpcConfigID = gameNpc.Config.Id,
                TreasureType = ETreasureType.TreasureMap,
                Id = b_Item.ItemUID,
            };
            b_Player.Send(notice);

            return;
        }

        /// <summary>
        /// 生成NPC，并且设定30分钟后消失
        /// </summary>
        /// <param name="npcID"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        private async Task<GameNpc> CreateNPC(MapComponent map,int npcID,int posX,int posY,Player player)
        {
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Npc_InfoConfigJson>().JsonDic;
            var npcMapComponenot = map.GetCustomComponent<NpcComponent>();
            if (mJsonDic.TryGetValue(npcID, out var mNpcConfig))
            {
                var mNpc = Root.CreateBuilder.GetInstance<GameNpc, long>(Help_UniqueValueHelper.GetServerUniqueValue());
                mNpc.SetConfig(mNpcConfig);
                mNpc.AfterAwake();
                npcMapComponenot.AllNpcDic[mNpc.Id] = mNpc;

                // 地图
                var mCellFieldTemp = map.GetMapCellFieldByPos(posX, posY);
                if (mCellFieldTemp != null)
                {
                    // 地图分域
                    mNpc.X = posX;
                    mNpc.Y = posY;
                    mNpc.Angle = 0;
                    mCellFieldTemp.FieldNpcDic[mNpc.Id] = mNpc;
                }
                var findPos = map.GetFindTheWay2D(posX, posY);
                map.MoveSendNotice(findPos, findPos, mNpc);
                //todo:替换掉这里的逻辑，引入计时器开始30分钟计时，计时结束销毁NPC、副本和藏宝图物品
                //await Root.MainFactory.GetCustomComponent<TimerComponent>().WaitAsync(DisposeTime);
                //if (mNpc != null)
                //{
                //    map.QuitMap(findPos, mNpc);

                //    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                //    mAttackResultNotice.AttackTarget = player.GameUserId;
                //    mAttackResultNotice.HpValue = 0;
                //    map.SendNotice(findPos, mAttackResultNotice);
                //    mNpc.Dispose();
                //}
                return mNpc;
            }
            return null;
        }


        /// <summary>
        /// 生成副本怪物
        /// </summary>
        /// <param name="mMapComponent"></param>
        /// <param name="randomIndex"></param>
        private void CreateMonster(MapComponent mMapComponent,Enemy_InfoConfig enemyConfig,int posX,int posY)
        {
            EnemyComponent b_EnemyComponent = mMapComponent.EnsureGetCustomComponent<EnemyComponent>();
            
            var mEnemy = Root.CreateBuilder.GetInstance<Enemy>(false);
            mEnemy.Awake(b_EnemyComponent);

            mEnemy.SetConfig(enemyConfig);
            mEnemy.AfterAwake();
            mEnemy.AwakeSkill();
            mEnemy.DataUpdateSkill();

            mEnemy.SetInstanceId(mEnemy.Id);

            mEnemy.SourcePosX = posX;
            mEnemy.SourcePosY = posY;

            b_EnemyComponent.AllEnemyDic[mEnemy.InstanceId] = mEnemy;

            var mFindTheWay = mMapComponent.GetFindTheWay2D(posX, posY);
            mMapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);
        }
    }
}