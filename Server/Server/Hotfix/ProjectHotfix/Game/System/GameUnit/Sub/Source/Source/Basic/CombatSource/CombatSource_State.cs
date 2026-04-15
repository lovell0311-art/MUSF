using System;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using ETModel;
using static ETModel.CombatSource;

namespace ETHotfix
{



    public static partial class CombatSourceSystem
    {
        /// <summary>
        /// 增加单位状态
        /// </summary>
        /// <param name="b_CombatUnitSource">状态增加目标</param>
        /// <param name="b_BattleHealthStats">状态</param>
        /// <param name="b_StrengthValue">状态强度</param>
        /// <param name="b_ContinueTime">持续时间,毫秒</param>
        /// <param name="b_TagId"></param>
        /// <param name="b_SyncAction"></param>
        /// <param name="b_BattleComponent"></param>
        /// <param name="b_ResetType">是否重置此状态 0:重置 1:叠加时间重置数值 2叠加数值重置时间 3叠加数值时间</param>
        /// <param name="b_StrengthValueMax">状态强度 允许叠加最大值</param>
        public static void AddHealthState(this CombatSource b_CombatUnitSource,
          E_BattleSkillStats b_BattleHealthStats,
          int b_StrengthValue,
          long b_ContinueTime,
          int b_TagId,
          Func<CombatSource.BattleSyncTimerTask> b_SyncAction,
          BattleComponent b_BattleComponent,
          E_SkillStatsResetType b_ResetType = E_SkillStatsResetType.RESET_ALL,
          int b_StrengthValueMax = 0,
          bool b_SendNotice = true)
        {
            if (b_CombatUnitSource.HealthStatsDic.TryGetValue(b_BattleHealthStats, out var mCacheList) == false)
            {
                mCacheList = b_CombatUnitSource.HealthStatsDic[b_BattleHealthStats] = Root.CreateBuilder.GetInstance<C_CombatUnitStatsCacheSource>();
            }
            if (mCacheList.CacheDatas.TryGetValue(b_TagId, out var mUnitStats) == false)
            {
                {
                    mUnitStats = Root.CreateBuilder.GetInstance<C_CombatUnitStatsSource>();
                    mUnitStats.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                    mUnitStats.TagId = b_TagId;
                    mUnitStats.ResetType = b_ResetType;

                    mCacheList.CacheDatas[b_TagId] = mUnitStats;
                }

                if (b_SyncAction != null)
                {
                    var mBattleSyncTimer = b_SyncAction.Invoke();
                    b_CombatUnitSource.AddTask(mBattleSyncTimer);

                    mUnitStats.SyncTimerTask = mBattleSyncTimer;
                }
            }

            mUnitStats.BattleSkillStats = b_BattleHealthStats;

            // 是否重置此状态 0:重置 1:叠加数值重置时间 2:叠加时间重置数值 3:叠加数值时间
            switch (b_ResetType)
            {
                case E_SkillStatsResetType.RESET_ALL:
                    {
                        mUnitStats.StrengthValue = b_StrengthValue;
                        mUnitStats.StrengthValueMax = b_StrengthValueMax;
                        mUnitStats.ContinueTime = b_BattleComponent.CurrentTimeTick + b_ContinueTime;
                    }
                    break;
                case E_SkillStatsResetType.RESET_TIME:
                    {
                        mUnitStats.StrengthValue += b_StrengthValue;
                        if (mUnitStats.StrengthValueMax != 0 && mUnitStats.StrengthValue > mUnitStats.StrengthValueMax)
                        {
                            mUnitStats.StrengthValue = mUnitStats.StrengthValueMax;
                        }
                        mUnitStats.ContinueTime = b_BattleComponent.CurrentTimeTick + b_ContinueTime;
                    }
                    break;
                case E_SkillStatsResetType.RESET_NUMBER:
                    {
                        mUnitStats.StrengthValue = b_StrengthValue;
                        mUnitStats.StrengthValueMax = b_StrengthValueMax;

                        if (mUnitStats.ContinueTime < b_BattleComponent.CurrentTimeTick) 
                            mUnitStats.ContinueTime = b_BattleComponent.CurrentTimeTick;
                        mUnitStats.ContinueTime += b_ContinueTime;
                    }
                    break;
                case E_SkillStatsResetType.RESET_NO:
                    {
                        mUnitStats.StrengthValue += b_StrengthValue;
                        if (mUnitStats.StrengthValueMax != 0 && mUnitStats.StrengthValue > mUnitStats.StrengthValueMax)
                        {
                            mUnitStats.StrengthValue = mUnitStats.StrengthValueMax;
                        }
                        if (mUnitStats.ContinueTime < b_BattleComponent.CurrentTimeTick)
                            mUnitStats.ContinueTime = b_BattleComponent.CurrentTimeTick;
                        mUnitStats.ContinueTime += b_ContinueTime;
                    }
                    break;
                default:
                    Log.Error($"ResetType:{b_ResetType} 未实现");
                    break;
            }
            if (mCacheList.ContinueTimeMax < mUnitStats.ContinueTime)
            {
                mCacheList.ContinueTimeMax = mUnitStats.ContinueTime;
            }

            if (b_SendNotice)
            {
                G2C_AttackBuffer_notice mAttackBufferNotice = new G2C_AttackBuffer_notice();
                mAttackBufferNotice.AttackTarget = b_CombatUnitSource.InstanceId;
                mAttackBufferNotice.BufferId = (long)b_BattleHealthStats | (long)b_TagId << 16;
                mAttackBufferNotice.Ticks = mCacheList.ContinueTimeMax - b_BattleComponent.CurrentTimeTick;

                b_BattleComponent.Parent.SendNotice(b_CombatUnitSource, mAttackBufferNotice);
            }
        }

