using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 大师属性 修改技能攻击距离
    /// </summary>
    [MessageHandler]
    public class G2C_AttackDistance_notice_Handler : AMHandler<G2C_AttackDistance_notice>
    {
        protected override void Run(ETModel.Session session, G2C_AttackDistance_notice message)
        {
            if (UIMainComponent.Instance != null)
            {
                UIMainComponent.Instance.ChangeSkllAttackRange((int)message.AttackType,(int)message.Distance);
            }
        }
    }
}