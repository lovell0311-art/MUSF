using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_LeaveTheTeamHandler : AMActorRpcHandler<C2G_LeaveTheTeam, G2C_LeaveTheTeam>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_LeaveTheTeam b_Request, G2C_LeaveTheTeam b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_LeaveTheTeam b_Request, G2C_LeaveTheTeam b_Response, Action<IMessage> b_Reply)
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1219);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("找不到组队组件!");
                b_Reply(b_Response);
                return false;
            }

            TeamComponent component = mPlayer.GetCustomComponent<TeamComponent>();
            if (component == null || component.TeamID == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1218);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不在队伍中，无法离开队伍!");
                b_Reply(b_Response);
                return true;
            }

            teamManager.LeaveTeam(component.TeamID, mPlayer);

            b_Reply(b_Response);
            return true;
        }
    }
}