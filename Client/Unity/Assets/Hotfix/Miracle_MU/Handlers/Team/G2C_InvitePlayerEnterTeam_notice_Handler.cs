using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 通知 邀请被拒绝或同意后推送
    /// </summary>
    [MessageHandler]
    public class G2C_InvitePlayerEnterTeam_notice_Handler : AMHandler<G2C_InvitePlayerEnterTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_InvitePlayerEnterTeam_notice message)
        {
           if(TeamDatas.OtherTeamMemberStatusList.Count!=5)
            UIComponent.Instance.VisibleUI(UIType.UIHint,message.IsAgree? $"<color=red>{message.PlayerName}</color> 同意加入队伍": $"<color=red>{message.PlayerName}</color> 拒绝加入队伍");

        }
    }
}