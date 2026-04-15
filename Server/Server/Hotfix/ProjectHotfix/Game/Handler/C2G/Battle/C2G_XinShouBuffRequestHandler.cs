using ETModel;
using System;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using UnityEngine;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_XinShouBuffRequestHandler : AMActorRpcHandler<C2G_XinShouBuffRequest, G2C_XinShouBuffResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_XinShouBuffRequest b_Request, G2C_XinShouBuffResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_XinShouBuffRequest b_Request, G2C_XinShouBuffResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            var mCurrentTemp = mPlayer.GetCustomComponent<GamePlayer>();

            if (mCurrentTemp.Data.Level > 150)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(443);
                b_Reply(b_Response);
                return false;
            }

            var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, mPlayer.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = 99;
                b_Reply(b_Response);
                return false;
            }
            var mBattleComponent = mMapComponent.GetCustomComponent<BattleComponent>();

            //int mSyncWaitTime = 4 * 60 * 60 * 1000;

            //Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
            //{
            //    var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
            //    mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.PropertyTask;
            //    mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
            //    mBattleSyncTimer.SyncWaitTime = mSyncWaitTime;
            //    mBattleSyncTimer.NextWaitTime = mBattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

            //    mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
            //    {
            //        if (b_CombatSource.IsDisposeable) return;

            //        b_CombatSource.RemoveHealthState(E_BattleSkillStats.XinShouBuff, b_BattleComponent);
            //        b_CombatSource.UpdateHealthState();
            //    };
            //    mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
            //    {
            //        if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

            //        if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.XinShouBuff, out var mCurrentHealthStats) == false)
            //        {
            //            return CombatSource.E_SyncTimerTaskResult.Dispose;
            //        }

            //        if (b_TimerTask.NextWaitTime == mCurrentHealthStats.ContinueTimeMax)
            //        {
            //            return CombatSource.E_SyncTimerTaskResult.Dispose;
            //        }
            //        if (b_TimeTick > mCurrentHealthStats.ContinueTimeMax)
            //        {
            //            return CombatSource.E_SyncTimerTaskResult.Dispose;
            //        }
            //        else
            //        {
            //            b_TimerTask.NextWaitTime = mCurrentHealthStats.ContinueTimeMax;

            //            b_CombatSource.AddTask(b_TimerTask);
            //        }
            //        return CombatSource.E_SyncTimerTaskResult.NextRound;
            //    };
            //    return mBattleSyncTimer;
            //};

            mCurrentTemp.AddHealthState(E_BattleSkillStats.XinShouBuff, 30, 0, 0, null, mBattleComponent);
            mCurrentTemp.UpdateHealthState();

            mCurrentTemp.Data.DBBufflist[(int)E_BattleSkillStats.XinShouBuff] = -1;
            mCurrentTemp.Data.Serialize();

            //保存数据库
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            mWriteDataComponent.Save(mCurrentTemp.Data, dBProxy).Coroutine();

            // 发布 ReceiveXinShouBuff 事件
            ETModel.EventType.ReceiveXinShouBuff.Instance.player = mPlayer;
            Root.EventSystem.OnRun("ReceiveXinShouBuff", ETModel.EventType.ReceiveXinShouBuff.Instance);

            b_Reply(b_Response);
            return true;
        }
    }
}