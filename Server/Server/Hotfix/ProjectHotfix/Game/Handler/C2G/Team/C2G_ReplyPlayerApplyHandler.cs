using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReplyPlayerApplyHandler : AMActorRpcHandler<C2G_ReplyPlayerApply, G2C_ReplyPlayerApply>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReplyPlayerApply b_Request, G2C_ReplyPlayerApply b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ReplyPlayerApply b_Request, G2C_ReplyPlayerApply b_Response, Action<IMessage> b_Reply)
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
            if (component == null || component.TeamID == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1224);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("您不在队伍中，无法同意申请！");
                b_Reply(b_Response);
                return false;
            }

            if (component.IsCaptain == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1225);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("只有队长能处理申请！");
                b_Reply(b_Response);
                return false;
            }

            bool isSend = true;
            Player applyPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.PlayerGameUserId);
            if (applyPlayer == null || applyPlayer.IsDisposeable)
            {
                b_Reply(b_Response);
                return true;
            }

            TeamComponent applyPlayerComponent = applyPlayer.GetCustomComponent<TeamComponent>();
            //判断申请的玩家是不是已经在队伍中
            if (applyPlayerComponent == null || applyPlayerComponent.TeamID == 0)
            {
                if (b_Request.IsAgree)
                {
                    if (!teamManager.AddPlayerInTeam(component.TeamID, applyPlayer))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1226);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("队伍已满，无法添加队员！");
                        b_Reply(b_Response);
                        return false;
                    }
                }
            }
            else
            {
                isSend = false;//之前申请的玩家已经进入别的组队，则不再次提醒申请结果
            }

            if (isSend)
            {
                var notice = new G2C_ApplyToJoinTheTeam_notice()
                {
                    PlayerGameUserId = mPlayer.GameUserId,
                    PlayerName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName,
                    IsAgree = b_Request.IsAgree
                };
                applyPlayer.Send(notice);
            }

            b_Reply(b_Response);
            return true;
        }
    }
}