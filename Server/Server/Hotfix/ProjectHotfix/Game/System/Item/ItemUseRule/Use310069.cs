
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using static ETModel.CombatSource;

namespace ETHotfix
{
    /// <summary>
    /// 经验印章
    /// </summary>
    [ItemUseRule(typeof(Use310069))]
    public class Use310069 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            // 使用道具可以在短是时间内增加基本经验值的200%，30分0秒,可使用。

            var mCurrentTemp = b_Player.GetCustomComponent<GamePlayer>();

            //if (mCurrentTemp.Data.OccupationLevel >= 3)
            //{
            //    b_Response.Error = 725;
            //    return;
            //}

            if (mCurrentTemp.HealthStatsDic.ContainsKey(E_BattleSkillStats.Use310069) || mCurrentTemp.HealthStatsDic.ContainsKey(E_BattleSkillStats.Use310070))
            {
                b_Response.Error = 743;
                return;
            }

            var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, b_Player.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            var mBattleComponent = mMapComponent.GetCustomComponent<BattleComponent>();

            Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
            {
                var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                mBattleSyncTimer.SyncWaitTime = b_Item.ConfigData.Value2;
                mBattleSyncTimer.NextWaitTime = mBattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                {
                    if (b_CombatSource.IsDisposeable) return;

                    b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310069, b_BattleComponent);
                    b_CombatSource.UpdateHealthState();
                };
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310069, out var mCurrentHealthStats) == false)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }

                    if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }
                    if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }
                    else
                    {
                        b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                        b_CombatSource.AddTask(b_TimerTask);
                    }
                    return CombatSource.E_SyncTimerTaskResult.NextRound;
                };
                return mBattleSyncTimer;
            };

            mCurrentTemp.AddHealthState(E_BattleSkillStats.Use310069, b_Item.ConfigData.Value, b_Item.ConfigData.Value2, 0, mCreateFunc, mBattleComponent);
            mCurrentTemp.UpdateHealthState();

            mCurrentTemp.Data.DBBufflist[(int)E_BattleSkillStats.Use310069] = mBattleComponent.CurrentTimeTick + b_Item.ConfigData.Value2;
            mCurrentTemp.Data.Serialize();

            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mCurrentTemp.Data, dBProxy).Coroutine();

            return;
        }
    }

    [ItemUseRule(typeof(Use310124))]
    public class Use310124 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            // 使用道具可以在短是时间内增加基本经验值的200%，30分0秒,可使用。

            var mCurrentTemp = b_Player.GetCustomComponent<GamePlayer>();

            if (mCurrentTemp.Data.Level >= 400)
            {
                b_Response.Error = 725;
                return;
            }

            //if (mCurrentTemp.HealthStatsDic.ContainsKey(E_BattleSkillStats.Use310069) || mCurrentTemp.HealthStatsDic.ContainsKey(E_BattleSkillStats.Use310070))
            //{
            //    b_Response.Error = 743;
            //    return;
            //}

            var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, b_Player.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            var mBattleComponent = mMapComponent.GetCustomComponent<BattleComponent>();

            Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
            {
                var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
                mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                long Tiem = b_Item.ConfigData.Value2;
                mBattleSyncTimer.SyncWaitTime = Tiem * 1000;
                mBattleSyncTimer.NextWaitTime = mBattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                {
                    if (b_CombatSource.IsDisposeable) return;

                    b_CombatSource.RemoveHealthState(E_BattleSkillStats.Use310069, b_BattleComponent);
                    b_CombatSource.UpdateHealthState();
                };
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.Use310069, out var mCurrentHealthStats) == false)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }

                    if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }
                    if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }
                    else
                    {
                        b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

                        b_CombatSource.AddTask(b_TimerTask);
                    }
                    return CombatSource.E_SyncTimerTaskResult.NextRound;
                };
                return mBattleSyncTimer;
            };

            long Tiem = b_Item.ConfigData.Value2;
            if (mCurrentTemp.HealthStatsDic.ContainsKey(E_BattleSkillStats.Use310069))
                mCurrentTemp.AddHealthState(E_BattleSkillStats.Use310069, b_Item.ConfigData.Value, Tiem * 1000, 0, mCreateFunc, mBattleComponent, E_SkillStatsResetType.RESET_NUMBER);
            else
                mCurrentTemp.AddHealthState(E_BattleSkillStats.Use310069, b_Item.ConfigData.Value, Tiem * 1000, 0, mCreateFunc, mBattleComponent);

            mCurrentTemp.UpdateHealthState();

            mCurrentTemp.Data.DBBufflist.TryGetValue((int)E_BattleSkillStats.Use310069, out var Value);
            if (mBattleComponent.CurrentTimeTick > Value)
                Value = mBattleComponent.CurrentTimeTick;
            mCurrentTemp.Data.DBBufflist[(int)E_BattleSkillStats.Use310069] = Value + Tiem * 1000;
            mCurrentTemp.Data.Serialize();

            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mCurrentTemp.Data, dBProxy).Coroutine();

            return;
        }
    }
}