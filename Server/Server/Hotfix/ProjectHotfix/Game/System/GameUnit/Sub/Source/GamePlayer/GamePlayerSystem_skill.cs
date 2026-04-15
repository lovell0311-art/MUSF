using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {

        public static void AwakeSkill(this GamePlayer b_Component)
        {
            if (b_Component.SkillGroup == null) b_Component.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
        }
        public static async Task DataUpdateSkill(this GamePlayer b_Component, bool notifyClient = true)
        {
            var mPlayer = b_Component.Player;
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
            if (mDataCache == null)
            {
                var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mDataCache = await HelpDb_DBGameSkillData.Init(mPlayer, mDataCacheManageComponent, dBProxy);
            }
            var mData = mDataCache.OnlyOne();
            if (mData == null)
            {
                mData = new DBGameSkillData()
                {
                    Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                    GameUserId = mPlayer.GameUserId,
                }; 
                var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                bool mSaveResult = await dBProxy.Save(mData);
                if (mSaveResult == false)
                {
                    return;
                }
                mDataCache.DataAdd(mData);
            }

            if (mData.SkillId == null) mData.DeSerialize();

            G2C_OpenSkillGroup_notice mOpenSkillGroup = new G2C_OpenSkillGroup_notice();
         
            var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
            switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
            {
                case E_GameOccupation.Spell:
                    {
                        if (!mData.SkillId.Contains(1))
                        {
                            // 法师初始技能，能量球
                            mData.SkillId.Add(1);
                        }
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SpellConfigJson>().JsonDic;
                        int mDoubleskillId = (int)E_HeroSkillId.LianJi27;
                        if (b_Component.Data.OccupationLevel >= 2)
                        {
                            if (mJsonDic.ContainsKey(mDoubleskillId) && mData.SkillId.Contains(mDoubleskillId) == false) mData.SkillId.Add(mDoubleskillId);
                        }
                      
                        for (int i = 0, len = mData.SkillId.Count; i < len; i++)
                        {
                            var mSkillId = mData.SkillId[i];

                            if (mSkillId == (int)E_HeroSkillId.DiLaoShu28) continue;//大师技能

                            if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                            {
                                if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);
                                    if (mSkillInstance == null) continue;
                                    b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }

                                mOpenSkillGroup.SkillIds.Add(mSkillId);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Swordsman:
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SwordsmanConfigJson>().JsonDic;
                        for (int i = 0, len = mData.SkillId.Count; i < len; i++)
                        {
                            var mSkillId = mData.SkillId[i];

                            if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                            {
                                if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);
                                    if (mSkillInstance == null) continue;
                                    b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }

                                mOpenSkillGroup.SkillIds.Add(mSkillId);
                            }
                        }

                    }
                    break;
                case E_GameOccupation.Archer:
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_ArcherConfigJson>().JsonDic;
                        int mDoubleskillId = (int)E_HeroSkillId.LianJi220;
                        if (b_Component.Data.OccupationLevel >= 2)
                        {
                            if (mJsonDic.ContainsKey(mDoubleskillId) && mData.SkillId.Contains(mDoubleskillId) == false) mData.SkillId.Add(mDoubleskillId);
                        }

                        for (int i = 0, len = mData.SkillId.Count; i < len; i++)
                        {
                            var mSkillId = mData.SkillId[i];

                            if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                            {
                                if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);
                                    if (mSkillInstance == null) continue;
                                    b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }

                                mOpenSkillGroup.SkillIds.Add(mSkillId);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Spellsword:
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SpellswordConfigJson>().JsonDic;
                        int mDoubleskillId = (int)E_HeroSkillId.LianJi329;
                        if (b_Component.Data.OccupationLevel >= 2)
                        {
                            if (mJsonDic.ContainsKey(mDoubleskillId) && mData.SkillId.Contains(mDoubleskillId) == false) mData.SkillId.Add(mDoubleskillId);
                        }
                       
                        for (int i = 0, len = mData.SkillId.Count; i < len; i++)
                        {
                            var mSkillId = mData.SkillId[i];

                            if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                            {
                                if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);
                                    if (mSkillInstance == null) continue;
                                    b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }

                                mOpenSkillGroup.SkillIds.Add(mSkillId);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Holyteacher:
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_HolyteacherConfigJson>().JsonDic;
                        int mDoubleskillId = (int)E_HeroSkillId.LianJi417;
                        if (b_Component.Data.OccupationLevel >= 2)
                        {
                            if (mJsonDic.ContainsKey(mDoubleskillId) && mData.SkillId.Contains(mDoubleskillId) == false) mData.SkillId.Add(mDoubleskillId);
                        }
                        for (int i = 0, len = mData.SkillId.Count; i < len; i++)
                        {
                            var mSkillId = mData.SkillId[i];

                            if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                            {
                                if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);
                                    if (mSkillInstance == null) continue;
                                    b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }

                                mOpenSkillGroup.SkillIds.Add(mSkillId);
                            }
                        }
                    }
                    break;
                case E_GameOccupation.SummonWarlock:
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SummonWarlockConfigJson>().JsonDic;
                        int mDoubleskillId = (int)E_HeroSkillId.LianJi525;
                        if (b_Component.Data.OccupationLevel >= 2)
                        {
                            if (mJsonDic.ContainsKey(mDoubleskillId) && mData.SkillId.Contains(mDoubleskillId) == false) mData.SkillId.Add(mDoubleskillId);
                        }
                 
                        for (int i = 0, len = mData.SkillId.Count; i < len; i++)
                        {
                            var mSkillId = mData.SkillId[i];

                            if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                            {
                                if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);
                                    if (mSkillInstance == null) continue;
                                    b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }

                            mOpenSkillGroup.SkillIds.Add(mSkillId);
                        }
                    }
                    break;
                case E_GameOccupation.Combat:
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_CombatConfigJson>().JsonDic;

                        for (int i = 0, len = mData.SkillId.Count; i < len; i++)
                        {
                            var mSkillId = mData.SkillId[i];

                            if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                            {
                                if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);
                                    if (mSkillInstance == null) continue;
                                    b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }

                            mOpenSkillGroup.SkillIds.Add(mSkillId);
                        }
                    }
                    break;
                case E_GameOccupation.GrowLancer:
                    {
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_DreamKnightConfigJson>().JsonDic;

                        for (int i = 0, len = mData.SkillId.Count; i < len; i++)
                        {
                            var mSkillId = mData.SkillId[i];

                            if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                            {
                                if (b_Component.SkillGroup.ContainsKey(mSkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mSkillId);
                                    if (mSkillInstance == null) continue;
                                    b_Component.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }

                            mOpenSkillGroup.SkillIds.Add(mSkillId);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (notifyClient)
            {
                mPlayer.Send(mOpenSkillGroup);
            }
        }

        public static void NotifyOpenSkillGroup(this GamePlayer b_Component)
        {
            if (b_Component?.Player == null)
            {
                return;
            }

            b_Component.EnsureMasterGrantedSkills(false);

            G2C_OpenSkillGroup_notice openSkillGroup = new G2C_OpenSkillGroup_notice();
            if (b_Component.SkillGroup != null)
            {
                foreach (int skillId in b_Component.SkillGroup.Keys.OrderBy(p => p))
                {
                    openSkillGroup.SkillIds.Add(skillId);
                }
            }

            b_Component.Player.Send(openSkillGroup);
        }
    }
}
