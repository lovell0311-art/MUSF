using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 通知 申请被拒绝或同意后推送
    /// </summary>
    [MessageHandler]
    public class G2C_ApplyToJoinTheTeam_notice_Handler : AMHandler<G2C_ApplyToJoinTheTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_ApplyToJoinTheTeam_notice message)
        {
           
              
         UIComponent.Instance.VisibleUI(UIType.UIHint, message.IsAgree?$"<color=red>{message.PlayerName}</color> 同意你加入队伍": $"<color=red>{message.PlayerName}</color> 拒绝你加入队伍");
         
        }
    }
}