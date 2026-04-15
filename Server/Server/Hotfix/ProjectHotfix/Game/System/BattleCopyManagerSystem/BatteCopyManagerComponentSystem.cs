using ETModel;
using System;
using System.Linq;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Collections.Generic;
using CustomFrameWork.Component;
using System.Threading.Tasks;
using System.Net;


namespace ETHotfix
{
    [EventMethod(typeof(BatteCopyManagerComponent), EventSystemType.INIT)]
    public class BatteCopyManagerComponentEventOnInt : ITEventMethodOnInit<BatteCopyManagerComponent>
    {
        public void OnInit(BatteCopyManagerComponent self)
        {
            self.OnInit();
        }
    }

    public static class BatteCopyManagerComponentSystem
    {
        public static void OnInit(this BatteCopyManagerComponent self)
        {
            //加载对应的配置表 创建副本的数据存入 字典中    
            Dictionary<int, BattleCopy_OpenConfig> mJsonOpenDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleCopy_OpenConfigJson>().JsonDic;
            foreach (var item in mJsonOpenDic)
            {
                BattleCopyComponent battleCopyCpt = Root.CreateBuilder.GetInstance<BattleCopyComponent, BatteCopyManagerComponent>(self);

                battleCopyCpt.Create(item.Value);
                if (item.Value.Id != (int)CopyType.CangBaoTu)
                {
                    battleCopyCpt.Run().Coroutine();
                }
                self.battleCopyMap.Add(item.Key, battleCopyCpt);
            }

            Dictionary<int, BattleCopy_ConditionConfig> mJsonConditionDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleCopy_ConditionConfigJson>().JsonDic;
            for (int i = 0; i < mJsonConditionDic.Count; i++)
            {
                int key = mJsonConditionDic.ElementAt(i).Key;
                BattleCopy_ConditionConfig conditionConfig = mJsonConditionDic.ElementAt(i).Value;
                CopyType copyType = (CopyType)key;
                switch (copyType)
                {
                    case CopyType.DemonSquae:
                        self.demonSquaeNum = conditionConfig.Challenge;
                        break;
                    case CopyType.RedCastle:
                        self.redCastleNum = conditionConfig.Challenge;
                        break;
                        /*case CopyType.AncientBattlefield:
                            self.AncientBattlefield = conditionConfig.Challenge;
                            break;*/
                }
                self.battleCopyMap[key].config = conditionConfig;
            }

        }

        public static BattleCopyComponent Get(this BatteCopyManagerComponent self, int type)
        {
            self.battleCopyMap.TryGetValue(type, out BattleCopyComponent battleCopy);

            if (battleCopy == null) return null;

            return battleCopy;
        }

        public static void BroadcastPlayerBattleCopy(this BatteCopyManagerComponent self, long targetGameUserId)
        {
            //Log.Debug("通知玩家副本状态");
            foreach (var item in self.battleCopyMap)
            {
                //if (item.Value.IsTimelimit) continue;
                item.Value.Broadcast(0, targetGameUserId);
            }
        }

