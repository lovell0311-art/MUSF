using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;


namespace ETHotfix
{
    /// <summary>
    /// 通知 已经学习的技能列表
    /// </summary>

    [MessageHandler]
    public class G2C_OpenSkillGroup_notice_Handler : AMHandler<G2C_OpenSkillGroup_notice>
    {
        protected override void Run(ETModel.Session session, G2C_OpenSkillGroup_notice message)
        {
          
            UnitEntityComponent.Instance.LocalRole.OwnSkills.Clear();
            foreach (var item in message.SkillIds)//已经学习的技能列表
            {
                if (!UnitEntityComponent.Instance.LocalRole.OwnSkills.Contains(item))
                UnitEntityComponent.Instance.LocalRole.OwnSkills.Add(item);
              
            }
        }
    }
}