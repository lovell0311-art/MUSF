
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
    /// 敏捷属性增加
    /// </summary>
    [BattleMasterAttribute(BindId = (int)E_BattleMaster_Id.IncreaseAgility517)]
    public partial class C_BattleMaster_IncreaseAgility517 : C_BattleMaster
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

            if (!(Config is BattleMaster_SummonWarlockConfig mConfig))
            {
                return;
            }

            UpLevel = mConfig.OtherDataDic.Keys.ToList();
            Consume = mConfig.Consume;

            IsDataHasError = false;
        }
    }

    /// <summary>
    /// 敏捷属性增加
    /// </summary>
    public partial class C_BattleMaster_IncreaseAgility517
    {
        public override bool TryUse(GamePlayer b_Attacker, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            if (!(Config is BattleMaster_SummonWarlockConfig mConfig))
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

                    if (mReadConfigComponent.GetJson<BattleMaster_SummonWarlockConfigJson>().JsonDic.TryGetValue(mLastId, out var mTargetConfig) == false)
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
                        if (mReadConfigComponent.GetJson<BattleMaster_SummonWarlockConfigJson>().JsonDic.TryGetValue(mLastId, out var mTargetConfig) == false)
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
            if (!(Config is BattleMaster_SummonWarlockConfig mConfig))
            {
                return false;
            }

            //BattleComponent.Log($"使用技能 <<{mConfig.Name}>> 使用者为:{b_Attacker.InstanceId}", false);

            // 敏捷属性提高{0:F}。

            int mMasterLevel = Data.SkillId[this.Id];
            b_Attacker.BattleMasteryDic[E_BattleMasteryState.IncreaseAgility517] = base.GetMasterValue(mConfig.OtherDataDic[mMasterLevel], mConfig.OtherDataUse);


            return true;
        }
    }
}
