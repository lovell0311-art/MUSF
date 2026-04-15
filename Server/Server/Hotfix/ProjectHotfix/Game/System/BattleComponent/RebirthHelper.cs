using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using UnityEngine;
using System;
using System.Linq;

namespace ETHotfix
{
    public static class RebirthHelper
    {
        #region 玩家角色复活
        /// <summary>
        /// 玩家角色复活 指定地图坐标
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="log"></param>
        /// <param name="reEnterMap">是否要重新进入地图</param>
        public static void Rebirth(GamePlayer unit, MapComponent map, int x, int y, string log, bool reEnterMap = true)
        {
            if (unit.IsDeath == false)
            {
                Log.Error($"a:{unit.Player.UserId} r:{unit.InstanceId} 角色还没死，无法复活 ({log})");
                return;
            }
            // 移除复活组件
            unit.RemoveCustomComponent<RebirthComponent>();
            // 开始复活
            unit.UnitData.Hp = unit.GetNumerial(E_GameProperty.PROP_HP_MAX);
            unit.UnitData.Mp = unit.GetNumerial(E_GameProperty.PROP_MP_MAX);
            unit.UnitData.SD = unit.GetNumerial(E_GameProperty.PROP_SD_MAX);
            unit.UnitData.AG = unit.GetNumerial(E_GameProperty.PROP_AG_MAX);
            unit.IsDeath = false;
            unit.IsAttacking = false;
            unit.DeathSleepTime = 0;


            Log.PLog($"a:{unit.Player.UserId} r:{unit.InstanceId} 复活(map:{map.MapId}:{map.Id} x:{x} y:{y})({log})");

            if (reEnterMap == true)
            {
                unit.SwitchMap(map, x, y);
            }
            else
            {
                // 副本中死亡，会自动传送出来。就不用重复进入地图了
                // 只需要重新设置下角色状态
                if (unit.CurrentMap == null)
                {
                    // 角色不在地图中，强制进入
                    map.Enter(unit, x, y);
                }
            }

            // 将buff重新添加回去
            unit.DataAddPropertyBufferGotoMap(map.GetCustomComponent<BattleComponent>());

            G2C_InitProperty_notice mInitProperty_notice = new G2C_InitProperty_notice();
            mInitProperty_notice.Hp = unit.GetNumerial(E_GameProperty.PROP_HP);
            mInitProperty_notice.Mp = unit.GetNumerial(E_GameProperty.PROP_MP);
            mInitProperty_notice.SD = unit.GetNumerial(E_GameProperty.PROP_SD);
            mInitProperty_notice.AG = unit.GetNumerial(E_GameProperty.PROP_AG);
            mInitProperty_notice.HpMax = unit.GetNumerial(E_GameProperty.PROP_HP_MAX);
            mInitProperty_notice.MpMax = unit.GetNumerial(E_GameProperty.PROP_MP_MAX);
            mInitProperty_notice.SDMax = unit.GetNumerial(E_GameProperty.PROP_SD_MAX);
            mInitProperty_notice.AGMax = unit.GetNumerial(E_GameProperty.PROP_AG_MAX);
            unit.Player.Send(mInitProperty_notice);
        }
        /// <summary>
        /// 玩家角色复活 默认
        /// </summary>
        public static void Rebirth(GamePlayer unit, string log)
        {
            MapComponent map = null;
            RebirthComponent rebirth = unit.GetCustomComponent<RebirthComponent>();
            if (rebirth != null)
            {
                map = rebirth.DeathMap;
            }
            else
            {
                // RebirthComponent 组件是死亡后下一帧(20ms)添加的
                map = unit.CurrentMap;
            }
            Vector2Int rebirthPos = new Vector2Int();

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(unit.Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                Log.Error($"数据异常 找不到区服 无法复活");
                return;
            }
            if (map != null)
            {
                Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(map.MapId, out var mJson);
                if (mJson.IsCopyMap == 1)
                {
                    //mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerExitCopyHandler(unit, mServerArea);
                    mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerDeathHandler(unit, mServerArea);
                    RebirthHelper.Rebirth(
                            unit,
                            unit.CurrentMap,
                            unit.UnitData.X,
                            unit.UnitData.Y,
                            "副本死亡复活2"
                            , reEnterMap: false);
                    return;
                }
                if (map.SpawnSafeFindTheWayDic.Count > 0)
                {
                    var mSafeAreaValues = map.SpawnSafeFindTheWayDic.Values.ToArray();
                    if (mSafeAreaValues.Length > 0)
                    {
                        int mRandomIndex = Help_RandomHelper.Range(0, mSafeAreaValues.Length);
                        var mRandemValueDic = mSafeAreaValues[mRandomIndex];

                        var mSafeAreaDicKeys = mRandemValueDic.Keys.ToArray();
                        int mRandomKeyIndex = Help_RandomHelper.Range(0, mSafeAreaDicKeys.Length);
                        var mRandemKeyValue = mSafeAreaDicKeys[mRandomKeyIndex];
                        var mRandemValue = mRandemValueDic[mRandemKeyValue];

                        var mFindTheWay2D = mRandemValue;

                        if (mFindTheWay2D.Map.MapId != map.MapId)
                        {
                            Log.PLog($"a:{unit.Player.UserId} r:{unit.InstanceId} 复活异常 {mFindTheWay2D.Map.MapId}:{map.MapId} ({log})");
                        }
                        rebirthPos.x = mFindTheWay2D.X;
                        rebirthPos.y = mFindTheWay2D.Y;
                    }
                }
                else
                {
                    map = null;
                }
            }
            if (map == null)
            {
                Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<CreateRole_InfoConfigJson>().JsonDic.TryGetValue(unit.Data.PlayerTypeId, out var mJson);
                map = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(mJson.InitMap);
                var mSafeAreaValues = map.SpawnSafeFindTheWayDic.Values.ToArray();
                if (mSafeAreaValues.Length > 0)
                {
                    int mRandomIndex = Help_RandomHelper.Range(0, mSafeAreaValues.Length);
                    var mRandemValueDic = mSafeAreaValues[mRandomIndex];

                    var mSafeAreaDicKeys = mRandemValueDic.Keys.ToArray();
                    int mRandomKeyIndex = Help_RandomHelper.Range(0, mSafeAreaDicKeys.Length);
                    var mRandemKeyValue = mSafeAreaDicKeys[mRandomKeyIndex];
                    var mRandemValue = mRandemValueDic[mRandemKeyValue];

                    var mFindTheWay2D = mRandemValue;

                    if (mFindTheWay2D.Map.MapId != mJson.InitMap)
                    {
                        Log.PLog($"a:{unit.Player.UserId} r:{unit.InstanceId} 复活异常 {mFindTheWay2D.Map.MapId}:{mJson.InitMap} ({log})");
                    }
                    rebirthPos.x = mFindTheWay2D.X;
                    rebirthPos.y = mFindTheWay2D.Y;
                }
                else
                {
                    throw new Exception($"初始地图没有安全区 mapId:{mJson.InitMap} a:{unit.Player.UserId} r:{unit.InstanceId} ({log})");
                }
            }
            Rebirth(unit, map, rebirthPos.x, rebirthPos.y, log);
        }
        #endregion