        public static async System.Threading.Tasks.Task<bool> PlayerDeathSend(this BatteCopyManagerComponent self, GamePlayer b_CombatSource, C_ServerArea mServerArea)
        {
            int Type = 0;
            switch (b_CombatSource.UnitData.Index)
            {
                case 100:
                    Type = 2;
                    break;
                case 101:
                    Type = 1;
                    break;
                case 102:
                    Type = 3;
                    break;
                case 110:
                    Type = 4;
                    break;
            }
            BattleCopyComponent battleCopyCpt = self.Get(Type);
            if (battleCopyCpt == null) return false;
            battleCopyCpt.DelSend(b_CombatSource.UnitData.GameUserId);
            return true;
        }
        /// <summary>
        /// 副本传送时退出副本
        /// </summary>
        /// <param name="self"></param>
        /// <param name="b_CombatSource"></param>
        /// <param name="mServerArea"></param>
        /// <returns></returns>
        public static bool PlayerExitCopyHandler(this BatteCopyManagerComponent self, GamePlayer b_CombatSource, C_ServerArea mServerArea)
        {
            self.PlayerDeathSend(b_CombatSource, mServerArea).Coroutine();

            int Type = 0;
            switch (b_CombatSource.UnitData.Index)
            {
                case 100:
                    Type = 2;
                    break;
                case 101:
                    Type = 1;
                    break;
                case 102:
                    Type = 3;
                    break;
                case 110:
                    Type = 4;
                    break;
                default: Type =-1;
                    break;

            }
            if (Type == -1)
            {
                return true;
            }
            BattleCopyComponent battleCopyCpt = self.Get(Type);
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            battleCopyCpt.QuitRoom(b_CombatSource.UnitData.GameUserId);
            battleCopyCpt.RemoveUser(b_CombatSource.UnitData.GameUserId);
            if (Type == 4)
                Log.PLog("RunCopyLog", $"PlayerId:{b_CombatSource.UnitData.GameUserId}RankData:{battleCopyCpt.copyRankDataDic.ContainsKey(b_CombatSource.UnitData.GameUserId)}Player:{battleCopyCpt.players.ContainsKey(b_CombatSource.UnitData.GameUserId)}    藏宝图副本退出");

            return true;
        }
        public static bool PlayerDeathTrialTowerHandler(this BatteCopyManagerComponent self, GamePlayer b_CombatSource, C_ServerArea mServerArea)
        {
            //BattleCopyComponent battleCopyCpt = self.Get((int)CopyType.DemonSquae);
            if (self.TrialTowerList.TryGetValue(b_CombatSource.Player.GameUserId, out var mapComponent))
            {
                var mTransferPointId = 400;
                var mJsonMapDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
                if (mJsonMapDic.TryGetValue(mTransferPointId, out var _TransferPointConfig) != false)
                {
                    if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(_TransferPointConfig.MapId, out var mTargetMapComponent) != false)
                    {
                        if (mTargetMapComponent.TransferPointFindTheWayDic.TryGetValue(mTransferPointId, out var mTransferPointlist) != false)
                        {
                            if (mTransferPointlist != null || mTransferPointlist.Count > 0)
                            {
                                var mPlayerUnitData = b_CombatSource.UnitData;
                                if (mPlayerUnitData != null)
                                {
                                    var mRandomIndex = Help_RandomHelper.Range(0, mTransferPointlist.Count);
                                    var mTransferPoint = mTransferPointlist[mRandomIndex];

                                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                    DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, b_CombatSource.Player.GameAreaId);
                                    // 切换地图
                                    mTargetMapComponent.Switch(b_CombatSource, mTransferPoint.X, mTransferPoint.Y);
                                    dBProxy.Save(mPlayerUnitData).Coroutine();
                                }
                            }
                        }
                    }
                }
                mapComponent.Dispose();
                self.TrialTowerList.Remove(b_CombatSource.Player.GameUserId);
            }
            return true;
        }
        /// <summary>
        /// 角色退出当前副本
        /// </summary>
        /// <param name="self"></param>
        /// <param name="b_CombatSource"></param>
        /// <param name="mServerArea"></param>
        public static bool PlayerDeathHandler(this BatteCopyManagerComponent self, GamePlayer b_CombatSource, C_ServerArea mServerArea)
        {
            //BattleCopyComponent battleCopyCpt = self.Get((int)CopyType.DemonSquae);
            int Type = 0;
            switch (b_CombatSource.UnitData.Index)
            {
                case 100:
                    Type = 2;
                    break;
                case 101:
                    Type = 1;
                    break;
                case 102:
                    Type = 3;
                    break;
                case 110:
                    Type = 4;
                    break;
                default:
                    Type = -1;
                     break;
            }
            if (Type == -1)
            {
                //bool KalimaQuit()
                {
                    var mTransferPointId = 400;
                    var mJsonMapDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
                    if (mJsonMapDic.TryGetValue(mTransferPointId, out var _TransferPointConfig) != false)
                    {
                        if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(_TransferPointConfig.MapId, out var mTargetMapComponent) != false)
                        {
                            if (mTargetMapComponent.TransferPointFindTheWayDic.TryGetValue(mTransferPointId, out var mTransferPointlist) != false)
                            {
                                if (mTransferPointlist != null || mTransferPointlist.Count > 0)
                                {
                                    var mPlayerUnitData = b_CombatSource.UnitData;
                                    if (mPlayerUnitData != null)
                                    {
                                        var mRandomIndex = Help_RandomHelper.Range(0, mTransferPointlist.Count);
                                        var mTransferPoint = mTransferPointlist[mRandomIndex];

                                        DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                        DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, b_CombatSource.Player.GameAreaId);
                                        // 切换地图
                                        mTargetMapComponent.Switch(b_CombatSource, mTransferPoint.X, mTransferPoint.Y);
                                        dBProxy.Save(mPlayerUnitData).Coroutine();

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                BattleCopyComponent battleCopyCpt = self.Get(Type);
                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                battleCopyCpt.QuitRoom(b_CombatSource.UnitData.GameUserId);
                bool result = battleCopyCpt.QuitMap(b_CombatSource.Player, mDBProxyManagerComponent, mServerArea);
                if (!result)
                {
                    return false;
                }
                battleCopyCpt.RemoveUser(b_CombatSource.UnitData.GameUserId);

                return true;
            }
        }
        public static async Task PlayerDeathHandlerAsync(this BatteCopyManagerComponent self, GamePlayer b_CombatSource, C_ServerArea mServerArea)
        {
            await self.PlayerDeathSend(b_CombatSource, mServerArea);
            self.PlayerDeathHandler(b_CombatSource, mServerArea);
        }

        public static void EnemyRebirthHandler(this BatteCopyManagerComponent self, Enemy b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            b_CombatSource.IsReallyDeath = true;

            //波数变化 怪物ID 会发生变化 ，复活时 如果怪物ID不一致，重新创建新的怪物
            if (b_MapComponent.MapId == 101 && b_CombatSource.Config.Id != b_MapComponent.monsterId)
            {
                //EnemyComponent b_EnemyComponent = b_MapComponent.GetCustomComponent<EnemyComponent>();
                //var mEnemy = Root.CreateBuilder.GetInstance<Enemy>(false);
                //var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;
                //mJsonDic.TryGetValue(b_MapComponent.monsterId, out var enemyInfoConfig);
                //if (enemyInfoConfig == null)
                //{
                //    Log.Error($"地图{b_MapComponent.MapId}怪物{b_CombatSource.Config.Id}地图怪物{b_MapComponent.monsterId}");
                //    return;
                //}
                //mEnemy.Awake(b_EnemyComponent);
                //mEnemy.SetConfig(enemyInfoConfig);
                //mEnemy.AfterAwake();
                //mEnemy.SetInstanceId(mEnemy.Id);
                //mEnemy.SourcePosX = b_CombatSource.SourcePosX;
                //mEnemy.SourcePosY = b_CombatSource.SourcePosY;
                //b_EnemyComponent.AllEnemyDic[mEnemy.InstanceId] = mEnemy;
                //var mFindTheWay = b_MapComponent.GetFindTheWay2D(mEnemy.SourcePosX, mEnemy.SourcePosY);
                    

                //C_FindTheWay2D mStartPos = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                //b_MapComponent.QuitMap(mStartPos, b_CombatSource);
                //b_MapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);
            }
            else
            {
                C_FindTheWay2D mStartPos = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                b_MapComponent.QuitMap(mStartPos, b_CombatSource);

                if (b_CombatSource.SourcePoslist != null)
                {// 切换复活点
                    var mIndex = Help_RandomHelper.Range(0, b_CombatSource.SourcePoslist.Count);
                    var mRandomValue = b_CombatSource.SourcePoslist[mIndex];

                    b_CombatSource.SourcePosX = mRandomValue.Item1;
                    b_CombatSource.SourcePosY = mRandomValue.Item2;
                }

                C_FindTheWay2D mEndPos = b_MapComponent.GetFindTheWay2D(b_CombatSource.SourcePosX, b_CombatSource.SourcePosY);
                if (mEndPos.IsStaticObstacle)
                {
                    Log.Error($"Enemy 复活后，复活点 IsStaticObstacle == true mapId:{b_MapComponent.Id}:{b_MapComponent.MapId} enemy.InstanceId:{b_CombatSource.InstanceId} x:{mEndPos.X} y:{mEndPos.Y}");
                    return;
                }

                b_CombatSource.UnitData.Hp = b_CombatSource.GetNumerial(E_GameProperty.PROP_HP_MAX);
                b_CombatSource.IsDeath = false;
                b_CombatSource.IsAttacking = false;
                b_CombatSource.IsReallyDeath = false;
                b_CombatSource.MoveSleepTime = b_CurrentTime + RandomHelper.RandomNumber(1, 4) * 1000;
                b_CombatSource.DeathSleepTime = 0;

                b_MapComponent.MoveSendNotice(null, mEndPos, b_CombatSource);
            }
        }

        public static MapComponent GetRoomMapComponent(this BatteCopyManagerComponent self, int mapId, long gameUserId)
        {
            int type = 0;
            switch (mapId)
            {
                case 101:
                    type = 1;
                    break;
                case 102:
                    type = 3;
                    break;
                case 100:
                    type = 2;
                    break;
                case 110:
                    type = 4;
                    break;
                default:
                    break;
            }
            //int type = mapId == 101 ? 1 : 2;
            BattleCopyComponent battleCopyCpt = self.Get(type);
            if (battleCopyCpt == null)
            {
                if (self.TrialTowerList.ContainsKey(gameUserId))
                    return self.TrialTowerList[gameUserId];
                else
                    return null;
            }
            else
                return battleCopyCpt.Get(gameUserId);
        }
    }
}
