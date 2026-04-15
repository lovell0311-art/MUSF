using System.Collections.Generic;

namespace ETHotfix
{
    public static class HolyteacherSkillCompat
    {
        private static readonly Dictionary<int, SkillInfos> CompatSkills = new Dictionary<int, SkillInfos>
        {
            [419] = Create(
                419,
                "\u9ed1\u8272\u5929\u7a7a",
                "\u5b66\u4e60\u8d85\u795e\u5927\u5e08\u5929\u8d4b\u540e\uff0c\u5bf9\u76ee\u6807\u53ca\u5468\u56f4\u534a\u5f848\u8303\u56f4\u5185\u654c\u4eba\u9020\u6210\u4f24\u5bb3\u3002",
                "heisetiankong",
                string.Empty,
                "Skill_0005",
                "Effect_DaShiHeiSeTianKong",
                1,
                500,
                8,
                "{\"1\":130}",
                "{\"1\":200,\"2\":8,\"10\":100,\"11\":260}",
                "{\"1\":400}",
                coolTime: 1500)
        };

        public static bool TryGet(int skillId, out SkillInfos skillInfo)
        {
            if (CompatSkills.TryGetValue(skillId, out SkillInfos source))
            {
                skillInfo = Clone(source);
                return true;
            }

            skillInfo = null;
            return false;
        }

        public static List<SkillInfos> GetAllSorted()
        {
            List<SkillInfos> result = new List<SkillInfos>(CompatSkills.Count);
            List<int> ids = new List<int>(CompatSkills.Keys);
            ids.Sort();

            for (int i = 0; i < ids.Count; ++i)
            {
                result.Add(Clone(CompatSkills[ids[i]]));
            }

            return result;
        }

        public static void AppendMissing(List<SkillInfos> skillInfos)
        {
            List<int> ids = new List<int>(CompatSkills.Keys);
            ids.Sort();

            bool changed = false;
            for (int i = 0; i < ids.Count; ++i)
            {
                int skillId = ids[i];
                SkillInfos compatSkill = Clone(CompatSkills[skillId]);
                int existingIndex = -1;
                for (int j = 0; j < skillInfos.Count; ++j)
                {
                    if (skillInfos[j].Id == skillId)
                    {
                        existingIndex = j;
                        break;
                    }
                }

                if (existingIndex >= 0)
                {
                    skillInfos[existingIndex] = compatSkill;
                }
                else
                {
                    skillInfos.Add(compatSkill);
                }

                changed = true;
            }

            if (changed)
            {
                skillInfos.Sort((left, right) => left.Id.CompareTo(right.Id));
            }
        }

        public static void OverrideIfNeeded(int skillId, SkillInfos skillInfo)
        {
            if (skillInfo == null)
            {
                return;
            }

            if (!CompatSkills.TryGetValue(skillId, out SkillInfos compatSkill))
            {
                return;
            }

            CopyTo(skillInfo, compatSkill);
        }

        private static SkillInfos Create(
            int id,
            string name,
            string describe,
            string icon,
            string soundName,
            string animatorTriggerIndex,
            string attackEffect,
            int skillType,
            int damageWait,
            int distance,
            string consume,
            string otherData,
            string useStandard,
            int coolTime = 0,
            int persistentTime = 0)
        {
            return new SkillInfos
            {
                Id = id,
                Name = name,
                Describe = describe,
                Icon = icon,
                SoundName = soundName,
                AnimatorTriggerIndex = animatorTriggerIndex,
                AttackEffect = attackEffect,
                HitEffect = string.Empty,
                skillType = skillType,
                DamageWait = damageWait,
                Distance = distance,
                CoolTime = coolTime,
                PersistentTime = persistentTime,
                Consume = consume,
                OtherData = otherData,
                UseStandard = useStandard,
                Lev_CanUser = useStandard.GetValue(1),
                Strength = useStandard.GetValue(2),
                Intell = useStandard.GetValue(3),
                Agile = useStandard.GetValue(4),
                Command = useStandard.GetValue(5),
                PhyStength = useStandard.GetValue(6)
            };
        }

        private static SkillInfos Clone(SkillInfos source)
        {
            return new SkillInfos
            {
                Id = source.Id,
                Name = source.Name,
                Describe = source.Describe,
                Icon = source.Icon,
                SoundName = source.SoundName,
                AnimatorTriggerIndex = source.AnimatorTriggerIndex,
                AttackEffect = source.AttackEffect,
                HitEffect = source.HitEffect,
                skillType = source.skillType,
                LearnStandard = source.LearnStandard,
                DamageWait = source.DamageWait,
                Distance = source.Distance,
                CoolTime = source.CoolTime,
                PersistentTime = source.PersistentTime,
                Consume = source.Consume,
                NeedBlue = source.NeedBlue,
                OtherData = source.OtherData,
                BaseDamage = source.BaseDamage,
                UseStandard = source.UseStandard,
                Lev_CanUser = source.Lev_CanUser,
                Strength = source.Strength,
                Intell = source.Intell,
                Agile = source.Agile,
                Command = source.Command,
                PhyStength = source.PhyStength
            };
        }

        private static void CopyTo(SkillInfos target, SkillInfos source)
        {
            target.Id = source.Id;
            target.Name = source.Name;
            target.Describe = source.Describe;
            target.Icon = source.Icon;
            target.SoundName = source.SoundName;
            target.AnimatorTriggerIndex = source.AnimatorTriggerIndex;
            target.AttackEffect = source.AttackEffect;
            target.HitEffect = source.HitEffect;
            target.skillType = source.skillType;
            target.LearnStandard = source.LearnStandard;
            target.DamageWait = source.DamageWait;
            target.Distance = source.Distance;
            target.CoolTime = source.CoolTime;
            target.PersistentTime = source.PersistentTime;
            target.Consume = source.Consume;
            target.NeedBlue = source.NeedBlue;
            target.OtherData = source.OtherData;
            target.BaseDamage = source.BaseDamage;
            target.UseStandard = source.UseStandard;
            target.Lev_CanUser = source.Lev_CanUser;
            target.Strength = source.Strength;
            target.Intell = source.Intell;
            target.Agile = source.Agile;
            target.Command = source.Command;
            target.PhyStength = source.PhyStength;
        }
    }
}
