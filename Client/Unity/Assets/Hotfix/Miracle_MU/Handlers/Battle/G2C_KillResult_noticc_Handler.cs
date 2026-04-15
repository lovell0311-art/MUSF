using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;

namespace ETHotfix
{  
    /// <summary>
    /// »÷É±¹«¸æ
    /// </summary>
    [MessageHandler]
    public class G2C_KillResult_noticc_Handler : AMHandler<G2C_KillResult_notice>
    {
        protected override void Run(ETModel.Session session, G2C_KillResult_notice message)
        {
            if (UIMainComponent.Instance != null)
            {
                UIMainComponent.Instance.ShowNotice($"<color=yellow>{message.AttackId}</color> »÷°ÜÁË <color=red>{message.BeAttackId}</color>");
            }
        }
    }
}