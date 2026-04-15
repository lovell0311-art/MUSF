using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReplyPlayerInvitationHandler : AMActorRpcHandler<C2G_ReplyPlayerInvitation, G2C_ReplyPlayerInvitation>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReplyPlayerInvitation b_Request, G2C_ReplyPlayerInvitation b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ReplyPlayerInvitation b_Request, G2C_ReplyPlayerInvitation b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            TeamManageComponent teamManager = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
            if (teamManager == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1212);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("找不到组队组件!");
                b_Reply(b_Response);
                return false;
            }

            TeamComponent component = mPlayer.GetCustomComponent<TeamComponent>();
            Player leaderPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.PlayerGameUserId);
            if (b_Request.IsAgree)
            {
                if (component == null || component.TeamID == 0)
                {

                    TeamComponent leaderComponent = leaderPlayer?.GetCustomComponent<TeamComponent>();
                    if (leaderPlayer != null && leaderComponent != null && leaderComponent.TeamID > 0)
                    {
                        //加入队伍
                        if (!teamManager.AddPlayerInTeam(leaderComponent.TeamID, mPlayer))
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1227);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("队伍已满，加入组队失败!");
                        }
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1228);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标玩家不在队伍中，加入组队失败!");
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1229);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("已经在队伍中，加入组队失败!");
                }
            }
            if (!b_Request.IsAuto)
            {
                //通知邀请者结果
                if (leaderPlayer != null)
                {
                    var notice = new G2C_InvitePlayerEnterTeam_notice()
                    {
                        PlayerGameUserId = mPlayer.GameUserId,
                        PlayerName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName,
                        IsAgree = b_Request.IsAgree
                    };
                    leaderPlayer.Send(notice);
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}