        /// <summary>
        /// 移除单位状态
        /// </summary>
        /// <param name="b_CombatUnitSource"></param>
        /// <param name="b_BattleHealthStats"></param>
        public static void RemoveHealthState(this CombatSource b_CombatUnitSource, E_BattleSkillStats b_BattleHealthStats, BattleComponent b_BattleComponent, bool b_RemoveTimeTask = false)
        {
            if (b_CombatUnitSource.HealthStatsDic.TryGetValue(b_BattleHealthStats, out var mCacheHealthStats))
            {
                G2C_AttackBufferEnd_notice message = new G2C_AttackBufferEnd_notice();
                message.AttackTarget = b_CombatUnitSource.InstanceId;
                message.BufferId = (int)b_BattleHealthStats;
                b_BattleComponent.Parent.SendNotice(b_CombatUnitSource, message);

                b_CombatUnitSource.HealthStatsDic.Remove(b_BattleHealthStats);

                if (b_RemoveTimeTask)
                {
                    var mHealthStatslist = mCacheHealthStats.CacheDatas.Values.ToArray();
                    for (int i = 0, len = mHealthStatslist.Length; i < len; i++)
                    {
                        var mHealthStats = mHealthStatslist[i];

                        if (mHealthStats.SyncTimerTask != null)
                        {
                            b_CombatUnitSource.SyncTaskTimerClear(mHealthStats.SyncTimerTask);
                            mHealthStats.SyncTimerTask = null;
                        }
                    }
                }

                mCacheHealthStats.Dispose();
            }
        }
        public static void RemoveAllHealthState(this CombatSource b_CombatUnitSource, BattleComponent b_BattleComponent)
        {
            if (b_CombatUnitSource.HealthStatsDic.Count > 0)
            {
                var mHealthStatsListTemp = b_CombatUnitSource.HealthStatsDic.Values.ToList();
                for (int i = 0, len = mHealthStatsListTemp.Count; i < len; i++)
                {
                    var mSingle = mHealthStatsListTemp[i];

                    mSingle.Dispose();
                }

                b_CombatUnitSource.HealthStatsDic.Clear();
            }
            b_CombatUnitSource.SyncTaskTimerDispose();
        }


        /// <summary>
        /// 更新单位状态
        /// </summary>
        /// <returns></returns>
        public static void UpdateHealthState(this CombatSource b_CombatUnitSource)
        {
            //if (b_CombatUnitSource.HealthStatsDic.ContainsKey(E_BattleSkillStats.SUPPRESS))
            //{
            //    b_CombatUnitSource.MainHealthStats = E_BattleHealthStats.ABNORMAL;
            //    return;
            //}
            //if (b_CombatUnitSource.HealthStatsDic.ContainsKey(E_BattleSkillStats.VERTIGO))
            //{
            //    b_CombatUnitSource.MainHealthStats = E_BattleHealthStats.ABNORMAL;
            //    return;
            //}
            //if (b_CombatUnitSource.HealthStatsDic.ContainsKey(E_BattleSkillStats.IMPRISON))
            //{
            //    b_CombatUnitSource.MainHealthStats = E_BattleHealthStats.ABNORMAL;
            //    return;
            //}
            //if (b_CombatUnitSource.HealthStatsDic.ContainsKey(E_BattleSkillStats.SILENT))
            //{
            //    b_CombatUnitSource.MainHealthStats = E_BattleHealthStats.ABNORMAL;
            //    return;
            //}


            b_CombatUnitSource.MainHealthStats = E_BattleHealthStats.NORMAL;
        }
    }
}