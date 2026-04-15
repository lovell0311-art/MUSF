using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ReMoveExchangeItemHandler : AMActorRpcHandler<C2G_ReMoveExchangeItem, G2C_ReMoveExchangeItem>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ReMoveExchangeItem b_Request, G2C_ReMoveExchangeItem b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_ReMoveExchangeItem b_Request, G2C_ReMoveExchangeItem b_Response, Action<IMessage> b_Reply)
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
                //拿到物品
                BackpackComponent backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
                Item item = backpackComponent.GetItemByUID(b_Request.ItemUUID);
                if (item != null)
                {
                    if (exchangeComponent.RemoveItem(item))
                    {
                        var notice = new G2C_ReMoveExchange_notice();
                        notice.ItemUUID = b_Request.ItemUUID;
                        notice.PlayerGameUserId = mPlayer.GameUserId;

                        //通知双方物品消息
                        mPlayer.Send(notice);
                        targetPlayer.Send(notice);
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(817);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("移除物品失败!物品不存在");
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(813);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包找不到物品!");
                }
            }
            else
            {
                mPlayer.Send(new G2C_ExchangeResult_notice()
                {
                    State = false,
                    ErrorMessage = "对方不在交易中，交易终止!"
                });
                exchangeComponent.EndExchange();
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(814);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("对方不在交易中或交易对象不是自己，交易终止!");
            }

            b_Reply(b_Response);
            return true;
        }
    }
}