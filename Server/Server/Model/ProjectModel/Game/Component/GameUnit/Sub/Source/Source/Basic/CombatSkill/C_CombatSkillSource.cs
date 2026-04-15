
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 技能战斗单位逻辑类
    /// </summary>
    public partial class C_CombatSkillSource : ADataContext<int>
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// 数据是不是有错误
        /// </summary>
        public bool IsDataHasError { get; set; }

        public override void ContextAwake(int b_Id)
        {
            Id = b_Id;
            IsDataHasError = true;
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            IsDataHasError = true;
            Id = default;
            Clear();
            base.Dispose();
        }
    }
    public partial class C_CombatSkillSource
    {
        ///// <summary>
        ///// 技能信息
        ///// </summary>
        //[JsonIgnore]
        //public C_CombatSkillInfoSource SkillInfo { get; set; }




        public virtual void Clear()
        {
            //if (SkillInfo != null)
            //{
            //    SkillInfo.Dispose();
            //    SkillInfo = null;
            //}
        }
    }

    public partial class C_CombatSkillSource
    {
        public virtual CombatSource FindTarget(CombatSource b_Attacker, long b_BeAttackerId,
            C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            return null;
        }
        public virtual CombatSource FindTarget(CombatSource b_Attacker, CombatSource b_BeAttacker,
            C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            return b_BeAttacker;
        }
        /// <summary>
        /// 是否满足使用条件
        /// </summary>
        /// <param name="b_Attacker"></param>
        /// <param name="b_BeAttacker"></param>
        /// <param name="b_Cell"></param>
        /// <param name="b_BattleComponent"></param>
        /// <param name="b_Response"></param>
        /// <returns></returns>
        public virtual bool TryUse(CombatSource b_Attacker, CombatSource b_BeAttacker,
            C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            return false;
        }
        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="b_Attacker"></param>
        /// <param name="b_BeAttacker"></param>
        /// <param name="b_BattleComponent"></param>
        /// <returns></returns>
        public virtual bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            return false;
        }
        public virtual bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent)
        {
            return UseSkill(b_Attacker, b_BeAttacker, b_BattleComponent);
        }
    }
}
