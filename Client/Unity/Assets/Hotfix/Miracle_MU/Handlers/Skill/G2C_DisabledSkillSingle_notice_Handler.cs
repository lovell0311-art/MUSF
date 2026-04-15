using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_DisabledSkillSingle_notice_Handler : AMHandler<G2C_DisabledSkillSingle_notice>
    {
        protected override void Run(ETModel.Session session, G2C_DisabledSkillSingle_notice message)
        {
            if (UnitEntityComponent.Instance.LocalRole.OwnSkills.Contains(message.SkillId))
            {
                UnitEntityComponent.Instance.LocalRole.OwnSkills.Remove(message.SkillId);
              
                var skillconfiguration = LocalDataJsonComponent.Instance.LoadData<Skillconfiguration>(UnitEntityComponent.Instance.LocalRole.LocalSkillFillName) ?? new Skillconfiguration();

                //判断是否已经装备 该技能
                for (int i = 0; i < skillconfiguration.SKilDic.Count; i++)
                {
                    if (skillconfiguration.SKilDic.ElementAt(i).Value == message.SkillId)
                    {
                        var slot = skillconfiguration.SKilDic.ElementAt(i).Key;

                        if (skillconfiguration.SKilDic.ContainsKey(slot))
                        {
                            skillconfiguration.SKilDic.Remove(slot);
                        }
                    }
                }
                LocalDataJsonComponent.Instance.SavaData(skillconfiguration, UnitEntityComponent.Instance.LocalRole.LocalSkillFillName);//保存 技能 配置信息
                UIMainComponent.Instance.LoadSkills();//更新 主界面的技能信息
            }
        }
    }
}
