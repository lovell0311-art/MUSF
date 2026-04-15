using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SetExchangeGoldHandler : AMActorRpcHandler<C2G_SetExchangeGold, G2C_SetExchangeGold>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SetExchangeGold b_Request, G2C_SetExchangeGold b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SetExchangeGold b_Request, G2C_SetExchangeGold b_Response, Action<IMessage> b_Reply)
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
            //检测自己是否正在交易中
            if (exchangeComponent.ExchangeTargetGameUserId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(809);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不在交易中，无法调用接口!");
                b_Reply(b_Response);
                return false;
            }

            //检测对方是否在交易中，若不在交易或交易对象不是自己则终止交易
            Player targetPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, exchangeComponent.ExchangeTargetGameUserId);
            if (targetPlayer != null && targetPlayer.GetCustomComponent<ExchangeComponent>().ExchangeTargetGameUserId == mPlayer.GameUserId)
            {
                //确认锁定状态
                if (exchangeComponent.IsLock)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(811);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("已锁定，无法操作!");
                    b_Reply(b_Response);
                    return true;
                }
                //设置金钱
                exchangeComponent.ExchangeCoin = b_Request.Gold;
                targetPlayer.Send(new G2C_TargetSetExchangeGold_notice() { 
                    Gold = b_Request.Gold
                });
                
            }
            else
            {
                mPlayer.Send(new G2C_ExchangeResult_notice()
                {
                    State = false,
                    ErrorMessage = "对方不在交易中，交易终止!"
                });
                exchangeComponent.EndExchange();
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(808);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("对方不在交易中或交易对象不是自己，交易终止!");
            }

            b_Reply(b_Response);
            return true;
        }
    }
}