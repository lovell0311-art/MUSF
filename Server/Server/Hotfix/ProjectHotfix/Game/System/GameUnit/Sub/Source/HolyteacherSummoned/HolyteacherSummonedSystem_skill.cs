using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{



    public static partial class HolyteacherSummonedSystem
    {
        public static void AwakeSkill(this HolyteacherSummoned b_Component)
        {
            if (b_Component.SkillGroup == null) b_Component.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
        }

        public static void DataUpdateSkill(this HolyteacherSummoned b_Component)
        {
            var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();

            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_monsterConfigJson>().JsonDic;
            var mAttackTypeKeys = b_Component.Config.AttackTypeDic.Keys.ToArray();
            for (int i = 0, len = mAttackTypeKeys.Length; i < len; i++)
            {
                var mSkillId = mAttackTypeKeys[i];

                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                {
                    if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                    {
                        var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);

                        b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                    }
                }
            }
        }
    }
}