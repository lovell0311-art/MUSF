using System;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Collections.Generic;
namespace ETModel
{



    public partial class CombatSource
    {
        public long InCastCombatRoundId { get; set; }
        public bool InCasting { get; set; }

        public long CombatRoundId { get; set; }

        public bool _IsAttacking;
        public bool IsAttacking
        {
            get
            {
                if (Identity != E_Identity.Hero)
                    return _IsAttacking;

                var mGamePlayer = this as GamePlayer;
                var mClientTime = mGamePlayer.Player.ClientTime.ClientTime;

                return NextAttackTime > mClientTime;
            }
            set
            {
                _IsAttacking = value;
            }
        }
        public long AttackTime { get; set; }
        public long NextAttackTime { get; set; }

        public bool IsDeath { get; set; }

        public bool isReallyDeath;
        public bool IsReallyDeath 
        {
            get { return isReallyDeath; }
            set
            {
                if(value)
                {
                    if(isReallyDeath == false)
                    {
                        // 添加到清理列表
                        CombatSourceRecycleComponent.Instance.Destroy(this);
                    }
                }
                else
                {
                    // 取消清理
                    isReallyDeath = value;
                }
            }
        }
        public CombatSource Enemy { get; set; }
        public CombatSource TargetEnemy { get; set; }

        /// <summary>
        /// 上一次战斗时间
        /// </summary>
        public long LastAttackTime { get; set; }

        public long LastUseSkillTime { get; set; }
        public int doubleHitId { get; set; }
        public int doubleHitId2 { get; set; }

        public long UpdateTick { get; set; }
        public override void Clear()
        {
            UpdateTick = 0;
            isReallyDeath = false;
            IsDeath = false;
            AttackTime = 0;
            NextAttackTime = 0;
            LastAttackTime = 0;
            IsAttacking = false;
            CombatRoundId = 0;
            InCasting = false;

            LastUseSkillTime = default;
            doubleHitId = 0;
            doubleHitId2 = 0;

            ClearCacheData();
            ClearEvent();
            ClearData();
            ClearUnitData();
            SyncTaskTimerDispose();
            base.Clear();
        }
    }

    [PrivateObject]
    public partial class CombatSource : CustomComponent
    {
        protected override int PoolCountMax => 0;

        private long instanceId;
        public void SetInstanceId(long b_InstanceId)
        {
            instanceId = b_InstanceId;
        }
        /// <summary>
        /// 战斗对象的场景对象Id  玩家为GameUserId 怪物为Id 
        /// </summary>
        public long InstanceId { get { return instanceId; } }

        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            instanceId = default;
            base.Dispose();
        }
    }
}