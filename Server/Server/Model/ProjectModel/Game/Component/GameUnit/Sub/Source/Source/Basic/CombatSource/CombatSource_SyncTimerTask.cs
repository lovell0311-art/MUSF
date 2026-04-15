using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;

namespace ETModel
{
    public partial class CombatSource
    {
        //public class BattleSyncTimerTask_SKillCoolTime : BattleSyncTimerTask
        //{
        //    public C_HeroSkillSource HeroSkillSource { get; set; }

        //    public override void Dispose()
        //    {
        //        if (IsDisposeable) return;

        //        if (HeroSkillSource != null)
        //        {
        //            HeroSkillSource.NextAttackTime = 0;
        //            HeroSkillSource = null;
        //        }
        //        base.Dispose();
        //    }
        //}
        public enum E_SyncTimerTaskResult
        {
            Dispose,

            NextRound,

            AutoNextRound,
        }
        public enum E_SyncTimerTaskType
        {
            Default,

            PropertyTask,

            DeathRemoveTask,
        }
        public class BattleSyncTimerTask : ADataContext
        {
            public bool IsRun { get; set; }
            public long CombatRoundId { get; set; }
            public E_SyncTimerTaskType TaskType { get; set; }
            public long SyncWaitTime { get; set; }
            public long NextWaitTime { get; set; }
            public Func<CombatSource, BattleComponent, BattleSyncTimerTask, long, E_SyncTimerTaskResult> SyncAction { get; set; }
            public Action<CombatSource, BattleComponent, BattleSyncTimerTask> DisposeAction { get; set; }
            public override void ContextAwake()
            {
                IsRun = true;
            }
            public override void Dispose()
            {
                if (IsDisposeable) return;

                TaskType = E_SyncTimerTaskType.Default;
                DisposeAction = null;
                SyncAction = null;
                SyncWaitTime = default;
                NextWaitTime = default;
                CombatRoundId = default;
                IsRun = false;

                base.Dispose();
            }
        }

        /// <summary>
        /// —” ±≥Ã–Ú
        /// </summary>
        private readonly SortedDictionary<long, List<BattleSyncTimerTask>> _WaitSyncTaskActions = new SortedDictionary<long, List<BattleSyncTimerTask>>();
        private long _MinTimeValueTask = long.MaxValue;
        public void SyncTaskTimerInit()
        {
            SyncTaskTimerClear();
        }
        public void SyncTaskTimerDispose()
        {
            SyncTaskTimerClear();
        }

        private void SyncTaskTimerClear()
        {
            if (_WaitSyncTaskActions != null && _WaitSyncTaskActions.Count > 0)
            {
                var mTemps = _WaitSyncTaskActions.Values.ToArray();
                for (int i = 0, len = mTemps.Length; i < len; i++)
                {
                    var mTemps2 = mTemps[i];
                    for (int j = 0, jlen = mTemps2.Count; j < jlen; j++)
                    {
                        mTemps2[j].Dispose();
                    }
                }
                _WaitSyncTaskActions.Clear();
            }
            _MinTimeValueTask = long.MaxValue;
        }
        public bool SyncTaskTimerClear(BattleSyncTimerTask b_SyncTimerTask)
        {
            if (_WaitSyncTaskActions != null && _WaitSyncTaskActions.Count > 0)
            {
                if (_WaitSyncTaskActions.ContainsKey(b_SyncTimerTask.NextWaitTime))
                {
                    b_SyncTimerTask.IsRun = false;

                    return true;
                }
            }
            return false;
        }
        public void SyncTaskTimerClear(E_SyncTimerTaskType b_SyncTimerTaskType)
        {
            if (_WaitSyncTaskActions != null && _WaitSyncTaskActions.Count > 0)
            {
                var mTempKeys = _WaitSyncTaskActions.Keys.ToArray();
                for (int i = 0, len = mTempKeys.Length; i < len; i++)
                {
                    var mTempsKey2 = mTempKeys[i];
                    var mTemps2 = _WaitSyncTaskActions[mTempsKey2];

                    for (int j = mTemps2.Count - 1; j >= 0; j--)
                    {
                        var mTemps3 = mTemps2[j];

                        if (mTemps3.TaskType == b_SyncTimerTaskType)
                            mTemps3.IsRun = false;
                    }
                }
            }
        }

