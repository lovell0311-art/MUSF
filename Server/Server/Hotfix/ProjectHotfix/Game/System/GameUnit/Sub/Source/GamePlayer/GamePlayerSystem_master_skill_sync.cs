using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    public static partial class GamePlayerSystem
    {
        public static bool EnsureMasterGrantedSkills(this GamePlayer b_Component, bool notifyClient = true)
        {
            if (b_Component?.Player == null)
            {
                return false;
            }

            DataCacheManageComponent dataCacheManageComponent = b_Component.Player.GetCustomComponent<DataCacheManageComponent>();
            if (dataCacheManageComponent == null)
            {
                return false;
            }

            DBMasterData masterData = dataCacheManageComponent.Get<DBMasterData>()?.OnlyOne();
            DBGameSkillData skillData = dataCacheManageComponent.Get<DBGameSkillData>()?.OnlyOne();
            if (masterData == null || skillData == null)
            {
                return false;
            }

            if (masterData.SkillId == null)
            {
                masterData.DeSerialize();
            }

            if (skillData.SkillId == null || skillData.SkillUpInfo == null)
            {
                skillData.DeSerialize();
            }

            if (masterData.SkillId == null || skillData.SkillId == null)
            {
                return false;
            }

            bool changed = false;
            bool skillDataChanged = false;
            SkillCreateBuilder skillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();

            foreach (var skillInfo in masterData.SkillId)
            {
                if (skillInfo.Value <= 0)
                {
                    continue;
                }

                if (!TryGetMasterGrantedSkillInfo(b_Component, skillInfo.Key, out E_BattleMasteryState masteryState, out int studySkillId))
                {
                    continue;
                }

                b_Component.BattleMasteryDic[masteryState] = 1;

                if (!b_Component.SkillGroup.ContainsKey(studySkillId))
                {
                    C_HeroSkillSource skillInstance = skillCreateBuilder.CreateHeroSKill(studySkillId);
                    if (skillInstance == null)
                    {
                        Log.Warning($"#MasterSkillSync# create skill failed. player={b_Component.Player.GameUserId} masterId={skillInfo.Key} skillId={studySkillId}");
                        continue;
                    }

                    b_Component.SkillGroup[skillInstance.Id] = skillInstance;
                    changed = true;
                }

                if (skillData.SkillId.Contains(studySkillId))
                {
                    continue;
                }

                skillData.SkillId.Add(studySkillId);
                skillDataChanged = true;
                changed = true;

                if (notifyClient)
                {
                    b_Component.Player.Send(new G2C_StudySkillSingle_notice { SkillId = studySkillId });
                }

                Log.Info($"#MasterSkillSync# repaired granted skill. player={b_Component.Player.GameUserId} masterId={skillInfo.Key} skillId={studySkillId}");
            }

            if (skillDataChanged)
            {
                skillData.Serialize();

                DBProxyManagerComponent dbProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                DBMongodbProxySaveManageComponent writeManageComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>();
                if (dbProxyManager != null && writeManageComponent != null)
                {
                    DBProxyComponent dBProxy = dbProxyManager.GetZoneDB(DBType.Core, b_Component.Player.GameAreaId);
                    writeManageComponent.Get(b_Component.Player.GameAreaId)?.Save(skillData, dBProxy).Coroutine();
                }
            }

            return changed;
        }

        private static bool TryGetMasterGrantedSkillInfo(GamePlayer b_Component, int masterId, out E_BattleMasteryState masteryState, out int studySkillId)
        {
            masteryState = default;
            studySkillId = 0;

            switch ((E_BattleMaster_Id)masterId)
            {
                case E_BattleMaster_Id.Common2009:
                    masteryState = E_BattleMasteryState.Common2009;
                    switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                    {
                        case E_GameOccupation.Swordsman:
                            studySkillId = (int)E_HeroSkillId.JianYu126;
                            return true;
                        case E_GameOccupation.Spellsword:
                            studySkillId = (int)E_HeroSkillId.JianYu335;
                            return true;
                        case E_GameOccupation.Combat:
                            studySkillId = (int)E_HeroSkillId.JianYu618;
                            return true;
                        case E_GameOccupation.GrowLancer:
                            studySkillId = (int)E_HeroSkillId.JianYu714;
                            return true;
                    }
                    return false;
                case E_BattleMaster_Id.Common2010:
                    masteryState = E_BattleMasteryState.Common2010;
                    switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                    {
                        case E_GameOccupation.Spell:
                            studySkillId = (int)E_HeroSkillId.YunXingYu30;
                            return true;
                        case E_GameOccupation.Spellsword:
                            studySkillId = (int)E_HeroSkillId.YunXingYu336;
                            return true;
                    }
                    return false;
                case E_BattleMaster_Id.Common2011:
                    masteryState = E_BattleMasteryState.Common2011;
                    if ((E_GameOccupation)b_Component.Data.PlayerTypeId == E_GameOccupation.Archer)
                    {
                        studySkillId = (int)E_HeroSkillId.JianYu230;
                        return true;
                    }
                    return false;
                case E_BattleMaster_Id.Common2012:
                    masteryState = E_BattleMasteryState.Common2012;
                    if ((E_GameOccupation)b_Component.Data.PlayerTypeId == E_GameOccupation.Holyteacher)
                    {
                        studySkillId = (int)E_HeroSkillId.HeiSeTianKong419;
                        return true;
                    }
                    return false;
                case E_BattleMaster_Id.Common2013:
                    masteryState = E_BattleMasteryState.Common2013;
                    if ((E_GameOccupation)b_Component.Data.PlayerTypeId == E_GameOccupation.SummonWarlock)
                    {
                        studySkillId = (int)E_HeroSkillId.YouHuoMeiYing527;
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }
    }
}