        #region 敌人复活
        public static void Rebirth(Enemy unit)
        {
            if (unit.IsDeath == false)
            {
                Log.Error($"enemy:{unit.InstanceId} 敌人还没死，无法复活");
                return;
            }

            RebirthComponent rebirth = unit.GetCustomComponent<RebirthComponent>();
            if (rebirth.DeathMap == null)
            {
                // 死亡前的地图已经不在了
                unit.IsReallyDeath = true;
                return;
            }
            //试炼塔的怪死亡不复活设置为真死
            if (rebirth.DeathMap.MapId == 111)
            {
                unit.IsReallyDeath = true;
                return;
            }
            MapComponent map = rebirth.DeathMap;

            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();

            if (mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(map.MapId, out var mMapConfig) == false)
            {
                unit.IsReallyDeath = true;
                return;
            }

            unit.RemoveCustomComponent<RebirthComponent>();

            if (mMapConfig.IsCopyMap == 1)
            {
                map.Parent.Parent.GetCustomComponent<BatteCopyManagerComponent>().EnemyRebirthHandler(
                    unit,
                    ETModel.ET.TimerComponent.Instance.TimeNow,
                    map);
            }
            else if (unit.Config.Monster_Type == 5)
            {
                unit.IsReallyDeath = true;

                EnemyComponent b_EnemyComponent = map.GetCustomComponent<EnemyComponent>();
                var mNewEnemy = b_EnemyComponent.InitMapBossEnemy(MapManageComponent.MapEnemySpawnPathCacheDic[map.MapId][200]);
                if (mNewEnemy != null)
                {
                    G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                    mMoveMessageNotice.X = mNewEnemy.UnitData.X;
                    mMoveMessageNotice.Y = mNewEnemy.UnitData.Y;
                    mMoveMessageNotice.ViewId = 10;
                    mMoveMessageNotice.MapId = mNewEnemy.UnitData.Index;
                    mMoveMessageNotice.ModelId = unit.Config.Id;

                    map.SendNoticeByServer(mMoveMessageNotice).Coroutine();
                }
            }
            else
            {
                if (unit.SourcePoslist != null)
                {// 切换复活点
                    var mIndex = Help_RandomHelper.Range(0, unit.SourcePoslist.Count);
                    var mRandomValue = unit.SourcePoslist[mIndex];

                    unit.SourcePosX = mRandomValue.Item1;
                    unit.SourcePosY = mRandomValue.Item2;
                }

                C_FindTheWay2D mEndPos = map.GetFindTheWay2D(unit.SourcePosX, unit.SourcePosY);
                if (mEndPos.IsStaticObstacle)
                {
                    Log.Error($"Enemy 复活后，复活点 IsStaticObstacle == true mapId:{map.Id}:{map.MapId} enemy.InstanceId:{unit.InstanceId} x:{mEndPos.X} y:{mEndPos.Y}");
                    return;
                }
                C_FindTheWay2D mStartPos = map.GetFindTheWay2D(unit);

                unit.UnitData.Hp = unit.GetNumerial(E_GameProperty.PROP_HP_MAX);
                unit.IsDeath = false;
                unit.IsAttacking = false;

                unit.MoveSleepTime = ETModel.ET.TimerComponent.Instance.TimeNow + Help_RandomHelper.Range(1, 4) * 1000;
                unit.DeathSleepTime = 0;
                unit.Pathlist = null;

                unit.RemoveAllHealthState(null);

                map.QuitMap(mStartPos, unit);
                map.MoveSendNotice(null, mEndPos, unit);

                unit.DataAddPropertyBuffer();

                if (unit.Config.Monster_Type == 1)
                {
                    G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                    mMoveMessageNotice.X = mEndPos.X;
                    mMoveMessageNotice.Y = mEndPos.Y;
                    mMoveMessageNotice.ViewId = 10;
                    mMoveMessageNotice.MapId = mEndPos.Map.MapId;
                    mMoveMessageNotice.ModelId = unit.Config.Id;

                    map.SendNoticeByServer(mMoveMessageNotice).Coroutine();
                }
            }
        }
        #endregion

