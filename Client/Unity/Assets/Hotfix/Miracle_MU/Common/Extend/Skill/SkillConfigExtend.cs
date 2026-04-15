using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 配置表扩展方法
    /// </summary>
    public static  class SkillConfigExtend
    {
      
        /// <summary>
        /// 根据玩家类型 获取对应的技能配置信息
        /// </summary>
        /// <param name="self">技能的配置表ID</param>
        /// <param name="roleType">玩家的类型</param>
        /// <param name="skillInfos">技能属性类</param>
        public static void GetSkillInfos_RoleType_Ref(this int self, E_RoleType roleType, ref SkillInfos skillInfos)
        {
           
            switch (roleType)
            {   
                case E_RoleType.Magician:
                    Skill_SpellConfig skill_Spell = ConfigComponent.Instance.GetItem<Skill_SpellConfig>((int)self);
                    if (skill_Spell == null) return;
                    skillInfos.Id = skill_Spell.Id;
                    skillInfos.Name = skill_Spell.Name;
                    skillInfos.Describe = skill_Spell.Describe;
                    skillInfos.Icon = skill_Spell.Icon;
                    skillInfos.SoundName = skill_Spell.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Spell.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Spell.AttackEffect;
                    skillInfos.HitEffect = skill_Spell.HitEffect;
                    skillInfos.skillType = skill_Spell.skillType;
                    skillInfos.DamageWait = skill_Spell.DamageWait;
                    skillInfos.Distance = skill_Spell.Distance;
                    skillInfos.CoolTime = skill_Spell.CoolTime;
                    skillInfos.PersistentTime = skill_Spell.PersistentTime;
                    skillInfos.Consume = skill_Spell.Consume;
                    skillInfos.OtherData = skill_Spell.OtherData;
                    skillInfos.UseStandard = skill_Spell.UseStandard;

                    skillInfos.Lev_CanUser = skill_Spell.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Spell.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Spell.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Spell.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Spell.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Spell.UseStandard.GetValue(6);
                    break;

                case E_RoleType.Swordsman:
                    Skill_SwordsmanConfig skill_Sword = ConfigComponent.Instance.GetItem<Skill_SwordsmanConfig>((int)self);
                    if (skill_Sword == null) return;
                    skillInfos.Id = skill_Sword.Id;
                    skillInfos.Name = skill_Sword.Name;
                    skillInfos.Describe = skill_Sword.Describe;
                    skillInfos.Icon = skill_Sword.Icon;
                    skillInfos.SoundName = skill_Sword.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Sword.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Sword.AttackEffect;
                    skillInfos.HitEffect = skill_Sword.HitEffect;
                    skillInfos.skillType = skill_Sword.skillType;
                    skillInfos.DamageWait = skill_Sword.DamageWait;
                    skillInfos.Distance = skill_Sword.Distance;
                    skillInfos.CoolTime = skill_Sword.CoolTime;
                    skillInfos.PersistentTime = skill_Sword.PersistentTime;
                    skillInfos.Consume = skill_Sword.Consume;
                    skillInfos.OtherData = skill_Sword.OtherData;
                    skillInfos.UseStandard = skill_Sword.UseStandard;

                    skillInfos.Lev_CanUser = skill_Sword.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Sword.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Sword.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Sword.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Sword.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Sword.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Archer:
                    var skill_Archer = ConfigComponent.Instance.GetItem<Skill_ArcherConfig>((int)self);
                    if (skill_Archer == null) return;
                    skillInfos.Id = skill_Archer.Id;
                    skillInfos.Name = skill_Archer.Name;
                    skillInfos.Describe = skill_Archer.Describe;
                    skillInfos.Icon = skill_Archer.Icon;
                    skillInfos.SoundName = skill_Archer.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Archer.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Archer.AttackEffect;
                    skillInfos.HitEffect = skill_Archer.HitEffect;
                    skillInfos.skillType = skill_Archer.skillType;
                    skillInfos.DamageWait = skill_Archer.DamageWait;
                    skillInfos.Distance = skill_Archer.Distance;
                    skillInfos.CoolTime = skill_Archer.CoolTime;
                    skillInfos.PersistentTime = skill_Archer.PersistentTime;
                    skillInfos.Consume = skill_Archer.Consume;
                    skillInfos.OtherData = skill_Archer.OtherData;
                    skillInfos.UseStandard = skill_Archer.UseStandard;

                    skillInfos.Lev_CanUser = skill_Archer.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Archer.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Archer.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Archer.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Archer.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Archer.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Magicswordsman:
                    Skill_SpellswordConfig skill_Spellsword = ConfigComponent.Instance.GetItem<Skill_SpellswordConfig>((int)self);
                    if (skill_Spellsword == null) return;
                    skillInfos.Id = skill_Spellsword.Id;
                    skillInfos.Name = skill_Spellsword.Name;
                    skillInfos.Describe = skill_Spellsword.Describe;
                    skillInfos.Icon = skill_Spellsword.Icon;
                    skillInfos.SoundName = skill_Spellsword.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Spellsword.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Spellsword.AttackEffect;
                    skillInfos.HitEffect = skill_Spellsword.HitEffect;
                    skillInfos.skillType = skill_Spellsword.skillType;
                    skillInfos.DamageWait = skill_Spellsword.DamageWait;
                    skillInfos.Distance = skill_Spellsword.Distance;
                    skillInfos.CoolTime = skill_Spellsword.CoolTime;
                    skillInfos.PersistentTime = skill_Spellsword.PersistentTime;
                    skillInfos.Consume = skill_Spellsword.Consume;
                    skillInfos.OtherData = skill_Spellsword.OtherData;
                    skillInfos.UseStandard = skill_Spellsword.UseStandard;

                    skillInfos.Lev_CanUser = skill_Spellsword.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Spellsword.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Spellsword.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Spellsword.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Spellsword.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Spellsword.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Holymentor:
                    Skill_HolyteacherConfig skill_Holyteacher = ConfigComponent.Instance.GetItem<Skill_HolyteacherConfig>((int)self);
                    if (skill_Holyteacher == null)
                    {
                        if (HolyteacherSkillCompat.TryGet((int)self, out SkillInfos compatSkill))
                        {
                            skillInfos = compatSkill;
                        }
                        return;
                    }
                    skillInfos.Id = skill_Holyteacher.Id;
                    skillInfos.Name = skill_Holyteacher.Name;
                    skillInfos.Describe = skill_Holyteacher.Describe;
                    skillInfos.Icon = skill_Holyteacher.Icon;
                    skillInfos.SoundName = skill_Holyteacher.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Holyteacher.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Holyteacher.AttackEffect;
                    skillInfos.HitEffect = skill_Holyteacher.HitEffect;
                    skillInfos.skillType = skill_Holyteacher.skillType;
                    skillInfos.DamageWait = skill_Holyteacher.DamageWait;
                    skillInfos.Distance = skill_Holyteacher.Distance;
                    skillInfos.CoolTime = skill_Holyteacher.CoolTime;
                    skillInfos.PersistentTime = skill_Holyteacher.PersistentTime;
                    skillInfos.Consume = skill_Holyteacher.Consume;
                    skillInfos.OtherData = skill_Holyteacher.OtherData;
                    skillInfos.UseStandard = skill_Holyteacher.UseStandard;

                    skillInfos.Lev_CanUser = skill_Holyteacher.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Holyteacher.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Holyteacher.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Holyteacher.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Holyteacher.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Holyteacher.UseStandard.GetValue(6);
                    HolyteacherSkillCompat.OverrideIfNeeded((int)self, skillInfos);
                    break;
                case E_RoleType.Summoner:
                    Skill_SummonWarlockConfig skill_SummonWarlock = ConfigComponent.Instance.GetItem<Skill_SummonWarlockConfig>((int)self);
                    if (skill_SummonWarlock == null) return;
                    skillInfos.Id = skill_SummonWarlock.Id;
                    skillInfos.Name = skill_SummonWarlock.Name;
                    skillInfos.Describe = skill_SummonWarlock.Describe;
                    skillInfos.Icon = skill_SummonWarlock.Icon;
                    skillInfos.SoundName = skill_SummonWarlock.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_SummonWarlock.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_SummonWarlock.AttackEffect;
                    skillInfos.HitEffect = skill_SummonWarlock.HitEffect;
                    skillInfos.skillType = skill_SummonWarlock.skillType;
                    skillInfos.DamageWait = skill_SummonWarlock.DamageWait;
                    skillInfos.Distance = skill_SummonWarlock.Distance;
                    skillInfos.CoolTime = skill_SummonWarlock.CoolTime;
                    skillInfos.PersistentTime = skill_SummonWarlock.PersistentTime;
                    skillInfos.Consume = skill_SummonWarlock.Consume;
                    skillInfos.OtherData = skill_SummonWarlock.OtherData;
                    skillInfos.UseStandard = skill_SummonWarlock.UseStandard;

                    skillInfos.Lev_CanUser = skill_SummonWarlock.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_SummonWarlock.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_SummonWarlock.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_SummonWarlock.UseStandard.GetValue(4);
                    skillInfos.Command = skill_SummonWarlock.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_SummonWarlock.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Gladiator:
                    Skill_CombatConfig skill_Combat = ConfigComponent.Instance.GetItem<Skill_CombatConfig>((int)self);
                    if (skill_Combat == null) return;
                    skillInfos.Id = skill_Combat.Id;
                    skillInfos.Name = skill_Combat.Name;
                    skillInfos.Describe = skill_Combat.Describe;
                    skillInfos.Icon = skill_Combat.Icon;
                    skillInfos.SoundName = skill_Combat.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Combat.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Combat.AttackEffect;
                    skillInfos.HitEffect = skill_Combat.HitEffect;
                    skillInfos.skillType = skill_Combat.skillType;
                    skillInfos.DamageWait = skill_Combat.DamageWait;
                    skillInfos.Distance = skill_Combat.Distance;
                    skillInfos.CoolTime = skill_Combat.CoolTime;
                    skillInfos.PersistentTime = skill_Combat.PersistentTime;
                    skillInfos.Consume = skill_Combat.Consume;
                    skillInfos.OtherData = skill_Combat.OtherData;
                    skillInfos.UseStandard = skill_Combat.UseStandard;

                    skillInfos.Lev_CanUser = skill_Combat.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Combat.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Combat.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Combat.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Combat.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Combat.UseStandard.GetValue(6);
                    break;
                case E_RoleType.GrowLancer:
                    Skill_DreamKnightConfig skill_DreamKnight = ConfigComponent.Instance.GetItem<Skill_DreamKnightConfig>((int)self);
                    if (skill_DreamKnight == null) return;
                    skillInfos.Id = skill_DreamKnight.Id;
                    skillInfos.Name = skill_DreamKnight.Name;
                    skillInfos.Describe = skill_DreamKnight.Describe;
                    skillInfos.Icon = skill_DreamKnight.Icon;
                    skillInfos.SoundName = skill_DreamKnight.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_DreamKnight.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_DreamKnight.AttackEffect;
                    skillInfos.HitEffect = skill_DreamKnight.HitEffect;
                    skillInfos.skillType = skill_DreamKnight.skillType;
                    skillInfos.DamageWait = skill_DreamKnight.DamageWait;
                    skillInfos.Distance = skill_DreamKnight.Distance;
                    skillInfos.CoolTime = skill_DreamKnight.CoolTime;
                    skillInfos.PersistentTime = skill_DreamKnight.PersistentTime;
                    skillInfos.Consume = skill_DreamKnight.Consume;
                    skillInfos.OtherData = skill_DreamKnight.OtherData;
                    skillInfos.UseStandard = skill_DreamKnight.UseStandard;

                    skillInfos.Lev_CanUser = skill_DreamKnight.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_DreamKnight.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_DreamKnight.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_DreamKnight.UseStandard.GetValue(4);
                    skillInfos.Command = skill_DreamKnight.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_DreamKnight.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Runemage:
                    Skill_MageRuneConfig skill_MageRune = ConfigComponent.Instance.GetItem<Skill_MageRuneConfig>((int)self);
                    if (skill_MageRune == null) return;
                    skillInfos.Id = skill_MageRune.Id;
                    skillInfos.Name = skill_MageRune.Name;
                    skillInfos.Describe = skill_MageRune.Describe;
                    skillInfos.Icon = skill_MageRune.Icon;
                    skillInfos.SoundName = skill_MageRune.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_MageRune.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_MageRune.AttackEffect;
                    skillInfos.HitEffect = skill_MageRune.HitEffect;
                    skillInfos.skillType = skill_MageRune.skillType;
                    skillInfos.DamageWait = skill_MageRune.DamageWait;
                    skillInfos.Distance = skill_MageRune.Distance;
                    skillInfos.CoolTime = skill_MageRune.CoolTime;
                    skillInfos.PersistentTime = skill_MageRune.PersistentTime;
                    skillInfos.Consume = skill_MageRune.Consume;
                    skillInfos.OtherData = skill_MageRune.OtherData;
                    skillInfos.UseStandard = skill_MageRune.UseStandard;

                    skillInfos.Lev_CanUser = skill_MageRune.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_MageRune.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_MageRune.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_MageRune.UseStandard.GetValue(4);
                    skillInfos.Command = skill_MageRune.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_MageRune.UseStandard.GetValue(6);
                    break;
                case E_RoleType.StrongWind:
                    Skill_StrongWindConfig skill_StrongWind = ConfigComponent.Instance.GetItem<Skill_StrongWindConfig>((int)self);
                    if (skill_StrongWind == null) return;
                    skillInfos.Id = skill_StrongWind.Id;
                    skillInfos.Name = skill_StrongWind.Name;
                    skillInfos.Describe = skill_StrongWind.Describe;
                    skillInfos.Icon = skill_StrongWind.Icon;
                    skillInfos.SoundName = skill_StrongWind.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_StrongWind.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_StrongWind.AttackEffect;
                    skillInfos.HitEffect = skill_StrongWind.HitEffect;
                    skillInfos.skillType = skill_StrongWind.skillType;
                    skillInfos.DamageWait = skill_StrongWind.DamageWait;
                    skillInfos.Distance = skill_StrongWind.Distance;
                    skillInfos.CoolTime = skill_StrongWind.CoolTime;
                    skillInfos.PersistentTime = skill_StrongWind.PersistentTime;
                    skillInfos.Consume = skill_StrongWind.Consume;
                    skillInfos.OtherData = skill_StrongWind.OtherData;
                    skillInfos.UseStandard = skill_StrongWind.UseStandard;

                    skillInfos.Lev_CanUser = skill_StrongWind.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_StrongWind.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_StrongWind.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_StrongWind.UseStandard.GetValue(4);
                    skillInfos.Command = skill_StrongWind.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_StrongWind.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Gunners:
                    Skill_MusketeersConfig skill_Musketeers = ConfigComponent.Instance.GetItem<Skill_MusketeersConfig>((int)self);
                    if (skill_Musketeers == null) return;
                    skillInfos.Id = skill_Musketeers.Id;
                    skillInfos.Name = skill_Musketeers.Name;
                    skillInfos.Describe = skill_Musketeers.Describe;
                    skillInfos.Icon = skill_Musketeers.Icon;
                    skillInfos.SoundName = skill_Musketeers.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Musketeers.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Musketeers.AttackEffect;
                    skillInfos.HitEffect = skill_Musketeers.HitEffect;
                    skillInfos.skillType = skill_Musketeers.skillType;
                    skillInfos.DamageWait = skill_Musketeers.DamageWait;
                    skillInfos.Distance = skill_Musketeers.Distance;
                    skillInfos.CoolTime = skill_Musketeers.CoolTime;
                    skillInfos.PersistentTime = skill_Musketeers.PersistentTime;
                    skillInfos.Consume = skill_Musketeers.Consume;
                    skillInfos.OtherData = skill_Musketeers.OtherData;
                    skillInfos.UseStandard = skill_Musketeers.UseStandard;

                    skillInfos.Lev_CanUser = skill_Musketeers.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Musketeers.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Musketeers.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Musketeers.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Musketeers.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Musketeers.UseStandard.GetValue(6);
                    break;
                case E_RoleType.WhiteMagician:
                    Skill_WhiteWizardConfig skill_WhiteWizard = ConfigComponent.Instance.GetItem<Skill_WhiteWizardConfig>((int)self);
                    if (skill_WhiteWizard == null) return;
                    skillInfos.Id = skill_WhiteWizard.Id;
                    skillInfos.Name = skill_WhiteWizard.Name;
                    skillInfos.Describe = skill_WhiteWizard.Describe;
                    skillInfos.Icon = skill_WhiteWizard.Icon;
                    skillInfos.SoundName = skill_WhiteWizard.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_WhiteWizard.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_WhiteWizard.AttackEffect;
                    skillInfos.HitEffect = skill_WhiteWizard.HitEffect;
                    skillInfos.skillType = skill_WhiteWizard.skillType;
                    skillInfos.DamageWait = skill_WhiteWizard.DamageWait;
                    skillInfos.Distance = skill_WhiteWizard.Distance;
                    skillInfos.CoolTime = skill_WhiteWizard.CoolTime;
                    skillInfos.PersistentTime = skill_WhiteWizard.PersistentTime;
                    skillInfos.Consume = skill_WhiteWizard.Consume;
                    skillInfos.OtherData = skill_WhiteWizard.OtherData;
                    skillInfos.UseStandard = skill_WhiteWizard.UseStandard;

                    skillInfos.Lev_CanUser = skill_WhiteWizard.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_WhiteWizard.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_WhiteWizard.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_WhiteWizard.UseStandard.GetValue(4);
                    skillInfos.Command = skill_WhiteWizard.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_WhiteWizard.UseStandard.GetValue(6);
                    break;
                case E_RoleType.WomanMagician:
                    Skill_FemaleWizardConfig skill_FemaleWizard = ConfigComponent.Instance.GetItem<Skill_FemaleWizardConfig>((int)self);
                    if (skill_FemaleWizard == null) return;
                    skillInfos.Id = skill_FemaleWizard.Id;
                    skillInfos.Name = skill_FemaleWizard.Name;
                    skillInfos.Describe = skill_FemaleWizard.Describe;
                    skillInfos.Icon = skill_FemaleWizard.Icon;
                    skillInfos.SoundName = skill_FemaleWizard.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_FemaleWizard.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_FemaleWizard.AttackEffect;
                    skillInfos.HitEffect = skill_FemaleWizard.HitEffect;
                    skillInfos.skillType = skill_FemaleWizard.skillType;
                    skillInfos.DamageWait = skill_FemaleWizard.DamageWait;
                    skillInfos.Distance = skill_FemaleWizard.Distance;
                    skillInfos.CoolTime = skill_FemaleWizard.CoolTime;
                    skillInfos.PersistentTime = skill_FemaleWizard.PersistentTime;
                    skillInfos.Consume = skill_FemaleWizard.Consume;
                    skillInfos.OtherData = skill_FemaleWizard.OtherData;
                    skillInfos.UseStandard = skill_FemaleWizard.UseStandard;

                    skillInfos.Lev_CanUser = skill_FemaleWizard.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_FemaleWizard.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_FemaleWizard.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_FemaleWizard.UseStandard.GetValue(4);
                    skillInfos.Command = skill_FemaleWizard.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_FemaleWizard.UseStandard.GetValue(6);
                    break;
                default:
                    break;
            }

          
        }
        public static void GetSkillInfos_RoleType_Out(this int self, E_RoleType roleType, out SkillInfos skillInfos)
        {
            skillInfos = new SkillInfos();
            switch (roleType)
            {
                case E_RoleType.Magician:
                    Skill_SpellConfig skill_Spell = ConfigComponent.Instance.GetItem<Skill_SpellConfig>((int)self);
                    skillInfos.Id = skill_Spell.Id;
                    skillInfos.Name = skill_Spell.Name;
                    skillInfos.Describe = skill_Spell.Describe;
                    skillInfos.Icon = skill_Spell.Icon;
                    skillInfos.SoundName = skill_Spell.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Spell.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Spell.AttackEffect;
                    skillInfos.HitEffect = skill_Spell.HitEffect;
                    skillInfos.skillType = skill_Spell.skillType;
                    skillInfos.DamageWait = skill_Spell.DamageWait;
                    skillInfos.Distance = skill_Spell.Distance;
                    skillInfos.CoolTime = skill_Spell.CoolTime;
                    skillInfos.PersistentTime = skill_Spell.PersistentTime;
                    skillInfos.Consume = skill_Spell.Consume;
                    skillInfos.OtherData = skill_Spell.OtherData;
                    skillInfos.UseStandard = skill_Spell.UseStandard;

                    skillInfos.Lev_CanUser = skill_Spell.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Spell.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Spell.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Spell.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Spell.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Spell.UseStandard.GetValue(6);
                    break;

                case E_RoleType.Swordsman:
                    Skill_SwordsmanConfig skill_Sword = ConfigComponent.Instance.GetItem<Skill_SwordsmanConfig>((int)self);
                    skillInfos.Id = skill_Sword.Id;
                    skillInfos.Name = skill_Sword.Name;
                    skillInfos.Describe = skill_Sword.Describe;
                    skillInfos.Icon = skill_Sword.Icon;
                    skillInfos.SoundName = skill_Sword.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Sword.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Sword.AttackEffect;
                    skillInfos.HitEffect = skill_Sword.HitEffect;
                    skillInfos.skillType = skill_Sword.skillType;
                    skillInfos.DamageWait = skill_Sword.DamageWait;
                    skillInfos.Distance = skill_Sword.Distance;
                    skillInfos.CoolTime = skill_Sword.CoolTime;
                    skillInfos.PersistentTime = skill_Sword.PersistentTime;
                    skillInfos.Consume = skill_Sword.Consume;
                    skillInfos.OtherData = skill_Sword.OtherData;
                    skillInfos.UseStandard = skill_Sword.UseStandard;

                    skillInfos.Lev_CanUser = skill_Sword.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Sword.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Sword.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Sword.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Sword.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Sword.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Archer:
                    Skill_ArcherConfig skill_Archer = ConfigComponent.Instance.GetItem<Skill_ArcherConfig>((int)self);
                    skillInfos.Id = skill_Archer.Id;
                    skillInfos.Name = skill_Archer.Name;
                    skillInfos.Describe = skill_Archer.Describe;
                    skillInfos.Icon = skill_Archer.Icon;
                    skillInfos.SoundName = skill_Archer.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Archer.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Archer.AttackEffect;
                    skillInfos.HitEffect = skill_Archer.HitEffect;
                    skillInfos.skillType = skill_Archer.skillType;
                    skillInfos.DamageWait = skill_Archer.DamageWait;
                    skillInfos.Distance = skill_Archer.Distance;
                    skillInfos.CoolTime = skill_Archer.CoolTime;
                    skillInfos.PersistentTime = skill_Archer.PersistentTime;
                    skillInfos.Consume = skill_Archer.Consume;
                    skillInfos.OtherData = skill_Archer.OtherData;
                    skillInfos.UseStandard = skill_Archer.UseStandard;

                    skillInfos.Lev_CanUser = skill_Archer.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Archer.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Archer.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Archer.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Archer.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Archer.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Magicswordsman:
                    Skill_SpellswordConfig skill_Spellsword = ConfigComponent.Instance.GetItem<Skill_SpellswordConfig>((int)self);
                    if (skill_Spellsword == null) return;
                    skillInfos.Id = skill_Spellsword.Id;
                    skillInfos.Name = skill_Spellsword.Name;
                    skillInfos.Describe = skill_Spellsword.Describe;
                    skillInfos.Icon = skill_Spellsword.Icon;
                    skillInfos.SoundName = skill_Spellsword.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Spellsword.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Spellsword.AttackEffect;
                    skillInfos.HitEffect = skill_Spellsword.HitEffect;
                    skillInfos.skillType = skill_Spellsword.skillType;
                    skillInfos.DamageWait = skill_Spellsword.DamageWait;
                    skillInfos.Distance = skill_Spellsword.Distance;
                    skillInfos.CoolTime = skill_Spellsword.CoolTime;
                    skillInfos.PersistentTime = skill_Spellsword.PersistentTime;
                    skillInfos.Consume = skill_Spellsword.Consume;
                    skillInfos.OtherData = skill_Spellsword.OtherData;
                    skillInfos.UseStandard = skill_Spellsword.UseStandard;

                    skillInfos.Lev_CanUser = skill_Spellsword.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Spellsword.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Spellsword.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Spellsword.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Spellsword.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Spellsword.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Holymentor:
                    Skill_HolyteacherConfig skill_Holyteacher = ConfigComponent.Instance.GetItem<Skill_HolyteacherConfig>((int)self);
                    if (skill_Holyteacher == null)
                    {
                        if (HolyteacherSkillCompat.TryGet((int)self, out SkillInfos compatSkill))
                        {
                            skillInfos = compatSkill;
                        }
                        return;
                    }
                    skillInfos.Id = skill_Holyteacher.Id;
                    skillInfos.Name = skill_Holyteacher.Name;
                    skillInfos.Describe = skill_Holyteacher.Describe;
                    skillInfos.Icon = skill_Holyteacher.Icon;
                    skillInfos.SoundName = skill_Holyteacher.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Holyteacher.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Holyteacher.AttackEffect;
                    skillInfos.HitEffect = skill_Holyteacher.HitEffect;
                    skillInfos.skillType = skill_Holyteacher.skillType;
                    skillInfos.DamageWait = skill_Holyteacher.DamageWait;
                    skillInfos.Distance = skill_Holyteacher.Distance;
                    skillInfos.CoolTime = skill_Holyteacher.CoolTime;
                    skillInfos.PersistentTime = skill_Holyteacher.PersistentTime;
                    skillInfos.Consume = skill_Holyteacher.Consume;
                    skillInfos.OtherData = skill_Holyteacher.OtherData;
                    skillInfos.UseStandard = skill_Holyteacher.UseStandard;

                    skillInfos.Lev_CanUser = skill_Holyteacher.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Holyteacher.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Holyteacher.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Holyteacher.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Holyteacher.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Holyteacher.UseStandard.GetValue(6);
                    HolyteacherSkillCompat.OverrideIfNeeded((int)self, skillInfos);
                    break;
                case E_RoleType.Summoner:
                    Skill_SummonWarlockConfig skill_SummonWarlock = ConfigComponent.Instance.GetItem<Skill_SummonWarlockConfig>((int)self);
                    if (skill_SummonWarlock == null) return;
                    skillInfos.Id = skill_SummonWarlock.Id;
                    skillInfos.Name = skill_SummonWarlock.Name;
                    skillInfos.Describe = skill_SummonWarlock.Describe;
                    skillInfos.Icon = skill_SummonWarlock.Icon;
                    skillInfos.SoundName = skill_SummonWarlock.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_SummonWarlock.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_SummonWarlock.AttackEffect;
                    skillInfos.HitEffect = skill_SummonWarlock.HitEffect;
                    skillInfos.skillType = skill_SummonWarlock.skillType;
                    skillInfos.DamageWait = skill_SummonWarlock.DamageWait;
                    skillInfos.Distance = skill_SummonWarlock.Distance;
                    skillInfos.CoolTime = skill_SummonWarlock.CoolTime;
                    skillInfos.PersistentTime = skill_SummonWarlock.PersistentTime;
                    skillInfos.Consume = skill_SummonWarlock.Consume;
                    skillInfos.OtherData = skill_SummonWarlock.OtherData;
                    skillInfos.UseStandard = skill_SummonWarlock.UseStandard;

                    skillInfos.Lev_CanUser = skill_SummonWarlock.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_SummonWarlock.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_SummonWarlock.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_SummonWarlock.UseStandard.GetValue(4);
                    skillInfos.Command = skill_SummonWarlock.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_SummonWarlock.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Gladiator:
                    Skill_CombatConfig skill_Combat = ConfigComponent.Instance.GetItem<Skill_CombatConfig>((int)self);
                    if (skill_Combat == null) return;
                    skillInfos.Id = skill_Combat.Id;
                    skillInfos.Name = skill_Combat.Name;
                    skillInfos.Describe = skill_Combat.Describe;
                    skillInfos.Icon = skill_Combat.Icon;
                    skillInfos.SoundName = skill_Combat.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Combat.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Combat.AttackEffect;
                    skillInfos.HitEffect = skill_Combat.HitEffect;
                    skillInfos.skillType = skill_Combat.skillType;
                    skillInfos.DamageWait = skill_Combat.DamageWait;
                    skillInfos.Distance = skill_Combat.Distance;
                    skillInfos.CoolTime = skill_Combat.CoolTime;
                    skillInfos.PersistentTime = skill_Combat.PersistentTime;
                    skillInfos.Consume = skill_Combat.Consume;
                    skillInfos.OtherData = skill_Combat.OtherData;
                    skillInfos.UseStandard = skill_Combat.UseStandard;

                    skillInfos.Lev_CanUser = skill_Combat.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Combat.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Combat.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Combat.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Combat.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Combat.UseStandard.GetValue(6);
                    break;
                case E_RoleType.GrowLancer:
                    Skill_DreamKnightConfig skill_DreamKnight = ConfigComponent.Instance.GetItem<Skill_DreamKnightConfig>((int)self);
                    if (skill_DreamKnight == null) return;
                    skillInfos.Id = skill_DreamKnight.Id;
                    skillInfos.Name = skill_DreamKnight.Name;
                    skillInfos.Describe = skill_DreamKnight.Describe;
                    skillInfos.Icon = skill_DreamKnight.Icon;
                    skillInfos.SoundName = skill_DreamKnight.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_DreamKnight.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_DreamKnight.AttackEffect;
                    skillInfos.HitEffect = skill_DreamKnight.HitEffect;
                    skillInfos.skillType = skill_DreamKnight.skillType;
                    skillInfos.DamageWait = skill_DreamKnight.DamageWait;
                    skillInfos.Distance = skill_DreamKnight.Distance;
                    skillInfos.CoolTime = skill_DreamKnight.CoolTime;
                    skillInfos.PersistentTime = skill_DreamKnight.PersistentTime;
                    skillInfos.Consume = skill_DreamKnight.Consume;
                    skillInfos.OtherData = skill_DreamKnight.OtherData;
                    skillInfos.UseStandard = skill_DreamKnight.UseStandard;

                    skillInfos.Lev_CanUser = skill_DreamKnight.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_DreamKnight.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_DreamKnight.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_DreamKnight.UseStandard.GetValue(4);
                    skillInfos.Command = skill_DreamKnight.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_DreamKnight.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Runemage:
                    Skill_MageRuneConfig skill_MageRune = ConfigComponent.Instance.GetItem<Skill_MageRuneConfig>((int)self);
                    if (skill_MageRune == null) return;
                    skillInfos.Id = skill_MageRune.Id;
                    skillInfos.Name = skill_MageRune.Name;
                    skillInfos.Describe = skill_MageRune.Describe;
                    skillInfos.Icon = skill_MageRune.Icon;
                    skillInfos.SoundName = skill_MageRune.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_MageRune.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_MageRune.AttackEffect;
                    skillInfos.HitEffect = skill_MageRune.HitEffect;
                    skillInfos.skillType = skill_MageRune.skillType;
                    skillInfos.DamageWait = skill_MageRune.DamageWait;
                    skillInfos.Distance = skill_MageRune.Distance;
                    skillInfos.CoolTime = skill_MageRune.CoolTime;
                    skillInfos.PersistentTime = skill_MageRune.PersistentTime;
                    skillInfos.Consume = skill_MageRune.Consume;
                    skillInfos.OtherData = skill_MageRune.OtherData;
                    skillInfos.UseStandard = skill_MageRune.UseStandard;

                    skillInfos.Lev_CanUser = skill_MageRune.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_MageRune.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_MageRune.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_MageRune.UseStandard.GetValue(4);
                    skillInfos.Command = skill_MageRune.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_MageRune.UseStandard.GetValue(6);
                    break;
                case E_RoleType.StrongWind:
                    Skill_StrongWindConfig skill_StrongWind = ConfigComponent.Instance.GetItem<Skill_StrongWindConfig>((int)self);
                    if (skill_StrongWind == null) return;
                    skillInfos.Id = skill_StrongWind.Id;
                    skillInfos.Name = skill_StrongWind.Name;
                    skillInfos.Describe = skill_StrongWind.Describe;
                    skillInfos.Icon = skill_StrongWind.Icon;
                    skillInfos.SoundName = skill_StrongWind.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_StrongWind.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_StrongWind.AttackEffect;
                    skillInfos.HitEffect = skill_StrongWind.HitEffect;
                    skillInfos.skillType = skill_StrongWind.skillType;
                    skillInfos.DamageWait = skill_StrongWind.DamageWait;
                    skillInfos.Distance = skill_StrongWind.Distance;
                    skillInfos.CoolTime = skill_StrongWind.CoolTime;
                    skillInfos.PersistentTime = skill_StrongWind.PersistentTime;
                    skillInfos.Consume = skill_StrongWind.Consume;
                    skillInfos.OtherData = skill_StrongWind.OtherData;
                    skillInfos.UseStandard = skill_StrongWind.UseStandard;

                    skillInfos.Lev_CanUser = skill_StrongWind.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_StrongWind.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_StrongWind.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_StrongWind.UseStandard.GetValue(4);
                    skillInfos.Command = skill_StrongWind.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_StrongWind.UseStandard.GetValue(6);
                    break;
                case E_RoleType.Gunners:
                    Skill_MusketeersConfig skill_Musketeers = ConfigComponent.Instance.GetItem<Skill_MusketeersConfig>((int)self);
                    if (skill_Musketeers == null) return;
                    skillInfos.Id = skill_Musketeers.Id;
                    skillInfos.Name = skill_Musketeers.Name;
                    skillInfos.Describe = skill_Musketeers.Describe;
                    skillInfos.Icon = skill_Musketeers.Icon;
                    skillInfos.SoundName = skill_Musketeers.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Musketeers.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Musketeers.AttackEffect;
                    skillInfos.HitEffect = skill_Musketeers.HitEffect;
                    skillInfos.skillType = skill_Musketeers.skillType;
                    skillInfos.DamageWait = skill_Musketeers.DamageWait;
                    skillInfos.Distance = skill_Musketeers.Distance;
                    skillInfos.CoolTime = skill_Musketeers.CoolTime;
                    skillInfos.PersistentTime = skill_Musketeers.PersistentTime;
                    skillInfos.Consume = skill_Musketeers.Consume;
                    skillInfos.OtherData = skill_Musketeers.OtherData;
                    skillInfos.UseStandard = skill_Musketeers.UseStandard;

                    skillInfos.Lev_CanUser = skill_Musketeers.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Musketeers.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Musketeers.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Musketeers.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Musketeers.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Musketeers.UseStandard.GetValue(6);
                    break;
                case E_RoleType.WhiteMagician:
                    Skill_WhiteWizardConfig skill_WhiteWizard = ConfigComponent.Instance.GetItem<Skill_WhiteWizardConfig>((int)self);
                    if (skill_WhiteWizard == null) return;
                    skillInfos.Id = skill_WhiteWizard.Id;
                    skillInfos.Name = skill_WhiteWizard.Name;
                    skillInfos.Describe = skill_WhiteWizard.Describe;
                    skillInfos.Icon = skill_WhiteWizard.Icon;
                    skillInfos.SoundName = skill_WhiteWizard.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_WhiteWizard.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_WhiteWizard.AttackEffect;
                    skillInfos.HitEffect = skill_WhiteWizard.HitEffect;
                    skillInfos.skillType = skill_WhiteWizard.skillType;
                    skillInfos.DamageWait = skill_WhiteWizard.DamageWait;
                    skillInfos.Distance = skill_WhiteWizard.Distance;
                    skillInfos.CoolTime = skill_WhiteWizard.CoolTime;
                    skillInfos.PersistentTime = skill_WhiteWizard.PersistentTime;
                    skillInfos.Consume = skill_WhiteWizard.Consume;
                    skillInfos.OtherData = skill_WhiteWizard.OtherData;
                    skillInfos.UseStandard = skill_WhiteWizard.UseStandard;

                    skillInfos.Lev_CanUser = skill_WhiteWizard.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_WhiteWizard.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_WhiteWizard.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_WhiteWizard.UseStandard.GetValue(4);
                    skillInfos.Command = skill_WhiteWizard.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_WhiteWizard.UseStandard.GetValue(6);
                    break;
                case E_RoleType.WomanMagician:
                    Skill_FemaleWizardConfig skill_FemaleWizard = ConfigComponent.Instance.GetItem<Skill_FemaleWizardConfig>((int)self);
                    if (skill_FemaleWizard == null) return;
                    skillInfos.Id = skill_FemaleWizard.Id;
                    skillInfos.Name = skill_FemaleWizard.Name;
                    skillInfos.Describe = skill_FemaleWizard.Describe;
                    skillInfos.Icon = skill_FemaleWizard.Icon;
                    skillInfos.SoundName = skill_FemaleWizard.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_FemaleWizard.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_FemaleWizard.AttackEffect;
                    skillInfos.HitEffect = skill_FemaleWizard.HitEffect;
                    skillInfos.skillType = skill_FemaleWizard.skillType;
                    skillInfos.DamageWait = skill_FemaleWizard.DamageWait;
                    skillInfos.Distance = skill_FemaleWizard.Distance;
                    skillInfos.CoolTime = skill_FemaleWizard.CoolTime;
                    skillInfos.PersistentTime = skill_FemaleWizard.PersistentTime;
                    skillInfos.Consume = skill_FemaleWizard.Consume;
                    skillInfos.OtherData = skill_FemaleWizard.OtherData;
                    skillInfos.UseStandard = skill_FemaleWizard.UseStandard;

                    skillInfos.Lev_CanUser = skill_FemaleWizard.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_FemaleWizard.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_FemaleWizard.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_FemaleWizard.UseStandard.GetValue(4);
                    skillInfos.Command = skill_FemaleWizard.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_FemaleWizard.UseStandard.GetValue(6);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 根据技能的配置表id 获取对应的技能配置信息
        /// </summary>
        /// <param name="self">技能的配置表ID</param>
        /// <param name="skillInfos"></param>
        public static void GetSkillInfos__Ref(this int self, ref SkillInfos skillInfos)
        {

            switch (self / 100)
            {

                case 0:
                    Skill_SpellConfig skill_Spell = ConfigComponent.Instance.GetItem<Skill_SpellConfig>((int)self);
                    skillInfos.Id = skill_Spell.Id;
                    skillInfos.Name = skill_Spell.Name;
                    skillInfos.Describe = skill_Spell.Describe;
                    skillInfos.Icon = skill_Spell.Icon;
                    skillInfos.SoundName = skill_Spell.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Spell.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Spell.AttackEffect;
                    skillInfos.HitEffect = skill_Spell.HitEffect;
                    skillInfos.skillType = skill_Spell.skillType;
                    skillInfos.DamageWait = skill_Spell.DamageWait;
                    skillInfos.Distance = skill_Spell.Distance;
                    skillInfos.CoolTime = skill_Spell.CoolTime;
                    skillInfos.PersistentTime = skill_Spell.PersistentTime;
                    skillInfos.Consume = skill_Spell.Consume;
                    skillInfos.OtherData = skill_Spell.OtherData;
                    skillInfos.UseStandard = skill_Spell.UseStandard;

                    skillInfos.Lev_CanUser = skill_Spell.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Spell.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Spell.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Spell.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Spell.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Spell.UseStandard.GetValue(6);
                    break;
                case 1:
                    Skill_SwordsmanConfig skill_Sword = ConfigComponent.Instance.GetItem<Skill_SwordsmanConfig>((int)self);
                    skillInfos.Id = skill_Sword.Id;
                    skillInfos.Name = skill_Sword.Name;
                    skillInfos.Describe = skill_Sword.Describe;
                    skillInfos.Icon = skill_Sword.Icon;
                    skillInfos.SoundName = skill_Sword.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Sword.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Sword.AttackEffect;
                    skillInfos.HitEffect = skill_Sword.HitEffect;
                    skillInfos.skillType = skill_Sword.skillType;
                    skillInfos.DamageWait = skill_Sword.DamageWait;
                    skillInfos.Distance = skill_Sword.Distance;
                    skillInfos.CoolTime = skill_Sword.CoolTime;
                    skillInfos.PersistentTime = skill_Sword.PersistentTime;
                    skillInfos.Consume = skill_Sword.Consume;
                    skillInfos.OtherData = skill_Sword.OtherData;
                    skillInfos.UseStandard = skill_Sword.UseStandard;

                    skillInfos.Lev_CanUser = skill_Sword.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Sword.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Sword.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Sword.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Sword.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Sword.UseStandard.GetValue(6);
                    break;
                case 2:
                    Skill_ArcherConfig skill_Archer = ConfigComponent.Instance.GetItem<Skill_ArcherConfig>((int)self);
                    skillInfos.Id = skill_Archer.Id;
                    skillInfos.Name = skill_Archer.Name;
                    skillInfos.Describe = skill_Archer.Describe;
                    skillInfos.Icon = skill_Archer.Icon;
                    skillInfos.SoundName = skill_Archer.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Archer.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Archer.AttackEffect;
                    skillInfos.HitEffect = skill_Archer.HitEffect;
                    skillInfos.skillType = skill_Archer.skillType;
                    skillInfos.DamageWait = skill_Archer.DamageWait;
                    skillInfos.Distance = skill_Archer.Distance;
                    skillInfos.CoolTime = skill_Archer.CoolTime;
                    skillInfos.PersistentTime = skill_Archer.PersistentTime;
                    skillInfos.Consume = skill_Archer.Consume;
                    skillInfos.OtherData = skill_Archer.OtherData;
                    skillInfos.UseStandard = skill_Archer.UseStandard;

                    skillInfos.Lev_CanUser = skill_Archer.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Archer.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Archer.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Archer.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Archer.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Archer.UseStandard.GetValue(6);
                    break;
                case 3:
                    Skill_SpellswordConfig skill_Spellsword = ConfigComponent.Instance.GetItem<Skill_SpellswordConfig>((int)self);
                    if (skill_Spellsword == null) return;
                    skillInfos.Id = skill_Spellsword.Id;
                    skillInfos.Name = skill_Spellsword.Name;
                    skillInfos.Describe = skill_Spellsword.Describe;
                    skillInfos.Icon = skill_Spellsword.Icon;
                    skillInfos.SoundName = skill_Spellsword.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Spellsword.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Spellsword.AttackEffect;
                    skillInfos.HitEffect = skill_Spellsword.HitEffect;
                    skillInfos.skillType = skill_Spellsword.skillType;
                    skillInfos.DamageWait = skill_Spellsword.DamageWait;
                    skillInfos.Distance = skill_Spellsword.Distance;
                    skillInfos.CoolTime = skill_Spellsword.CoolTime;
                    skillInfos.PersistentTime = skill_Spellsword.PersistentTime;
                    skillInfos.Consume = skill_Spellsword.Consume;
                    skillInfos.OtherData = skill_Spellsword.OtherData;
                    skillInfos.UseStandard = skill_Spellsword.UseStandard;

                    skillInfos.Lev_CanUser = skill_Spellsword.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Spellsword.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Spellsword.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Spellsword.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Spellsword.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Spellsword.UseStandard.GetValue(6);
                    break;
                case 4:
                    Skill_HolyteacherConfig skill_Holyteacher = ConfigComponent.Instance.GetItem<Skill_HolyteacherConfig>((int)self);
                    if (skill_Holyteacher == null)
                    {
                        if (HolyteacherSkillCompat.TryGet((int)self, out SkillInfos compatSkill))
                        {
                            skillInfos = compatSkill;
                        }
                        return;
                    }
                    skillInfos.Id = skill_Holyteacher.Id;
                    skillInfos.Name = skill_Holyteacher.Name;
                    skillInfos.Describe = skill_Holyteacher.Describe;
                    skillInfos.Icon = skill_Holyteacher.Icon;
                    skillInfos.SoundName = skill_Holyteacher.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Holyteacher.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Holyteacher.AttackEffect;
                    skillInfos.HitEffect = skill_Holyteacher.HitEffect;
                    skillInfos.skillType = skill_Holyteacher.skillType;
                    skillInfos.DamageWait = skill_Holyteacher.DamageWait;
                    skillInfos.Distance = skill_Holyteacher.Distance;
                    skillInfos.CoolTime = skill_Holyteacher.CoolTime;
                    skillInfos.PersistentTime = skill_Holyteacher.PersistentTime;
                    skillInfos.Consume = skill_Holyteacher.Consume;
                    skillInfos.OtherData = skill_Holyteacher.OtherData;
                    skillInfos.UseStandard = skill_Holyteacher.UseStandard;

                    skillInfos.Lev_CanUser = skill_Holyteacher.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Holyteacher.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Holyteacher.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Holyteacher.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Holyteacher.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Holyteacher.UseStandard.GetValue(6);
                    HolyteacherSkillCompat.OverrideIfNeeded((int)self, skillInfos);
                    break;
                case 5:
                    Skill_SummonWarlockConfig skill_SummonWarlock = ConfigComponent.Instance.GetItem<Skill_SummonWarlockConfig>((int)self);
                    if (skill_SummonWarlock == null) return;
                    skillInfos.Id = skill_SummonWarlock.Id;
                    skillInfos.Name = skill_SummonWarlock.Name;
                    skillInfos.Describe = skill_SummonWarlock.Describe;
                    skillInfos.Icon = skill_SummonWarlock.Icon;
                    skillInfos.SoundName = skill_SummonWarlock.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_SummonWarlock.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_SummonWarlock.AttackEffect;
                    skillInfos.HitEffect = skill_SummonWarlock.HitEffect;
                    skillInfos.skillType = skill_SummonWarlock.skillType;
                    skillInfos.DamageWait = skill_SummonWarlock.DamageWait;
                    skillInfos.Distance = skill_SummonWarlock.Distance;
                    skillInfos.CoolTime = skill_SummonWarlock.CoolTime;
                    skillInfos.PersistentTime = skill_SummonWarlock.PersistentTime;
                    skillInfos.Consume = skill_SummonWarlock.Consume;
                    skillInfos.OtherData = skill_SummonWarlock.OtherData;
                    skillInfos.UseStandard = skill_SummonWarlock.UseStandard;

                    skillInfos.Lev_CanUser = skill_SummonWarlock.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_SummonWarlock.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_SummonWarlock.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_SummonWarlock.UseStandard.GetValue(4);
                    skillInfos.Command = skill_SummonWarlock.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_SummonWarlock.UseStandard.GetValue(6);
                    break;
                case 6:
                    Skill_CombatConfig skill_Combat = ConfigComponent.Instance.GetItem<Skill_CombatConfig>((int)self);
                    if (skill_Combat == null) return;
                    skillInfos.Id = skill_Combat.Id;
                    skillInfos.Name = skill_Combat.Name;
                    skillInfos.Describe = skill_Combat.Describe;
                    skillInfos.Icon = skill_Combat.Icon;
                    skillInfos.SoundName = skill_Combat.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Combat.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Combat.AttackEffect;
                    skillInfos.HitEffect = skill_Combat.HitEffect;
                    skillInfos.skillType = skill_Combat.skillType;
                    skillInfos.DamageWait = skill_Combat.DamageWait;
                    skillInfos.Distance = skill_Combat.Distance;
                    skillInfos.CoolTime = skill_Combat.CoolTime;
                    skillInfos.PersistentTime = skill_Combat.PersistentTime;
                    skillInfos.Consume = skill_Combat.Consume;
                    skillInfos.OtherData = skill_Combat.OtherData;
                    skillInfos.UseStandard = skill_Combat.UseStandard;

                    skillInfos.Lev_CanUser = skill_Combat.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Combat.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Combat.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Combat.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Combat.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Combat.UseStandard.GetValue(6);
                    break;
                case 7:
                    Skill_DreamKnightConfig skill_DreamKnight = ConfigComponent.Instance.GetItem<Skill_DreamKnightConfig>((int)self);
                    if (skill_DreamKnight == null) return;
                    skillInfos.Id = skill_DreamKnight.Id;
                    skillInfos.Name = skill_DreamKnight.Name;
                    skillInfos.Describe = skill_DreamKnight.Describe;
                    skillInfos.Icon = skill_DreamKnight.Icon;
                    skillInfos.SoundName = skill_DreamKnight.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_DreamKnight.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_DreamKnight.AttackEffect;
                    skillInfos.HitEffect = skill_DreamKnight.HitEffect;
                    skillInfos.skillType = skill_DreamKnight.skillType;
                    skillInfos.DamageWait = skill_DreamKnight.DamageWait;
                    skillInfos.Distance = skill_DreamKnight.Distance;
                    skillInfos.CoolTime = skill_DreamKnight.CoolTime;
                    skillInfos.PersistentTime = skill_DreamKnight.PersistentTime;
                    skillInfos.Consume = skill_DreamKnight.Consume;
                    skillInfos.OtherData = skill_DreamKnight.OtherData;
                    skillInfos.UseStandard = skill_DreamKnight.UseStandard;

                    skillInfos.Lev_CanUser = skill_DreamKnight.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_DreamKnight.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_DreamKnight.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_DreamKnight.UseStandard.GetValue(4);
                    skillInfos.Command = skill_DreamKnight.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_DreamKnight.UseStandard.GetValue(6);
                    break;
                case 8:
                    Skill_MageRuneConfig skill_MageRune = ConfigComponent.Instance.GetItem<Skill_MageRuneConfig>((int)self);
                    if (skill_MageRune == null) return;
                    skillInfos.Id = skill_MageRune.Id;
                    skillInfos.Name = skill_MageRune.Name;
                    skillInfos.Describe = skill_MageRune.Describe;
                    skillInfos.Icon = skill_MageRune.Icon;
                    skillInfos.SoundName = skill_MageRune.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_MageRune.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_MageRune.AttackEffect;
                    skillInfos.HitEffect = skill_MageRune.HitEffect;
                    skillInfos.skillType = skill_MageRune.skillType;
                    skillInfos.DamageWait = skill_MageRune.DamageWait;
                    skillInfos.Distance = skill_MageRune.Distance;
                    skillInfos.CoolTime = skill_MageRune.CoolTime;
                    skillInfos.PersistentTime = skill_MageRune.PersistentTime;
                    skillInfos.Consume = skill_MageRune.Consume;
                    skillInfos.OtherData = skill_MageRune.OtherData;
                    skillInfos.UseStandard = skill_MageRune.UseStandard;

                    skillInfos.Lev_CanUser = skill_MageRune.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_MageRune.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_MageRune.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_MageRune.UseStandard.GetValue(4);
                    skillInfos.Command = skill_MageRune.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_MageRune.UseStandard.GetValue(6);
                    break;
                case 9:
                    Skill_StrongWindConfig skill_StrongWind = ConfigComponent.Instance.GetItem<Skill_StrongWindConfig>((int)self);
                    if (skill_StrongWind == null) return;
                    skillInfos.Id = skill_StrongWind.Id;
                    skillInfos.Name = skill_StrongWind.Name;
                    skillInfos.Describe = skill_StrongWind.Describe;
                    skillInfos.Icon = skill_StrongWind.Icon;
                    skillInfos.SoundName = skill_StrongWind.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_StrongWind.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_StrongWind.AttackEffect;
                    skillInfos.HitEffect = skill_StrongWind.HitEffect;
                    skillInfos.skillType = skill_StrongWind.skillType;
                    skillInfos.DamageWait = skill_StrongWind.DamageWait;
                    skillInfos.Distance = skill_StrongWind.Distance;
                    skillInfos.CoolTime = skill_StrongWind.CoolTime;
                    skillInfos.PersistentTime = skill_StrongWind.PersistentTime;
                    skillInfos.Consume = skill_StrongWind.Consume;
                    skillInfos.OtherData = skill_StrongWind.OtherData;
                    skillInfos.UseStandard = skill_StrongWind.UseStandard;

                    skillInfos.Lev_CanUser = skill_StrongWind.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_StrongWind.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_StrongWind.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_StrongWind.UseStandard.GetValue(4);
                    skillInfos.Command = skill_StrongWind.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_StrongWind.UseStandard.GetValue(6);
                    break;
                case 10:
                    Skill_MusketeersConfig skill_Musketeers = ConfigComponent.Instance.GetItem<Skill_MusketeersConfig>((int)self);
                    if (skill_Musketeers == null) return;
                    skillInfos.Id = skill_Musketeers.Id;
                    skillInfos.Name = skill_Musketeers.Name;
                    skillInfos.Describe = skill_Musketeers.Describe;
                    skillInfos.Icon = skill_Musketeers.Icon;
                    skillInfos.SoundName = skill_Musketeers.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Musketeers.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Musketeers.AttackEffect;
                    skillInfos.HitEffect = skill_Musketeers.HitEffect;
                    skillInfos.skillType = skill_Musketeers.skillType;
                    skillInfos.DamageWait = skill_Musketeers.DamageWait;
                    skillInfos.Distance = skill_Musketeers.Distance;
                    skillInfos.CoolTime = skill_Musketeers.CoolTime;
                    skillInfos.PersistentTime = skill_Musketeers.PersistentTime;
                    skillInfos.Consume = skill_Musketeers.Consume;
                    skillInfos.OtherData = skill_Musketeers.OtherData;
                    skillInfos.UseStandard = skill_Musketeers.UseStandard;

                    skillInfos.Lev_CanUser = skill_Musketeers.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Musketeers.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Musketeers.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Musketeers.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Musketeers.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Musketeers.UseStandard.GetValue(6);
                    break;
                case 11:
                    Skill_WhiteWizardConfig skill_WhiteWizard = ConfigComponent.Instance.GetItem<Skill_WhiteWizardConfig>((int)self);
                    if (skill_WhiteWizard == null) return;
                    skillInfos.Id = skill_WhiteWizard.Id;
                    skillInfos.Name = skill_WhiteWizard.Name;
                    skillInfos.Describe = skill_WhiteWizard.Describe;
                    skillInfos.Icon = skill_WhiteWizard.Icon;
                    skillInfos.SoundName = skill_WhiteWizard.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_WhiteWizard.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_WhiteWizard.AttackEffect;
                    skillInfos.HitEffect = skill_WhiteWizard.HitEffect;
                    skillInfos.skillType = skill_WhiteWizard.skillType;
                    skillInfos.DamageWait = skill_WhiteWizard.DamageWait;
                    skillInfos.Distance = skill_WhiteWizard.Distance;
                    skillInfos.CoolTime = skill_WhiteWizard.CoolTime;
                    skillInfos.PersistentTime = skill_WhiteWizard.PersistentTime;
                    skillInfos.Consume = skill_WhiteWizard.Consume;
                    skillInfos.OtherData = skill_WhiteWizard.OtherData;
                    skillInfos.UseStandard = skill_WhiteWizard.UseStandard;

                    skillInfos.Lev_CanUser = skill_WhiteWizard.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_WhiteWizard.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_WhiteWizard.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_WhiteWizard.UseStandard.GetValue(4);
                    skillInfos.Command = skill_WhiteWizard.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_WhiteWizard.UseStandard.GetValue(6);
                    break;
                case 12:
                    Skill_FemaleWizardConfig skill_FemaleWizard = ConfigComponent.Instance.GetItem<Skill_FemaleWizardConfig>((int)self);
                    if (skill_FemaleWizard == null) return;
                    skillInfos.Id = skill_FemaleWizard.Id;
                    skillInfos.Name = skill_FemaleWizard.Name;
                    skillInfos.Describe = skill_FemaleWizard.Describe;
                    skillInfos.Icon = skill_FemaleWizard.Icon;
                    skillInfos.SoundName = skill_FemaleWizard.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_FemaleWizard.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_FemaleWizard.AttackEffect;
                    skillInfos.HitEffect = skill_FemaleWizard.HitEffect;
                    skillInfos.skillType = skill_FemaleWizard.skillType;
                    skillInfos.DamageWait = skill_FemaleWizard.DamageWait;
                    skillInfos.Distance = skill_FemaleWizard.Distance;
                    skillInfos.CoolTime = skill_FemaleWizard.CoolTime;
                    skillInfos.PersistentTime = skill_FemaleWizard.PersistentTime;
                    skillInfos.Consume = skill_FemaleWizard.Consume;
                    skillInfos.OtherData = skill_FemaleWizard.OtherData;
                    skillInfos.UseStandard = skill_FemaleWizard.UseStandard;

                    skillInfos.Lev_CanUser = skill_FemaleWizard.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_FemaleWizard.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_FemaleWizard.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_FemaleWizard.UseStandard.GetValue(4);
                    skillInfos.Command = skill_FemaleWizard.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_FemaleWizard.UseStandard.GetValue(6);
                    break;
                case 100://怪物 技能
                    Skill_monsterConfig skill_Monster = ConfigComponent.Instance.GetItem<Skill_monsterConfig>((int)self);
                    if (skill_Monster == null) return;
                    skillInfos.Id = skill_Monster.Id;
                    skillInfos.Name = skill_Monster.Name;
                    skillInfos.Describe = skill_Monster.Describe;
                    skillInfos.Icon = skill_Monster.Icon;
                    skillInfos.SoundName = skill_Monster.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Monster.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Monster.AttackEffect;
                    skillInfos.HitEffect = skill_Monster.HitEffect;
                    skillInfos.skillType = skill_Monster.skillType;
                    skillInfos.DamageWait = skill_Monster.DamageWait;
                    skillInfos.Distance = skill_Monster.Distance;
                    skillInfos.CoolTime = skill_Monster.CoolTime;
                    skillInfos.PersistentTime = skill_Monster.PersistentTime;
                    skillInfos.Consume = skill_Monster.Consume;
                    skillInfos.OtherData = skill_Monster.OtherData;
                    skillInfos.UseStandard = skill_Monster.UseStandard;

                    skillInfos.Lev_CanUser = skill_Monster.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Monster.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Monster.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Monster.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Monster.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Monster.UseStandard.GetValue(6);
                    break;
                default:
                    break;
            }
        }
        public static void GetSkillInfos__Out(this int self, out SkillInfos skillInfos)
        {
            skillInfos = new SkillInfos();
            switch (self / 100)
            {
                case 0:
                    if (ConfigComponent.Instance.GetItem<Skill_SpellConfig>((int)self) is Skill_SpellConfig skill_Spell)
                    {
                        skillInfos.Id = skill_Spell.Id;
                        skillInfos.Name = skill_Spell.Name;
                        skillInfos.Describe = skill_Spell.Describe;
                        skillInfos.Icon = skill_Spell.Icon;
                        skillInfos.SoundName = skill_Spell.SoundName;
                        skillInfos.AnimatorTriggerIndex = skill_Spell.AnimatorTriggerIndex;
                        skillInfos.AttackEffect = skill_Spell.AttackEffect;
                        skillInfos.HitEffect = skill_Spell.HitEffect;
                        skillInfos.skillType = skill_Spell.skillType;
                        skillInfos.DamageWait = skill_Spell.DamageWait;
                        skillInfos.Distance = skill_Spell.Distance;
                        skillInfos.CoolTime = skill_Spell.CoolTime;
                        skillInfos.PersistentTime = skill_Spell.PersistentTime;
                        skillInfos.Consume = skill_Spell.Consume;
                        skillInfos.OtherData = skill_Spell.OtherData;
                        skillInfos.UseStandard = skill_Spell.UseStandard;

                        skillInfos.Lev_CanUser = skill_Spell.UseStandard.GetValue(1);//使用等级
                        skillInfos.Strength = skill_Spell.UseStandard.GetValue(2);
                        skillInfos.Intell = skill_Spell.UseStandard.GetValue(3);
                        skillInfos.Agile = skill_Spell.UseStandard.GetValue(4);
                        skillInfos.Command = skill_Spell.UseStandard.GetValue(5);
                        skillInfos.PhyStength = skill_Spell.UseStandard.GetValue(6);
                    }
                    break;
                case 1:
                    if (ConfigComponent.Instance.GetItem<Skill_SwordsmanConfig>((int)self) is Skill_SwordsmanConfig skill_Sword)
                    {
                        skillInfos.Id = skill_Sword.Id;
                        skillInfos.Name = skill_Sword.Name;
                        skillInfos.Describe = skill_Sword.Describe;
                        skillInfos.Icon = skill_Sword.Icon;
                        skillInfos.SoundName = skill_Sword.SoundName;
                        skillInfos.AnimatorTriggerIndex = skill_Sword.AnimatorTriggerIndex;
                        skillInfos.AttackEffect = skill_Sword.AttackEffect;
                        skillInfos.HitEffect = skill_Sword.HitEffect;
                        skillInfos.skillType = skill_Sword.skillType;
                        skillInfos.DamageWait = skill_Sword.DamageWait;
                        skillInfos.Distance = skill_Sword.Distance;
                        skillInfos.CoolTime = skill_Sword.CoolTime;
                        skillInfos.PersistentTime = skill_Sword.PersistentTime;
                        skillInfos.Consume = skill_Sword.Consume;
                        skillInfos.OtherData = skill_Sword.OtherData;
                        skillInfos.UseStandard = skill_Sword.UseStandard;

                        skillInfos.Lev_CanUser = skill_Sword.UseStandard.GetValue(1);//使用等级
                        skillInfos.Strength = skill_Sword.UseStandard.GetValue(2);
                        skillInfos.Intell = skill_Sword.UseStandard.GetValue(3);
                        skillInfos.Agile = skill_Sword.UseStandard.GetValue(4);
                        skillInfos.Command = skill_Sword.UseStandard.GetValue(5);
                        skillInfos.PhyStength = skill_Sword.UseStandard.GetValue(6);
                    }
                    break;
                case 2:
                    if (ConfigComponent.Instance.GetItem<Skill_ArcherConfig>((int)self) is Skill_ArcherConfig skill_Archer)
                    {
                        skillInfos.Id = skill_Archer.Id;
                        skillInfos.Name = skill_Archer.Name;
                        skillInfos.Describe = skill_Archer.Describe;
                        skillInfos.Icon = skill_Archer.Icon;
                        skillInfos.SoundName = skill_Archer.SoundName;
                        skillInfos.AnimatorTriggerIndex = skill_Archer.AnimatorTriggerIndex;
                        skillInfos.AttackEffect = skill_Archer.AttackEffect;
                        skillInfos.HitEffect = skill_Archer.HitEffect;
                        skillInfos.skillType = skill_Archer.skillType;
                        skillInfos.DamageWait = skill_Archer.DamageWait;
                        skillInfos.Distance = skill_Archer.Distance;
                        skillInfos.CoolTime = skill_Archer.CoolTime;
                        skillInfos.PersistentTime = skill_Archer.PersistentTime;
                        skillInfos.Consume = skill_Archer.Consume;
                        skillInfos.OtherData = skill_Archer.OtherData;
                        skillInfos.UseStandard = skill_Archer.UseStandard;

                        skillInfos.Lev_CanUser = skill_Archer.UseStandard.GetValue(1);//使用等级
                        skillInfos.Strength = skill_Archer.UseStandard.GetValue(2);
                        skillInfos.Intell = skill_Archer.UseStandard.GetValue(3);
                        skillInfos.Agile = skill_Archer.UseStandard.GetValue(4);
                        skillInfos.Command = skill_Archer.UseStandard.GetValue(5);
                        skillInfos.PhyStength = skill_Archer.UseStandard.GetValue(6);
                    }
                    break;
                case 3:
                    Skill_SpellswordConfig skill_Spellsword = ConfigComponent.Instance.GetItem<Skill_SpellswordConfig>((int)self);
                    if (skill_Spellsword == null) return;
                    skillInfos.Id = skill_Spellsword.Id;
                    skillInfos.Name = skill_Spellsword.Name;
                    skillInfos.Describe = skill_Spellsword.Describe;
                    skillInfos.Icon = skill_Spellsword.Icon;
                    skillInfos.SoundName = skill_Spellsword.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Spellsword.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Spellsword.AttackEffect;
                    skillInfos.HitEffect = skill_Spellsword.HitEffect;
                    skillInfos.skillType = skill_Spellsword.skillType;
                    skillInfos.DamageWait = skill_Spellsword.DamageWait;
                    skillInfos.Distance = skill_Spellsword.Distance;
                    skillInfos.CoolTime = skill_Spellsword.CoolTime;
                    skillInfos.PersistentTime = skill_Spellsword.PersistentTime;
                    skillInfos.Consume = skill_Spellsword.Consume;
                    skillInfos.OtherData = skill_Spellsword.OtherData;
                    skillInfos.UseStandard = skill_Spellsword.UseStandard;

                    skillInfos.Lev_CanUser = skill_Spellsword.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Spellsword.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Spellsword.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Spellsword.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Spellsword.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Spellsword.UseStandard.GetValue(6);
                    break;
                case 4:
                    Skill_HolyteacherConfig skill_Holyteacher = ConfigComponent.Instance.GetItem<Skill_HolyteacherConfig>((int)self);
                    if (skill_Holyteacher == null) return;
                    skillInfos.Id = skill_Holyteacher.Id;
                    skillInfos.Name = skill_Holyteacher.Name;
                    skillInfos.Describe = skill_Holyteacher.Describe;
                    skillInfos.Icon = skill_Holyteacher.Icon;
                    skillInfos.SoundName = skill_Holyteacher.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Holyteacher.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Holyteacher.AttackEffect;
                    skillInfos.HitEffect = skill_Holyteacher.HitEffect;
                    skillInfos.skillType = skill_Holyteacher.skillType;
                    skillInfos.DamageWait = skill_Holyteacher.DamageWait;
                    skillInfos.Distance = skill_Holyteacher.Distance;
                    skillInfos.CoolTime = skill_Holyteacher.CoolTime;
                    skillInfos.PersistentTime = skill_Holyteacher.PersistentTime;
                    skillInfos.Consume = skill_Holyteacher.Consume;
                    skillInfos.OtherData = skill_Holyteacher.OtherData;
                    skillInfos.UseStandard = skill_Holyteacher.UseStandard;

                    skillInfos.Lev_CanUser = skill_Holyteacher.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Holyteacher.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Holyteacher.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Holyteacher.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Holyteacher.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Holyteacher.UseStandard.GetValue(6);
                    HolyteacherSkillCompat.OverrideIfNeeded((int)self, skillInfos);
                    break;
                case 5:
                    Skill_SummonWarlockConfig skill_SummonWarlock = ConfigComponent.Instance.GetItem<Skill_SummonWarlockConfig>((int)self);
                    if (skill_SummonWarlock == null) return;
                    skillInfos.Id = skill_SummonWarlock.Id;
                    skillInfos.Name = skill_SummonWarlock.Name;
                    skillInfos.Describe = skill_SummonWarlock.Describe;
                    skillInfos.Icon = skill_SummonWarlock.Icon;
                    skillInfos.SoundName = skill_SummonWarlock.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_SummonWarlock.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_SummonWarlock.AttackEffect;
                    skillInfos.HitEffect = skill_SummonWarlock.HitEffect;
                    skillInfos.skillType = skill_SummonWarlock.skillType;
                    skillInfos.DamageWait = skill_SummonWarlock.DamageWait;
                    skillInfos.Distance = skill_SummonWarlock.Distance;
                    skillInfos.CoolTime = skill_SummonWarlock.CoolTime;
                    skillInfos.PersistentTime = skill_SummonWarlock.PersistentTime;
                    skillInfos.Consume = skill_SummonWarlock.Consume;
                    skillInfos.OtherData = skill_SummonWarlock.OtherData;
                    skillInfos.UseStandard = skill_SummonWarlock.UseStandard;

                    skillInfos.Lev_CanUser = skill_SummonWarlock.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_SummonWarlock.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_SummonWarlock.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_SummonWarlock.UseStandard.GetValue(4);
                    skillInfos.Command = skill_SummonWarlock.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_SummonWarlock.UseStandard.GetValue(6);
                    break;
                case 6:
                    Skill_CombatConfig skill_Combat = ConfigComponent.Instance.GetItem<Skill_CombatConfig>((int)self);
                    if (skill_Combat == null) return;
                    skillInfos.Id = skill_Combat.Id;
                    skillInfos.Name = skill_Combat.Name;
                    skillInfos.Describe = skill_Combat.Describe;
                    skillInfos.Icon = skill_Combat.Icon;
                    skillInfos.SoundName = skill_Combat.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Combat.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Combat.AttackEffect;
                    skillInfos.HitEffect = skill_Combat.HitEffect;
                    skillInfos.skillType = skill_Combat.skillType;
                    skillInfos.DamageWait = skill_Combat.DamageWait;
                    skillInfos.Distance = skill_Combat.Distance;
                    skillInfos.CoolTime = skill_Combat.CoolTime;
                    skillInfos.PersistentTime = skill_Combat.PersistentTime;
                    skillInfos.Consume = skill_Combat.Consume;
                    skillInfos.OtherData = skill_Combat.OtherData;
                    skillInfos.UseStandard = skill_Combat.UseStandard;

                    skillInfos.Lev_CanUser = skill_Combat.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Combat.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Combat.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Combat.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Combat.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Combat.UseStandard.GetValue(6);
                    break;
                case 7:
                    Skill_DreamKnightConfig skill_DreamKnight = ConfigComponent.Instance.GetItem<Skill_DreamKnightConfig>((int)self);
                    if (skill_DreamKnight == null) return;
                    skillInfos.Id = skill_DreamKnight.Id;
                    skillInfos.Name = skill_DreamKnight.Name;
                    skillInfos.Describe = skill_DreamKnight.Describe;
                    skillInfos.Icon = skill_DreamKnight.Icon;
                    skillInfos.SoundName = skill_DreamKnight.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_DreamKnight.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_DreamKnight.AttackEffect;
                    skillInfos.HitEffect = skill_DreamKnight.HitEffect;
                    skillInfos.skillType = skill_DreamKnight.skillType;
                    skillInfos.DamageWait = skill_DreamKnight.DamageWait;
                    skillInfos.Distance = skill_DreamKnight.Distance;
                    skillInfos.CoolTime = skill_DreamKnight.CoolTime;
                    skillInfos.PersistentTime = skill_DreamKnight.PersistentTime;
                    skillInfos.Consume = skill_DreamKnight.Consume;
                    skillInfos.OtherData = skill_DreamKnight.OtherData;
                    skillInfos.UseStandard = skill_DreamKnight.UseStandard;

                    skillInfos.Lev_CanUser = skill_DreamKnight.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_DreamKnight.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_DreamKnight.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_DreamKnight.UseStandard.GetValue(4);
                    skillInfos.Command = skill_DreamKnight.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_DreamKnight.UseStandard.GetValue(6);
                    break;
                case 8:
                    Skill_MageRuneConfig skill_MageRune = ConfigComponent.Instance.GetItem<Skill_MageRuneConfig>((int)self);
                    if (skill_MageRune == null) return;
                    skillInfos.Id = skill_MageRune.Id;
                    skillInfos.Name = skill_MageRune.Name;
                    skillInfos.Describe = skill_MageRune.Describe;
                    skillInfos.Icon = skill_MageRune.Icon;
                    skillInfos.SoundName = skill_MageRune.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_MageRune.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_MageRune.AttackEffect;
                    skillInfos.HitEffect = skill_MageRune.HitEffect;
                    skillInfos.skillType = skill_MageRune.skillType;
                    skillInfos.DamageWait = skill_MageRune.DamageWait;
                    skillInfos.Distance = skill_MageRune.Distance;
                    skillInfos.CoolTime = skill_MageRune.CoolTime;
                    skillInfos.PersistentTime = skill_MageRune.PersistentTime;
                    skillInfos.Consume = skill_MageRune.Consume;
                    skillInfos.OtherData = skill_MageRune.OtherData;
                    skillInfos.UseStandard = skill_MageRune.UseStandard;

                    skillInfos.Lev_CanUser = skill_MageRune.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_MageRune.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_MageRune.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_MageRune.UseStandard.GetValue(4);
                    skillInfos.Command = skill_MageRune.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_MageRune.UseStandard.GetValue(6);
                    break;
                case 9:
                    Skill_StrongWindConfig skill_StrongWind = ConfigComponent.Instance.GetItem<Skill_StrongWindConfig>((int)self);
                    if (skill_StrongWind == null) return;
                    skillInfos.Id = skill_StrongWind.Id;
                    skillInfos.Name = skill_StrongWind.Name;
                    skillInfos.Describe = skill_StrongWind.Describe;
                    skillInfos.Icon = skill_StrongWind.Icon;
                    skillInfos.SoundName = skill_StrongWind.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_StrongWind.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_StrongWind.AttackEffect;
                    skillInfos.HitEffect = skill_StrongWind.HitEffect;
                    skillInfos.skillType = skill_StrongWind.skillType;
                    skillInfos.DamageWait = skill_StrongWind.DamageWait;
                    skillInfos.Distance = skill_StrongWind.Distance;
                    skillInfos.CoolTime = skill_StrongWind.CoolTime;
                    skillInfos.PersistentTime = skill_StrongWind.PersistentTime;
                    skillInfos.Consume = skill_StrongWind.Consume;
                    skillInfos.OtherData = skill_StrongWind.OtherData;
                    skillInfos.UseStandard = skill_StrongWind.UseStandard;

                    skillInfos.Lev_CanUser = skill_StrongWind.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_StrongWind.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_StrongWind.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_StrongWind.UseStandard.GetValue(4);
                    skillInfos.Command = skill_StrongWind.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_StrongWind.UseStandard.GetValue(6);
                    break;
                case 10:
                    Skill_MusketeersConfig skill_Musketeers = ConfigComponent.Instance.GetItem<Skill_MusketeersConfig>((int)self);
                    if (skill_Musketeers == null) return;
                    skillInfos.Id = skill_Musketeers.Id;
                    skillInfos.Name = skill_Musketeers.Name;
                    skillInfos.Describe = skill_Musketeers.Describe;
                    skillInfos.Icon = skill_Musketeers.Icon;
                    skillInfos.SoundName = skill_Musketeers.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Musketeers.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Musketeers.AttackEffect;
                    skillInfos.HitEffect = skill_Musketeers.HitEffect;
                    skillInfos.skillType = skill_Musketeers.skillType;
                    skillInfos.DamageWait = skill_Musketeers.DamageWait;
                    skillInfos.Distance = skill_Musketeers.Distance;
                    skillInfos.CoolTime = skill_Musketeers.CoolTime;
                    skillInfos.PersistentTime = skill_Musketeers.PersistentTime;
                    skillInfos.Consume = skill_Musketeers.Consume;
                    skillInfos.OtherData = skill_Musketeers.OtherData;
                    skillInfos.UseStandard = skill_Musketeers.UseStandard;

                    skillInfos.Lev_CanUser = skill_Musketeers.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Musketeers.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Musketeers.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Musketeers.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Musketeers.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Musketeers.UseStandard.GetValue(6);
                    break;
                case 11:
                    Skill_WhiteWizardConfig skill_WhiteWizard = ConfigComponent.Instance.GetItem<Skill_WhiteWizardConfig>((int)self);
                    if (skill_WhiteWizard == null) return;
                    skillInfos.Id = skill_WhiteWizard.Id;
                    skillInfos.Name = skill_WhiteWizard.Name;
                    skillInfos.Describe = skill_WhiteWizard.Describe;
                    skillInfos.Icon = skill_WhiteWizard.Icon;
                    skillInfos.SoundName = skill_WhiteWizard.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_WhiteWizard.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_WhiteWizard.AttackEffect;
                    skillInfos.HitEffect = skill_WhiteWizard.HitEffect;
                    skillInfos.skillType = skill_WhiteWizard.skillType;
                    skillInfos.DamageWait = skill_WhiteWizard.DamageWait;
                    skillInfos.Distance = skill_WhiteWizard.Distance;
                    skillInfos.CoolTime = skill_WhiteWizard.CoolTime;
                    skillInfos.PersistentTime = skill_WhiteWizard.PersistentTime;
                    skillInfos.Consume = skill_WhiteWizard.Consume;
                    skillInfos.OtherData = skill_WhiteWizard.OtherData;
                    skillInfos.UseStandard = skill_WhiteWizard.UseStandard;

                    skillInfos.Lev_CanUser = skill_WhiteWizard.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_WhiteWizard.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_WhiteWizard.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_WhiteWizard.UseStandard.GetValue(4);
                    skillInfos.Command = skill_WhiteWizard.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_WhiteWizard.UseStandard.GetValue(6);
                    break;
                case 12:
                    Skill_FemaleWizardConfig skill_FemaleWizard = ConfigComponent.Instance.GetItem<Skill_FemaleWizardConfig>((int)self);
                    if (skill_FemaleWizard == null) return;
                    skillInfos.Id = skill_FemaleWizard.Id;
                    skillInfos.Name = skill_FemaleWizard.Name;
                    skillInfos.Describe = skill_FemaleWizard.Describe;
                    skillInfos.Icon = skill_FemaleWizard.Icon;
                    skillInfos.SoundName = skill_FemaleWizard.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_FemaleWizard.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_FemaleWizard.AttackEffect;
                    skillInfos.HitEffect = skill_FemaleWizard.HitEffect;
                    skillInfos.skillType = skill_FemaleWizard.skillType;
                    skillInfos.DamageWait = skill_FemaleWizard.DamageWait;
                    skillInfos.Distance = skill_FemaleWizard.Distance;
                    skillInfos.CoolTime = skill_FemaleWizard.CoolTime;
                    skillInfos.PersistentTime = skill_FemaleWizard.PersistentTime;
                    skillInfos.Consume = skill_FemaleWizard.Consume;
                    skillInfos.OtherData = skill_FemaleWizard.OtherData;
                    skillInfos.UseStandard = skill_FemaleWizard.UseStandard;

                    skillInfos.Lev_CanUser = skill_FemaleWizard.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_FemaleWizard.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_FemaleWizard.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_FemaleWizard.UseStandard.GetValue(4);
                    skillInfos.Command = skill_FemaleWizard.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_FemaleWizard.UseStandard.GetValue(6);
                    break;
                case 100://怪物 技能
                    Skill_monsterConfig skill_Monster = ConfigComponent.Instance.GetItem<Skill_monsterConfig>((int)self);
                    if (skill_Monster == null) return;
                    skillInfos.Id = skill_Monster.Id;
                    skillInfos.Name = skill_Monster.Name;
                    skillInfos.Describe = skill_Monster.Describe;
                    skillInfos.Icon = skill_Monster.Icon;
                    skillInfos.SoundName = skill_Monster.SoundName;
                    skillInfos.AnimatorTriggerIndex = skill_Monster.AnimatorTriggerIndex;
                    skillInfos.AttackEffect = skill_Monster.AttackEffect;
                    skillInfos.HitEffect = skill_Monster.HitEffect;
                    skillInfos.skillType = skill_Monster.skillType;
                    skillInfos.DamageWait = skill_Monster.DamageWait;
                    skillInfos.Distance = skill_Monster.Distance;
                    skillInfos.CoolTime = skill_Monster.CoolTime;
                    skillInfos.PersistentTime = skill_Monster.PersistentTime;
                    skillInfos.Consume = skill_Monster.Consume;
                    skillInfos.OtherData = skill_Monster.OtherData;
                    skillInfos.UseStandard = skill_Monster.UseStandard;

                    skillInfos.Lev_CanUser = skill_Monster.UseStandard.GetValue(1);//使用等级
                    skillInfos.Strength = skill_Monster.UseStandard.GetValue(2);
                    skillInfos.Intell = skill_Monster.UseStandard.GetValue(3);
                    skillInfos.Agile = skill_Monster.UseStandard.GetValue(4);
                    skillInfos.Command = skill_Monster.UseStandard.GetValue(5);
                    skillInfos.PhyStength = skill_Monster.UseStandard.GetValue(6);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 更具角色类型 得到该角色的所有技能
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <param name="skillInfos">对应的技能集合</param>
        public static void GetAllSkillConfig(this E_RoleType roleType, out List<SkillInfos> skillInfos)
        {
            skillInfos = new List<SkillInfos>();
            switch (roleType)
            {
                case E_RoleType.Magician:
                    var skilllist = ConfigComponent.Instance.GetAll<Skill_SpellConfig>();
                    foreach (Skill_SpellConfig skill_Info in skilllist.Cast<Skill_SpellConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.Swordsman:
                    var skilSwordllist = ConfigComponent.Instance.GetAll<Skill_SwordsmanConfig>();
                    foreach (Skill_SwordsmanConfig skill_Info in skilSwordllist.Cast<Skill_SwordsmanConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.Archer:
                    var skilArcherllist = ConfigComponent.Instance.GetAll<Skill_ArcherConfig>();
                    foreach (Skill_ArcherConfig skill_Info in skilArcherllist.Cast<Skill_ArcherConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.Magicswordsman:
                    var skilMagicswordlist = ConfigComponent.Instance.GetAll<Skill_SpellswordConfig>();
                    foreach (Skill_SpellswordConfig skill_Info in skilMagicswordlist.Cast<Skill_SpellswordConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.Holymentor:
                    var skillHolyteacherList = ConfigComponent.Instance.GetAll<Skill_HolyteacherConfig>();
                    foreach (Skill_HolyteacherConfig skill_Info in skillHolyteacherList.Cast<Skill_HolyteacherConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,
                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    HolyteacherSkillCompat.AppendMissing(skillInfos);
                    break;
                case E_RoleType.Summoner:
                    var skilSummonerlist = ConfigComponent.Instance.GetAll<Skill_SummonWarlockConfig>();
                    foreach (Skill_SummonWarlockConfig skill_Info in skilSummonerlist.Cast<Skill_SummonWarlockConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.Gladiator:
                    var skilGladiatorlist = ConfigComponent.Instance.GetAll<Skill_CombatConfig>();
                    foreach (Skill_CombatConfig skill_Info in skilGladiatorlist.Cast<Skill_CombatConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.GrowLancer:
                    var skilGrowLancerlist = ConfigComponent.Instance.GetAll<Skill_DreamKnightConfig>();
                    foreach (Skill_DreamKnightConfig skill_Info in skilGrowLancerlist.Cast<Skill_DreamKnightConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.Runemage:
                    var skilRunemagelist = ConfigComponent.Instance.GetAll<Skill_MageRuneConfig>();
                    foreach (Skill_MageRuneConfig skill_Info in skilRunemagelist.Cast<Skill_MageRuneConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.StrongWind:
                    var skilStrongWindlist = ConfigComponent.Instance.GetAll<Skill_StrongWindConfig>();
                    foreach (Skill_StrongWindConfig skill_Info in skilStrongWindlist.Cast<Skill_StrongWindConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.Gunners:
                    var skilGunnerslist = ConfigComponent.Instance.GetAll<Skill_MusketeersConfig>();
                    foreach (Skill_MusketeersConfig skill_Info in skilGunnerslist.Cast<Skill_MusketeersConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.WhiteMagician:
                    var skilWhiteMagicianlist = ConfigComponent.Instance.GetAll<Skill_WhiteWizardConfig>();
                    foreach (Skill_WhiteWizardConfig skill_Info in skilWhiteMagicianlist.Cast<Skill_WhiteWizardConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                case E_RoleType.WomanMagician:
                    var skilWomanMagicianlist = ConfigComponent.Instance.GetAll<Skill_FemaleWizardConfig>();
                    foreach (Skill_FemaleWizardConfig skill_Info in skilWomanMagicianlist.Cast<Skill_FemaleWizardConfig>())
                    {
                        skillInfos.Add(new SkillInfos
                        {
                            Id = skill_Info.Id,
                            Name = skill_Info.Name,
                            Describe = skill_Info.Describe,
                            Icon = skill_Info.Icon,
                            SoundName = skill_Info.SoundName,
                            AnimatorTriggerIndex = skill_Info.AnimatorTriggerIndex,
                            AttackEffect = skill_Info.AttackEffect,
                            HitEffect = skill_Info.HitEffect,
                            skillType = skill_Info.skillType,
                            DamageWait = skill_Info.DamageWait,
                            Distance = skill_Info.Distance,
                            CoolTime = skill_Info.CoolTime,
                            PersistentTime = skill_Info.PersistentTime,
                            Consume = skill_Info.Consume,
                            OtherData = skill_Info.OtherData,
                            UseStandard = skill_Info.UseStandard,

                            Lev_CanUser = skill_Info.UseStandard.GetValue(1),//使用等级
                            Strength = skill_Info.UseStandard.GetValue(2),
                            Intell = skill_Info.UseStandard.GetValue(3),
                            Agile = skill_Info.UseStandard.GetValue(4),
                            Command = skill_Info.UseStandard.GetValue(5),
                            PhyStength = skill_Info.UseStandard.GetValue(6),
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 根据 Key 得到对应的值
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public static int GetValue(this string self, int key)
        {
            if(string.IsNullOrEmpty(self)) return 0;
            Regex reg = new Regex(@"[\{\}]");  //去掉{}
            string a1 = reg.Replace(self, "");
            var str = a1.Split(',');
            if (str.Length > 0)
            {
                for (int i = 0; i < str.Length; i++)
                {

                    var dicStrs = str[i].Split(':');
                    if (int.TryParse(dicStrs[0], out int k) && k == key && int.TryParse(dicStrs[1], out int result))
                    {
                        return result;
                    }

                }
            }
            return 0;
        }
    }
}
