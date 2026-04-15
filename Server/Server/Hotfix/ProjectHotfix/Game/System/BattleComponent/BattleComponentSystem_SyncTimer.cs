using CustomFrameWork;
using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using static ETModel.BattleComponent;

namespace ETHotfix
{

    public static partial class BattleComponentSystem
    {

        /// <summary>
        /// 增加一个延时器
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_WaitTime"></param>
        /// <param name="b_WaitSyncAction"></param>
        public static long WaitSync(this BattleComponent b_Component, int b_WaitTime, long b_AttackerId, long b_BeAttackerId, Action<long, long, long> b_WaitSyncAction, Action b_StartSyncAction = null, int b_ZoneId = 0)
        {
            if (b_StartSyncAction != null) b_StartSyncAction.Invoke();

            if (b_WaitTime == 0)
            {
                b_WaitSyncAction.Invoke(-1, b_AttackerId, b_BeAttackerId);
                return 0;
            }
            var mBattleSyncTimer = Root.CreateBuilder.GetInstance<BattleComponent.BattleSyncTimer>();
            mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateUnitId(b_ZoneId);
            mBattleSyncTimer.AttackerId = b_AttackerId;
            mBattleSyncTimer.BeAttackerId = b_BeAttackerId;
            mBattleSyncTimer.SyncWaitTime = b_WaitTime;
            mBattleSyncTimer.Time = b_Component.CurrentTimeTick + b_WaitTime;
            mBattleSyncTimer.SyncAction = b_WaitSyncAction;

            b_Component.Add(mBattleSyncTimer);

            return mBattleSyncTimer.CombatRoundId;
        }
    }
}