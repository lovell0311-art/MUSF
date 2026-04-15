
using System;
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
    /// <summary>
    /// 使用技能书学习技能
    /// </summary>
    [ItemUseRule(typeof(UseStudySkill))]
    public class UseStudySkill : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();
            if (!b_Item.CanUse(mGamePlayer))
            {
                b_Response.Error = 2302;
                return;
            }

            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            DataCacheManageComponent mDataCacheManageComponent = b_Player.GetCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
            if (mDataCache == null)
            {
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

            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (mReadConfigComponent.GetJson<Item_SkillBooksConfigJson>().JsonDic.TryGetValue(b_Item.ConfigID, out var mItemConfigTemp) == false)
            {
                //配置数据不存在
                b_Response.Error = 713;
                return;
            }

            if (mItemConfigTemp.ValueDic.TryGetValue(mGamePlayer.Data.PlayerTypeId, out var mStudySkillId) == false)
            {
                //技能书不能被当前职业使用
                b_Response.Error = 714;
                return;
            }

            switch ((E_GameOccupation)mGamePlayer.Data.PlayerTypeId)
            {
                case E_GameOccupation.Spell:
                    {
                        var mJsonDic = mReadConfigComponent.GetJson<Skill_SpellConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillConfig) == false)
                        {
                            //技能书对应职业技能不存在
                            b_Response.Error = 715;
                            return;
                        }
                    }
                    break;
                case E_GameOccupation.Swordsman:
                    {
                        var mJsonDic = mReadConfigComponent.GetJson<Skill_SwordsmanConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillConfig) == false)
                        {
                            //技能书对应职业技能不存在
                            b_Response.Error = 715;
                            return;
                        }
                    }
                    break;
                case E_GameOccupation.Archer:
                    {
                        var mJsonDic = mReadConfigComponent.GetJson<Skill_ArcherConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillConfig) == false)
                        {
                            //技能书对应职业技能不存在
                            b_Response.Error = 715;
                            return;
                        }
                    }
                    break;
                case E_GameOccupation.Spellsword:
                    {
                        var mJsonDic = mReadConfigComponent.GetJson<Skill_SpellswordConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillConfig) == false)
                        {
                            //技能书对应职业技能不存在
                            b_Response.Error = 715;
                            return;
                        }
                    }
                    break;
                case E_GameOccupation.Holyteacher:
                    {
                        var mJsonDic = mReadConfigComponent.GetJson<Skill_HolyteacherConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillConfig) == false)
                        {
                            //技能书对应职业技能不存在
                            b_Response.Error = 715;
                            return;
                        }
                    }
                    break;
                case E_GameOccupation.SummonWarlock:
                    {
                        var mJsonDic = mReadConfigComponent.GetJson<Skill_SummonWarlockConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillConfig) == false)
                        {
                            //技能书对应职业技能不存在
                            b_Response.Error = 715; 
                            return;
                        }
                    }
                    break;
                case E_GameOccupation.Combat:
                    break;
                case E_GameOccupation.GrowLancer:
                    break;
                default:
                    break;
            }

            if (mData.SkillId.Contains(mStudySkillId))
            {
                //技能已学习,技能书使用失败
                b_Response.Error = 716;
                return;
            }

            if (mGamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
            {
                var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                mGamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
            }
            mData.SkillId.Add(mStudySkillId);

            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
            mSkillSingle.SkillId = mStudySkillId;
            b_Player.Send(mSkillSingle);

            mData.Serialize();

            var dBProxy2 = mDBProxyManagerComponent.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
            mWriteDataComponent.Save(mData, dBProxy2).Coroutine();
        }
    }
}