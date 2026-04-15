
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
    /// 赤焰兽孵化蛋
    /// </summary>
    [ItemUseRule(typeof(Use310076))]
    public class Use310076 : C_ItemUseRule<Player, Item, IResponse>
    {
        const int DisposeTime = 1800000;
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 2803;
                return;
            }
            int Lv = b_Player.GetCustomComponent<GamePlayer>().Data.Level;
            if (Lv < 100)
            {
                b_Response.Error = 1600;
                return;
            }
            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();
            MapManageComponent mapManage = mServerArea.GetCustomComponent<MapManageComponent>();
            int randomIndex = RandomHelper.RandomNumber(1, 4);//随机1~3地图 勇者 仙踪 幻术
            MapComponent map = mapManage.GetMapByMapIndex(randomIndex);
            int pointMaxID = 0;
            int randomPointID = 0;
            int PosX = -1;
            int PosY = -1;

            switch (randomIndex)
            {
                case ConstMapId.YongZheDaLu:
                    var yongDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Yong_ShuaGuaiConfigJson>().JsonDic;
                    pointMaxID = yongDic.Count;
                    randomPointID = RandomHelper.RandomNumber(1, pointMaxID + 1);
                    if (yongDic.TryGetValue(randomPointID, out var value))
                    {
                        PosX = value.PosX;
                        PosY = value.PosY;
                    }
                    break;
                case ConstMapId.XianZongLin:
                    var xianDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<XianLin_ShuaGuaiConfigJson>().JsonDic;
                    pointMaxID = xianDic.Count;
                    randomPointID = RandomHelper.RandomNumber(1, pointMaxID + 1);
                    if (xianDic.TryGetValue(randomPointID, out var value2))
                    {
                        PosX = value2.PosX;
                        PosY = value2.PosY;
                    }
                    break;
                case ConstMapId.HuanShuYuan:
                    var huanDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<HuanShuYuan_ShaGuaiConfigJson>().JsonDic;
                    pointMaxID = huanDic.Count;
                    randomPointID = RandomHelper.RandomNumber(1, pointMaxID + 1);
                    if (huanDic.TryGetValue(randomPointID, out var value3))
                    {
                        PosX = value3.PosX;
                        PosY = value3.PosY;
                    }
                    break;
                default:
                    b_Response.Error = 99;
                    return;
            }
            if (PosX == -1 || PosY == -1)
            {
                b_Response.Error = 2805;
                return;
            }

            var enemyJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;
            List<int> enemyList = null;

            //if (100 <= Lv && Lv <= 200)
            //    enemyList = new List<int>() { 570, 571 };
            //else if (201 <= Lv && Lv <= 300)
            //    enemyList = new List<int>() { 572, 573, 574 };
            //else //if(301 <= Lv)
            //    enemyList = new List<int>() { 575, 576, 577 };
            if (100 <= Lv && Lv <= 200)
                enemyList = new List<int>() { 562, 563 };
            else if (201 <= Lv && Lv <= 300)
                enemyList = new List<int>() { 564, 565, 566 };
            else //if(301 <= Lv)
                enemyList = new List<int>() { 567, 568, 569 };

            var Point = new Vector2Int(PosX, PosY);
            int r = new Random().Next(enemyList.Count);
            int MobId = enemyList[r];
            (int, int) Id = (12008, 30);
            Enemy monster = CreateMonster(map, enemyJsonDic[MobId], PosX, PosY, b_Player.GameUserId,12,Id, b_Item.ItemUID);
            G2C_CangBaoTuPosUpdate_notice notice2 = new G2C_CangBaoTuPosUpdate_notice()
            {
                MapIndex = randomIndex,
                PosX = PosX,
                PosY = PosY,
                NpcConfigID = MobId,
                TreasureType = ETreasureType.FuHuaDan,
                Id = b_Item.ItemUID,
            };
            mGamePlayer.SetMGDropItem(Id);
            b_Player.Send(notice2);

            b_Item.SetProp(EItemValue.TreasureMapId, randomIndex);
            b_Item.SetProp(EItemValue.TreasureZoneId, mServerArea.GameAreaRouteId);
            b_Item.SetProp(EItemValue.TreasureNpcConfigId, MobId);
            b_Item.SetProp(EItemValue.TreasureKeyIdA, (int)(monster.InstanceId >> 32));
            b_Item.SetProp(EItemValue.TreasureKeyIdB, (int)(monster.InstanceId & 0xffffffff));
            b_Item.SetProp(EItemValue.TreasurePosX, PosX);
            b_Item.SetProp(EItemValue.TreasurePosY, PosY);
            b_Item.SetProp(EItemValue.IsUsing, 1);
            b_Item.OnlySaveDB();
            b_Item.SendAllPropertyData(b_Player);

            b_Response.Error = 0;
            return;
        }



        /// <summary>
        /// 生成副本怪物
        /// </summary>
        /// <param name="mMapComponent"></param>
        /// <param name="randomIndex"></param>
        private Enemy CreateMonster(MapComponent mMapComponent, Enemy_InfoConfig enemyConfig, int posX, int posY, long GamePlayerId, int DropId,(int,int) Id, long itemUid)
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
            mEnemy.CreatePlayerId = GamePlayerId;
            mEnemy.CreateItemUID = itemUid;
            mEnemy.DropId = DropId;
            mEnemy.MGItem = Id;
            b_EnemyComponent.AllEnemyDic[mEnemy.InstanceId] = mEnemy;

            var mFindTheWay = mMapComponent.GetFindTheWay2D(posX, posY);
            mMapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);

            return mEnemy;

        }
    }
}