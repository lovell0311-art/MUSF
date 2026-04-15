
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 樱花饼
    /// </summary>
    [ItemUseRule(typeof(Use310062))]
    public class Use310062 : C_ItemUseRule<Player, Item, IResponse>
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

            // 最大生命值增加700，30分0秒,可使用

            var mCurrentTemp = b_Player.GetCustomComponent<GamePlayer>();

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

                    if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaBing310062, out var hp_Curse))
                    {
                        b_CombatSource.RemoveHealthState(E_BattleSkillStats.UseYingHuaBing310062, b_BattleComponent);
                        b_CombatSource.UpdateHealthState();

                        var mHpmax = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                        if (b_CombatSource.UnitData.Hp > mHpmax)
                        {
                            b_CombatSource.UnitData.Hp = mHpmax;
                        }

                        void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                        {
                            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                            mBattleKVData.Key = (int)b_GameProperty;
                            mBattleKVData.Value = b_CombatSource.GetNumerialFunc(b_GameProperty);
                            b_ChangeValue_notice.Info.Add(mBattleKVData);
                        }
                        G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
                        mChangeValueMessage.GameUserId = b_CombatSource.InstanceId;

                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
                        AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

                        b_BattleComponent.Parent.SendNotice(b_CombatSource, mChangeValueMessage);
                    }
                };
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.UseYingHuaBing310062, out var mCurrentHealthStats) == false)
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

            mCurrentTemp.AddHealthState(E_BattleSkillStats.UseYingHuaBing310062, b_Item.ConfigData.Value, b_Item.ConfigData.Value2, 0, mCreateFunc, mBattleComponent);
            mCurrentTemp.UpdateHealthState();

            mCurrentTemp.Data.DBBufflist[(int)E_BattleSkillStats.UseYingHuaBing310062] = mBattleComponent.CurrentTimeTick + b_Item.ConfigData.Value2;
            mCurrentTemp.Data.Serialize();

            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mCurrentTemp.Data, dBProxy).Coroutine();

            mCurrentTemp.UnitData.Hp += b_Item.ConfigData.Value;
            var mCurrentTempHpmax = mCurrentTemp.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
            if (mCurrentTemp.UnitData.Hp > mCurrentTempHpmax)
            {
                mCurrentTemp.UnitData.Hp = mCurrentTempHpmax;
            }

            void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
            {
                G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                mBattleKVData.Key = (int)b_GameProperty;
                mBattleKVData.Value = mCurrentTemp.GetNumerial(b_GameProperty);
                b_ChangeValue_notice.Info.Add(mBattleKVData);
            }

            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            mChangeValueMessage.GameUserId = mCurrentTemp.InstanceId;

            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP);
            AddPropertyNotice(mChangeValueMessage, E_GameProperty.PROP_HP_MAX);

            mMapComponent.SendNotice(mCurrentTemp, mChangeValueMessage);

            return;
        }
    }
}