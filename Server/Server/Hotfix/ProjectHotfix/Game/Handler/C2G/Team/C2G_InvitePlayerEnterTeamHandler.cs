using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_InvitePlayerEnterTeamHandler : AMActorRpcHandler<C2G_InvitePlayerEnterTeam, G2C_InvitePlayerEnterTeam>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_InvitePlayerEnterTeam b_Request, G2C_InvitePlayerEnterTeam b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_InvitePlayerEnterTeam b_Request, G2C_InvitePlayerEnterTeam b_Response, Action<IMessage> b_Reply)
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1206);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("找不到组队组件!");
                b_Reply(b_Response);
                return false;
            }

            TeamComponent component = mPlayer.GetCustomComponent<TeamComponent>();
            if (component == null || component.TeamID == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1204);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不在组队中，无法邀请玩家");
                b_Reply(b_Response);
                return false;
            }

            if (!teamManager.isCanAddMember(component.TeamID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1208);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("队伍已满，无法邀请其他人");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameUserId == b_Request.PlayerGameUserId)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1209);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不能邀请自己入队");
                b_Reply(b_Response);
                return false;
            }
            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.PlayerGameUserId);
            if (targetPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1210);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("被邀请的玩家不存在或已下线");
                b_Reply(b_Response);
                return false;
            }

            var targetTeamComponent = targetPlayer.GetCustomComponent<TeamComponent>();
            if (targetTeamComponent == null || targetTeamComponent.TeamID == 0)
            {
                targetPlayer.Send(new G2C_PlayerInviteJoinTheTeam_notice()
                {
                    PlayerGameUserId = mPlayer.GameUserId,
                    PlayerName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName,
                });
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1211);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("被邀请的玩家已经在别的队伍中");
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}