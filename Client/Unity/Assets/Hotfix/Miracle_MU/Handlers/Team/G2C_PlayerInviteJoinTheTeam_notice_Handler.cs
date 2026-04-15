using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 通知 其他玩家邀请加入团队
    /// </summary>
    [MessageHandler]
    public class G2C_PlayerInviteJoinTheTeam_notice_Handler : AMHandler<G2C_PlayerInviteJoinTheTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_PlayerInviteJoinTheTeam_notice message)
        {
            if (TeamDatas.IsAutoAcceptTeam)
            {
                ///自动同意加入队伍
                ReplyPlayerInvitation(true).Coroutine();
            }
            else
            {
                UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirmComponent.SetTipText($"<color=red>{message.PlayerName}</color> 邀请你加入队伍");
                uIConfirmComponent.AddActionEvent(() =>
                {
                    //同意 邀请
                    ReplyPlayerInvitation(true).Coroutine();
                });
                uIConfirmComponent.AddCancelEventAction(() =>
                {
                    //拒绝 邀请
                    ReplyPlayerInvitation(false).Coroutine();
                });
            }
            ///同意或拒绝玩家邀请 
            async ETVoid ReplyPlayerInvitation(bool IsAgree)
            {
                G2C_ReplyPlayerInvitation g2C_Reply = (G2C_ReplyPlayerInvitation)await SessionComponent.Instance.Session.Call(new C2G_ReplyPlayerInvitation 
                {
                
                 PlayerGameUserId=message.PlayerGameUserId,
                 IsAgree=IsAgree,
                 IsAuto= TeamDatas.IsAutoAcceptTeam
                });
            }
        }
    }
}