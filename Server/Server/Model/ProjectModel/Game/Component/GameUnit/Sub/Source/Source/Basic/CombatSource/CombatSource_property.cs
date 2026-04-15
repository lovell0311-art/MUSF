using System;
using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    public partial class CombatSource
    {
        /// <summary>
        /// 获取属性方式
        /// </summary>
        public Func<E_GameProperty, int> GetNumerialFunc { get; set; }

        /// <summary>
        /// 身份
        /// </summary>
        public E_Identity Identity { get; set; } = E_Identity.Enemy;
        /// <summary>
        /// 职业
        /// </summary>
        public E_GameOccupation GameHeroType { get; set; } = E_GameOccupation.Spell;
        /// <summary>
        /// 性别
        /// </summary>
        public E_GameHeroSexType GameHeroSexType { get; set; } = E_GameHeroSexType.BOY;







        /// <summary>
        /// 技能组
        /// </summary>
        public Dictionary<int, C_HeroSkillSource> SkillGroup { get; set; }
        public Dictionary<int, C_BattleMaster> MasterGroup { get; set; }


        private void ClearData()
        {
            GetNumerialFunc = null;
            Identity = E_Identity.Enemy;
            GameHeroType = E_GameOccupation.Spell;
            GameHeroSexType = E_GameHeroSexType.BOY;

            if (SkillGroup != null)
            {
                var mSkillGroupTemp = SkillGroup.Values.ToList();
                for (int i = 0, len = mSkillGroupTemp.Count; i < len; i++)
                {
                    mSkillGroupTemp[i].Dispose();
                }
                SkillGroup.Clear();
            }
            if (MasterGroup != null)
            {
                var mMasterGroupTemp = MasterGroup.Values.ToList();
                for (int i = 0, len = mMasterGroupTemp.Count; i < len; i++)
                {
                    mMasterGroupTemp[i].Dispose();
                }
                MasterGroup.Clear();
            }
        }
    }

}