        #region 宠物复活
        public static void Rebirth(Pets unit)
        {
            if (unit.IsDeath == false)
            {
                Log.Error($"r:{unit.InstanceId} 宠物还没死，无法复活");
                return;
            }
            unit.RemoveCustomComponent<RebirthComponent>();

            unit.IsDeath = false;
            unit.IsAttacking = false;
            unit.dBPetsData.DeathTime = 0;
            unit.DeathSleepTime = 0;
            unit.MoveNeedTime = 0;
            unit.dBPetsData.PetsHP = unit.GetNumerial(E_GameProperty.PROP_HP_MAX);
            unit.dBPetsData.PetsMP = unit.GetNumerial(E_GameProperty.PROP_MP_MAX);
            unit.Pathlist = null;

            unit.CurrentMap?.Leave(unit);

            unit.DataAddPropertyBufferGotoMap(unit.GamePlayer.CurrentMap.GetCustomComponent<BattleComponent>());

            if (unit.GamePlayer != null &&
                unit.GamePlayer.Pets != null &&
                unit.GamePlayer.Pets.Id == unit.Id)
            {
                G2C_AttributeChangeMessage g2C_AttributeChangeMessage = new G2C_AttributeChangeMessage();
                g2C_AttributeChangeMessage.PetsName = unit.dBPetsData.PetsName;
                g2C_AttributeChangeMessage.PetsHP = unit.GetNumerialFunc(E_GameProperty.PROP_HP);
                g2C_AttributeChangeMessage.PetsHPMax = unit.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                g2C_AttributeChangeMessage.PetsMP = unit.GetNumerialFunc(E_GameProperty.PROP_MP);
                g2C_AttributeChangeMessage.PetsMPMax = unit.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                g2C_AttributeChangeMessage.IsDeath = unit.IsDeath ? 1 : 0;
                unit.GamePlayer.Player.Send(g2C_AttributeChangeMessage);

                // 是第一个宠物，才会自动进入地图
                unit.GamePlayer.CurrentMap.Enter(
                    unit,
                    unit.GamePlayer.Position);

                var equipComponent = unit.GamePlayer.Player.GetCustomComponent<EquipmentComponent>();
                if (equipComponent != null)
                {
                    equipComponent.ApplyEquipProp();
                }
            }
        }
        #endregion

        public static void Rebirth(Summoned unit, MapComponent map, int x, int y)
        {

        }
    }
}
