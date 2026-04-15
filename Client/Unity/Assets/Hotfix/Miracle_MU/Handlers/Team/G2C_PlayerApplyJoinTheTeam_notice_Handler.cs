using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 通知 其他玩家申请加入团队
    /// </summary>
    [MessageHandler]
    public class G2C_PlayerApplyJoinTheTeam_notice_Handler : AMHandler<G2C_PlayerApplyJoinTheTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_PlayerApplyJoinTheTeam_notice message)
        {

            ApplyPalyerListAdd();

            UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
            uIConfirmComponent.SetTipText($"<color=red>{message.PlayerData.Name}</color> 申请加入队伍");
            uIConfirmComponent.AddActionEvent(() => 
            {   
                //同意入伍
                ReplyPlayerApply(true).Coroutine();
            });
            uIConfirmComponent.AddCancelEventAction(() => 
            {
                //拒绝入伍
                ReplyPlayerApply(false).Coroutine();
            });

            ///既没有点击【同意】也没有点击【拒绝】超过30秒钟，则关闭“组队确认弹窗”同时对方会在屏幕中间弹出悬浮提示“对方无响应”
            TimerComponent.Instance.RegisterTimeCallBack(30000, () => 
            {
                UIComponent.Instance.InVisibilityUI(UIType.UIConfirm);
            });


            ///同意 或拒绝 玩家入队申请
            async ETVoid ReplyPlayerApply(bool IsAgree)
            {
                G2C_ReplyPlayerApply g2C_ReplyPlayer = (G2C_ReplyPlayerApply)await SessionComponent.Instance.Session.Call(new C2G_ReplyPlayerApply
                {
                    PlayerGameUserId = message.PlayerData.GameUserId,
                    IsAgree = IsAgree,
                    IsAuto = false//TODO 设置 自动同意 玩家入队
                });
                if (g2C_ReplyPlayer.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReplyPlayer.Error.GetTipInfo());
                }
                else
                {
                    ApplyPlayersListRemove();
                }
            }


            void ApplyPlayersListRemove() 
            {
                /// 从好友 申请 列表中 移除 该玩家
                if (TeamDatas.ApplyPlayersList.Exists(r => r.roleUUId == message.PlayerData.GameUserId))
                {
                    var member = TeamDatas.ApplyPlayersList.Find(r => r.roleUUId == message.PlayerData.GameUserId);
                    TeamDatas.ApplyPlayersList.Remove(member);
                }
                //组队 面板打开时 
                if (UIComponent.Instance.Get(UIType.UITeam) != null)
                    UIComponent.Instance.Get(UIType.UITeam).GetComponent<UITeamComponent>().ApplyPlayersScrollView.Items = TeamDatas.ApplyPlayersList;
            }

            void ApplyPalyerListAdd() 
            {
                //是否已经 存在
                if (!TeamDatas.ApplyPlayersList.Exists(r => r.roleUUId == message.PlayerData.GameUserId))
                {
                    //新加
                    TeamDatas.ApplyPlayersList.Add(new OtherPlayerInfo
                    {
                        roleUUId = message.PlayerData.GameUserId,
                        roleLev = message.PlayerData.Level,//等级
                        roleName = message.PlayerData.Name,
                        roleType =message.PlayerData.PlayerTypeId,//职业
                        OccupationLevel = message.PlayerData.OccupationLevel,//转职等级
                    });
                    //组队 面板打开时 
                    if (UIComponent.Instance.Get(UIType.UITeam) != null)
                        UIComponent.Instance.Get(UIType.UITeam).GetComponent<UITeamComponent>().ApplyPlayersScrollView.Items = TeamDatas.ApplyPlayersList;
                } 
               
            }
        }
       
    }
}
