
using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// 武将技能战斗单位逻辑类
    /// </summary>
    public partial class C_HeroSkillSource : C_CombatSkillSource
    {
        [JsonIgnore]
        private C_ConfigInfo config;
        [JsonIgnore]
        public C_ConfigInfo Config
        {
            get => config;
            private set
            {
                config = value;
                if (config == null)
                {
                    BattleComponent.Log("技能配置null", true);
                }
            }
        }
        public void SetConfig(C_ConfigInfo b_Config)
        {
            Config = b_Config;
        }

        public override void Clear()
        {
            base.Clear();

            Config = null;
            AfterClear();
        }
    }
    public partial class C_HeroSkillSource
    {
        /// <summary>
        /// 耗蓝
        /// </summary>
        public int MP { get; protected set; }
        /// <summary>
        /// AG
        /// </summary>
        public int AG { get; protected set; }

        /// <summary>
        /// 冷却时间
        /// </summary>
        public int CoolTime { get; protected set; }

        public long NextAttackTime { get; set; }

        public virtual int GetCoolTime(CombatSource b_Attacker)
        {
            return CoolTime;
        }

        public virtual void AfterClear()
        {
            NextAttackTime = 0;
            CoolTime = 0;
            MP = 0;
            AG = 0;
        }
    }
    public partial class C_HeroSkillSource
    {
        public virtual void AfterAwake()
        {

        }
        public virtual void DataUpdate()
        {

        }
    }
}
