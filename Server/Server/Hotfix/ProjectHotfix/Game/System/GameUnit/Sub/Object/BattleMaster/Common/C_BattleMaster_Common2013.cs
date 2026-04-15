
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
    /// 诱惑魅影
    /// </summary>
    [BattleMasterAttribute(BindId = (int)E_BattleMaster_Id.Common2013)]
    public partial class C_BattleMaster_Common2013 : C_BattleMaster
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

            if (!(Config is BattleMaster_ALLConfig mConfig))
            {
                return;
            }

            UpLevel = mConfig.OtherDataDic.Keys.ToList();
            Consume = mConfig.Consume;

            IsDataHasError = false;
        }
    }

    /// <summary>
    /// 诱惑魅影
    /// </summary>
    public partial class C_BattleMaster_Common2013
    {
        #region try
        public override bool TryUse(GamePlayer b_Attacker, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            if (!(Config is BattleMaster_ALLConfig mConfig))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(600);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能配置数据异常!");
                return false;
            }

            if (mConfig.UseRoleDic.ContainsKey(b_Attacker.Data.PlayerTypeId) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(604);
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

                    if (mReadConfigComponent.GetJson<BattleMaster_ALLConfigJson>().JsonDic.TryGetValue(mLastId, out var mTargetConfig) == false)
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

            return true;
        }
        #endregion

        public override bool UseSkill(GamePlayer b_Attacker, BattleComponent b_BattleComponent, bool b_AddPoint)
        {
            if (!(Config is BattleMaster_ALLConfig mConfig))
            {
                return false;
            }

            //BattleComponent.Log($"使用技能 <<{mConfig.Name}>> 使用者为:{b_Attacker.InstanceId}", false);

            // 诱惑魅影

            int mMasterLevel = Data.SkillId[this.Id];
            if (mMasterLevel == 0) return true;
            b_Attacker.BattleMasteryDic[E_BattleMasteryState.Common2013] = 1;

            var mStudySkillId = (int)E_HeroSkillId.YouHuoMeiYing527;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>();
            var mJsonDic = mReadConfigComponent.GetJson<Skill_SummonWarlockConfigJson>().JsonDic;
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
