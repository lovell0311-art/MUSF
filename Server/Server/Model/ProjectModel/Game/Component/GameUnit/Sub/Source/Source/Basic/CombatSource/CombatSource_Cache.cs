using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    public partial class CombatSource
    {
        private void ClearCacheData()
        {
            if (ChangePropertyDic.Count > 0) ChangePropertyDic.Clear();
            if (GamePropertyDic.Count > 0) GamePropertyDic.Clear();
            if (BattleMasteryDic.Count > 0) BattleMasteryDic.Clear();
            if (EquipPropertyDic.Count > 0) EquipPropertyDic.Clear();

            if (HealthStatsDic.Count > 0)
            {
                var mHealthStatsListTemp = HealthStatsDic.Values.ToList();
                for (int i = 0, len = mHealthStatsListTemp.Count; i < len; i++)
                {
                    var mSingle = mHealthStatsListTemp[i];

                    mSingle.Dispose();
                }
                HealthStatsDic.Clear();
            }
            if (GamePropertyNumerialDic.Count > 0)
            {
                var mHealthStatsListTemp = GamePropertyNumerialDic.Values.ToList();
                for (int i = 0, len = mHealthStatsListTemp.Count; i < len; i++)
                {
                    var mSingle = mHealthStatsListTemp[i];

                    mSingle.Dispose();
                }
                GamePropertyNumerialDic.Clear();
            }
        }
        /// <summary>
        /// 变更属性校验
        /// </summary>
        public Dictionary<E_GameProperty, (bool, int)> ChangePropertyDic { get; set; } = new Dictionary<E_GameProperty, (bool, int)>();

        public Dictionary<E_GameProperty, C_PropertyNumerial> GamePropertyNumerialDic { get; set; } = new Dictionary<E_GameProperty, C_PropertyNumerial>();

        /// <summary>
        /// 主状态
        /// 是否受到负面状态
        /// </summary>
        public E_BattleHealthStats MainHealthStats { get; set; } = E_BattleHealthStats.NORMAL;
        /// <summary>
        /// 固定属性
        /// </summary>
        public Dictionary<E_GameProperty, int> GamePropertyDic { get; set; } = new Dictionary<E_GameProperty, int>();
        /// <summary>
        /// 大师状态
        /// </summary>
        public Dictionary<E_BattleMasteryState, int> BattleMasteryDic { get; set; } = new Dictionary<E_BattleMasteryState, int>();
        /// <summary>
        /// 装备加成总属性
        /// </summary>
        public Dictionary<E_GameProperty, int> EquipPropertyDic { get; set; } = new Dictionary<E_GameProperty, int>();
        /// <summary>
        /// 受到负面状态 数据缓存
        /// </summary>
        public Dictionary<E_BattleSkillStats, C_CombatUnitStatsCacheSource> HealthStatsDic { get; set; } = new Dictionary<E_BattleSkillStats, C_CombatUnitStatsCacheSource>();

        public class C_CombatUnitStatsCacheSource : ADataContext
        {
            /// <summary>
            /// 所有技能效果最长的有效时间
            /// </summary>
            public long ContinueTimeMax { get; set; }
            public E_SyncTimerTaskType TaskType { get; set; } = E_SyncTimerTaskType.Default;
            public Dictionary<int, C_CombatUnitStatsSource> CacheDatas { get; set; }

            public override void ContextAwake()
            {
                if (CacheDatas == null)
                {
                    CacheDatas = new Dictionary<int, C_CombatUnitStatsSource>();
                }
                else
                {
                    var mCacheDatas = CacheDatas.Values.ToList();
                    for (int i = 0, len = mCacheDatas.Count; i < len; i++)
                    {
                        mCacheDatas[i].Dispose();
                    }
                    CacheDatas.Clear();
                }

                ContinueTimeMax = 0;
            }
            public override void Dispose()
            {
                if (IsDisposeable) return;

                TaskType = E_SyncTimerTaskType.Default;
                ContinueTimeMax = default;
                if (CacheDatas != null)
                {
                    if (CacheDatas.Count > 0)
                    {
                        var mCacheDatas = CacheDatas.Values.ToList();
                        for (int i = 0, len = mCacheDatas.Count; i < len; i++)
                        {
                            mCacheDatas[i].Dispose();
                        }
                        CacheDatas.Clear();
                    }

                    CacheDatas = null;
                }
                base.Dispose();
            }
        }
        /// <summary>
        /// 重置技能附加状态类型
        /// </summary>
        public enum E_SkillStatsResetType
        {
            /// <summary>
            /// 0:重置
            /// </summary>
            RESET_ALL,
            /// <summary>
            /// 1:叠加数值重置时间
            /// </summary>
            RESET_TIME,
            /// <summary>
            /// 2:叠加时间重置数值
            /// </summary>
            RESET_NUMBER,
            /// <summary>
            /// 3:叠加数值时间
            /// </summary>
            RESET_NO
        }
        /// <summary>
        /// 战斗单位状态
        /// </summary>
        public class C_CombatUnitStatsSource : ADataContext
        {
            public long CombatRoundId { get; set; }

            /// <summary>
            /// 战斗状态  异常信息
            /// </summary>
            public E_BattleSkillStats BattleSkillStats { get; set; }

            public BattleSyncTimerTask SyncTimerTask { get; set; }

            public Dictionary<int, int> CacheData { get; set; }

            /// <summary>
            /// 战斗状态强度 例如:减速40% 武力上升30%
            /// </summary>
            public int StrengthValue { get; set; }
            /// <summary>
            /// 最大战斗状态强度
            /// 叠加最大值
            /// </summary>
            public int StrengthValueMax { get; set; }
            /// <summary>
            /// 战斗状态持续时间
            /// </summary>
            public long ContinueTime { get; set; }
            /// <summary>
            /// 标签
            /// </summary>
            public int TagId { get; set; }
            /// <summary>
            /// 重置技能附加状态类型
            /// </summary>
            public E_SkillStatsResetType ResetType { get; set; }

            public override void Dispose()
            {
                if (IsDisposeable) return;

                TagId = default;
                CombatRoundId = default;
                StrengthValue = default;
                ContinueTime = default;
                CombatRoundId = default;
                ResetType = default;
                StrengthValueMax = default;
                SyncTimerTask = null;
                if (CacheData != null) CacheData = null;

                base.Dispose();
            }
        }
    }
}