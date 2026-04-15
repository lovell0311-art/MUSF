using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReplyPlayerExchangeInviteHandler : AMActorRpcHandler<C2G_ReplyPlayerExchangeInvite, G2C_ReplyPlayerExchangeInvite>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReplyPlayerExchangeInvite b_Request, G2C_ReplyPlayerExchangeInvite b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ReplyPlayerExchangeInvite b_Request, G2C_ReplyPlayerExchangeInvite b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
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

            ExchangeComponent exchangeComponent = mPlayer.GetCustomComponent<ExchangeComponent>();
            if (exchangeComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(810);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("交易组件异常!");
                b_Reply(b_Response);
                return false;
            }

            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.PlayerGameUserId);
            if (targetPlayer != null)
            {
                //不同意时直接发送拒绝请求，不做判断
                if (!b_Request.IsAgree)
                {
                    targetPlayer.Send(new G2C_InvitePlayerExchangeResult_notice()
                    {
                        IsAgree = b_Request.IsAgree,
                        PlayerGameUserId = mPlayer.GameUserId,
                        PlayerName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName
                    });
                    b_Reply(b_Response);
                    return true;
                }
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(802);
                b_Reply(b_Response);
                return false;
            }
            //检测自己是否正在交易中
            if (exchangeComponent.ExchangeTargetGameUserId > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(803);
                b_Reply(b_Response);
                return false;
            }
            ExchangeComponent targetExchangeComponent = targetPlayer.GetCustomComponent<ExchangeComponent>();
            if (targetExchangeComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(801);
                b_Reply(b_Response);
                return false;
            }

            //检测目标是否正在交易中
            if (targetExchangeComponent.ExchangeTargetGameUserId > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(800);
                b_Reply(b_Response);
                return false;
            }
            GamePlayer mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            PlayerWarAllianceComponent warAllianceComponent = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            targetPlayer.Send(new G2C_InvitePlayerExchangeResult_notice()
            {
                IsAgree = b_Request.IsAgree,
                PlayerGameUserId = mPlayer.GameUserId,
                PlayerName = mGamePlayer.Data.NickName,
                PlayerLevel = mGamePlayer.Data.Level,
                PlayerWarAllianceName = (warAllianceComponent == null)?"": warAllianceComponent.WarAllianceName,
            });

            var targetGamePlayer = targetPlayer.GetCustomComponent<GamePlayer>();
            string targetWarAllianceName = targetGamePlayer.Player.GetCustomComponent<PlayerWarAllianceComponent>()?.WarAllianceName;
            b_Response.PlayerName = targetGamePlayer.Data.NickName;
            b_Response.PlayerLevel = targetGamePlayer.Data.Level;
            b_Response.PlayerWarAllianceName = (targetWarAllianceName == null) ? "" : targetWarAllianceName;

            //开始交易
            exchangeComponent.StartExchange(targetPlayer.GameUserId);
            targetExchangeComponent.StartExchange(mPlayer.GameUserId);

            b_Reply(b_Response);
            return true;
        }
    }
}