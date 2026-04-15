
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomFrameWork;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 突袭技能
    /// </summary>
    [BattleMasterAttribute(BindId = (int)E_BattleMaster_Id.Skill_TuXi_138)]
    public partial class C_BattleMaster_Skill_TuXi_138 : C_BattleMaster
    {
        public override void AfterAwake()
        { // 只调用一次  
            IsDataHasError = false;
            DataUpdate();
        }
        public override void DataUpdate()
        {  //数据变化 更新变更数据 
            if (IsDataHasError) return;
            IsDataHasError = true;

            if (!(Config is BattleMaster_SwordsmanConfig mConfig))
            {
                return;
            }

            UpLevel = mConfig.OtherDataDic.Keys.ToList();
            Consume = mConfig.Consume;

            IsDataHasError = false;
        }
    }

    /// <summary>
    /// 突袭技能
    /// </summary>
    public partial class C_BattleMaster_Skill_TuXi_138
    {
        public override bool TryUse(GamePlayer b_Attacker, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            if (!(Config is BattleMaster_SwordsmanConfig mConfig))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(600);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能配置数据异常!");
                return false;
            }
            if (!ConstServer.PlayerMaster)
            {
                if (b_Attacker.Data.Level < 400)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(601);
                    return false;
                }
                if (b_Attacker.Data.OccupationLevel < 3)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(602);
                    return false;
                }
            }
            if (mConfig.FrontIds != null && mConfig.FrontIds.Count > 0)
            {
                var mReadConfigComponent = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>();
                for (int i = 0, len = mConfig.FrontIds.Count; i < len; i++)
                {
                    var mLastId = mConfig.FrontIds[i];

                    if (Data.SkillId.TryGetValue(mLastId, out var mLastLevelTemp) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(603);
                        return false;
                    }

                    if (mReadConfigComponent.GetJson<BattleMaster_SwordsmanConfigJson>().JsonDic.TryGetValue(mLastId, out var mTargetConfig) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(604);
                        return false;
                    }
                    if (mLastLevelTemp * mTargetConfig.Consume < 10)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(603);
                        return false;
                    }
                }
            }
            if (mConfig.LastIds != null && mConfig.LastIds.Count > 0)
            {
                var mReadConfigComponent = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>();
                int mLastLevel = 0;
                for (int i = 0, len = mConfig.LastIds.Count; i < len; i++)
                {
                    var mLastId = mConfig.LastIds[i];

                    if (Data.SkillId.TryGetValue(mLastId, out var mLastLevelTemp))
                    {
                        if (mReadConfigComponent.GetJson<BattleMaster_SwordsmanConfigJson>().JsonDic.TryGetValue(mLastId, out var mTargetConfig) == false)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(607);
                            return false;
                        }
                        mLastLevel += (mLastLevelTemp * mTargetConfig.Consume);
                    }
                }
                if (mLastLevel < 10)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(608);
                    return false;
                }
            }

            return true;
        }

        public override bool UseSkill(GamePlayer b_Attacker, BattleComponent b_BattleComponent, bool b_AddPoint)
        {
            if (!(Config is BattleMaster_SwordsmanConfig mConfig))
            {
                return false;
            }

            //BattleComponent.Log($"使用技能 <<{mConfig.Name}>> 使用者为:{b_Attacker.InstanceId}", false);

            // 快速扑向3格以内的慕白用强烈一击造成伤害，1秒冷却时间。

            int mMasterLevel = Data.SkillId[this.Id];
            b_Attacker.BattleMasteryDic[E_BattleMasteryState.Skill_TuXi_138] = 1;

            var mStudySkillId = (int)E_HeroSkillId.TuXi124;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>();
            var mJsonDic = mReadConfigComponent.GetJson<Skill_SwordsmanConfigJson>().JsonDic;
            if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillConfig) == false)
            {
                return false;
            }

            if (b_Attacker.SkillGroup.ContainsKey(mStudySkillId) == false)
            {
                var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                b_Attacker.SkillGroup[mSkillInstance.Id] = mSkillInstance;
            }
            DataCacheManageComponent mDataCacheManageComponent = b_Attacker.Player.GetCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
            if (mDataCache == null)
            {
                return false;
            }
            var mData = mDataCache.OnlyOne();
            if (mData == null)
            {
                return false;
            }
            if (mData.SkillId.Contains(mStudySkillId) == false) mData.SkillId.Add(mStudySkillId);
            mData.Serialize();

            G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
            mSkillSingle.SkillId = mStudySkillId;
            b_Attacker.Player.Send(mSkillSingle);

            return true;
        }
    }
}
