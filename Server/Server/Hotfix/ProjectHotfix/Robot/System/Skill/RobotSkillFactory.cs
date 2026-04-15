using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix.Robot
{
    public static class RobotSkillFactory
    {
        public static RobotSkill Create(Scene clientScene,int skillId)
        {
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            RobotSkill skill = null;
            switch ((E_GameOccupation)((skillId / 100)+1))
            {
                case E_GameOccupation.Spell:
                    {
                        var jsonDic = readConfig.GetJson<Skill_SpellConfigJson>().JsonDic;
                        if(jsonDic.TryGetValue(skillId,out var config))
                        {
                            skill = ComponentFactory.Create<RobotSkill>();
                            skill.ConfigId = skillId;
                            skill.Name = config.Name;
                            skill.SkillType = (RobotSkillType)config.skillType;
                            skill.Distance = config.Distance;
                            skill.CoolTime = config.CoolTime;
                            foreach (var kv in config.ConsumeDic)
                            {
                                skill.Consume.Add((RobotSkillConsume)kv.Key, kv.Value);
                            }
                            foreach (var kv in config.UseStandardDic)
                            {
                                skill.UseStandard.Add((RobotSkillUseStandard)kv.Key, kv.Value);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Swordsman:
                    {
                        var jsonDic = readConfig.GetJson<Skill_SwordsmanConfigJson>().JsonDic;
                        if (jsonDic.TryGetValue(skillId, out var config))
                        {
                            skill = ComponentFactory.Create<RobotSkill>();
                            skill.ConfigId = skillId;
                            skill.Name = config.Name;
                            skill.SkillType = (RobotSkillType)config.skillType;
                            skill.Distance = config.Distance;
                            skill.CoolTime = config.CoolTime;
                            foreach (var kv in config.ConsumeDic)
                            {
                                skill.Consume.Add((RobotSkillConsume)kv.Key, kv.Value);
                            }
                            foreach (var kv in config.UseStandardDic)
                            {
                                skill.UseStandard.Add((RobotSkillUseStandard)kv.Key, kv.Value);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Archer:
                    {
                        var jsonDic = readConfig.GetJson<Skill_ArcherConfigJson>().JsonDic;
                        if (jsonDic.TryGetValue(skillId, out var config))
                        {
                            skill = ComponentFactory.Create<RobotSkill>();
                            skill.ConfigId = skillId;
                            skill.Name = config.Name;
                            skill.SkillType = (RobotSkillType)config.skillType;
                            skill.Distance = config.Distance;
                            skill.CoolTime = config.CoolTime;
                            foreach (var kv in config.ConsumeDic)
                            {
                                skill.Consume.Add((RobotSkillConsume)kv.Key, kv.Value);
                            }
                            foreach (var kv in config.UseStandardDic)
                            {
                                skill.UseStandard.Add((RobotSkillUseStandard)kv.Key, kv.Value);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Spellsword:
                    {
                        var jsonDic = readConfig.GetJson<Skill_SpellswordConfigJson>().JsonDic;
                        if (jsonDic.TryGetValue(skillId, out var config))
                        {
                            skill = ComponentFactory.Create<RobotSkill>();
                            skill.ConfigId = skillId;
                            skill.Name = config.Name;
                            skill.SkillType = (RobotSkillType)config.skillType;
                            skill.Distance = config.Distance;
                            skill.CoolTime = config.CoolTime;
                            foreach (var kv in config.ConsumeDic)
                            {
                                skill.Consume.Add((RobotSkillConsume)kv.Key, kv.Value);
                            }
                            foreach (var kv in config.UseStandardDic)
                            {
                                skill.UseStandard.Add((RobotSkillUseStandard)kv.Key, kv.Value);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Holyteacher:
                    {
                        var jsonDic = readConfig.GetJson<Skill_HolyteacherConfigJson>().JsonDic;
                        if (jsonDic.TryGetValue(skillId, out var config))
                        {
                            skill = ComponentFactory.Create<RobotSkill>();
                            skill.ConfigId = skillId;
                            skill.Name = config.Name;
                            skill.SkillType = (RobotSkillType)config.skillType;
                            skill.Distance = config.Distance;
                            skill.CoolTime = config.CoolTime;
                            foreach (var kv in config.ConsumeDic)
                            {
                                skill.Consume.Add((RobotSkillConsume)kv.Key, kv.Value);
                            }
                            foreach (var kv in config.UseStandardDic)
                            {
                                skill.UseStandard.Add((RobotSkillUseStandard)kv.Key, kv.Value);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.SummonWarlock:
                    {
                        var jsonDic = readConfig.GetJson<Skill_SummonWarlockConfigJson>().JsonDic;
                        if (jsonDic.TryGetValue(skillId, out var config))
                        {
                            skill = ComponentFactory.Create<RobotSkill>();
                            skill.ConfigId = skillId;
                            skill.Name = config.Name;
                            skill.SkillType = (RobotSkillType)config.skillType;
                            skill.Distance = config.Distance;
                            skill.CoolTime = config.CoolTime;
                            foreach (var kv in config.ConsumeDic)
                            {
                                skill.Consume.Add((RobotSkillConsume)kv.Key, kv.Value);
                            }
                            foreach (var kv in config.UseStandardDic)
                            {
                                skill.UseStandard.Add((RobotSkillUseStandard)kv.Key, kv.Value);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Combat:
                    {
                        var jsonDic = readConfig.GetJson<Skill_CombatConfigJson>().JsonDic;
                        if (jsonDic.TryGetValue(skillId, out var config))
                        {
                            skill = ComponentFactory.Create<RobotSkill>();
                            skill.ConfigId = skillId;
                            skill.Name = config.Name;
                            skill.SkillType = (RobotSkillType)config.skillType;
                            skill.Distance = config.Distance;
                            skill.CoolTime = config.CoolTime;
                            foreach (var kv in config.ConsumeDic)
                            {
                                skill.Consume.Add((RobotSkillConsume)kv.Key, kv.Value);
                            }
                            foreach (var kv in config.UseStandardDic)
                            {
                                skill.UseStandard.Add((RobotSkillUseStandard)kv.Key, kv.Value);
                            }
                        }
                    }
                    break;
            }
            if(skill != null)
            {
                skill.AddComponent<ClientSceneComponent, Scene>(clientScene);
            }
            return skill;
        }
    }
}
