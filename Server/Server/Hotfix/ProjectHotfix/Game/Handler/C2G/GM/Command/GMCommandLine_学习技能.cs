
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [GameMasterCommandLine("学习技能")]
    public class GMCommandLine_学习技能 : C_GameMasterCommandLine<Player, IResponse>
    {
        public override async Task Run(Player b_Player, List<int> b_Parameter, IResponse b_Response)
        {
            //if (b_Parameter.Count < 0)
            //{
            //    b_Response.Error = 99;
            //    b_Response.Message = "参数不对";
            //    return;
            //}
            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();
            DBGamePlayerData mPlayerData = mGamePlayer.Data;

            DataCacheManageComponent mDataCacheManageComponent = b_Player.GetCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
            if (mDataCache == null)
            {
                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, b_Player.GameAreaId);
                mDataCache = await HelpDb_DBGameSkillData.Init(b_Player, mDataCacheManageComponent, dBProxy);
            }
            var mData = mDataCache.OnlyOne();
            if (mData == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "技能学习失败!3";
                return;
            }

            switch ((E_GameOccupation)mPlayerData.PlayerTypeId)
            {
                case E_GameOccupation.Spell:
                    {
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SpellConfigJson>().JsonDic;
                        var mJsonlist = mJsonDic.Values.ToArray();
                        for (int i = 0, len = mJsonlist.Length; i < len; i++)
                        {
                            var mSkillJson = mJsonlist[i];

                            if (mData.SkillId.Contains(mSkillJson.Id))
                            {
                                continue;
                            }

                            int mStudySkillId = mSkillJson.Id;
                            mData.SkillId.Add(mSkillJson.Id);

                            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                            mSkillSingle.SkillId = mStudySkillId;
                            b_Player.Send(mSkillSingle);

                            if (mJsonDic.ContainsKey(mStudySkillId))
                            {
                                if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);
                                    if (mSkillInstance == null) continue;
                                    mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Swordsman:
                    {
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SwordsmanConfigJson>().JsonDic;
                        var mJsonlist = mJsonDic.Values.ToArray();
                        for (int i = 0, len = mJsonlist.Length; i < len; i++)
                        {
                            var mSkillJson = mJsonlist[i];

                            if (mData.SkillId.Contains(mSkillJson.Id))
                            {
                                continue;
                            }

                            int mStudySkillId = mSkillJson.Id;
                            mData.SkillId.Add(mSkillJson.Id);

                            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                            mSkillSingle.SkillId = mStudySkillId;
                            b_Player.Send(mSkillSingle);

                            if (mJsonDic.ContainsKey(mStudySkillId))
                            {
                                if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);
                                    if (mSkillInstance == null) continue;
                                    mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Archer:
                    {
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_ArcherConfigJson>().JsonDic;
                        var mJsonlist = mJsonDic.Values.ToArray();
                        for (int i = 0, len = mJsonlist.Length; i < len; i++)
                        {
                            var mSkillJson = mJsonlist[i];

                            if (mData.SkillId.Contains(mSkillJson.Id))
                            {
                                continue;
                            }

                            int mStudySkillId = mSkillJson.Id;
                            mData.SkillId.Add(mSkillJson.Id);

                            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                            mSkillSingle.SkillId = mStudySkillId;
                            b_Player.Send(mSkillSingle);

                            if (mJsonDic.ContainsKey(mStudySkillId))
                            {
                                if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                    if (mSkillInstance == null) continue;
                                    mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Spellsword:
                    {
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SpellswordConfigJson>().JsonDic;
                        var mJsonlist = mJsonDic.Values.ToArray();
                        for (int i = 0, len = mJsonlist.Length; i < len; i++)
                        {
                            var mSkillJson = mJsonlist[i];

                            if (mData.SkillId.Contains(mSkillJson.Id))
                            {
                                continue;
                            }

                            int mStudySkillId = mSkillJson.Id;
                            mData.SkillId.Add(mSkillJson.Id);

                            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                            mSkillSingle.SkillId = mStudySkillId;
                            b_Player.Send(mSkillSingle);

                            if (mJsonDic.ContainsKey(mStudySkillId))
                            {
                                if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);
                                    if (mSkillInstance == null) continue;
                                    mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Holyteacher:
                    {
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_HolyteacherConfigJson>().JsonDic;
                        var mJsonlist = mJsonDic.Values.ToArray();
                        for (int i = 0, len = mJsonlist.Length; i < len; i++)
                        {
                            var mSkillJson = mJsonlist[i];

                            if (mData.SkillId.Contains(mSkillJson.Id))
                            {
                                continue;
                            }

                            int mStudySkillId = mSkillJson.Id;
                            mData.SkillId.Add(mSkillJson.Id);

                            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                            mSkillSingle.SkillId = mStudySkillId;
                            b_Player.Send(mSkillSingle);

                            if (mJsonDic.ContainsKey(mStudySkillId))
                            {
                                if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);
                                    if (mSkillInstance == null) continue;
                                    mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }
                        }
                    }
                    break;
                case E_GameOccupation.SummonWarlock:
                    {
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SummonWarlockConfigJson>().JsonDic;
                        var mJsonlist = mJsonDic.Values.ToArray();
                        for (int i = 0, len = mJsonlist.Length; i < len; i++)
                        {
                            var mSkillJson = mJsonlist[i];

                            if (mData.SkillId.Contains(mSkillJson.Id))
                            {
                                continue;
                            }

                            int mStudySkillId = mSkillJson.Id;
                            mData.SkillId.Add(mSkillJson.Id);

                            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                            mSkillSingle.SkillId = mStudySkillId;
                            b_Player.Send(mSkillSingle);

                            if (mJsonDic.ContainsKey(mStudySkillId))
                            {
                                if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                    if(mSkillInstance== null) continue;
                                    mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Combat:
                    {
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_CombatConfigJson>().JsonDic;
                        var mJsonlist = mJsonDic.Values.ToArray();
                        for (int i = 0, len = mJsonlist.Length; i < len; i++)
                        {
                            var mSkillJson = mJsonlist[i];

                            if (mData.SkillId.Contains(mSkillJson.Id))
                            {
                                continue;
                            }

                            int mStudySkillId = mSkillJson.Id;
                            mData.SkillId.Add(mSkillJson.Id);

                            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                            mSkillSingle.SkillId = mStudySkillId;
                            b_Player.Send(mSkillSingle);

                            if (mJsonDic.ContainsKey(mStudySkillId))
                            {
                                if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                    if (mSkillInstance == null) continue;
                                    mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }
                        }
                    }
                    break;
                case E_GameOccupation.GrowLancer:
                    {
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_DreamKnightConfigJson>().JsonDic;
                        var mJsonlist = mJsonDic.Values.ToArray();
                        for (int i = 0, len = mJsonlist.Length; i < len; i++)
                        {
                            var mSkillJson = mJsonlist[i];

                            if (mData.SkillId.Contains(mSkillJson.Id))
                            {
                                continue;
                            }

                            int mStudySkillId = mSkillJson.Id;
                            mData.SkillId.Add(mSkillJson.Id);

                            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                            mSkillSingle.SkillId = mStudySkillId;
                            b_Player.Send(mSkillSingle);

                            if (mJsonDic.ContainsKey(mStudySkillId))
                            {
                                if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                    if (mSkillInstance == null) continue;
                                    mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            {
                mData.Serialize();

                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManagerComponent.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
                mWriteDataComponent.Save(mData, dBProxy2).Coroutine();
            }
        }
    }
}