        public void RunSyncTaskAction(long b_timeNow, BattleComponent b_BattleComponent)
        {
            if (IsDisposeable) return;
            if (_WaitSyncTaskActions.Count == 0) return;

            if (b_timeNow < _MinTimeValueTask) return;

            RemoveTimerTask(b_timeNow, b_BattleComponent);
            _MinTimeValueTask = GetTimerTaskMinKey();
        }

        private void RemoveTimerTask(long b_TimeValue, BattleComponent b_BattleComponent)
        {
            //lock (objLock)
            {
                var mWaitKeylist = _WaitSyncTaskActions.Keys.ToList();
                for (int i = 0, len = mWaitKeylist.Count; i < len; i++)
                {
                    long mWaitKey = mWaitKeylist[i];

                    if (b_TimeValue < mWaitKey)
                    {
                        break;
                    }

                    if (_WaitSyncTaskActions.TryGetValue(mWaitKey, out var mWaitValue))
                    {
                        if (mWaitValue.Count > 0)
                        {
                            void TaskDispose(BattleSyncTimerTask b_TimerTask, CombatSource b_CombatSource, BattleComponent b_BattleComponentTemp)
                            {
                                if (b_TimerTask.DisposeAction != null)
                                {
                                    b_TimerTask.DisposeAction.Invoke(b_CombatSource, b_BattleComponentTemp, b_TimerTask);
                                    b_TimerTask.DisposeAction = null;
                                }
                                b_TimerTask.Dispose();
                            }
                            void TaskNextTime(BattleSyncTimerTask b_TimerTask)
                            {
                                b_TimerTask.NextWaitTime = b_TimerTask.NextWaitTime + b_TimerTask.SyncWaitTime;
                                if (b_TimeValue > b_TimerTask.NextWaitTime)
                                {
                                    var mTimeValue = b_TimeValue - b_TimerTask.NextWaitTime;
                                    var mTimeValueMultiple = mTimeValue / b_TimerTask.SyncWaitTime;
                                    var mAddTimeValue = mTimeValueMultiple * b_TimerTask.SyncWaitTime;
                                    b_TimerTask.NextWaitTime = b_TimerTask.NextWaitTime + mAddTimeValue + b_TimerTask.SyncWaitTime;
                                }
                            }

                            for (int j = 0, zlen = mWaitValue.Count; j < zlen; j++)
                            {
                                var mSyncTimerTask = mWaitValue[j];
                                if (mSyncTimerTask == null) continue;
                                if (mSyncTimerTask.IsDisposeable) continue;

                                if (mSyncTimerTask.IsRun == false)
                                {
                                    mSyncTimerTask.Dispose();
                                    continue;
                                }

                                var mResult = mSyncTimerTask.SyncAction(this, b_BattleComponent, mSyncTimerTask, b_TimeValue);

                                switch (mResult)
                                {
                                    case E_SyncTimerTaskResult.Dispose:
                                        TaskDispose(mSyncTimerTask, this, b_BattleComponent);
                                        break;
                                    case E_SyncTimerTaskResult.NextRound:
                                        break;
                                    case E_SyncTimerTaskResult.AutoNextRound:
                                        if (mSyncTimerTask.SyncWaitTime == 0)
                                        {
                                            TaskDispose(mSyncTimerTask, this, b_BattleComponent);
                                            break;
                                        }

                                        TaskNextTime(mSyncTimerTask);

                                        if (mSyncTimerTask.NextWaitTime == mWaitKey)
                                            TaskDispose(mSyncTimerTask, this, b_BattleComponent);
                                        else
                                            AddTask(mSyncTimerTask);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            mWaitValue.Clear();
                        }

                        _WaitSyncTaskActions.Remove(mWaitKey);
                    }
                }
            }
        }
        private long GetTimerTaskMinKey()
        {
            long mResult = long.MaxValue;

            var mWaitSyncKeylist = _WaitSyncTaskActions.Keys.ToList();
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



        public void AddTask(BattleSyncTimerTask b_BattleSyncTimer)
        {
            List<BattleSyncTimerTask> taskList = default;
            //lock (objLock)
            {
                if (_WaitSyncTaskActions.TryGetValue(b_BattleSyncTimer.NextWaitTime, out taskList) == false)
                    _WaitSyncTaskActions[b_BattleSyncTimer.NextWaitTime] = taskList = new List<BattleSyncTimerTask>();
            }
            taskList.Add(b_BattleSyncTimer);

            if (_MinTimeValueTask > b_BattleSyncTimer.NextWaitTime)
            {
                _MinTimeValueTask = b_BattleSyncTimer.NextWaitTime;
            }
        }
    }
}