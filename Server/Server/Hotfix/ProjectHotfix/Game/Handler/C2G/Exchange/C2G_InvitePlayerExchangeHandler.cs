using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_InvitePlayerExchangeHandler : AMActorRpcHandler<C2G_InvitePlayerExchange, G2C_InvitePlayerExchange>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_InvitePlayerExchange b_Request, G2C_InvitePlayerExchange b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_InvitePlayerExchange b_Request, G2C_InvitePlayerExchange b_Response, Action<IMessage> b_Reply)
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(816);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("交易组件异常!");
                b_Reply(b_Response);
                return false;
            }
            if (exchangeComponent.ExchangeTime > Help_TimeHelper.GetNowSecond())
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(818);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("交易频繁，请稍后再试!");
                b_Reply(b_Response);
                return false;
            }
            //检测自己是否正在交易中
            if (exchangeComponent.ExchangeTargetGameUserId > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(803);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("已在交易中，无法再次邀请交易!");
                b_Reply(b_Response);
                return false;
            }

            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.PlayerGameUserId);
            if (targetPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(802);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标玩家不存在或不在线");
                b_Reply(b_Response);
                return false;
            }

            ExchangeComponent targetExchangeComponent = targetPlayer.GetCustomComponent<ExchangeComponent>();
            if (targetExchangeComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(801);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标玩家交易组件异常!");
                b_Reply(b_Response);
                return false;
            }
            if (targetExchangeComponent.ExchangeTime > Help_TimeHelper.GetNowSecond())
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(819);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("对方交易频繁，请稍后再试!");
                b_Reply(b_Response);
                return false;
            }
            //检测目标是否正在交易中
            if (targetExchangeComponent.ExchangeTargetGameUserId > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(800);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标玩家已在交易中，无法邀请交易!");
                b_Reply(b_Response);
                return false;
            }

            //发送邀请
            targetPlayer.Send(new G2C_InvitePlayerExchange_notice()
            {
                PlayerGameUserId = mPlayer.GameUserId,
                PlayerName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName
            });

            b_Reply(b_Response);
            return true;
        }
    }
}