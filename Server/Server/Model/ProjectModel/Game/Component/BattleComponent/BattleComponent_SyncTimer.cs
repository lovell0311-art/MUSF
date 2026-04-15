using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;

namespace ETModel
{
    public partial class BattleComponent
    {
        public class BattleSyncTimer : ADataContext
        {
            public long CombatRoundId { get; set; }
            public long AttackerId { get; set; }
            public long BeAttackerId { get; set; }
            public int SyncWaitTime { get; set; }
            public long Time { get; set; }
            public Action<long, long, long> SyncAction { get; set; }

            public override void Dispose()
            {
                if (IsDisposeable) return;

                SyncAction = null;
                SyncWaitTime = default;
                Time = default;
                CombatRoundId = default;
                AttackerId = default;
                BeAttackerId = default;

                base.Dispose();
            }
        }

        /// <summary>
        /// —” ±≥Ã–Ú
        /// </summary>
        public readonly SortedDictionary<long, List<BattleSyncTimer>> _WaitSyncActions = new SortedDictionary<long, List<BattleSyncTimer>>();
        private long _MinTimeValue = long.MaxValue;
        public void SyncTimerInit()
        {
            SyncTimerClear();
        }
        public void SyncTimerDispose()
        {
            SyncTimerClear();
        }


        private void SyncTimerClear()
        {
            if (_WaitSyncActions != null && _WaitSyncActions.Count > 0)
            {
                var mTemps = _WaitSyncActions.Values.ToArray();
                for (int i = 0, len = mTemps.Length; i < len; i++)
                {
                    var mTemps2 = mTemps[i];
                    for (int j = 0, jlen = mTemps2.Count; j < jlen; j++)
                    {
                        mTemps2[j].Dispose();
                    }
                }
                _WaitSyncActions.Clear();
            }
        }

        public void RunSyncAction()
        {
            if (_WaitSyncActions.Count == 0) return;

            if (this.CurrentTimeTick < _MinTimeValue) return;

            RemoveTimer(this.CurrentTimeTick);
            _MinTimeValue = GetTimerMinKey();
        }

        private void RemoveTimer(long b_TimeValue)
        {
            //lock (objLock)
            {
                var mWaitKeylist = _WaitSyncActions.Keys.ToList();
                for (int i = 0, len = mWaitKeylist.Count; i < len; i++)
                {
                    long mWaitKey = mWaitKeylist[i];

                    if (b_TimeValue < mWaitKey)
                    {
                        break;
                    }

                    if (_WaitSyncActions.TryGetValue(mWaitKey, out var mWaitValue))
                    {
                        if (mWaitValue.Count > 0)
                        {
                            for (int j = 0, zlen = mWaitValue.Count; j < zlen; j++)
                            {
                                var mSyncTimerTask = mWaitValue[j];
                                if (mSyncTimerTask == null) continue;
                                if (mSyncTimerTask.IsDisposeable) continue;

                                try
                                {
                                    mSyncTimerTask.SyncAction(mSyncTimerTask.CombatRoundId, mSyncTimerTask.AttackerId, mSyncTimerTask.BeAttackerId);
                                }
                                catch (Exception e)
                                {
                                    CustomFrameWork.Log.Error(e);
                                }

                                mSyncTimerTask.Dispose();
                            }
                            mWaitValue.Clear();
                        }

                        _WaitSyncActions.Remove(mWaitKey);
                    }
                }
            }
        }
        private long GetTimerMinKey()
        {
            long mResult = long.MaxValue;

            var mWaitSyncKeylist = _WaitSyncActions.Keys.ToList();
            for (int i = 0, len = mWaitSyncKeylist.Count; i < len; i++)
            {
                var mWaitSyncKey = mWaitSyncKeylist[i];
                if (mResult > mWaitSyncKey)
                {
                    mResult = mWaitSyncKey;
                }
            }
            return mResult;
        }



        public void Add(BattleSyncTimer b_BattleSyncTimer)
        {
            List<BattleSyncTimer> taskList = default;
            //lock (objLock)
            {
                if (_WaitSyncActions.TryGetValue(b_BattleSyncTimer.Time, out taskList) == false)
                    _WaitSyncActions[b_BattleSyncTimer.Time] = taskList = new List<BattleSyncTimer>();
            }
            taskList.Add(b_BattleSyncTimer);

            if (_MinTimeValue > b_BattleSyncTimer.Time)
            {
                _MinTimeValue = b_BattleSyncTimer.Time;
            }
        }
    }
}