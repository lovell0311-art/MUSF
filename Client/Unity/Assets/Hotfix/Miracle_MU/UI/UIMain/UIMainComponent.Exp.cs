using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;


namespace ETHotfix
{
    /// <summary>
    /// 经验
    /// </summary>
    public partial class UIMainComponent
    {
        private Image expvalue;
        RoleConfig_ExperienceConfig experienceConfig;//经验配置表
        public Text expvaluetxt;
        public void Init_Exp() 
        {
            ReferenceCollector collector = ReferenceCollector_Main.GetImage("Exp").gameObject.GetReferenceCollector();
            expvalue = collector.GetImage("expvalue");
            expvaluetxt = collector.GetText("value");
         
            experienceConfig = ConfigComponent.Instance.GetItem<RoleConfig_ExperienceConfig>(roleEntity.Level);//根据玩家当前等级 得到当前等级升级到下一级 所需的最大经验值
            if (experienceConfig != null)
            {
                expvalue.fillAmount = (float)roleEntity.Property.GetProperValue(E_GameProperty.Exprience) / experienceConfig.Exprience;//当前所拥有的经验/当前升级所需的最大经验值 =》经验进度条
                expvaluetxt.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.Exprience)}/{experienceConfig.Exprience}";
            }
            else
            {
                expvalue.fillAmount = 1;
            }
        }
        /// <summary>
        /// 更新经验 进度
        /// </summary>
        public void UpdateExperience() 
        {
            
            experienceConfig = ConfigComponent.Instance.GetItem<RoleConfig_ExperienceConfig>(roleEntity.Level);//根据玩家当前等级 得到当前等级升级到下一级 所需的最大经验值

            if (experienceConfig != null)
            {
                expvalue.fillAmount = (float)roleEntity.Property.GetProperValue(E_GameProperty.Exprience) / experienceConfig.Exprience;//当前所拥有的经验/当前升级所需的最大经验值 =》经验进度条
                expvaluetxt.text = $"{roleEntity.Property.GetProperValue(E_GameProperty.Exprience)}/{experienceConfig.Exprience}";
            }
            else
            {
                expvalue.fillAmount = 1;
            }
            TimerComponent.Instance.RegisterTimeCallBack(100, cheakBeginner);
            SetTopUpHint(roleEntity.Level);
        }
        public void cheakBeginner()
        {
            if (roleEntity.Level == 2)
            {
                if (BeginnerGuideData.BeginnerGuideCountTime)
                {
                    if (BeginnerGuideData.IsCompleteTrigger(45,45))
                    {
                        BeginnerGuideData.SetBeginnerGuide(45);
                        SetBeginnerGuide(true);
                      
                    }
                }
            }
           
        }

        public void SetArributeRedDot( bool isShow)
        {
            RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Attribute, isShow ? 1 : 0);
            UIMainComponent.Instance.RedDotFriendCheack();
        }

    }
}