using System;
using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    public partial class CombatSource
    {
        /// <summary>
        /// 帧刷新
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionFrameUpdate { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();
        /// <summary>
        /// 秒刷新
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionSecondUpdate { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();


        /// <summary>
        /// 回复  泉水回复
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionReply { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();
        /// <summary>
        /// 死亡
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionDeath { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();

        /// <summary>
        /// 攻击
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionAttack { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();
        /// <summary>
        /// 受到攻击
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionInjure { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();
        /// <summary>
        /// 受到技能攻击
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionInjureBySkill { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();
        /// <summary>
        /// 受到小兵攻击
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionInjureBySolider { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();
        /// <summary>
        /// 受到攻击 闪避时
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionInjureDodge { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();
        /// <summary>
        /// 使用技能
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionUseSkill { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();

        /// <summary>
        /// 击杀
        /// </summary>
        public Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> ActionKill { get; set; } = new Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>>();


        private void ClearEvent()
        {
            if (ActionFrameUpdate.Count > 0) ActionFrameUpdate.Clear();
            if (ActionSecondUpdate.Count > 0) ActionSecondUpdate.Clear();

            if (ActionReply.Count > 0) ActionReply.Clear();
            if (ActionDeath.Count > 0) ActionDeath.Clear();
            if (ActionAttack.Count > 0) ActionAttack.Clear();
            if (ActionInjure.Count > 0) ActionInjure.Clear();
            if (ActionInjureBySolider.Count > 0) ActionInjureBySolider.Clear();
            if (ActionInjureDodge.Count > 0) ActionInjureDodge.Clear();
            if (ActionUseSkill.Count > 0) ActionUseSkill.Clear();
            if (ActionInjureBySkill.Count > 0) ActionInjureBySkill.Clear();
            if (ActionKill.Count > 0) ActionKill.Clear();


        }

        public void RunAction(Dictionary<int, Func<CombatSource, CombatSource, BattleComponent, bool>> b_keyValuePairs, CombatSource b_CombatSource, CombatSource b_TargetCombatSource, BattleComponent b_BattleComponent)
        {
            if (b_keyValuePairs != null && b_keyValuePairs.Count > 0)
            {
                var mRunActions = b_keyValuePairs.Values.ToList();
                for (int i = 0, len = mRunActions.Count; i < len; i++)
                {
                    mRunActions[i].Invoke(b_CombatSource, b_TargetCombatSource, b_BattleComponent);
                }
            }
        }
    }
}