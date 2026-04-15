using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 学习的技能 通知
    /// </summary>
    [MessageHandler]
    public class G2C_StudySkillSingle_notice_Handler : AMHandler<G2C_StudySkillSingle_notice>
    {
        protected override void Run(ETModel.Session session, G2C_StudySkillSingle_notice message)
        {
            RoleEntity localRole = UnitEntityComponent.Instance?.LocalRole;
            if (localRole == null)
            {
                return;
            }

            if (!localRole.OwnSkills.Contains(message.SkillId))
            {
                localRole.OwnSkills.Add(message.SkillId);
            }

            TryAutoEquipLearnedActiveSkill(localRole, message.SkillId);

            TimerComponent.Instance.RegisterTimeCallBack(100, CheakBeginner);
            void CheakBeginner()
            {
                if (BeginnerGuideData.BeginnerGuideCountTime)
                {
                    if (BeginnerGuideData.IsCompleteTrigger(53, 53))
                    {
                        BeginnerGuideData.SetBeginnerGuide(53);
                        UIMainComponent.Instance.SetBeginnerGuide(true);
                    }
                }
            }
        }

        private static void TryAutoEquipLearnedActiveSkill(RoleEntity localRole, int skillId)
        {
            skillId.GetSkillInfos_RoleType_Out(localRole.RoleType, out SkillInfos skillInfo);
            if (skillInfo == null || skillInfo.skillType != 1)
            {
                RefreshSkillViews();
                return;
            }

            Skillconfiguration skillconfiguration =
                LocalDataJsonComponent.Instance.LoadData<Skillconfiguration>(localRole.LocalSkillFillName) ??
                new Skillconfiguration();

            foreach (KeyValuePair<string, int> entry in skillconfiguration.SKilDic)
            {
                if (entry.Value == skillId)
                {
                    RefreshSkillViews();
                    return;
                }
            }

            for (int slotIndex = 0; slotIndex < 4; ++slotIndex)
            {
                string slotKey = slotIndex.ToString();
                if (skillconfiguration.SKilDic.ContainsKey(slotKey))
                {
                    continue;
                }

                skillconfiguration.SKilDic[slotKey] = skillId;
                LocalDataJsonComponent.Instance.SavaData(skillconfiguration, localRole.LocalSkillFillName);
                RefreshSkillViews();
                return;
            }

            RefreshSkillViews();
        }

        private static void RefreshSkillViews()
        {
            if (UIMainComponent.Instance != null)
            {
                UIMainComponent.Instance.LoadSkills();
            }
        }
    }
}
