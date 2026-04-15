using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 大师 配置表 拓展
    /// </summary>
    public static class BattleMasterConfigExtend
    {
        public static void GetBattleMasterInfos_RoleType_Ref(this int self, E_RoleType roleType, ref MasterPoint battleMasterInfos)
        {
            switch (roleType)
            {
                case E_RoleType.Magician:
                    BattleMaster_SpellConfig battleMaster_Spell = ConfigComponent.Instance.GetItem<BattleMaster_SpellConfig>((int)self);
                    if (battleMaster_Spell == null) return;
                    battleMasterInfos.Id = battleMaster_Spell.Id;
                    battleMasterInfos.Name = battleMaster_Spell.Name;
                    battleMasterInfos.Describe = battleMaster_Spell.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Spell.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Spell.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Spell.LastIds;
                    battleMasterInfos.Consume = battleMaster_Spell.Consume;
                    battleMasterInfos.OtherData = battleMaster_Spell.OtherData;
                    battleMasterInfos.percenttage = battleMaster_Spell.OtherDataUse;
                    break;
                case E_RoleType.Swordsman:
                    BattleMaster_SwordsmanConfig battleMaster_Swordsman = ConfigComponent.Instance.GetItem<BattleMaster_SwordsmanConfig>((int)self);
                    if (battleMaster_Swordsman == null) return;
                    battleMasterInfos.Id = battleMaster_Swordsman.Id;
                    battleMasterInfos.Name = battleMaster_Swordsman.Name;
                    battleMasterInfos.Describe = battleMaster_Swordsman.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Swordsman.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Swordsman.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Swordsman.LastIds;
                    battleMasterInfos.Consume = battleMaster_Swordsman.Consume;
                    battleMasterInfos.OtherData = battleMaster_Swordsman.OtherData;
                    battleMasterInfos.percenttage = battleMaster_Swordsman.OtherDataUse;
                    break;
                case E_RoleType.Archer:
                    BattleMaster_ArcherConfig battleMaster_Archer = ConfigComponent.Instance.GetItem<BattleMaster_ArcherConfig>((int)self);
                    if (battleMaster_Archer == null) return;
                    battleMasterInfos.Id = battleMaster_Archer.Id;
                    battleMasterInfos.Name = battleMaster_Archer.Name;
                    battleMasterInfos.Describe = battleMaster_Archer.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Archer.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Archer.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Archer.LastIds;
                    battleMasterInfos.Consume = battleMaster_Archer.Consume;
                    battleMasterInfos.OtherData = battleMaster_Archer.OtherData;
                    battleMasterInfos.percenttage = battleMaster_Archer.OtherDataUse;
                    break;
                case E_RoleType.Magicswordsman:
                    BattleMaster_SpellswordConfig battleMaster_Spellsword = ConfigComponent.Instance.GetItem<BattleMaster_SpellswordConfig>((int)self);
                    if (battleMaster_Spellsword == null) return;
                    battleMasterInfos.Id = battleMaster_Spellsword.Id;
                    battleMasterInfos.Name = battleMaster_Spellsword.Name;
                    battleMasterInfos.Describe = battleMaster_Spellsword.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Spellsword.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Spellsword.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Spellsword.LastIds;
                    battleMasterInfos.Consume = battleMaster_Spellsword.Consume;
                    battleMasterInfos.OtherData = battleMaster_Spellsword.OtherData;
                    battleMasterInfos.percenttage = battleMaster_Spellsword.OtherDataUse;
                    break;
                case E_RoleType.Holymentor:
                    BattleMaster_HolyteacherConfig battleMaster_Holyteacher = ConfigComponent.Instance.GetItem<BattleMaster_HolyteacherConfig>((int)self);
                    if (battleMaster_Holyteacher == null) return;
                    battleMasterInfos.Id = battleMaster_Holyteacher.Id;
                    battleMasterInfos.Name = battleMaster_Holyteacher.Name;
                    battleMasterInfos.Describe = battleMaster_Holyteacher.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Holyteacher.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Holyteacher.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Holyteacher.LastIds;
                    battleMasterInfos.Consume = battleMaster_Holyteacher.Consume;
                    battleMasterInfos.OtherData = battleMaster_Holyteacher.OtherData;
                    battleMasterInfos.percenttage = battleMaster_Holyteacher.OtherDataUse;
                    break;
                case E_RoleType.Summoner:
                    BattleMaster_SummonWarlockConfig battleMaster_SummonWarlock = ConfigComponent.Instance.GetItem<BattleMaster_SummonWarlockConfig>((int)self);
                    if (battleMaster_SummonWarlock == null) return;
                    battleMasterInfos.Id = battleMaster_SummonWarlock.Id;
                    battleMasterInfos.Name = battleMaster_SummonWarlock.Name;
                    battleMasterInfos.Describe = battleMaster_SummonWarlock.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_SummonWarlock.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_SummonWarlock.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_SummonWarlock.LastIds;
                    battleMasterInfos.Consume = battleMaster_SummonWarlock.Consume;
                    battleMasterInfos.OtherData = battleMaster_SummonWarlock.OtherData;
                    battleMasterInfos.percenttage = battleMaster_SummonWarlock.OtherDataUse;
                    break;
                case E_RoleType.Gladiator:
                    BattleMaster_CombatConfig battleMaster_Combat = ConfigComponent.Instance.GetItem<BattleMaster_CombatConfig>((int)self);
                    if (battleMaster_Combat == null) return;
                    battleMasterInfos.Id = battleMaster_Combat.Id;
                    battleMasterInfos.Name = battleMaster_Combat.Name;
                    battleMasterInfos.Describe = battleMaster_Combat.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Combat.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Combat.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Combat.LastIds;
                    battleMasterInfos.Consume = battleMaster_Combat.Consume;
                    battleMasterInfos.OtherData = battleMaster_Combat.OtherData;
                    battleMasterInfos.percenttage = battleMaster_Combat.OtherDataUse;
                    break;
                case E_RoleType.GrowLancer:
                    BattleMaster_DreamKnightConfig battleMaster_DreamKnight = ConfigComponent.Instance.GetItem<BattleMaster_DreamKnightConfig>((int)self);
                    if (battleMaster_DreamKnight == null) return;
                    battleMasterInfos.Id = battleMaster_DreamKnight.Id;
                    battleMasterInfos.Name = battleMaster_DreamKnight.Name;
                    battleMasterInfos.Describe = battleMaster_DreamKnight.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_DreamKnight.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_DreamKnight.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_DreamKnight.LastIds;
                    battleMasterInfos.Consume = battleMaster_DreamKnight.Consume;
                    battleMasterInfos.OtherData = battleMaster_DreamKnight.OtherData;
                    battleMasterInfos.percenttage = battleMaster_DreamKnight.OtherDataUse;
                    break;
                case E_RoleType.Runemage:
                    BattleMaster_MageRuneConfig battleMaster_MageRune = ConfigComponent.Instance.GetItem<BattleMaster_MageRuneConfig>((int)self);
                    if (battleMaster_MageRune == null) return;
                    battleMasterInfos.Id = battleMaster_MageRune.Id;
                    battleMasterInfos.Name = battleMaster_MageRune.Name;
                    battleMasterInfos.Describe = battleMaster_MageRune.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_MageRune.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_MageRune.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_MageRune.LastIds;
                    battleMasterInfos.Consume = battleMaster_MageRune.Consume;
                    battleMasterInfos.OtherData = battleMaster_MageRune.OtherData;
                    battleMasterInfos.percenttage = battleMaster_MageRune.OtherDataUse;
                    break;
                case E_RoleType.StrongWind:
                    BattleMaster_StrongWindConfig battleMaster_StrongWind = ConfigComponent.Instance.GetItem<BattleMaster_StrongWindConfig>((int)self);
                    if (battleMaster_StrongWind == null) return;
                    battleMasterInfos.Id = battleMaster_StrongWind.Id;
                    battleMasterInfos.Name = battleMaster_StrongWind.Name;
                    battleMasterInfos.Describe = battleMaster_StrongWind.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_StrongWind.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_StrongWind.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_StrongWind.LastIds;
                    battleMasterInfos.Consume = battleMaster_StrongWind.Consume;
                    battleMasterInfos.OtherData = battleMaster_StrongWind.OtherData;
                    battleMasterInfos.percenttage = battleMaster_StrongWind.OtherDataUse;
                    break;
                case E_RoleType.Gunners:
                    BattleMaster_MusketeersConfig battleMaster_Musketeers = ConfigComponent.Instance.GetItem<BattleMaster_MusketeersConfig>((int)self);
                    if (battleMaster_Musketeers == null) return;
                    battleMasterInfos.Id = battleMaster_Musketeers.Id;
                    battleMasterInfos.Name = battleMaster_Musketeers.Name;
                    battleMasterInfos.Describe = battleMaster_Musketeers.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Musketeers.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Musketeers.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Musketeers.LastIds;
                    battleMasterInfos.Consume = battleMaster_Musketeers.Consume;
                    battleMasterInfos.OtherData = battleMaster_Musketeers.OtherData;
                    battleMasterInfos.percenttage = battleMaster_Musketeers.OtherDataUse;
                    break;
                case E_RoleType.WhiteMagician:
                    break;
                case E_RoleType.WomanMagician:
                    break;
                default:
                    break;
            }
        }

        public static void GetBattleMasterInfos_RoleType_Out(this int self, E_RoleType roleType, out MasterPoint battleMasterInfos)
        {
            battleMasterInfos = new MasterPoint();
            switch (roleType)
            {
                case E_RoleType.Magician:
                    BattleMaster_SpellConfig battleMaster_Spell = ConfigComponent.Instance.GetItem<BattleMaster_SpellConfig>((int)self);
                    if (battleMaster_Spell == null) return;
                    battleMasterInfos.Id = battleMaster_Spell.Id;
                    battleMasterInfos.Name = battleMaster_Spell.Name;
                    battleMasterInfos.Describe = battleMaster_Spell.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Spell.LayerLevel;
                    battleMasterInfos.percenttage = battleMaster_Spell.OtherDataUse;
                    battleMasterInfos.FrontIds = battleMaster_Spell.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Spell.LastIds;
                    battleMasterInfos.Consume = battleMaster_Spell.Consume;
                    battleMasterInfos.OtherData = battleMaster_Spell.OtherData;
                    break;
                case E_RoleType.Swordsman:
                    BattleMaster_SwordsmanConfig battleMaster_Swordsman = ConfigComponent.Instance.GetItem<BattleMaster_SwordsmanConfig>((int)self);
                    if (battleMaster_Swordsman == null) return;
                    battleMasterInfos.Id = battleMaster_Swordsman.Id;
                    battleMasterInfos.Name = battleMaster_Swordsman.Name;
                    battleMasterInfos.Describe = battleMaster_Swordsman.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Swordsman.LayerLevel;
                    battleMasterInfos.percenttage = battleMaster_Swordsman.OtherDataUse;
                    battleMasterInfos.FrontIds = battleMaster_Swordsman.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Swordsman.LastIds;
                    battleMasterInfos.Consume = battleMaster_Swordsman.Consume;
                    battleMasterInfos.OtherData = battleMaster_Swordsman.OtherData;
                    break;
                case E_RoleType.Archer:
                    BattleMaster_ArcherConfig battleMaster_Archer = ConfigComponent.Instance.GetItem<BattleMaster_ArcherConfig>((int)self);
                    if (battleMaster_Archer == null) return;
                    battleMasterInfos.Id = battleMaster_Archer.Id;
                    battleMasterInfos.Name = battleMaster_Archer.Name;
                    battleMasterInfos.Describe = battleMaster_Archer.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Archer.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Archer.FrontIds;
                    battleMasterInfos.percenttage = battleMaster_Archer.OtherDataUse;
                    battleMasterInfos.LastIds = battleMaster_Archer.LastIds;
                    battleMasterInfos.Consume = battleMaster_Archer.Consume;
                    battleMasterInfos.OtherData = battleMaster_Archer.OtherData;
                    break;
                case E_RoleType.Magicswordsman:
                    BattleMaster_SpellswordConfig battleMaster_Spellsword = ConfigComponent.Instance.GetItem<BattleMaster_SpellswordConfig>((int)self);
                    if (battleMaster_Spellsword == null) return;
                    battleMasterInfos.Id = battleMaster_Spellsword.Id;
                    battleMasterInfos.Name = battleMaster_Spellsword.Name;
                    battleMasterInfos.Describe = battleMaster_Spellsword.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Spellsword.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Spellsword.FrontIds;
                    battleMasterInfos.percenttage = battleMaster_Spellsword.OtherDataUse;
                    battleMasterInfos.LastIds = battleMaster_Spellsword.LastIds;
                    battleMasterInfos.Consume = battleMaster_Spellsword.Consume;
                    battleMasterInfos.OtherData = battleMaster_Spellsword.OtherData;
                    break;
                case E_RoleType.Holymentor:
                    BattleMaster_HolyteacherConfig battleMaster_Holyteacher = ConfigComponent.Instance.GetItem<BattleMaster_HolyteacherConfig>((int)self);
                    if (battleMaster_Holyteacher == null) return;
                    battleMasterInfos.Id = battleMaster_Holyteacher.Id;
                    battleMasterInfos.Name = battleMaster_Holyteacher.Name;
                    battleMasterInfos.Describe = battleMaster_Holyteacher.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Holyteacher.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Holyteacher.FrontIds;
                    battleMasterInfos.percenttage = battleMaster_Holyteacher.OtherDataUse;
                    battleMasterInfos.LastIds = battleMaster_Holyteacher.LastIds;
                    battleMasterInfos.Consume = battleMaster_Holyteacher.Consume;
                    battleMasterInfos.OtherData = battleMaster_Holyteacher.OtherData;
                    break;
                case E_RoleType.Summoner:
                    BattleMaster_SummonWarlockConfig battleMaster_SummonWarlock = ConfigComponent.Instance.GetItem<BattleMaster_SummonWarlockConfig>((int)self);
                    if (battleMaster_SummonWarlock == null) return;
                    battleMasterInfos.Id = battleMaster_SummonWarlock.Id;
                    battleMasterInfos.Name = battleMaster_SummonWarlock.Name;
                    battleMasterInfos.Describe = battleMaster_SummonWarlock.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_SummonWarlock.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_SummonWarlock.FrontIds;
                    battleMasterInfos.percenttage = battleMaster_SummonWarlock.OtherDataUse;
                    battleMasterInfos.LastIds = battleMaster_SummonWarlock.LastIds;
                    battleMasterInfos.Consume = battleMaster_SummonWarlock.Consume;
                    battleMasterInfos.OtherData = battleMaster_SummonWarlock.OtherData;
                    break;
                case E_RoleType.Gladiator:
                    BattleMaster_CombatConfig battleMaster_Combat = ConfigComponent.Instance.GetItem<BattleMaster_CombatConfig>((int)self);
                    if (battleMaster_Combat == null) return;
                    battleMasterInfos.Id = battleMaster_Combat.Id;
                    battleMasterInfos.Name = battleMaster_Combat.Name;
                    battleMasterInfos.Describe = battleMaster_Combat.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Combat.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Combat.FrontIds;
                    battleMasterInfos.percenttage = battleMaster_Combat.OtherDataUse;
                    battleMasterInfos.LastIds = battleMaster_Combat.LastIds;
                    battleMasterInfos.Consume = battleMaster_Combat.Consume;
                    battleMasterInfos.OtherData = battleMaster_Combat.OtherData;
                    break;
                case E_RoleType.GrowLancer:
                    BattleMaster_DreamKnightConfig battleMaster_DreamKnight = ConfigComponent.Instance.GetItem<BattleMaster_DreamKnightConfig>((int)self);
                    if (battleMaster_DreamKnight == null) return;
                    battleMasterInfos.Id = battleMaster_DreamKnight.Id;
                    battleMasterInfos.Name = battleMaster_DreamKnight.Name;
                    battleMasterInfos.Describe = battleMaster_DreamKnight.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_DreamKnight.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_DreamKnight.FrontIds;
                    battleMasterInfos.percenttage = battleMaster_DreamKnight.OtherDataUse;
                    battleMasterInfos.LastIds = battleMaster_DreamKnight.LastIds;
                    battleMasterInfos.Consume = battleMaster_DreamKnight.Consume;
                    battleMasterInfos.OtherData = battleMaster_DreamKnight.OtherData;
                    break;
                case E_RoleType.Runemage:
                    BattleMaster_MageRuneConfig battleMaster_MageRune = ConfigComponent.Instance.GetItem<BattleMaster_MageRuneConfig>((int)self);
                    if (battleMaster_MageRune == null) return;
                    battleMasterInfos.Id = battleMaster_MageRune.Id;
                    battleMasterInfos.Name = battleMaster_MageRune.Name;
                    battleMasterInfos.Describe = battleMaster_MageRune.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_MageRune.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_MageRune.FrontIds;
                    battleMasterInfos.percenttage = battleMaster_MageRune.OtherDataUse;
                    battleMasterInfos.LastIds = battleMaster_MageRune.LastIds;
                    battleMasterInfos.Consume = battleMaster_MageRune.Consume;
                    battleMasterInfos.OtherData = battleMaster_MageRune.OtherData;
                    break;
                case E_RoleType.StrongWind:
                    BattleMaster_StrongWindConfig battleMaster_StrongWind = ConfigComponent.Instance.GetItem<BattleMaster_StrongWindConfig>((int)self);
                    if (battleMaster_StrongWind == null) return;
                    battleMasterInfos.Id = battleMaster_StrongWind.Id;
                    battleMasterInfos.Name = battleMaster_StrongWind.Name;
                    battleMasterInfos.Describe = battleMaster_StrongWind.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_StrongWind.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_StrongWind.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_StrongWind.LastIds;
                    battleMasterInfos.percenttage = battleMaster_StrongWind.OtherDataUse;
                    battleMasterInfos.Consume = battleMaster_StrongWind.Consume;
                    battleMasterInfos.OtherData = battleMaster_StrongWind.OtherData;
                    break;
                case E_RoleType.Gunners:
                    BattleMaster_MusketeersConfig battleMaster_Musketeers = ConfigComponent.Instance.GetItem<BattleMaster_MusketeersConfig>((int)self);
                    if (battleMaster_Musketeers == null) return;
                    battleMasterInfos.Id = battleMaster_Musketeers.Id;
                    battleMasterInfos.Name = battleMaster_Musketeers.Name;
                    battleMasterInfos.Describe = battleMaster_Musketeers.Describe;
                    battleMasterInfos.LayerLevel = battleMaster_Musketeers.LayerLevel;
                    battleMasterInfos.FrontIds = battleMaster_Musketeers.FrontIds;
                    battleMasterInfos.LastIds = battleMaster_Musketeers.LastIds;
                    battleMasterInfos.percenttage = battleMaster_Musketeers.OtherDataUse;
                    battleMasterInfos.Consume = battleMaster_Musketeers.Consume;
                    battleMasterInfos.OtherData = battleMaster_Musketeers.OtherData;
                    break;
                case E_RoleType.WhiteMagician:
                    break;
                case E_RoleType.WomanMagician:
                    break;
                default:
                    break;
            }
        }

        public static void GetBattleMasterALL_RoleType_Ref(this int self, E_RoleType roleType, ref MasterPoint battleMasterInfos)
        {
            BattleMaster_ALLConfig battleMaster_Spell = ConfigComponent.Instance.GetItem<BattleMaster_ALLConfig>((int)self);
            if (battleMaster_Spell == null) return;
            battleMasterInfos.Id = battleMaster_Spell.Id;
            battleMasterInfos.Name = battleMaster_Spell.Name;
            battleMasterInfos.Describe = battleMaster_Spell.Describe;
            battleMasterInfos.LayerLevel = battleMaster_Spell.LayerLevel;
            battleMasterInfos.FrontIds = battleMaster_Spell.FrontIds;
            battleMasterInfos.percenttage = battleMaster_Spell.OtherDataUse;
            battleMasterInfos.Consume = battleMaster_Spell.Consume;
            battleMasterInfos.OtherData = battleMaster_Spell.OtherData;
            battleMasterInfos.Unlock = battleMaster_Spell.Unlock;
        }

        public static string GetSkillType(this int configID, E_RoleType roleType)
        {
            string skillType = string.Empty;
            if (configID > 2000)
            {
                skillType = "超神大师";
                return skillType;
            }
            switch (roleType)
            {
                case E_RoleType.Magician:
                    if (configID < 24)
                    {
                        skillType = "平稳";
                    }
                    else if (configID < 46)
                    {
                        skillType = "智慧";
                    }
                    else if (configID < 72)
                    {
                        skillType = "超越";
                    }
                    break;
                case E_RoleType.Swordsman:
                    if (configID < 122)
                    {
                        skillType = "保护";
                    }
                    else if (configID < 143)
                    {
                        skillType = "勇猛";
                    }
                    else if (configID < 171)
                    {
                        skillType = "愤怒";
                    }
                    break;
                case E_RoleType.Archer:
                    if (configID < 224)
                    {
                        skillType = "加护";
                    }
                    else if (configID < 253)
                    {
                        skillType = "救援";
                    }
                    else if (configID < 277)
                    {
                        skillType = "疾风";
                    }
                    break;
                case E_RoleType.Magicswordsman:
                    if (configID < 324)
                    {
                        skillType = "坚固";
                    }
                    else if (configID < 351)
                    {
                        skillType = "斗志";
                    }
                    else if (configID < 377)
                    {
                        skillType = "终结";
                    }
                    break; 
                case E_RoleType.Holymentor:
                    if (configID < 424)
                    {
                        skillType = "坚决";
                    }
                    else if (configID < 442)
                    {
                        skillType = "正义";
                    }
                    else if (configID < 471)
                    {
                        skillType = "征服";
                    }
                    break;
                case E_RoleType.Summoner:
                    if (configID < 521)
                    {
                        skillType = "守护";
                    }
                    else if (configID < 541)
                    {
                        skillType = "混沌";
                    }
                    else if (configID < 566)
                    {
                        skillType = "荣誉";
                    }
                    break;
                case E_RoleType.Gladiator:
                    if (configID < 621)
                    {
                        skillType = "根性";
                    }
                    else if (configID < 640)
                    {
                        skillType = "意志";
                    }
                    else if (configID < 661)
                    {
                        skillType = "破坏";
                    }
                    break;
                case E_RoleType.Gunners:
                    if (configID < 721)
                    {
                        skillType = "意识";
                    }
                    else if (configID < 738)
                    {
                        skillType = "流血";
                    }
                    else if (configID < 756)
                    {
                        skillType = "组织";
                    }
                    break;
                case E_RoleType.StrongWind:
                    if (configID < 822)
                    {
                        skillType = "惩处";
                    }
                    else if (configID < 839)
                    {
                        skillType = "严酷";
                    }
                    else if (configID < 858)
                    {
                        skillType = "无情";
                    }
                    break;
                case E_RoleType.GrowLancer:
                    if (configID < 923)
                    {
                        skillType = "神圣";
                    }
                    else if (configID < 942)
                    {
                        skillType = "惩处";
                    }
                    else if (configID < 769)
                    {
                        skillType = "信念";
                    }
                    break;
                case E_RoleType.Runemage:
                    if (configID < 1023)
                    {
                        skillType = "复仇";
                    }
                    else if (configID < 1044)
                    {
                        skillType = "残酷";
                    }
                    else if (configID < 1065)
                    {
                        skillType = "冷血";
                    }
                    break;
                case E_RoleType.WhiteMagician:
                    break;
                case E_RoleType.WomanMagician:
                    break;
                default:
                    break;
            }
            return skillType;
        }
    }

}
